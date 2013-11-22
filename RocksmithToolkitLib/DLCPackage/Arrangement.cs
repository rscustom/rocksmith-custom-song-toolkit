using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage
{
    public enum RouteMask : int {
        // Used for lessons or for display only in song list
        None = 0,
        Lead = 1,
        Rhythm = 2,
        Any = 3,
        Bass = 4
    }

    public class Arrangement
    {
        public SongFile SongFile { get; set; }
        public SongXML SongXml { get; set; }
        // Song Information
        public ArrangementType ArrangementType { get; set; }
        public int ArrangementSort { get; set; }
        public ArrangementName Name { get; set; }
        public string Tuning { get; set; }
        public double TuningPitch { get; set; }
        public int ScrollSpeed { get; set; }
        public PluckedType PluckedType { get; set; }
        // Gameplay Path
        public RouteMask RouteMask { get; set; }
        // Tone Selector
        public string ToneBase { get; set; }
        public string ToneMultiplayer { get; set; }
        public string ToneA { get; set; }
        public string ToneB { get; set; }
        public string ToneC { get; set; }
        public string ToneD { get; set; }
        // DLC ID
        public Guid Id { get; set; }
        public int MasterId { get; set; }

        public Arrangement()
        {
            Id = IdGenerator.Guid();
            MasterId = ArrangementType == Sng.ArrangementType.Vocal ? 1 : RandomGenerator.NextInt();
        }

        public Arrangement(Attributes2014 attr, string xmlSongFile)
        {
            var song = Song2014.LoadFromFile(xmlSongFile);

            this.SongFile = new SongFile();

            this.SongXml = new SongXML();
            this.SongXml.File = xmlSongFile;

            this.ArrangementType = (ArrangementType)attr.ArrangementType;
            this.ArrangementSort = attr.ArrangementSort;
            this.Name = (ArrangementName)Enum.Parse(typeof(ArrangementName), attr.ArrangementName);
            this.Tuning = TuningDefinitionRepository.Instance().Select(attr.Tuning, GameVersion.RS2014).UIName;
            this.ScrollSpeed = (int)attr.DynamicVisualDensity.Last();
            this.PluckedType = (PluckedType)attr.ArrangementProperties.BassPick;
            this.RouteMask = (RouteMask)attr.ArrangementProperties.RouteMask;
            this.ToneBase = attr.Tone_Base;
            this.ToneMultiplayer = attr.Tone_Multiplayer;
            this.ToneA = attr.Tone_A;
            this.ToneB = attr.Tone_B;
            this.ToneC = attr.Tone_C;
            this.ToneD = attr.Tone_D;

            this.Id = Guid.Parse(attr.PersistentID);
            this.MasterId = attr.MasterID_RDV;
        }
        
        public override string ToString()
        {
            switch (ArrangementType)
            {
                case ArrangementType.Bass:
                    return String.Format("{0} [{1}]", ArrangementType, Tuning);
                case ArrangementType.Vocal:
                    return String.Format("{0}", ArrangementType);
                default:
                    return String.Format("{0} - {1} [{2}]", ArrangementType, Name, Tuning);
            }
        }
    }
}
