using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Apex.Data.Entities.Accounts
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser()
            : base()
        {
        }
    }
}
