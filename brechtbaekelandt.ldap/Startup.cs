using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using brechtbaekelandt.ldap.Data;
using brechtbaekelandt.ldap.Identity;
using brechtbaekelandt.ldap.Services;
using brechtbaekelandt.ldap.Identity.Models;
using brechtbaekelandt.ldap.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace brechtbaekelandt.ldap
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
            services
               .Configure<IdentityOptions>(
                   options =>
                   {
                       // Password settings
                       options.Password.RequireDigit = true;
                       options.Password.RequiredLength = 6;
                       options.Password.RequireNonAlphanumeric = true;
                       options.Password.RequireUppercase = false;
                       options.Password.RequireLowercase = true;
                       //options.Password.RequiredUniqueChars = 6;

                       // Lockout settings
                       options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                       options.Lockout.MaxFailedAccessAttempts = 10;
                       options.Lockout.AllowedForNewUsers = true;

                       // User settings
                       options.User.RequireUniqueEmail = true;
                       //options.User.AllowedUserNameCharacters =
                       //    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


                   });

            services
                   .Configure<LdapSettings>(
                       Configuration.GetSection("LdapSettings"));

            services.AddDbContext<LdapDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<LdapUser, IdentityRole>()
                .AddEntityFrameworkStores<LdapDbContext>()
                .AddUserManager<LdapUserManager>()
                .AddSignInManager<LdapSignInManager>()
                .AddDefaultTokenProviders();

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.Cookie.Name = "brechtbaekelandt.ldap.identity";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Expiration = TimeSpan.FromDays(150);
                    options.LoginPath = "/Account/Signin"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                    options.LogoutPath = "/Account/Signout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                    options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                    options.SlidingExpiration = true;
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ILdapService, LdapService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
