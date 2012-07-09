using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeploymentStrategies
{
    public abstract class DeploymentStrategy  : IComparable<DeploymentStrategy>
    {
        public RemoteDeploymentStrategyType StrategyType { get; protected set; }
        public int OriginalIndex { get; protected set; }

        protected DeploymentStrategy(RemoteDeploymentStrategyType strategyType, int index)
        {
            StrategyType = strategyType;
            OriginalIndex = index;
        }

        private const string SectionBreaker = "----------------------------------------------------";
        protected static readonly string StartSectionBreaker = Environment.NewLine + SectionBreaker;
        protected static readonly string EndSectionBreaker = SectionBreaker + Environment.NewLine;

        protected string EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(string paramString)
        {
            return paramString.Trim() + " ";
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

            Process p = new Process {StartInfo = msdeployProcess, EnableRaisingEvents = true};
            p.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);
            
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.CancelOutputRead();
        }

        public abstract void Execute();

        public abstract bool ValidateParameters();

        public virtual IList<DeploymentStrategy> GetExtraRequiredStrategies()
        {
            return new List<DeploymentStrategy>();
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(DeploymentStrategy other)
        {            
            if (StrategyType == RemoteDeploymentStrategyType.FileDeployment && other.StrategyType == RemoteDeploymentStrategyType.FileDeployment) { return 0; }
            if (StrategyType == RemoteDeploymentStrategyType.FileDeployment) { return -1; }
            if (other.StrategyType == RemoteDeploymentStrategyType.FileDeployment) { return 1; }

            if (StrategyType == RemoteDeploymentStrategyType.VdirRemoval && other.StrategyType == RemoteDeploymentStrategyType.VdirRemoval) { return 0; }
            if (StrategyType == RemoteDeploymentStrategyType.VdirRemoval) { return -1; }
            if (other.StrategyType == RemoteDeploymentStrategyType.VdirRemoval) { return 1; }

            if (StrategyType == RemoteDeploymentStrategyType.WebsiteRemoval && other.StrategyType == RemoteDeploymentStrategyType.WebsiteRemoval) { return 0; }
            if (StrategyType == RemoteDeploymentStrategyType.WebsiteRemoval) { return -1; }
            if (other.StrategyType == RemoteDeploymentStrategyType.WebsiteRemoval) { return 1; }

            if (StrategyType == RemoteDeploymentStrategyType.AppPoolRemoval && other.StrategyType == RemoteDeploymentStrategyType.AppPoolRemoval) { return 0; }
            if (StrategyType == RemoteDeploymentStrategyType.AppPoolRemoval) { return -1; }
            if (other.StrategyType == RemoteDeploymentStrategyType.AppPoolRemoval) { return 1; }

            return OriginalIndex.CompareTo(other.OriginalIndex);

            // -1 = before 'other' object

            // files copies first
            // vdir removal
            // website removal
            // app pool removal
            //original index specified.
        }
    }

    public enum RemoteDeploymentStrategyType
    {
        FileDeployment,
        AppPoolCreation,
        WebSiteCreation,
        VdirCreation,
        VdirRemoval,
        WebsiteRemoval,
        AppPoolRemoval,
        GenericRemoteExecution,
    }


}
