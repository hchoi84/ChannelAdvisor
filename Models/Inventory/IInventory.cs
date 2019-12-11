using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public interface IInventory
  {
    Task<Inventory> AddInventoryAsync(JArray inventory);
  }
}