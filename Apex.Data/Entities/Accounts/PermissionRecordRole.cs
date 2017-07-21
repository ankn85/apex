namespace Apex.Data.Entities.Accounts
{
    public class PermissionRecordRole : BaseEntity
    {
        public int ApplicationRoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public int PermissionRecordId { get; set; }

        public virtual PermissionRecord PermissionRecord { get; set; }

        public int Permission { get; set; }
    }
}
