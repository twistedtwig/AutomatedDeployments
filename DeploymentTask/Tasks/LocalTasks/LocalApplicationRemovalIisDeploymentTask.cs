using System;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalApplicationRemovalIisDeploymentTask : LocalRemovalIisDeploymentTaskBase
    {
        private const string Pattern = @"(APP.NAME=(['''''',""""""]){0,1}(([\-a-z-A-Z0-9_/.]|\s)+)\2)";
        private readonly Regex Regex = new Regex(Pattern, RegexOptions.Compiled);

        public LocalApplicationRemovalIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Local Task To Remove an Appliction"; } }
        public override int ExpectedReturnValue { get { return 0; } }

        protected override string CreateParameterString(string parameter)
        {
            return string.Format(" delete app \"{0}\"", parameter);
        }
        
        protected override string CmdFileName { get { return "RemoveApplication"; } }
        protected override string CmdFileNameExtension { get { return "cmd"; } }
        protected override string CmdFileNameExe { get { return ActionComponentGraph.AppCmdExe; } }
        protected override string CmdFileParameterDestinationPath { get { return ActionComponentGraph.DestinationContentPath; } }
        protected override Regex ConfigFileNamePattern { get { return Regex; } }
    }
}