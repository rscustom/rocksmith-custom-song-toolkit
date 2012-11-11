using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithDLCCreator;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;

namespace RocksmithToolkitLib.DLCPackage
{
    public class Arrangement
    {
        public Arrangement()
        {
            Id = IdGenerator.Guid();
        }
        public string Name { get; set; }
        public Guid Id { get; private set; }
        public SongFile SongFile { get; set; }
        public SongXML SongXml { get; set; }
        public bool PowerChords { get; set; }
        public bool BarChords { get; set; }
        public bool DoubleStops { get; set; }
        public bool DropDPowerChords { get; set; }
        public bool FifthsAndOctaves { get; set; }
        public bool FretHandMutes { get; set; }
        public bool OpenChords { get; set; }
        public bool IsVocal { get; set; }
        public int RelativeDifficulty { get; set; }
        public bool SlapAndPop { get; set; }
        public bool PreBends { get; set; }
        public string Tuning { get; set; }
        public bool Vibrato { get; set; }
        public int SongDifficulty { get; set; }
        public string Artist { get; set; }
        public int AverageTempo { get; set; }
        public string SongDisplayName { get; set; }
        public int SongYear { get; set; }
        public override string ToString()
        {
            return String.Format("{0} - {1}", Name, Artist);
        }
    }
}
