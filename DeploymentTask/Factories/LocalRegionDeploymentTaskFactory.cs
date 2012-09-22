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
                        }
                        deploymentTaskCollection.Add(new LocalAppPoolInstallIisDeploymentTask(action as IisActionComponentGraph));
                        break;
                    case ActionType.AppPoolRemoval:
                        deploymentTaskCollection.Add(new LocalAppPoolRemovalIisDeploymentTask(action as IisActionComponentGraph));                        
                        break;
                    case ActionType.WebSiteCreation:
                        if (action.ForceInstall)
                        {
                            deploymentTaskCollection.Add(new LocalSiteRemovalIisDeploymentTask(action as IisActionComponentGraph));                            
                        }
                        deploymentTaskCollection.Add(new LocalSiteInstallIisDeploymentTask(action as IisActionComponentGraph));                                                
                        break;
                    case ActionType.WebsiteRemoval:
                        deploymentTaskCollection.Add(new LocalSiteRemovalIisDeploymentTask(action as IisActionComponentGraph));                            
                        break;
                    case ActionType.AppCreation:
                        if (action.ForceInstall)
                        {
                            deploymentTaskCollection.Add(new LocalApplicationRemovalIisDeploymentTask(action as IisActionComponentGraph));                            
                        }
                        deploymentTaskCollection.Add(new LocalApplicationInstallIisDeploymentTask(action as IisActionComponentGraph));                                                
                        break;
                    case ActionType.AppRemoval:
                        deploymentTaskCollection.Add(new LocalApplicationRemovalIisDeploymentTask(action as IisActionComponentGraph));                            
                        break;
                    case ActionType.CreatePackage:
                        deploymentTaskCollection.Add(PackageCreationTaskFactory.Create(action as PackageCreationComponentGraph, DeploymentType.local));
                        break;
                    case ActionType.DeployPackage:
                        deploymentTaskCollection.Add(new LocalProjectPackgeDeploymentTask(action as PackageDeploymentComponentGraph));
                        break;
                    case ActionType.ApplicationExecution:
                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        
    }
}
