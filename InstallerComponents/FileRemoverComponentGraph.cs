using System.Collections.Generic;

namespace InstallerComponents
{
    public class FileRemoverComponentGraph : FileComponentBaseGraph
    {
        public FileRemoverComponentGraph(string name, IDictionary<string, string> values) : base(name, values, ComponentType.FileRemover)
        {
        }

        public string DeleteRemoteFolder { get; set; }

        public bool ShouldDeleteRmoteFolder
        {
            get
            {
                bool del;
                if (bool.TryParse(DeleteRemoteFolder, out del))
                {
                    return del;
                }

                return false;
            }
        }
    }
}
