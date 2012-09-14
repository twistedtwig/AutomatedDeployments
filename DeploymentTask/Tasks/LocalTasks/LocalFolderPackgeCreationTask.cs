using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalFolderPackgeCreationTask : FolderPackageCreationTaskBase
    {
        public LocalFolderPackgeCreationTask(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Local folder package creation Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            return InvokeMsDeploy();
        }
    }
}