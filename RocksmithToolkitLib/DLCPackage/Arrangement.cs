using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
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
        public bool BonusArr { get; set; } // = false;
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
        // preserve EOF and DDS comments
        [IgnoreDataMember] // required for SaveTemplate feature
        public IEnumerable<XComment> XmlComments { get; set; }

        public Arrangement()
        {
            Id = IdGenerator.Guid();
            MasterId = (ArrangementType == ArrangementType.Vocal || ArrangementType == ArrangementType.ShowLight) ? 1 : RandomGenerator.NextInt();
        }

        public Arrangement(Attributes2014 attr, string xmlSongFile)
        {
            var song = Song2014.LoadFromFile(xmlSongFile);

            this.SongFile = new SongFile();
            this.SongFile.File = "";

            this.SongXml = new SongXML();
            this.SongXml.File = xmlSongFile;
            //Tuning
            TuningDefinition tuning = null;
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
                    // TODO: trying to fix bass tuning issue
                    tuning = TuningDefinitionRepository.Instance().Select(song.Tuning, GameVersion.RS2014);
                    // tuning = TuningDefinitionRepository.Instance().SelectForBass(song.Tuning, GameVersion.RS2014);
                    break;
                case ArrangementName.Vocals:
                    this.ArrangementType = Sng.ArrangementType.Vocal;
                    break;
            }

            if (tuning == null)
            {
                tuning = new TuningDefinition();
                tuning.UIName = tuning.Name = tuning.NameFromStrings(song.Tuning, this.ArrangementType == Sng.ArrangementType.Bass);
                tuning.Custom = true;
                tuning.GameVersion = GameVersion.RS2014;
                tuning.Tuning = song.Tuning;
                TuningDefinitionRepository.Instance().Add(tuning, true);
            }

            this.Tuning = tuning.UIName;
            this.TuningStrings = tuning.Tuning;
            this.CapoFret = attr.CapoFret;
            if (attr.CentOffset != null)
                this.TuningPitch = attr.CentOffset.Cents2Frequency();
            //Properties
            this.ArrangementSort = attr.ArrangementSort;
            this.Name = (ArrangementName)Enum.Parse(typeof(ArrangementName), attr.ArrangementName);
            this.ScrollSpeed = Convert.ToInt32(attr.DynamicVisualDensity.Last() * 10);
            this.PluckedType = (PluckedType)attr.ArrangementProperties.BassPick;
            this.RouteMask = (RouteMask)attr.ArrangementProperties.RouteMask;
            this.BonusArr = attr.ArrangementProperties.BonusArr == 1;
            this.Metronome = (Metronome)attr.ArrangementProperties.Metronome;
            this.ToneMultiplayer = attr.Tone_Multiplayer;
            this.Id = Guid.Parse(attr.PersistentID);
            this.MasterId = attr.MasterID_RDV;
            this.XmlComments = Song2014.ReadXmlComments(xmlSongFile);
            //Tones
            if (attr.Tones == null) // RS2012
            {
                this.ToneBase = attr.Tone_Base;

                if (attr.Tone_A != null || attr.Tone_B != null || attr.Tone_C != null || attr.Tone_D != null)
                    throw new DataException("RS2012 CDLC has extraneous tone data.");
            }
            else // RS2014 or Converter RS2012
            {
                // TODO: optimize using common Arrangment.cs method
                // verify the xml Tone_ exists in tone.manifest.json
                foreach (var jsonTone in attr.Tones)
                {
                    if (jsonTone == null)
                        continue;

                    // fix for tone.id (may not be needed/used by game)
                    Int32 toneId = 0;

                    // cleanup the xml arrangment file too
                    if (jsonTone.Name.ToLower() == attr.Tone_Base.ToLower())
                        this.ToneBase = song.ToneBase = attr.Tone_Base;
                    if (jsonTone.Name.ToLower() == attr.Tone_A.ToLower())
                        this.ToneA = song.ToneA = attr.Tone_A;
                    if (jsonTone.Name.ToLower() == attr.Tone_B.ToLower())
                    {
                        this.ToneB = song.ToneB = attr.Tone_B;
                        toneId = 1;
                    }
                    if (jsonTone.Name.ToLower() == attr.Tone_C.ToLower())
                    {
                        this.ToneC = song.ToneC = attr.Tone_C;
                        toneId = 2;
                    }
                    if (jsonTone.Name.ToLower() == attr.Tone_D.ToLower())
                    {
                        this.ToneD = song.ToneD = attr.Tone_D;
                        toneId = 3;
                    }

                    // update EOF tone name and set tone id
                    if (song.Tones != null)
                        foreach (var xmlTone in song.Tones)
                        {
                            // fix some old toolkit behavior
                            if (xmlTone.Name == "ToneA")
                                xmlTone.Name = attr.Tone_A;
                            if (xmlTone.Name == "ToneB")
                                xmlTone.Name = attr.Tone_B;
                            if (xmlTone.Name == "ToneC")
                                xmlTone.Name = attr.Tone_C;
                            if (xmlTone.Name == "ToneD")
                                xmlTone.Name = attr.Tone_D;

                            if (xmlTone.Name.ToLower() == jsonTone.Name.ToLower() || jsonTone.Name.ToLower().Contains(xmlTone.Name.ToLower()))
                            {
                                xmlTone.Name = jsonTone.Name;
                                xmlTone.Id = toneId;
                            }
                        }

                    if (song.Tones == null && toneId > 0)
                        throw new InvalidDataException("Custom tones were not set properly in EOF" + Environment.NewLine + "Please reauthor XML arrangement in EOF and fix custom tone consistency.");
                }

                // write changes to xml arrangement
                using (var stream = File.Open(xmlSongFile, FileMode.Create))
                    song.Serialize(stream);
            }
        }

        public override string ToString()
        {
            var toneDesc = String.Empty;
            if (!String.IsNullOrEmpty(ToneBase))
                toneDesc = ToneBase;
            // do not initially display duplicate ToneA in Arrangements listbox
            if (!String.IsNullOrEmpty(ToneA) && ToneBase != ToneA)
                toneDesc += String.Format(", {0}", ToneA);
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
                case ArrangementType.ShowLight:
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
