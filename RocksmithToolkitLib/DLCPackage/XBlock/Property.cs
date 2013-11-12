using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    [XmlType("property")]
    public class Property
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlElement("set")]
        public ISet Set { get; set; }

        public void Serialize(XElement element)
        {
            var el = new XElement("property", new XAttribute("name", Name));
            Set.Serialize(el);
            element.Add(el);
        }
    }
}
