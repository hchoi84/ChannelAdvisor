using System;
using System.Threading.Tasks;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChannelAdvisor.Models
{
  public class SQLGolfioUserRepo : IGolfioUser
  {
    private readonly UserManager<GolfioUser> _golfioUser;
    private readonly AppDbContext _db;
    private readonly SignInManager<GolfioUser> _signInManager;

    public SQLGolfioUserRepo(UserManager<GolfioUser> golfioUser, AppDbContext db, SignInManager<GolfioUser> signInManager)
    {
      _golfioUser = golfioUser;
      _db = db;
      _signInManager = signInManager;
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

    public async Task<GolfioUser> GetUserInfoAsync(string email)
    {
      if (email.Contains("@"))
        return await _db.GolfioUsers.FirstOrDefaultAsync(golfioUser => golfioUser.Email == email);
      else
        return await _db.GolfioUsers.FirstOrDefaultAsync(golfioUser => golfioUser.Id == email);
    }

    public async Task<string> CreateEmailConfirmationToken(GolfioUser golfioUser)
    {
      return await _golfioUser.GenerateEmailConfirmationTokenAsync(golfioUser);
    }

    public async Task<IdentityResult> ConfirmEmailTokenAsync(GolfioUser golfioUser, string token)
    {
      return await _golfioUser.ConfirmEmailAsync(golfioUser, token);
    }

    public async Task<bool> IsValidLoginAsync(GolfioUser golfioUser, string password)
    {
      bool isValidPassword = await _golfioUser.CheckPasswordAsync(golfioUser, password);
      bool isValidEmail = golfioUser.EmailConfirmed;

      return isValidPassword && isValidEmail;
    }

    public async Task<SignInResult> SignInUserAsync(LoginViewModel loginVM)
    {
      return await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);
    }

    public async Task SignOutUserAsync() => await _signInManager.SignOutAsync();
  }
}