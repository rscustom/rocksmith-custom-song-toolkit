using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;


namespace RocksmithToolkitGUI.Config
{
    public static class ConfigGlobals
    {
        public static string DefaultToneFile { get; set; }
        public static string DefaultProjectFolder { get; set; }
        public static Logger Log { get; set; }
        public static bool IsUnitTest { get; set; }
    }

}
