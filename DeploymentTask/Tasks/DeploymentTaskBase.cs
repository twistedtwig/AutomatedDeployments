using System;
using System.Diagnostics;
using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class DeploymentTaskBase<T> : DeploymentTaskRoot where T : ActionComponentGraphBase
    {
        protected T ActionComponentGraph;

        protected DeploymentTaskBase(T actionComponentGraph)
        {
            ActionComponentGraph = actionComponentGraph;
        }

        
        protected string EnsureStringhasOnlyOneTrailingWhiteSpace(string value)
        {
            return value.Trim() + " ";
        }

        protected int InvokeExe(string pathToExe, string commandArgs)
        {
            ProcessStartInfo msdeployProcess = new ProcessStartInfo(pathToExe)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = commandArgs,
            };

            Process p = new Process { StartInfo = msdeployProcess, EnableRaisingEvents = true };
            p.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);

            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.CancelOutputRead();
            return p.ExitCode;
        }

        protected bool CreateFile(string path, string content, bool overRideCurrent)
        {
            if (File.Exists(path))
            {
                if (overRideCurrent)
                {
                    File.Delete(path);
                }
                else
                {
                    return false;
                }
            }

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


        protected static FileCopyActionComponentGraph CreateSingleFileCopyActionComponentGraphFrom(IisActionComponentGraph iisActionComponentGraph, string fileName)
        {
            FileCopyActionComponentGraph fileCopyAction = new FileCopyActionComponentGraph();
            fileCopyAction.AppCmdExe = iisActionComponentGraph.AppCmdExe;
            fileCopyAction.MsDeployExe = iisActionComponentGraph.MsDeployExe;
            fileCopyAction.ActionType = ActionType.FileDeployment;
            fileCopyAction.CleanUp = iisActionComponentGraph.CleanUp;
            fileCopyAction.DestinationComputerName = iisActionComponentGraph.DestinationComputerName;
            fileCopyAction.DestinationPassword = iisActionComponentGraph.DestinationPassword;
            fileCopyAction.DestinationUserName = iisActionComponentGraph.DestinationUserName;
            fileCopyAction.SourceContentPath = Path.Combine(iisActionComponentGraph.SourceContentPath, fileName);
            fileCopyAction.DestinationContentPath = Path.Combine(iisActionComponentGraph.DestinationContentPath, fileName);
            fileCopyAction.ForceInstall = iisActionComponentGraph.ForceInstall;
            return fileCopyAction;
        }



        private const string SectionBreaker = "----------------------------------------------------";
        protected readonly string StartSectionBreaker = Environment.NewLine + SectionBreaker;
        protected readonly string EndSectionBreaker = SectionBreaker + Environment.NewLine;
    }
}
