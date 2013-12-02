using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    [XmlType("property")]
    public class Property2014
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlElement("set")]
        public Set Set { get; set; }
    }
}
