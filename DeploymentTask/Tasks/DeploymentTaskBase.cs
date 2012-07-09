using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class DeploymentTaskBase<T> : DeploymentTaskRoot where T : ActionComponentGraphBase
    {
        protected T ActionComponentGraph;

        protected DeploymentTaskBase(T actionComponentGraph)
        {
            ActionComponentGraph = actionComponentGraph;
        }

        
    }
}
