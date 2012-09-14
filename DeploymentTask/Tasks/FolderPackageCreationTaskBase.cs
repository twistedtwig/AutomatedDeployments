using System.IO;
using System.Text;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class FolderPackageCreationTaskBase : DeploymentTaskBase<PackageCreationComponentGraph>
    {
        protected FolderPackageCreationTaskBase(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }
        
        protected int InvokeMsDeploy()
        {
            if (string.IsNullOrWhiteSpace(ActionComponentGraph.MsDeployExe))
                return -1;

            //"C:\Program Files\IIS\Microsoft Web Deploy\msdeploy.exe" -source:iisApp='C:\temp\deploy\installer' -dest:package="C:\websites\aa\membersapplication.zip" -verb:sync

            StringBuilder argBuilder = new StringBuilder();

            argBuilder.Append(string.Format(" -source:iisApp='{0}'", ActionComponentGraph.SourceContentPath));
            argBuilder.Append(string.Format(" -dest:package='{0}'", ActionComponentGraph.OutputLocation));
            argBuilder.Append(" -verb:sync");

            int result = InvokeExe(ActionComponentGraph.MsDeployExe, argBuilder.ToString());
            System.Console.WriteLine("Finished creating package file.");
            return result;
        }

        protected bool IsDestinationPathFolder()
        {
            int lastSlash = ActionComponentGraph.DestinationContentPath.LastIndexOf(@"\");
            if (lastSlash == 0) return false;

            string trailingSection = ActionComponentGraph.DestinationContentPath.Substring(lastSlash);
            return !trailingSection.Contains(".");
        }

        protected string GetFullDestinationZipFilePath()
        {
            if (IsDestinationPathFolder())
            {
                FileInfo zipFile = new FileInfo(ActionComponentGraph.OutputLocation);
                return Path.Combine(ActionComponentGraph.DestinationContentPath, zipFile.Name);
            }

            return ActionComponentGraph.DestinationContentPath;
        }
    }
}
