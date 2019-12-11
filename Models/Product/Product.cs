using System;
using System.Collections.Generic;

namespace ChannelAdvisor.Models
{
  public class Product
  {
    public int Id { get; set; }
    public int AttributeId { get; set; }
    public DateTime CreateDateUtc { get; set; }
    public DateTime UpdateDateUtc { get; set; }
    public DateTime QuantityUpdateDateUtc { get; set; }
    public DateTime? LastSaleDateUtc { get; set; }
    public string ASIN { get; set; } = null;
    public string Brand { get; set; }
    public string Condition { get; set; } = null;
    public string Description { get; set; } = null;
    public string MPN { get; set; } = null;
    public string Sku { get; set; }
    public string Title { get; set; }
    public string UPC { get; set; }
    public string WarehouseLocation { get; set; }
    public int Height { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Weight { get; set; }
    public float Cost { get; set; }
    public float RetailPrice { get; set; }
    public float BuyItNowPrice { get; set; }
    public bool IsParent { get; set; }
    public int? ParentProductID { get; set; }
    public string RelationshipName { get; set; }

    public Attribute Attribute { get; set; }
    public string LabelNames { get; set; }
    
    public int QtyFBA { get; set; }
    public int QtyGolfio { get; set; }
  }
}