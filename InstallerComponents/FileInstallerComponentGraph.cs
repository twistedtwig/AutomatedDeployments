using System.Collections.Generic;

namespace InstallerComponents
{
    public class FileInstallerComponentGraph : FileComponentBaseGraph
    {
        public FileInstallerComponentGraph(string name, IDictionary<string, string> values) : base(name, values, ComponentType.FileInstaller)
        {
        }
    }
}
