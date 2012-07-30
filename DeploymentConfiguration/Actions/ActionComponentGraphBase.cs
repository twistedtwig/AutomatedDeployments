namespace DeploymentConfiguration.Actions
{
    public abstract class ActionComponentGraphBase
    {
        protected ActionComponentGraphBase()
        {
            AppCmdExe = @"C:\Windows\System32\inetsrv\appcmd.exe";
            MsDeployExe = @"C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe";

            SourceContentPath = string.Empty;
            DestinationContentPath = string.Empty;

            CleanUp = true;
            ForceInstall = false;
        }

        public ActionType ActionType { get; set; }
        public bool ForceInstall { get; set; }
        public bool CleanUp { get; set; }

        public string SourceContentPath { get; set; }
        public string DestinationContentPath { get; set; }

        public string AppCmdExe { get; set; }        
        public string MsDeployExe { get; set; }

        public string DestinationComputerName { get; set; }
        public string DestinationUserName { get; set; }
        public string DestinationPassword { get; set; }
    }
}