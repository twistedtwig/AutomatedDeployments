using System;

namespace DeploymentTask.Exceptions
{
    public class DeploymentTaskException : Exception
    {
        public DeploymentTaskException() { }
        public DeploymentTaskException(string message, int returnValue) : this(message, returnValue, null) { }
        public DeploymentTaskException(string message, int returnValue, Exception ex) : base(message, ex)
        {
            ErrorMessage = message;
            ReturnValue = returnValue;
        }

        public int ReturnValue { get; set; }
        public String ErrorMessage { get; set; }
    }
}