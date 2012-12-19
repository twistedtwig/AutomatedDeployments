using System.Text;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public static class MsDeployTaskExtensions
    {
        public static string GetMsDeployExecuteCmdSource(string pathToCmd)
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-source:runCommand='{0}'", pathToCmd));
        }

        public static string GetMsDeployExecuteCmdDestination(ActionComponentGraphBase ActionComponentGraph)
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-dest:auto,computerName='{0}',userName='{1}',password='{2}'",
                ActionComponentGraph.DestinationComputerName, ActionComponentGraph.DestinationUserName, ActionComponentGraph.DestinationPassword));
        }

        public static string GetMsDeployExecuteCmdSync()
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace("-verb:sync");
        }

        private static string EnsureStringhasOnlyOneTrailingWhiteSpace(string value)
        {
            return value.Trim() + " ";
        }


        public static string GetMsDeployExecuteCmdParams(ActionComponentGraphBase actionComponentGraph, string pathToCmd)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append(MsDeployTaskExtensions.GetMsDeployExecuteCmdSource(pathToCmd));
            parameters.Append(MsDeployTaskExtensions.GetMsDeployExecuteCmdDestination(actionComponentGraph));
            parameters.Append(MsDeployTaskExtensions.GetMsDeployExecuteCmdSync());

            return parameters.ToString();
        }

        public static string GetMsDeployDeleteFileParams(ActionComponentGraphBase actionComponentGraph, string filePath)
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-dest:ContentPath='{0}',computerName='{1}',userName='{2}',password='{3}' -verb:delete",
                filePath, actionComponentGraph.DestinationComputerName, actionComponentGraph.DestinationUserName, actionComponentGraph.DestinationPassword));
        }
        

    }
}
