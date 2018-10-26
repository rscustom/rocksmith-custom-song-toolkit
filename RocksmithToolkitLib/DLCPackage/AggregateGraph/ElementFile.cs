using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class ElementFile : Element
    {
        public string File { get; set; }
        public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(File); } }
        // TODO: monitor this change
        public string Version { get; set; }
    }
}
