using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class AlbumArt : ElementFile
    {
        public AlbumArt()
        {
            LLID = IdGenerator.LLID();
        }
        public string LLID { get; private set; }
    }
}
