using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Websites.ViewModels.Accounts
{
    public sealed class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }

        public IEnumerable<SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
