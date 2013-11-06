using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlockRS2014
{
    [XmlType("property")]
    public class PropertyRS2014
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlElement("set")]
        public SetRS2014 Set { get; set; }
    }
}
