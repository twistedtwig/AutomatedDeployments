using System;
using System.Collections.Generic;
using CustomConfigurations;

namespace InstallerComponents
{
    public class ComponentFactory
    {
        private const string ComponentTypeString = "ComponentType";

        public static ComponentBase Create(IDictionary<string, string> globalValues, ConfigSection section, int index)
        {
            if (!section.ContainsKey(ComponentTypeString))
            {
                throw new ArgumentException("No ComponentType given for section: " + section.Name);
            }

            //assign all global values to the local configSection.
            IDictionary<string, string> localValues = section.ValuesAsDictionary;
            foreach (KeyValuePair<string, string> globalValue in globalValues)
            {
                if (!localValues.ContainsKey(globalValue.Key))
                {
                    localValues.Add(globalValue.Key, globalValue.Value);
                }
            }

            ComponentBase item = null;

            switch (ParseToComponentType(section[ComponentTypeString]))
            {
                case ComponentType.FileInstaller:
                    item = new  FileInstallerComponentGraph(section.Name, localValues);
                    break;
                case ComponentType.FileRemover:
                    item = new FileRemoverComponentGraph(section.Name, localValues);
                    break;
                case ComponentType.AppPoolInstaller:
                    item = new AppPoolInstallerComponentGraph(section.Name, localValues);
                    break;
                case ComponentType.WebSiteInstaller:
                case ComponentType.ApplicationInstaller:
                    item =  null;
                    break;
                case ComponentType.RemoteExecutionComponent:
                    item = new RemoteExecutionComponentGraph(section.Name, localValues);
                    break;
                case ComponentType.AppPoolRemover:
                    item = new IisRemoverComponentGraph(section.Name, localValues, ComponentType.AppPoolRemover);
                    break;
                case ComponentType.WebSiteRemover:
                    item = new IisRemoverComponentGraph(section.Name, localValues, ComponentType.WebSiteRemover);
                    break;
                case ComponentType.ApplicationRemover:
                    item = new IisRemoverComponentGraph(section.Name, localValues, ComponentType.ApplicationRemover);
                    break;
                default:
                    throw new ArgumentException("unknown installer section: " + section[ComponentTypeString]);                
            }

            if (item != null)
            {
                item.Index = index;
            }

            return item;
        }

        public static ComponentType ParseToComponentType(string name)
        {
            ComponentType componentType;
            if (Enum.TryParse(name, true, out componentType))
            {
                return componentType;
            }

            throw new ArgumentException("unknown installer componentType: " + name);
        }
    }
}
