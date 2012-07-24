using System;
using DeploymentConfiguration.Actions;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask.Tasks.LocalTasks;

namespace DeploymentTask.Factories
{
    public class LocalRegionDeploymentTaskFactory : RegionDeploymentTaskFactoryBase<LocalDeploymentStrategyComponentGraphBase>
    {

        protected override void CreateTasks(DeploymentTaskCollection deploymentTaskCollection, LocalDeploymentStrategyComponentGraphBase deploymentComponentGraph)
        {
            foreach (ActionComponentGraphBase action in deploymentComponentGraph.Actions)
            {
                //Create all tasks for each action.
                switch (action.ActionType)
                {
                    case ActionType.FileDeployment:
                        deploymentTaskCollection.Add(new LocalFileSystemCopyDeploymentTask(action as FileCopyActionComponentGraph));
                        break;
                    case ActionType.AppPoolCreation:
                        if (action.ForceInstall)
                        {
                            deploymentTaskCollection.Add(new LocalAppPoolRemovalIisDeploymentTask(action as IisActionComponentGraph));
                            deploymentTaskCollection.Add(new LocalAppPoolRemovalIisDeploymentTask(action as IisActionComponentGraph));
                        }
                        deploymentTaskCollection.Add(new LocalAppPoolInstallIisDeploymentTask(action as IisActionComponentGraph));
                        break;
                    case ActionType.AppPoolRemoval:
                        deploymentTaskCollection.Add(new LocalAppPoolRemovalIisDeploymentTask(action as IisActionComponentGraph));                        
                        break;
                    case ActionType.WebSiteCreation:
                        break;
                    case ActionType.WebsiteRemoval:
                        break;
                    case ActionType.AppCreation:
                        deploymentTaskCollection.Add(new LocalApplicationInstallIisDeploymentTask(action as IisActionComponentGraph));                                                
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
