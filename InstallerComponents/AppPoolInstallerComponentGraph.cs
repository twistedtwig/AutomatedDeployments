using System.Collections.Generic;

namespace InstallerComponents
{
    public class AppPoolInstallerComponentGraph : IisInstallerComponentBaseGraph
    {
        public AppPoolInstallerComponentGraph(string name, IDictionary<string, string> values) : base(name, values, ComponentType.AppPoolInstaller)
        {
        }
    }
}
