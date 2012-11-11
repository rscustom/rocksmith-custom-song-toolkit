using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithDLCCreator;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class SongXML : ElementFile
    {
        public SongXML()
        {
            LLID = IdGenerator.LLID();
        }
        public string LLID { get; private set; }
    }
}
