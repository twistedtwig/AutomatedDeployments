using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;

namespace IisInterrogation
{
    internal class IisRepository
    {
        public ApplicationPool GetAppPool(string appPoolName)
        {
            if(string.IsNullOrWhiteSpace(appPoolName)) throw new ArgumentException("appPoolName");
            
            ServerManager serverManager = new ServerManager();
            if(!serverManager.ApplicationPools.Any(a => a.Name.Equals(appPoolName))) throw new ArgumentOutOfRangeException("no appPool by the name: " + appPoolName);

            return serverManager.ApplicationPools[appPoolName];
        }

        public IList<ApplicationPool> GetAllAppPools()
        {
            ServerManager serverManager = new ServerManager();
            return serverManager.ApplicationPools.Select(a => a).ToList();            
        }

        public Site GetWebSite(string webSiteName)
        {
            if(string.IsNullOrWhiteSpace(webSiteName)) throw new ArgumentException("webSiteName");

            ServerManager serverManager = new ServerManager();
            if(!serverManager.Sites.Any(s => s.Name.Equals(webSiteName))) throw new ArgumentOutOfRangeException("no website by name: " + webSiteName);

            return serverManager.Sites[webSiteName];
        }

        public IList<Site> GetAllWebSites()
        {
            ServerManager serverManager = new ServerManager();
            return serverManager.Sites.Select(s => s).ToList();
        }

        public IList<Site> GetAllWesbitesForAppPool(string appPoolName)
        {
            ServerManager serverManager = new ServerManager();

            return serverManager.Sites.Where(site => site.ApplicationDefaults.ApplicationPoolName.Equals(appPoolName)).ToList();
        }

        public IList<Application> GetAllApplicaitonsForWebSite(string webSiteName)
        {
            ServerManager serverManager = new ServerManager();
            if (!serverManager.Sites.Any(s => s.Name.Equals(webSiteName)))
            {
                throw new ArgumentException("no website by that name");                
            }

            return serverManager.Sites[webSiteName].Applications.Select(x => x).ToList();
        }

        public IList<Application> GetAllApplicationsForAppPool(string appPoolName)
        {
            ServerManager serverManager = new ServerManager();
            List<Application> applications = new List<Application>();

            foreach (Site site in serverManager.Sites)
            {
                applications.AddRange(GetAllApplications(appPoolName, site));
            }

            return applications;
        }

        public IList<Application> GetAllApplicationsForAppPoolSite(string webSiteName, string appPoolName)
        {
            return GetAllApplications(appPoolName, GetWebSite(webSiteName));
        }

        private IList<Application> GetAllApplications(string appPoolName, Site site)
        {
            IList<Application> applications = new List<Application>();
            if (string.IsNullOrWhiteSpace(appPoolName)) return applications;
            if (site == null) return applications;

            foreach (Application application in site.Applications)
            {
                if (application.ApplicationPoolName.Equals(appPoolName))
                {
                    applications.Add(application);
                }
            }

            return applications;
        }        
    }
}
