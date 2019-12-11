using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChannelAdvisor.Models
{
  public interface IProduct
  {
    Task<int> AddAsync(Product product, int attributeId);
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> GetProductAsync(int Id);
  }
}