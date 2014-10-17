using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class SongFile : ElementFile
    {
        public SongFile()
        {
            LLID = IdGenerator.LLID();
        }
        public string LLID { get; private set; }

		Guid g;
		public Guid LLIDGuid {
			get {
				Guid.TryParse (LLID, out g);
				return g;
			}
		}
    }
}
