using DeploymentConfiguration.Actions;
using Logging;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployApplicationExecutionDeploymentTask : DeploymentTaskBase<ApplicationExecutionDeploymentComponentGraph>
    {
        private static readonly Logger Logger = Logger.GetLogger();

        public MsDeployApplicationExecutionDeploymentTask(ApplicationExecutionDeploymentComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            Logger.Log(string.Format("Executing {0} task", DisplayName));
            //need msdeploy path
            string msdeployPath = FindFirstValidFileFromList(ActionComponentGraph.MsDeployExeLocations, "MSDEPLOY", true);


            // call msdeploy to execute application file on remote machine
            var msDeployExecuteCmdParams = MsDeployTaskExtensions.GetMsDeployExecuteCmdParams(ActionComponentGraph, ActionComponentGraph.SourceContentPath, ActionComponentGraph.WaitInterval);


            int result = InvokeExe(msdeployPath, msDeployExecuteCmdParams);

            // clean up local and remote files
            if (ActionComponentGraph.CleanUp)
            {
                //delete remote file
                int tempResult = InvokeExe(msdeployPath, MsDeployTaskExtensions.GetMsDeployDeleteFileParams(ActionComponentGraph, ActionComponentGraph.SourceContentPath));
                if (tempResult != 0) result = tempResult;
            }

            Logger.Log(string.Format("Completed {0}.", DisplayName));
            Logger.Log(EndSectionBreaker);

            return result;
        }

        public override string DisplayName
        {
            get { return "remote application execution"; }
        }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }

        public override bool RequiresAdminRights
        {
            get { return ActionComponentGraph.RequiresAdminRights; }
        }
    }
}