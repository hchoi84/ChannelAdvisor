using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChannelAdvisor.Models
{
  public interface IGolfioUser
  {
    Task<IdentityResult> RegisterAsync(RegisterViewModel regiterViewModel);
    Task<GolfioUser> GetUserInfoAsync(string email);
    Task<string> CreateEmailConfirmationToken(GolfioUser golfioUser);
    Task<IdentityResult> ConfirmEmailTokenAsync(GolfioUser golfioUser, string token);
    Task<bool> IsValidLoginAsync(GolfioUser golfioUser, string password);
    Task<SignInResult> SignInUserAsync(LoginViewModel loginVM);
    Task SignOutUserAsync();
    Task<GolfioUser> FindByEmailAsync(string email);
    Task<string> GeneratePasswordResetTokenAsync(GolfioUser golfioUser);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel resetPasswordVM);
    Task<List<GolfioUser>> GetAllUsersAsync();
    Task<List<Claim>> GetUserClaimsAsync(GolfioUser golfioUser);
  }
}