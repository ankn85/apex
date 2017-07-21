namespace Apex.Admin.ViewModels.Layouts
{
    public class CrumbViewModel
    {
        public CrumbViewModel()
        {
        }

        public CrumbViewModel(string text)
            : this(text, null, null)
        {
        }

        public CrumbViewModel(string text, string link)
            : this(text, link, null)
        {
        }

        public CrumbViewModel(string text, string link, string icon)
        {
            Text = text;
            Link = link;
            Icon = icon;
        }

        public string Text { get; set; }

        public bool HasText
        {
            get
            {
                return !string.IsNullOrEmpty(Text);
            }
        }

        public string Link { get; set; }

        public bool HasLink
        {
            get
            {
                return !string.IsNullOrEmpty(Link);
            }
        }

        public string Icon { get; set; }

        public bool HasIcon
        {
            get
            {
                return !string.IsNullOrEmpty(Icon);
            }
        }
    }
}
