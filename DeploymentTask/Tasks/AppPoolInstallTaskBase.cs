using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class AppPoolInstallTaskBase : IisDeploymentTaskBase
    {
        protected AppPoolInstallTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
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

        protected override string CreateParameterString(string destinationConfigPath)
        {
            return " add apppool /IN < " +  Path.Combine(destinationConfigPath, ActionComponentGraph.PathToConfigFile);
        }

        protected IList<string> FindAppPoolNames(string pathToConfigFile)
        {
            const string pattern = @"(APPPOOL.NAME=(['''''',""""""]){0,1}(([a-z-A-Z0-9_-]|\s)+)\2)";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return FindIisSettingsNamesFromConfig(pathToConfigFile, regex);
        }
    }
}