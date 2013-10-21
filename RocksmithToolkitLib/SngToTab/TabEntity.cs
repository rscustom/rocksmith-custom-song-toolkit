using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocksmithToolkitLib.SngToTab
{
    abstract public class TabEntity
    {
        public readonly TabFile TabFile;
        public readonly Single Time;

        public TabEntity(TabFile tabFile, Single time)
        {
            TabFile = tabFile;
            Time = time;
        }

        public abstract void Apply(TabFile tabFile);
    }
}
