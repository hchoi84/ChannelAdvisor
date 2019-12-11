using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public interface ILabel
  {
    Task<List<Label>> GetProductLabelsAsync(int productId);
    Task<int> GetLabelIdByNameAsync(string labelName);
    Task<Label> GetLabelAsync(int labelId);
  }
}