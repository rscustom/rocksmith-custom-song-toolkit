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
        public ArrangementType ArrangementType { get; set; }
        public int RelativeDifficulty { get; set; }
        public string Tuning { get; set; }
        public string ToneName { get; set; }
        public int ScrollSpeed { get; set; }
        public PluckedType PluckedType { get; set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
