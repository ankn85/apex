using System.Threading.Tasks;
using Apex.Admin.ViewModels.DataTables;
using Apex.Admin.ViewModels.Emails;
using Apex.Data.Entities.Emails;
using Apex.Services.Emails;
using Apex.Services.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    public class EmailAccountController : AdminController
    {
        private readonly IEmailAccountService _emailAccountService;

        public EmailAccountController(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(DataTablesRequest model)
        {
            model.ParseFormData(Request.Form);

            var emailAccounts = await _emailAccountService.GetListAsync(model.SortColumnName, model.SortDirection);

            return model.CreateResponse(emailAccounts.TotalRecords, emailAccounts.TotalRecordsFiltered, emailAccounts);
        }

        public IActionResult Create()
        {
            return View("CreateOrUpdate", new EmailAccountViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmailAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                EmailAccount entity = ParseEmailAccount(model);
                await _emailAccountService.CreateAsync(entity);

                return RedirectToAction(nameof(Index));
            }

            return View("CreateOrUpdate", model);
        }

        public async Task<IActionResult> Update(int id)
        {
            EmailAccount entity = await _emailAccountService.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            EmailAccountViewModel model = ParseEmailAccountViewModel(entity);

            return View("CreateOrUpdate", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EmailAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                EmailAccount entity = ParseEmailAccount(model);
                await _emailAccountService.UpdateAsync(entity);

                return RedirectToAction(nameof(Index));
            }

            return View("CreateOrUpdate", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int[] ids)
        {
            int idsCount = ids != null ? ids.Length : 0;

            if (idsCount == 0)
            {
                return BadRequestApiError("EmailAccountId", "'EmailAccount Ids' should not be empty.");
            }

            int effectedRows = idsCount == 1 ?
                await _emailAccountService.DeleteAsync(ids[0]) :
                await _emailAccountService.DeleteAsync(ids);

            return Ok(effectedRows);
        }

        [NonAction]
        private EmailAccount ParseEmailAccount(EmailAccountViewModel model)
        {
            return new EmailAccount
            {
                Id = model.Id,
                Email = model.Email.Trim(),
                DisplayName = model.DisplayName.TrimNull(),
                Host = model.Host.Trim(),
                Port = model.Port,
                UserName = model.UserName.Trim(),
                Password = model.Password.Trim(),
                EnableSsl = model.EnableSsl,
                UseDefaultCredentials = model.UseDefaultCredentials,
                IsDefaultEmailAccount = model.IsDefaultEmailAccount
            };
        }

        [NonAction]
        private EmailAccountViewModel ParseEmailAccountViewModel(EmailAccount entity)
        {
            return new EmailAccountViewModel
            {
                Id = entity.Id,
                Email = entity.Email,
                DisplayName = entity.DisplayName,
                Host = entity.Host,
                Port = entity.Port,
                UserName = entity.UserName,
                Password = entity.Password,
                EnableSsl = entity.EnableSsl,
                UseDefaultCredentials = entity.UseDefaultCredentials,
                IsDefaultEmailAccount = entity.IsDefaultEmailAccount
            };
        }
    }
}
