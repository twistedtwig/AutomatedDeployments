using System;
using DeploymentConfiguration.Actions;
using Logging;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployProjectPackgeDeploymentTask : ProjectPackageDeploymentTaskBase
    {
        private static Logger logger = Logger.GetLogger();

        public MsDeployProjectPackgeDeploymentTask(PackageDeploymentComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Remote project file package Deployment Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            logger.Log(StartSectionBreaker);
            logger.Log("Executing MSDEPLOY package deployment:");

            //take the zipped package file, 
            if (!CheckZipPackageFileExists()) { return -1; }
            int result = ExpectedReturnValue;

            UnZipFileToTempLocation();
            
            string finalPackageLocation = FindPackageFileRootLocation();
            logger.Log(string.Format("Copying package from '{0}' to '{1}'", finalPackageLocation, DestinationPath));
            //copy stuff to remote server... take whole folder.
            result = new MsDeployFileCopyDeploymentTask(CreateFolderCopyActionComponentGraphFrom(ActionComponentGraph, finalPackageLocation, DestinationPath)).Execute();

            //perform clean up at the end.
            RegisterForCleanUpTempLocation();
            logger.Log("Finished Deploying package.");
            logger.Log(EndSectionBreaker);
                
            return result;
        }
    }
}