using System;
using System.IO;
using System.Text;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;
using Logging;

namespace DeploymentTask.Tasks
{
    public abstract class ProjectPackageCreationTaskBase : DeploymentTaskBase<PackageCreationComponentGraph>
    {
        private static Logger logger = Logger.GetLogger();

        protected ProjectPackageCreationTaskBase(PackageCreationComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }
        
        protected bool CheckProjectFileExists()
        {
            return File.Exists(ActionComponentGraph.SourceContentPath);
        }
        
        protected int InvokeMsBuild()
        {
            if (string.IsNullOrWhiteSpace(ActionComponentGraph.MsBuildExe))
                return -1;

            StringBuilder argBuilder = new StringBuilder();

            argBuilder.Append(string.Format("\"{0}\"", FileHelper.MapRelativePath(Directory.GetCurrentDirectory(), ActionComponentGraph.SourceContentPath)));
            argBuilder.Append(" /target:clean /target:package");
            
            if(!string.IsNullOrWhiteSpace(ActionComponentGraph.ConfigurationType))
            {
                argBuilder.Append(string.Format(" /p:Configuration={0}", ActionComponentGraph.ConfigurationType));
            }
            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.InternalPath))
            {
                argBuilder.Append(string.Format(" /p:_PackageTempDir=\"{0}\"", ActionComponentGraph.InternalPath));
            }

            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.OutputLocation))
            {
                argBuilder.Append(string.Format(" /p:PackageLocation=\"{0}\"", FileHelper.MapRelativePath(Directory.GetCurrentDirectory(), ActionComponentGraph.OutputLocation)));
            }

            argBuilder.Append(string.Format(" /p:AutoParameterizationWebConfigConnectionStrings={0}", ActionComponentGraph.AutoParameterizationWebConfigConnectionStrings));

            logger.Log(StartSectionBreaker);
            logger.Log("Executing project package creation command:");
            logger.Log(string.Format("{0}: {1}", ActionComponentGraph.MsBuildExe, argBuilder));

            int result = InvokeExe(ActionComponentGraph.MsBuildExe, argBuilder.ToString());

            logger.Log("Completed package creation.");
            logger.Log(EndSectionBreaker);

            return result;
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
            logger.Log("deleting files: {0}, folder {1}", fileName, folderPath, LoggingLevel.Verbose);
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
                logger.Log("Deleting file: " + file.Name);
                file.Delete();
            }

            //delete PackageTmp folder and all contents
            string tempPackageFolderPath = FileHelper.MapRelativePath(folderPath, "PackageTmp");
            if (Directory.Exists(tempPackageFolderPath))
            {
                logger.Log("Deleting folder: PackageTmp");
                Directory.Delete(tempPackageFolderPath, true);
            }
        }
    }
}
