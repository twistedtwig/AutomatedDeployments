using System;
using IisInterrogation;

namespace IISBackup.ViewModels
{
    public class WebSiteOverViewViewModel : ViewModelBase
    {     
        public WebSiteOverViewViewModel(string name) : this(name, new IisInformationController()) { }

        public WebSiteOverViewViewModel(string name, IisInformationController controller)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
            if (controller == null) throw new ArgumentNullException("controller");
            Name = name;

            NumberOfApplications = controller.ListApplicaitonsForWebSite(name).Count;
            AppPoolName = controller.GetWebSite(name).ApplicationDefaults.ApplicationPoolName;
        }

        public string Name { get; set; }
        public int NumberOfApplications { get; set; }
        public string AppPoolName { get; set; }

        
        protected override void RefreshPageData() { }
    }
}
