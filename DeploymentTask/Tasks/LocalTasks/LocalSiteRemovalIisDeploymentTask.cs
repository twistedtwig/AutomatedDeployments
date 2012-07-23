using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalSiteRemovalIisDeploymentTask : LocalRemovalIisDeploymentTaskBase
    {
        public LocalSiteRemovalIisDeploymentTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName
        {
            get { throw new System.NotImplementedException(); }
        }

        public override int ExpectedReturnValue
        {
            get { throw new System.NotImplementedException(); }
        }

        protected override string CreateParameterString(string parameter)
        {
            throw new System.NotImplementedException();
        }

        protected override string CmdFileName
        {
            get { throw new System.NotImplementedException(); }
        }

        protected override string CmdFileNameExtension
        {
            get { throw new System.NotImplementedException(); }
        }

        protected override string CmdFileNameExe
        {
            get { throw new System.NotImplementedException(); }
        }

        protected override string CmdFileParameterDestinationPath
        {
            get { throw new System.NotImplementedException(); }
        }

        protected override Regex ConfigFileNamePattern
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}