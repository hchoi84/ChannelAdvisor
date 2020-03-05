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

    public List<Product> AddProducts(JArray products)
    {
      foreach (var p in products)
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
      }

      return _products;
    }

    public List<Product> AddProduct(JObject p)
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

    public List<Product> GetAllProducts() => _products;
  }
}