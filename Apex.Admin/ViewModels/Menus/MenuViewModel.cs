using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apex.Data.Entities.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Admin.ViewModels.Menus
{
    public sealed class MenuViewModel
    {
        public MenuViewModel()
        {
        }

        public MenuViewModel(Menu entity)
        {
            Id = entity.Id;
            ParentId = entity.ParentId;
            Title = entity.Title;
            Url = entity.Url;
            Icon = entity.Icon;
            Note = entity.Note;
            NoteBackground = entity.NoteBackground;
            Priority = entity.Priority;
            Active = entity.Active;
        }

        [HiddenInput]
        public int Id { get; set; }

        [Display(Name = "Parent")]
        public int? ParentId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Note { get; set; }

        [Display(Name = "Note Background")]
        public string NoteBackground { get; set; }

        public int Priority { get; set; }

        public bool Active { get; set; }

        public IEnumerable<SelectListItem> Menus { get; set; }
    }
}
