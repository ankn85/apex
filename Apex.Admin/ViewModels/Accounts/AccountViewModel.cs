using System.ComponentModel.DataAnnotations;
using Apex.Services.Constants;

namespace Apex.Admin.ViewModels.Accounts
{
    public sealed class AccountViewModel
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = ValidationRules.MinPasswordLength)]
        public string Password { get; set; }

        public bool Locked { get; set; }

        public string[] Roles { get; set; }
    }
}
