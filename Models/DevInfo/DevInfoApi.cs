using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public class DevInfoApi : IDevInfo
  {
    private DevInfo _devInfo;

    public DevInfoApi()
    {
      _devInfo = new DevInfo();
    }

    public string GetAccessToken()
    {
      if (_devInfo.Expire < DateTime.Now || _devInfo.Expire == null)
      {
        string auth = string.Concat(_devInfo.ApplicationId, ":", _devInfo.SharedSecret);
        byte[] authBytes = Encoding.ASCII.GetBytes(auth);
        var encodedAuth = Convert.ToBase64String(authBytes);
        string authorization = string.Concat("Basic ", encodedAuth);

        var request = new HttpRequestMessage
        {
          RequestUri = new Uri(_devInfo.TokenUrl),
          Method = HttpMethod.Post,
          Headers = {
            { HttpRequestHeader.Authorization.ToString(), authorization },
            { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
          },
          Content = new StringContent($"grant_type=refresh_token&refresh_token={_devInfo.RefreshToken}", Encoding.UTF8, "application/json"),
        };

        var client = new HttpClient();
        var response = client.SendAsync(request).Result;
        var content = response.Content;
        var json = content.ReadAsStringAsync().Result;
        var result = JObject.Parse(json);
        _devInfo.AccessToken = result["access_token"].ToString();
        _devInfo.Expire = DateTime.Now.AddSeconds(Convert.ToDouble(result["expires_in"]) - _devInfo.TokenExpireBuffer);
        return result["access_token"].ToString();
      }

      return _devInfo.AccessToken;
    }
  }
}