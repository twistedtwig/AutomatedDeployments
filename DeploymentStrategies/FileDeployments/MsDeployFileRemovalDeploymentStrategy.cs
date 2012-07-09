using System;
using System.IO;
using System.Text;
using InstallerComponents;

namespace DeploymentStrategies.FileDeployments
{
    public class MsDeployFileRemovalDeploymentStrategypublic : FileDeploymentstrategyBase
    {
        protected FileRemoverComponentGraph FileRemoverComponentGraph;

        public MsDeployFileRemovalDeploymentStrategypublic(FileComponentBaseGraph fileComponentGraph) : base(fileComponentGraph)
        {
            if (!(fileComponentGraph is FileRemoverComponentGraph))
            {
                throw new ArgumentException("wrong file component graph type");
            }

            FileRemoverComponentGraph = fileComponentGraph as FileRemoverComponentGraph;
        }

        /// <summary>
        /// will push the files to the destination
        /// </summary>
        public override void PushFiles()
        {
            string msDeployExe = string.Empty;            
            if (!string.IsNullOrWhiteSpace(FileComponentGraph.MsdeployExe))
            {
                msDeployExe = FileComponentGraph.MsdeployExe;
            }

            if(!File.Exists(msDeployExe))
            {
                throw new FileNotFoundException("msdeploy.exe was not found");
            }

            if (!ValidateParameters(FileComponentGraph))
            {
                throw new ArgumentException("file component doesn't have all the required information.");
            }

            StringBuilder builder = new StringBuilder();
            if (FileRemoverComponentGraph.ShouldDeleteRmoteFolder)
            {
                builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(GetDestinationDeleteDirectoryAndChildren(FileComponentGraph)));                
            }
            else
            {
                builder.Append(EnsureOutputParameterStringHasOneAndOnlyOneSpaceCharacterAtEnd(GetDestinationDeleteFilesOnly(FileComponentGraph)));                
            }
            builder.Append(GetSync());

            Console.WriteLine(StartSectionBreaker);
            Console.WriteLine("Executing MSDEPLOY File removal command:");
            Console.WriteLine(builder.ToString());

            InvokeExe(msDeployExe, builder.ToString());

            Console.WriteLine("Completed file removal.");
            Console.WriteLine(EndSectionBreaker);

            //"C:\Program Files\IIS\Microsoft Web Deploy V2\\msdeploy.exe" 
            //-source:contentPath='C:\Temp\deploy' 
            //-dest:contentPath='C:\websites\installer',computerName='192.168.10.98:8172',userName='administrator',password='wiggle_4_it' 
            //-verb:sync
        }

        /// <summary>
        /// ensures that the parameters passed in are all valid and all parameters required have been given
        /// </summary>
        /// <param name="fileComponentGraph"> </param>
        /// <returns></returns>
        public override bool ValidateParameters(FileComponentBaseGraph fileComponentGraph)
        {
            if (fileComponentGraph == null)
                return false;

            if (string.IsNullOrWhiteSpace(fileComponentGraph.DestinationContentPath))
            {
                return false;
            }
            
            return true;
        }

        private string GetDestinationDeleteFilesOnly(FileComponentBaseGraph fileComponentGraph)
        {
            if (string.IsNullOrWhiteSpace(fileComponentGraph.DestinationContentPath))
            {
                throw new ArgumentException("destination contentpath was empty or not given.");
            }

            if (string.IsNullOrWhiteSpace(fileComponentGraph.DestinationComputerName))
            {
                throw new ArgumentException("destination comptuername was empty or not given.");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(string.Format("-source:contentPath={0} ", GetEmptyFolderPath()));

            builder.Append("-dest:");
            builder.Append("contentPath='" + fileComponentGraph.DestinationContentPath + "'");
            builder.Append(",computername='" + fileComponentGraph.DestinationComputerName + "'");

            if (!string.IsNullOrWhiteSpace(fileComponentGraph.DestinationUserName))
            {
                builder.Append(",username='" + fileComponentGraph.DestinationUserName + "'");
            }

            if (!string.IsNullOrWhiteSpace(fileComponentGraph.DestinationPassword))
            {
                builder.Append(",password='" + fileComponentGraph.DestinationPassword + "'");
            }

            return builder.ToString();
        }
        
        private string GetDestinationDeleteDirectoryAndChildren(FileComponentBaseGraph fileComponentGraph)
        {
            if (string.IsNullOrWhiteSpace(fileComponentGraph.DestinationContentPath))
            {
                throw new ArgumentException("destination contentpath was empty or not given.");
            }

            if (string.IsNullOrWhiteSpace(fileComponentGraph.DestinationComputerName))
            {
                throw new ArgumentException("destination comptuername was empty or not given.");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("-dest:");
            builder.Append("dirpath='" + fileComponentGraph.DestinationContentPath + "'");
            builder.Append(",computername='" + fileComponentGraph.DestinationComputerName + "'");

            if (!string.IsNullOrWhiteSpace(fileComponentGraph.DestinationUserName))
            {
                builder.Append(",username='" + fileComponentGraph.DestinationUserName + "'");
            }

            if (!string.IsNullOrWhiteSpace(fileComponentGraph.DestinationPassword))
            {
                builder.Append(",password='" + fileComponentGraph.DestinationPassword + "'");
            }

            return builder.ToString();
        }

        private string GetSync()
        {
            return FileRemoverComponentGraph.ShouldDeleteRmoteFolder ? "-verb:delete" : "-verb:sync";
        }

        public override void Execute()
        {
            PushFiles();
        }

        public override bool ValidateParameters()
        {
            if (FileComponentGraph == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(FileComponentGraph.MsdeployExe))
            {
                return false;
            }


            if (string.IsNullOrWhiteSpace(FileComponentGraph.DestinationComputerName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(FileComponentGraph.SourceContentPath))
            {
                return false;
            }            

            return true;
        }
    }
}
