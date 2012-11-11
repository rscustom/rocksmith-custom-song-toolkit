using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    public class MultiItemSet : ISet
    {
        public List<string> Values { get; set; }

        public void Serialize(XElement element)
        {
            for (int i = 0; i < Values.Count; i++)
                element.Add(new XElement("set", new XAttribute("index", i), new XAttribute("value", Values[i])));
        }
    }
}
