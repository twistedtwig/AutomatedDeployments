using System.Collections.Generic;


namespace InstallerComponents
{
    public class VdirInstallerComponentGraph : IisInstallerComponentBaseGraph
    {
        public VdirInstallerComponentGraph(string name, IDictionary<string, string> values) : base(name, values, ComponentType.ApplicationInstaller)
        {
        }

    }
}