using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChannelAdvisor.Controllers
{
  public class UserController : Controller
  {
    private readonly IGolfioUser _golfioUser;

    public UserController(IGolfioUser golfioUser)
    {
      _golfioUser = golfioUser;
    }

    [HttpGet("User/Edit")]
    public async Task<IActionResult> Edit()
    {
      UserEditViewModel userEditVM = new UserEditViewModel();
      List<string> claimTypes = new List<string>();

      string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      userEditVM.GolfioUser = await _golfioUser.GetUserAsync(userId);
      
      (await _golfioUser.GetUserClaimsAsync(userEditVM.GolfioUser))
        .ForEach(uc => claimTypes.Add(uc.Type.ToString()));

      userEditVM.UserClaims = string.Join(", ", claimTypes);

      return View(userEditVM);
    }

    [HttpPost("User/Edit/UpdatePassword")]
    public async Task<IActionResult> ChangePassword(UserEditViewModel userEditVM)
    {
      IdentityResult identityResult = await _golfioUser.ChangePasswordAsync(userEditVM);

      if (identityResult.Succeeded)
      {
        TempData["ChangePassword"] = "Password has been updated";
      }
      else
      {
        TempData["ChangePassword"] = "Password update failed";
      }

      return RedirectToAction("Edit");
    }
  }
}