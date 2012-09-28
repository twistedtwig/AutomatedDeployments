using System;
using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.LocalTasks
{
    public class LocalProjectPackgeDeploymentTask : ProjectPackageDeploymentTaskBase
    {
        public LocalProjectPackgeDeploymentTask(PackageDeploymentComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override string DisplayName { get { return "Local project file package creation Task"; } }
        public override int ExpectedReturnValue { get { return 0; } }
        public override bool RequiresAdminRights { get { return false; } }

        public override int Execute()
        {
            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine("Executing local project package deployment command:");

            //take the zipped package file, 
            if (!CheckZipPackageFileExists()) { return -1; }
            int result = ExpectedReturnValue;

            UnZipFileToTempLocation();
            
            //if Forcing overwrite so no local files persist delete directory
            if (ActionComponentGraph.ForceInstall)
            {
                DeleteDirectory(DestinationPath);
                Directory.CreateDirectory(DestinationPath);
            }

            string finalPackageLocation = FindPackageFileRootLocation();
            Console.WriteLine(string.Format("Copying package from '{0}' to '{1}'", finalPackageLocation, DestinationPath));
            result = new LocalFileSystemCopyDeploymentTask(CreateFolderCopyActionComponentGraphFrom(ActionComponentGraph, finalPackageLocation, DestinationPath)).Execute();

            //perform clean up at the end.
            CleanUpTempLocation();
            Console.WriteLine("Finished Deploying package.");
            Console.WriteLine(EndSectionBreaker);

            return result;
        }

    }
}