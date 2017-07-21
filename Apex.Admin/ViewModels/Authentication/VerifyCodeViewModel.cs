using System.ComponentModel.DataAnnotations;

namespace Apex.Admin.ViewModels.Authentication
{
    public sealed class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }
}
