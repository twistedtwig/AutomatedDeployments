using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;
using GenericMethods;
using Logging;

namespace DeploymentTask.Tasks
{
    public abstract class DeploymentTaskBase<T> : DeploymentTaskRoot where T : ActionComponentGraphBase
    {
        private static Logger logger = Logger.GetLogger();
        protected T ActionComponentGraph;

        protected DeploymentTaskBase(T actionComponentGraph)
        {
            ActionComponentGraph = actionComponentGraph;

            if (RequiresAdminRights)
            {
                if (!IsAdministrator)
                {
                    throw new SecurityException("requires admin permission to execute this task");
                }
            }
        }
        
        protected string EnsureStringhasOnlyOneTrailingWhiteSpace(string value)
        {
            return value.Trim() + " ";
        }

        protected int InvokeExe(string pathToExe, string commandArgs)
        {
            logger.Log("Invoking '{0}' with args '{1}'", pathToExe, commandArgs, LoggingLevel.Verbose);
            ProcessStartInfo msdeployProcess = new ProcessStartInfo(pathToExe)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = commandArgs,
            };

            Process p = new Process { StartInfo = msdeployProcess, EnableRaisingEvents = true };
            p.OutputDataReceived += (sender, args) => logger.Log("received output: {0}", args.Data);

            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.CancelOutputRead();

            logger.Log("Finished Invoking '{0}'", pathToExe, LoggingLevel.Verbose);
            return p.ExitCode;
        }

