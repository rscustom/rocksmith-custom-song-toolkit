using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace RocksmithDLCCreator.XBlock
{
    public interface ISet
    {
        void Serialize(XElement element);
    }
}
