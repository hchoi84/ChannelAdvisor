using System.Security.Claims;
using System.Threading.Tasks;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChannelAdvisor.Models
{
  public interface IGolfioUser
  {
    Task<IdentityResult> RegisterAsync(RegisterViewModel regiterViewModel);
    GolfioUser GetUserInfo(string email);
    Task<string> CreateEmailConfirmationToken(GolfioUser golfioUser);
    Task<IdentityResult> ConfirmEmailTokenAsync(GolfioUser golfioUser, string token);
  }
}