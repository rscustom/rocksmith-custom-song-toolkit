using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RocksmithToolkitLib;

namespace RocksmithToolkitLib {
    public class SongAppId {
        [XmlAttribute("Version")]
        public GameVersion GameVersion { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string AppId { get; set; }

        [XmlIgnore]
        public string DisplayName {
            get { return this.ToString(); }
        }

        public override string ToString() {
            return string.Format("{0} - {1}", Name, AppId);
        }
    }
}
