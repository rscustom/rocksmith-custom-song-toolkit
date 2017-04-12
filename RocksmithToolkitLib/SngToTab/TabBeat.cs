using System;

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
