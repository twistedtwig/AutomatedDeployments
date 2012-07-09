using System;
using IisInterrogation;

namespace IISBackup.ViewModels
{
    public class AppPoolOverViewViewModel : ViewModelBase
    {
        public AppPoolOverViewViewModel(string name) : this(name, new IisInformationController()) { }

        public AppPoolOverViewViewModel(string name, IisInformationController controller)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
            if (controller == null) throw new ArgumentNullException("controller");
            Name = name;

            NumberOfWebSites = controller.ListSitesFromAppPool(Name).Count;
            NumberOfVirtualDirectories = controller.ListApplicationsFromAppPool(Name).Count;
        }

        public string Name { get; set; }
        public int NumberOfWebSites { get; set; }
        public int NumberOfVirtualDirectories { get; set; }


        protected override void RefreshPageData() {  }
    }
}
