using System.Collections.Generic;

namespace ChannelAdvisor.Models
{
  public interface IChannelAdvisor
  {
    string GetAccessToken();
    void RetrieveProductsFromAPI();
  }
}