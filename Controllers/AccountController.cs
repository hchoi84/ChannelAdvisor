using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChannelAdvisor.Controllers
{
  [AllowAnonymous]
  public class AccountController : Controller
  {
    private readonly IGolfioUser _golfioUser;

    public AccountController(IGolfioUser golfioUser)
    {
      _golfioUser = golfioUser;
    }

    [HttpGet("/login")]
    public IActionResult Login() => View();

    [HttpGet("/register")]
    public IActionResult Register() => View();


    // TODO: Implement email confirmation token
    // TODO: Display message to user after registering
    // TODO: If error from server, display error message (duplicate username)
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      IdentityResult result = await _golfioUser.RegisterAsync(registerViewModel);

      if (result.Succeeded)
      {
        GolfioUser golfioUser = _golfioUser.GetUserInfo(registerViewModel.Email);
        return Json(golfioUser);
      }
      else
      {
        Dictionary<string, string> errors = new Dictionary<string, string>();

        foreach (var error in result.Errors)
        {
          errors.Add(error.Code, error.Description);
        }

        return Json(errors);
      }
    }
  }
}