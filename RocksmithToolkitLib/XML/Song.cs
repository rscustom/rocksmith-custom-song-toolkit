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
        public SongPhrases(): this(new SongPhrase[0]) { }
        public SongPhrases(IEnumerable<SongPhrase> phrases)
        {
            Phrase = phrases.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Phrase == null ? 0 : Phrase.Length;
            }
            set { }
        }

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
        public SongPhraseIterations(): this(new SongPhraseIteration[0]) { }
        public SongPhraseIterations(IEnumerable<SongPhraseIteration> phraseIterations)
        {
            PhraseIteration = phraseIterations.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return PhraseIteration == null ? 0 : PhraseIteration.Length;
            }
            set { }
        }

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
        public SongLinkedDiffs(): this(new SongLinkedDiff[0]) { }
        public SongLinkedDiffs(IEnumerable<SongLinkedDiff> linkedDiffs)
        {
            LinkedDiff = linkedDiffs.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return LinkedDiff == null ? 0 : LinkedDiff.Length;
            }
            set { }
        }

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
        public SongPhraseProperties(): this(new SongPhraseProperty[0]) { }
        public SongPhraseProperties(IEnumerable<SongPhraseProperty> phraseProperties)
        {
            PhraseProperty = phraseProperties.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return PhraseProperty == null ? 0 : PhraseProperty.Length;
            }
            set { }
        }

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
        public SongChordTemplates(): this(new SongChordTemplate[0]) { }
        public SongChordTemplates(IEnumerable<SongChordTemplate> chordTemplates)
        {
            ChordTemplate = chordTemplates.ToArray();
        }
        
        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return ChordTemplate == null ? 0 : ChordTemplate.Length;
            }
            set { }
        }

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
        public SongFretHandMuteTemplates(): this(new SongFretHandMuteTemplate[0]) { }
        public SongFretHandMuteTemplates(IEnumerable<SongFretHandMuteTemplate> fretHandMuteTemplates)
        {
            FretHandMuteTemplate = fretHandMuteTemplates.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return FretHandMuteTemplate == null ? 0 : FretHandMuteTemplate.Length;
            }
            set { }
        }

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
        public SongEbeats(): this(new SongEbeat[0]) { }
        public SongEbeats(IEnumerable<SongEbeat> ebeats)
        {
            Ebeat = ebeats.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Ebeat == null ? 0 : Ebeat.Length;
            }
            set { }
        }

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
        public SongSections(): this(new SongSection[0]) { }
        public SongSections(IEnumerable<SongSection> sections)
        {
            Section = sections.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Section == null ? 0 : Section.Length;
            }
            set { }
        }

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
        public SongEvents(): this(new SongEvent[0]) { }
        public SongEvents(IEnumerable<SongEvent> events)
        {
            Event = events.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Event == null ? 0 : Event.Length;
            }
            set { }
        }

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
        public SongLevels(): this(new SongLevel[0]) { }
        public SongLevels(IEnumerable<SongLevel> levels)
        {
            Level = levels.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Level == null ? 0 : Level.Length;
            }
            set { }
        }

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
        public SongNotes(): this(new SongNote[0]) { }
        public SongNotes(IEnumerable<SongNote> notes)
        {
            Note = notes.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Note == null ? 0 : Note.Length;
            }
            set { }
        }

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
        public SongChords(): this(new SongChord[0]) { }
        public SongChords(IEnumerable<SongChord> chords)
        {
            Chord = chords.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Chord == null ? 0 : Chord.Length;
            }
            set { }
        }

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
        public SongFretHandMutes(): this(new SongFretHandMute[0]) { }
        public SongFretHandMutes(IEnumerable<SongFretHandMute> fretHandMutes)
        {
            FretHandMute = fretHandMutes.ToArray();
        }
       
        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return FretHandMute == null ? 0 : FretHandMute.Length;
            }
            set { }
        }

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
        public SongControls(): this(new SongControl[0]) { }
        public SongControls(IEnumerable<SongControl> controls)
        {
            Control = controls.ToArray();
        }
        
        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Control == null ? 0 : Control.Length;
            }
            set { }
        }

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
        public SongAnchors(): this(new SongAnchor[0]) { }
        public SongAnchors(IEnumerable<SongAnchor> anchors)
        {
            Anchor = anchors.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return Anchor == null ? 0 : Anchor.Length;
            }
            set { }
        }

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
        public SongHandShapes(): this(new SongHandShape[0]) { }
        public SongHandShapes(IEnumerable<SongHandShape> handShapes)
        {
            HandShape = handShapes.ToArray();
        }

        [XmlAttribute("count")]
        public Int32 Count
        {
            get
            {
                return HandShape == null ? 0 : HandShape.Length;
            }
            set { }
        }

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
