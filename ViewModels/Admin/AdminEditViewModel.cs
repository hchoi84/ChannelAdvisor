using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ChannelAdvisor.Models;
using ChannelAdvisor.Utilities;

namespace ChannelAdvisor.ViewModels
{
  public class AdminEditViewModel
  {
    public string UserId { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    public List<ClaimInfo> ClaimInfos { get; set; }

    [Display(Name = "Old Password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "Doesn't match with New Password")]
    [DataType(DataType.Password)]
    public string NewConfirmPassword { get; set; }

    public AdminEditViewModel()
    {
      ClaimInfos = new List<ClaimInfo>();
    }
  }
}