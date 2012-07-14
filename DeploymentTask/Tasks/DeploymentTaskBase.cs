using System;
using System.Diagnostics;
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

        protected void InvokeExe(string pathToExe, string commandArgs)
        {
            ProcessStartInfo msdeployProcess = new ProcessStartInfo(pathToExe)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = commandArgs
            };

            Process p = new Process { StartInfo = msdeployProcess, EnableRaisingEvents = true };
            p.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);

            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.CancelOutputRead();
        }



        private const string SectionBreaker = "----------------------------------------------------";
        protected readonly string StartSectionBreaker = Environment.NewLine + SectionBreaker;
        protected readonly string EndSectionBreaker = SectionBreaker + Environment.NewLine;
    }
}
