using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using InstallerComponents;
using DeploymentStrategies.ExecutionStratagies;

namespace DeploymentStrategies.IisCreation
{
    public abstract class IisTaskCreation : DeploymentStrategy
    {
        protected IisInstallerComponentBaseGraph IisInstallerComponent;

        public string PathToInstallerFile { get; protected set; }
        
        protected IisTaskCreation(IisInstallerComponentBaseGraph iisInstallerComponent, RemoteDeploymentStrategyType strategyType) : base(strategyType, iisInstallerComponent.Index)
        {
            IisInstallerComponent = iisInstallerComponent;

            if (!CheckLocalInstallFolderExists())
            {
                throw new ArgumentException("the local install folder doesn't exist: " + iisInstallerComponent.LocalInstallFolder);
            }

            if (!CheckConfigFileExists())
            {
                throw new ArgumentException("config file not found: " + iisInstallerComponent.PathToConfigFile);
            }

            EnsureNoInstallerFileExists();

            PathToInstallerFile = GetPathToInstallerFile();
        }

        public abstract string GetPathToInstallerFile();
        public abstract RemoteExecutionStrategy CreateDeploymentStrategy();

        protected bool CheckLocalInstallFolderExists()
        {
            return Directory.Exists(IisInstallerComponent.LocalInstallFolder);
        }

        //check config file exists
        protected bool CheckConfigFileExists()
        {
            return File.Exists(Path.Combine(IisInstallerComponent.LocalInstallFolder, IisInstallerComponent.PathToConfigFile));
        }

        //ensure no file exists for installer file we will create
        protected void EnsureNoInstallerFileExists()
        {
            if (File.Exists(PathToInstallerFile))
            {
                File.Delete(PathToInstallerFile);
            }
        }

        protected RemoteExecutionStrategy CreateDeploymentStrategyBase(string command)
        {
            if (!ValidateParameters())
            {
                throw new ArgumentException("error with Iis component graph parameters");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(IisInstallerComponent.AppCmdExe));            
            builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(command));
            builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(Path.Combine(IisInstallerComponent.RemoteInstallFolder, IisInstallerComponent.PathToConfigFile)));

            File.WriteAllText(PathToInstallerFile, builder.ToString());

            Dictionary<string, string> values = new Dictionary<string,string>();
            values.Add("MsdeployExe", IisInstallerComponent.MsdeployExe);
            values.Add("PathToCmd", PathToInstallerFile);
            values.Add("DestinationComputerName", IisInstallerComponent.DestinationComputerName);

            if (!string.IsNullOrWhiteSpace(IisInstallerComponent.DestinationUserName))
            {
                values.Add("DestinationUserName", IisInstallerComponent.DestinationUserName);
            }
            if (!string.IsNullOrWhiteSpace(IisInstallerComponent.DestinationPassword))
            {
                values.Add("DestinationPassword", IisInstallerComponent.DestinationPassword);
            }

            RemoteExecutionComponentGraph remoteExecutionComponentGraph = new RemoteExecutionComponentGraph(IisInstallerComponent.Name, values);
            return new MsDeployRemoteExecutionStrategy(remoteExecutionComponentGraph, StrategyType);

            //want to execute the file just created on the reomte server:

            //"C:\Program Files\IIS\Microsoft Web Deploy V2\\msdeploy.exe" 
            //-source:runCommand='C:\Temp\deploy\createIISSetup.cmd',waitinterval=5000 
            //-dest:auto,computerName='192.168.10.98:8172',userName='administrator',password='wiggle_4_it' 
            //-verb:sync
        }
    }
}
