using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisor.Models;

namespace ChannelAdvisor.Models
{
  public class LocalProductRepo : IProduct
  {
    private readonly IAttribute _attribute;
    private List<Product> _products = new List<Product>();

    public LocalProductRepo(IAttribute attribute)
    {
      _attribute = attribute;
    }

    public async Task<int> AddAsync(Product product, int attributeId, Inventory inventory, string joinedLabelNames)
    {
      Product productInDB = _products.FirstOrDefault(p => p.Id == product.Id) ;
      if (productInDB != null)
      {
        _products.Remove(productInDB);
      }

      product.AttributeId = attributeId;
      product.LabelNames = joinedLabelNames;
      product.QtyFBA = inventory.QtyFBA;
      product.QtyWH = inventory.QtyWH;
      _products.Add(product);

      return product.Id;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
      var products = _products;
      foreach (var p in products)
      {
        p.Attribute = await _attribute.GetAttributeAsync(p.AttributeId);
      }
      return _products;
    }

    public async Task<Product> GetProductAsync(int id)
    {
      return _products.FirstOrDefault(p => p.Id == id);
    }
  }
}