using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitGUI.Config
{
    // TODO: develop/use this class more for other global variables to reduce redundancies elsewhere
    public static class Globals
    {
        public static string DefaultToneFile { get; set; }
        public static string DefaultProjectDir { get; set; }

        // FIXME: IsUnitTest should be a Solution wide global variable        
        public static bool IsUnitTest { get; set; }
    }

}
