using System;
using DeploymentConfiguration.Actions;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask.Tasks.MsDeployTasks;

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
                        deploymentTaskCollection.Add(new MsDeployFileCopyDeploymentTask(action as FileCopyActionComponentGraph));
                        break;
                    case ActionType.AppPoolCreation:
                        if (action.ForceInstall)
                        {
                            deploymentTaskCollection.Add(new MsDeployAppPoolRemovalIisDeploymentTask(action as IisActionComponentGraph));
                        }
                        deploymentTaskCollection.Add(new MsDeployAppPoolInstallIisDeploymentTask(action as IisActionComponentGraph));
                        break;
                    case ActionType.AppPoolRemoval:
                        deploymentTaskCollection.Add(new MsDeployAppPoolRemovalIisDeploymentTask(action as IisActionComponentGraph));
                        break;
                    case ActionType.WebSiteCreation:
                        if (action.ForceInstall)
                        {
                            deploymentTaskCollection.Add(new MsDeploySiteRemovalIisDeploymentTask(action as IisActionComponentGraph));
                        }
                        deploymentTaskCollection.Add(new MsDeploySiteInstallIisDeploymentTask(action as IisActionComponentGraph));
                        break;
                    case ActionType.WebsiteRemoval:
                        deploymentTaskCollection.Add(new MsDeploySiteRemovalIisDeploymentTask(action as IisActionComponentGraph));
                        break;
                    case ActionType.AppCreation:
                        if (action.ForceInstall)
                        {
                            deploymentTaskCollection.Add(new MsDeployApplicationRemovalIisDeploymentTask(action as IisActionComponentGraph));                            
                        }
                        deploymentTaskCollection.Add(new MsDeployApplicationInstallIisDeploymentTask(action as IisActionComponentGraph));
                        break;
                    case ActionType.AppRemoval:
                            deploymentTaskCollection.Add(new MsDeployApplicationRemovalIisDeploymentTask(action as IisActionComponentGraph));                            
                        break;
                    case ActionType.CreatePackage:
                        deploymentTaskCollection.Add(PackageCreationTaskFactory.Create(action as PackageCreationComponentGraph, DeploymentType.msDeploy));
                        break;
                    case ActionType.DeployPackage:
                        deploymentTaskCollection.Add(new MsDeployProjectPackgeDeploymentTask(action as PackageDeploymentComponentGraph));
                        break;
                    case ActionType.ApplicationExecution:
                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        
    }
}
