using System;
using System.IO;
using System.Text;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public class MsDeployFileCopyDeploymentTask : FileDeploymentTaskBase
    {
        public MsDeployFileCopyDeploymentTask(FileCopyActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            //need msdeploy path
            if(string.IsNullOrWhiteSpace(ActionComponentGraph.MsDeployExe))
            {
                throw new ArgumentException("msdeploy path not given");
            }

            if (!File.Exists(ActionComponentGraph.MsDeployExe))
            {
                throw new FileNotFoundException("msdeploy.exe was not found");
            }

            //invoke exe with params.

            StringBuilder msDeployParams = new StringBuilder();
            msDeployParams.Append(GetSource());
            msDeployParams.Append(GetDestination());
            msDeployParams.Append(GetSync());

            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine("Executing MSDEPLOY File Deployment command:");
            Console.WriteLine(msDeployParams.ToString());

            int result = InvokeExe(ActionComponentGraph.MsDeployExe, msDeployParams.ToString());

            Console.WriteLine("Completed file deployment.");
            Console.WriteLine(EndSectionBreaker);

            //if we got here things went OK, (probably)
            return result;
        }

        private string GetSource()
        {
            if(string.IsNullOrWhiteSpace(ActionComponentGraph.SourceContentPath))
            {
                throw new ArgumentException("source contentpath was empty or not given.");
            }

            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-source:contentPath='{0}'", ActionComponentGraph.SourceContentPath));
        }

        private string GetDestination()
        {
            if (string.IsNullOrWhiteSpace(ActionComponentGraph.DestinationContentPath))
            {
                throw new ArgumentException("destination contentpath was empty or not given.");
            }

            if (string.IsNullOrWhiteSpace(ActionComponentGraph.DestinationComputerName))
            {
                throw new ArgumentException("destination comptuername was empty or not given.");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("-dest:");
            builder.Append("contentpath='" + ActionComponentGraph.DestinationContentPath + "'");
            builder.Append(",computername='" + ActionComponentGraph.DestinationComputerName + "'");

            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.DestinationUserName))
            {
                builder.Append(",username='" + ActionComponentGraph.DestinationUserName + "'");
            }

            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.DestinationPassword))
            {
                builder.Append(",password='" + ActionComponentGraph.DestinationPassword + "'");
            }

            return EnsureStringhasOnlyOneTrailingWhiteSpace(builder.ToString());
        }

        private string GetSync()
        {
            StringBuilder builder = new StringBuilder();

            if (!ActionComponentGraph.ForceInstall)
            {
                builder.Append("-enableRule:DoNotDeleteRule ");
            }
            builder.Append(EnsureStringhasOnlyOneTrailingWhiteSpace("-verb:sync"));
            return EnsureStringhasOnlyOneTrailingWhiteSpace(builder.ToString());
        }


        public override string DisplayName
        {
            get { return "MsDeploy Remote File Copy"; }
        }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }
    }
}