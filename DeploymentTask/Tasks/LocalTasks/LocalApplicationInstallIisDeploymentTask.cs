using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalApplicationInstallIisDeploymentTask : LocalInstallIisDeploymentTaskBase
    {
        public LocalApplicationInstallIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        protected override string CmdFileName { get { return "localApplicationCmd"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return  ActionComponentGraph.SourceContentPath; } }

        public override string DisplayName { get { return "Local Task To Install Application"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return " add app /IN < \"" + FileHelper.MapRelativePath(parameter, ActionComponentGraph.PathToConfigFile) + "\"";
        }
    }
}