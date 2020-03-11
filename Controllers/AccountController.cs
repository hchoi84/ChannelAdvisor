using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChannelAdvisor.Controllers
{
  [AllowAnonymous]
  public class AccountController : Controller
  { 
    [HttpGet("/login")]
    public IActionResult Login() => View();

    [HttpGet("/register")]
    public IActionResult Register() => View();

    // TODO: Confirm model validation
    // TODO: Implement email confirmation token
    // TODO: Create User in DB
    [HttpPost("/register")]
    public IActionResult Register(RegisterViewModel registerViewModel)
    {
      return Json(registerViewModel);
    }
  }
}