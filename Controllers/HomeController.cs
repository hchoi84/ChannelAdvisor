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
      ViewBag.AT = accessToken;
      return View();
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
