using System.Security.Authentication;
using ChannelAdvisor.Securities;
using ChannelAdvisor.Utilities;
using MailKit.Net.Smtp;
using MimeKit;

namespace ChannelAdvisor.Models
{
  public class Email : IEmail
  {
    public void EmailToken(GolfioUser golfioUser, string tokenLink, EmailType emailType)
    {
      string EmailSubject;

      EmailSecret emailSecret = new EmailSecret();
      EmailSubject = "Please confirm your email";
      MailboxAddress from = new MailboxAddress("Golfio Admin", emailSecret.emailAddress);
      MailboxAddress to = new MailboxAddress(golfioUser.FullName, golfioUser.Email);

      BodyBuilder bodyBuilder = new BodyBuilder();
      if (emailType == EmailType.EmailConfirmation)
      {
        bodyBuilder.HtmlBody =
          $"<h1>Hello {golfioUser.FullName} </h1> \n\n" +
          "<p>You've recently registered for Project Tracker</p> \n\n" +
          "<p>Please click below to confirm your email address</p> \n\n" +
          $"<a href='{tokenLink}'><button style='color:#fff; background-color:#007bff; border-color:#007bff;'>Confirm</button></a> \n\n" +
          "<p>If the link doesn't work, you can copy and paste the below URL</p> \n\n" +
          $"<p> {tokenLink} </p> \n\n\n" +
          "<p>Thank you!</p>";
      }
      else
      {
        bodyBuilder.HtmlBody =
          $"<h1>Hello {golfioUser.FullName} </h1> \n\n" +
          "<p>You've recently requested for password reset</p> \n\n" +
          "<p>Please click below to reset your password</p> \n\n" +
          $"<a href='{tokenLink}'><button style='color:#fff; background-color:#007bff; border-color:#007bff;'>Confirm</button></a> \n\n" +
          "<p>If the link doesn't work, you can copy and paste the below URL</p> \n\n" +
          $"<p> {tokenLink} </p> \n\n\n" +
          "<p>Thank you!</p>";
      }

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