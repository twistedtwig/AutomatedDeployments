using System;
using System.Text;
using DeploymentConfiguration.Actions;
using Logging;

namespace DeploymentTask.Tasks.MsDeployTasks
{
    public class MsDeployFileCopyDeploymentTask : FileDeploymentTaskBase
    {
        private static Logger logger = Logger.GetLogger();

        public MsDeployFileCopyDeploymentTask(FileCopyActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            logger.Log("Executing MSDEPLOY file copy task", LoggingLevel.Verbose);
            //need msdeploy path
            string msdeployPath = FindFirstValidFileFromList(ActionComponentGraph.MsDeployExeLocations, "MSDEPLOY", true);
            
            //invoke exe with params.

            StringBuilder msDeployParams = new StringBuilder();
            msDeployParams.Append(GetSource());
            msDeployParams.Append(GetDestination());
            msDeployParams.Append(GetSync());
            msDeployParams.Append(EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-allowUntrusted:{0} ", ActionComponentGraph.AllowUntrusted)));

            logger.Log(StartSectionBreaker);
            logger.Log("Executing MSDEPLOY File Deployment command:");
            logger.Log(msDeployParams.ToString());

            int result = InvokeExe(msdeployPath, msDeployParams.ToString());

            logger.Log("Completed file deployment.");
            logger.Log(EndSectionBreaker);

            //if we got here things went OK, (probably)
            return result;
        }

        private string GetSource()
        {
            if(string.IsNullOrWhiteSpace(ActionComponentGraph.SourceContentPath))
            {
                logger.Log("", LoggingLevel.Verbose);
                logger.Log("ActionComponentGraph.SourceContentPath file not found!", LoggingLevel.Verbose);
                logger.Log("Throwing FileNotFoundException", LoggingLevel.Verbose);
                throw new ArgumentException("source contentpath was empty or not given.");
            }

            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-source:contentPath='{0}'", ActionComponentGraph.SourceContentPath));
        }

        private string GetDestination()
        {
            if (string.IsNullOrWhiteSpace(ActionComponentGraph.DestinationContentPath))
            {
                logger.Log("", LoggingLevel.Verbose);
                logger.Log("ActionComponentGraph.DestinationContentPath file not found!", LoggingLevel.Verbose);
                logger.Log("Throwing FileNotFoundException", LoggingLevel.Verbose);
                throw new ArgumentException("destination contentpath was empty or not given.");
            }

            if (string.IsNullOrWhiteSpace(ActionComponentGraph.DestinationComputerName))
            {
                logger.Log("", LoggingLevel.Verbose);
                logger.Log("ActionComponentGraph.DestinationComputerName file not found!", LoggingLevel.Verbose);
                logger.Log("Throwing FileNotFoundException", LoggingLevel.Verbose);
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
            builder.Append(EnsureStringhasOnlyOneTrailingWhiteSpace("-verb:sync,"));
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