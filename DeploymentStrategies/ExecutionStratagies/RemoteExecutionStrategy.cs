using InstallerComponents;

namespace DeploymentStrategies.ExecutionStratagies
{
    public abstract class RemoteExecutionStrategy : DeploymentStrategy
    {
        protected RemoteExecutionComponentGraph ComponentGraph;

        protected RemoteExecutionStrategy(RemoteExecutionComponentGraph componentGraph, RemoteDeploymentStrategyType strategyType) : base(strategyType, componentGraph.Index)
        {
            ComponentGraph = componentGraph;
        }
    }
}
