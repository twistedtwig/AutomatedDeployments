using System;
using DeploymentConfiguration.DeploymentStrategies;

namespace DeploymentTask.Factories
{
    public class DeploymentTaskFactory
    {
        public static DeploymentTaskCollection Create(DeploymentStrategyComponentGraphBase deploymentComponentGraph)
        {
            return Create(false, deploymentComponentGraph);
        }


        /// <summary>
        /// Will iterate through the deploymentComponentGraph and create all required tasks and add them to a DeploymentTaskCollection
        /// </summary>
        /// <param name="breakOnError"> </param>
        /// <param name="deploymentComponentGraph"></param>
        /// <returns></returns>
        public static DeploymentTaskCollection Create(bool breakOnError, DeploymentStrategyComponentGraphBase deploymentComponentGraph)
        {
            if (deploymentComponentGraph is LocalDeploymentStrategyComponentGraphBase)
            {
                return new LocalRegionDeploymentTaskFactory().Create(breakOnError, deploymentComponentGraph as LocalDeploymentStrategyComponentGraphBase);
            }

            if (deploymentComponentGraph is RemoteDeploymentStrategyComponentGraphBase)
            {
                return new RemoteDeploymentTaskFactory().Create(breakOnError, deploymentComponentGraph as RemoteDeploymentStrategyComponentGraphBase);
            }

            throw new ArgumentOutOfRangeException("deploymentComponentGraph");
        }

    }
}
