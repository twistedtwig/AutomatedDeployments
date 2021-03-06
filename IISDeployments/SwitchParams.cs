﻿
namespace IISDeployments
{
    public class SwitchParams
    {
        public SwitchParams()
        {
            SetExePath = true;
        }

        public bool? BreakOnError { get; set; }
        public bool? Verbose { get; set; }
        public bool? CleanUp { get; set; }
        public bool? Force { get; set; }
        public string ConfigSection { get; set; }
        public string ConfigPath { get; set; }
        public bool SetExePath { get; set; }
        public bool? ForceLocal { get; set; }

        public bool? Encrypt { get; set; }
        public bool? Decrypt { get; set; }
        public bool? CreateKeyFile { get; set; }
   
    }
}
