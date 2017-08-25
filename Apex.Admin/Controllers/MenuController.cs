using Apex.Admin.ViewModels.Menus;
using Apex.Data.Entities.Menus;
using Apex.Services.Extensions;
using Apex.Services.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apex.Admin.Controllers
{
    public class MenuController : AdminController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            var menus = await _menuService.GetListAsync();

            return View(menus);
        }

        public async Task<IActionResult> Create()
        {
            MenuViewModel model = new MenuViewModel
            {
                Active = true
            };

            await AssignMenusAsync(model);

            return View("Save", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                Menu entity = ParseMenu(model);
                await _menuService.CreateAsync(entity);

                return RedirectToAction(nameof(Index));
            }

            await AssignMenusAsync(model);

            return View("Save", model);
        }

        public async Task<IActionResult> Update(int id)
        {
            Menu entity = await _menuService.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            MenuViewModel model = new MenuViewModel(entity);
            await AssignMenusAsync(model);

            return View("Save", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                Menu entity = await _menuService.FindAsync(model.Id);

                if (entity == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                entity = ParseMenu(model, entity);
                await _menuService.UpdateAsync(entity);

                return RedirectToAction(nameof(Index));
            }

            await AssignMenusAsync(model);

            return View("Save", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int[] ids)
        {
            int effectedRows = 0;
            IEnumerable<Menu> entities = await _menuService.FindAsync(ids);

            if (entities.Any())
            {
                effectedRows = await _menuService.DeleteAsync(entities);
            }

            return Ok(effectedRows);
        }

        private async Task AssignMenusAsync(MenuViewModel model)
        {
            IList<SelectListItem> menuItems = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "None" }
            };

            var menus = await _menuService.GetListAsync();

            foreach (Menu menu in menus)
            {
                menuItems.Add(new SelectListItem { Value = menu.Id.ToString(), Text = menu.Title });
                BuildMenuItems(menu, menuItems, menu.Title);
            }

            model.Menus = menuItems;
        }

        private void BuildMenuItems(Menu parent, IList<SelectListItem> menuItems, string parentName)
        {
            parentName += " » ";
            var subMenus = parent.SubMenus.OrderBy(m => m.Priority);

            foreach (var menu in subMenus)
            {
                menuItems.Add(new SelectListItem
                {
                    Value = menu.Id.ToString(),
                    Text = parentName + menu.Title
                });

                BuildMenuItems(menu, menuItems, parentName + menu.Title);
            }
        }

        private Menu ParseMenu(MenuViewModel model, Menu entity = null)
        {
            entity = entity ?? new Menu();

            entity.ParentId = model.ParentId > 0 ? model.ParentId : null;
            entity.Title = model.Title.Trim();
            entity.Url = model.Url.TrimNull();
            entity.Icon = model.Icon.TrimNull();
            entity.Note = model.Note.TrimNull();
            entity.NoteBackground = model.NoteBackground.TrimNull();
            entity.Priority = model.Priority;
            entity.Active = model.Active;

            return entity;
        }
    }
}
