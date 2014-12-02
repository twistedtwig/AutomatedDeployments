using System;
using System.IO;
using System.Linq;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;
using Ionic.Zip;
using Logging;

namespace DeploymentTask.Tasks
{
    public abstract class ProjectPackageDeploymentTaskBase : DeploymentTaskBase<PackageDeploymentComponentGraph>
    {
        private static Logger logger = Logger.GetLogger();
        
        protected ProjectPackageDeploymentTaskBase(PackageDeploymentComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }
        
        protected bool CheckZipPackageFileExists()
        {
            return File.Exists(ActionComponentGraph.SourceContentPath);
        }

        protected string TempLocation { get; private set; }


        protected string DestinationPath
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(ActionComponentGraph.DestinationContentPath) && ActionComponentGraph.AllowUseOfDestinationFolder)
                                ? ActionComponentGraph.DestinationContentPath
                                : ArchiveXmlFilePath;
            }
        }
        
        protected void UnZipFileToTempLocation()
        {
            GetTempLocation();

            logger.Log("UnZipFile temp location: {0}", TempLocation, LoggingLevel.Verbose);
            
            
            logger.Log("unpacking zip package to: " + TempLocation);
            using (ZipFile zipFile = ZipFile.Read(ActionComponentGraph.SourceContentPath))
            {
                // here, we extract every entry, but we could extract conditionally
                // based on entry name, size, date, checkbox status, etc.  
                foreach (ZipEntry e in zipFile)
                {
                    e.Extract(TempLocation, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void GetTempLocation()
        {
            //only want to have one temp path, no need to keep creating new ones for each run.
            if (string.IsNullOrWhiteSpace(TempLocation))
            {
                TempLocation = Path.Combine(Path.GetPathRoot(Path.GetTempPath()), "temp", Guid.NewGuid().ToString());
                RegisterForCleanUpTempLocation();
                Directory.CreateDirectory(TempLocation);
                logger.Log("Created Directory: {0}", TempLocation, LoggingLevel.Verbose);
            }
        }

        protected void RegisterForCleanUpTempLocation()
        {
            FoldersToBeCleanedUp.Add(TempLocation);
        }

        protected void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }


        private string _ArchiveXmlFilePath = string.Empty;
        private string ArchiveXmlFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_ArchiveXmlFilePath))
                {
                    string path = FileHelper.MapRelativePath(TempLocation, "Archive.xml");
                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException(path);
                    }

                    foreach (string line in File.ReadAllLines(path))
                    {
                        var sline = line.Trim();
                        if (sline.StartsWith(@"<iisApp path="""))
                        {
                            sline = sline.Substring((@"<iisApp path=""").Length);
                            return sline.Substring(0, sline.IndexOf('"'));
                        }
                                                
                    }

                    throw new ArgumentNullException("IisApp Path not found in archive.xml");
                }

                return _ArchiveXmlFilePath;
            }
        }

        protected string FindPackageFileRootLocation()
        {
            //get archive xml path
            string archivePath = ArchiveXmlFilePath;

            //remove the drive
            archivePath = archivePath.Substring(3);

            //split into directories
            string[] archiveDirs = archivePath.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            //get temp folder location
            DirectoryInfo baseDir = new DirectoryInfo(TempLocation);

            //get Content folder
            //get next folder (takes drive issue out)
            DirectoryInfo currentDir = baseDir.GetDirectories("Content").First().GetDirectories().First();
            DirectoryInfo[] directories = currentDir.GetDirectories();


            //foreach directory sting from archive xml follow directory down until no more strings
            foreach (string dir in archiveDirs)
            {
                currentDir = directories.First(d => d.Name.Equals(dir));
                directories = currentDir.GetDirectories();
            }

            //that next folder should be the root.
            return currentDir.FullName;
        }


        protected string AppOffLineFileName { get { return "app_offline.htm"; } }

        protected string CreateAppOffLineFile()
        {
            GetTempLocation();
            string filePath = Path.Combine(TempLocation, AppOffLineFileName);
            //using is here so that the file stream is closed straight away.
            using (File.Create(filePath)) { }
            return filePath;
        }
        
    }
}
