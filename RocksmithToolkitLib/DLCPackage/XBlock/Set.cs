using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    public class Set : ISet
    {
        public string Value { get; set; }
        public void Serialize(XElement element)
        {
            element.Add(new XElement("set", new XAttribute("value", Value)));
        }
    }
}
