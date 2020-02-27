using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public interface IChannelAdvisor
  {
    Task<JObject> RetrieveProductsFromAPI(string reqUri);
  }
}