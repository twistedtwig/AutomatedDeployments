using System.Collections.Generic;
using DeploymentConfiguration.Actions;

namespace DeploymentConfiguration.DeploymentStrategies
{
    public abstract class DeploymentStrategyComponentGraphBase
    {
        protected DeploymentStrategyComponentGraphBase()
        {
            Actions = new List<ActionComponentGraphBase>();
            ShouldSortTasks = false;
        }

        public string AppCmdExe { get; set; }
        public string DestinationComputerName { get; set; }
        public bool ShouldSortTasks { get; set; }

        public IList<ActionComponentGraphBase> Actions { get; set; }
    }
}
