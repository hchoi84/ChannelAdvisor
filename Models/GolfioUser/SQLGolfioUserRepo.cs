using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChannelAdvisor.Utilities;
using ChannelAdvisor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

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

    public async Task<GolfioUser> GetUserAsync(string value)
    {
      if (value.Contains("@"))
        return await _userManager.FindByEmailAsync(value);
      else
        return await _userManager.FindByIdAsync(value);
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

    public async Task<IdentityResult> UpdateUserInfo(AdminEditViewModel adminEditVM)
    {
      GolfioUser golfioUser = await _userManager.FindByIdAsync(adminEditVM.UserId);

      golfioUser.FirstName = adminEditVM.FirstName;
      golfioUser.LastName = adminEditVM.LastName;
      golfioUser.Email = adminEditVM.Email;
      golfioUser.UserName = adminEditVM.Email;
      golfioUser.NormalizedUserName = adminEditVM.Email;
      golfioUser.NormalizedEmail = adminEditVM.Email;

      return await _userManager.UpdateAsync(golfioUser);
    }

    public async Task<IdentityResult> UpdateAccessPermission(AdminEditViewModel adminEditVM)
    {
      // Retrieve user Claims
      IdentityResult identityResult = new IdentityResult();
      GolfioUser golfioUser = await _userManager.FindByIdAsync(adminEditVM.UserId);
      List<Claim> userClaims = await GetUserClaimsAsync(golfioUser);

      // Does adminEditVM.ClaimInfos.ClaimType contain Admin?
      ClaimInfo adminClaimInfo = adminEditVM.ClaimInfos.FirstOrDefault(ci => ci.ClaimType == ClaimType.Admin.ToString());
        // If yes, check DB to see if there's Admin
        if (adminClaimInfo != null)
        {
          bool isClaimInDB = userClaims.Any(uc => uc.Type.ToString() == adminClaimInfo.ClaimType);

          // If no, add and remove all other claims
          if (!isClaimInDB)
          {
            await _userManager.RemoveClaimsAsync(golfioUser, userClaims);
            Claim claim = new Claim(adminClaimInfo.ClaimType, "true");
            return await _userManager.AddClaimAsync(golfioUser, claim);
          }
        }

      // Separate ClaimValue True and False
      foreach (var claimInfo in adminEditVM.ClaimInfos.Where(ci => ci.ClaimType != ClaimType.Admin.ToString()))
      {
        if (claimInfo.IsSelected == true)
        {
          bool isClaimInDB = userClaims.Any(uc => uc.Type.ToString() == claimInfo.ClaimType);

          // If found, continue
          if (isClaimInDB)
          {
            continue;
          }
          // If not found, create
          else
          {
            Claim claim = new Claim(claimInfo.ClaimType, "true");
            identityResult = await _userManager.AddClaimAsync(golfioUser, claim);
          }
        }
        else
        {
          Claim claimInDB = userClaims.FirstOrDefault(uc => uc.Type.ToString() == claimInfo.ClaimType);

          // If found, delete
          if (claimInDB != null)
          {
            identityResult = await _userManager.RemoveClaimAsync(golfioUser, claimInDB);

            if (!identityResult.Succeeded)
            {
              return identityResult;
            }
          }
          // if not found, continue
          else
          {
            continue;
          }
        }

        if (!identityResult.Succeeded)
        {
          return identityResult;
        }
      }

      return identityResult;
    }

    public async Task<IdentityResult> ChangePasswordAsync(AdminEditViewModel adminEditVM)
    {
      GolfioUser golfioUser = await _userManager.FindByIdAsync(adminEditVM.UserId);
      return await _userManager.ChangePasswordAsync(golfioUser, adminEditVM.OldPassword, adminEditVM.NewPassword);
    }

    public async Task<IdentityResult> DeleteAsync(string userId)
    {
      try
      {
        GolfioUser golfioUser = await _userManager.FindByIdAsync(userId);
        return await _userManager.DeleteAsync(golfioUser);
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~");
        Console.WriteLine(e.InnerException);
        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~");
        return IdentityResult.Failed();
      }
    }
  }
}