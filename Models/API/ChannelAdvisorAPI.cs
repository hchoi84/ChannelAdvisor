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
    private readonly IProduct _product;
    private readonly IAttribute _attribute;
    private readonly ILabel _label;
    private readonly IInventory _inventory;

    public ChannelAdvisorAPI(IProduct product, IAttribute attribute, ILabel label, IInventory inventory)
    {
      _inventory = inventory;
      _label = label;
      _attribute = attribute;
      _product = product;
    }

    public string GetAccessToken()
    {
      if (DevInfo.Expire < DateTime.Now || DevInfo.Expire == null)
      {
        string auth = string.Concat(DevInfo.ApplicationId, ":", DevInfo.SharedSecret);
        byte[] authBytes = Encoding.ASCII.GetBytes(auth);
        var encodedAuth = Convert.ToBase64String(authBytes);
        string authorization = string.Concat("Basic ", encodedAuth);

        var request = new HttpRequestMessage
        {
          RequestUri = new Uri(DevInfo.TokenUrl),
          Method = HttpMethod.Post,
          Headers = {
            { HttpRequestHeader.Authorization.ToString(), authorization },
            { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
          },
          Content = new StringContent($"grant_type=refresh_token&refresh_token={DevInfo.RefreshToken}", Encoding.UTF8, "application/json"),
        };

        var client = new HttpClient();
        var response = client.SendAsync(request).Result;
        var content = response.Content;
        var json = content.ReadAsStringAsync().Result;
        var result = JObject.Parse(json);
        DevInfo.AccessToken = result["access_token"].ToString();
        DevInfo.Expire = DateTime.Now.AddSeconds(Convert.ToDouble(result["expires_in"]) - DevInfo.TokenExpireBuffer);
        return result["access_token"].ToString();
      }

      return DevInfo.AccessToken;
    }

    public async void RetrieveProductsFromAPI()
    {
      var request = new HttpRequestMessage
      {
        RequestUri = new Uri($"https://api.channeladvisor.com/v1/Products?access_token={GetAccessToken()}&$filter=Sku eq 'ANN0385_001' or Sku eq 'ANN0238_001'&$expand=Attributes,Labels,Images,DCQuantities"),
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
        var inventory = await _inventory.AddInventoryAsync((JArray)p["DCQuantities"]);

        var productLabels = (JArray)p["Labels"];
        List<string> productLabelNames = new List<string>();
        foreach (var obj in productLabels)
        {
          productLabelNames.Add((string)obj["Name"]);
        }
        productLabelNames.Sort();
        string joinedLabelNames = String.Join(", ", productLabelNames);

        var productId = await _product.AddAsync(p.ToObject<Product>(), attributeId, inventory, joinedLabelNames);
        
        await _label.AddAsync(productId, joinedLabelNames);
      }
    }
  }
}