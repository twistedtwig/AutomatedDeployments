using System.Collections.Generic;

namespace InstallerComponents
{
    public class ComponentGraph
    {
        public ComponentGraph()
        {            
            Components = new List<ComponentBase>();
        }

        public IList<ComponentBase> Components { get; set; }
        public int Index { get; set; }
    }
}
