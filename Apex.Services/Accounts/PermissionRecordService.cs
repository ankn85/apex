//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Apex.Data;
//using Apex.Data.Entities.Accounts;
//using Microsoft.EntityFrameworkCore;

//namespace Apex.Services.Accounts
//{
//    public class PermissionRecordService : BaseService, IPermissionRecordService
//    {
//        public PermissionRecordService(ObjectDbContext dbContext)
//            : base(dbContext)
//        {
//        }

//        public async Task<IEnumerable<PermissionRecord>> GetPermissionRecordsByUser(ApplicationUser user)
//        {
//            if (user != null && user.Roles != null && user.Roles.Any())
//            {
//                var roleIds = user.Roles.Select(r => r.RoleId).ToList();

//                return await PermissionRecords.AsNoTracking()
//                    .Include(pr => pr.PermissionRecordRoles)
//                    .Where(pr => pr.PermissionRecordRoles.Any(prr => roleIds.Contains(prr.ApplicationRoleId)))
//                    .ToListAsync();
//            }

//            return new List<PermissionRecord>();
//        }

//        private DbSet<PermissionRecord> PermissionRecords
//        {
//            get
//            {
//                return _dbContext.Set<PermissionRecord>();
//            }
//        }
//    }
//}
