using System;
using System.IO;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public abstract class MsDeployInstallIisDeploymentTaskBase : IisDeploymentTaskBase
    {
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

            if (string.IsNullOrWhiteSpace(actionComponentGraph.MsDeployExe))
            {
                throw new ArgumentNullException("MsDeployExe");
            }
        }

        protected abstract string CmdFileName { get; }
        protected abstract string CmdFileNameExtension { get; }
        protected abstract string CmdFileNameExe { get; }
        protected abstract string CmdFileParameterDestinationPath { get; }

        public override int Execute()
        {
            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine(string.Format("Executing MSDEPLOY {0}:", DisplayName));

            //ensure config file has been pushed to remote server.
            new MsDeployFileCopyDeploymentTask(CreateConfigFileCopyActionComponentGraphFrom(ActionComponentGraph)).Execute();

            string fileName = CreateRandomFileName(CmdFileName, CmdFileNameExtension);
            string filePath = FileHelper.MapRelativePath(ActionComponentGraph.SourceContentPath, fileName);
            CreateFile(filePath, CmdFileNameExe + " " + CreateParameterString(CmdFileParameterDestinationPath), true);
            //ensure cmd file has been pushed to remote server.
            new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, fileName)).Execute();

            // call msdeploy to execute appcmd file on remote machine
            int result = InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployExecuteCmdParams(FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, fileName)));

            // clean up local and remote files
            if (ActionComponentGraph.CleanUp)
            {
                //delete local file(s)
                File.Delete(filePath);

                //delete remote file(s)
                int tempResult = InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployDeleteFileParams(FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, fileName)));
                if (tempResult != 0) result = tempResult;

                InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployDeleteFileParams(FileHelper.MapRelativePath(ActionComponentGraph.DestinationContentPath, ActionComponentGraph.PathToConfigFile)));
                if (tempResult != 0) result = tempResult;
            }

            Console.WriteLine(string.Format("Completed {0}.", DisplayName));
            Console.WriteLine(EndSectionBreaker);

            return result;
        }


    }
}