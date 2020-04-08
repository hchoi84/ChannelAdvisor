using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using ChannelAdvisor.Securities;
using ChannelAdvisor.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl)
    {
      GolfioUser golfioUser = new GolfioUser();
      bool isValidLogin;
      SignInResult signInResult;

      if (!ModelState.IsValid)
      {
        return View(loginVM);
      }

      golfioUser = await _golfioUser.GetUserInfoAsync(loginVM.Email);

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

    [HttpGet("/register")]
    public IActionResult Register() => View();

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
        GolfioUser golfioUser = await _golfioUser.GetUserInfoAsync(registerViewModel.Email);
        var token = await _golfioUser.CreateEmailConfirmationToken(golfioUser);
        var tokenLink = Url.Action("ConfirmEmail", "Account", new { userId = golfioUser.Id, token = token }, Request.Scheme);
        
        SendEmailConfirmationLink(golfioUser, tokenLink);

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

    [HttpGet("/emailconfirmation")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      if (userId == null || token == null)
      {
        TempData["MessageTitle"] = "Error";
        TempData["Message"] = "The email confirmation token link is invalid";
        
        return RedirectToAction("Login");
      }

      GolfioUser golfioUser = await _golfioUser.GetUserInfoAsync(userId);

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

    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
      await _golfioUser.SignOutUserAsync();
      HttpContext.Session.Clear();

      TempData["MessageTitle"] = "Logout Successful";
      TempData["Message"] = "Have a nice day! :)";

      return RedirectToAction("Login");
    }

    public void SendEmailConfirmationLink(GolfioUser golfioUser, string tokenLink)
    {
      string EmailSubject;

      EmailSecret emailSecret = new EmailSecret();
      EmailSubject = "Please confirm your email";
      MailboxAddress from = new MailboxAddress("Golfio Admin", emailSecret.emailAddress);
      MailboxAddress to = new MailboxAddress(golfioUser.GetFullName, golfioUser.Email);

      BodyBuilder bodyBuilder = new BodyBuilder();
      bodyBuilder.TextBody = tokenLink;

      MimeMessage message = new MimeMessage();
      message.From.Add(from);
      message.To.Add(to);
      message.Subject = EmailSubject;
      message.Body = bodyBuilder.ToMessageBody();

      using (SmtpClient client = new SmtpClient())
      {
        client.SslProtocols = SslProtocols.Tls;
        client.Connect(emailSecret.smtpServerAddress, emailSecret.port, emailSecret.useSSL);
        client.Authenticate(emailSecret.emailAddress, emailSecret.apiPassword);
        client.Send(message);
        client.Disconnect(true);
      }
    }
  }
}