using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployProjectPackgeCreationTask : ProjectPackageCreationTaskBase
    {
        public MsDeployProjectPackgeCreationTask(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Remote project file package creation Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            //take the project file, 
            if (!CheckProjectFileExists()) { return -1; }

            //use msbuild to do:

            //msbuild PROJECTFILE /target:clean /target:package /p:_PackageTempDir=C:\Package /p:PackageLocation=C:\temp\mvctesting\mytest.zip
            var result = InvokeMsBuild();

            FileNameAndFolder fileAndFolder = FindFileNameAndFolderPath();
            if (ActionComponentGraph.ZipFileOnly)
            {                
                DeleteFiles(fileAndFolder.FileName, fileAndFolder.FolderName);
                
                ActionComponentGraph.SourceContentPath = fileAndFolder.FolderName;
                new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, fileAndFolder.FileName + ".zip")).Execute();
            }
            else
            {
                //copy stuff to remote server... take whole folder.
                new MsDeployFileCopyDeploymentTask(CreateFolderCopyActionComponentGraphFrom(ActionComponentGraph, fileAndFolder.FolderName)).Execute();    
            }            

            return result;

        }
    }
}