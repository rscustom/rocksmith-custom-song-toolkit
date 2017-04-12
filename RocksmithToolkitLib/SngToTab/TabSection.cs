using System;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.SngToTab
{
    public class TabSection : TabEntity
    {
        public readonly string Name;
        public readonly Single Start;
        public readonly Single End;

        public TabSection(TabFile tabFile, SongSection section)
            : base(tabFile, section.StartTime)
        {
            Name = section.Name;
            Start = section.StartTime;
            End = section.EndTime;
        }

        public override void Apply(TabFile tabFile)
        {
            // Uppercase the first letter
            string name = Name;
            if (name.Length > 0)
                name = char.ToUpper(name[0]) + name.Substring(1);

            string line = name + " (" + Common.TimeToString(Start) + " - " + Common.TimeToString(End) + ")";
            tabFile.AppendLine("");
            tabFile.AppendLine("");
            tabFile.AppendLine("");
            tabFile.AppendLine(line);
        }

        public override string ToString()
        {
            return "****** SECTION: " + Name + " ******";
        }
    }
}
