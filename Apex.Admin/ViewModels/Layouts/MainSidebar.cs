namespace Apex.Admin.ViewModels.Layouts
{
    public sealed class MainSidebar
    {
        public MainSidebar(string fullName, MenuItem[] menuItems)
        {
            FullName = fullName;
            MenuItems = menuItems ?? new MenuItem[0];
        }

        public string FullName { get; }

        public MenuItem[] MenuItems { get; }
    }
}
