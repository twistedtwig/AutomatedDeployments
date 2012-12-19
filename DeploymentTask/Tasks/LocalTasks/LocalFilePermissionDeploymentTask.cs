using System;
using DeploymentConfiguration.Actions;
using DeploymentTask.Exceptions;
using Logging;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalFilePermissionDeploymentTask : FilePermissionDeploymentTaskBase
    {
        private static Logger logger = Logger.GetLogger();

        public LocalFilePermissionDeploymentTask(SetFilePermissionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            logger.Log(StartSectionBreaker);
            logger.Log("Executing local file permission command:");

            if(!CheckSourceExists())
            {
                throw new DeploymentTaskException(string.Format("Source folder not found, '{0}'", ActionComponentGraph.Folder), -1);
            }
           
            try
            {
                InvokeExe(GetExe(), GetParameters());
            }
            catch (Exception exception)
            {                
                throw new DeploymentTaskException(exception.Message, -1, exception);
            }

            logger.Log("Completed file permission.");
            logger.Log(EndSectionBreaker);

            return 0;
        }

        public override string DisplayName
        {
            get { return "Local File System Peermission"; }
        }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }    
    }
}