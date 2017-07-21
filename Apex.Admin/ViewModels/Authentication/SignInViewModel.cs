using System.ComponentModel.DataAnnotations;

namespace Apex.Admin.ViewModels.Authentication
{
    public sealed class SignInViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
