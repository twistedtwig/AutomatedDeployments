using System;
using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;
using Logging;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public abstract class MsDeployInstallIisDeploymentTaskBase : IisDeploymentTaskBase
    {
        private static Logger logger = Logger.GetLogger();
        private readonly string MsdeployPath = string.Empty;

        protected MsDeployInstallIisDeploymentTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
            if (string.IsNullOrWhiteSpace(actionComponentGraph.AppCmdExe))
            {
                throw new ArgumentNullException("AppCmdExe");
            }

            if (string.IsNullOrWhiteSpace(actionComponentGraph.PathToConfigFile))
            {
                throw new ArgumentNullException("PathToConfigFile");
            }

            MsdeployPath = FindFirstValidFileFromList(ActionComponentGraph.MsDeployExeLocations, "MSDEPLOY", true);
        }

        protected abstract string CmdFileName { get; }
        protected abstract string CmdFileNameExtension { get; }
        protected abstract string CmdFileNameExe { get; }
        protected abstract string CmdFileParameterDestinationPath { get; }

        public override int Execute()
        {
            logger.Log(StartSectionBreaker);
            logger.Log(string.Format("Executing MSDEPLOY {0}:", DisplayName));

            //ensure config file has been pushed to remote server.
            new MsDeployFileCopyDeploymentTask(CreateConfigFileCopyActionComponentGraphFrom(ActionComponentGraph)).Execute();

            string fileName = CreateRandomFileName(CmdFileName, CmdFileNameExtension);
            string filePath = FileHelper.MapRelativePath(ActionComponentGraph.SourceContentPath, fileName);
            CreateFile(filePath, CmdFileNameExe + " " + CreateParameterString(CmdFileParameterDestinationPath), true);
            //ensure cmd file has been pushed to remote server.
            new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, fileName)).Execute();

            // call msdeploy to execute appcmd file on remote machine
            int result = InvokeExe(MsdeployPath, GetMsDeployExecuteCmdParams(FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, fileName)));

            // clean up local and remote files
            if (ActionComponentGraph.CleanUp)
            {
                //delete local file(s)
                File.Delete(filePath);

                //delete remote file(s)
                int tempResult = InvokeExe(MsdeployPath, GetMsDeployDeleteFileParams(FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, fileName)));
                if (tempResult != 0) result = tempResult;

                InvokeExe(MsdeployPath, GetMsDeployDeleteFileParams(FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, ActionComponentGraph.PathToConfigFile)));
                if (tempResult != 0) result = tempResult;
            }

            logger.Log(string.Format("Completed {0}.", DisplayName));
            logger.Log(EndSectionBreaker);

            return result;
        }


    }
}