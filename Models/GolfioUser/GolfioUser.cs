using System;
using Microsoft.AspNetCore.Identity;

namespace ChannelAdvisor.Models
{
  public class GolfioUser : IdentityUser
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OfficeLocation { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;

    public string GetFullName
    {
      get {return $"{FirstName} {LastName}"; }
    }
  }
}