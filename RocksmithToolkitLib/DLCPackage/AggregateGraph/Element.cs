using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class Element
    {
        public Guid UUID { get; set; }
        public Element()
        {
            do
                UUID = Guid.NewGuid();
            while (UUID == Guid.Empty);
        }
    }
}
