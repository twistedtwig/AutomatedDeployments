using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeploymentConfiguration.DeploymentStrategies;

namespace DeploymentTask.Factories
{
    public abstract class RegionDeploymentTaskFactoryBase<T> where T : DeploymentStrategyComponentGraphBase
    {

        public DeploymentTaskCollection Create(bool breakOnError, T deploymentStrategyComponentGraph)
        {
            if (deploymentStrategyComponentGraph == null) throw new ArgumentNullException("deploymentStrategyComponentGraph");

            DeploymentTaskCollection deploymentTaskCollection = new DeploymentTaskCollection(breakOnError);
            if (!deploymentStrategyComponentGraph.Actions.Any()) return deploymentTaskCollection;

            CreateTasks(deploymentTaskCollection, deploymentStrategyComponentGraph);

            return deploymentTaskCollection;
        }

        protected abstract void CreateTasks(DeploymentTaskCollection deploymentTaskCollection, T deploymentComponentGraph);
    }
}