        protected bool CreateFile(string path, string content, bool overRideCurrent)
        {
            logger.Log("Creating File '{0}'", path, LoggingLevel.Verbose);
            if (File.Exists(path))
            {
                if (overRideCurrent)
                {
                    logger.Log("Forcing delete of file '{0}'", path, LoggingLevel.Verbose);
                    File.Delete(path);
                }
                else
                {
                    return false;
                }
            }

            if (path.Contains(@"\"))
            {                
                string dir = path.Substring(0, path.LastIndexOf(@"\"));
                if (!Directory.Exists(dir))
                {
                    logger.Log("Creating directory '{0}' for file '{1}'", dir, path, LoggingLevel.Verbose);
                    Directory.CreateDirectory(dir);
                }
            }
            
            logger.Log("Writing contents to file '{0}'", path, LoggingLevel.Verbose);
            File.WriteAllText(path, content);
            return true;
        }
        
        protected string CreateRandomFileName(string fileNamBeginning, string extension)
        {
            string ticks = DateTime.Now.Millisecond.ToString();
            string val = ticks.Substring(0, ticks.Length >= 4 ? 4 : ticks.Length);

            return fileNamBeginning + val + (string.IsNullOrWhiteSpace(extension) ? "" : "." + extension);
        }

        protected static FileCopyActionComponentGraph CreateConfigFileCopyActionComponentGraphFrom(IisActionComponentGraph iisActionComponentGraph)
        {
            return CreateSingleFileCopyActionComponentGraphFrom(iisActionComponentGraph, iisActionComponentGraph.PathToConfigFile);
        }


        protected static FileCopyActionComponentGraph CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraphBase iisActionComponentGraph, string fileName)
        {
            return CreateSingleFileCopyActionComponentGraphFrom(iisActionComponentGraph,
                FileHelper.MapRelativePath(Directory.GetCurrentDirectory(), FileHelper.MapRelativePath(iisActionComponentGraph.SourceContentPath, fileName)),
                FileHelper.MapRelativePath(iisActionComponentGraph.DestinationContentPath, fileName));
        }

        protected static FileCopyActionComponentGraph CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraphBase iisActionComponentGraph, string fullSourceFilePath, string fullDestinationFilePath)
        {
            FileCopyActionComponentGraph fileCopyAction = new FileCopyActionComponentGraph();
            fileCopyAction.AppCmdExe = iisActionComponentGraph.AppCmdExe;
            fileCopyAction.MsDeployExeLocations = iisActionComponentGraph.MsDeployExeLocations;
            fileCopyAction.ActionType = ActionType.FileDeployment;
            fileCopyAction.CleanUp = iisActionComponentGraph.CleanUp;
            fileCopyAction.DestinationComputerName = iisActionComponentGraph.DestinationComputerName;
            fileCopyAction.DestinationPassword = iisActionComponentGraph.DestinationPassword;
            fileCopyAction.DestinationUserName = iisActionComponentGraph.DestinationUserName;
            fileCopyAction.SourceContentPath = fullSourceFilePath;
            fileCopyAction.DestinationContentPath = fullDestinationFilePath;
            fileCopyAction.ForceInstall = iisActionComponentGraph.ForceInstall;
            return fileCopyAction;
        }

        protected static FileCopyActionComponentGraph CreateFolderCopyActionComponentGraphFrom(ActionComponentGraphBase iisActionComponentGraph, string folderPath)
        {
            return CreateFolderCopyActionComponentGraphFrom(iisActionComponentGraph, folderPath, iisActionComponentGraph.DestinationContentPath);
        }

        protected static FileCopyActionComponentGraph CreateFolderCopyActionComponentGraphFrom(ActionComponentGraphBase iisActionComponentGraph, string sourceFolderPath, string destinationFolderPath)
        {
            FileCopyActionComponentGraph fileCopyAction = new FileCopyActionComponentGraph();
            fileCopyAction.AppCmdExe = iisActionComponentGraph.AppCmdExe;
            fileCopyAction.MsDeployExeLocations = iisActionComponentGraph.MsDeployExeLocations;
            fileCopyAction.ActionType = ActionType.FileDeployment;
            fileCopyAction.CleanUp = iisActionComponentGraph.CleanUp;
            fileCopyAction.DestinationComputerName = iisActionComponentGraph.DestinationComputerName;
            fileCopyAction.DestinationPassword = iisActionComponentGraph.DestinationPassword;
            fileCopyAction.DestinationUserName = iisActionComponentGraph.DestinationUserName;
            fileCopyAction.SourceContentPath = sourceFolderPath;
            fileCopyAction.DestinationContentPath = destinationFolderPath;
            fileCopyAction.ForceInstall = iisActionComponentGraph.ForceInstall;
            return fileCopyAction;
        }



        private const string SectionBreaker = "----------------------------------------------------";
        protected readonly string StartSectionBreaker = Environment.NewLine + SectionBreaker;
        protected readonly string EndSectionBreaker = SectionBreaker + Environment.NewLine;


        
        public int CompareComponentGraph(DeploymentTaskBase<T> other)
        {
            if (other.DoesObjectInheritFromGenericBaseClass(typeof(FileDeploymentTaskBase)))
            {
                var thisTask = this as FileDeploymentTaskBase;
                if (thisTask == null) return 1;

                var othertask = other as FileDeploymentTaskBase;
                if (othertask== null) return -1;

                return thisTask.CompareFileComponentGraph(othertask);
            }

            if (other.DoesObjectInheritFromGenericBaseClass(typeof(IisDeploymentTaskBase)))
            {
                var thisTask = this as IisDeploymentTaskBase;
                if (thisTask == null) return 1;

                var othertask = other as IisDeploymentTaskBase;
                if (othertask == null) return -1;

                return thisTask.CompareIisComponentGraph(othertask);
            }

            throw new ArgumentOutOfRangeException();
        }

        protected string CleanStringOfNonFileTypeCharacters(string value)
        {
            string newValue = value.Trim();
            newValue = newValue.Replace(" ", "-");
            newValue = newValue.Replace(@"\", "");
            newValue = newValue.Replace(@"/", "");
            newValue = newValue.Replace(@"@", "");
            newValue = newValue.Replace(@".", "_");
                        
            return newValue;
        }

        protected string FindFirstValidFileFromList(IEnumerable<string> list, string description = "", bool throwOnNotFound = false)
        {
            if(string.IsNullOrWhiteSpace(description))
            {
                logger.Log("trying to find a valid file path", LoggingLevel.Verbose);
            }
            else
            {
                logger.Log("Trying to find a valid locatin for {0}.", description, LoggingLevel.Verbose);                
            }

            foreach (string path in list)
            {
                logger.Log("checking file path for '{0}'", path, LoggingLevel.Verbose);
                if (File.Exists(path))
                {
                    logger.Log("Found valid path '{0}'", path, LoggingLevel.Verbose);
                    return path;
                }
            }


            if (throwOnNotFound)
            {
                logger.Log("No valid path found in list", LoggingLevel.Verbose);
                logger.Log("throwing ArgumentOutOfRangeException", LoggingLevel.Verbose);
                throw new ArgumentOutOfRangeException();
            }

            logger.Log("No valid path found in list returning empty string", LoggingLevel.Verbose);
            return string.Empty;
        }


    }
}
