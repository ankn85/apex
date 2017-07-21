using System.ComponentModel.DataAnnotations;

namespace Apex.Websites.ViewModels.Accounts
{
    public sealed class SignInViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
