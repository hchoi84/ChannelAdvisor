using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using ChannelAdvisor.Utilities;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ChannelAdvisor.Controllers
{
  [AllowAnonymous]
  public class AccountController : Controller
  {
    private readonly IGolfioUser _golfioUser;
    private readonly IEmail _email;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IGolfioUser golfioUser, IEmail email, ILogger<AccountController> logger)
    {
      _golfioUser = golfioUser;
      _email = email;
      _logger = logger;
    }

    [HttpGet("/Login")]
    public IActionResult Login() => View();

    [HttpPost("/Login")]
    public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl)
    {
      GolfioUser golfioUser = new GolfioUser();
      bool isValidLogin;
      SignInResult signInResult;

      if (!ModelState.IsValid)
      {
        return View(loginVM);
      }

      golfioUser = await _golfioUser.GetUserAsync(loginVM.Email);

      if (golfioUser == null)
      {
        TempData["MessageTitle"] = "Login Failed";
        TempData["Message"] = "Invalid Login Attempt";
        return View(loginVM);
      }

      isValidLogin = await _golfioUser.IsValidLoginAsync(golfioUser, loginVM.Password);

      if (!isValidLogin)
      {
        TempData["MessageTitle"] = "Login Failed";
        TempData["Message"] = "Invalid Login Attempt";
        return View(loginVM);
      }

      signInResult = await _golfioUser.SignInUserAsync(loginVM);

      if (!signInResult.Succeeded)
      {
        TempData["MessageTitle"] = "Login Failed";
        TempData["Message"] = "Invalid Login Attempt";
        return View(loginVM);
      }

      HttpContext.Session.SetString("FullName", golfioUser.GetFullName);

      if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction("Index", "Home");
      }
    }

    [HttpGet("/Register")]
    public IActionResult Register() => View();

    [HttpPost("/Register")]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      IdentityResult result = await _golfioUser.RegisterAsync(registerViewModel);

      if (result.Succeeded)
      {
        GolfioUser golfioUser = await _golfioUser.GetUserAsync(registerViewModel.Email);
        var token = await _golfioUser.CreateEmailConfirmationToken(golfioUser);
        var tokenLink = Url.Action("ConfirmEmail", "Account", new { userId = golfioUser.Id, token = token }, Request.Scheme);

        // _email.EmailToken(golfioUser, tokenLink, EmailType.EmailConfirmation);
        _logger.Log(LogLevel.Warning, tokenLink);

        TempData["MessageTitle"] = "Registration Success";
        TempData["Message"] = "Please check your email for confirmation link";

        return RedirectToAction("Login");
      }
      else
      {
        Dictionary<string, string> errors = new Dictionary<string, string>();

        foreach (var error in result.Errors)
        {
          errors.Add(error.Code, error.Description);
        }

        ViewBag.Errors = errors;
        // return Json(errors);
        return View();
      }
    }

    [HttpGet("/EmailConfirmation")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      if (userId == null || token == null)
      {
        TempData["MessageTitle"] = "Error";
        TempData["Message"] = "The email confirmation token link is invalid";
        
        return RedirectToAction("Login");
      }

      GolfioUser golfioUser = await _golfioUser.GetUserAsync(userId);

      if (golfioUser == null)
      {
        TempData["MessageTitle"] = "Error";
        TempData["Message"] = "No user found";

        return RedirectToAction("Login");
      }

      IdentityResult result = await _golfioUser.ConfirmEmailTokenAsync(golfioUser, token);

      if (result.Succeeded)
      {
        TempData["MessageTitle"] = "Email Confirmed";
        TempData["Message"] = "You may now login";

        return RedirectToAction("Login");
      }
      else
      {
        TempData["MessageTitle"] = "Error";
        TempData["Message"] = "Something went wrong while confirming the email token";

        return RedirectToAction("Login");
      }
    }

    [HttpPost("/Logout")]
    public async Task<IActionResult> Logout()
    {
      await _golfioUser.SignOutUserAsync();
      HttpContext.Session.Clear();

      TempData["MessageTitle"] = "Logout Successful";
      TempData["Message"] = "Have a nice day! :)";

      return RedirectToAction("Login");
    }

    [HttpGet("/ForgotPassword")]
    public IActionResult ForgotPassword() => View();

    [HttpPost("/ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      GolfioUser golfioUser = await _golfioUser.FindByEmailAsync(forgotPasswordVM.Email);

      if (golfioUser == null)
      {
        TempData["MessageTitle"] = "Invalid Email";
        TempData["Message"] = "Please ensure the email is correct and you've already registered";
        return RedirectToAction("Login");
      }

      if (!golfioUser.EmailConfirmed)
      {
        TempData["MessageTitle"] = "Email Not Confirmed";
        TempData["Message"] = "You have not confirmed your email yet";
        return RedirectToAction("Login");
      }

      string token = await _golfioUser.GeneratePasswordResetTokenAsync(golfioUser);

      var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = forgotPasswordVM.Email, token = token }, Request.Scheme);

      _email.EmailToken(golfioUser, passwordResetLink, EmailType.PasswordReset);

      TempData["MessageTitle"] = "Email Sent";
      TempData["Message"] = "Please check your email for password reset link";
      
      return RedirectToAction("Login");
    }

    [HttpGet("/ResetPassword")]
    public IActionResult ResetPassword(string email, string token)
    {
      if (token == null || email == null)
      {
        TempData["MessageTitle"] = "Invalid Token";
        TempData["Message"] = "Invalid password reset token";
        return RedirectToAction("Login");
      }

      return View();
    }

    [HttpPost("/ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordVM)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      IdentityResult identityResult = await _golfioUser.ResetPasswordAsync(resetPasswordVM);

      if (identityResult.Errors.Any())
      {
        List<string> errorMessages = new List<string>();
        string errMsg;
        
        foreach (var error in identityResult.Errors)
        {
          errorMessages.Add(error.Description);
        }

        errMsg = String.Join(", ", errorMessages);

        TempData["MessageTitle"] = "Error";
        TempData["Message"] = $"{errMsg}";
        
        return RedirectToAction("Login");
      }

      TempData["MessageTitle"] = "Success";
      TempData["Message"] = "Password has been reset successfully";

      return RedirectToAction("Login");
    }
  }
}