using System;

namespace ChannelAdvisor.Models
{
  public class Inventory
  {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int QtyGolfio { get; set; }
    public int QtyFBA { get; set; }
    public DateTime Created { get; set; }
  }
}