using Apex.Data.Entities.Accounts;
using Apex.Data.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data
{
    public sealed class ObjectDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ObjectDbContext(DbContextOptions<ObjectDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            IEntityMapper mapper = new ObjectEntityMapper();
            mapper.MapEntities(builder);
        }
    }
}
