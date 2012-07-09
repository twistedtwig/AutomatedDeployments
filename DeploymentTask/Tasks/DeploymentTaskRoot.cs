namespace DeploymentTask.Tasks
{
    /// <summary>
    /// Gives the ability to group all the deployment tasks together whilst using the next layer to be generic for the component graph they use.
    /// </summary>
    public abstract class DeploymentTaskRoot
    {
        /// <summary>
        /// Executes a given deployment Task returning the "return code" from the task execution, (such as xcopy return code 0).
        /// </summary>
        /// <returns></returns>
        public abstract int Execute();

        public abstract string DisplayName { get; }
        public abstract int ExpectedReturnValue { get; }
    }
}