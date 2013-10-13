using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.DLCPackage
{
    public class Arrangement
    {
        public Arrangement()
        {
            Id = IdGenerator.Guid();
            MasterId = ArrangementType == Sng.ArrangementType.Vocal ? 1 : RandomGenerator.NextInt();
        }
        public ArrangementName Name { get; set; }
        public Guid Id { get; set; }
        public int MasterId { get; set; }
        public SongFile SongFile { get; set; }
        public SongXML SongXml { get; set; }
        public bool PowerChords { get; set; }
        public bool BarChords { get; set; }
        public bool DoubleStops { get; set; }
        public bool DropDPowerChords { get; set; }
        public bool FifthsAndOctaves { get; set; }
        public bool FretHandMutes { get; set; }
        public bool OpenChords { get; set; }
        public ArrangementType ArrangementType { get; set; }
        public int RelativeDifficulty { get; set; }
        public bool TwoFingerPlucking { get; set; }
        public bool Syncopation { get; set; }
        public bool Prebends { get; set; }
        public string Tuning { get; set; }
        public bool Vibrato { get; set; }
        public string ToneName { get; set; }
        public int ScrollSpeed { get; set; }
        public PluckedType PluckedType { get; set; }
        public override string ToString()
        {
            return ArrangementType == Sng.ArrangementType.Vocal ? Name.ToString() : String.Format("{0} - {1}", Name, ToneName);
        }
    }
}
