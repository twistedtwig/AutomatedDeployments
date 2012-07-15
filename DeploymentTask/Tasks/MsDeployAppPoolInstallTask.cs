using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public class MsDeployAppPoolInstallTask : AppPoolInstallTaskBase
    {
        public MsDeployAppPoolInstallTask(IisActionComponentGraph actionComponentGraph) : base(actionComponentGraph)
        {
        }
       
        public override int Execute()
        {
            //ensure config file has been pushed to remote server.
            new MsDeployFileCopyDeploymentTask(CreateConfigFileCopyActionComponentGraphFrom(ActionComponentGraph)).Execute();

            if (ActionComponentGraph.ForceInstall)
            {
                //TODO Create App Pool Removal Task first. - make sure it is executed.
            }
            
            string CreateAppPoolCmdFileName = CreateRandomFileName("CreateAppPool", "cmd");
            string CreateAppPoolCmdPath = Path.Combine(ActionComponentGraph.SourceContentPath, CreateAppPoolCmdFileName);
            // want to create local cmd file to do appcmd
            CreateFile(CreateAppPoolCmdPath, ActionComponentGraph.AppCmdExe + " " + CreateParameterString(ActionComponentGraph.DestinationContentPath), true);
            //ensure cmd file has been pushed to remote server.
            new MsDeployFileCopyDeploymentTask(CreateSingleFileCopyActionComponentGraphFrom(ActionComponentGraph, CreateAppPoolCmdFileName)).Execute();

            // call msdeploy to execute appcmd file on remote machine
            int result = InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployExecuteCmdParams(Path.Combine(ActionComponentGraph.DestinationContentPath, CreateAppPoolCmdFileName)));

            // clean up local and remote files
            if (ActionComponentGraph.CleanUp)
            {
                //delete local file(s)
                File.Delete(CreateAppPoolCmdPath);

                //delete remote file(s)
                int tempResult = InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployDeleteFileParams(Path.Combine(ActionComponentGraph.DestinationContentPath, CreateAppPoolCmdFileName)));
                if(tempResult != 0) result = tempResult;

                InvokeExe(ActionComponentGraph.MsDeployExe, GetMsDeployDeleteFileParams(Path.Combine(ActionComponentGraph.DestinationContentPath, ActionComponentGraph.PathToConfigFile)));
                if (tempResult != 0) result = tempResult;
            }

            return result;
        }

        public override string DisplayName
        {
            get { return "MsDeploy App Pool installation"; }
        }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }
        
    }
}