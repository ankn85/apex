using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Admin.ViewModels.Authentication
{
    public sealed class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }

        public IEnumerable<SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }
    }
}
