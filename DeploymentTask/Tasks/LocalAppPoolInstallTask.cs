using System.IO;
using DeploymentConfiguration.Actions;

namespace DeploymentTask.Tasks
{
    public class LocalAppPoolInstallTask : AppPoolInstallTaskBase
    {
        public LocalAppPoolInstallTask(IisActionComponentGraph actionComponentGraph)
            : base(actionComponentGraph)
        {
        }

        public override int Execute()
        {
            if (ActionComponentGraph.ForceInstall)
            {
                //TODO create app pool removal task first.
            }



            string cmdFileName = CreateRandomFileName("localAppPoolCmd", "cmd");

            string cmdFileFullPath = Path.Combine(ActionComponentGraph.SourceContentPath, cmdFileName);
            CreateFile(cmdFileFullPath, EnsureStringhasOnlyOneTrailingWhiteSpace(ActionComponentGraph.AppCmdExe) + CreateParameterString(ActionComponentGraph.SourceContentPath).Trim(), true);
            int result = InvokeExe(cmdFileFullPath, string.Empty);

            if (ActionComponentGraph.CleanUp)
            {
                File.Delete(cmdFileFullPath);
            }

            return result;
        }

        public override string DisplayName
        {
            get { return "Local App Pool Installation"; }
        }

        public override int ExpectedReturnValue
        {
            get { return 0; }
        }


    }
}