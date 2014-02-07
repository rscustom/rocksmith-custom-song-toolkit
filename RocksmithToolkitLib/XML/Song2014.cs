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
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.Xml {
    [XmlRoot("song", Namespace = "", IsNullable = false)]
    public class Song2014 {
        [XmlAttribute("version")] // RS2014 is 7 or above
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

        [XmlElement("tonebase")]
        public string ToneBase { get; set; }

        [XmlElement("tonea")]
        public string ToneA { get; set; }

        [XmlElement("toneb")]
        public string ToneB { get; set; }

        [XmlElement("tonec")]
        public string ToneC { get; set; }

        [XmlElement("toned")]
        public string ToneD { get; set; }

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

        public Song2014() { }

        public Song2014(Sng2014HSL.Sng sngData, Attributes2014 attr = null) {
            Version = "7";

            if (attr != null) {
                // If manifest is passed, fill general song information
                Title = attr.SongName;
                Arrangement = ((ArrangementName)attr.ArrangementType).ToString();
                Part = (short)attr.SongPartition;
                Offset = attr.SongOffset;
                CentOffset = Convert.ToString(attr.CentOffset);
                SongLength = (float)attr.SongLength;
                SongNameSort = attr.SongNameSort;
                AverageTempo = attr.SongAverageTempo;
                Tuning = attr.Tuning;
                Capo = Convert.ToByte(attr.CapoFret);
                ArtistName = attr.ArtistName;
                ArtistNameSort = attr.ArtistNameSort;
                AlbumName = attr.AlbumName;
                AlbumNameSort = attr.AlbumNameSort;
                AlbumYear = Convert.ToString(attr.SongYear) ?? "";
                AlbumArt = attr.AlbumArt;
                CrowdSpeed = "1";
                ArrangementProperties = attr.ArrangementProperties;
                LastConversionDateTime = attr.LastConversionDateTime;

                ToneBase = attr.Tone_Base;
                ToneA = attr.Tone_A;
                ToneB = attr.Tone_B;
                ToneC = attr.Tone_C;
                ToneD = attr.Tone_D;
            } else {
                Part = sngData.Metadata.Part;
                SongLength = sngData.Metadata.SongLength;
                Tuning = new TuningStrings(sngData.Metadata.Tuning);
                Capo = (sngData.Metadata.CapoFretId >= 0) ? sngData.Metadata.CapoFretId : (byte)0;
                LastConversionDateTime = sngData.Metadata.LastConversionDateTime.ToNullTerminatedAscii();                
            }

            Tones = (attr != null) ? SongTone2014.Parse(sngData.Tones, attr) : SongTone2014.Parse(sngData.Tones);
            
            //Sections can be obtained from manifest or sng file (manifest preferred)
            ChordTemplates = (attr != null) ? SongChordTemplate2014.Parse(attr.ChordTemplates) : SongChordTemplate2014.Parse(sngData.Chords);
            Sections = (attr != null) ? SongSection.Parse(attr.Sections) : SongSection.Parse(sngData.Sections);

            //Can be obtained from manifest or sng file (sng preferred)
            Phrases = SongPhrase.Parse(sngData.Phrases);
            PhraseIterations = SongPhraseIteration2014.Parse(sngData.PhraseIterations);

            //Only in SNG
            Ebeats = SongEbeat.Parse(sngData.BPMs);
            StartBeat = sngData.BPMs.BPMs[0].Time;
            Events = SongEvent.Parse(sngData.Events);

            Levels = SongLevel2014.Parse(sngData);

            //Not used in RS2014 customs at this time. Need to check official files
            NewLinkedDiff = SongNewLinkedDiff.Parse(sngData.NLD);
            PhraseProperties = SongPhraseProperty.Parse(sngData.PhraseExtraInfo);
            LinkedDiffs = new SongLinkedDiff[0];
            FretHandMuteTemplates = new SongFretHandMuteTemplate[0];
        }

        public static Song2014 LoadFromFile(string xmlSongRS2014File) {
            using (var reader = new StreamReader(xmlSongRS2014File))
            {
                return new XmlStreamingDeserializer<Song2014>(reader).Deserialize();
            }
        }

        public void Serialize(Stream stream) {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings {
                Indent = true,
                OmitXmlDeclaration = true
            })) {
                new XmlSerializer(typeof(Song2014)).Serialize(writer, this, ns);
            }

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
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

        [XmlArray("heroLevels")]
        [XmlArrayItem("heroLevel")]
        public HeroLevel[] HeroLevels { get; set; }
        
        public static SongPhraseIteration2014[] Parse(Sng2014HSL.PhraseIterationSection piSection) {
            var piter = new SongPhraseIteration2014[piSection.Count];
            for (int i = 0; i < piSection.Count; i++) {
                var pi = new SongPhraseIteration2014();
                pi.PhraseId = piSection.PhraseIterations[i].PhraseId;
                pi.Time = piSection.PhraseIterations[i].StartTime;
                pi.Variation = "";
                if (!piSection.PhraseIterations[i].Difficulty.SequenceEqual(new Int32[] { 0,0,0 }))
                    pi.HeroLevels = HeroLevel.Parse(piSection.PhraseIterations[i]);
                piter[i] = pi;
            }
            return piter;
        }
    }

    [XmlType("heroLevels")]
    public class HeroLevel {
        [XmlAttribute("hero")]
        public int Hero { get; set; }

        [XmlAttribute("difficulty")]
        public Byte Difficulty { get; set; }

        internal static HeroLevel[] Parse(DLCPackage.Manifest.PhraseIteration phraseIteration) {
            var heroLevels = new HeroLevel[3];
            for (var i = 0; i < heroLevels.Length; i++) {
                var hero = new HeroLevel();
                hero.Hero = i + 1;
                hero.Difficulty = (byte)phraseIteration.MaxScorePerDifficulty[i];
                heroLevels[i] = hero;
            }
            return heroLevels;
        }

        internal static HeroLevel[] Parse(Sng2014HSL.PhraseIteration phraseIteration) {
            var heroLevels = new HeroLevel[3];
            for(var i = 0; i < heroLevels.Length; i++) {
                var hero = new HeroLevel();
                hero.Hero = i + 1;
                hero.Difficulty = (byte)phraseIteration.Difficulty[i];
                heroLevels[i] = hero;
            }
            return heroLevels;
        }
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

        public static SongNewLinkedDiff[] Parse(Sng2014HSL.NLinkedDifficultySection nlinkedDifficultySection) {
            var newLinkedDiff = new SongNewLinkedDiff[nlinkedDifficultySection.Count];
            for (int i = 0; i < nlinkedDifficultySection.Count; i++) {
                var nld = new SongNewLinkedDiff();
                nld.LevelBreak = nlinkedDifficultySection.NLinkedDifficulties[i].LevelBreak;
                nld.PhraseCount = nlinkedDifficultySection.NLinkedDifficulties[i].PhraseCount;
                nld.Nld_phrase = SongNld_phrase.Parse(nlinkedDifficultySection.NLinkedDifficulties[i].NLD_Phrase);
                nld.Ratio = ""; //TODO: ???
            }
            return newLinkedDiff;
        }
    }

    [XmlType("nld_phrase")]
    public class SongNld_phrase {
        [XmlAttribute("id")]
        public Int32 Id { get; set; }

        internal static List<SongNld_phrase> Parse(int[] nldp) {
            var songNldp = new List<SongNld_phrase>();
            foreach (var n in nldp)
                songNldp.Add(new SongNld_phrase() { Id = n });
            return songNldp;
        }
    }

    public class SongChordTemplate2014 
    {
        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }

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

        internal static SongChordTemplate2014[] Parse(List<DLCPackage.Manifest.ChordTemplate> cteamplateList) {
            var chordTemplates = new SongChordTemplate2014[cteamplateList.Count];
            for (int i = 0; i < cteamplateList.Count; i++) {
                var sct2014 = new SongChordTemplate2014();
                sct2014.ChordName = sct2014.DisplayName = cteamplateList[i].ChordName;
                sct2014.Finger0 = (sbyte)cteamplateList[i].Fingers[0];
                sct2014.Finger1 = (sbyte)cteamplateList[i].Fingers[1];
                sct2014.Finger2 = (sbyte)cteamplateList[i].Fingers[2];
                sct2014.Finger3 = (sbyte)cteamplateList[i].Fingers[3];
                sct2014.Finger4 = (sbyte)cteamplateList[i].Fingers[4];
                sct2014.Finger5 = (sbyte)cteamplateList[i].Fingers[5];
                sct2014.Fret0 = (sbyte)cteamplateList[i].Frets[0];
                sct2014.Fret1 = (sbyte)cteamplateList[i].Frets[1];
                sct2014.Fret2 = (sbyte)cteamplateList[i].Frets[2];
                sct2014.Fret3 = (sbyte)cteamplateList[i].Frets[3];
                sct2014.Fret4 = (sbyte)cteamplateList[i].Frets[4];
                sct2014.Fret5 = (sbyte)cteamplateList[i].Frets[5];
                chordTemplates[i] = sct2014;
            }
            return chordTemplates;
        }

        internal static SongChordTemplate2014[] Parse(Sng2014HSL.ChordSection chordSection) {
            var chordTemplates = new SongChordTemplate2014[chordSection.Count];
            for (int i = 0; i < chordSection.Count; i++) {
                var sct2014 = new SongChordTemplate2014();
                sct2014.ChordName = sct2014.DisplayName = chordSection.Chords[i].Name.ToNullTerminatedAscii();
                sct2014.Finger0 = (sbyte)chordSection.Chords[i].Fingers[0];
                sct2014.Finger1 = (sbyte)chordSection.Chords[i].Fingers[1];
                sct2014.Finger2 = (sbyte)chordSection.Chords[i].Fingers[2];
                sct2014.Finger3 = (sbyte)chordSection.Chords[i].Fingers[3];
                sct2014.Finger4 = (sbyte)chordSection.Chords[i].Fingers[4];
                sct2014.Finger5 = (sbyte)chordSection.Chords[i].Fingers[5];
                sct2014.Fret0 = (sbyte)chordSection.Chords[i].Frets[0];
                sct2014.Fret1 = (sbyte)chordSection.Chords[i].Frets[1];
                sct2014.Fret2 = (sbyte)chordSection.Chords[i].Frets[2];
                sct2014.Fret3 = (sbyte)chordSection.Chords[i].Frets[3];
                sct2014.Fret4 = (sbyte)chordSection.Chords[i].Frets[4];
                sct2014.Fret5 = (sbyte)chordSection.Chords[i].Frets[5];
                chordTemplates[i] = sct2014;
            }
            return chordTemplates;
        }
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

        internal static SongLevel2014[] Parse(Sng2014HSL.Sng sngData) {
            var levels = new SongLevel2014[sngData.Arrangements.Count];
            for (var i = 0; i < sngData.Arrangements.Count; i++) {
                var level = new SongLevel2014();
                level.Difficulty = sngData.Arrangements.Arrangements[i].Difficulty;
                level.Notes = SongNote2014.Parse(sngData.Arrangements.Arrangements[i].Notes);
                level.Chords = SongChord2014.Parse(sngData);
                level.Anchors = SongAnchor2014.Parse(sngData.Arrangements.Arrangements[i].Anchors);
                level.HandShapes = SongHandShape.Parse(sngData.Arrangements.Arrangements[i]);
                levels[i] = level;
            }
            return levels;
        }
    }

    public class SongNote2014
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

        [XmlAttribute("linkNext")]
        public Int32 LinkNext { get; set; }

        [XmlAttribute("accent")]
        public Int32 Accent { get; set; }

        [XmlAttribute("leftHand")]
        public SByte LeftHand { get; set; }

        [XmlAttribute("mute")]
        public Int32 Mute { get; set; }

        [XmlAttribute("harmonicPinch")]
        public Int32 HarmonicPinch { get; set; }

        [XmlAttribute("pickDirection")]
        public Int32 PickDirection { get; set; }

        [XmlAttribute("rightHand")]
        public Int32 RightHand { get; set; }

        [XmlAttribute("slideUnpitchTo")]
        public SByte SlideUnpitchTo { get; set; }

        [XmlAttribute("tap")]
        public Byte Tap { get; set; }

        [XmlAttribute("vibrato")]
        public Int16 Vibrato { get; set; }

        [XmlArray("bendValues")]
        [XmlArrayItem("bendValue")]
        public BendValue[] BendValues { get; set; }

        internal static SongNote2014[] Parse(Sng2014HSL.NotesSection notesSection) {
            var notes = new SongNote2014[notesSection.Count];
            for (var i = 0; i < notesSection.Count; i++) {
                var note = new SongNote2014();
                note.Time = notesSection.Notes[i].Time;
                note.Fret = (sbyte)notesSection.Notes[i].FretId;
                note.String = notesSection.Notes[i].StringIndex;
                note.PickDirection = notesSection.Notes[i].PickDirection;
                note.LeftHand = (sbyte)notesSection.Notes[i].LeftHand;
                note.SlideTo = (sbyte)notesSection.Notes[i].SlideTo;
                note.SlideUnpitchTo = (sbyte)notesSection.Notes[i].SlideUnpitchTo;
                note.Tap = notesSection.Notes[i].Tap;
                note.Slap = (sbyte)notesSection.Notes[i].Slap;
                note.Pluck = (sbyte)notesSection.Notes[i].Pluck;
                note.Vibrato = notesSection.Notes[i].Vibrato;
                note.Sustain = notesSection.Notes[i].Sustain;
                note.Bend = (byte)notesSection.Notes[i].MaxBend;
                note.BendValues = BendValue.Parse(notesSection.Notes[i].BendData);
                note.parseNoteMask(notesSection.Notes[i].NoteMask, true);
                notes[i] = note;
            }
            return notes;
        }

        private void parseNoteMask(uint p, bool single) {
            //TODO: Parse note mask here
        }
    }

    [XmlType("bendValues")]
    public class BendValue {
        [XmlAttribute("time")]
        public float Time { get; set; }

        [XmlAttribute("step")]
        public float Step { get; set; }

        [XmlAttribute("unk5")]
        public Byte Unk5 { get; set; }

        internal static BendValue[] Parse(Sng2014HSL.BendDataSection bendDataSection) {
            var bendValues = new BendValue[bendDataSection.Count];
            for (var i = 0; i < bendDataSection.Count; i++) {
                var bend = new BendValue();
                bend.Time = bendDataSection.BendData[i].Time;
                bend.Step = bendDataSection.BendData[i].Step;
                bend.Unk5 = bendDataSection.BendData[i].Unk5;
                bendValues[i] = bend;
            }
            return (bendValues.Length > 0) ? bendValues : null;
        }
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

        internal static SongChord2014[] Parse(Sng2014HSL.Sng sngData) {
            var chords = new SongChord2014[sngData.Chords.Count];
            for (var i = 0; i < sngData.Chords.Count; i++) {
                //var chord = new SongChord2014();
                //TODO: Parse chords here
                //chords[i] = chord;
            }
            return chords;
        }
    }

    public class SongAnchor2014 : SongAnchor {
        [XmlAttribute("width")]
        public Single Width { get; set; }

        internal static SongAnchor2014[] Parse(Sng2014HSL.AnchorSection anchorSection) {
            var anchors = new SongAnchor2014[anchorSection.Count];
            for (var i = 0; i < anchorSection.Count; i++) {
                var anchor = new SongAnchor2014();
                anchor.Time = anchorSection.Anchors[i].StartBeatTime;
                anchor.Fret = anchorSection.Anchors[i].FretId;
                anchor.Width = anchorSection.Anchors[i].Width;
                anchors[i] = anchor;
            }
            return anchors;
        }
    }

    [XmlType("tone")]
    public class SongTone2014 {
        [XmlAttribute("time")]
        public float Time { get; set; }

        [XmlAttribute("id")]
        public Int32 Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        internal static SongTone2014[] Parse(Sng2014HSL.ToneSection toneSection, Attributes2014 attr = null) {
            var tones = new SongTone2014[toneSection.Count];
            for (var i = 0; i < toneSection.Count; i++) {
                var tone = new SongTone2014();
                tone.Id = toneSection.Tones[i].ToneId;
                tone.Time = toneSection.Tones[i].Time;

                if (attr != null) {
                    // Get tone name
                    switch (tone.Id) {
                        case 0:
                            tone.Name = attr.Tone_A;
                            break;
                        case 1:
                            tone.Name = attr.Tone_B;
                            break;
                        case 2:
                            tone.Name = attr.Tone_C;
                            break;
                        case 3:
                            tone.Name = attr.Tone_D;
                            break;
                        default:
                            tone.Name = "importedtone_" + tone.Id;
                            break;
                    }
                } else
                    tone.Name = "importedtone_" + tone.Id;
                    
                tones[i] = tone;
            }
            return tones;
        }
    }
}
