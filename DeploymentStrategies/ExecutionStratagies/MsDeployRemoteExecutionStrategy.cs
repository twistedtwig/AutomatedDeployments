using System;
using System.IO;
using System.Text;
using InstallerComponents;

namespace DeploymentStrategies.ExecutionStratagies
{
    public class MsDeployRemoteExecutionStrategy : RemoteExecutionStrategy
    {
        public MsDeployRemoteExecutionStrategy(RemoteExecutionComponentGraph componentGraph) : base(componentGraph, RemoteDeploymentStrategyType.GenericRemoteExecution)
        { }

        public MsDeployRemoteExecutionStrategy(RemoteExecutionComponentGraph componentGraph, RemoteDeploymentStrategyType strategyType) : base(componentGraph, strategyType)
        { }

        public override void Execute()
        {
            string msDeployExe = string.Empty;
            if (!string.IsNullOrWhiteSpace(ComponentGraph.MsdeployExe))
            {
                msDeployExe = ComponentGraph.MsdeployExe;
            }

            if (!File.Exists(msDeployExe))
            {
                throw new FileNotFoundException("msdeploy.exe was not found");
            }

            if (!ValidateParameters())
            {
                throw new ArgumentException("remote execution component doesn't have all the required information.");
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(GetSource(ComponentGraph)));
            builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(GetDestination(ComponentGraph)));
            builder.Append(GetSync());

            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine("Executing MSDEPLOY remote execution command:");
            Console.WriteLine(builder.ToString());

            InvokeExe(msDeployExe, builder.ToString());

            Console.WriteLine("Completed remote execution task.");
            Console.WriteLine(EndSectionBreaker);
        }

        public override bool ValidateParameters()
        {

            if (string.IsNullOrWhiteSpace(ComponentGraph.MsdeployExe))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ComponentGraph.DestinationComputerName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ComponentGraph.PathToCmd))
            {
                return false;
            }

            return true;
        }


        private string GetSource(RemoteExecutionComponentGraph componentGraph)
        {
            if (string.IsNullOrWhiteSpace(componentGraph.PathToCmd))
            {
                throw new ArgumentException("path to cmd was null or empty");
            }
            return "-source:runCommand='" + componentGraph.PathToCmd + "',waitinterval=5000";
        }

        private string GetDestination(RemoteExecutionComponentGraph componentGraph)
        {
            if (string.IsNullOrWhiteSpace(componentGraph.DestinationComputerName))
            {
                throw new ArgumentException("destination comptuername was empty or not given.");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("-dest:auto");
            builder.Append(",computername='" + componentGraph.DestinationComputerName + "'");

            if (!string.IsNullOrWhiteSpace(componentGraph.DestinationUserName))
            {
                builder.Append(",username='" + componentGraph.DestinationUserName + "'");
            }

            if (!string.IsNullOrWhiteSpace(componentGraph.DestinationPassword))
            {
                builder.Append(",password='" + componentGraph.DestinationPassword + "'");
            }

            return builder.ToString();
        }

        private string GetSync()
        {
            return "-verb:sync";
        }        
    }
}
