using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSystem.Helper
{
    public static class FileHelper
    {

        public static void CopyContents(string currentDirectory, string outputPath, bool recursiveCopy = true)
        {
            CopyContents(currentDirectory, outputPath, new List<string>(), new List<string>(), recursiveCopy);
        }

        public static void CopyContents(string currentDirectory, string outputPath, IList<string> includes, IList<string> excludes, bool recursiveCopy = true)
        {
            if (includes.Any() && excludes.Any())
            {
                foreach (string exclude in excludes)
                {
                    includes.Remove(exclude);
                }
            }

            CopyActualContents(currentDirectory, outputPath, includes, excludes, string.Empty, false, recursiveCopy);
        }

        public static void CopyContents(string currentDirectory, string outPutPath, string searchCriteria, bool recursiveSearch = true, bool recursiveCopy = true)
        {
            CopyActualContents(currentDirectory, outPutPath, new List<string>(), new List<string>(), searchCriteria, recursiveSearch, recursiveCopy);
        }

        private static void CopyActualContents(string currentDirectory, string outputPath, IList<string> includes, IList<string> excludes, string searchCriteria, bool recursiveSearch, bool recursiveCopy)
        {
            Directory.CreateDirectory(outputPath);
            DirectoryInfo currentDir = new DirectoryInfo(currentDirectory);
            if (!currentDir.Exists) return;

            FileInfo[] fileInfos = !string.IsNullOrWhiteSpace(searchCriteria) ? currentDir.GetFiles(searchCriteria) : currentDir.GetFiles();

            foreach (FileInfo file in fileInfos)
            {
                if (includes.Any())
                {
                    if (includes.Any(include => file.FullName.Contains(include)))
                    {
                        file.CopyTo(Path.Combine(outputPath, file.Name), true);
                    }
                }
                else if (excludes.Any())
                {
                    if (!excludes.Any(exclude => file.FullName.Contains(exclude)))
                    {
                        file.CopyTo(Path.Combine(outputPath, file.Name), true);
                    }
                }
                else
                {
                    file.CopyTo(Path.Combine(outputPath, file.Name), true);
                }
            }

            if (recursiveCopy)
            {
                DirectoryInfo[] childDirs = currentDir.GetDirectories();
                foreach (DirectoryInfo directoryInfo in childDirs)
                {
                    Directory.CreateDirectory(Path.Combine(outputPath, directoryInfo.Name));

                    if (Directory.Exists(Path.Combine(outputPath, directoryInfo.Name)))
                    {
                        CopyActualContents(directoryInfo.FullName, Path.Combine(outputPath, directoryInfo.Name), includes, excludes, recursiveSearch ? searchCriteria : string.Empty, recursiveSearch, recursiveCopy);
                    }
                }
            }
        }
    }
}
