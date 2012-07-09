using System.IO;
using InstallerComponents;
using DeploymentStrategies.ExecutionStratagies;

namespace DeploymentStrategies.IisRemoval
{
    public class AppPoolRemover : IisTaskRemoval
    {
        public AppPoolRemover(IisRemoverComponentGraph iisRemoverComponent) : base(iisRemoverComponent, RemoteDeploymentStrategyType.AppPoolRemoval)
        {
        }

        public override void Execute()
        {
            throw new System.NotImplementedException("The app Pool Creation Task should not be Executed, it returns a new object after creating the requird files.");
        }

        public override string GetPathToRemovalFile()
        {
            return Path.Combine(IisRemoverComponent.LocalInstallFolder, "appPoolRemovalScript.cmd");
        }

        public override RemoteExecutionStrategy CreateDeploymentStrategy()
        {
            return CreateDeploymentStrategyBase("delete apppool " + IisRemoverComponent.ItemToRemove);
        }
    }
}