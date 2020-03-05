using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public interface IProduct
  {
    List<Product> AddProducts(JArray products);
    List<Product> GetAllProducts();
  }
}