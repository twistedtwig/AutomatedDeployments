using System.Collections.Generic;

namespace DeploymentConfiguration.Actions
{
    public abstract class ActionComponentGraphBase
    {
        protected ActionComponentGraphBase()
        {
            MsDeployExeLocations = new List<string> 
            { 
                @"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe", 
                @"C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe", 
                @"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe", 
                @"C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe"
            };
            AppCmdExe = @"C:\Windows\System32\inetsrv\appcmd.exe";
            
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
        public List<string> MsDeployExeLocations { get; set; }

        public string DestinationComputerName { get; set; }
        public string DestinationUserName { get; set; }
        public string DestinationPassword { get; set; }
    }
}