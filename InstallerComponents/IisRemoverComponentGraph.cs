using System.Collections.Generic;

namespace InstallerComponents
{
    public class IisRemoverComponentGraph : ComponentBase
    {
        public IisRemoverComponentGraph(string name, IDictionary<string, string> values, ComponentType componentType) : base(name, values, componentType)
        {            
        }

        public string MsdeployExe { get; set; }
        public string AppCmdExe { get; set; }

        public string LocalInstallFolder { get; set; }
        public string RemoteInstallFolder { get; set; }
        public string ItemToRemove { get; set; }

        public string DestinationComputerName { get; set; }
        public string DestinationUserName { get; set; }
        public string DestinationPassword { get; set; }        
    }
}
