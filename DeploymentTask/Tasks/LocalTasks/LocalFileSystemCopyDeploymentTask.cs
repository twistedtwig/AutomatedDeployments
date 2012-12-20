using System;
using System.IO;
using DeploymentConfiguration.Actions;
using DeploymentTask.Exceptions;
using FileSystem.Helper;
using Logging;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalFileSystemCopyDeploymentTask : FileDeploymentTaskBase
    {
        private static Logger logger = Logger.GetLogger();

        public LocalFileSystemCopyDeploymentTask(FileCopyActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            logger.Log(StartSectionBreaker);
            logger.Log("Executing local file copy command:");

            if(!CheckSourceExists())
            {
                throw new DeploymentTaskException(string.Format("Source folder not found, '{0}'", ActionComponentGraph.SourceContentPath), -1);
            }

            if (ActionComponentGraph.ForceInstall)
            {
                DeleteDestination();
            }

            try
            {
                logger.Log(string.Format("Copying Files from: '{0}' to '{1}'", ActionComponentGraph.SourceContentPath, ActionComponentGraph.DestinationContentPath));
                FileHelper.CopyContents(ActionComponentGraph.SourceContentPath, ActionComponentGraph.DestinationContentPath);
            }
            catch (Exception exception)
            {                
                throw new DeploymentTaskException(exception.Message, -1, exception);
            }

            logger.Log("Completed file copy.");
            logger.Log(EndSectionBreaker);

            return 0;
        }

        public override string DisplayName
        {
            get { return "Local File System Copy"; }
        }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }



        protected void DeleteDestination()
        {
            logger.Log(string.Format("Deleting Files from: '{0}'", ActionComponentGraph.DestinationContentPath));
            Directory.Delete(ActionComponentGraph.DestinationContentPath, true);
        }
    }
}