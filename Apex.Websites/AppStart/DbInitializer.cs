using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Accounts;
using Apex.Data.Entities.Emails;
using Apex.Services.Accounts;
using Apex.Services.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Apex.Websites.AppStart
{
    public sealed class DbInitializer
    {
        public async Task Initialize(
            ObjectDbContext dbContext,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IPermissionProvider permissionProvider)
        {
            await dbContext.Database.EnsureCreatedAsync();

            await SeedIdentityAsync(roleManager, userManager);

            await SeedPermissionRecordsAsync(dbContext, permissionProvider, roleManager);

            await SeedEmailAccountsAsync(dbContext);
        }

        private async Task SeedIdentityAsync(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            var roles = new string[] { "Administrators", "Supervisors", "Moderators", "Registereds", "Guests" };

            foreach (var roleName in roles)
            {
                var identityRole = await roleManager.FindByNameAsync(roleName);

                if (identityRole == null)
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            string email = "ankn85@yahoo.com";
            string password = "1qazXSW@";

            var user = await userManager.FindByNameAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Logan Weapon X",
                    Gender = "Male",
                    Birthday = new DateTime(1974, 11, 1),
                    Address = "Marvel Comics"
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, roles);
                }
            }
        }

        private async Task SeedPermissionRecordsAsync(
            ObjectDbContext dbContext,
            IPermissionProvider permissionProvider,
            RoleManager<ApplicationRole> roleManager)
        {
            var permissionRecordDbSet = dbContext.Set<PermissionRecord>();

            if (!await permissionRecordDbSet.AnyAsync())
            {
                string roleName = "Administrators";
                var role = await roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    var permissionRecords = permissionProvider.GetPermissionRecords();

                    foreach (var item in permissionRecords)
                    {
                        await permissionRecordDbSet.AddAsync(new PermissionRecord
                        {
                            Name = item.Name,
                            SystemName = item.SystemName,
                            Category = item.Category,
                            PermissionRecordRoles = new List<PermissionRecordRole>
                            {
                                new PermissionRecordRole { ApplicationRoleId = role.Id, Permission = (int)Permission.Full }
                            }
                        });
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SeedEmailAccountsAsync(ObjectDbContext dbContext)
        {
            string email = "eschoolapi@gmail.com";
            string password = "1qaw3(OLP_";

            var emailAccountDbSet = dbContext.Set<EmailAccount>();

            if (!await emailAccountDbSet.AnyAsync(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                await emailAccountDbSet.AddAsync(new EmailAccount
                {
                    Email = email,
                    DisplayName = "No Reply",
                    Host = "smtp.gmail.com",
                    Port = 587,
                    UserName = email,
                    Password = password,
                    EnableSsl = false,
                    UseDefaultCredentials = true,
                    IsDefaultEmailAccount = true
                });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
