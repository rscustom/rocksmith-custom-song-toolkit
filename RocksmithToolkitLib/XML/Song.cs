using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.Xml
{
    [XmlRoot("song", Namespace = "", IsNullable = false)]
    public class Song
    {
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

        [XmlElement("phrases")]
        public SongPhrases Phrases { get; set; }

        [XmlElement("phraseIterations")]
        public SongPhraseIterations PhraseIterations { get; set; }

        [XmlElement("linkedDiffs")]
        public SongLinkedDiffs LinkedDiffs { get; set; }

        [XmlElement("phraseProperties")]
        public SongPhraseProperties PhraseProperties { get; set; }

        [XmlElement("chordTemplates")]
        public SongChordTemplates ChordTemplates { get; set; }

        [XmlElement("fretHandMuteTemplates")]
        public SongFretHandMuteTemplates FretHandMuteTemplates { get; set; }

        [XmlElement("controls")]
        public SongControls Controls { get; set; }

        [XmlElement("ebeats")]
        public SongEbeats Ebeats { get; set; }

        [XmlElement("sections")]
        public SongSections Sections { get; set; }

        [XmlElement("events")]
        public SongEvents Events { get; set; }

        [XmlElement("levels")]
        public SongLevels Levels { get; set; }
        public bool HasBends()
        {
            return Levels.Level.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes.Note).Any(y => y.Bend > 0);
        }

        public bool HasHarmonics()
        {
            return Levels.Level.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes.Note).Any(y => y.Harmonic > 0);
        }

        public bool HasHOPOs()
        {
            return Levels.Level.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes.Note).Any(y => y.Hopo > 0);
        }

        public bool HasPalmMutes()
        {
            return Levels.Level.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes.Note).Any(y => y.PalmMute > 0);
        }

        public bool HasSlides()
        {
            return Levels.Level.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes.Note).Any(y => y.SlideTo >= 0);
        }

        public bool HasSustain()
        {
            return Levels.Level.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes.Note).Any(y => y.Sustain > 0);
        }

        public bool HasTremolo()
        {
            return Levels.Level.SelectMany(x => x.Notes == null ? new SongNote[0] : x.Notes.Note).Any(y => y.Tremolo > 0);
        }
    }

    [XmlType("phrases")]
    public class SongPhrases
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("phrase")]
        public SongPhrase[] Phrase { get; set; }
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

    [XmlType("phraseIterations")]
    public class SongPhraseIterations
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("phraseIteration")]
        public SongPhraseIteration[] PhraseIteration { get; set; }
    }

    [XmlType("phraseIteration")]
    public class SongPhraseIteration
    {
        [XmlAttribute("phraseId")]
        public Int32 PhraseId { get; set; }

        [XmlAttribute("time")]
        public Single Time { get; set; }
    }

    [XmlType("linkedDiffs")]
    public class SongLinkedDiffs
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("linkedDiff")]
        public SongLinkedDiff[] LinkedDiff { get; set; }
    }

    [XmlType("linkedDiff")]
    public class SongLinkedDiff
    {
        [XmlAttribute("parentId")]
        public Int32 ParentId { get; set; }

        [XmlAttribute("childId")]
        public Int32 ChildId { get; set; }
    }

    [XmlType("phraseProperties")]
    public class SongPhraseProperties
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("phraseProperty")]
        public SongPhraseProperty[] PhraseProperty { get; set; }
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

    [XmlType("chordTemplates")]
    public class SongChordTemplates
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("chordTemplate")]
        public SongChordTemplate[] ChordTemplate { get; set; }
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

    [XmlType("fretHandMuteTemplates")]
    public class SongFretHandMuteTemplates
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("fretHandMuteTemplate")]
        public SongFretHandMuteTemplate[] FretHandMuteTemplate { get; set; }
    }

    //TBD
    [XmlType("fretHandMuteTemplate")]
    public class SongFretHandMuteTemplate
    {
        //TBD
    }

    [XmlType("ebeats")]
    public class SongEbeats
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("ebeat")]
        public SongEbeat[] Ebeat { get; set; }
    }

    [XmlType("ebeat")]
    public class SongEbeat
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("measure")]
        public Int32 Measure { get; set; }
    }

    [XmlType("sections")]
    public class SongSections
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("section")]
        public SongSection[] Section { get; set; }
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

    [XmlType("events")]
    public class SongEvents
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("event")]
        public SongEvent[] Event { get; set; }
    }

    [XmlType("event")]
    public class SongEvent
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }
    }

    [XmlType("levels")]
    public class SongLevels
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("level")]
        public SongLevel[] Level { get; set; }
    }

    [XmlType("level")]
    public class SongLevel
    {
        [XmlAttribute("difficulty")]
        public Int32 Difficulty { get; set; }

        [XmlElement("notes")]
        public SongNotes Notes { get; set; }

        [XmlElement("chords")]
        public SongChords Chords { get; set; }

        [XmlElement("fretHandMutes")]
        public SongFretHandMutes FretHandMutes { get; set; }

        [XmlElement("anchors")]
        public SongAnchors Anchors { get; set; }

        [XmlElement("handShapes")]
        public SongHandShapes HandShapes { get; set; }
    }

    [XmlType("notes")]
    public class SongNotes
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("note")]
        public SongNote[] Note { get; set; }
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
    }

    [XmlType("chords")]
    public class SongChords
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("chord")]
        public SongChord[] Chord { get; set; }
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

    [XmlType("fretHandMutes")]
    public class SongFretHandMutes
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("fretHandMute")]
        public SongFretHandMute[] FretHandMute { get; set; }
    }

    //TBD
    [XmlType("fretHandMute")]
    public class SongFretHandMute
    {
        //TBD
    }

    [XmlType("controls")]
    public class SongControls
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("control")]
        public SongControl[] Control { get; set; }
    }

    [XmlType("control")]
    public class SongControl
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }
    }

    [XmlType("anchors")]
    public class SongAnchors
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("anchor")]
        public SongAnchor[] Anchor { get; set; }
    }

    [XmlType("anchor")]
    public class SongAnchor
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("fret")]
        public Int32 Fret { get; set; }
    }

    [XmlType("handShapes")]
    public class SongHandShapes
    {
        [XmlAttribute("count")]
        public Int32 Count { get; set; }

        [XmlElement("handShape")]
        public SongHandShape[] HandShape { get; set; }
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
