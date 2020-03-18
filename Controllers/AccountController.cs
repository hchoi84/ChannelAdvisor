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

    // TODO: If error from server, display error message (ex: duplicate username)
    // TODO: Create message view
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
        var token = await _golfioUser.CreateEmailConfirmationToken(golfioUser);
        var tokenLink = Url.Action("ConfirmEmail", "Account", new { userId = golfioUser.Id, token = token }, Request.Scheme);
        
        EmailClient.SendEmailConfirmationLink(golfioUser, tokenLink);

        return Json($"Hello {golfioUser.GetFullName}. Your confirmation email has been sent");
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


    [HttpGet("/emailconfirmation")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      if (userId == null || token == null)
      {
        return RedirectToAction("Index", "Home");
      }

      GolfioUser golfioUser = _golfioUser.GetUserInfo(userId);

      if (golfioUser == null)
      {
        ViewBag.errorMessage = "The link is invalid";
        return View("Error");
      }

      IdentityResult result = await _golfioUser.ConfirmEmailTokenAsync(golfioUser, token);

      if (result.Succeeded)
      {
        return Json("Success");
      }
      else
      {
        return Json("Failed");
      }
    }
  }
}