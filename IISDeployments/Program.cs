using System;
using DeploymentConfiguration;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask;
using DeploymentTask.Factories;

namespace IISDeployments
{
    class Program
    {
        static void Main()
        {
            DeploymentStrategyComponentGraphBase localdeploymentComponentGraph = ConfigurationLoader.Load("localtestSetup", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            DeploymentTaskCollection localdeploymentTaskCollection = DeploymentTaskFactory.Create(localdeploymentComponentGraph);
            localdeploymentTaskCollection.Execute();




            DeploymentStrategyComponentGraphBase remotedeploymentComponentGraph = ConfigurationLoader.Load("remotetestSetup", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            DeploymentTaskCollection remotedeploymentTaskCollection = DeploymentTaskFactory.Create(remotedeploymentComponentGraph);
            remotedeploymentTaskCollection.Execute();



//            IList<ComponentGraph> installerComponentGraphs = ConfigurationController.LoadInstallerComponentGraphs();
//
//            foreach (ComponentGraph componentGraph in installerComponentGraphs)
//            {
//                List<RemoteDeploymentStrategy> deploymentTasks = new List<RemoteDeploymentStrategy>();
//                foreach (var installerComponent in componentGraph.Components)
//                {
//                    deploymentTasks.AddRange(RemoteDeploymentStrategyFactory.Create(installerComponent));
//                }
//
//                deploymentTasks.Sort();
//
//                //created all tasks and files for this deployment, now execute tasks in order.
//                foreach (RemoteDeploymentStrategy task in deploymentTasks)
//                {
//                    task.Execute();
//                }                
//            }
        }
    }
}
