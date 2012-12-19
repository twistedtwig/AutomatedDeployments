using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class FilePermissionDeploymentTaskBase : DeploymentTaskBase<SetFilePermissionComponentGraph>
    {
        protected FilePermissionDeploymentTaskBase(SetFilePermissionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {

        }

        protected bool CheckSourceExists()
        {
            return Directory.Exists(ActionComponentGraph.Folder);
        }
       
        public override bool RequiresAdminRights
        {
            get { return true; }
        }

        protected string GetExe()
        {
            return "icacls";
        }

        protected string GetParameters()
        {
            return string.Format("\"{0}\" /grant \"{1}\":(OI)(CI)F", ActionComponentGraph.Folder, ActionComponentGraph.UserName);
        }
    }
}