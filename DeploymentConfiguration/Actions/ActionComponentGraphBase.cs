namespace DeploymentConfiguration.Actions
{
    public abstract class ActionComponentGraphBase
    {
        public ActionType ActionType { get; set; }
        public bool ForceInstall { get; set; }

        public string SourceContentPath { get; set; }
        public string DestinationContentPath { get; set; }
    }
}