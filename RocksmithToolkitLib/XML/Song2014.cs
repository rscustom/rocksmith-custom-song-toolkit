using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.Xml {
    [XmlRoot("song", Namespace = "", IsNullable = false)]
    public class Song2014 {
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
        public Byte Capo { get; set; }
        
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
        public SongArrangementProperties2014 ArrangementProperties { get; set; }

        [XmlElement("lastConversionDateTime")]
        public string LastConversionDateTime { get; set; }

        [XmlArray("phrases")]
        [XmlArrayItem("phrase")]
        public SongPhrase[] Phrases { get; set; }

        [XmlArray("phraseIterations")]
        [XmlArrayItem("phraseIteration", typeof(SongPhraseIteration), Type = typeof(SongPhraseIteration2014))]
        public SongPhraseIteration2014[] PhraseIterations { get; set; }

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
        public SongChordTemplate2014[] ChordTemplates { get; set; }

        [XmlArray("fretHandMuteTemplates")]
        [XmlArrayItem("fretHandMuteTemplate")]
        public SongFretHandMuteTemplate[] FretHandMuteTemplates { get; set; }

        [XmlArray("controls")]
        [XmlArrayItem("control")]
        public SongControl[] Controls { get; set; }

        [XmlArray("tones")]
        [XmlArrayItem("tone")]
        public SongTone2014[] Tones { get; set; }

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
        [XmlArrayItem("level", typeof(SongLevel2014))]
        public SongLevel2014[] Levels { get; set; }

        public static Song2014 LoadFromFile(string xmlSongRS2014File) {
            Song2014 xmlSongRS2014 = null;

            using (var reader = new StreamReader(xmlSongRS2014File)) {
                var serializer = new XmlSerializer(typeof(Song2014));
                xmlSongRS2014 = (Song2014)serializer.Deserialize(reader);
            }

            return xmlSongRS2014;
        }
    }

    public class SongArrangementProperties2014 : SongArrangementProperties {
        [JsonProperty("bonusArr")]
        [XmlAttribute("bonusArr")]
        public Int32 BonusArr { get; set; }

        [JsonProperty("pathLead")]
        [XmlAttribute("pathLead")]
        public Int32 PathLead { get; set; }

        [JsonProperty("pathRhythm")]
        [XmlAttribute("pathRhythm")]
        public Int32 PathRhythm { get; set; }

        [JsonProperty("pathBass")]
        [XmlAttribute("pathBass")]
        public Int32 PathBass { get; set; }

        [JsonProperty("routeMask")]
        [XmlAttribute("routeMask")]
        public Int32 RouteMask { get; set; }
    }

    public class SongPhraseIteration2014 : SongPhraseIteration {
        [XmlAttribute("variation")]
        public string Variation { get; set; }
    }

    [XmlType("newLinkedDiffs")]
    public class SongNewLinkedDiff {
        [XmlAttribute("levelBreak")]
        public Int32 LevelBreak { get; set; }

        [XmlAttribute("ratio")]
        public string Ratio { get; set; }

        [XmlAttribute("phraseCount")]
        public Int32 PhraseCount { get; set; }

        [XmlElement("nld_phrase")]
        public List<SongNld_phrase> Nld_phrase { get; set; }
    }

    [XmlType("nld_phrase")]
    public class SongNld_phrase {
        [XmlAttribute("id")]
        public Int32 Id { get; set; }
    }

    public class SongChordTemplate2014 : SongChordTemplate
    {
        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }        
    }

    public class SongLevel2014 {
        [XmlAttribute("difficulty")]
        public Int32 Difficulty { get; set; }

        [XmlArray("notes")]
        [XmlArrayItem("note")]
        public SongNote2014[] Notes { get; set; }

        [XmlArray("chords")]
        [XmlArrayItem("chord")]
        public SongChord2014[] Chords { get; set; }

        [XmlArray("anchors")]
        [XmlArrayItem("anchor")]
        public SongAnchor2014[] Anchors { get; set; }

        [XmlArray("handShapes")]
        [XmlArrayItem("handShape")]
        public SongHandShape[] HandShapes { get; set; }
    }

    public class SongNote2014 : SongNote {
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
        public Int16 Vibrato { get; set; }
    }

    public class SongChord2014 : SongChord {
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
        public SongNote2014[] chordNotes { get; set; }
    }

    public class SongAnchor2014 : SongAnchor {
        [XmlAttribute("width")]
        public Single Width { get; set; }
    }

    [XmlType("tone")]
    public class SongTone2014 {
        [XmlAttribute("time")]
        public float Time { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
