using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage {
    public class TuningDefinition {
        [XmlAttribute("Version")]
        public GameVersion GameVersion { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string UIName { get; set; }
        [XmlElement]
        public TuningStrings Tuning { get; set; }

        public override string ToString() {
            return UIName;
        }
    }
}
