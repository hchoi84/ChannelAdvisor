using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisor.Models;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChannelAdvisor.Controllers
{
  public class AdminController : Controller
  {
    private readonly IGolfioUser _golfioUser;

    public AdminController(IGolfioUser golfioUser)
    {
      _golfioUser = golfioUser;
    }

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
          FullName = golfioUser.GetFullName,
          Claims = await _golfioUser.GetUserClaimsAsync(golfioUser),
        };

        adminIndexViewModels.Add(adminIndexViewModel);
      };

      // Sort by user name
      adminIndexViewModels.OrderBy(adminIndexVM => adminIndexVM.FullName);

      return View(adminIndexViewModels);
    }


  }
}