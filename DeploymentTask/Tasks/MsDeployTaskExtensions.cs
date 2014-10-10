using System.Text;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public static class MsDeployTaskExtensions
    {
        public static string GetMsDeployExecuteCmdSource(string pathToCmd)
        {
            var runCmd = string.Format("-source:runCommand='{0}'", pathToCmd);

            if (pathToCmd.EndsWith(".exe"))
            {
                runCmd += ",dontUseCommandExe=true";
            }

            return EnsureStringhasOnlyOneTrailingWhiteSpace(runCmd);
        }

        public static string GetMsDeployExecuteCmdDestination(ActionComponentGraphBase ActionComponentGraph)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("-dest:auto,computerName='{0}'",ActionComponentGraph.DestinationComputerName));

            if (!string.IsNullOrWhiteSpace(ActionComponentGraph.AuthType))
            {
                builder.Append(string.Format(",authType={0}", ActionComponentGraph.AuthType));                
            }

            builder.Append(string.Format(",userName={0}", ActionComponentGraph.DestinationUserName));
            builder.Append(string.Format(",password={0}", ActionComponentGraph.DestinationPassword));                

            return EnsureStringhasOnlyOneTrailingWhiteSpace(builder.ToString());
        }

        public static string GetMsDeployExecuteCmdSync()
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace("-verb:sync");
        }

        public static string GetMsDeployAllowUntrusted(ActionComponentGraphBase actionComponentGraph)
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-allowUntrusted:{0} ", actionComponentGraph.AllowUntrusted));
        }

        public static string EnsureStringhasOnlyOneTrailingWhiteSpace(string value)
        {
            return value.Trim() + " ";
        }


        public static string GetMsDeployExecuteCmdParams(ActionComponentGraphBase actionComponentGraph, string pathToCmd)
        {
            var parameters = new StringBuilder();
            parameters.Append(GetMsDeployExecuteCmdSource(pathToCmd));
            parameters.Append(GetMsDeployExecuteCmdDestination(actionComponentGraph));
            parameters.Append(GetMsDeployExecuteCmdSync());
            parameters.Append(GetMsDeployAllowUntrusted(actionComponentGraph));

            return parameters.ToString();
        }

        public static string GetMsDeployDeleteFileParams(ActionComponentGraphBase actionComponentGraph, string filePath)
        {
            return EnsureStringhasOnlyOneTrailingWhiteSpace(string.Format("-dest:ContentPath='{0}',computerName='{1}',userName='{2}',password='{3}' -verb:delete",
                filePath, actionComponentGraph.DestinationComputerName, actionComponentGraph.DestinationUserName, actionComponentGraph.DestinationPassword));
        }
        

    }
}
