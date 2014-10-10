namespace DeploymentConfiguration.Actions
{
    public class ApplicationExecutionDeploymentComponentGraph : ActionComponentGraphBase
    {
        public ApplicationExecutionDeploymentComponentGraph()
        {
        }

        public bool RequiresAdminRights { get; set; }
    }
}