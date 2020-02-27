using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public interface IProduct
  {
    Task<List<Product>> AddProductsAsync(JArray products);
    Task<List<Product>> AddProductAsync(JObject product);
    Task<List<Product>> GetAllProductsAsync();
    // Task<Product> GetProductAsync(int Id);
  }
}