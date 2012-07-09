using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public class MsDeployFileCopyDeploymentTask : FileDeploymentTaskBase
    {
        public MsDeployFileCopyDeploymentTask(FileCopyActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            throw new System.NotImplementedException();
        }

        public override string DisplayName
        {
            get { return "MsDeploy Remote File Copy"; }
        }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }
    }
}