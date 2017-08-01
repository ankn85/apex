using System;
using System.Reflection;
using Apex.Data;
using Apex.Data.Entities.Accounts;
using Apex.Services.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Apex.Websites.AppStart
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCustomIdentity(this IServiceCollection services, string connectionString)
        {
            string migrationsAssembly = typeof(ObjectDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ObjectDbContext>(opts =>
            {
                opts.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly));
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(opts =>
            {
                // Password settings.
                PasswordOptions passwordOpts = opts.Password;
                passwordOpts.RequireDigit = false;
                passwordOpts.RequiredLength = ValidationRules.MinPasswordLength;
                passwordOpts.RequireNonAlphanumeric = false;
                passwordOpts.RequireUppercase = false;
                passwordOpts.RequireLowercase = false;

                // Lockout settings.
                LockoutOptions lockoutOpts = opts.Lockout;
                lockoutOpts.AllowedForNewUsers = true;
                lockoutOpts.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                lockoutOpts.MaxFailedAccessAttempts = ValidationRules.MaxFailedAccessAttemptsToLockout;

                // Cookie settings.
                var cookieOpts = opts.Cookies.ApplicationCookie;
                //cookieOpts.AccessDeniedPath = "";
                cookieOpts.AuthenticationScheme = "apexCookie";
                //cookieOpts.CookieName = ".apexIdentity";
                //cookieOpts.CookiePath = "/";
                //cookieOpts.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo("C:\\Github\\Identity\\artifacts"));
                //cookieOpts.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                cookieOpts.LoginPath = new PathString("/admin/authentication/signin");
                cookieOpts.LogoutPath = new PathString("/admin/authentication/signout");
                cookieOpts.AccessDeniedPath = new PathString("/admin/error/forbidden");

                // User settings.
                //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                opts.User.RequireUniqueEmail = true;

                // SignIn settings.
                SignInOptions signinOpts = opts.SignIn;
                signinOpts.RequireConfirmedEmail = true;
                signinOpts.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<ObjectDbContext, int>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IApplicationBuilder UseCustomIdentity(this IApplicationBuilder app)
        {
            app.UseIdentity();

            return app;
        }
    }
}
