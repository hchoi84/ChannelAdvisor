using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public class LocalProductRepo : IProduct
  {
    private List<Product> _products = new List<Product>();
    private List<string> _labels = new List<string>()
    {
      "NPIP", "MAPShow", "MAPNoShow", "Closeout", "Discount"
    };

    public LocalProductRepo()
    {
    }

    public async Task<List<Product>> AddProductsAsync(JArray products)
    {
      foreach (var p in products)
      {
        Product product = p.ToObject<Product>();

        var attributes = (JArray)p["Attributes"];
        product.AllName = attributes.FirstOrDefault(prod => prod["Name"].ToString() == "All Name")["Value"].ToString();

        var productLabels = (JArray)p["Labels"];
        // List<string> productLabelNames = new List<string>();
        foreach (var obj in productLabels)
        {
          if (_labels.Contains((string)obj["Name"]))
          {
            // productLabelNames.Add((string)obj["Name"]);
            product.LabelNames = (string)obj["Name"];
            break;
          }
        }
        // productLabelNames.Sort();
        // string joinedLabelNames = string.Join(", ", productLabelNames);
        // product.LabelNames = joinedLabelNames;

        var quantities = (JArray)p["DCQuantities"];
        
        var qtyFBA = quantities.FirstOrDefault(q => Convert.ToInt32(q["DistributionCenterID"]) == -4);
        product.QtyFBA = qtyFBA != null ? (int)qtyFBA["AvailableQuantity"] : 0;
        product.QtyWH = Convert.ToInt32(quantities.FirstOrDefault(q => Convert.ToInt32(q["DistributionCenterID"]) == 0)["AvailableQuantity"]);

        _products.Add(product);
      }

      return _products;

      // Product productInDB = _products.FirstOrDefault(p => p.Id == product.Id) ;
      // if (productInDB != null)
      // {
      //   _products.Remove(productInDB);
      // }

      // product.LabelNames = joinedLabelNames;
      // _products.Add(product);

      // return product.Id;
    }

    public async Task<List<Product>> AddProductAsync(JObject p)
    {
      Product product = p.ToObject<Product>();

      var attributes = (JArray)p["Attributes"];
      product.AllName = attributes.FirstOrDefault(prod => prod["Name"].ToString() == "All Name")["Value"].ToString();

      var productLabels = (JArray)p["Labels"];
      foreach (var obj in productLabels)
      {
        if (_labels.Contains((string)obj["Name"]))
        {
          product.LabelNames = (string)obj["Name"];
          break;
        }
      }

      var quantities = (JArray)p["DCQuantities"];
      
      var qtyFBA = quantities.FirstOrDefault(q => Convert.ToInt32(q["DistributionCenterID"]) == -4);
      product.QtyFBA = qtyFBA != null ? (int)qtyFBA["AvailableQuantity"] : 0;
      product.QtyWH = Convert.ToInt32(quantities.FirstOrDefault(q => Convert.ToInt32(q["DistributionCenterID"]) == 0)["AvailableQuantity"]);

      _products.Add(product);

      return _products;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
      // var products = _products;
      // foreach (var p in products)
      // {
      //   p.Attribute = await _attribute.GetAttributeAsync(p.AttributeId);
      // }
      return _products;
    }

    // public async Task<Product> GetProductAsync(int id)
    // {
    //   return _products.FirstOrDefault(p => p.Id == id);
    // }
  }
}