using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace RocksmithToolkitLib.Xml {
    [XmlRoot("song", Namespace = "", IsNullable = false)]
    public class SongRS2014 {
        [XmlAttribute("version")] // RS2014 is 7
        public string Version { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("arrangement")]
        public string Arrangement { get; set; }

        [XmlElement("part")]
        public Int16 Part { get; set; }

        [XmlElement("offset")]
        public Single Offset { get; set; }

        [XmlElement("centOffset")]
        public string CentOffset { get; set; }

        [XmlElement("songLength")]
        public Single SongLength { get; set; }

        [XmlElement("songNameSort")]
        public string SongNameSort { get; set; }

        [XmlElement("startBeat")]
        public Single StartBeat { get; set; }

        [XmlElement("averageTempo")]
        public Single AverageTempo { get; set; }

        [XmlElement("tuning")]
        public TuningStrings Tuning { get; set; }

        [XmlElement("capo")]
        public string Capo { get; set; }
        
        [XmlElement("artistName")]
        public string ArtistName { get; set; }

        [XmlElement("artistNameSort")]
        public string ArtistNameSort { get; set; }

        [XmlElement("albumName")]
        public string AlbumName { get; set; }

        [XmlElement("albumNameSort")]
        public string AlbumNameSort { get; set; }

        [XmlElement("albumYear")]
        public string AlbumYear { get; set; }

        [XmlElement("albumArt")]
        public string AlbumArt { get; set; }

        [XmlElement("crowdSpeed")]
        public string CrowdSpeed { get; set; }

        [XmlElement("arrangementProperties")]
        public SongArrangementPropertiesAttributes ArrangementProperties { get; set; }

        [XmlElement("lastConversionDateTime")]
        public string LastConversionDateTime { get; set; }

        [XmlArray("phrases")]
        [XmlArrayItem("phrase")]
        public SongPhrase[] Phrases { get; set; }

        [XmlArray("phraseIterations")]
        [XmlArrayItem("phraseIteration", typeof(SongPhraseIteration), Type = typeof(SongPhraseIterationRS2014))]
        public SongPhraseIterationRS2014[] PhraseIterations { get; set; }

        [XmlArray("newLinkedDiffs")]
        [XmlArrayItem("newLinkedDiff")]
        public SongNewLinkedDiff[] NewLinkedDiff { get; set; }

        [XmlArray("linkedDiffs")]
        [XmlArrayItem("linkedDiff")]
        public SongLinkedDiff[] LinkedDiffs { get; set; }

        [XmlArray("phraseProperties")]
        [XmlArrayItem("phraseProperty")]
        public SongPhraseProperty[] PhraseProperties { get; set; }

        [XmlArray("chordTemplates")]
        [XmlArrayItem("chordTemplate")]
        public SongChordTemplateRS2014[] ChordTemplates { get; set; }

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
        [XmlArrayItem("level", typeof(SongLevelRS2014))]
        public SongLevelRS2014[] Levels { get; set; }

        public static SongRS2014 LoadFromFile(string xmlSongRS2014File) {
            SongRS2014 xmlSongRS2014 = null;

            using (var reader = new StreamReader(xmlSongRS2014File)) {
                var serializer = new XmlSerializer(typeof(SongRS2014));
                xmlSongRS2014 = (SongRS2014)serializer.Deserialize(reader);
            }

            return xmlSongRS2014;
        }
    }

    [XmlType("arrangementProperties")]
    public class SongArrangementPropertiesAttributes {
        [XmlAttribute("represent")]
        public Int32 Represent { get; set; }

        [XmlAttribute("bonusArr")]
        public Int32 BonusArr { get; set; }

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
        public Int32 PickDirection { get; set; }

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
        public Int32 Vibrato { get; set; }

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

        [XmlAttribute("sustain")]
        public Int32 Sustain { get; set; }

        [XmlAttribute("pathLead")]
        public Int32 PathLead { get; set; }

        [XmlAttribute("pathRhythm")]
        public Int32 PathRhythm { get; set; }

        [XmlAttribute("pathBass")]
        public Int32 PathBass { get; set; }
    }

    public class SongPhraseIterationRS2014 : SongPhraseIteration {
        [XmlAttribute("variation")]
        public string Variation { get; set; }
    }

    [XmlType("newLinkedDiffs")]
    public class SongNewLinkedDiff {
        [XmlAttribute("levelBreak")]
        public string LevelBreak { get; set; }

        [XmlAttribute("ratio")]
        public string Ratio { get; set; }

        [XmlAttribute("phraseCount")]
        public string PhraseCount { get; set; }

        [XmlElement("nld_phrase")]
        public SongNld_phrase Nld_phrase { get; set; }
    }

    [XmlType("nld_phrase")]
    public class SongNld_phrase {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }

    public class SongChordTemplateRS2014 : SongChordTemplate {
        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }
        
    }

    public class SongLevelRS2014 {
        [XmlAttribute("difficulty")]
        public Int32 Difficulty { get; set; }

        [XmlArray("notes")]
        [XmlArrayItem("note")]
        public SongNoteRS2014[] Notes { get; set; }

        [XmlArray("chords")]
        [XmlArrayItem("chord")]
        public SongChordRS2014[] Chords { get; set; }

        [XmlArray("anchors")]
        [XmlArrayItem("anchor")]
        public SongAnchorRS2014[] Anchors { get; set; }

        [XmlArray("handShapes")]
        [XmlArrayItem("handShape")]
        public SongHandShape[] HandShapes { get; set; }
    }

    public class SongNoteRS2014 : SongNote {
        [XmlAttribute("linkNext")]
        public Int32 LinkNext { get; set; }

        [XmlAttribute("accent")]
        public Int32 Accent { get; set; }

        [XmlAttribute("leftHand")]
        public Int32 LeftHand { get; set; }

        [XmlAttribute("mute")]
        public Int32 Mute { get; set; }

        [XmlAttribute("harmonicPinch")]
        public Int32 HarmonicPinch { get; set; }

        [XmlAttribute("pickDirection")]
        public Int32 PickDirection { get; set; }

        [XmlAttribute("rightHand")]
        public Int32 RightHand { get; set; }

        [XmlAttribute("slideUnpitchTo")]
        public Int32 SlideUnpitchTo { get; set; }

        [XmlAttribute("tap")]
        public Int32 Tap { get; set; }

        [XmlAttribute("vibrato")]
        public Int32 Vibrato { get; set; }
    }

    public class SongChordRS2014 : SongChord {
        [XmlAttribute("linkNext")]
        public Int32 LinkNext { get; set; }

        [XmlAttribute("accent")]
        public Int32 Accent { get; set; }

        [XmlAttribute("fretHandMute")]
        public Int32 FretHandMute { get; set; }

        [XmlAttribute("palmMute")]
        public Int32 PalmMute { get; set; }

        [XmlAttribute("hopo")]
        public Int32 Hopo { get; set; }

        [XmlElement("chordNote")]
        public SongNoteRS2014[] chordNotes { get; set; }
    }

    public class SongAnchorRS2014 : SongAnchor {
        [XmlAttribute("width")]
        public Single Width { get; set; }
    }
}
