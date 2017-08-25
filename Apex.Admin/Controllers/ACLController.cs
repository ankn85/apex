using Apex.Services.Accounts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Apex.Admin.Controllers
{
    public class ACLController : AdminController
    {
        private readonly IACLService _aclService;

        public ACLController(IACLService aclService)
        {
            _aclService = aclService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _aclService.GetListAsync();

            return View(model);
        }
    }
}
