using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.Extensions;
using System.Xml.Linq;

namespace RocksmithToolkitLib.XML
{
    [XmlRoot("song", Namespace = "", IsNullable = false)]
    public class Song
    {
        [XmlAttribute("version")] // RS1 is 4
        public string Version { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

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

        #region EOF Elements

        [XmlIgnore]
        [XmlElement("startBeat")]
        public Single StartBeat { get; set; }

        [XmlIgnore]
        [XmlElement("averageTempo")]
        public Single AverageTempo { get; set; }

        [XmlIgnore]
        [XmlElement("tuning")]
        public TuningStrings Tuning { get; set; }

        [XmlIgnore]
        [XmlElement("artistName")]
        public string ArtistName { get; set; }

        [XmlIgnore]
        [XmlElement("albumName")]
        public string AlbumName { get; set; }

        [XmlIgnore]
        [XmlElement("albumYear")]
        public string AlbumYear { get; set; }

        [XmlIgnore]
        [XmlElement("arrangementProperties")]
        public SongArrangementProperties ArrangementProperties { get; set; }

        #endregion

        #region Old techniques
        //# RS1 old song xml have no arrangement properties
        private bool HasArrangementProperties
        {
            get
            {
                return ArrangementProperties != null;
            }
        }

        public bool HasPowerChords()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.PowerChords == 1;
            else
                return Levels.Any(c => c.Chords != null && HasPowerChords(c.Chords));
        }
        private bool HasPowerChords(SongChord[] songChord)
        {
            return true; //Pending (old song xml only)
        }

        public bool HasBarChords()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.BarreChords == 1;
            else
                return Levels.Any(c => c.Chords != null && HasBarChords(c.Chords));
        }
        private bool HasBarChords(SongChord[] songChord)
        {
            return true; //Pending (old song xml only)
        }

