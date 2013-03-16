using System;
using System.Collections.Generic;
using System.Linq;
using CustomConfigurations;
using DeploymentConfiguration.Actions;
using DeploymentConfiguration.DeploymentStrategies;

namespace DeploymentConfiguration
{
    public class ConfigurationLoader
    {
        public static DeploymentStrategyComponentGraphBase Load(string deploymentName, string configPath)
        {
            return Load(deploymentName, configPath, null);
        }

        public static DeploymentStrategyComponentGraphBase Load(string deploymentName, string configPath, bool? forceLocal)
        {
            Config deployments = new Config(configPath);
            if(!deployments.SectionNames.Contains(deploymentName))
            {
                throw new ArgumentOutOfRangeException("deploymentName");
            }

            //got the deployment we want to create / setup
            ConfigSection deployment = deployments.GetSection(deploymentName);
            if (!deployment.ContainsKey("DestinationComputerName"))
            {
                throw new ArgumentOutOfRangeException("DestinationComputerName");
            }

            string destination = deployment["DestinationComputerName"];
            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentException("DestinationComputerName is empty");
            }

            DeploymentStrategyComponentGraphBase deploymentStrategyComponentGraph;

            if (destination.ToLower().Contains("localhost") || (forceLocal.HasValue && forceLocal.Value))
            {
                deploymentStrategyComponentGraph = deployment.Create<LocalDeploymentStrategyComponentGraphBase>();
            }
            else
            {
                deploymentStrategyComponentGraph = deployment.Create<RemoteDeploymentStrategyComponentGraphBase>();                
            }

            

            if (!deployment.ContainsSubCollections)
            {
                return deploymentStrategyComponentGraph;
            }

            //create the actions for the deployment.
            CreateActions(deploymentStrategyComponentGraph, deployment, GetMsdeployLocations(deployment));

            return deploymentStrategyComponentGraph;
        }

        private static void CreateActions(DeploymentStrategyComponentGraphBase deploymentStrategyComponentGraph, ConfigSection deployment, List<string> msdeployLocations)
        {
            foreach (ConfigSection section in deployment.Collections.GetCollections())
            {
                if (!section.ContainsKey("ComponentType"))
                {
                    //ignore sections such as MsDeploy Locations
                    continue;
                }

                ActionComponentGraphBase actionComponentGraph;
                ActionType actionType = TryParseComponentType(section["ComponentType"]);
                switch (actionType)
                {
                    case ActionType.FileDeployment:
                        actionComponentGraph = section.Create<FileCopyActionComponentGraph>();
                        break;
                    case ActionType.FilePermission:
                        actionComponentGraph = section.Create<SetFilePermissionComponentGraph>();
                        break;
                    case ActionType.AppPoolCreation:
                    case ActionType.AppPoolRemoval:
                    case ActionType.WebSiteCreation:
                    case ActionType.WebsiteRemoval:
                    case ActionType.AppCreation:
                    case ActionType.AppRemoval:
                        actionComponentGraph = section.Create<IisActionComponentGraph>();
                        break;
                    case ActionType.ApplicationExecution:
                        throw new NotImplementedException("need to implement the general do something on a machine action");
                        break;
                        case ActionType.CreatePackage:
                        actionComponentGraph = section.Create<PackageCreationComponentGraph>();
                        break;
                        case ActionType.DeployPackage:
                        actionComponentGraph = section.Create<PackageDeploymentComponentGraph>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                actionComponentGraph.ActionType = actionType;
                actionComponentGraph.MsDeployExeLocations.AddRange(msdeployLocations);
                deploymentStrategyComponentGraph.Actions.Add(actionComponentGraph);
            }
        }

        private static List<string> GetMsdeployLocations(ConfigSection deployment)
        {
            //see if there are any msdeploylocations specified
            List<string> msdeployLocations = new List<string>();
            if (deployment.Collections.SectionNames.Contains("MsDeployLocations"))
            {
                var msdeployLocs = deployment.Collections.GetCollection("MsDeployLocations");
                msdeployLocations.AddRange(msdeployLocs.ValuesAsDictionary.Where(x => !x.IsInherited).Select(location => location.Value));
            }
            return msdeployLocations;
        }

        internal static ActionType TryParseComponentType(string name)
        {
            ActionType componentType;
            if(Enum.TryParse(name, out componentType))
            {
                return componentType;
            }

            throw new ArgumentException("unknown componentType given: " + name);
        }
    }    
}
