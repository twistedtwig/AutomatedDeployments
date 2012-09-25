using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;
using Ionic.Zip;

namespace DeploymentTask.Tasks
{
    public abstract class ProjectPackageDeploymentTaskBase : DeploymentTaskBase<PackageDeploymentComponentGraph>
    {
        private const string Pattern = @"(<iisApp path=(['''''',""""""]){0,1}((?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.]+)\\(?:[\w]+\\)*\w([\w.])+)\2)";
        private readonly Regex Regex = new Regex(Pattern, RegexOptions.Compiled);

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
            TempLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            CleanUpTempLocation();

            Directory.CreateDirectory(TempLocation);
            Console.WriteLine("unpacking zip package to: " + TempLocation);
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

        protected void CleanUpTempLocation()
        {
            DeleteDirectory(TempLocation);
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
                        Match match = Regex.Match(line);
                        if (match.Success)
                        {
                            _ArchiveXmlFilePath = match.Groups[3].Value;
                            return _ArchiveXmlFilePath;
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
       
    }
}
