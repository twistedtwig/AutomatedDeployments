using System;
using DeploymentConfiguration.Actions;
using Logging;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalProjectPackgeCreationTask : ProjectPackageCreationTaskBase
    {
        private static Logger logger = Logger.GetLogger();

        public LocalProjectPackgeCreationTask(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Local project file package creation Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            logger.Log(StartSectionBreaker);
            logger.Log("Executing local project packaging command:");

            //take the project file, 
            if (!CheckProjectFileExists()) { return -1; }

                //use msbuild to do:

            //msbuild PROJECTFILE /target:clean /target:package /p:_PackageTempDir=C:\Package /p:PackageLocation=C:\temp\mvctesting\mytest.zip
            var result = InvokeMsBuild();

            if (ActionComponentGraph.ZipFileOnly)
            {
                FileNameAndFolder fileAndFolder = FindFileNameAndFolderPath();
                DeleteFiles(fileAndFolder.FileName, fileAndFolder.FolderName);
            }

            logger.Log("Completed project packaging.");
            logger.Log(EndSectionBreaker);

            return result;
        }

    }
}