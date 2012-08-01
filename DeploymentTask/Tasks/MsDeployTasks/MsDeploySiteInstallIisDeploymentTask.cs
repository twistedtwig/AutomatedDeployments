using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeploySiteInstallIisDeploymentTask : MsDeployInstallIisDeploymentTaskBase
    {
        public MsDeploySiteInstallIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "MsDeploy Task To Install a Site"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return " add site /IN < " + Path.Combine(parameter, ActionComponentGraph.PathToConfigFile);
        }

        protected override string CmdFileName { get { return "CreateSite"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return ActionComponentGraph.DestinationContentPath; } }
    }
}
