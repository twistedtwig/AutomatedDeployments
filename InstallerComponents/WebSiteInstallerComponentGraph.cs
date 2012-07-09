using System.Collections.Generic;

namespace InstallerComponents
{
    public class WebSiteInstallerComponentGraph : IisInstallerComponentBaseGraph
    {
        public WebSiteInstallerComponentGraph(string name, IDictionary<string, string> values) : base(name, values, ComponentType.WebSiteInstaller)
        {
        }

    }
}