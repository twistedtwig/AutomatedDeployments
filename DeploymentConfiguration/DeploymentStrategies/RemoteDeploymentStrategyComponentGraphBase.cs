namespace DeploymentConfiguration.DeploymentStrategies
{
    public class RemoteDeploymentStrategyComponentGraphBase : DeploymentStrategyComponentGraphBase
    {
        public string MsDeployExe { get; set; }
        public string DestinationUserName { get; set; }
        public string DestinationPassword { get; set; }
    
    }
}