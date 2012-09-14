using System;
using DeploymentConfiguration.Actions;
using DeploymentTask.Tasks;
using DeploymentTask.Tasks.LocalTasks;
using DeploymentTask.Tasks.MsDeployTasks;

namespace DeploymentTask.Factories
{
    public enum DeploymentType
    {
        local,
        msDeploy,
    }

    public class PackageCreationTaskFactory
    {
        public static DeploymentTaskRoot Create(PackageCreationComponentGraph action, DeploymentType deploymentType)
        {
            if (action.SourceContentPath.EndsWith(@".csproj")
                || action.SourceContentPath.EndsWith(@".vbproj")
                || action.SourceContentPath.EndsWith(@".wdproj"))
            {
                if (deploymentType == DeploymentType.local)
                {
                    return new LocalProjectPackgeCreationTask(action);
                }

                if (deploymentType == DeploymentType.msDeploy)
                {
                    return new MsDeployProjectPackgeCreationTask(action);
                }

                throw new ArgumentOutOfRangeException("unknown deployment type for project package creation");
            }

            if (deploymentType == DeploymentType.local)
            {
                return new LocalFolderPackgeCreationTask(action);
            }

            if (deploymentType == DeploymentType.msDeploy)
            {
                return new MsDeployFolderPackgeCreationTask(action);
            }

            throw new ArgumentOutOfRangeException("unknown deployment type for folder package creation");
        }
    }
}