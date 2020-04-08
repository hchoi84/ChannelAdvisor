using ChannelAdvisor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

    // This method gets called by the runtime. Use this method to add services to the container.
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

      services.AddSingleton<IChannelAdvisor, ChannelAdvisorAPI>();
      services.AddSingleton<IProduct, LocalProductRepo>();
      services.AddScoped<IGolfioUser, SQLGolfioUserRepo>();

      services.AddDbContextPool<AppDbContext>(options => options.UseMySql(Configuration.GetConnectionString(DBConnectionInfo)));

      services.AddSession();
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
