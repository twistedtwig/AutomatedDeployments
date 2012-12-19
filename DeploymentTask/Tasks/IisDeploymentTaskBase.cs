using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeploymentConfiguration.Actions;
using Logging;

namespace DeploymentTask.Tasks
{
    public abstract class IisDeploymentTaskBase : DeploymentTaskBase<IisActionComponentGraph>
    {
        private static Logger logger = Logger.GetLogger();

        protected IisDeploymentTaskBase(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
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
                logger.Log(string.Format("No matches found for regex '{0}' in file '{1}'", regex, pathToConfigFile));
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

        public override bool RequiresAdminRights { get { return true; }
        }
    }
}
