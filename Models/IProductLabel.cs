using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChannelAdvisor.Models
{
    public interface IProductLabel
    {
      Task<ProductLabel> AddAsync(int productId, int labelId);
      Task<string> GetProductLabelNamesAsync(int productId);
    }
}