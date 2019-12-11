using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChannelAdvisor.Models
{
  public class LocalProductLabelRepo : IProductLabel
  {
    private readonly ILabel _label;
    private List<ProductLabel> _productLabels = new List<ProductLabel>();

    public LocalProductLabelRepo(ILabel label)
    {
      _label = label;
    }

    public async Task<ProductLabel> AddAsync(int productId, int labelId)
    {
      var productLabel = new ProductLabel();
      productLabel.Id = _productLabels.Any() ? _productLabels.Max(pl => pl.Id) + 1 : 1;
      productLabel.ProductId = productId;
      productLabel.LabelId = labelId;

      _productLabels.Add(productLabel);
      return productLabel;
    }

    public async Task<string> GetProductLabelNamesAsync(int productId)
    {
      List<ProductLabel> productLabels = _productLabels.Where(pl => pl.ProductId == productId).ToList();
      foreach (var pl in productLabels)
      {
        pl.Label = await _label.GetLabelAsync(pl.LabelId);
      }

      List<string> labelNames = new List<string>();
      List<ProductLabel> sortedByName = productLabels.OrderBy(pl => pl.Label.Name).ToList();
      foreach (var pl in sortedByName)
      {
        labelNames.Add(pl.Label.Name);
      }

      string result = string.Join(", ",labelNames);

      return result;
    }
  }
}