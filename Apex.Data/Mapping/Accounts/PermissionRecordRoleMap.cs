using Apex.Data.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping.Accounts
{
    public sealed class PermissionRecordRoleMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<PermissionRecordRole>();

            builder.ToTable("PermissionRecordRoles", "dbo").HasKey(p => p.Id);

            builder.HasOne(prr => prr.Role)
               .WithMany(r => r.PermissionRecordRoles)
               .HasForeignKey(prr => prr.ApplicationRoleId);

            builder.HasOne(prr => prr.PermissionRecord)
               .WithMany(pr => pr.PermissionRecordRoles)
               .HasForeignKey(prr => prr.PermissionRecordId);
        }
    }
}
