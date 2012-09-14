using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployFolderPackgeCreationTask : FolderPackageCreationTaskBase
    {
        public MsDeployFolderPackgeCreationTask(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "remote folder package creation Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            int result = InvokeMsDeploy();
            if (result != ExpectedReturnValue) return result;

            return  new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, ActionComponentGraph.OutputLocation, GetFullDestinationZipFilePath())).Execute();
        }
    }
}