﻿using Apex.Admin.Models;
using Apex.Services.Accounts;
using Apex.Services.Caching;
using Apex.Services.Emails;
using Apex.Services.Logs;
using Apex.Services.Menus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apex.Websites.AppStart
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfigurationRoot configuration)
        {
            // If you need access to generic IConfiguration this is required.
            services.AddSingleton<IConfiguration>(x => configuration);

            // Add functionality to inject IOptions<T>.
            services.AddOptions();

            //services.Configure<AppSettings>(appSettings =>
            //{
            //    appSettings.MemoryCacheInMinutes = int.Parse(configuration["AppSettings:MemoryCacheInMinutes"]);
            //    appSettings.ServerUploadFolder = configuration["AppSettings:ServerUploadFolder"];
            //});

            // Infrastructure.
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();

            // Systems.
            services.AddScoped<ILogService, LogService>();

            // Files.
            //services.AddScoped<IFileService, FileService>();

            // Emails
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IQueuedEmailService, QueuedEmailService>();
            //services.AddScoped<IEmailSender, MailKitEmailSender>();

            // Menus.
            services.AddScoped<IMenuService, MenuService>();

            // Accounts.
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IACLService, ACLService>();

            // Admin Context.
            services.AddScoped<IAdminContext, AdminContext>();

            return services;
        }
    }
}
