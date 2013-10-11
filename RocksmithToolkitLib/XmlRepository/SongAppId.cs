using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage {
    public class SongAppId {
        [XmlAttribute]
        public string Name { get; set; }
        
        [XmlAttribute]
        public string AppId { get; set; }

        public override string ToString() {
            return string.Format("{0} - {1}", Name, AppId);
        }
    }
}
