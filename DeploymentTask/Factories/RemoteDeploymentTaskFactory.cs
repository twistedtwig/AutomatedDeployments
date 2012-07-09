using System;
using System.Linq;
using DeploymentConfiguration.Actions;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask.Tasks;

namespace DeploymentTask.Factories
{
    public class RemoteDeploymentTaskFactory
    {
        public DeploymentTaskCollection Create(bool breakOnError, RemoteDeploymentStrategyComponentGraphBase deploymentStrategyComponentGraph)
        {
            if (deploymentStrategyComponentGraph == null) throw new ArgumentNullException("deploymentStrategyComponentGraph");

            DeploymentTaskCollection deploymentTaskCollection = new DeploymentTaskCollection(breakOnError);
            if (!deploymentStrategyComponentGraph.Actions.Any()) return deploymentTaskCollection;

            CreateTasks(deploymentTaskCollection, deploymentStrategyComponentGraph);

            return deploymentTaskCollection;
        }

        private static void CreateTasks(DeploymentTaskCollection deploymentTaskCollection, RemoteDeploymentStrategyComponentGraphBase deploymentComponentGraph)
        {
            foreach (ActionComponentGraphBase action in deploymentComponentGraph.Actions)
            {
                //Create all tasks for each action.
                switch (action.ActionType)
                {
                    case ActionType.FileDeployment:
                        deploymentTaskCollection.DeploymentTasks.Add(new MsDeployFileCopyDeploymentTask(action as FileCopyActionComponentGraph));
                        break;
                    case ActionType.AppPoolCreation:
                        break;
                    case ActionType.AppPoolRemoval:
                        break;
                    case ActionType.WebSiteCreation:
                        break;
                    case ActionType.WebsiteRemoval:
                        break;
                    case ActionType.AppCreation:
                        break;
                    case ActionType.AppRemoval:
                        break;
                    case ActionType.ApplicationExecution:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
