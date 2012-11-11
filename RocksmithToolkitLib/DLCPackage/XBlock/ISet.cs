using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RocksmithToolkitLib.DLCPackage.XBlock
{
    public interface ISet
    {
        void Serialize(XElement element);
    }
}
