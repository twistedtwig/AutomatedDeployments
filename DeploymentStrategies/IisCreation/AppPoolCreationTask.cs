using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using InstallerComponents;
using DeploymentStrategies.ExecutionStratagies;
using DeploymentStrategies.IisRemoval;

namespace DeploymentStrategies.IisCreation
{
    public class AppPoolCreationTask : IisTaskCreation
    {
        public AppPoolCreationTask(IisInstallerComponentBaseGraph iisInstallerComponent) : base(iisInstallerComponent, RemoteDeploymentStrategyType.AppPoolCreation)
        {
        }

        //check config file exists
        //check no file exists for installer file we will create
        //create installer file.
        //create Iisdeploymentstrategy and return it.
        public override string GetPathToInstallerFile()
        {
            return Path.Combine(IisInstallerComponent.LocalInstallFolder, "appPoolCreationScript.cmd");
        }

        public override RemoteExecutionStrategy CreateDeploymentStrategy()
        {
            return CreateDeploymentStrategyBase("add apppool /IN < ");
        }

        public override IList<DeploymentStrategy> GetExtraRequiredStrategies()
        {
            IList<DeploymentStrategy> strategies = new List<DeploymentStrategy>();
            //get the appPool config file, find the app pool name, create AppPoolRemover for it.
            string fullPathToConfig = Path.Combine(IisInstallerComponent.LocalInstallFolder, IisInstallerComponent.PathToConfigFile);
            if (!File.Exists(fullPathToConfig))
            {
                return strategies;
            }
            const string pattern = @"(APPPOOL.NAME=(['''''',""""""]){0,1}(([a-z-A-Z0-9_-]|\s)+)\2)";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);

            foreach (string line in File.ReadAllLines(fullPathToConfig))
            {                
                Match match = regex.Match(line);
                if (match.Success)
                {
                    string appPoolName = match.Groups[3].Value;
                    if (string.IsNullOrWhiteSpace(appPoolName))
                    {
                        throw new ArgumentException("Error reading app pools from config for force remove before instal: " + IisInstallerComponent.PathToConfigFile);
                    }
                    //use regex (see quick tests) to find the app pool name from the file.  read each line at a time till you find it.

                    //if found it then create command, if not consoel out that falied to find it and return empy tlist.
                    Dictionary<string, string> values = new Dictionary<string, string>
                                            {
                                                {"MsdeployExe", IisInstallerComponent.MsdeployExe},
                                                {"AppCmdExe", IisInstallerComponent.AppCmdExe},
                                                {"LocalInstallFolder", IisInstallerComponent.LocalInstallFolder},
                                                {"RemoteInstallFolder",IisInstallerComponent.RemoteInstallFolder},
                                                {"ItemToRemove", appPoolName},
                                                {"DestinationComputerName",IisInstallerComponent.DestinationComputerName}
                                            };

                    if(!string.IsNullOrWhiteSpace(IisInstallerComponent.DestinationUserName))
                    {
                        values.Add("DestinationUserName", IisInstallerComponent.DestinationUserName);
                    }
                    if (!string.IsNullOrWhiteSpace(IisInstallerComponent.DestinationPassword))
                    {
                        values.Add("DestinationPassword", IisInstallerComponent.DestinationPassword);
                    }

                    Console.WriteLine("Creating AppPool Removal task to force update of appPool installer script: '{0}'", appPoolName);
                    IisRemoverComponentGraph componentGraph = new IisRemoverComponentGraph(IisInstallerComponent.Name + "-AppPoolForce-Remove", values, ComponentType.AppPoolRemover);
                    strategies.Add(new AppPoolRemover(componentGraph).CreateDeploymentStrategy());
                }
            }
            return strategies;
        }

        public override void Execute()
        {
            throw new NotImplementedException("The app Pool Creation Task should not be Executed, it returns a new object after creating the requird files.");
        }

        public override bool ValidateParameters()
        {
            if(IisInstallerComponent == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisInstallerComponent.MsdeployExe))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisInstallerComponent.AppCmdExe))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisInstallerComponent.DestinationComputerName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisInstallerComponent.PathToConfigFile))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisInstallerComponent.RemoteInstallFolder))
            {
                return false;
            }

            return true;
        }
    }
}
