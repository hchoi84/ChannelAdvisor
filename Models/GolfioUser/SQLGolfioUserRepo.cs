using System;
using System.Threading.Tasks;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq;

namespace ChannelAdvisor.Models
{
  public class SQLGolfioUserRepo : IGolfioUser
  {
    private readonly UserManager<GolfioUser> _golfioUser;
    private readonly AppDbContext _db;
    public SQLGolfioUserRepo(UserManager<GolfioUser> golfioUser, AppDbContext db)
    {
      _golfioUser = golfioUser;
      _db = db;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterViewModel registerViewModel)
    {
      GolfioUser golfioUser = new GolfioUser
      {
        FirstName = registerViewModel.FirstName,
        LastName = registerViewModel.LastName,
        Email = registerViewModel.Email,
        UserName = registerViewModel.Email,
        OfficeLocation = Enum.GetName(typeof(OfficeLocation), Convert.ToInt32(registerViewModel.OfficeLocation)),
      };

      return await _golfioUser.CreateAsync(golfioUser, registerViewModel.Password);
    }

    public GolfioUser GetUserInfo(string email)
    {
      if (email.Contains("@"))
        return _db.GolfioUsers.FirstOrDefault(golfioUser => golfioUser.Email == email);
      else
        return _db.GolfioUsers.FirstOrDefault(golfioUser => golfioUser.Id == email);
    }

    public async Task<string> CreateEmailConfirmationToken(GolfioUser golfioUser)
    {
      return await _golfioUser.GenerateEmailConfirmationTokenAsync(golfioUser);
    }

    public async Task<IdentityResult> ConfirmEmailTokenAsync(GolfioUser golfioUser, string token)
    {
      return await _golfioUser.ConfirmEmailAsync(golfioUser, token);
      
    }
  }
}