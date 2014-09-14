namespace DeploymentConfiguration.Actions
{
    public class RemoteDetailsComponentGraph : ActionComponentGraphBase
    {
        public string DestinationUserName { get; set; }
        public string DestinationPassword { get; set; }
        public string AuthType { get; set; }
    }
}