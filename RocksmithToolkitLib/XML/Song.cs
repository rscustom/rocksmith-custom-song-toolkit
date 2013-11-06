using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.IO;

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
        public bool HasBends()
        {
            return Levels.Any(x => x.Notes == null ? false : x.Notes.Any(y => y.Bend > 0));
        }

        public bool HasSlapAndPop()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Pluck > 0 || y.Slap > 0);
        }

        public bool HasHarmonics()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Harmonic > 0);
        }

        public bool HasHOPOs()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Hopo > 0);
        }

        public bool HasPalmMutes()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.PalmMute > 0);
        }

        public bool HasSlides()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.SlideTo >= 0);
        }

        public bool HasSustain()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Sustain > 0);
        }

        public bool HasTremolo()
        {
            return Levels.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes).Any(y => y.Tremolo > 0);
        }

        public static Song LoadFromFile(string xmlSongFile) {
            Song XmlSong = null;

            using (var reader = new StreamReader(xmlSongFile)) {
                var serializer = new XmlSerializer(typeof(Song));
                XmlSong = (Song)serializer.Deserialize(reader);
            }

            return XmlSong;
        }
    }

    [XmlType("tuning")]
    public class TuningStrings {
        [XmlAttribute("string0")]
        public Int32 String0 { get; set; }

        [XmlAttribute("string1")]
        public Int32 String1 { get; set; }

        [XmlAttribute("string2")]
        public Int32 String2 { get; set; }

        [XmlAttribute("string3")]
        public Int32 String3 { get; set; }

        [XmlAttribute("string4")]
        public Int32 String4 { get; set; }

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
        public Int32 Disparity { get; set; }

        [XmlAttribute("maxDifficulty")]
        public Int32 MaxDifficulty { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("ignore")]
        public Int32 Ignore { get; set; }

        [XmlAttribute("solo")]
        public Int32 Solo { get; set; }
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
        public Int32 Fret0 { get; set; }

        [XmlAttribute("fret1")]
        public Int32 Fret1 { get; set; }

        [XmlAttribute("fret2")]
        public Int32 Fret2 { get; set; }

        [XmlAttribute("fret3")]
        public Int32 Fret3 { get; set; }

        [XmlAttribute("fret4")]
        public Int32 Fret4 { get; set; }

        [XmlAttribute("fret5")]
        public Int32 Fret5 { get; set; }

        [XmlAttribute("finger0")]
        public Int32 Finger0 { get; set; }

        [XmlAttribute("finger1")]
        public Int32 Finger1 { get; set; }

        [XmlAttribute("finger2")]
        public Int32 Finger2 { get; set; }

        [XmlAttribute("finger3")]
        public Int32 Finger3 { get; set; }

        [XmlAttribute("finger4")]
        public Int32 Finger4 { get; set; }

        [XmlAttribute("finger5")]
        public Int32 Finger5 { get; set; }
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
        public Int32 Measure { get; set; }
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
        public Int32 String { get; set; }

        [XmlAttribute("slideTo")]
        public Int32 SlideTo { get; set; }

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
        public Int32 Fret { get; set; }

        [XmlAttribute("bend")]
        public Int32 Bend { get; set; }

        [XmlAttribute("pluck")]
        public Int32 Pluck { get; set; }

        [XmlAttribute("slap")]
        public Int32 Slap { get; set; }
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
