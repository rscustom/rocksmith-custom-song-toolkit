using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlockRS2014
{
    [XmlType("set")]
    public class SetRS2014
    {
        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
