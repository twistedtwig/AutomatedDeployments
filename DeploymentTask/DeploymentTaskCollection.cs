using System;
using System.Collections.Generic;
using DeploymentConfiguration.Actions;
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

        public DeploymentTaskCollection() : this(false) { }

        public DeploymentTaskCollection(bool breakOnError)
        {
            DeploymentTasks = new List<DeploymentTaskRoot>();
            BreakOnError = breakOnError;
        }

        public IList<DeploymentTaskRoot> DeploymentTasks { get; set; }

        /// <summary>
        /// Execute each Task in turn.
        /// </summary>
        public void Execute()
        {
            if (DeploymentTasks == null) throw new ArgumentException("Deployment Tasks not setup correctly");

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
                        DeploymentTaskException depEx = new DeploymentTaskException(string.Format("unhandled error in '{0}', '{1}'", task.DisplayName, ex.Message), -1);
                        collectionException.DeploymentTaskExceptions.Add(depEx);
                    }

                    if (BreakOnError) throw collectionException;
                }
            }

            if (collectionException != null) throw collectionException;
        }
    }
}
