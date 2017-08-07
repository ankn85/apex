using System;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Accounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Apex.Websites.AppStart
{
    public static class StartupTasksExtensions
    {
        public static IApplicationBuilder UseStartupTasks(this IApplicationBuilder app)
        {
            InitDefaultData(app);

            return app;
        }

        private static void InitDefaultData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                ObjectDbContext dbContext = serviceProvider.GetRequiredService<ObjectDbContext>();
                RoleManager<ApplicationRole> roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var dbInitializer = new DbInitializer();

                // This protects from deadlocks by starting the async method on the ThreadPool.
                Task.Run(() => dbInitializer.Initialize(dbContext, roleManager, userManager)).Wait();
            }
        }
    }
}
