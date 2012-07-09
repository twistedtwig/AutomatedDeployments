using System;
using System.Collections.Generic;
using InstallerComponents;
using DeploymentStrategies.FileDeployments;
using DeploymentStrategies.IisCreation;
using DeploymentStrategies.IisRemoval;

namespace DeploymentStrategies
{
    public class DeploymentStrategyFactory
    {
        public static IList<DeploymentStrategy> Create(ComponentBase installerComponent)
        {
            List<DeploymentStrategy> strategies = new List<DeploymentStrategy>();
            DeploymentStrategy strategy = null;
            switch (installerComponent.ComponentTypeEnum)
            {                    
                case ComponentType.FileInstaller:
                case ComponentType.FileRemover:
                    if (installerComponent is FileComponentBaseGraph)
                    {
                        strategy = FileDeploymentFactory.Create(installerComponent as FileComponentBaseGraph);                        
                    }
                    break;

                case ComponentType.AppPoolInstaller:
                    IisInstallerComponentBaseGraph iisInstallerComponent = installerComponent as IisInstallerComponentBaseGraph;
                    if (iisInstallerComponent == null)
                    {
                        break;
                    }
                    //create the cmd file to be able to execute it remotely, then create the action to actually perform the remote execution.
                    AppPoolCreationTask appPoolCreationTask = new AppPoolCreationTask(iisInstallerComponent);
                    strategies.AddRange(appPoolCreationTask.GetExtraRequiredStrategies());
                    strategy = appPoolCreationTask.CreateDeploymentStrategy();
                    break;
                    
                case ComponentType.WebSiteInstaller:
                    break;
                case ComponentType.VdirInstaller:
                    break;

                case ComponentType.AppPoolRemover:
                    IisRemoverComponentGraph appPoolRemoveComponent = installerComponent as IisRemoverComponentGraph;
                    if (appPoolRemoveComponent == null)
                    {
                        break;
                    }

                    AppPoolRemover appPoolRemover = new AppPoolRemover(appPoolRemoveComponent);
                    strategies.AddRange(appPoolRemover.GetExtraRequiredStrategies());
                    strategy = appPoolRemover.CreateDeploymentStrategy();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (strategy != null)
            {
                strategies.Add(strategy);
                strategies.AddRange(strategy.GetExtraRequiredStrategies());
            }
            return strategies;
        }
    }
}
