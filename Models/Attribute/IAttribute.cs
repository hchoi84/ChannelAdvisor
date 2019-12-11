using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public interface IAttribute
  {
    Task<int> AddAsync(JArray attributes);
    Task<List<Attribute>> GetAllAttributesAsync();
    Task<Attribute> GetAttributeAsync(int ProductId);
  }
}