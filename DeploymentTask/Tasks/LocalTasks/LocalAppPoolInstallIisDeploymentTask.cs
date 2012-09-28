using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalAppPoolInstallIisDeploymentTask : LocalInstallIisDeploymentTaskBase
    {
        public LocalAppPoolInstallIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }
        
        protected override string CmdFileName { get { return "localAppPoolCmd"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return ActionComponentGraph.SourceContentPath; } }

        public override string DisplayName { get { return "Local Task To Install App Pool"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return " add apppool /IN < \"" + FileHelper.MapRelativePath(parameter, ActionComponentGraph.PathToConfigFile) + "\"";
        }
    }
}