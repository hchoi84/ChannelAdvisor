using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using ChannelAdvisor.Securities;
using ChannelAdvisor.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

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
    public IActionResult Register()
    {
      return View();
    }

    // TODO: If error from server, display error message (ex: duplicate username)
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