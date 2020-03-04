using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChannelAdvisor.Models;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ChannelAdvisor.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IChannelAdvisor _channelAdvisor;
    private readonly IProduct _product;

    public HomeController(ILogger<HomeController> logger, IChannelAdvisor channelAdvisor, IProduct product)
    {
      _product = product;
      _logger = logger;
      _channelAdvisor = channelAdvisor;
    }

    public IActionResult Index() => View();
    
    [HttpPost]
    public async Task<IActionResult> NoSales()
    {
      List<int> distinctParentProductIDs = new List<int>();
      List<string> filterToGetChilds = new List<string>();
      List<string> filterToGetParents = new List<string>();

      distinctParentProductIDs = await GetDistinctParentIDs();

      while(distinctParentProductIDs.Count() > 0)
      {
        int distinctParentProductIDsCount = distinctParentProductIDs.Count() > 10 ? 10 : distinctParentProductIDs.Count();
        for (int i = distinctParentProductIDsCount; i > 0; i--)
        {
          filterToGetChilds.Add($"ParentProductID eq {distinctParentProductIDs[0]}");
          filterToGetParents.Add($"ID eq {distinctParentProductIDs[0]}");
          distinctParentProductIDs.RemoveAt(0);
        }

        GetAndStoreProductInfo(filterToGetChilds, filterToGetParents);
      }
      
      return View((await _product.GetAllProductsAsync()).OrderBy(p => p.Sku).ToList());
    }

    public async Task<List<int>> GetDistinctParentIDs()
    {
      List<int> parentProductIDs = new List<int>();
      string reqUri, nextUri;
      JObject result;

      reqUri = $"https://api.channeladvisor.com/v1/Products/?access_token={DevInfo.GetAccessToken()}&$filter=LastSaleDateUtc lt 2016-04-01";
      result = await Task.Run(() => _channelAdvisor.RetrieveProductsFromAPI(reqUri));

      var noSaleProducts = (JArray)result["value"];
      nextUri = (string)result["@odata.nextLink"];

      foreach (JToken product in noSaleProducts)
      {
        parentProductIDs.Add(Convert.ToInt32(product["ParentProductID"]));
      }

      while (nextUri != null)
      {
        result = await Task.Run(() => _channelAdvisor.RetrieveProductsFromAPI(nextUri));
        noSaleProducts = (JArray)result["value"];
        nextUri = (string)result["@odata.nextLink"];

        foreach (JToken product in noSaleProducts)
        {
          parentProductIDs.Add(Convert.ToInt32(product["ParentProductID"]));
        }
      }

      return parentProductIDs.Distinct().ToList();
    }
    
    public void GetAndStoreProductInfo(List<string> filterToGetChilds, List<string> filterToGetParents)
    {
      List<Task> tasks = new List<Task>();
      string reqUri;
      string nextUri;
      string filterToGetChild;
      string filterToGetParent;
      JObject result;

      filterToGetChild = String.Join(" or ", filterToGetChilds.ToArray());
      filterToGetParent = String.Join(" or ", filterToGetParents.ToArray());
      filterToGetChilds.Clear();
      filterToGetParents.Clear();

      // Get Parents
      tasks.Add(Task.Run(async () => {
        reqUri = $"https://api.channeladvisor.com/v1/Products?access_token={DevInfo.GetAccessToken()}&$filter={filterToGetParent}&$expand=Attributes,Labels,DCQuantities";
        result = _channelAdvisor.RetrieveProductsFromAPI(reqUri);
        await _product.AddProductsAsync((JArray)result["value"]);
      }));

      // Get Childs
      tasks.Add(Task.Run(async () => {
        reqUri = $"https://api.channeladvisor.com/v1/Products?access_token={DevInfo.GetAccessToken()}&$filter=({filterToGetChild}) and TotalAvailableQuantity gt 0&$expand=Attributes,Labels,DCQuantities";
        result = await Task.Run(() => _channelAdvisor.RetrieveProductsFromAPI(reqUri));
        await _product.AddProductsAsync((JArray)result["value"]);
        nextUri = (string)result["@odata.nextLink"];

        while (nextUri != null)
        {
          result = await Task.Run(() => _channelAdvisor.RetrieveProductsFromAPI(nextUri));
          await _product.AddProductsAsync((JArray)result["value"]);
          nextUri = (string)result["@odata.nextLink"];
        }
      }));

      Task.WaitAll(tasks.ToArray());
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
