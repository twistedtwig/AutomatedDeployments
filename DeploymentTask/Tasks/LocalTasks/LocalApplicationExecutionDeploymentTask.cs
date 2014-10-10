using System;
using DeploymentConfiguration.Actions;
using DeploymentTask.Exceptions;
using Logging;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalApplicationExecutionDeploymentTask : DeploymentTaskBase<ApplicationExecutionDeploymentComponentGraph>
    {
        private static readonly Logger Logger = Logger.GetLogger();

        public LocalApplicationExecutionDeploymentTask(ApplicationExecutionDeploymentComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            Logger.Log(StartSectionBreaker);
            Logger.Log("Executing local applicaiton execution command:");

            try
            {
                Logger.Log(string.Format("running application: '{0}'", ActionComponentGraph.SourceContentPath));
                InvokeExe(ActionComponentGraph.SourceContentPath, "");
            }
            catch (Exception exception)
            {                
                throw new DeploymentTaskException(exception.Message, -1, exception);
            }

            Logger.Log("Completed application execution.");
            Logger.Log(EndSectionBreaker);

            return 0;
        }

        public override string DisplayName
        {
            get { return "Local application execution"; }
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