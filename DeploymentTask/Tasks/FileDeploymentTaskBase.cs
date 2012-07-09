using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class FileDeploymentTaskBase : DeploymentTaskBase<FileCopyActionComponentGraph>
    {
        protected FileDeploymentTaskBase(FileCopyActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {

        }

        protected bool CheckSourceExists()
        {
            return Directory.Exists(ActionComponentGraph.SourceContentPath);
        }
        
    }
}