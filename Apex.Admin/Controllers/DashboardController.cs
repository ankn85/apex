using System.Collections.Generic;
using Apex.Admin.Attributes;
using Apex.Admin.ViewModels.Layouts;
using Apex.Services.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    public class DashboardController : AdminController
    {
        [AdminPermission(Permission.Read)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetMessages()
        {
            var messages = new List<MessageViewModel>(8)
            {
                new MessageViewModel
                {
                    Image = "../../AdminLTE-2.3.7/dist/img/user2-160x160.jpg",
                    Title = "Support Team",
                    TimeStamp = "5 mins",
                    Content = "Why not buy a new awesome theme?"
                },
                new MessageViewModel
                {
                    Image = "../../AdminLTE-2.3.7/dist/img/user3-128x128.jpg",
                    Title = "AdminLTE Design Team",
                    TimeStamp = "2 hours",
                    Content = "Why not buy a new awesome theme?"
                },
                new MessageViewModel
                {
                    Image = "../../AdminLTE-2.3.7/dist/img/user4-128x128.jpg",
                    Title = "Developers",
                    TimeStamp = "Today",
                    Content = "Why not buy a new awesome theme?"
                },
                new MessageViewModel
                {
                    Image = "../../AdminLTE-2.3.7/dist/img/user3-128x128.jpg",
                    Title = "Sales Department",
                    TimeStamp = "Yesterday",
                    Content = "Why not buy a new awesome theme?"
                },
                new MessageViewModel
                {
                    Image = "../../AdminLTE-2.3.7/dist/img/user4-128x128.jpg",
                    Title = "Reviewers",
                    TimeStamp = "2 days",
                    Content = "Why not buy a new awesome theme?"
                }
            };

            return Json(messages);
        }

        public IActionResult GetNotifications()
        {
            var notifications = new List<NotificationViewModel>(8)
            {
                new NotificationViewModel
                {
                    Icon = "fa fa-users text-aqua",
                    Content = "5 new members joined today"
                },
                new NotificationViewModel
                {
                    Icon = "fa fa-warning text-yellow",
                    Content = "Very long description here that may not fit into the page and may cause design problems"
                },
                new NotificationViewModel
                {
                    Icon = "fa fa-users text-red",
                    Content = "5 new members joined"
                },
                new NotificationViewModel
                {
                    Icon = "fa fa-shopping-cart text-green",
                    Content = "25 sales made"
                },
                new NotificationViewModel
                {
                    Icon = "fa fa-user text-red",
                    Content = "You changed your username"
                }
            };

            return Json(notifications);
        }

        public IActionResult GetTasks()
        {
            var tasks = new List<TaskViewModel>(8)
            {
                new TaskViewModel
                {
                    Title = "Design some buttons",
                    ProgressBarColor = "progress-bar-aqua",
                    Progress = 20,
                    Complete = 20
                },
                new TaskViewModel
                {
                    Title = "Create a nice theme",
                    ProgressBarColor = "progress-bar-green",
                    Progress = 20,
                    Complete = 40
                },
                new TaskViewModel
                {
                    Title = "Some task I need to do",
                    ProgressBarColor = "progress-bar-red",
                    Progress = 20,
                    Complete = 60
                },
                new TaskViewModel
                {
                    Title = "Make beautiful transitions",
                    ProgressBarColor = "progress-bar-yellow",
                    Progress = 20,
                    Complete = 80
                }
            };

            return Json(tasks);
        }
    }
}
