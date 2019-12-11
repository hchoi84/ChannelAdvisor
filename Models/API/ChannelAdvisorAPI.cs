using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public class ChannelAdvisorAPI : IChannelAdvisor
  {
    private DevInfo _devInfo;
    private readonly IProduct _product;
    private readonly IAttribute _attribute;
    private readonly ILabel _label;
    private readonly IProductLabel _productLabel;

    public ChannelAdvisorAPI(IProduct product, IAttribute attribute, ILabel label, IProductLabel productLabel)
    {
      _productLabel = productLabel;
      _label = label;
      _attribute = attribute;
      _product = product;
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

    public async void RetrieveProductsFromAPI()
    {
      var request = new HttpRequestMessage
      {
        RequestUri = new Uri($"https://api.channeladvisor.com/v1/Products?access_token={GetAccessToken()}&$filter=Sku eq 'ANN0385_001' or Sku eq 'TAE0064'&$expand=Attributes,Labels,Images,DCQuantities"),
        Method = HttpMethod.Get,
      };

      var client = new HttpClient();
      var response = client.SendAsync(request).Result;
      var content = response.Content;
      var json = content.ReadAsStringAsync().Result;
      var result = JObject.Parse(json);

      JArray productArray = (JArray)result["value"];
      foreach (var p in productArray)
      {
        var attributeId = await _attribute.AddAsync((JArray)p["Attributes"]);
        var productId = await _product.AddAsync(p.ToObject<Product>(), attributeId);

        foreach (var label in (JArray)p["Labels"])
        {
          var labelId = await _label.GetLabelIdByNameAsync(label["Name"].ToString());
          await _productLabel.AddAsync(productId, labelId);
        }
      }
    }
  }
}