using System.Collections.Generic;
using System.Linq;
using Clients.Common;
using IisInterrogation;
using Microsoft.Web.Administration;

namespace IISBackup.ViewModels
{
    public class AppPoolViewModel : ViewModelBase
    {
        private IisInformationController iisInformationController = new IisInformationController();

        public AppPoolViewModel()
        {
            SetAppPool();
        }

        public RelayCommand UpdateSelectedAppPoolsCommand
        {
            get { return new RelayCommand(param => UpdateSelectedAppPools(param as IList<string>)); }
        }

        private void UpdateSelectedAppPools(IList<string> appPools)
        {
            if (appPools == null)
            {
                AppPool = null;
            }
            else if (appPools.Count == 1 && !string.IsNullOrWhiteSpace(appPools[0]))
            {
                AppPool = iisInformationController.GetAppPool(appPools[0]);
            }
            else
            {
                AppPool = null;                
            }

            IisApplications = CollateIisApps(iisInformationController, appPools);
        }

        private void SetAppPool()
        {
            IisInformationController controller = new IisInformationController();
            
            AppPools = iisInformationController.ListAppPools().Select(pool => new AppPoolOverViewViewModel(pool.Name, controller)).ToList();
            AppPool = null;
        }

        private IList<IisApplicationViewModel> CollateIisApps(IisInformationController controller, IList<string> appPools)
        {
            IList<IisApplicationViewModel> appOverViews = new List<IisApplicationViewModel>();
            IList<Application> apps = controller.ListApplicationsFromAppPools(appPools);
            IList<Site> sites = controller.ListSitesFromAppPools(appPools);

            foreach (Application application in apps)
            {
                appOverViews.Add(new IisApplicationViewModel() { ApplicationType = IisApplicationType.Application, Name = application.Path });
            }

            foreach (Site site in sites)
            {
                appOverViews.Add(new IisApplicationViewModel() { ApplicationType = IisApplicationType.WebSite, Name = site.Name});
            }

            return appOverViews;
        }

        protected override void RefreshPageData()
        {
            SetAppPool();
        }

        private IList<AppPoolOverViewViewModel> appPools;
        public IList<AppPoolOverViewViewModel> AppPools
        {
            get { return appPools; }

            protected set
            {
                appPools = value;
                OnPropertyChanged("AppPools");    
            }
        }

        private ApplicationPool appPool;
        public ApplicationPool AppPool
        {
            get { return appPool; }

            protected set
            {
                appPool = value;
                OnPropertyChanged("AppPool");
            }
        }


        private IList<IisApplicationViewModel> iisApplications;
        public IList<IisApplicationViewModel> IisApplications
        {
            get { return iisApplications; }

            protected set
            {
                iisApplications = value;
                OnPropertyChanged("IisApplications");
            }
        }
        
    }
}
