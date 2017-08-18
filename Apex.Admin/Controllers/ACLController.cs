using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    public class ACLController : AdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
