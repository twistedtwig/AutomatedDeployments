using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;

namespace IisInterrogation
{
    public class IisInformationController
    {
        
        public ApplicationPool GetAppPool(string name)
        {
            IisRepository iisRepo = new IisRepository();
            return iisRepo.GetAppPool(name);
        }

        public IList<ApplicationPool> ListAppPools()
        {
            IisRepository iisRepo = new IisRepository();
            return iisRepo.GetAllAppPools();
        }








        public Site GetWebSite(string name)
        {
            IisRepository iisRepo = new IisRepository();
            return iisRepo.GetWebSite(name);
        }

        public IList<Site> ListWebsites()
        {
            IisRepository iisRepo = new IisRepository();
            return iisRepo.GetAllWebSites();
        }

        public IList<Site> ListSitesFromAppPool(string appPoolName)
        {
            return ListSitesFromAppPools(new List<string> {appPoolName});
        }

        public IList<Site> ListSitesFromAppPools(IList<string> appPoolNames)
        {
            IisRepository iisRepo = new IisRepository();
            IList<Site> sites = new List<Site>();
            foreach (string appPoolName in appPoolNames)
            {
                IList<Site> webSites = iisRepo.GetAllWesbitesForAppPool(appPoolName);
                foreach (Site webSite in webSites.Where(webSite => !sites.Any(x => x.Name.Equals(webSite.Name))))
                {
                    sites.Add(webSite);
                }
            }

            return sites;
        }

        
        

        public Application GetApplication(string website, string appPath)
        {
            return ListApplicaitonsForWebSite(website).FirstOrDefault(a => a.Path.Equals(appPath));
        }

        public IList<Application> ListApplicationsFromAppPool(string appPoolName)
        {
            return ListApplicationsFromAppPools(new List<string> {appPoolName});
        }

        public IList<Application> ListApplicationsFromAppPools(IList<string> appPoolNames)
        {
            IisRepository iisRepo = new IisRepository();
            IList<Application> applications = new List<Application>();
            foreach (string appPoolName in appPoolNames)
            {
                IList<Application> apps = iisRepo.GetAllApplicationsForAppPool(appPoolName);
                foreach (Application app in apps.Where(app => !applications.Any(x => x.ApplicationPoolName.Equals(app.ApplicationPoolName))))
                {
                    applications.Add(app);
                }
            }

            return applications;
        }

        public IList<Application> ListApplicaitonsForWebSite(string webSiteName)
        {
            IisRepository iisRepo = new IisRepository();
            return iisRepo.GetAllApplicaitonsForWebSite(webSiteName);
        }

        public IList<Application> ListAllApplicaitons()
        {
            List<Application> apps = new List<Application>();
            IisRepository iisRepo = new IisRepository();
            IList<Site> sites = iisRepo.GetAllWebSites();
            foreach (Site site in sites)
            {
                apps.AddRange(site.Applications);
            }

            return apps;
        }
         
    }
}
