using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChannelAdvisor.Models
{
  public class SQLGolfioUserRepo : IGolfioUser
  {
    private readonly UserManager<GolfioUser> _userManager;
    private readonly AppDbContext _db;
    private readonly SignInManager<GolfioUser> _signInManager;

    public SQLGolfioUserRepo(UserManager<GolfioUser> userManager, AppDbContext db, SignInManager<GolfioUser> signInManager)
    {
      _userManager = userManager;
      _db = db;
      _signInManager = signInManager;
    }

    // TODO: Assign user to the appropriate claims
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

      IdentityResult identityResult = await _userManager.CreateAsync(golfioUser, registerViewModel.Password);

      if (!identityResult.Succeeded)
      {
        return identityResult;
      }

      var userCount = _userManager.Users.Count();

      if (userCount <= 1)
      {
        Claim newClaim = new Claim(ClaimType.Admin.ToString(), "true");
        identityResult = await _userManager.AddClaimAsync(golfioUser, newClaim);
      }
      else
      {
        Claim newClaim = new Claim(ClaimType.User.ToString(), "true");
        identityResult = await _userManager.AddClaimAsync(golfioUser, newClaim);
      }

      return identityResult;
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
      return await _userManager.GenerateEmailConfirmationTokenAsync(golfioUser);
    }

    public async Task<IdentityResult> ConfirmEmailTokenAsync(GolfioUser golfioUser, string token)
    {
      return await _userManager.ConfirmEmailAsync(golfioUser, token);
    }

    public async Task<bool> IsValidLoginAsync(GolfioUser golfioUser, string password)
    {
      bool isValidPassword = await _userManager.CheckPasswordAsync(golfioUser, password);
      bool isValidEmail = golfioUser.EmailConfirmed;

      return isValidPassword && isValidEmail;
    }

    public async Task<SignInResult> SignInUserAsync(LoginViewModel loginVM)
    {
      return await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);
    }

    public async Task SignOutUserAsync() => await _signInManager.SignOutAsync();

    public async Task<GolfioUser> FindByEmailAsync(string email)
    {
      return await _db.GolfioUsers.FirstOrDefaultAsync(golfioUser => golfioUser.Email == email);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(GolfioUser golfioUser)
    {
      return await _userManager.GeneratePasswordResetTokenAsync(golfioUser);
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel resetPasswordVM)
    {
      GolfioUser golfioUser = await _db.GolfioUsers.FirstOrDefaultAsync(golfioUser => golfioUser.Email == resetPasswordVM.Email);

      return await _userManager.ResetPasswordAsync(golfioUser, resetPasswordVM.Token, resetPasswordVM.Password);
    }

    public async Task<List<GolfioUser>> GetAllUsersAsync()
    {
      return await _db.GolfioUsers.ToListAsync();
    }

    public async Task<List<Claim>> GetUserClaimsAsync(GolfioUser golfioUser)
    {
      return (await _userManager.GetClaimsAsync(golfioUser)).ToList();
    }
  }
}