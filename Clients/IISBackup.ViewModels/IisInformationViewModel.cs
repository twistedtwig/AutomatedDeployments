using System;
using Clients.Common;
using IISBackup.ViewModels.Navigation;

namespace IISBackup.ViewModels
{
    public class IisInformationViewModel : ViewModelBase
    {
        public IisInformationViewModel()
        {
            CurrentViewModel = null;
        }

        public RelayCommand ExportAllIisSettings
        {
            get { return new RelayCommand(param => TestExportAllIisSettings()); }
        }

        public RelayCommand ShowAppPoolViewModelCommand
        {
            get { return new RelayCommand(param => CurrentViewModel = new AppPoolViewModel()); }
        }

        public RelayCommand ShowWebSiteViewModelCommand
        {
            get { return new RelayCommand(param => CurrentViewModel = new WebSiteViewModel(this)); }
        }

        public RelayCommand ShowVirtualDirectoryViewModelCommand
        {
            get { return new RelayCommand(param => CurrentViewModel = new ApplicationViewModel(this)); }
        }

        public RelayCommand NavigateToCommand
        {
            get { return new RelayCommand(param => SwitchViewModel(param as NavigationParmeters)); }
        }


        private void TestExportAllIisSettings()
        {
            //TODO err how do I deal with the wanting a file path!!! / folder path.. err no idea.  maybe want to have a view to deal with all of it., maybe want a view that will deal with N number of files / folders to be sorted, dialog would be nice
        }

        private void SwitchViewModel(NavigationParmeters navigationParmeters)
        {
            if (navigationParmeters == null) {  return; }

            switch (navigationParmeters.ViewModelType.Name)
            {
                case "ApplicationViewModel":
                    ApplicationNavigationParmeters appParams =  navigationParmeters as ApplicationNavigationParmeters;
                    if (appParams != null)
                    {
                        CurrentViewModel = new ApplicationViewModel(this, appParams.Website, appParams.AppPath);
                    }
                    break;

                case "WebSiteViewModel":
                    WebsiteNavigationParmeters siteParams = navigationParmeters as WebsiteNavigationParmeters;
                    if (siteParams != null)
                    {
                        CurrentViewModel = new WebSiteViewModel(this, siteParams.Name, siteParams.ApplicationPaths);
                    }
                    break;

            }
        }

        private ViewModelBase currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return currentViewModel; } 
            set
            {
                currentViewModel = value; 
                OnPropertyChanged("CurrentViewModel");
            }
        }

        protected override void RefreshPageData()
        {
            throw new NotImplementedException();
        }
    }
}
