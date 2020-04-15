using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChannelAdvisor.Models
{
  public class GolfioUser : IdentityUser
  {
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Display(Name = "Office Location")]
    public string OfficeLocation { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;

    [Display(Name = "Full Name")]
    public string FullName
    {
      get {return $"{FirstName} {LastName}"; }
    }
  }
}