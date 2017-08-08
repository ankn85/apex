using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Admin.Attributes;
using Apex.Admin.ViewModels.DataTables;
using Apex.Admin.ViewModels.Emails;
using Apex.Data.Entities.Emails;
using Apex.Services.Emails;
using Apex.Services.Enums;
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

        //[AdminPermission(Permission.Read)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[AdminPermission(Permission.Read)]
        public async Task<IActionResult> Search(DataTablesRequest model)
        {
            model.ParseFormData(Request.Form);

            var emailAccounts = await _emailAccountService.GetListAsync(model.SortColumnName, model.SortDirection);

            return model.CreateResponse(
                emailAccounts.TotalRecords,
                emailAccounts.TotalRecordsFiltered,
                emailAccounts);
        }

        //[AdminPermission(Permission.Host)]
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new EmailAccountViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        //[AdminPermission(Permission.Host)]
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

        //[AdminPermission(Permission.Host)]
        public async Task<IActionResult> Update(int id)
        {
            EmailAccount entity = await _emailAccountService.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("CreateOrUpdate", new EmailAccountViewModel(entity));
        }

        [HttpPost, ValidateAntiForgeryToken]
        //[AdminPermission(Permission.Host)]
        public async Task<IActionResult> Update(EmailAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                EmailAccount entity = await _emailAccountService.FindAsync(model.Id);

                if (entity == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                entity = ParseEmailAccount(model, entity);
                await _emailAccountService.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            return View("CreateOrUpdate", model);
        }

        [HttpPost]
        //[AdminPermission(Permission.Host)]
        public async Task<IActionResult> Delete(int[] ids)
        {
            int effectedRows = 0;
            IEnumerable<EmailAccount> entities = await _emailAccountService.FindAsync(ids);

            if (entities.Any())
            {
                effectedRows = await _emailAccountService.DeleteAsync(entities);
            }

            return Ok(effectedRows);
        }

        private EmailAccount ParseEmailAccount(EmailAccountViewModel model, EmailAccount entity = null)
        {
            entity = entity ?? new EmailAccount();

            entity.Email = model.Email.Trim();
            entity.DisplayName = model.DisplayName.TrimNull();
            entity.Host = model.Host.Trim();
            entity.Port = model.Port;
            entity.UserName = model.UserName.Trim();
            entity.Password = model.Password;
            entity.EnableSsl = model.EnableSsl;
            entity.UseDefaultCredentials = model.UseDefaultCredentials;
            entity.IsDefaultEmailAccount = model.IsDefaultEmailAccount;

            return entity;
        }
    }
}
