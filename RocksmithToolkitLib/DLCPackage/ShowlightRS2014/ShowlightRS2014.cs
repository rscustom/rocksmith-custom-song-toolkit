using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.ShowlightRS2014
{
    [XmlType("showlight")]
    public class ShowlightRS2014
    {
        [XmlAttribute("Time")]
        public float Time { get; set; }
        [XmlAttribute("note")]
        public int Note { get; set; }
    }
}