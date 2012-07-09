using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerComponents
{
    public abstract class FileComponentBaseGraph : InstallerComponentBase
    {
        protected FileComponentBaseGraph(string name, IDictionary<string, string> values, ComponentType componentType) : base(name, values, componentType)
        {
        }

        public string DeploymentType { get; set; }
        public string MsdeployExe { get; set; }

        public string SourceContentPath { get; set; }
        public string DestinationContentPath { get; set; }
        public string DestinationComputerName { get; set; }
        public string DestinationUserName { get; set; }
        public string DestinationPassword { get; set; }
    }
}
