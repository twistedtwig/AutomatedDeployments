using System.Collections.Generic;
using System.Linq;
using Clients.Common;
using IISBackup.ViewModels.Navigation;
using IisInterrogation;

namespace IISBackup.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        private IisInformationController iisInformationController = new IisInformationController();
        private IisInformationViewModel ParentVieModel;

        public ApplicationViewModel(IisInformationViewModel parentVieModel) :this(parentVieModel, string.Empty, string.Empty) { }

        public ApplicationViewModel(IisInformationViewModel parentVieModel, string website, string appPath)
        {
            SetApplicationViewModel();
            ParentVieModel = parentVieModel;
            if (!string.IsNullOrWhiteSpace(website) && !string.IsNullOrWhiteSpace(appPath))
            {                
                SelectedApplicationOverView = ApplicationOverviewViewModels.FirstOrDefault(a => a.ParentWebsite.Equals(website) && a.Path.Equals(appPath));
            }
        }

        public RelayCommand NavigateToWebsiteCommand
        {
            get { return new RelayCommand(param => NavigateToWebsite(param as ApplicationOverVewViewModel)); }
        }

        private void NavigateToWebsite(ApplicationOverVewViewModel applicationViewModel)
        {
            if (applicationViewModel == null) return;

            ParentVieModel.NavigateToCommand.Execute(new WebsiteNavigationParmeters(applicationViewModel.ParentWebsite, new List<string> { applicationViewModel.Path }));
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

        private ApplicationOverVewViewModel selectedApplicationOverView;
        public ApplicationOverVewViewModel SelectedApplicationOverView
        {
            get { return selectedApplicationOverView; }
            set
            {
                selectedApplicationOverView = value;
                OnPropertyChanged("SelectedApplicationOverView");                       
            }
        }

        

        private void SetApplicationViewModel()
        {
            ApplicationOverviewViewModels = ApplicationOverViewViewModelFactory.GetAll(iisInformationController);
            SelectedApplicationOverView = null;
        }

        protected override void RefreshPageData()
        {
            SetApplicationViewModel();
        }
    }
}
