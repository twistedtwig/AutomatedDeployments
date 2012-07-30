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

        public int CompareFileComponentGraph(FileDeploymentTaskBase other)
        {
            if (other== null) return -1;
            int value = 0;
            value = ActionComponentGraph.DestinationComputerName.CompareTo(other.ActionComponentGraph.DestinationComputerName);
            if (value != 0) return value;

            value = ActionComponentGraph.SourceContentPath.CompareTo(other.ActionComponentGraph.SourceContentPath);
            if (value != 0) return value;

            return ActionComponentGraph.DestinationContentPath.CompareTo(other.ActionComponentGraph.DestinationContentPath);
        }

        public override bool RequiresAdminRights { get { return false; }
        }
        
    }
}