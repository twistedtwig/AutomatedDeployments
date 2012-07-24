using System;
using DeploymentConfiguration;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask;
using DeploymentTask.Factories;

namespace IISDeployments
{
    internal class Program
    {
        private static void Main()
        {
            DeploymentStrategyComponentGraphBase localdeploymentComponentGraph = ConfigurationLoader.Load("localtestSetup", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            DeploymentTaskCollection localdeploymentTaskCollection = DeploymentTaskFactory.Create(localdeploymentComponentGraph);
            localdeploymentTaskCollection.Execute();




            DeploymentStrategyComponentGraphBase remotedeploymentComponentGraph = ConfigurationLoader.Load("remotetestSetup", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            DeploymentTaskCollection remotedeploymentTaskCollection = DeploymentTaskFactory.Create(remotedeploymentComponentGraph);
            remotedeploymentTaskCollection.Execute();
        }
    }
}