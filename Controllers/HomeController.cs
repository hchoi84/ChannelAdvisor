using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChannelAdvisor.Models;
using System.Threading.Tasks;

namespace ChannelAdvisor.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IChannelAdvisor _channelAdvisor;
    private readonly IProduct _product;
    private readonly IAttribute _attribute;

    public HomeController(ILogger<HomeController> logger, IChannelAdvisor channelAdvisor, IProduct product, IAttribute attribute)
    {
      _attribute = attribute;
      _product = product;
      _logger = logger;
      _channelAdvisor = channelAdvisor;
    }

    public async Task<IActionResult> Index()
    {
      SyncProducts();
      var products = _product.GetAllProductsAsync();

      return Json(products);
    }

    public IActionResult SyncProducts()
    {
      _channelAdvisor.RetrieveProductsFromAPI();

      return RedirectToAction("Index");
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
