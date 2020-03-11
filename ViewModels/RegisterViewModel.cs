using System;
using System.ComponentModel.DataAnnotations;
using ChannelAdvisor.Models;

namespace ChannelAdvisor.ViewModels
{
  public class RegisterViewModel
  {
    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    [DataType(DataType.Text)]
    [MinLength(2, ErrorMessage = "Must be at least {1} characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    [DataType(DataType.Text)]
    [MinLength(2, ErrorMessage = "Must be at least {1} characters")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Must choose an option")]
    [Display(Name = "Office Location")]
    [DataType(DataType.Text)]
    public OfficeLocation OfficeLocation { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Must be between {2} to {1} characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password Confirmation is required")]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Does not match with Password")]
    public string PasswordConfirm { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;
  }
}