using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public abstract class IisDeploymentTaskBase : DeploymentTaskBase<IisActionComponentGraph>
    {
        protected IisDeploymentTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        protected string GetMsDeployExecuteCmdSource(string pathToCmd)
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-source:runCommand='{0}'", pathToCmd));
        }

        protected string GetMsDeployExecuteCmdDestination()
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-dest:auto,computerName='{0}',userName='{1}',password='{2}'", 
                ActionComponentGraph.DestinationComputerName, ActionComponentGraph.DestinationUserName, ActionComponentGraph.DestinationPassword));
        }

        protected string GetMsDeployExecuteCmdSync()
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace("-verb:sync");
        }

        protected string GetMsDeployExecuteCmdParams(string pathToCmd)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append(GetMsDeployExecuteCmdSource(pathToCmd));
            parameters.Append(GetMsDeployExecuteCmdDestination());
            parameters.Append(GetMsDeployExecuteCmdSync());

            return parameters.ToString();
        }


        protected string GetMsDeployDeleteFileParams(string filePath)
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-dest:ContentPath='{0}',computerName='{1}',userName='{2}',password='{3}' -verb:delete",
                filePath, ActionComponentGraph.DestinationComputerName, ActionComponentGraph.DestinationUserName, ActionComponentGraph.DestinationPassword));
        }

        protected IList<string> FindIisSettingsNamesFromConfig(string pathToConfigFile, Regex regex)
        {
            IList<string> iisSettingNames = new List<string>();

            foreach (string line in File.ReadAllLines(pathToConfigFile))
            {
                Match match = regex.Match(line);
                if (match.Success)
                {
                    string settingName = match.Groups[3].Value;
                    if (string.IsNullOrWhiteSpace(settingName))
                    {
                        throw new ArgumentException("Error reading IIS Setting from config for removal: " + pathToConfigFile);
                    }

                    if (!iisSettingNames.Contains(settingName))
                    {
                        iisSettingNames.Add(settingName);
                    }
                }
            }

            if (!iisSettingNames.Any())
            {
                Console.WriteLine(string.Format("No matches found for regex '{0}' in file '{1}'", regex, pathToConfigFile));
            }
            return iisSettingNames;
        }
       
        public int CompareIisComponentGraph(IisDeploymentTaskBase other)
        {
            if (other == null) return -1;
            int value = 0;
            value = ActionComponentGraph.DestinationComputerName.CompareTo(other.ActionComponentGraph.DestinationComputerName);
            if (value != 0) return value;

            value = ActionComponentGraph.SourceContentPath.CompareTo(other.ActionComponentGraph.SourceContentPath);
            if (value != 0) return value;

            value = ActionComponentGraph.DestinationContentPath.CompareTo(other.ActionComponentGraph.DestinationContentPath);
            if (value != 0) return value;

            return ActionComponentGraph.PathToConfigFile.CompareTo(other.ActionComponentGraph.PathToConfigFile);
        }

        protected abstract string CreateParameterString(string parameter);
    }
}
