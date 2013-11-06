using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.ManifestRS2014.Tone
{
    public class ToneRS2014
    {
        public GearRS2014 GearList { get; set; }
        public bool IsCustom { get; set; }
        public float Volume { get; set; }
        public List<string> ToneDescriptors { get; set; }
        public string Key { get; set; }
        public string NameSeparator { get; set; }
        public string Name { get; set; }
        public float SortOrder { get; set; }
    }
}
