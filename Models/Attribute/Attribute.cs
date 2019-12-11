using System;

namespace ChannelAdvisor.Models
{
  public class Attribute
  {
    public int Id { get; set; }
    public string AllName { get; set; }
    public float AMZPrice { get; set; }
    public string BCCategoryTree { get; set; }
    public float BCPrice { get; set; }
    public string Color { get; set; }
    public string DetailedCategory { get; set; }
    public string Gender { get; set; }
    public string InvFlag { get; set; }
    public string ItemName { get; set; }
    public string Qty { get; set; }
    public float ShippingFee { get; set; }
    public string SpecialOffers { get; set; }
    public string Sync { get; set; }
    public DateTime Updated { get; set; }
    public int Year { get; set; }
  }
}