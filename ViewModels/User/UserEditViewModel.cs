using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ChannelAdvisor.Models;

namespace ChannelAdvisor.ViewModels
{
  public class UserEditViewModel
  {
    public GolfioUser GolfioUser { get; set; }
    public string UserClaims { get; set; }

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

    public UserEditViewModel()
    {
      GolfioUser = new GolfioUser();
    }
  }
}