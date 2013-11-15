using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.Xml
{
    [XmlRoot("song", Namespace = "", IsNullable = false)]
    public class Song
    {
        [XmlAttribute("version")] // RS1 is 4
        public string Version { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("artist")]
        public string Artist { get; set; }

        [XmlElement("arrangement")]
        public string Arrangement { get; set; }

        [XmlElement("part")]
        public Int16 Part { get; set; }

        [XmlElement("offset")]
        public Single Offset { get; set; }

        [XmlElement("songLength")]
        public Single SongLength { get; set; }

        [XmlElement("lastConversionDateTime")]
        public string LastConversionDateTime { get; set; }

        [XmlElement("startBeat")]
        public Single StartBeat { get; set; }

        [XmlElement("averageTempo")]
        public Single AverageTempo { get; set; }

        [XmlElement("tuning")]
        public TuningStrings Tuning { get; set; }

        [XmlElement("artistName")]
        public string ArtistName { get; set; }

        [XmlElement("albumName")]
        public string AlbumName { get; set; }

        [XmlElement("albumYear")]
        public string AlbumYear { get; set; }

        [XmlElement("arrangementProperties")]
        public SongArrangementProperties ArrangementProperties { get; set; }

        [XmlArray("phrases")]
        [XmlArrayItem("phrase")]
        public SongPhrase[] Phrases { get; set; }

        [XmlArray("phraseIterations")]
        [XmlArrayItem("phraseIteration")]
        public SongPhraseIteration[] PhraseIterations { get; set; }

        [XmlArray("linkedDiffs")]
        [XmlArrayItem("linkedDiff")]
        public SongLinkedDiff[] LinkedDiffs { get; set; }

        [XmlArray("phraseProperties")]
        [XmlArrayItem("phraseProperty")]
        public SongPhraseProperty[] PhraseProperties { get; set; }

        [XmlArray("chordTemplates")]
        [XmlArrayItem("chordTemplate")]
        public SongChordTemplate[] ChordTemplates { get; set; }

        [XmlArray("fretHandMuteTemplates")]
        [XmlArrayItem("fretHandMuteTemplate")]
        public SongFretHandMuteTemplate[] FretHandMuteTemplates { get; set; }

        [XmlArray("controls")]
        [XmlArrayItem("control")]
        public SongControl[] Controls { get; set; }

        [XmlArray("ebeats")]
        [XmlArrayItem("ebeat")]
        public SongEbeat[] Ebeats { get; set; }

        [XmlArray("sections")]
        [XmlArrayItem("section")]
        public SongSection[] Sections { get; set; }

        [XmlArray("events")]
        [XmlArrayItem("event")]
        public SongEvent[] Events { get; set; }

        [XmlArray("levels")]
        [XmlArrayItem("level")]
        public SongLevel[] Levels { get; set; }

        #region Old techniques
        //# RS1 old song xml have no arrangement properties
        private bool HasArrangementProperties {
            get {
                return ArrangementProperties != null;
            }
        }

        public bool HasPowerChords() {
            if (HasArrangementProperties)
                return ArrangementProperties.PowerChords == 1;
            else
                return Levels.Any(c => c.Chords == null ? false : HasPowerChords(c.Chords));
        }
        private bool HasPowerChords(SongChord[] songChord) {
            return true; //Pending (old song xml only)
        }

        public bool HasBarChords() {
            if (HasArrangementProperties)
                return ArrangementProperties.BarreChords == 1;
            else
                return Levels.Any(c => c.Chords == null ? false : HasBarChords(c.Chords));
        }
        private bool HasBarChords(SongChord[] songChord) {
            return true; //Pending (old song xml only)
        }

        public bool HasOpenChords() {
            if (HasArrangementProperties)
                return ArrangementProperties.OpenChords == 1;
            else
                return Levels.Any(c => c.Chords == null ? false : HasOpenChords(c.Chords));
        }
        private bool HasOpenChords(SongChord[] songChord) {
            return true; //Pending (old song xml only)
        }

        public bool HasDoubleStops() {
            if (HasArrangementProperties)
                return ArrangementProperties.DoubleStops == 1;
            else
                return Levels.Any(c => c.Chords == null ? false : HasDoubleStops(c.Chords));
        }
        private bool HasDoubleStops(SongChord[] songChord) {
            return true; //Pending (old song xml only)
        }

        public bool HasDropDPowerChords() {
            if (HasArrangementProperties)
                return ArrangementProperties.DropDPower == 1;
            else
                return Levels.Any(c => c.Chords == null ? false : HasDropDPowerChords(c.Chords));
        }
        private bool HasDropDPowerChords(SongChord[] songChord) {
            return true; //Pending (old song xml only)
        }

        public bool HasBends()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Bends == 1;
            else
                return Levels.Any(x => x.Notes == null ? false : x.Notes.Any(y => y.Bend > 0));
        }

        public bool HasSlapAndPop()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.SlapPop == 1;
            else
                return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Pluck > 0 || y.Slap > 0);
        }

        public bool HasHarmonics()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Harmonics == 1;
            else
                return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Harmonic > 0);
        }

        public bool HasHOPOs()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Hopo == 1;
            else
                return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Hopo > 0);
        }

        public bool HasFretHandMutes() {
            if (HasArrangementProperties)
                return ArrangementProperties.FretHandMutes == 1;
            else
                return false; //No definition in old XML
        }

        public bool HasPrebends() { //Identify only bend, no definition in old XML
            if (HasArrangementProperties)
                return ArrangementProperties.Bends == 1;
            else
                return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Bend > 0);
        }

        public bool HasVibrato() {
            if (HasArrangementProperties)
                return ArrangementProperties.Vibrato == 1;
            else
                return false; //No definition in old XML
        }

        public bool HasPalmMutes()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.PalmMutes == 1;
            else
                return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.PalmMute > 0);
        }

        public bool HasSlides()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Slides == 1;
            else
                return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.SlideTo >= 0);
        }

        public bool HasSustain()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Sustain > 0);
        }

        public bool HasTremolo()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Tremolo == 1;
            else
                return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Tremolo > 0);
        }

        public bool HasTwoFingerPlucking() {
            if (HasArrangementProperties)
                return ArrangementProperties.TwoFingerPicking == 1;
            else
                return false; //No definition in old XML
        }

        public bool HasFifthsAndOctaves() {
            if (HasArrangementProperties)
                return ArrangementProperties.FifthsAndOctaves == 1;
            else
                return false; //No definition in old XML
        }

        public bool HasSyncopation() {
            if (HasArrangementProperties)
                return ArrangementProperties.Syncopation == 1;
            else
                return false; //No definition in old XML
        }

        #endregion

        public static Song LoadFromFile(string xmlSongFile) {
            Song XmlSong = null;

            using (var reader = new StreamReader(xmlSongFile)) {
                var serializer = new XmlSerializer(typeof(Song));
                XmlSong = (Song)serializer.Deserialize(reader);
            }

            return XmlSong;
        }
    }

    [XmlType("arrangementProperties")]
    public class SongArrangementProperties {
        [XmlAttribute("represent")]
        public Int32 Represent { get; set; }

        [XmlAttribute("standardTuning")]
        public Int32 StandardTuning { get; set; }

        [XmlAttribute("nonStandardChords")]
        public Int32 NonStandardChords { get; set; }

        [XmlAttribute("barreChords")]
        public Int32 BarreChords { get; set; }

        [XmlAttribute("powerChords")]
        public Int32 PowerChords { get; set; }

        [XmlAttribute("dropDPower")]
        public Int32 DropDPower { get; set; }

        [XmlAttribute("openChords")]
        public Int32 OpenChords { get; set; }

        [XmlAttribute("fingerPicking")]
        public Int32 FingerPicking { get; set; }

        [XmlAttribute("pickDirection")]
        public sbyte PickDirection { get; set; }

        [XmlAttribute("doubleStops")]
        public Int32 DoubleStops { get; set; }

        [XmlAttribute("palmMutes")]
        public Int32 PalmMutes { get; set; }

        [XmlAttribute("harmonics")]
        public Int32 Harmonics { get; set; }

        [XmlAttribute("pinchHarmonics")]
        public Int32 PinchHarmonics { get; set; }

        [XmlAttribute("hopo")]
        public Int32 Hopo { get; set; }

        [XmlAttribute("tremolo")]
        public Int32 Tremolo { get; set; }

        [XmlAttribute("slides")]
        public Int32 Slides { get; set; }

        [XmlAttribute("unpitchedSlides")]
        public Int32 UnpitchedSlides { get; set; }

        [XmlAttribute("bends")]
        public Int32 Bends { get; set; }

        [XmlAttribute("tapping")]
        public Int32 Tapping { get; set; }

        [XmlAttribute("vibrato")]
        public Int16 Vibrato { get; set; }

        [XmlAttribute("fretHandMutes")]
        public Int32 FretHandMutes { get; set; }

        [XmlAttribute("slapPop")]
        public Int32 SlapPop { get; set; }

        [XmlAttribute("twoFingerPicking")]
        public Int32 TwoFingerPicking { get; set; }

        [XmlAttribute("fifthsAndOctaves")]
        public Int32 FifthsAndOctaves { get; set; }

        [XmlAttribute("syncopation")]
        public Int32 Syncopation { get; set; }

        [XmlAttribute("bassPick")]
        public Int32 BassPick { get; set; }
    }

    [XmlType("tuning")]
    public class TuningStrings {
        [JsonProperty("string0")]
        [XmlAttribute("string0")]
        public Int32 String0 { get; set; }

        [JsonProperty("string1")]
        [XmlAttribute("string1")]
        public Int32 String1 { get; set; }

        [JsonProperty("string2")]
        [XmlAttribute("string2")]
        public Int32 String2 { get; set; }

        [JsonProperty("string3")]
        [XmlAttribute("string3")]
        public Int32 String3 { get; set; }

        [JsonProperty("string4")]
        [XmlAttribute("string4")]
        public Int32 String4 { get; set; }

        [JsonProperty("string5")]
        [XmlAttribute("string5")]
        public Int32 String5 { get; set; }

        public int[] ToArray() {
            Int32[] strings = { String0, String1, String2, String3, String4, String5 };
            return strings;
        }
    }

    [XmlType("phrase")]
    public class SongPhrase
    {
        [XmlAttribute("disparity")]
        public Byte Disparity { get; set; }

        [XmlAttribute("maxDifficulty")]
        public Int32 MaxDifficulty { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("ignore")]
        public Byte Ignore { get; set; }

        [XmlAttribute("solo")]
        public Byte Solo { get; set; }
    }

    [XmlType("phraseIteration")]
    public class SongPhraseIteration
    {
        [XmlAttribute("phraseId")]
        public Int32 PhraseId { get; set; }

        [XmlAttribute("time")]
        public Single Time { get; set; }
    }

    [XmlType("linkedDiff")]
    public class SongLinkedDiff
    {
        [XmlAttribute("parentId")]
        public Int32 ParentId { get; set; }

        [XmlAttribute("childId")]
        public Int32 ChildId { get; set; }
    }

    [XmlType("phraseProperty")]
    public class SongPhraseProperty
    {
        [XmlAttribute("phraseId")]
        public Int32 PhraseId { get; set; }

        [XmlAttribute("redundant")]
        public Int16 Redundant { get; set; }

        [XmlAttribute("levelJump")]
        public Int16 LevelJump { get; set; }

        [XmlAttribute("empty")]
        public Int32 Empty { get; set; }

        [XmlAttribute("difficulty")]
        public Int32 Difficulty { get; set; }
    }

    [XmlType("chordTemplate")]
    public class SongChordTemplate
    {
        [XmlAttribute("chordName")]
        public string ChordName { get; set; }
        
        [XmlAttribute("fret0")]
        public sbyte Fret0 { get; set; }

        [XmlAttribute("fret1")]
        public sbyte Fret1 { get; set; }

        [XmlAttribute("fret2")]
        public sbyte Fret2 { get; set; }

        [XmlAttribute("fret3")]
        public sbyte Fret3 { get; set; }

        [XmlAttribute("fret4")]
        public sbyte Fret4 { get; set; }

        [XmlAttribute("fret5")]
        public sbyte Fret5 { get; set; }

        [XmlAttribute("finger0")]
        public sbyte Finger0 { get; set; }

        [XmlAttribute("finger1")]
        public sbyte Finger1 { get; set; }

        [XmlAttribute("finger2")]
        public sbyte Finger2 { get; set; }

        [XmlAttribute("finger3")]
        public sbyte Finger3 { get; set; }

        [XmlAttribute("finger4")]
        public sbyte Finger4 { get; set; }

        [XmlAttribute("finger5")]
        public sbyte Finger5 { get; set; }
    }

    //TBD
    [XmlType("fretHandMuteTemplate")]
    public class SongFretHandMuteTemplate
    {
        //TBD
    }

    [XmlType("ebeat")]
    public class SongEbeat
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("measure")]
        public Int16 Measure { get; set; }
    }

    [XmlType("section")]
    public class SongSection
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("startTime")]
        public Single StartTime { get; set; }

        [XmlAttribute("number")]
        public Int32 Number { get; set; }
    }

    [XmlType("event")]
    public class SongEvent
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }
    }

    [XmlType("level")]
    public class SongLevel
    {
        [XmlAttribute("difficulty")]
        public Int32 Difficulty { get; set; }

        [XmlArray("notes")]
        [XmlArrayItem("note")]
        public SongNote[] Notes { get; set; }

        [XmlArray("chords")]
        [XmlArrayItem("chord")]
        public SongChord[] Chords { get; set; }

        [XmlArray("fretHandMutes")]
        [XmlArrayItem("fretHandMute")]
        public SongFretHandMute[] FretHandMutes { get; set; }

        [XmlArray("anchors")]
        [XmlArrayItem("anchor")]
        public SongAnchor[] Anchors { get; set; }

        [XmlArray("handShapes")]
        [XmlArrayItem("handShape")]
        public SongHandShape[] HandShapes { get; set; }
    }

    [XmlType("note")]
    public class SongNote
    {       
        [XmlAttribute("ignore")]
        public Byte Ignore { get; set; }

        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("tremolo")]
        public Byte Tremolo { get; set; }

        [XmlAttribute("sustain")]
        public Single Sustain { get; set; }

        [XmlAttribute("string")]
        public Byte String { get; set; }

        [XmlAttribute("slideTo")]
        public sbyte SlideTo { get; set; }

        [XmlAttribute("pullOff")]
        public Byte PullOff { get; set; }

        [XmlAttribute("palmMute")]
        public Byte PalmMute { get; set; }

        [XmlAttribute("hopo")]
        public Byte Hopo { get; set; }

        [XmlAttribute("harmonic")]
        public Byte Harmonic { get; set; }

        [XmlAttribute("hammerOn")]
        public Byte HammerOn { get; set; }

        [XmlAttribute("fret")]
        public SByte Fret { get; set; }

        [XmlAttribute("bend")]
        public Byte Bend { get; set; }

        [XmlAttribute("pluck")]
        public sbyte Pluck { get; set; }

        [XmlAttribute("slap")]
        public sbyte Slap { get; set; }
    }

    [XmlType("chord")]
    public class SongChord
    {
        [XmlAttribute("ignore")]
        public Byte Ignore { get; set; }

        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("strum")]
        public string Strum { get; set; }

        [XmlAttribute("highDensity")]
        public Byte HighDensity { get; set; }

        [XmlAttribute("chordId")]
        public Int32 ChordId { get; set; }
    }

    //TBD
    [XmlType("fretHandMute")]
    public class SongFretHandMute
    {
        //TBD
    }

    [XmlType("control")]
    public class SongControl
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }
    }

    [XmlType("anchor")]
    public class SongAnchor
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("fret")]
        public Int32 Fret { get; set; }
    }

    [XmlType("handShape")]
    public class SongHandShape
    {
        [XmlAttribute("startTime")]
        public Single StartTime { get; set; }

        [XmlAttribute("chordId")]
        public Int32 ChordId { get; set; }

        [XmlAttribute("endTime")]
        public Single EndTime { get; set; }
    }
}
