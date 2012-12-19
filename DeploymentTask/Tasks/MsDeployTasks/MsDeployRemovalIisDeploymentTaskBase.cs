using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;
using Logging;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public abstract class MsDeployRemovalIisDeploymentTaskBase : IisDeploymentTaskBase
    {
        private static Logger logger = Logger.GetLogger();
        private readonly string MsdeployPath = string.Empty;

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

            MsdeployPath = FindFirstValidFileFromList(actionComponentGraph.MsDeployExeLocations, "MSDPELOY", true);
        }

        protected abstract string CmdFileName { get; }
        protected abstract string CmdFileNameExtension { get; }
        protected abstract string CmdFileNameExe { get; }
        protected abstract string CmdFileParameterDestinationPath { get; }
        protected abstract Regex ConfigFileNamePattern { get; }

        public override int Execute()
        {
            logger.Log(StartSectionBreaker);
            logger.Log(string.Format("Executing MSDEPLOY {0}:", DisplayName));

            //get config file... ensure not null
            string pathToConfigFile = Path.Combine(ActionComponentGraph.SourceContentPath, ActionComponentGraph.PathToConfigFile);
            if (!File.Exists(pathToConfigFile))
            {
                throw new FileNotFoundException(ActionComponentGraph.PathToConfigFile);
            }

            logger.Log("Searching {0} for IIS name values", pathToConfigFile, LoggingLevel.Verbose);
            //get all names from it from regex.
            IList<string> names = FindIisSettingsNamesFromConfig(pathToConfigFile, ConfigFileNamePattern);
            int result = ExpectedReturnValue;
            foreach (string name in names)
            {                
                logger.Log("Found IIS name '{0}'", name, LoggingLevel.Verbose);
                //create tempfile to remove all pool by name given
                string fileName = CreateRandomFileName(CmdFileName + CleanStringOfNonFileTypeCharacters(name), CmdFileNameExtension);
                string filePath = Path.Combine(ActionComponentGraph.SourceContentPath, fileName);
                CreateFile(filePath, CmdFileNameExe + " " + CreateParameterString(name), true);
                //ensure cmd file has been pushed to remote server.
                new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, fileName)).Execute();

                // call msdeploy to execute appcmd file on remote machine
                result = InvokeExe(MsdeployPath, MsDeployTaskExtensions.GetMsDeployExecuteCmdParams(ActionComponentGraph, Path.Combine(ActionComponentGraph.DestinationContentPath, fileName)));

                if (ActionComponentGraph.CleanUp)
                {
                    //delete local file(s)
                    File.Delete(filePath);

                    //delete remote file(s)
                    int tempResult = InvokeExe(MsdeployPath, MsDeployTaskExtensions.GetMsDeployDeleteFileParams(ActionComponentGraph, Path.Combine(ActionComponentGraph.DestinationContentPath, fileName)));
                    if (tempResult != 0) result = tempResult;

                    InvokeExe(MsdeployPath, MsDeployTaskExtensions.GetMsDeployDeleteFileParams(ActionComponentGraph, Path.Combine(ActionComponentGraph.DestinationContentPath, ActionComponentGraph.PathToConfigFile)));
                    if (tempResult != 0) result = tempResult;
                }
            }

            logger.Log(string.Format("Completed {0}:", DisplayName));
            logger.Log(EndSectionBreaker);

            return result;
        }       
    }
}