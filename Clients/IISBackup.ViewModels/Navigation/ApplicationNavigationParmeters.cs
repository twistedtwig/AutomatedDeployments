using System;

namespace IISBackup.ViewModels.Navigation
{
    public class ApplicationNavigationParmeters : NavigationParmeters  
    {
        public ApplicationNavigationParmeters(string website, string appPath)
        {
            Website = website;
            AppPath = appPath;

            ViewModelType = typeof (ApplicationViewModel);
        }

        public String Website { get; set; }
        public String AppPath { get; set; }
    }
}