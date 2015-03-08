using System;
using System.Linq;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage
{
    public enum RouteMask : int
    {
        // Used for lessons or for display only in song list
        None = 0,
        Lead = 1,
        Rhythm = 2,
        Any = 3,
        Bass = 4
    }

    public enum DNAId : int
    {
        None = 0,
        Solo = 1,
        Riff = 2,
        Chord = 3
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
        public TuningStrings TuningStrings { get; set; }
        public double TuningPitch { get; set; }
        public decimal CapoFret { get; set; }
        public int ScrollSpeed { get; set; }
        public PluckedType PluckedType { get; set; }
        public bool CustomFont { get; set; } //true for JVocals and custom fonts (planned)
        public string FontSng { get; set; }
        // cache parsing results (speeds up generating for multiple platforms)
        public Sng2014File Sng2014 { get; set; }
        // Gameplay Path
        public RouteMask RouteMask { get; set; }
        public bool BonusArr = false;
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
        // Motronome.
        public Metronome Metronome { get; set; }

        public Arrangement()
        {
            Id = IdGenerator.Guid();
            MasterId = ArrangementType == Sng.ArrangementType.Vocal ? 1 : RandomGenerator.NextInt();
        }

        public Arrangement(Attributes2014 attr, string xmlSongFile)
        {
            var song = Song2014.LoadFromFile(xmlSongFile);

            this.SongFile = new SongFile();
            this.SongFile.File = "";

            this.SongXml = new SongXML();
            this.SongXml.File = xmlSongFile;
            TuningDefinition tuning = null;

            if (attr.InputEvent == "CONVERT") // RS1
            {
                // TODO: Get tuning from song.manifest.json
                if (song.Tuning == null)
                    song.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };

                switch (song.Arrangement)
                {
                    case "Lead":
                        this.RouteMask = RouteMask.Lead;
                        this.ArrangementType = Sng.ArrangementType.Guitar;
                        tuning = TuningDefinitionRepository.Instance().Select(song.Tuning, GameVersion.RS2012);
                        break;
                    case "Rhythm":
                        this.RouteMask = RouteMask.Rhythm;
                        this.ArrangementType = Sng.ArrangementType.Guitar;
                        tuning = TuningDefinitionRepository.Instance().Select(song.Tuning, GameVersion.RS2012);
                        break;
                    case "Combo":
                        if (song.Part == 1)
                            this.RouteMask = RouteMask.Rhythm;
                        if (song.Part == 2)
                            this.RouteMask = RouteMask.Lead;

                        this.ArrangementType = Sng.ArrangementType.Guitar;
                        tuning = TuningDefinitionRepository.Instance().Select(song.Tuning, GameVersion.RS2012);
                        break;
                    case "Bass":
                        this.RouteMask = RouteMask.Bass;
                        this.ArrangementType = Sng.ArrangementType.Bass;
                        tuning = TuningDefinitionRepository.Instance().SelectForBass(song.Tuning, GameVersion.RS2012);
                        break;
                    case "Vocals":
                        this.ArrangementType = Sng.ArrangementType.Vocal;
                        break;
                }

                if (song.CentOffset != null)
                    this.TuningPitch = Convert.ToDouble(song.CentOffset).Cents2Frequency();
                else
                    this.TuningPitch = 440;

                this.CapoFret = song.Capo;
                this.ArrangementSort = 0;
                this.Name = (ArrangementName)Enum.Parse(typeof(ArrangementName), song.Arrangement);
                this.BonusArr = false; // (song.Part > 1);
                // this.ScrollSpeed = 2;
                this.ToneBase = song.ToneBase;
                // this.ToneMultiplayer = song;
                this.ToneA = song.ToneA;
                this.ToneB = song.ToneB;
                this.ToneC = song.ToneC;
                this.ToneD = song.ToneD;
                // TODO: Confirm valid and working for RS1->RS2
                // may want to get new IDs everytime to avoid conflicts
                 this.Id = IdGenerator.Guid();
                this.MasterId = RandomGenerator.NextInt();
            }
            else // RS2
            {
                switch ((ArrangementName)attr.ArrangementType)
                {
                    case ArrangementName.Lead:
                    case ArrangementName.Rhythm:
                    case ArrangementName.Combo:
                        this.ArrangementType = Sng.ArrangementType.Guitar;
                        tuning = TuningDefinitionRepository.Instance().Select(song.Tuning, GameVersion.RS2014);
                        break;
                    case ArrangementName.Bass:
                        this.ArrangementType = Sng.ArrangementType.Bass;
                        tuning = TuningDefinitionRepository.Instance().SelectForBass(song.Tuning, GameVersion.RS2014);
                        break;
                    case ArrangementName.Vocals:
                        this.ArrangementType = Sng.ArrangementType.Vocal;
                        break;
                }

                if (attr.CentOffset != null)
                    this.TuningPitch = attr.CentOffset.Cents2Frequency();
                this.CapoFret = attr.CapoFret;
                this.ArrangementSort = attr.ArrangementSort;
                this.Name = (ArrangementName)Enum.Parse(typeof(ArrangementName), attr.ArrangementName);
                this.ScrollSpeed = Convert.ToInt32(attr.DynamicVisualDensity.Last() * 10);
                this.PluckedType = (PluckedType)attr.ArrangementProperties.BassPick;
                this.RouteMask = (RouteMask)attr.ArrangementProperties.RouteMask;
                this.BonusArr = attr.ArrangementProperties.BonusArr == 1;
                this.Metronome = (Metronome)attr.ArrangementProperties.Metronome;
                this.ToneBase = attr.Tone_Base;
                this.ToneMultiplayer = attr.Tone_Multiplayer;
                this.ToneA = attr.Tone_A;
                this.ToneB = attr.Tone_B;
                this.ToneC = attr.Tone_C;
                this.ToneD = attr.Tone_D;

                this.Id = Guid.Parse(attr.PersistentID);
                this.MasterId = attr.MasterID_RDV;
            }

            if (tuning == null)
            {
                tuning = new TuningDefinition();
                tuning.UIName = tuning.Name = tuning.NameFromStrings(song.Tuning, false);
                tuning.Custom = true;
                if (attr != null)
                    tuning.GameVersion = GameVersion.RS2014;
                else
                    tuning.GameVersion = GameVersion.RS2012;

                tuning.Tuning = song.Tuning;
                TuningDefinitionRepository.Instance().Add(tuning, true);
            }
            this.Tuning = tuning.UIName;
            this.TuningStrings = tuning.Tuning;
        }

        public override string ToString()
        {
            var toneDesc = String.Empty;
            if (!String.IsNullOrEmpty(ToneBase))
                toneDesc = ToneBase;
            if (!String.IsNullOrEmpty(ToneB))
                toneDesc += String.Format(", {0}", ToneB);
            if (!String.IsNullOrEmpty(ToneC))
                toneDesc += String.Format(", {0}", ToneC);
            if (!String.IsNullOrEmpty(ToneD))
                toneDesc += String.Format(", {0}", ToneD);

            var capoInfo = String.Empty;
            if (CapoFret > 0)
                capoInfo = String.Format(", Capo Fret {0}", CapoFret);

            var pitchInfo = String.Empty;
            if (!TuningPitch.Equals(440.0))
                pitchInfo = String.Format(": A{0}", TuningPitch);

            var metDesc = String.Empty;
            if (Metronome == Metronome.Generate)
                metDesc = " +Metronome";

            switch (ArrangementType)
            {
                case ArrangementType.Bass:
                    return String.Format("{0} [{1}{2}{3}] ({4}){5}", ArrangementType, Tuning, pitchInfo, capoInfo, toneDesc, metDesc);
                case ArrangementType.Vocal:
                    return String.Format("{0}", Name);
                default:
                    return String.Format("{0} - {1} [{2}{3}{4}] ({5}){6}", ArrangementType, Name, Tuning, pitchInfo, capoInfo, toneDesc, metDesc);
            }
        }

        public void CleanCache()
        {
            Sng2014 = null;
        }

    }
}
