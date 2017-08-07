using Apex.Data.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping.Accounts
{
    public sealed class ApplicationRoleMenuMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ApplicationRoleMenu>();

            builder.ToTable("AspNetRoleMenus", "dbo").HasKey(p => p.Id);

            builder.HasOne(rm => rm.Role)
               .WithMany(r => r.RoleMenus)
               .HasForeignKey(rm => rm.RoleId);

            builder.HasOne(rm => rm.Menu)
               .WithMany(m => m.RoleMenus)
               .HasForeignKey(rm => rm.MenuId);
        }
    }
}
