namespace DeploymentConfiguration.Actions
{
    public class PackageDeploymentComponentGraph : ActionComponentGraphBase
    {
        public PackageDeploymentComponentGraph()
        {
            AllowUseOfDestinationFolder = false;
        }

        public bool AllowUseOfDestinationFolder { get; set; }

    }
}