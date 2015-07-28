namespace DeploymentConfiguration.Actions
{
    public class ApplicationExecutionDeploymentComponentGraph : ActionComponentGraphBase
    {
        public ApplicationExecutionDeploymentComponentGraph()
        {          
            WaitInterval = int.MinValue;
        }

        public bool RequiresAdminRights { get; set; }
        public int WaitInterval { get; set; }
    }
}