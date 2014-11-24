using System;
using System.Collections.Generic;
using System.Security.Principal;
using DeploymentConfiguration.Actions;
using DeploymentTask.Tasks.LocalTasks;
using DeploymentTask.Tasks.MsDeployTasks;

namespace DeploymentTask.Tasks
{
    /// <summary>
    /// Gives the ability to group all the deployment tasks together whilst using the next layer to be generic for the component graph they use.
    /// </summary>
    public abstract class DeploymentTaskRoot : IComparable<DeploymentTaskRoot>
    {
        protected DeploymentTaskRoot()
        {
            FoldersToBeCleanedUp = new List<string>();
        }

        /// <summary>
        /// Executes a given deployment Task returning the "return code" from the task execution, (such as xcopy return code 0).
        /// </summary>
        /// <returns></returns>
        public abstract int Execute();

        public abstract string DisplayName { get; }
        public abstract int ExpectedReturnValue { get; }
        public abstract bool RequiresAdminRights { get; }

        public IList<string> FoldersToBeCleanedUp { get; set; }

        protected bool IsAdministrator
        {
            get
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public int CompareTo(DeploymentTaskRoot other)
        {
            if (other == null) return -1;


            if (GetType() == typeof(LocalApplicationRemovalIisDeploymentTask) || GetType() == typeof(MsDeployApplicationRemovalIisDeploymentTask))
            {
                if ((GetType() == typeof(LocalApplicationRemovalIisDeploymentTask) && other.GetType() == typeof(LocalApplicationRemovalIisDeploymentTask))
               || (GetType() == typeof(MsDeployApplicationRemovalIisDeploymentTask) && other.GetType() == typeof(MsDeployApplicationRemovalIisDeploymentTask)))
                {
                    return ((DeploymentTaskBase<IisActionComponentGraph>)this).CompareComponentGraph((DeploymentTaskBase<IisActionComponentGraph>)other);
                }

                return -1;
            }


            if (GetType() == typeof(LocalSiteRemovalIisDeploymentTask) || GetType() == typeof(MsDeploySiteRemovalIisDeploymentTask))
            {
                if (other.GetType() == typeof(LocalApplicationRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeployApplicationRemovalIisDeploymentTask))
                {
                    return 1;
                }

                if ((GetType() == typeof(LocalSiteRemovalIisDeploymentTask) && other.GetType() == typeof(LocalSiteRemovalIisDeploymentTask))
                  || (GetType() == typeof(MsDeploySiteRemovalIisDeploymentTask) && other.GetType() == typeof(MsDeploySiteRemovalIisDeploymentTask)))
                {
                    return ((DeploymentTaskBase<IisActionComponentGraph>)this).CompareComponentGraph((DeploymentTaskBase<IisActionComponentGraph>)other);
                }

                return -1;
            }


            if (GetType() == typeof(LocalAppPoolRemovalIisDeploymentTask) || GetType() == typeof(MsDeployAppPoolRemovalIisDeploymentTask))
            {
                if (other.GetType() == typeof(LocalApplicationRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeployApplicationRemovalIisDeploymentTask) ||
                    (other.GetType() == typeof(LocalSiteRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeploySiteRemovalIisDeploymentTask)))
                {
                    return 1;
                }

                if ((GetType() == typeof(LocalAppPoolRemovalIisDeploymentTask) && other.GetType() == typeof(LocalAppPoolRemovalIisDeploymentTask))
                  || (GetType() == typeof(MsDeployAppPoolRemovalIisDeploymentTask) && other.GetType() == typeof(MsDeployAppPoolRemovalIisDeploymentTask)))
                {
                    return ((DeploymentTaskBase<IisActionComponentGraph>)this).CompareComponentGraph((DeploymentTaskBase<IisActionComponentGraph>)other);
                }

                return -1;
            }


            if (GetType() == typeof(LocalFileSystemCopyDeploymentTask) || GetType() == typeof(MsDeployFileCopyDeploymentTask))
            {
                if (other.GetType() == typeof(LocalApplicationRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeployApplicationRemovalIisDeploymentTask) ||
                    (other.GetType() == typeof(LocalSiteRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeploySiteRemovalIisDeploymentTask)) ||
                    (other.GetType() == typeof(LocalAppPoolRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeployAppPoolRemovalIisDeploymentTask)))
                {
                    return 1;
                }

                if ((GetType() == typeof(LocalFileSystemCopyDeploymentTask) && other.GetType() == typeof(LocalFileSystemCopyDeploymentTask))
                  || (GetType() == typeof(MsDeployFileCopyDeploymentTask) && other.GetType() == typeof(MsDeployFileCopyDeploymentTask)))
                {
                    return ((DeploymentTaskBase<FileCopyActionComponentGraph>)this).CompareComponentGraph((DeploymentTaskBase<FileCopyActionComponentGraph>)other);
                }

                return -1;
            }

            if (GetType() == typeof(LocalAppPoolInstallIisDeploymentTask) || GetType() == typeof(MsDeployAppPoolInstallIisDeploymentTask))
            {
                if (other.GetType() == typeof(LocalApplicationRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeployApplicationRemovalIisDeploymentTask) ||
                    (other.GetType() == typeof(LocalSiteRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeploySiteRemovalIisDeploymentTask)) ||
                    (other.GetType() == typeof(LocalAppPoolRemovalIisDeploymentTask) || other.GetType() == typeof(MsDeployAppPoolRemovalIisDeploymentTask)) ||
                    (other.GetType() == typeof(LocalFileSystemCopyDeploymentTask) || other.GetType() == typeof(MsDeployFileCopyDeploymentTask)))
                {
                    return 1;
                }

                if ((GetType() == typeof(LocalAppPoolInstallIisDeploymentTask) && other.GetType() == typeof(LocalAppPoolInstallIisDeploymentTask))
                  || (GetType() == typeof(MsDeployAppPoolInstallIisDeploymentTask) && other.GetType() == typeof(MsDeployAppPoolInstallIisDeploymentTask)))
                {
                    return ((DeploymentTaskBase<IisActionComponentGraph>)this).CompareComponentGraph((DeploymentTaskBase<IisActionComponentGraph>)other);
                }

                return -1;
            }


            //TODO website install compare

            //TODO website application install compare


            return 1;
        }

        public override bool Equals(object obj)
        {
            DeploymentTaskRoot other = obj as DeploymentTaskRoot;
            if (other == null) return false;

            return CompareTo(other) == 0;
        }


        public bool Equals(DeploymentTaskRoot other)
        {
            return !ReferenceEquals(null, other);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}