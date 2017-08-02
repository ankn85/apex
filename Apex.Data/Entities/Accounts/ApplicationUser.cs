using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Apex.Data.Entities.Accounts
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser()
            : base()
        {
        }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public string Address { get; set; }
    }
}
