using System.ComponentModel.DataAnnotations;

namespace Apex.Admin.ViewModels.Authentication
{
    public sealed class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
