using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public abstract class MsDeployRemovalIisDeploymentTaskBase : IisDeploymentTaskBase
    {
        protected MsDeployRemovalIisDeploymentTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
            if (string.IsNullOrWhiteSpace(actionComponentGraph.AppCmdExe))
            {
                throw new ArgumentNullException("AppCmdExe");
            }

            if (string.IsNullOrWhiteSpace(actionComponentGraph.PathToConfigFile))
            {
                throw new ArgumentNullException("PathToConfigFile");
            }

            if (string.IsNullOrWhiteSpace(actionComponentGraph.MsDeployExe))
            {
                throw new ArgumentNullException("MsDeployExe");
            }
        }

        protected abstract string CmdFileName { get; }
        protected abstract string CmdFileNameExtension { get; }
        protected abstract string CmdFileNameExe { get; }
        protected abstract string CmdFileParameterDestinationPath { get; }
        protected abstract Regex ConfigFileNamePattern { get; }

        public override int Execute()
        {
            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine(string.Format("Executing MSDEPLOY {0}:", DisplayName));

            //get config file... ensure not null
            if (!File.Exists(Path.Combine(ActionComponentGraph.SourceContentPath, ActionComponentGraph.PathToConfigFile)))
            {
                throw new FileNotFoundException(ActionComponentGraph.PathToConfigFile);
            }

            //get all names from it from regex.
            IList<string> names = FindIisSettingsNamesFromConfig(Path.Combine(ActionComponentGraph.SourceContentPath, ActionComponentGraph.PathToConfigFile), ConfigFileNamePattern);
            int result = ExpectedReturnValue;
            foreach (string name in names)
            {                
                //create tempfile to remove all pool by name given
                string fileName = CreateRandomFileName(CmdFileName + CleanStringOfNonFileTypeCharacters(name), CmdFileNameExtension);
                string filePath = Path.Combine(ActionComponentGraph.SourceContentPath, fileName);
                CreateFile(filePath, CmdFileNameExe + " " + CreateParameterString(name), true);
                //ensure cmd file has been pushed to remote server.
                new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, fileName)).Execute();

                // call msdeploy to execute appcmd file on remote machine
                result = InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployExecuteCmdParams(Path.Combine(ActionComponentGraph.DestinationContentPath, fileName)));

                if (ActionComponentGraph.CleanUp)
                {
                    //delete local file(s)
                    File.Delete(filePath);

                    //delete remote file(s)
                    int tempResult = InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployDeleteFileParams(Path.Combine(ActionComponentGraph.DestinationContentPath, fileName)));
                    if (tempResult != 0) result = tempResult;

                    InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployDeleteFileParams(Path.Combine(ActionComponentGraph.DestinationContentPath, ActionComponentGraph.PathToConfigFile)));
                    if (tempResult != 0) result = tempResult;
                }
            }

            Console.WriteLine(string.Format("Completed {0}:", DisplayName));
            Console.WriteLine(EndSectionBreaker);

            return result;
        }       
    }
}