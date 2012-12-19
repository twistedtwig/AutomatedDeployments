using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;
using Logging;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployFilePermissionDeploymentTask : FilePermissionDeploymentTaskBase
    {
        private static Logger logger = Logger.GetLogger();

        public MsDeployFilePermissionDeploymentTask(SetFilePermissionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            logger.Log("Executing MSDEPLOY file permission task", LoggingLevel.Verbose);
            //need msdeploy path
            string msdeployPath = FindFirstValidFileFromList(ActionComponentGraph.MsDeployExeLocations, "MSDEPLOY", true);
            
            string fileName = CreateRandomFileName(CmdFileName, CmdFileNameExtension);
            string filePath = FileHelper.MapRelativePath(ActionComponentGraph.SourceContentPath, fileName);
            CreateFile(filePath, GetExe() + " " + GetParameters(), true);
            //ensure cmd file has been pushed to remote server.
            new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, fileName)).Execute();

            // call msdeploy to execute appcmd file on remote machine
            int result = InvokeExe(msdeployPath, MsDeployTaskExtensions.GetMsDeployExecuteCmdParams(ActionComponentGraph, FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, fileName)));

            // clean up local and remote files
            if (ActionComponentGraph.CleanUp)
            {
                //delete local file
                File.Delete(filePath);

                //delete remote file
                int tempResult = InvokeExe(msdeployPath, MsDeployTaskExtensions.GetMsDeployDeleteFileParams(ActionComponentGraph, FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, fileName)));
                if (tempResult != 0) result = tempResult;
            }

            logger.Log(string.Format("Completed {0}.", DisplayName));
            logger.Log(EndSectionBreaker);

            return result;
        }
        
        public override string DisplayName
        {
            get { return "MsDeploy Remote File permission"; }
        }

        protected string CmdFileName { get { return "SetPermission"; } }
        protected string CmdFileNameExtension { get { return "cmd"; } }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }
    }
}