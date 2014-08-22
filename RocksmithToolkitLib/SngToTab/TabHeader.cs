using System;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.SngToTab
{
    // Contains basic tab information
    public class TabHeader
    {
        public readonly TabFile TabFile;
        public readonly string Title;
        public readonly string Artist;
        public readonly string Length;
        public readonly TuningInfo TuningInfo;

        public TabHeader(TabFile tabFile, SngFile sngFile)
        {
            TabFile = tabFile;
            Title = sngFile.Metadata.SongTitle;
            Artist = sngFile.Metadata.Artist;
            Length = Common.TimeToString(sngFile.Metadata.Length);
            TuningInfo = tabFile.TuningInfos[(Tuning)sngFile.Metadata.Tuning];
        }

        public void Apply(TabFile tabFile)
        {
            tabFile.AppendLine("TITLE: " + Title + Environment.NewLine);
            if (Artist != "DUMMY")
                tabFile.AppendLine("ARTIST: " + Artist + Environment.NewLine);
            tabFile.AppendLine("LENGTH: " + Length + Environment.NewLine);
            tabFile.AppendLine("TUNING: " + TuningInfo.Description + Environment.NewLine);
        }
    }

}
