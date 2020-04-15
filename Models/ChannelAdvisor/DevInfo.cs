using System;
using System.Net;
using System.Net.Http;
using System.Text;
using ChannelAdvisor.Securities;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public static class DevInfo
  {
    public static string GetAccessToken()
    {
      ChannelAdvisorSecret caSecret = new ChannelAdvisorSecret();

      if (caSecret.Expire < DateTime.Now || caSecret.Expire == null)
      {
        string auth = string.Concat(caSecret.ApplicationId, ":", caSecret.SharedSecret);
        byte[] authBytes = Encoding.ASCII.GetBytes(auth);
        var encodedAuth = Convert.ToBase64String(authBytes);
        string authorization = string.Concat("Basic ", encodedAuth);

        var request = new HttpRequestMessage
        {
          RequestUri = new Uri(caSecret.TokenUrl),
          Method = HttpMethod.Post,
          Headers = {
            { HttpRequestHeader.Authorization.ToString(), authorization },
            { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
          },
          Content = new StringContent($"grant_type=refresh_token&refresh_token={caSecret.RefreshToken}", Encoding.UTF8, "application/json"),
        };

        var client = new HttpClient();
        var response = client.SendAsync(request).Result;
        var content = response.Content;
        var json = content.ReadAsStringAsync().Result;
        var result = JObject.Parse(json);
        caSecret.AccessToken = result["access_token"].ToString();
        caSecret.Expire = DateTime.Now.AddSeconds(Convert.ToDouble(result["expires_in"]) - caSecret.TokenExpireBuffer);
        return result["access_token"].ToString();
      }

      return caSecret.AccessToken;
    }
  }
}