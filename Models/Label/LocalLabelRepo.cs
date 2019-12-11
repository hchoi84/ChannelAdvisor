using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public class LocalLabelRepo : ILabel
  {
    private List<Label> _labels = new List<Label>();

    public async Task<List<Label>> GetProductLabelsAsync(int productId)
    {
      throw new NotImplementedException();      
    }

    public async Task<Label> GetLabelAsync(int labelId)
    {
      return _labels.FirstOrDefault(l => l.Id == labelId);
    }

    public Task<int> AddSync(string label)
    {
      throw new NotImplementedException();
    }

    public async Task<Label> GetLabelByNameAsync(string labelName)
    {
      var label = _labels.FirstOrDefault(l => l.Name == labelName);
      if (label == null)
      {
        Label newLabel = new Label()
        {
          Id = _labels.Any() ? _labels.Max(l => l.Id) + 1 : 1,
          Name = labelName
        };

        _labels.Add(newLabel);
        return newLabel;
      }

      return label;
    }
  }
}