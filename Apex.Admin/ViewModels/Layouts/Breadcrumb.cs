using System.Collections.Generic;

namespace Apex.Admin.ViewModels.Layouts
{
    public class Breadcrumb : List<Crumb>
    {
        public Breadcrumb(IEnumerable<Crumb> crumbs)
            : base(crumbs)
        {
        }
    }
}
