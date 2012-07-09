using System.Collections.Generic;

namespace IISBackup.ViewModels.Navigation
{
    public class WebsiteNavigationParmeters : NavigationParmeters
    {
        public WebsiteNavigationParmeters(string website) : this(website, null) { }

        public WebsiteNavigationParmeters(string website, IList<string> applicationPaths)
        {
            ApplicationPaths  = applicationPaths ?? new List<string>();
            Name = website;

            ViewModelType = typeof(WebSiteViewModel);
        }

        public string Name { get; set; }
        public IList<string> ApplicationPaths { get; set; }
    }
}