
namespace DeploymentConfiguration.Actions
{
    public class PackageCreationComponentGraph : ActionComponentGraphBase
    {
        public PackageCreationComponentGraph()
        {
            MsBuildExe = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe";
            ZipFileOnly = false;
            AutoParameterizationWebConfigConnectionStrings = false;
            ShouldPushPackageToRemoteMachine = false;
        }

        public string MsBuildExe { get; set; }
        public string Name { get; set; }
        public string InternalPath { get; set; }
        public string OutputLocation { get; set; }
        public string ConfigurationType { get; set; }
        public bool ZipFileOnly { get; set; }
        public bool AutoParameterizationWebConfigConnectionStrings { get; set; }
        public bool ShouldPushPackageToRemoteMachine { get; set; }
    }
}
