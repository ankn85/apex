using System.Collections.Generic;

namespace Apex.Admin.ViewModels.Layouts
{
    public class BreadcrumbViewModel : List<CrumbViewModel>
    {
        public BreadcrumbViewModel(IEnumerable<CrumbViewModel> crumbs)
            : base(crumbs)
        {
        }
    }
}
