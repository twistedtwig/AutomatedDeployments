using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class AppPoolRemovalTaskBase : IisDeploymentTaskBase
    {
        protected AppPoolRemovalTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
            if (string.IsNullOrWhiteSpace(actionComponentGraph.AppCmdExe))
            {
                throw new ArgumentNullException("AppCmdExe");
            }

            if (string.IsNullOrWhiteSpace(actionComponentGraph.PathToConfigFile))
            {
                throw new ArgumentNullException("PathToConfigFile");
            }
        }
        
        protected override string CreateParameterString(string destinationConfigPath)
        {
            throw new NotImplementedException();
        }
    }
}
