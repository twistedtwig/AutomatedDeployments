using System;
using System.Collections.Generic;

namespace InstallerComponents
{
    public abstract class InstallerComponentBase : ComponentBase
    {
        private string _forceInstall;
        public string ForceInstall
        {
            get { return _forceInstall; }
            set
            {
                if(value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                {
                    _forceInstall = value;
                }
            }
        }

        protected InstallerComponentBase(string name, IDictionary<string, string> values, ComponentType componentType) : base(name, values, componentType)
        { }        
        
        public bool ShouldRemoveBeforeInstall
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ForceInstall))
                    return false;

                bool result;
                return bool.TryParse(ForceInstall, out result) && result;
            }
        }

        
    }
}