        public bool HasOpenChords()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.OpenChords == 1;
            else
                return Levels.Any(c => c.Chords != null && HasOpenChords(c.Chords));
        }
        private bool HasOpenChords(SongChord[] songChord)
        {
            return true; //Pending (old song xml only)
        }

        public bool HasDoubleStops()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.DoubleStops == 1;
            else
                return Levels.Any(c => c.Chords != null && HasDoubleStops(c.Chords));
        }
        private bool HasDoubleStops(SongChord[] songChord)
        {
            return true; //Pending (old song xml only)
        }

        public bool HasDropDPowerChords()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.DropDPower == 1;
            else
                return Levels.Any(c => c.Chords != null && HasDropDPowerChords(c.Chords));
        }
        private bool HasDropDPowerChords(SongChord[] songChord)
        {
            return true; //Pending (old song xml only)
        }

        public bool HasBends()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Bends == 1;
            else
                return Levels.Any(x => x.Notes != null && x.Notes.Any(y => y.Bend > 0));
        }

        public bool HasSlapAndPop()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.SlapPop == 1;
            else
                return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.Pluck > 0 || y.Slap > 0);
        }

        public bool HasHarmonics()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Harmonics == 1;
            else
                return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.Harmonic > 0);
        }

        public bool HasHOPOs()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Hopo == 1;
            else
                return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.Hopo > 0);
        }

        public bool HasFretHandMutes()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.FretHandMutes == 1;
            else
                return false; //No definition in old XML
        }

        public bool HasPrebends()
        { //Identify only bend, no definition in old XML
            if (HasArrangementProperties)
                return ArrangementProperties.Bends == 1;
            else
                return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.Bend > 0);
        }

        public bool HasVibrato()
        {
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
                return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.PalmMute > 0);
        }

        public bool HasSlides()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Slides == 1;
            else
                return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.SlideTo >= 0);
        }

        public bool HasSustain()
        {
            return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.Sustain > 0);
        }

        public bool HasTremolo()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Tremolo == 1;
            else
                return Levels.SelectMany(x => x.Notes ?? new SongNote[0]).Any(y => y.Tremolo > 0);
        }

        public bool HasTwoFingerPlucking()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.TwoFingerPicking == 1;
            else
                return false; //No definition in old XML
        }

        public bool HasFifthsAndOctaves()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.FifthsAndOctaves == 1;
            else
                return false; //No definition in old XML
        }

        public bool HasSyncopation()
        {
            if (HasArrangementProperties)
                return ArrangementProperties.Syncopation == 1;
            else
                return false; //No definition in old XML
        }

        #endregion

        public void Serialize(Stream stream, bool omitXmlDeclaration = false)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var song = new MemoryStream();
            using (var writer = XmlWriter.Create(song, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = omitXmlDeclaration, Encoding = new UTF8Encoding(false) }))
            {
                new XmlSerializer(typeof(Song)).Serialize(writer, this, ns);
            }

            FixArrayAttribs(song);
            stream.Write(song.GetBuffer(), 0, (int)song.Position);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Writes attribute count for specific nodes (required/used by DDC)
        /// </summary>
        /// <param name="xml">Xml stream.</param>
        private static void FixArrayAttribs(Stream xml)
        {
            string[] anodes = { "phrases", "phraseIterations", "newLinkedDiffs", "linkedDiffs", "phraseProperties", "chordTemplates", "fretHandMuteTemplates", "fretHandMutes",
                                  /*Required by DDC*/ "ebeats", "sections", "events", "levels", "notes", "chords", "anchors", "handShapes", "tones" };

            xml.Position = 0;
            var doc = XDocument.Load(xml);
            foreach (var n in anodes)
            {
                var es = doc.Descendants(n).ToArray();
                if (!es.Any()) continue;
                foreach (var e in es)
                {
                    var ret = e.Attribute("count");
                    if (ret == null)
                        e.Add(new XAttribute("count", e.Elements().Count()));
                    else
                        ret.SetValue(e.Elements().Count());
                }
            }
            xml.Position = 0;
            doc.Save(xml);
        }

        public static Song LoadFromFile(string xmlSongFile)
        {
            Song XmlSong = null;

            using (var reader = new StreamReader(xmlSongFile))
            {
                var serializer = new XmlSerializer(typeof(Song));
                XmlSong = (Song)serializer.Deserialize(reader);
            }

            return XmlSong;
        }
    }

    [XmlType("arrangementProperties")]
    public class SongArrangementProperties
    {
        [JsonProperty("represent")]
        [XmlAttribute("represent")]
        public Int32 Represent { get; set; }

        [JsonProperty("standardTuning")]
        [XmlAttribute("standardTuning")]
        public Int32 StandardTuning { get; set; }

        [JsonProperty("nonStandardChords")]
        [XmlAttribute("nonStandardChords")]
        public Int32 NonStandardChords { get; set; }

        [JsonProperty("barreChords")]
        [XmlAttribute("barreChords")]
        public Int32 BarreChords { get; set; }

        [JsonProperty("powerChords")]
        [XmlAttribute("powerChords")]
        public Int32 PowerChords { get; set; }

        [JsonProperty("dropDPower")]
        [XmlAttribute("dropDPower")]
        public Int32 DropDPower { get; set; }

        [JsonProperty("openChords")]
        [XmlAttribute("openChords")]
        public Int32 OpenChords { get; set; }

        [JsonProperty("fingerPicking")]
        [XmlAttribute("fingerPicking")]
        public Int32 FingerPicking { get; set; }

        [JsonProperty("pickDirection")]
        [XmlAttribute("pickDirection")]
        public sbyte PickDirection { get; set; }

        [JsonProperty("doubleStops")]
        [XmlAttribute("doubleStops")]
        public Int32 DoubleStops { get; set; }

        [JsonProperty("palmMutes")]
        [XmlAttribute("palmMutes")]
        public Int32 PalmMutes { get; set; }

        [JsonProperty("harmonics")]
        [XmlAttribute("harmonics")]
        public Int32 Harmonics { get; set; }

        [JsonProperty("pinchHarmonics")]
        [XmlAttribute("pinchHarmonics")]
        public Int32 PinchHarmonics { get; set; }

        [JsonProperty("hopo")]
        [XmlAttribute("hopo")]
        public Int32 Hopo { get; set; }

        [JsonProperty("tremolo")]
        [XmlAttribute("tremolo")]
        public Int32 Tremolo { get; set; }

        [JsonProperty("slides")]
        [XmlAttribute("slides")]
        public Int32 Slides { get; set; }

        [JsonProperty("unpitchedSlides")]
        [XmlAttribute("unpitchedSlides")]
        public Int32 UnpitchedSlides { get; set; }

        [JsonProperty("bends")]
        [XmlAttribute("bends")]
        public Int32 Bends { get; set; }

        [JsonProperty("tapping")]
        [XmlAttribute("tapping")]
        public Int32 Tapping { get; set; }

        [JsonProperty("vibrato")]
        [XmlAttribute("vibrato")]
        public Int16 Vibrato { get; set; }

        [JsonProperty("fretHandMutes")]
        [XmlAttribute("fretHandMutes")]
        public Int32 FretHandMutes { get; set; }

        [JsonProperty("slapPop")]
        [XmlAttribute("slapPop")]
        public Int32 SlapPop { get; set; }

        [JsonProperty("twoFingerPicking")]
        [XmlAttribute("twoFingerPicking")]
        public Int32 TwoFingerPicking { get; set; }

        [JsonProperty("fifthsAndOctaves")]
        [XmlAttribute("fifthsAndOctaves")]
        public Int32 FifthsAndOctaves { get; set; }

        [JsonProperty("syncopation")]
        [XmlAttribute("syncopation")]
        public Int32 Syncopation { get; set; }

        [JsonProperty("bassPick")]
        [XmlAttribute("bassPick")]
        public Int32 BassPick { get; set; }

        [JsonProperty("sustain")]
        [XmlAttribute("sustain")]
        public Int32 Sustain { get; set; }
    }

    // TODO: monitor that xml templates are properly serialized
    // the use of [Serializable] may causes malformed tunining elements in templates
    [DataContract] // here for datacontract templates to work properly
    [Serializable] // here for bin serializer to work properly in other programs
    [XmlType("tuning")]
    public class TuningStrings : IEquatable<TuningStrings>
    {
        // stay consistent SNG TuningStrings data stored as Int16 (short)
        [DataMember]
        [JsonProperty("string0")]
        [XmlAttribute("string0")]
        public Int16 String0 { get; set; }

        [DataMember]
        [JsonProperty("string1")]
        [XmlAttribute("string1")]
        public Int16 String1 { get; set; }

        [DataMember]
        [JsonProperty("string2")]
        [XmlAttribute("string2")]
        public Int16 String2 { get; set; }

        [DataMember]
        [JsonProperty("string3")]
        [XmlAttribute("string3")]
        public Int16 String3 { get; set; }

        [DataMember]
        [JsonProperty("string4")]
        [XmlAttribute("string4")]
        public Int16 String4 { get; set; }

        [DataMember]
        [JsonProperty("string5")]
        [XmlAttribute("string5")]
        public Int16 String5 { get; set; }

        public TuningStrings() { }

        public TuningStrings(Int16[] stringArray)
        {
            String0 = stringArray[0];
            String1 = stringArray[1];
            String2 = stringArray[2];
            String3 = stringArray[3];
            if (stringArray.Length > 4)
                String4 = stringArray[4];
            if (stringArray.Length > 5)
                String5 = stringArray[5];
        }

        [Obsolete("Deprecated, please use regular ToArray() method.", true)]
        public Int16[] ToShortArray()
        {
            return ToArray();
        }

        public Int16[] ToArray()
        {
            return new Int16[] { String0, String1, String2, String3, String4, String5 };
        }

        [Obsolete("Deprecated, please use regular ToArray() function.", true)]
        public Int16[] ToBassArray()
        {
            return new Int16[] { String0, String1, String2, String3, 0, 0 };
        }

        #region IEquatable implementation
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as TuningStrings;
            return other != null && Equals(other);
        }

        public bool Equals(TuningStrings other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.ToArray().SequenceEqual(other.ToArray());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = String0.GetHashCode();
                hashCode = (hashCode * 397) ^ String1.GetHashCode();
                hashCode = (hashCode * 397) ^ String2.GetHashCode();
                hashCode = (hashCode * 397) ^ String3.GetHashCode();
                hashCode = (hashCode * 397) ^ String4.GetHashCode();
                hashCode = (hashCode * 397) ^ String5.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TuningStrings left, TuningStrings right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TuningStrings left, TuningStrings right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    [XmlType("phrase")]
    public class SongPhrase
    {
        [XmlAttribute("disparity")]
        public Byte Disparity { get; set; }

        [XmlAttribute("ignore")]
        public Byte Ignore { get; set; }

        [XmlAttribute("maxDifficulty")]
        public Int32 MaxDifficulty { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("solo")]
        public Byte Solo { get; set; }

        internal static SongPhrase[] Parse(List<DLCPackage.Manifest.Phrase> phraseList)
        {
            var phrases = new SongPhrase[phraseList.Count];
            for (int i = 0; i < phraseList.Count; i++)
            {
                phrases[i] = new SongPhrase
                {
                    //Disparity = 0;
                    //Ignore = 0;
                    MaxDifficulty = phraseList[i].MaxDifficulty,
                    Name = phraseList[i].Name,
                    Solo = (byte)(phraseList[i].Name.ToLower().Contains("solo") ? 1 : 0)
                };
            }
            return phrases;
        }

        public static SongPhrase[] Parse(Sng2014HSL.PhraseSection sngPhraseSection)
        {
            var phrases = new SongPhrase[sngPhraseSection.Count];
            for (int i = 0; i < sngPhraseSection.Count; i++)
            {
                phrases[i] = new SongPhrase
                {
                    Disparity = sngPhraseSection.Phrases[i].Disparity,
                    Ignore = sngPhraseSection.Phrases[i].Ignore,
                    MaxDifficulty = sngPhraseSection.Phrases[i].MaxDifficulty,
                    Name = sngPhraseSection.Phrases[i].Name.ToNullTerminatedAscii(),
                    Solo = sngPhraseSection.Phrases[i].Solo
                };
            }
            return phrases;
        }
    }

    [XmlType("phraseIteration")]
    public class SongPhraseIteration
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("phraseId")]
        public Int32 PhraseId { get; set; }
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

        internal static SongPhraseProperty[] Parse(Sng2014HSL.PhraseExtraInfoByLevelSection phraseExtraInfoByLevelSection)
        {
            var phraseProperties = new SongPhraseProperty[phraseExtraInfoByLevelSection.Count];
            for (var i = 0; i < phraseExtraInfoByLevelSection.Count; i++)
            {
                phraseProperties[i] = new SongPhraseProperty
                {
                    PhraseId = phraseExtraInfoByLevelSection.PhraseExtraInfoByLevel[i].PhraseId,
                    Redundant = phraseExtraInfoByLevelSection.PhraseExtraInfoByLevel[i].Redundant,
                    LevelJump = phraseExtraInfoByLevelSection.PhraseExtraInfoByLevel[i].LevelJump,
                    Empty = phraseExtraInfoByLevelSection.PhraseExtraInfoByLevel[i].Empty,
                    Difficulty = phraseExtraInfoByLevelSection.PhraseExtraInfoByLevel[i].Difficulty
                };
            }
            return phraseProperties;
        }
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

        private Int16 _measure = -1; // sets default value -1
        [XmlAttribute("measure")]
        public Int16 Measure
        {
            get { return _measure; }
            set { _measure = value; }
        }

        internal static SongEbeat[] Parse(Sng2014HSL.BpmSection bpmSection)
        {
            var songEbeats = new SongEbeat[bpmSection.Count];
            for (var i = 0; i < bpmSection.Count; i++)
            {
                var sEbeat = new SongEbeat();
                sEbeat.Time = bpmSection.BPMs[i].Time;
                sEbeat.Measure = -1;
                if (bpmSection.BPMs[i].Mask != 0)
                    sEbeat.Measure = bpmSection.BPMs[i].Measure;
                songEbeats[i] = sEbeat;
            }
            return songEbeats;
        }
    }

    [XmlType("section")]
    public class SongSection
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("number")]
        public Int32 Number { get; set; }

        [XmlAttribute("startTime")]
        public Single StartTime { get; set; }

        internal static SongSection[] Parse(List<DLCPackage.Manifest.Section> manifestSectionList)
        {
            var songSections = new SongSection[manifestSectionList.Count];
            for (int i = 0; i < manifestSectionList.Count; i++)
            {
                var songSection = new SongSection();
                songSection.Name = manifestSectionList[i].Name;
                songSection.Number = manifestSectionList[i].Number;
                songSection.StartTime = manifestSectionList[i].StartTime;
                songSections[i] = songSection;
            }
            return songSections;
        }

        internal static SongSection[] Parse(Sng2014HSL.SectionSection sectionSection)
        {
            var songSections = new SongSection[sectionSection.Count];
            for (int i = 0; i < sectionSection.Count; i++)
            {
                var songSection = new SongSection();
                songSection.Name = sectionSection.Sections[i].Name.ToNullTerminatedAscii();
                songSection.Number = sectionSection.Sections[i].Number;
                songSection.StartTime = sectionSection.Sections[i].StartTime;
                songSections[i] = songSection;
            }
            return songSections;
        }
    }

    [XmlType("event")]
    public class SongEvent
    {
        [XmlAttribute("time")]
        public Single Time { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }

        internal static SongEvent[] Parse(Sng2014HSL.EventSection eventSection)
        {
            var songEvents = new SongEvent[eventSection.Count];
            for (var i = 0; i < eventSection.Count; i++)
            {
                songEvents[i] = new SongEvent
                {
                    Code = eventSection.Events[i].EventName.ToNullTerminatedAscii(),
                    Time = eventSection.Events[i].Time
                };
            }
            return songEvents;
        }
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
        [XmlAttribute("chordId")]
        public Int32 ChordId { get; set; }

        [XmlAttribute("endTime")]
        public Single EndTime { get; set; }

        [XmlAttribute("startTime")]
        public Single StartTime { get; set; }

        internal static SongHandShape[] Parse(Sng2014HSL.Arrangement arrangement)
        {
            var count = arrangement.Fingerprints1.Count + arrangement.Fingerprints2.Count;

            var fprints = new List<Sng2014HSL.Fingerprint>();
            fprints.AddRange(arrangement.Fingerprints1.Fingerprints);
            fprints.AddRange(arrangement.Fingerprints2.Fingerprints);
            fprints = fprints.OrderBy(e => e.StartTime).ToList<Sng2014HSL.Fingerprint>();

            var hshapes = new SongHandShape[count];
            for (var i = 0; i < count; i++)
            {
                var hs = new SongHandShape();
                hs.StartTime = fprints[i].StartTime;
                hs.EndTime = fprints[i].EndTime;
                hs.ChordId = fprints[i].ChordId;
                hshapes[i] = hs;
            }

            return hshapes;
        }

    }
}
