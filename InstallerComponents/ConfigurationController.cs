using System;
using System.Collections.Generic;
using System.Linq;
using CustomConfigurations;

namespace InstallerComponents
{
    public class ConfigurationController
    {
        public static IList<ComponentGraph> LoadInstallerComponentGraphs()
        {
            Config deployments = new Config(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            if (deployments == null)
            {
                throw new ApplicationException("could not load configuraiton");
            }
            List<string> sectionNames = deployments.SectionNames.ToList();
            if (sectionNames == null || !sectionNames.Any())
            {
                throw new ApplicationException("no sections deployments were found in the configuration file.");
            }

            if (sectionNames.Count() != deployments.Count)
            {
                throw new ArgumentException("error loading configuration, number of deployments vs sections name differ");
            }

            IList<ComponentGraph> componentGraphs = new List<ComponentGraph>(deployments.Count);

            for (int i = 0; i < deployments.Count; i++)
            {
                ComponentGraph component = new ComponentGraph();
                string deploymentName = sectionNames[i];
                Console.WriteLine("Reading Configuration group '{0}'", deploymentName);      
                
                ConfigSection deployment = deployments.GetSection(deploymentName);
                if (deployment == null)
                {
                    Console.WriteLine("Error reading configuraiton group '{0}'", deploymentName);
                    continue;
                }

                IDictionary<string, string> globalValues = deployment.ValuesAsDictionary;

                component.Index = deployment.Index; //set index of all items for sorting later.
                List<ConfigSection> deploymentConfigCollections = deployment.Collections.GetCollections().ToList();
                for (int index = 0; index < deploymentConfigCollections.Count(); index++)
                {
                    ConfigSection installerSection = deploymentConfigCollections[index];
                    Console.WriteLine("Reading Configuration component section '{0}'", installerSection.Name);
                    var item = ComponentFactory.Create(globalValues, installerSection, index);
                    if (item != null)
                    {
                        component.Components.Add(item);
                    }
                }

                componentGraphs.Add(component);
            }

            return componentGraphs;
        }
    }
}
