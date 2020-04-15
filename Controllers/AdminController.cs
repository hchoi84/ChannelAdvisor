using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using ChannelAdvisor.Utilities;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChannelAdvisor.Controllers
{
  [Authorize(Policy = "Admin")]
  public class AdminController : Controller
  {
    private readonly IGolfioUser _golfioUser;

    public AdminController(IGolfioUser golfioUser)
    {
      _golfioUser = golfioUser;
    }

    [HttpGet("Admin")]
    public async Task<IActionResult> Index()
    {
      // Get all users
      List<GolfioUser> golfioUsers = await _golfioUser.GetAllUsersAsync();

      // Convert each user to Admin Index View Model
      // Get all claims for each user
      List<AdminIndexViewModel> adminIndexViewModels = new List<AdminIndexViewModel>();
      
      foreach (GolfioUser golfioUser in golfioUsers)
      {
        AdminIndexViewModel adminIndexViewModel = new AdminIndexViewModel
        {
          Id = golfioUser.Id,
          FullName = golfioUser.FullName,
          Claims = await _golfioUser.GetUserClaimsAsync(golfioUser),
        };

        adminIndexViewModels.Add(adminIndexViewModel);
      };

      // Sort by user name
      adminIndexViewModels.OrderBy(adminIndexVM => adminIndexVM.FullName);

      return View(adminIndexViewModels);
    }

    [HttpGet("Admin/Edit/{userId}")]
    public async Task<IActionResult> Edit(string userId)
    {

      // Get user information
      GolfioUser golfioUser = await _golfioUser.GetUserAsync(userId);

      AdminEditViewModel adminEditVM = new AdminEditViewModel()
      {
        UserId = golfioUser.Id,
        FirstName = golfioUser.FirstName,
        LastName = golfioUser.LastName,
        Email = golfioUser.Email,
      };      

      // Get user claim(s)
      List<Claim> claims = await _golfioUser.GetUserClaimsAsync(golfioUser);

      foreach (var claimType in Enum.GetValues(typeof(ClaimType)).Cast<ClaimType>())
      {
        ClaimInfo claimInfo = new ClaimInfo()
        {
          ClaimType = claimType.ToString(),
          IsSelected = false,
        };

        adminEditVM.ClaimInfos.Add(claimInfo);
      }

      foreach (var claim in claims)
      {
        ClaimInfo claimInfo = adminEditVM.ClaimInfos.FirstOrDefault(ci => ci.ClaimType == claim.Type.ToString());
        claimInfo.IsSelected = Convert.ToBoolean(claim.Value);
      }

      return View(adminEditVM);
    }

    [HttpPost("Admin/Edit")]
    public async Task<IActionResult> EditUserInfo(AdminEditViewModel adminEditVM)
    {
      if (string.IsNullOrEmpty(adminEditVM.FirstName) ||
          string.IsNullOrEmpty(adminEditVM.LastName) ||
          string.IsNullOrEmpty(adminEditVM.Email))
      {
        ModelState.AddModelError(string.Empty, "All fields are required");
        return View();
      }

      IdentityResult identityResult = await _golfioUser.UpdateUserInfo(adminEditVM);
      
      if (!identityResult.Succeeded)
      {
        ModelState.AddModelError(string.Empty, "Update failed");
        return View();
      }
      else
      {
        TempData["EditUserInfo"] = "Information updated successfully";
        return RedirectToAction("Edit", new { userId = adminEditVM.UserId });
      }
    }

    [HttpPost("Admin/Edit/AccessPermission")]
    public async Task<IActionResult> EditAccessPermission(AdminEditViewModel adminEditVM)
    {
      IdentityResult identityResult = await _golfioUser.UpdateAccessPermission(adminEditVM);

      if (identityResult == null)
      {
        TempData["AccessPermission"] = "Updating user Access Permission failed";
        return RedirectToAction("Edit", new { userId = adminEditVM.UserId });
      }
      else
      {
        TempData["AccessPermission"] = "Updating user Access Permission Successful";
        return RedirectToAction("Edit", new { userId = adminEditVM.UserId });
      }
    }

    [HttpPost("Admin/Delete/{userId}")]
    public async Task<IActionResult> Delete(string userId)
    {
      var identityResult = await _golfioUser.DeleteAsync(userId);

      if (identityResult.Succeeded)
      {
        return RedirectToAction("Index");
      }
      else
      {
        TempData["MessageTitle"] = "Action Failed";
        TempData["Message"] = "Deleting the user failed due to dependencies";
        return RedirectToAction("Index");
      }
    }
  }
}