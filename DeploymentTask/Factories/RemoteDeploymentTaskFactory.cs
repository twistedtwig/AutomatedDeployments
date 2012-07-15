using System;
using System.IO;
using DeploymentConfiguration.Actions;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask.Tasks;

namespace DeploymentTask.Factories
{
    public class RemoteDeploymentTaskFactory : RegionDeploymentTaskFactoryBase<RemoteDeploymentStrategyComponentGraphBase>
    {

        protected override void CreateTasks(DeploymentTaskCollection deploymentTaskCollection, RemoteDeploymentStrategyComponentGraphBase deploymentComponentGraph)
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
                        deploymentTaskCollection.DeploymentTasks.Add(new MsDeployAppPoolInstallTask(action as IisActionComponentGraph));
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
