using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployAppPoolInstallIisDeploymentTask : MsDeployInstallIisDeploymentTaskBase
    {
        public MsDeployAppPoolInstallIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "MsDeploy Task To Install App Pool"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return " add apppool /IN < " + FileHelper.MapRelativePath(parameter, ActionComponentGraph.PathToConfigFile);
        }

        protected override string CmdFileName { get { return "CreateAppPool"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return ActionComponentGraph.DestinationContentPath; } }
    }
}