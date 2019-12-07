namespace ChannelAdvisor.Models
{
  public class Product
  {
    public int Id { get; set; }
    public string Sku { get; set; }
    public string Brand { get; set; }
    public float RetailPrice { get; set; }
    public float BuyItNowPrice { get; set; }
    public float Cost { get; set; }
  }
}