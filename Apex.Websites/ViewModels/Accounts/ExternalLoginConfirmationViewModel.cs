using System.ComponentModel.DataAnnotations;

namespace Apex.Websites.ViewModels.Accounts
{
    public sealed class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
