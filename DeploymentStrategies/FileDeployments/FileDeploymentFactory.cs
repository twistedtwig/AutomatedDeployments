using System;
using InstallerComponents;

namespace DeploymentStrategies.FileDeployments
{
    public class FileDeploymentFactory
    {

        public static FileDeploymentstrategyBase Create(FileComponentBaseGraph fileComponentGraph)
        {
            if (fileComponentGraph.ComponentTypeEnum == ComponentType.FileInstaller)
            {
                FileDeploymentStrategyType fileDeploymentStrategyType = ParseToStrategy(fileComponentGraph.DeploymentType);
                switch (fileDeploymentStrategyType)
                {
                    case FileDeploymentStrategyType.MsDeploy:
                        return new MsDeployFileInstallerDeploymentStrategy(fileComponentGraph);
                    case FileDeploymentStrategyType.FTP:
                        break;
                    case FileDeploymentStrategyType.XCopy:
                        break;
                }

                throw new ArgumentException("unknown strategy name given: " + fileDeploymentStrategyType);
            }
            if (fileComponentGraph.ComponentTypeEnum == ComponentType.FileRemover)
            {
                FileDeploymentStrategyType fileDeploymentStrategyType = ParseToStrategy(fileComponentGraph.DeploymentType);
                switch (fileDeploymentStrategyType)
                {
                    case FileDeploymentStrategyType.MsDeploy:
                        return new MsDeployFileRemovalDeploymentStrategypublic(fileComponentGraph);
                    case FileDeploymentStrategyType.FTP:
                        break;
                    case FileDeploymentStrategyType.XCopy:
                        break;
                }

                throw new ArgumentException("unknown strategy name given: " + fileDeploymentStrategyType);
            }            

            throw new ArgumentException("unable to determine file deployment strategy required for: " + fileComponentGraph.Name);
        }

        public static FileDeploymentStrategyType ParseToStrategy(string strategyName)
        {
            FileDeploymentStrategyType strategyType;
            bool parse = Enum.TryParse(strategyName, true, out strategyType);
            if (parse)
            {
                return strategyType;
            }

            throw new ArgumentException("unknown strategy name given: " + strategyName);
        }
    }
}
