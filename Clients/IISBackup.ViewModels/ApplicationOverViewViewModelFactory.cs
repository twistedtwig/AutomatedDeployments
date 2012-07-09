using System;
using System.Collections.Generic;
using System.Linq;
using IisInterrogation;
using Microsoft.Web.Administration;

namespace IISBackup.ViewModels
{
    public class ApplicationOverViewViewModelFactory
    {

        public static IList<ApplicationOverVewViewModel> Get(string websiteName, IisInformationController controller)
        {
            Site webSite = controller.GetWebSite(websiteName);
            if(webSite == null) throw new ArgumentOutOfRangeException("websiteName");

            return webSite.Applications.Select(app => new ApplicationOverVewViewModel()
                                                          {
                                                              Path = app.Path, AppPoolName = app.ApplicationPoolName, EnabledProtocols = app.EnabledProtocols, ParentWebsite = websiteName
                                                          }).ToList();
        }

        public static IList<ApplicationOverVewViewModel> GetAll(IisInformationController controller)
        {
            List<ApplicationOverVewViewModel> apps = new List<ApplicationOverVewViewModel>();
            foreach (Site site in controller.ListWebsites())
            {
                apps.AddRange(Get(site.Name, controller));
            }

            return apps;
        }
    }
}
