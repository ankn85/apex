using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    public class ErrorController : AdminController
    {
        public IActionResult InternalServerError()
        {
            return View();
        }

        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
