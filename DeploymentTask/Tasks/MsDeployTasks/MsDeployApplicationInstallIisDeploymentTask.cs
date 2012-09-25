using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployApplicationInstallIisDeploymentTask : MsDeployInstallIisDeploymentTaskBase
    {
        public MsDeployApplicationInstallIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "MsDeploy Task To Install Application"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return " add app /IN < " + FileHelper.MapRelativePath(parameter, ActionComponentGraph.PathToConfigFile);
        }

        protected override string CmdFileName { get { return "CreateApplication"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return ActionComponentGraph.DestinationContentPath; } }
    }
}