using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public interface ILabel
  {
    Task<Label> AddAsync(int productId, string joinedLabelName);
    Task<Label> GetLabelByNameAsync(string labelName);
    Task<Label> GetLabelAsync(int labelId);
  }
}