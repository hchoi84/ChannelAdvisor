using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChannelAdvisor.Models
{
  public interface IProduct
  {
    Task<int> AddAsync(Product product, int attributeId, Inventory inventory, string joinedLabelNames);
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> GetProductAsync(int Id);
  }
}