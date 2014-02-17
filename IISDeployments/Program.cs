using System;
using System.IO;
using DeploymentConfiguration;
using DeploymentConfiguration.Actions;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask;
using DeploymentTask.Exceptions;
using DeploymentTask.Factories;
using Logging;

namespace IISDeployments
{
    public class Program
    {
        private static Logger logger = Logger.GetLogger();

        private static void Main(string[] args)
        {
            try
            {
                SwitchParams consoleParams = ParseParameters(args);

                if (consoleParams.Encrypt.HasValue || consoleParams.Decrypt.HasValue)
                {
                    if (consoleParams.Encrypt.HasValue)
                    {
                        logger.Log("Encryting the deployments section");
                        ConfigurationLoader.EncryptDeployments();                  
                    }

                    if (consoleParams.Decrypt.HasValue)
                    {
                        logger.Log("Decrypting the deployments section");
                        ConfigurationLoader.DecryptDeployments();
                    }

                    return;
                }


                consoleParams.ConfigPath = string.IsNullOrWhiteSpace(consoleParams.ConfigPath) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : consoleParams.ConfigPath;
                if (consoleParams.SetExePath) {  SetDirToExecutingDir(); }

                DisplayConsoleParams(consoleParams);

                DeploymentStrategyComponentGraphBase deploymentComponentGraph = ConfigurationLoader.Load(consoleParams.ConfigSection, consoleParams.ConfigPath, consoleParams.ForceLocal);
                UpdateCompoentGraphWithOverLoads(deploymentComponentGraph, consoleParams);

                DeploymentTaskCollection deploymentTaskCollection = consoleParams.BreakOnError.HasValue
                    ? DeploymentTaskFactory.Create(consoleParams.BreakOnError.Value, deploymentComponentGraph)
                    : DeploymentTaskFactory.Create(deploymentComponentGraph);
                deploymentTaskCollection.Execute();
            }
            catch (DeploymentCollectionException exception)
            {                
                logger.Log(exception.DeploymentTaskExceptions.Count <= 1 ? "There was an error" : "There were multiple errors");
                foreach (DeploymentTaskException taskException in exception.DeploymentTaskExceptions)
                {
                    logger.Log(taskException.Message + " - " + taskException.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Log("something bad happened: " + ex.Message);
            }            
        }

        private static SwitchParams ParseParameters(string[] args)
        {
            SwitchParams consoleParams = new SwitchParams();
            if (args == null || args.Length == 0) return consoleParams;

            try
            {

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].Trim().ToUpper())
                    {
                        case "/CONFIGSECTION":
                            if (args.Length > i)
                            {
                                consoleParams.ConfigSection = args[++i];
                                logger.Log("ConfigSection set to: " + consoleParams.ConfigSection);
                            }
                            break;
                        case "/CONFIGPATH":
                            if (args.Length > i)
                            {
                                consoleParams.ConfigPath = args[++i];
                                logger.Log("ConfigPath set to: " + consoleParams.ConfigPath);
                            }
                            break;
                        case "/FORCE":
                            if (args.Length > i)
                            {
                                bool val;
                                if (bool.TryParse(args[++i], out val))
                                {
                                    consoleParams.Force = val;
                                    logger.Log("Force install set to: " + consoleParams.Force);
                                }
                            }
                            break;
                        case "/BREAKONERROR":
                            if (args.Length > i)
                            {
                                bool val;
                                if (bool.TryParse(args[++i], out val))
                                {
                                    consoleParams.BreakOnError = val;
                                    logger.Log("Break on Error set to: " + consoleParams.BreakOnError);
                                }
                            }
                            break;
                        case "/CLEANUP":
                            if (args.Length > i)
                            {
                                bool val;
                                if (bool.TryParse(args[++i], out val))
                                {
                                    consoleParams.CleanUp = val;
                                    logger.Log("Clean up set to: " + consoleParams.CleanUp);
                                }
                            }
                            break;
                        case "/VERBOSE":
                            if (args.Length > i)
                            {
                                bool val;
                                if (bool.TryParse(args[++i], out val))
                                {
                                    consoleParams.Verbose = val;
                                    logger.LoggingLevel = consoleParams.Verbose.Value ? LoggingLevel.Verbose : LoggingLevel.Normal;
                                    logger.Log("Verbose logging set to: " + logger.LoggingLevel);
                                }
                            }
                            break;
                        case "/FORCELOCAL":
                            if (args.Length > i)
                            {
                                bool val;
                                if (bool.TryParse(args[++i], out val))
                                {
                                    consoleParams.ForceLocal = val;
                                    logger.Log("Force local set to: " + consoleParams.ForceLocal);
                                }
                            }
                            else
                            {
                                consoleParams.ForceLocal = true;
                            }
                            break;
                        case "/ENCRYPT":
                            consoleParams.Encrypt = true;
                            break;
                        case "/DECRYPT":
                            consoleParams.Decrypt = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log("error while reading params" + ex.Message);
                logger.Log("will continue with values gained so far");
            }

            return consoleParams;
        }

        private static void DisplayConsoleParams(SwitchParams consoleParams)
        {
            if(consoleParams.BreakOnError.HasValue)
            {
                logger.Log(string.Format("BreakOn Error: {0}", consoleParams.BreakOnError));
            }
            if (consoleParams.Verbose.HasValue)
            {
                logger.Log(string.Format("Verbose logging: {0}", consoleParams.Verbose));
            }
            if(consoleParams.CleanUp.HasValue)
            {
                logger.Log(string.Format("Clean up: {0}", consoleParams.CleanUp));
            }
            if(!string.IsNullOrWhiteSpace(consoleParams.ConfigPath))
            {
                logger.Log(string.Format("config path: {0}", consoleParams.ConfigPath));
            }
            if(!string.IsNullOrWhiteSpace(consoleParams.ConfigSection))
            {
                logger.Log(string.Format("config section: {0}", consoleParams.ConfigSection));
            }
            if(consoleParams.Force.HasValue)
            {
                logger.Log(string.Format("Force action: {0}", consoleParams.Force));
            }

            logger.Log(string.Format("Setting current Directory to location of EXE (default=true): {0}", consoleParams.SetExePath));                
            logger.Log("Working dir: {0}", Directory.GetCurrentDirectory());

            logger.Log("");
        }

        private static void UpdateCompoentGraphWithOverLoads(DeploymentStrategyComponentGraphBase deploymentComponentGraph, SwitchParams consoleParams)
        {
            foreach (ActionComponentGraphBase action in deploymentComponentGraph.Actions)
            {
                if (consoleParams.CleanUp.HasValue) { action.CleanUp = consoleParams.CleanUp.Value; }
                if (consoleParams.Force.HasValue) { action.ForceInstall = consoleParams.Force.Value; }                
            }
        }

        private static void SetDirToExecutingDir()
        {
            string fullName = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
            string path = fullName.Substring(8, (fullName.LastIndexOf(@"/") - 8));
            Directory.SetCurrentDirectory(path);
        }

    }
}