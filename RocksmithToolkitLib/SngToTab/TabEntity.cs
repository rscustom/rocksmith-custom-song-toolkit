using System;

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
