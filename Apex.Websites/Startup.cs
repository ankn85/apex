using System;
using System.IO;
using Apex.Data.Configurations;
using Apex.Websites.AppStart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Apex.Websites
{
    public class Startup
    {
        private readonly string _connectionString;
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            IConfigurationRoot config = builder.Build();
            _connectionString = config.GetConnectionString("DefaultConnection");

            // Load from Database.
            builder.AddEntityFrameworkConfig(opts => opts.UseSqlServer(_connectionString));

            // Set our site-wide config.
            _configuration = builder.Build();

            env.ConfigureNLog("nlog.config");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Enable CORS.
            services.AddCustomCors();

            services.AddCustomIdentity(_connectionString);

            services.AddCustomWebMarkupMin();

            // Submitting large number of form-values.
            //services.Configure<FormOptions>(opts => opts.ValueCountLimit = int.Parse(_configuration["General:RequestFormSizeLimit"]));

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession(opts => opts.IdleTimeout = TimeSpan.FromMinutes(20));

            services.AddCustomMvc();

            services.AddApplicationService(_configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole(_configuration.GetSection("Logging"))
                .AddDebug()
                .AddNLog();

            app.AddNLogWeb();

            var variables = LogManager.Configuration.Variables;
            variables["connectionString"] = _connectionString;
            variables["configDir"] = "Logs";
            // ----- End of NLog -----

            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }*/

            app.UseStaticFiles();

            app.UseCustomCors();

            app.UseCustomIdentity();

            app.UseCustomWebMarkupMin();

            app.UseSession();

            app.UseCustomMvc();

            app.UseStartupTasks();
        }
    }
}
