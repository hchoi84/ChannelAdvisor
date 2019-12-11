using System;

namespace ChannelAdvisor.Models
{
  public class Label
  {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Labels { get; set; }
    public DateTime Created { get; set; }
  }
}