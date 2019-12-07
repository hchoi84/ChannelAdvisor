using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChannelAdvisor.Models;
using System.Text;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IDevInfo _devInfo;

    public HomeController(ILogger<HomeController> logger, IDevInfo devInfo)
    {
      _logger = logger;
      _devInfo = devInfo;
    }

    public IActionResult Index()
    {
      string accessToken = _devInfo.GetAccessToken();

      // var request = new HttpRequestMessage
      // {
      //   RequestUri = new Uri($"https://api.channeladvisor.com/v1/Products?$filter=Sku eq 'TAE0064_30' or Sku eq 'TAE0064_29' or Sku eq 'TAE0064'&access_token={accessToken}"),
      //   Method = HttpMethod.Get,
      // };

      var request = new HttpRequestMessage
      {
        RequestUri = new Uri($"https://api.channeladvisor.com/v1/Products(3892728)/Attributes?access_token={accessToken}"),
        Method = HttpMethod.Get,
      };

      var client = new HttpClient();
      var response = client.SendAsync(request).Result;
      var content = response.Content;
      var json = content.ReadAsStringAsync().Result;
      var result = JObject.Parse(json);
      JArray a = (JArray)result["value"];
      List<Product> products = a.ToObject<List<Product>>();

      // return View(products);
      return Json(result);
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
