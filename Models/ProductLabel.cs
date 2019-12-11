using System.Collections.Generic;

namespace ChannelAdvisor.Models
{
  public class ProductLabel
  {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int LabelId { get; set; }

    public Product Product { get; set; }
    public Label Label { get; set; }
  }
}