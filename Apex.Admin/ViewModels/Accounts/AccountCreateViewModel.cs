using System.ComponentModel.DataAnnotations;
using Apex.Data.Entities.Accounts;
using Apex.Services.Constants;

namespace Apex.Admin.ViewModels.Accounts
{
    public sealed class AccountCreateViewModel : AccountViewModel
    {
        public AccountCreateViewModel()
            : base()
        {
        }

        public AccountCreateViewModel(ApplicationUser entity, string[] roleNames)
            : base(entity, roleNames)
        {
        }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = ValidationRules.MinPasswordLength)]
        public string Password { get; set; }
    }
}
