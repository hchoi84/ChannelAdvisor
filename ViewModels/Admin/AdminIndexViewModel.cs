using System.Collections.Generic;
using System.Security.Claims;

namespace ChannelAdvisor.ViewModels
{
  public class AdminIndexViewModel
  {
    public bool IsSelected { get; set; }
    public string Id { get; set; }
    public string FullName { get; set; }
    public List<Claim> Claims { get; set; }
    public string ClaimSelected { get; set; }
  }
}