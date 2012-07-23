using System;
using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.LocalTasks
{
    public abstract class LocalInstallIisDeploymentTaskBase : IisDeploymentTaskBase
    {
        protected LocalInstallIisDeploymentTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
            if (string.IsNullOrWhiteSpace(actionComponentGraph.AppCmdExe))
            {
                throw new ArgumentNullException("AppCmdExe");
            }

            if (string.IsNullOrWhiteSpace(actionComponentGraph.PathToConfigFile))
            {
                throw new ArgumentNullException("PathToConfigFile");
            }          
        }

        protected abstract string CmdFileName { get; }
        protected abstract string CmdFileNameExtension { get; }
        protected abstract string CmdFileNameExe { get; }
        protected abstract string CmdFileParameterDestinationPath { get; }

        public override int Execute()
        {
            string fileName = CreateRandomFileName(CmdFileName, CmdFileNameExtension);
            string filePath = Path.Combine(ActionComponentGraph.SourceContentPath, fileName);
            CreateFile(filePath, CmdFileNameExe + " " + CreateParameterString(CmdFileParameterDestinationPath), true);
           
            // call msdeploy to execute appcmd file on remote machine
            int result = InvokeExe(filePath, string.Empty);

            if (ActionComponentGraph.CleanUp)
            {
                File.Delete(filePath);
            }

            return result;
        }
    }
}