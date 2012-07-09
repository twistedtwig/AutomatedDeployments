using System;
using System.Collections.Generic;
using System.Linq;
using Clients.Common;
using IISBackup.ViewModels.Navigation;
using IisInterrogation;
using Microsoft.Web.Administration;

namespace IISBackup.ViewModels
{
    public class WebSiteViewModel : ViewModelBase
    {
        private IisInformationController iisInformationController = new IisInformationController();
        private IisInformationViewModel ParentViewModel;

        public WebSiteViewModel(IisInformationViewModel parentViewModel) : this(parentViewModel, string.Empty) { }

        public WebSiteViewModel(IisInformationViewModel parentViewModel, string websiteName) : this(parentViewModel, websiteName, null) { }

        public WebSiteViewModel(IisInformationViewModel parentViewModel, string websiteName, IList<string> appPaths)
        {
            ParentViewModel = parentViewModel;
            SetWebSiteViewModel();

            SelectedWebSiteOverView = Websites.FirstOrDefault(w => w.Name.Equals(websiteName));

            if (appPaths != null && appPaths.Any())
            {
                foreach (string appPath in appPaths)
                {
                    foreach (ApplicationOverVewViewModel model in ApplicationOverviewViewModels)
                    {
                        model.IsSelected = model.Path.Equals(appPath);
                    }
                }
            }            
        }

        public RelayCommand NavigateToApplicationCommand
        {
            get { return new RelayCommand(param => NavigateToApplication(param as ApplicationOverVewViewModel)); }
        }

        private void NavigateToApplication(ApplicationOverVewViewModel applicationViewModel)
        {
            if(applicationViewModel == null) return;

            ParentViewModel.NavigateToCommand.Execute(new ApplicationNavigationParmeters(applicationViewModel.ParentWebsite, applicationViewModel.Path));
        }

        private IList<WebSiteOverViewViewModel> websites;
        public IList<WebSiteOverViewViewModel> Websites
        {
            get { return websites; }
            set
            {
                websites = value;
                OnPropertyChanged("Websites");
            }
        }

        private WebSiteOverViewViewModel selectedWebSiteOverView;
        public WebSiteOverViewViewModel SelectedWebSiteOverView
        {
            get { return selectedWebSiteOverView; }
            set
            {
                selectedWebSiteOverView = value;
                OnPropertyChanged("SelectedWebSiteOverView");
                if (selectedWebSiteOverView != null)
                {
                    WebSite = iisInformationController.GetWebSite(selectedWebSiteOverView.Name);
                    ApplicationOverviewViewModels = ApplicationOverViewViewModelFactory.Get(webSite.Name, iisInformationController);
                }
            }
        }


        private Site webSite;
        public Site WebSite
        {
            get { return webSite; }
            set
            {
                if (webSite != value)
                {
                    webSite = value;
                    OnPropertyChanged("WebSite");
                }
            }
        }

        private IList<ApplicationOverVewViewModel> applicationOverviewViewModels;
        public IList<ApplicationOverVewViewModel> ApplicationOverviewViewModels
        {
            get { return applicationOverviewViewModels; }
            set
            {
                applicationOverviewViewModels = value;
                OnPropertyChanged("ApplicationOverviewViewModels");
            }
        }


        private void SetWebSiteViewModel()
        {
            Websites = iisInformationController.ListWebsites().Select(site => new WebSiteOverViewViewModel(site.Name, iisInformationController)).ToList();
            webSite = null;
        }

        protected override void RefreshPageData()
        {
            SetWebSiteViewModel();
        }
    }
}
