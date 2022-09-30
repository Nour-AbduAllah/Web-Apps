using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            #region Change default Login path
            //We are using this part of code to solve a problem caused by
            //Authorize attribute which can not find the path of login page
            //it tries to go to "/Account/Login" but we need to change it to "/Identity/Account/Login"
            //services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
            //    options =>
            //    {
            //        //We can add other options here
            //        options.LoginPath = "/Identity/Account/Login";
            //    });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            #endregion

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                             .AddRazorPagesOptions(options =>
                             {
                                 options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                                 options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                             });
            services.AddSession(options =>
            {
                //Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                //Make the session cookie essential
                options.Cookie.IsEssential = true;
            }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMvcWithDefaultRoute();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "root",
                    template: "{area=Customer}/{controller}/{action}/{id?}"
                );
                routes.MapAreaRoute(
                    name: "areas",
                    areaName: "Customer",
                    template: "{controller=Home}/{action=Index}/{id?}/{returnUrl?}");
                //routes.MapRoute(
                //"Default",                                              // Route name
                //"{area:exists}/{controller}/{action}/{id}",                           // URL with parameters
                //new { area = "Customer", controller = "Home", action = "Index", id = "" }  // Parameter defaults
                //);
            }
            );
        }
    }
}
