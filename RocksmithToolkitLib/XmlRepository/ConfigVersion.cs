using System;
using System.IO;

namespace RocksmithToolkitLib.XmlRepository
{
    public static class ConfigVersion
    {
        // used to force RocksmithToolkitUpdater 
        // to do a fresh install of RocksmithToolkitLib.*.xml
        public static string Number { get; set; }
    }
}
