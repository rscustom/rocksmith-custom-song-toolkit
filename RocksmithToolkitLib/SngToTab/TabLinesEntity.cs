using System;

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
