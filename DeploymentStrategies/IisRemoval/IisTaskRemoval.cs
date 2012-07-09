using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using InstallerComponents;
using DeploymentStrategies.ExecutionStratagies;

namespace DeploymentStrategies.IisRemoval
{
    public abstract class IisTaskRemoval : DeploymentStrategy
    {
        protected IisRemoverComponentGraph IisRemoverComponent;

        public string PathToRemovalFile { get; protected set; }

        protected IisTaskRemoval(IisRemoverComponentGraph iisRemoverComponent, RemoteDeploymentStrategyType strategyType) : base(strategyType, iisRemoverComponent.Index)
        {
            IisRemoverComponent = iisRemoverComponent;

            if (!CheckLocalInstallFolderExists())
            {
                throw new ArgumentException("the local install folder doesn't exist: " + iisRemoverComponent.LocalInstallFolder);
            }

            EnsureNoInstallerFileExists();

            PathToRemovalFile = GetPathToRemovalFile();
        }

        public abstract string GetPathToRemovalFile();
        public abstract RemoteExecutionStrategy CreateDeploymentStrategy();

        protected bool CheckLocalInstallFolderExists()
        {
            return Directory.Exists(IisRemoverComponent.LocalInstallFolder);
        }

        //ensure no file exists for installer file we will create
        protected void EnsureNoInstallerFileExists()
        {
            if (File.Exists(PathToRemovalFile))
            {
                File.Delete(PathToRemovalFile);
            }
        }

        protected RemoteExecutionStrategy CreateDeploymentStrategyBase(string command)
        {
            if (!ValidateParameters())
            {
                throw new ArgumentException("error with Iis component graph parameters");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(IisRemoverComponent.AppCmdExe));            
            builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(command));           

            File.WriteAllText(PathToRemovalFile, builder.ToString());

            Dictionary<string, string> values = new Dictionary<string,string>();
            values.Add("MsdeployExe", IisRemoverComponent.MsdeployExe);
            values.Add("PathToCmd", PathToRemovalFile);
            values.Add("DestinationComputerName", IisRemoverComponent.DestinationComputerName);
            if(!string.IsNullOrWhiteSpace(IisRemoverComponent.DestinationUserName))
            {
                values.Add("DestinationUserName", IisRemoverComponent.DestinationUserName);
            }
            if (!string.IsNullOrWhiteSpace(IisRemoverComponent.DestinationPassword))
            {
                values.Add("DestinationPassword", IisRemoverComponent.DestinationPassword);
            }

            RemoteExecutionComponentGraph remoteExecutionComponentGraph = new RemoteExecutionComponentGraph(IisRemoverComponent.Name, values);

            return new MsDeployRemoteExecutionStrategy(remoteExecutionComponentGraph, StrategyType);

            //want to execute the file just created on the reomte server:

            //"C:\Program Files\IIS\Microsoft Web Deploy V2\\msdeploy.exe" 
            //-source:runCommand='C:\Temp\deploy\createIISSetup.cmd',waitinterval=5000 
            //-dest:auto,computerName='192.168.10.98:8172',userName='administrator',password='wiggle_4_it' 
            //-verb:sync
        }

        public override bool ValidateParameters()
        {
            if (IisRemoverComponent == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisRemoverComponent.MsdeployExe))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisRemoverComponent.AppCmdExe))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisRemoverComponent.DestinationComputerName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(IisRemoverComponent.ItemToRemove))
            {
                return false;
            }

            return true;
        }

    }
}
