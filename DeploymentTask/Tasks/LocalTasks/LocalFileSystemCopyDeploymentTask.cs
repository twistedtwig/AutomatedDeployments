using System;
using System.IO;
using DeploymentConfiguration.Actions;
using DeploymentTask.Exceptions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalFileSystemCopyDeploymentTask : FileDeploymentTaskBase
    {
        public LocalFileSystemCopyDeploymentTask(FileCopyActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine("Executing local file copy command:");

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
                FileHelper.CopyContents(ActionComponentGraph.SourceContentPath, ActionComponentGraph.DestinationContentPath);
            }
            catch (Exception exception)
            {                
                throw new DeploymentTaskException(exception.Message, -1, exception);
            }

            Console.WriteLine("Completed file copy.");
            Console.WriteLine(EndSectionBreaker);

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
            Directory.Delete(ActionComponentGraph.DestinationContentPath, true);
        }
    }
}