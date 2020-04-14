using ChannelAdvisor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChannelAdvisor
{
  public class Startup
  {
    private IConfiguration Configuration { get; }
    private string DBConnectionInfo { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
      DBConnectionInfo = "DbConnection";
    }

    // TODO: Implement claims
    public void ConfigureServices(IServiceCollection services)
    {
      // services.AddControllersWithViews().AddNewtonsoftJson();
      services.AddMvc(options => {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        options.Filters.Add(new AuthorizeFilter(policy));
      });

      services.AddIdentity<GolfioUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

      // Must go after .AddIdentity
      services.ConfigureApplicationCookie(options => 
      {
        options.AccessDeniedPath = new PathString("/AccessDenied");
        options.LoginPath = new PathString("/Login");
      });

      services.AddSingleton<IChannelAdvisor, ChannelAdvisorAPI>();
      services.AddSingleton<IProduct, LocalProductRepo>();

      services.AddScoped<IGolfioUser, SQLGolfioUserRepo>();
      services.AddScoped<IEmail, Email>();

      services.AddDbContextPool<AppDbContext>(options => options.UseMySql(Configuration.GetConnectionString(DBConnectionInfo)));

      services.AddSession();

      services.AddAuthorization(options =>
      {
        options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimType.Admin.ToString(), "true"));
        
        options.AddPolicy("NoSale", policy => policy.RequireAssertion(context =>
          context.User.HasClaim(claim => claim.Type == ClaimType.Admin.ToString() && claim.Value == "true") || 
          context.User.HasClaim(claim => claim.Type == ClaimType.NoSale.ToString() && claim.Value == "true")));
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSession();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
