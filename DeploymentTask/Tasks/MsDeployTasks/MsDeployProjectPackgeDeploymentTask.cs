using System;
using System.IO;
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

            var appOffLineFileTempPath = string.Empty;
            var appOffLineFileFinalPath = string.Empty;
            //take the site down whilst deploying the files
            if (ActionComponentGraph.TakeIisDown)
            {
                appOffLineFileTempPath = CreateAppOffLineFile();
                appOffLineFileFinalPath = Path.Combine(DestinationPath, AppOffLineFileName);
                result = new MsDeployFileCopyDeploymentTask(CreateFolderCopyActionComponentGraphFrom(ActionComponentGraph, appOffLineFileTempPath, appOffLineFileFinalPath)).Execute();
            }
           
            
            string finalPackageLocation = FindPackageFileRootLocation();
            logger.Log(string.Format("Copying package from '{0}' to '{1}'", finalPackageLocation, DestinationPath));
            //copy stuff to remote server... take whole folder.
            result = new MsDeployFileCopyDeploymentTask(CreateFolderCopyActionComponentGraphFrom(ActionComponentGraph, finalPackageLocation, DestinationPath)).Execute();

            //bring the site back online.
            if (!string.IsNullOrWhiteSpace(appOffLineFileFinalPath))
            {
                string msdeployPath = FindFirstValidFileFromList(ActionComponentGraph.MsDeployExeLocations, "MSDEPLOY", true);
                int tempResult = InvokeExe(msdeployPath, MsDeployTaskExtensions.GetMsDeployDeleteFileParams(ActionComponentGraph, appOffLineFileFinalPath));
                if (tempResult != 0) result = tempResult;
            }

            //perform clean up at the end.
            RegisterForCleanUpTempLocation();
            logger.Log("Finished Deploying package.");
            logger.Log(EndSectionBreaker);
                
            return result;
        }
    }
}