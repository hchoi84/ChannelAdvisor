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

    public ChannelAdvisorAPI(IProduct product)
    {
      _product = product;
    }

    public JObject RetrieveProductsFromAPI(string reqUri)
    {
      var request = new HttpRequestMessage
      {
        RequestUri = new Uri(reqUri),
        Method = HttpMethod.Get,
      };

      var client = new HttpClient();
      var response = client.SendAsync(request).Result;
      var content = response.Content;
      var json = content.ReadAsStringAsync().Result;
      return JObject.Parse(json);
    }
  }
}