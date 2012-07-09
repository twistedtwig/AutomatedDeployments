using System.Collections.Generic;

namespace InstallerComponents
{
    public class RemoteExecutionComponentGraph : ComponentBase
    {
        public RemoteExecutionComponentGraph(string name, IDictionary<string, string> values) : base(name, values, ComponentType.RemoteExecutionComponent)
        {
        }

        public string MsdeployExe { get; set; }

        public string PathToCmd { get; set; }
        public string DestinationComputerName { get; set; }
        public string DestinationUserName { get; set; }
        public string DestinationPassword { get; set; }        

    }
}
