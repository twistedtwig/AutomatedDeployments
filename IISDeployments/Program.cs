using System;
using DeploymentConfiguration;
using DeploymentConfiguration.Actions;
using DeploymentConfiguration.DeploymentStrategies;
using DeploymentTask;
using DeploymentTask.Exceptions;
using DeploymentTask.Factories;

namespace IISDeployments
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                SwitchParams consoleParams = ParseParameters(args);

                consoleParams.ConfigPath = string.IsNullOrWhiteSpace(consoleParams.ConfigPath) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : consoleParams.ConfigPath;

                DisplayConsoleParams(consoleParams);

                DeploymentStrategyComponentGraphBase deploymentComponentGraph = ConfigurationLoader.Load(consoleParams.ConfigSection, consoleParams.ConfigPath);
                UpdateCompoentGraphWithOverLoads(deploymentComponentGraph, consoleParams);

                DeploymentTaskCollection deploymentTaskCollection = consoleParams.BreakOnError.HasValue
                    ? DeploymentTaskFactory.Create(consoleParams.BreakOnError.Value, deploymentComponentGraph)
                    : DeploymentTaskFactory.Create(deploymentComponentGraph);
                deploymentTaskCollection.Execute();
            }
            catch (DeploymentCollectionException exception)
            {                
                Console.WriteLine(exception.DeploymentTaskExceptions.Count <= 1 ? "There was an error" : "There were multiple errors");
                foreach (DeploymentTaskException taskException in exception.DeploymentTaskExceptions)
                {
                    Console.WriteLine(taskException.Message + " - " + taskException.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("something bad happened: " + ex.Message);
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
                                Console.WriteLine("ConfigSection set to: " + consoleParams.ConfigSection);
                            }
                            break;
                        case "/CONFIGPATH":
                            if (args.Length > i)
                            {
                                consoleParams.ConfigPath = args[++i];
                                Console.WriteLine("ConfigPath set to: " + consoleParams.ConfigPath);
                            }
                            break;
                        case "/FORCE":
                            if (args.Length > i)
                            {
                                bool val;
                                if (bool.TryParse(args[++i], out val))
                                {
                                    consoleParams.Force = val;
                                    Console.WriteLine("Force install set to: " + consoleParams.Force);
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
                                    Console.WriteLine("Break on Error set to: " + consoleParams.BreakOnError);
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
                                    Console.WriteLine("Clean up set to: " + consoleParams.CleanUp);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error while reading params" + ex.Message);
                Console.WriteLine("will continue with values gained so far");
            }

            return consoleParams;
        }

        private static void DisplayConsoleParams(SwitchParams consoleParams)
        {
            if(consoleParams.BreakOnError.HasValue)
            {
                Console.WriteLine(string.Format("BreakOn Error: {0}", consoleParams.BreakOnError));
            }
            if(consoleParams.CleanUp.HasValue)
            {
                Console.WriteLine(string.Format("Clean up: {0}", consoleParams.CleanUp));
            }
            if(!string.IsNullOrWhiteSpace(consoleParams.ConfigPath))
            {
                Console.WriteLine(string.Format("config path: {0}", consoleParams.ConfigPath));
            }
            if(!string.IsNullOrWhiteSpace(consoleParams.ConfigSection))
            {
                Console.WriteLine(string.Format("config section: {0}", consoleParams.ConfigSection));
            }
            if(consoleParams.Force.HasValue)
            {
                Console.WriteLine(string.Format("Force action: {0}", consoleParams.Force));
            }

            Console.WriteLine();
        }

        private static void UpdateCompoentGraphWithOverLoads(DeploymentStrategyComponentGraphBase deploymentComponentGraph, SwitchParams consoleParams)
        {
            foreach (ActionComponentGraphBase action in deploymentComponentGraph.Actions)
            {
                if (consoleParams.CleanUp.HasValue) { action.CleanUp = consoleParams.CleanUp.Value; }
                if (consoleParams.Force.HasValue) { action.ForceInstall = consoleParams.Force.Value; }                
            }
        }

    }
}