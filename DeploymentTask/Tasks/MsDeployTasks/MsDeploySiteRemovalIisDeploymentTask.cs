using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeploySiteRemovalIisDeploymentTask : MsDeployRemovalIisDeploymentTaskBase
    {
        private const string Pattern = @"(SITE.NAME=(['''''',""""""]){0,1}(([\-a-z-A-Z0-9_/.]|\s)+)\2)";
        private readonly Regex Regex = new Regex(Pattern, RegexOptions.Compiled);

        public MsDeploySiteRemovalIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "MsDeploy Task To Remove a Site"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return string.Format(" delete site \"{0}\"", parameter);
        }

        protected override string CmdFileName { get { return "RemoveSite"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return ActionComponentGraph.DestinationContentPath; } }
        protected override Regex ConfigFileNamePattern { get { return Regex; } }
    }
}