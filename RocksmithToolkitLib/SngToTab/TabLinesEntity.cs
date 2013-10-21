using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.SngToTab
{
    abstract public class TabLinesEntity : TabEntity
    {
        public TabLinesEntity(TabFile tabFile, Single time)
            : base(tabFile, time)
        {
        }

        public abstract string[] GetLines();
    }
}
