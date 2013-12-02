using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    [XmlType("set")]
    public class Set : ISet
    {
        [XmlAttribute("value")]
        public string Value { get; set; }

        public void Serialize(XElement element)
        {
            element.Add(new XElement("set", new XAttribute("value", Value)));
        }
    }
}
