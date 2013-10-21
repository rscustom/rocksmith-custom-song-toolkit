using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.SngToTab
{
    public class TabBeat : TabEntity
    {
        public TabBeat(TabFile tabFile, Single time)
            : base(tabFile, time)
        {
        }

        public override void Apply(TabFile tabFile)
        {
        }
    }
}
