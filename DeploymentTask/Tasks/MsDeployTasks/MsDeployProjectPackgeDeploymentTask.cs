using System;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployProjectPackgeDeploymentTask : ProjectPackageDeploymentTaskBase
    {
        public MsDeployProjectPackgeDeploymentTask(PackageDeploymentComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Remote project file package creation Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine("Executing MSDEPLOY package deployment:");

            //take the zipped package file, 
            if (!CheckZipPackageFileExists()) { return -1; }
            int result = ExpectedReturnValue;

            UnZipFileToTempLocation();
            
            string finalPackageLocation = FindPackageFileRootLocation();
            Console.WriteLine(string.Format("Copying package from '{0}' to '{1}'", finalPackageLocation, DestinationPath));
            //copy stuff to remote server... take whole folder.
            result = new MsDeployFileCopyDeploymentTask(CreateFolderCopyActionComponentGraphFrom(ActionComponentGraph, finalPackageLocation, DestinationPath)).Execute();

            //perform clean up at the end.
            CleanUpTempLocation();
            Console.WriteLine("Finished Deploying package.");
            Console.WriteLine(EndSectionBreaker);
                
            return result;
        }
    }
}