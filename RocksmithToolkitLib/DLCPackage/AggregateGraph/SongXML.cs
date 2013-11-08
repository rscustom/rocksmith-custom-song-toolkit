using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class SongXML : ElementFile
    {
        public string LLID { get; private set; }
        public Song SongXmlContent { get { return Song.LoadFromFile(base.File); } }

        public SongXML()
        {
            LLID = IdGenerator.LLID();
        }        
    }
}
