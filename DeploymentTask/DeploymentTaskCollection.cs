using System;
using System.Collections.Generic;
using System.IO;
using DeploymentTask.Exceptions;
using DeploymentTask.Tasks;

namespace DeploymentTask
{
    /// <summary>
    /// Collection of Deployment tasks. Will execute them in turn.  If set to break on exception then will return straight away, otherwise errors will be collected and returned at the end.
    /// </summary>
    public class DeploymentTaskCollection
    {
        private readonly bool BreakOnError;
        private readonly bool ShouldSortTasks;
        
        public DeploymentTaskCollection(bool breakOnError, bool shouldSortTasks)
        {
            DeploymentTasks = new List<DeploymentTaskRoot>();
            BreakOnError = breakOnError;
            ShouldSortTasks = shouldSortTasks;
        }

        /// <summary>
        /// Execute each Task in turn.
        /// </summary>
        public void Execute()
        {
            if (DeploymentTasks == null) throw new ArgumentException("Deployment Tasks not setup correctly");
            if(ShouldSortTasks) { Sort(); }

            var foldersToBeRemoved = new List<string>();

            DeploymentCollectionException collectionException = null;
            foreach (DeploymentTaskRoot task in DeploymentTasks)
            {
                try
                {
                    int taskResult = task.Execute();
                    if (taskResult != task.ExpectedReturnValue)
                    {
                        throw new DeploymentTaskException(string.Format("{0} failed with exit code: {1}", task.DisplayName, taskResult), taskResult);
                    }
                }
                catch (Exception ex)
                {
                    if (collectionException == null) collectionException = new DeploymentCollectionException();

                    if (ex is DeploymentTaskException)
                    {
                        collectionException.DeploymentTaskExceptions.Add((DeploymentTaskException) ex);                        
                    }
                    else
                    {
                        var depEx = new DeploymentTaskException(string.Format("unhandled error in '{0}', '{1}'", task.DisplayName, ex.Message), -1);
                        collectionException.DeploymentTaskExceptions.Add(depEx);
                    }

                    if (BreakOnError) throw collectionException;
                }

                foldersToBeRemoved.AddRange(task.FoldersToBeCleanedUp);
            }

            CleanUpTempFolders(foldersToBeRemoved);
            
            if (collectionException != null) throw collectionException;
        }

        private void CleanUpTempFolders(IEnumerable<string> foldersToBeRemoved)
        {
            foreach (var path in foldersToBeRemoved)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
        }

        protected List<DeploymentTaskRoot> DeploymentTasks { get; private set; }

        public void Add(DeploymentTaskRoot item)
        {
            if (!DeploymentTasks.Contains(item))
            {
                DeploymentTasks.Add(item);
            }
        }

        public int Count { get { return DeploymentTasks.Count; } }

        public DeploymentTaskRoot this[int index] { get { return DeploymentTasks[index]; } }

        public void Sort()
        {
            DeploymentTasks.Sort();
        }
    }
}
