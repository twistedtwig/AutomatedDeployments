using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalSiteInstallIisDeploymentTask : LocalInstallIisDeploymentTaskBase
    {
        public LocalSiteInstallIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        protected override string CmdFileName { get { return "localSiteCmd"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return  ActionComponentGraph.SourceContentPath; } }

        public override string DisplayName { get { return "Local Task To Install a Site"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return " add site /IN < " + FileHelper.MapRelativePath(parameter, ActionComponentGraph.PathToConfigFile);
        }
    }
}
