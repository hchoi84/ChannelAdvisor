using System.ComponentModel.DataAnnotations;

namespace ChannelAdvisor.ViewModels
{
  public class ForgotPasswordViewModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}