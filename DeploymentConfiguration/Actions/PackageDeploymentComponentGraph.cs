namespace DeploymentConfiguration.Actions
{
    public class PackageDeploymentComponentGraph : ActionComponentGraphBase
    {
        public PackageDeploymentComponentGraph()
        {
            AllowUseOfDestinationFolder = false;
        }

        public bool AllowUseOfDestinationFolder { get; set; }
        public bool TakeIisDown { get; set; }

    }
}