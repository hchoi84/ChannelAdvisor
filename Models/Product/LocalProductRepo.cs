using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisor.Models;

namespace ChannelAdvisor.Models
{
  public class LocalProductRepo : IProduct
  {
    private readonly IAttribute _attribute;
    private readonly IProductLabel _productLabel;
    private List<Product> _products = new List<Product>();

    public LocalProductRepo(IAttribute attribute, IProductLabel productLabel)
    {
      _attribute = attribute;
      _productLabel = productLabel;
    }

    public async Task<int> AddAsync(Product product, int attributeId)
    {
      product.AttributeId = attributeId;
      _products.Add(product);

      return product.Id;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
      var products = _products;
      foreach (var p in products)
      {
        p.Attribute = await _attribute.GetAttributeAsync(p.AttributeId);
        p.LabelNames = await _productLabel.GetProductLabelNamesAsync(p.Id);
      }
      return _products;
    }

    public async Task<Product> GetProductAsync(int id)
    {
      return _products.FirstOrDefault(p => p.Id == id);
    }
  }
}