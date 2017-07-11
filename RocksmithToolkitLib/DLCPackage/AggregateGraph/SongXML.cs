using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.XML;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class SongXML : ElementFile
    {
        public string LLID { get; private set; }
        
        public SongXML()
        {
            LLID = IdGenerator.LLID();
        }

        Guid g;
        public Guid LLIDGuid {
            get {
                Guid.TryParse (LLID, out g);
                return g;
            }
        }        
    }
}
