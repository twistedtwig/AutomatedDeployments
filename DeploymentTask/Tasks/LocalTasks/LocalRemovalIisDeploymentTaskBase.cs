using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;
using FileSystem.Helper;

namespace DeploymentTask.Tasks.LocalTasks
{
    public abstract class LocalRemovalIisDeploymentTaskBase : IisDeploymentTaskBase
    {
        protected LocalRemovalIisDeploymentTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
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
        protected abstract Regex ConfigFileNamePattern { get; }

        public override int Execute()
        {
            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine(string.Format("Executing local {0}:", DisplayName));

            //get config file... ensure not null
            if (!File.Exists(FileHelper.MapRelativePath(ActionComponentGraph.SourceContentPath, ActionComponentGraph.PathToConfigFile)))
            {
                throw new FileNotFoundException(ActionComponentGraph.PathToConfigFile);
            }

            //get all names from it from regex.
            IList<string> names = FindIisSettingsNamesFromConfig(FileHelper.MapRelativePath(ActionComponentGraph.SourceContentPath, ActionComponentGraph.PathToConfigFile), ConfigFileNamePattern);
            int result = ExpectedReturnValue;
            foreach (string name in names)
            {
                //create tempfile to remove all pool by name given
                string fileName = CreateRandomFileName(CmdFileName +  CleanStringOfNonFileTypeCharacters(name), CmdFileNameExtension);
                string filePath = FileHelper.MapRelativePath(ActionComponentGraph.SourceContentPath, fileName);
                CreateFile(filePath, CmdFileNameExe + " " + CreateParameterString(name), true);

                //execute cmd file
                result = InvokeExe(filePath, string.Empty);

                if (ActionComponentGraph.CleanUp)
                {
                    File.Delete(filePath);
                }
            }

            Console.WriteLine(string.Format("Completed {0}.", DisplayName));
            Console.WriteLine(EndSectionBreaker);

            return result;
        }
    }
}