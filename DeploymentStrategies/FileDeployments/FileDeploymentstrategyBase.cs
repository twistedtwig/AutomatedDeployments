using System;
using System.Globalization;
using System.IO;
using InstallerComponents;

namespace DeploymentStrategies.FileDeployments
{
    /// <summary>
    /// Helper class to deal with common deployment situations, such as File IO.
    /// </summary>
    public abstract class FileDeploymentstrategyBase : DeploymentStrategy
    {
        protected FileComponentBaseGraph FileComponentGraph;

        protected FileDeploymentstrategyBase(FileComponentBaseGraph fileComponentGraph) : base(RemoteDeploymentStrategyType.FileDeployment, fileComponentGraph.Index)
        {
            FileComponentGraph = fileComponentGraph;
        }
        /// <summary>
        /// Gets the source location from the parameters.  Will ensure that it is a valid file or directory, 
        /// if none is given it will take the currently executing directory as the source.
        /// If one is given but is not valid on the executing machine a DirectoryNotFoundException will be thrown.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException">invalid file or directory given</exception>
        protected string GetSourceLocation(string sourceParamName)
        {
            string sourceLocation;
            if (string.IsNullOrWhiteSpace(sourceParamName))
            {
                sourceLocation = Directory.GetCurrentDirectory();
            }            
            else
            {
                string tempSource = sourceParamName;
                if (Directory.Exists(tempSource) || File.Exists(tempSource))
                {
                    sourceLocation = tempSource;
                }
                else
                {
                    throw new DirectoryNotFoundException();
                }
            }
            return sourceLocation;
        }

        protected string GetEmptyFolderPath()
        {
            string tempPath = Path.GetTempPath();
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            string innerFolder = Path.Combine(tempPath, DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            if (Directory.Exists(innerFolder))
            {
                Directory.Delete(innerFolder);
            }
            Directory.CreateDirectory(innerFolder);
            return innerFolder;
        }


        /// <summary>
        /// will push the files to the destination
        /// </summary>
        public abstract void PushFiles();

        /// <summary>
        /// ensures that the parameters passed in are all valid and all parameters required have been given
        /// </summary>        
        /// <param name="fileComponentGraph"> </param>
        /// <returns></returns>
        public abstract bool ValidateParameters(FileComponentBaseGraph fileComponentGraph);
    }
}
