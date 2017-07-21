using Microsoft.AspNetCore.Mvc;

namespace Apex.Websites.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        { 
        }

        public IActionResult Index()
        { 
            return View();
        }
    }
}
