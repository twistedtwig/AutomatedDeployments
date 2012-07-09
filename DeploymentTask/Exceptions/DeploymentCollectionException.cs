using System;
using System.Collections.Generic;

namespace DeploymentTask.Exceptions
{
    public class DeploymentCollectionException : Exception
    {
        public DeploymentCollectionException() : this(null) { }

        public DeploymentCollectionException(Exception innerException) :base(string.Empty, innerException)
        {
            DeploymentTaskExceptions = new List<DeploymentTaskException>();            
        }

        public IList<DeploymentTaskException> DeploymentTaskExceptions { get; set; }
    }
}
