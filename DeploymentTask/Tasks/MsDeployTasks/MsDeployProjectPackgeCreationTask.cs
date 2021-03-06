using System;
using DeploymentConfiguration.Actions;
using Logging;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployProjectPackgeCreationTask : ProjectPackageCreationTaskBase
    {
        private static Logger logger = Logger.GetLogger();

        public MsDeployProjectPackgeCreationTask(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Remote project file package creation Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            logger.Log(StartSectionBreaker);
            logger.Log("Executing MSDEPLOY package creation command:");

            //take the project file, 
            if (!CheckProjectFileExists()) { return -1; }

            //use msbuild to do:

            //msbuild PROJECTFILE /target:clean /target:package /p:_PackageTempDir=C:\Package /p:PackageLocation=C:\temp\mvctesting\mytest.zip
            var result = InvokeMsBuild();

            FileNameAndFolder fileAndFolder = FindFileNameAndFolderPath();
            string finalPath = string.Empty;
            if (ActionComponentGraph.ZipFileOnly)
            {                
                DeleteFiles(fileAndFolder.FileName, fileAndFolder.FolderName);
                
                ActionComponentGraph.SourceContentPath = fileAndFolder.FolderName;
                finalPath = fileAndFolder.FileName + ".zip";
            }
            else
            {
                //copy stuff to remote server... take whole folder.
                finalPath = fileAndFolder.FolderName;
            }

            if (ActionComponentGraph.ShouldPushPackageToRemoteMachine)
            {
                new MsDeployFileCopyDeploymentTask(CreateFolderCopyActionComponentGraphFrom(ActionComponentGraph, finalPath)).Execute();                    
            }

            logger.Log("Completed package creation.");
            logger.Log(EndSectionBreaker);

            return result;

        }
    }
}