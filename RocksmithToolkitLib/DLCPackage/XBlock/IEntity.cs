using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    public interface IEntity
    {
        string Id { get; set; }
        string ModelName { get; set; }
        string Name { get; set; }
        int Iterations { get; set; }

        void Serialize(XElement element);
    }
}
