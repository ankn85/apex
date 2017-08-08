using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Accounts;
using Apex.Data.Entities.Emails;
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
            UserManager<ApplicationUser> userManager)
        {
            await dbContext.Database.EnsureCreatedAsync();

            await SeedIdentityAsync(roleManager, userManager);

            await SeedMenusAsync(dbContext, roleManager);

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
                    FullName = "System Administrator"
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, roles);
                }
            }
        }

        private async Task SeedMenusAsync(
            ObjectDbContext dbContext,
            RoleManager<ApplicationRole> roleManager)
        {
            var menuDbSet = dbContext.Set<Menu>();

            if (!await menuDbSet.AnyAsync())
            {
                string adminRole = "Administrators";
                ApplicationRole role = await roleManager.FindByNameAsync(adminRole);

                if (role != null)
                {
                    DateTime utcNow = DateTime.UtcNow;

                    IEnumerable<Menu> menus = new List<Menu>
                    {
                        new Menu
                        {
                            Title = "Dashboard",
                            Description = "Dashboard",
                            Url = "/admin/dashboard",
                            Icon = "fa fa-dashboard",
                            Priority = 1,
                            RoleMenus = new List<ApplicationRoleMenu>
                            {
                                new ApplicationRoleMenu
                                {
                                    Role = role,
                                    Permission = (int)Permission.Full,
                                    CreatedOnUtc = utcNow
                                }
                            }
                        },
                        new Menu
                        {
                            Title = "Users & Roles",
                            Description = "Users & Roles",
                            Icon = "fa fa-users",
                            Priority = 10,
                            RoleMenus = new List<ApplicationRoleMenu>
                            {
                                new ApplicationRoleMenu
                                {
                                    Role = role,
                                    Permission = (int)Permission.Full,
                                    CreatedOnUtc = utcNow
                                }
                            },
                            SubMenus = new List<Menu>
                            {
                                new Menu
                                {
                                    Title = "User",
                                    Description = "User",
                                    Url = "/admin/user",
                                    Icon = "fa fa-circle-o",
                                    Priority = 1,
                                    RoleMenus = new List<ApplicationRoleMenu>
                                    {
                                        new ApplicationRoleMenu
                                        {
                                            Role = role,
                                            Permission = (int)Permission.Full,
                                            CreatedOnUtc = utcNow
                                        }
                                    }
                                },
                                new Menu
                                {
                                    Title = "Role",
                                    Description = "Role",
                                    Url = "/admin/role",
                                    Icon = "fa fa-circle-o",
                                    Priority = 2,
                                    RoleMenus = new List<ApplicationRoleMenu>
                                    {
                                        new ApplicationRoleMenu
                                        {
                                            Role = role,
                                            Permission = (int)Permission.Full,
                                            CreatedOnUtc = utcNow
                                        }
                                    }
                                }
                            }
                        },
                        new Menu
                        {
                            Title = "System",
                            Description = "System",
                            Icon = "fa fa-gears",
                            Priority = 30,
                            RoleMenus = new List<ApplicationRoleMenu>
                            {
                                new ApplicationRoleMenu
                                {
                                    Role = role,
                                    Permission = (int)Permission.Full,
                                    CreatedOnUtc = utcNow
                                }
                            },
                            SubMenus = new List<Menu>
                            {
                                new Menu
                                {
                                    Title = "Email Account",
                                    Description = "Email Account",
                                    Url = "/admin/emailaccount",
                                    Icon = "fa fa-circle-o",
                                    Priority = 1,
                                    RoleMenus = new List<ApplicationRoleMenu>
                                    {
                                        new ApplicationRoleMenu
                                        {
                                            Role = role,
                                            Permission = (int)Permission.Full,
                                            CreatedOnUtc = utcNow
                                        }
                                    }
                                },
                                new Menu
                                {
                                    Title = "Log",
                                    Description = "Log",
                                    Url = "/admin/log",
                                    Icon = "fa fa-circle-o",
                                    Priority = 2,
                                    RoleMenus = new List<ApplicationRoleMenu>
                                    {
                                        new ApplicationRoleMenu
                                        {
                                            Role = role,
                                            Permission = (int)Permission.Full,
                                            CreatedOnUtc = utcNow
                                        }
                                    }
                                }
                            }
                        }

                    };

                    await menuDbSet.AddRangeAsync(menus);

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
