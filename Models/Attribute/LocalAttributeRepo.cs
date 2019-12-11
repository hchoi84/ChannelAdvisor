using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public class LocalAttributeRepo : IAttribute
  {
    List<Attribute> Attributes = new List<Attribute>();

    public async Task<int> AddAsync(JArray attributes)
    {
      Attribute att = new Attribute();
      att.Id = Attributes.Any() ? Attributes.Max(x => x.Id) + 1 : 1;
      att.AllName = GetValue(attributes, "All Name");
      att.AMZPrice = float.Parse(GetValue(attributes, "AMZPrice"));
      att.BCCategoryTree = GetValue(attributes, "bc category tree");
      att.BCPrice = float.Parse(GetValue(attributes, "BCPrice"));
      att.Color = GetValue(attributes, "Color");
      att.DetailedCategory = GetValue(attributes, "detailed category");
      att.Gender = GetValue(attributes, "gender");
      att.InvFlag = GetValue(attributes, "invflag");
      att.ItemName = GetValue(attributes, "Item Name");
      att.Qty = GetValue(attributes, "qty");
      att.ShippingFee = float.Parse(GetValue(attributes, "Shipping Fee"));
      att.SpecialOffers = GetValue(attributes, "Special Offers");
      att.Sync = GetValue(attributes, "sync");
      att.Updated = Convert.ToDateTime(GetValue(attributes, "updated"));
      att.Year = Convert.ToInt32(GetValue(attributes, "Year"));

      Attributes.Add(att);
      return att.Id;
    }

    public async Task<List<Attribute>> GetAllAttributesAsync()
    {
      return Attributes;
    }

    public async Task<Attribute> GetAttributeAsync(int attributeId)
    {
      return Attributes.FirstOrDefault(a => a.Id == attributeId);
    }

    public string GetValue(JArray attributes, string attributeName)
    {
      var obj = attributes.FirstOrDefault(a => (string)a["Name"] == attributeName);
      if (obj != null)
      {
        return (string)obj["Value"];
      }

      return null;
    }

  }
}