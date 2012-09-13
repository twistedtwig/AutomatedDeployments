using System;
using System.IO;
using System.Text;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class ProjectPackageCreationTaskBase : DeploymentTaskBase<PackageCreationComponentGraph>
    {
        protected ProjectPackageCreationTaskBase(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }
        
        protected bool CheckProjectFileExists()
        {
            return File.Exists(ActionComponentGraph.SourceContentPath);
        }

        protected string OutputLocationUsed { get; private set; }

        protected int InvokeMsBuild()
        {
            if (string.IsNullOrWhiteSpace(ActionComponentGraph.MsBuildExe))
                return -1;

            StringBuilder argBuilder = new StringBuilder();

            argBuilder.Append(ActionComponentGraph.SourceContentPath);
            argBuilder.Append(" /target:clean /target:package");
            
            if(!string.IsNullOrWhiteSpace(ActionComponentGraph.ConfigurationType))
            {
                argBuilder.Append(string.Format(" /p:Configuration={0}", ActionComponentGraph.ConfigurationType));
            }
            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.InternalPath))
            {
                argBuilder.Append(string.Format(" /p:_PackageTempDir={0}", ActionComponentGraph.InternalPath));
            }

            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.OutputLocation))
            {
                argBuilder.Append(string.Format(" /p:PackageLocation={0}", ActionComponentGraph.OutputLocation));
            }

            return InvokeExe(ActionComponentGraph.MsBuildExe, argBuilder.ToString());
        }

        /// <summary>
        /// Finds the file name and folder of the package created by the invokeMsBuild, i.e. must be called after invoke otherwise will not return correct data.
        /// </summary>
        /// <returns></returns>
        protected FileNameAndFolder FindFileNameAndFolderPath()
        {
            string folder = string.Empty;
            string fileName = string.Empty;

            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.OutputLocation))
            {
                FileInfo zipFile = new FileInfo(ActionComponentGraph.OutputLocation);
                if (zipFile.Exists && zipFile.Directory != null)
                {
                    folder = zipFile.Directory.FullName;
                    fileName = zipFile.Name.Substring(0, zipFile.Name.LastIndexOf("."));

                    return new FileNameAndFolder() { FileName = fileName, FolderName = folder };
                }
            }

            FileInfo projFile = new FileInfo(ActionComponentGraph.SourceContentPath);
            if (!projFile.Exists || projFile.Directory == null)
            {
                throw new ArgumentNullException("no file found");
            }

            fileName = projFile.Name.Substring(0, projFile.Name.LastIndexOf("."));
            folder = Path.Combine(projFile.Directory.FullName,
                                  "obj",
                                  string.IsNullOrWhiteSpace(ActionComponentGraph.ConfigurationType)
                                      ? "Debug" : ActionComponentGraph.ConfigurationType,
                                  "Package");

            return new FileNameAndFolder() { FileName = fileName, FolderName = folder };
        }

        protected void DeleteFiles(string fileName, string folderPath)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderPath))
            {
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            if (!dir.Exists) { return; }

            FileInfo[] files = dir.GetFiles(fileName + ".*", SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files)
            {
                if (file.Extension.Equals(".zip")) { continue; }
                System.Console.WriteLine("Deleting file: " + file.Name);
                file.Delete();
            }

            //delete PackageTmp folder and all contents
            string tempPackageFolderPath = Path.Combine(folderPath, "PackageTmp");
            if (Directory.Exists(tempPackageFolderPath))
            {
                System.Console.WriteLine("Deleting folder: PackageTmp");
                Directory.Delete(tempPackageFolderPath, true);
            }
        }
    }

    public class FileNameAndFolder
    {
        public string FileName { get; set; }
        public string FolderName { get; set; }
        
    }
}
