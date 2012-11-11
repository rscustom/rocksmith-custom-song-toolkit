using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class Manifest
    {
        public Dictionary<string, Dictionary<string, Attributes>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }
    }
}
