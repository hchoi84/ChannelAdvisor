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
      List<int> parentProductIDs = new List<int>();
      List<int> distinctParentProductIDs = new List<int>();
      List<string> filterToGetChilds = new List<string>();
      List<string> filterToGetParents = new List<string>();
      string nextUri;
      string filter;
      string filter2;
      string reqUri;
      JObject result;

      reqUri = $"https://api.channeladvisor.com/v1/Products/?access_token={DevInfo.GetAccessToken()}&$filter=LastSaleDateUtc lt 2016-04-01";
      result = await _channelAdvisor.RetrieveProductsFromAPI(reqUri);
      var noSaleProducts = (JArray)result["value"];
      nextUri = (string)result["@odata.nextLink"];

      foreach (JToken product in noSaleProducts)
      {
        parentProductIDs.Add(Convert.ToInt32(product["ParentProductID"]));
      }

      while (nextUri != null)
      {
        result = await _channelAdvisor.RetrieveProductsFromAPI(nextUri);
        noSaleProducts = (JArray)result["value"];
        nextUri = (string)result["@odata.nextLink"];

        foreach (JToken product in noSaleProducts)
        {
          parentProductIDs.Add(Convert.ToInt32(product["ParentProductID"]));
        }
      }

      distinctParentProductIDs = parentProductIDs.Distinct().ToList();

      while(distinctParentProductIDs.Count() > 0)
      {
        int distinctParentProductIDsCount = distinctParentProductIDs.Count() > 10 ? 10 : distinctParentProductIDs.Count();
        for (int i = distinctParentProductIDsCount; i > 0; i--)
        {
          string temp = $"ParentProductID eq {distinctParentProductIDs[0]}";
          string temp2 = $"ID eq {distinctParentProductIDs[0]}";
          filterToGetChilds.Add(temp);
          filterToGetParents.Add(temp2);
          distinctParentProductIDs.RemoveAt(0);
        }

        filter = String.Join(" or ", filterToGetChilds.ToArray());
        filter2 = String.Join(" or ", filterToGetParents.ToArray());
        filterToGetChilds.Clear();
        filterToGetParents.Clear();

        // Get Parents
        reqUri = $"https://api.channeladvisor.com/v1/Products?access_token={DevInfo.GetAccessToken()}&$filter={filter2}&$expand=Attributes,Labels,DCQuantities";
        result = await _channelAdvisor.RetrieveProductsFromAPI(reqUri);
        await _product.AddProductsAsync((JArray)result["value"]);

        // Gets Childs
        reqUri = $"https://api.channeladvisor.com/v1/Products?access_token={DevInfo.GetAccessToken()}&$filter=({filter}) and TotalAvailableQuantity gt 0&$expand=Attributes,Labels,DCQuantities";
        result = await _channelAdvisor.RetrieveProductsFromAPI(reqUri);
        nextUri = (string)result["@odata.nextLink"];
        await _product.AddProductsAsync((JArray)result["value"]);

        while (nextUri != null)
        {
          result = await _channelAdvisor.RetrieveProductsFromAPI(nextUri);
          await _product.AddProductsAsync((JArray)result["value"]);
          nextUri = (string)result["@odata.nextLink"];
        }
      }
      
      return View((await _product.GetAllProductsAsync()).OrderBy(p => p.Sku).ToList());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
