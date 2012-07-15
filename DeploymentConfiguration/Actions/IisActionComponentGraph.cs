namespace DeploymentConfiguration.Actions
{
    public class IisActionComponentGraph : ActionComponentGraphBase
    {
        public string PathToConfigFile { get; set; }
        
        public IisActionComponentGraph Clone()
        {
            var newComponentGraph = new IisActionComponentGraph();
            newComponentGraph.ActionType = ActionType;
            newComponentGraph.AppCmdExe = AppCmdExe;
            newComponentGraph.CleanUp = CleanUp;
            newComponentGraph.DestinationComputerName = DestinationComputerName;
            newComponentGraph.DestinationContentPath = DestinationContentPath;
            newComponentGraph.DestinationPassword = DestinationPassword;
            newComponentGraph.DestinationUserName = DestinationUserName;
            newComponentGraph.ForceInstall = ForceInstall;
            newComponentGraph.MsDeployExe = MsDeployExe;
            newComponentGraph.PathToConfigFile = PathToConfigFile;
            newComponentGraph.SourceContentPath = SourceContentPath;

            return newComponentGraph;
        }
    }
}