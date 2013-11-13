using System;
using System.IO;

namespace RocksmithToolkitLib.Sng
{
    public class Sng2014 {
        public BpmSection BPMs { get; set; }
        public PhraseSection Phrases { get; set; }
        public ChordSection Chords { get; set; }
        public ChordNotesSection ChordNotes { get; set; }

        public VocalSection Vocals { get; set; }
        public SymbolsHeaderSection SymbolsHeader { get; set; }
        public SymbolsTextureSection SymbolsTexture { get; set; }
        public SymbolDefinitionSection SymbolsDefinition { get; set; }

        public PhraseIterationSection PhraseIterations { get; set; }
        public PhraseExtraInfoByLevelSection PhraseExtraInfo { get; set; }
        public NLinkedDifficultySection NLD { get; set; }
        public ActionSection Actions { get; set; }
        public EventSection Events { get; set; }
        public ToneSection Tones { get; set; }
        public DnaSection DNAs { get; set; }
        public SectionSection Sections { get; set; }
        public ArrangementSection Arrangements { get; set; }
        public Metadata2014 Metadata { get; set; }

        public string[] _order = {
            "BPMs",
            "Phrases",
            "Chords",
            "ChordNotes",
            "Vocals",
            "SymbolsHeader",
            "SymbolsTexture",
            "SymbolsDefinition",
            "PhraseIterations",
            "PhraseExtraInfo",
            "NLD",
            "Actions",
            "Events",
            "Tones",
            "DNAs",
            "Sections",
            "Arrangements",
            "Metadata"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.BPMs = new BpmSection(); this.BPMs.Read(r);
            this.Phrases = new PhraseSection(); this.Phrases.Read(r);
            this.Chords = new ChordSection(); this.Chords.Read(r);
            this.ChordNotes = new ChordNotesSection(); this.ChordNotes.Read(r);
            this.Vocals = new VocalSection(); this.Vocals.Read(r);
            this.SymbolsHeader = new SymbolsHeaderSection(); this.SymbolsHeader.Read(r);
            this.SymbolsTexture = new SymbolsTextureSection(); this.SymbolsTexture.Read(r);
            this.SymbolsDefinition = new SymbolDefinitionSection(); this.SymbolsDefinition.Read(r);
            this.PhraseIterations = new PhraseIterationSection(); this.PhraseIterations.Read(r);
            this.PhraseExtraInfo = new PhraseExtraInfoByLevelSection(); this.PhraseExtraInfo.Read(r);
            this.NLD = new NLinkedDifficultySection(); this.NLD.Read(r);
            this.Actions = new ActionSection(); this.Actions.Read(r);
            this.Events = new EventSection(); this.Events.Read(r);
            this.Tones = new ToneSection(); this.Tones.Read(r);
            this.DNAs = new DnaSection(); this.DNAs.Read(r);
            this.Sections = new SectionSection(); this.Sections.Read(r);
            this.Arrangements = new ArrangementSection(); this.Arrangements.Read(r);
            this.Metadata = new Metadata2014(); this.Metadata.Read(r);
        }
    }

    public class SngBpm
    {
        public float Time { get; set; }
        public Int16 Measure { get; set; }
        public Int16 Beat { get; set; }
        public Int32 PhraseIteration { get; set; }
        public Int32 Mask { get; set; }

        public string[] _order = {
            "Time",
            "Measure",
            "Beat",
            "PhraseIteration",
            "Mask"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Time = r.ReadSingle();
            this.Measure = r.ReadInt16();
            this.Beat = r.ReadInt16();
            this.PhraseIteration = r.ReadInt32();
            this.Mask = r.ReadInt32();
        }
    }
    
    public class BpmSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngBpm[] BPMs { get; set; }

        public string[] _order = {
            "Count",
            "BPMs"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.BPMs = new SngBpm[this.Count]; for (int i = 0; i < this.Count; i++) { SngBpm obj = new SngBpm(); obj.Read(r); this.BPMs[i] = obj; }
        }
    }

    public class SngPhrase2014
    {
        public Byte Solo { get; set; }
        public Byte Disparity { get; set; }
        public Byte Ignore { get; set; }
        public Byte Padding { get; set; }
        public Int32 MaxDifficulty { get; set; }
        public Int32 PhraseIterationLinks { get; set; }
        public Byte[] _Name = new Byte[32];
        public Byte[] Name { get { return this._Name; } set { _Name = value; } }

        public string[] _order = {
            "Solo",
            "Disparity",
            "Ignore",
            "Padding",
            "MaxDifficulty",
            "PhraseIterationLinks",
            "Name"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Solo = r.ReadByte();
            this.Disparity = r.ReadByte();
            this.Ignore = r.ReadByte();
            this.Padding = r.ReadByte();
            this.MaxDifficulty = r.ReadInt32();
            this.PhraseIterationLinks = r.ReadInt32();
            this.Name = r.ReadBytes(32);
        }
    }

    public class PhraseSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngPhrase2014[] Phrases { get; set; }

        public string[] _order = {
            "Count",
            "Phrases"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Phrases = new SngPhrase2014[this.Count]; for (int i = 0; i < this.Count; i++) { SngPhrase2014 obj = new SngPhrase2014(); obj.Read(r); this.Phrases[i] = obj; }
        }
    }

    public class SngChord
    {
        public Int32 Mask { get; set; }
        public Byte[] _Frets = new Byte[6];
        public Byte[] Frets { get { return this._Frets; } set { _Frets = value; } }
        public Byte[] _Fingers = new Byte[6];
        public Byte[] Fingers { get { return this._Fingers; } set { _Fingers = value; } }
        public Int32[] _Notes = new Int32[6];
        public Int32[] Notes { get { return this._Notes; } set { _Notes = value; } }
        public Byte[] _Name = new Byte[32];
        public Byte[] Name { get { return this._Name; } set { _Name = value; } }

        public string[] _order = {
            "Mask",
            "Frets",
            "Fingers",
            "Notes",
            "Name"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Mask = r.ReadInt32();
            this.Frets = r.ReadBytes(6);
            this.Fingers = r.ReadBytes(6);
            this.Notes = new Int32[6]; for (int i=0; i<6; i++) this.Notes[i] = r.ReadInt32();
            this.Name = r.ReadBytes(32);
        }
    }

    public class ChordSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngChord[] Chords { get; set; }

        public string[] _order = {
            "Count",
            "Chords"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Chords = new SngChord[this.Count]; for (int i = 0; i < this.Count; i++) { SngChord obj = new SngChord(); obj.Read(r); this.Chords[i] = obj; }
        }
    }

    public class SngBendData32
    {
        public float Time { get; set; }
        public float Step { get; set; }
        public Int16 Unk3_0 { get; set; }
        public Byte Unk4_0 { get; set; }
        public Byte Unk5 { get; set; }

        public string[] _order = {
            "Time",
            "Step",
            "Unk3_0",
            "Unk4_0",
            "Unk5"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Time = r.ReadSingle();
            this.Step = r.ReadSingle();
            this.Unk3_0 = r.ReadInt16();
            this.Unk4_0 = r.ReadByte();
            this.Unk5 = r.ReadByte();
        }
    }

    public class SngBendData
    {
        public SngBendData32[] _BendData32 = new SngBendData32[32];
        public SngBendData32[] BendData32 { get { return this._BendData32; } set { _BendData32 = value; } }
        public Int32 UsedCount { get; set; }

        public string[] _order = {
            "BendData32",
            "UsedCount"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.BendData32 = new SngBendData32[32]; for (int i = 0; i < 32; i++) { SngBendData32 obj = new SngBendData32(); obj.Read(r); this.BendData32[i] = obj; }
            this.UsedCount = r.ReadInt32();
        }
    }

    public class BendDataSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngBendData32[] BendData { get; set; }

        public string[] _order = {
            "Count",
            "BendData"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.BendData = new SngBendData32[this.Count]; for (int i = 0; i < this.Count; i++) { SngBendData32 obj = new SngBendData32(); obj.Read(r); this.BendData[i] = obj; }
        }
    }

    public class ChordNotes
    {
        public Int32[] _NoteMask = new Int32[6];
        public Int32[] NoteMask { get { return this._NoteMask; } set { _NoteMask = value; } }
        public SngBendData[] _BendData = new SngBendData[6];
        public SngBendData[] BendData { get { return this._BendData; } set { _BendData = value; } }
        public Byte[] _StartFretId = new Byte[6];
        public Byte[] StartFretId { get { return this._StartFretId; } set { _StartFretId = value; } }
        public Byte[] _EndFretId = new Byte[6];
        public Byte[] EndFretId { get { return this._EndFretId; } set { _EndFretId = value; } }
        public Int16[] _Unk_0 = new Int16[6];
        public Int16[] Unk_0 { get { return this._Unk_0; } set { _Unk_0 = value; } }

        public string[] _order = {
            "NoteMask",
            "BendData",
            "StartFretId",
            "EndFretId",
            "Unk_0"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.NoteMask = new Int32[6]; for (int i=0; i<6; i++) this.NoteMask[i] = r.ReadInt32();
            this.BendData = new SngBendData[6]; for (int i = 0; i < 6; i++) { SngBendData obj = new SngBendData(); obj.Read(r); this.BendData[i] = obj; }
            this.StartFretId = r.ReadBytes(6);
            this.EndFretId = r.ReadBytes(6);
            this.Unk_0 = new Int16[6]; for (int i=0; i<6; i++) this.Unk_0[i] = r.ReadInt16();
        }
    }

    public class ChordNotesSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public ChordNotes[] ChordNotes { get; set; }

        public string[] _order = {
            "Count",
            "ChordNotes"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.ChordNotes = new ChordNotes[this.Count]; for (int i=0; i<this.Count; i++) { ChordNotes obj = new ChordNotes(); obj.Read(r); this.ChordNotes[i] = obj; }
        }
    }

    public class SngVocal2014
    {
        public float Time { get; set; }
        public Int32 Note { get; set; }
        public float Length { get; set; }
        public Byte[] _Lyric = new Byte[48];
        public Byte[] Lyric { get { return this._Lyric; } set { _Lyric = value; } }

        public string[] _order = {
            "Time",
            "Note",
            "Length",
            "Lyric"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Time = r.ReadSingle();
            this.Note = r.ReadInt32();
            this.Length = r.ReadSingle();
            this.Lyric = r.ReadBytes(48);
        }
    }

    public class VocalSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngVocal2014[] Vocals { get; set; }

        public string[] _order = {
            "Count",
            "Vocals"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Vocals = new SngVocal2014[this.Count]; for (int i = 0; i < this.Count; i++) { SngVocal2014 obj = new SngVocal2014(); obj.Read(r); this.Vocals[i] = obj; }
        }
    }

    public class SngSymbolsHeader
    {
        public Int32 Unk1 { get; set; }
        public Int32 Unk2 { get; set; }
        public Int32 Unk3 { get; set; }
        public Int32 Unk4 { get; set; }
        public Int32 Unk5 { get; set; }
        public Int32 Unk6 { get; set; }
        public Int32 Unk7 { get; set; }
        public Int32 Unk8 { get; set; }

        public string[] _order = {
            "Unk1",
            "Unk2",
            "Unk3",
            "Unk4",
            "Unk5",
            "Unk6",
            "Unk7",
            "Unk8"
        };

        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Unk1 = r.ReadInt32();
            this.Unk2 = r.ReadInt32();
            this.Unk3 = r.ReadInt32();
            this.Unk4 = r.ReadInt32();
            this.Unk5 = r.ReadInt32();
            this.Unk6 = r.ReadInt32();
            this.Unk7 = r.ReadInt32();
            this.Unk8 = r.ReadInt32();
        }
    }

    public class SymbolsHeaderSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngSymbolsHeader[] SymbolsHeader { get; set; }

        public string[] _order = {
            "Count",
            "SymbolsHeader"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.SymbolsHeader = new SngSymbolsHeader[this.Count]; for (int i = 0; i < this.Count; i++) { SngSymbolsHeader obj = new SngSymbolsHeader(); obj.Read(r); this.SymbolsHeader[i] = obj; }
        }
    }

    public class SngSymbolsTexture
    {
        public Byte[] _Unk = new Byte[144];
        public Byte[] Unk { get { return this._Unk; } set { _Unk = value; } }

        public string[] _order = {
            "Unk"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Unk = r.ReadBytes(144);
        }
    }

    public class SymbolsTextureSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngSymbolsTexture[] SymbolsTextures { get; set; }

        public string[] _order = {
            "Count",
            "SymbolsTextures"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.SymbolsTextures = new SngSymbolsTexture[this.Count]; for (int i = 0; i < this.Count; i++) { SngSymbolsTexture obj = new SngSymbolsTexture(); obj.Read(r); this.SymbolsTextures[i] = obj; }
        }
    }

    public class SngSymbolDefinition
    {
        public Byte[] _Unk = new Byte[44];
        public Byte[] Unk { get { return this._Unk; } set { _Unk = value; } }

        public string[] _order = {
            "Unk"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Unk = r.ReadBytes(44);
        }
    }

    public class SymbolDefinitionSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngSymbolDefinition[] SymbolDefinitions { get; set; }

        public string[] _order = {
            "Count",
            "SymbolDefinitions"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.SymbolDefinitions = new SngSymbolDefinition[this.Count]; for (int i = 0; i < this.Count; i++) { SngSymbolDefinition obj = new SngSymbolDefinition(); obj.Read(r); this.SymbolDefinitions[i] = obj; }
        }
    }

    public class SngPhraseIteration2014
    {
        public Int32 PhraseId { get; set; }
        public float StartTime { get; set; }
        public float NextPhraseTime { get; set; }
        public Int32 Unk3 { get; set; }
        public Int32 Unk4 { get; set; }
        public Int32 Unk5 { get; set; }

        public string[] _order = {
            "PhraseId",
            "StartTime",
            "NextPhraseTime",
            "Unk3",
            "Unk4",
            "Unk5"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.PhraseId = r.ReadInt32();
            this.StartTime = r.ReadSingle();
            this.NextPhraseTime = r.ReadSingle();
            this.Unk3 = r.ReadInt32();
            this.Unk4 = r.ReadInt32();
            this.Unk5 = r.ReadInt32();
        }
    }

    public class PhraseIterationSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngPhraseIteration2014[] PhraseIterations { get; set; }

        public string[] _order = {
            "Count",
            "PhraseIterations"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.PhraseIterations = new SngPhraseIteration2014[this.Count]; for (int i = 0; i < this.Count; i++) { SngPhraseIteration2014 obj = new SngPhraseIteration2014(); obj.Read(r); this.PhraseIterations[i] = obj; }
        }
    }

    public class SngPhraseExtraInfoByLevel
    {
        public Int32 PhraseId { get; set; }
        public Int32 Difficulty { get; set; }
        public Int32 Empty { get; set; }
        public Byte LevelJump { get; set; }
        public Int16 Redundant { get; set; }
        public Byte Padding { get; set; }

        public string[] _order = {
            "PhraseId",
            "Difficulty",
            "Empty",
            "LevelJump",
            "Redundant",
            "Padding"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.PhraseId = r.ReadInt32();
            this.Difficulty = r.ReadInt32();
            this.Empty = r.ReadInt32();
            this.LevelJump = r.ReadByte();
            this.Redundant = r.ReadInt16();
            this.Padding = r.ReadByte();
        }
    }

    public class PhraseExtraInfoByLevelSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngPhraseExtraInfoByLevel[] PhraseExtraInfoByLevel { get; set; }

        public string[] _order = {
            "Count",
            "PhraseExtraInfoByLevel"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.PhraseExtraInfoByLevel = new SngPhraseExtraInfoByLevel[this.Count]; for (int i = 0; i < this.Count; i++) { SngPhraseExtraInfoByLevel obj = new SngPhraseExtraInfoByLevel(); obj.Read(r); this.PhraseExtraInfoByLevel[i] = obj; }
        }
    }

    public class SngNLinkedDifficulty
    {
        public Int32 LevelBreak { get; set; }
        public Int32 PhraseCount { get; set; }
        // count = PhraseCount
        public Int32[] NLD_Phrase { get; set; }

        public string[] _order = {
            "LevelBreak",
            "PhraseCount",
            "NLD_Phrase"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.LevelBreak = r.ReadInt32();
            this.PhraseCount = r.ReadInt32();
            this.NLD_Phrase = new Int32[this.PhraseCount]; for (int i=0; i<this.PhraseCount; i++) this.NLD_Phrase[i] = r.ReadInt32();
        }
    }

    public class NLinkedDifficultySection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngNLinkedDifficulty[] NLinkedDifficulties { get; set; }

        public string[] _order = {
            "Count",
            "NLinkedDifficulties"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.NLinkedDifficulties = new SngNLinkedDifficulty[this.Count]; for (int i = 0; i < this.Count; i++) { SngNLinkedDifficulty obj = new SngNLinkedDifficulty(); obj.Read(r); this.NLinkedDifficulties[i] = obj; }
        }
    }

    public class SngAction
    {
        public float Time { get; set; }
        public Byte[] _ActionName = new Byte[256];
        public Byte[] ActionName { get { return this._ActionName; } set { _ActionName = value; } }

        public string[] _order = {
            "Time",
            "ActionName"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Time = r.ReadSingle();
            this.ActionName = r.ReadBytes(256);
        }
    }

    public class ActionSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngAction[] Actions { get; set; }

        public string[] _order = {
            "Count",
            "Actions"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Actions = new SngAction[this.Count]; for (int i = 0; i < this.Count; i++) { SngAction obj = new SngAction(); obj.Read(r); this.Actions[i] = obj; }
        }
    }

    public class SngEvent
    {
        public float Time { get; set; }
        public Byte[] _EventName = new Byte[256];
        public Byte[] EventName { get { return this._EventName; } set { _EventName = value; } }

        public string[] _order = {
            "Time",
            "EventName"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Time = r.ReadSingle();
            this.EventName = r.ReadBytes(256);
        }
    }

    public class EventSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngEvent[] Events { get; set; }

        public string[] _order = {
            "Count",
            "Events"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Events = new SngEvent[this.Count]; for (int i = 0; i < this.Count; i++) { SngEvent obj = new SngEvent(); obj.Read(r); this.Events[i] = obj; }
        }
    }

    public class SngTone
    {
        public float Time { get; set; }
        public Int32 ToneId { get; set; }

        public string[] _order = {
            "Time",
            "ToneId"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Time = r.ReadSingle();
            this.ToneId = r.ReadInt32();
        }
    }

    public class ToneSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngTone[] Tones { get; set; }

        public string[] _order = {
            "Count",
            "Tones"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Tones = new SngTone[this.Count]; for (int i = 0; i < this.Count; i++) { SngTone obj = new SngTone(); obj.Read(r); this.Tones[i] = obj; }
        }
    }

    public class SngDna
    {
        public float Time { get; set; }
        public Int32 DnaId { get; set; }

        public string[] _order = {
            "Time",
            "DnaId"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Time = r.ReadSingle();
            this.DnaId = r.ReadInt32();
        }
    }

    public class DnaSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngDna[] Dnas { get; set; }

        public string[] _order = {
            "Count",
            "Dnas"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Dnas = new SngDna[this.Count]; for (int i = 0; i < this.Count; i++) { SngDna obj = new SngDna(); obj.Read(r); this.Dnas[i] = obj; }
        }
    }

    public class SngSection
    {
        public Byte[] _Name = new Byte[32];
        public Byte[] Name { get { return this._Name; } set { _Name = value; } }
        public Int32 Number { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public Int32 StartPhraseIterationId { get; set; }
        public Int32 EndPhraseIterationId { get; set; }
        public Int32 Unk12 { get; set; }
        public Int32 Unk13 { get; set; }
        public Int32 Unk14 { get; set; }
        public Int32 Unk15 { get; set; }
        public Int32 Unk16_0 { get; set; }
        public Int32 Unk17_0 { get; set; }
        public Int32 Unk18_0 { get; set; }
        public Int32 Unk19_0 { get; set; }
        public Int32 Unk20_0 { get; set; }

        public string[] _order = {
            "Name",
            "Number",
            "StartTime",
            "EndTime",
            "StartPhraseIterationId",
            "EndPhraseIterationId",
            "Unk12",
            "Unk13",
            "Unk14",
            "Unk15",
            "Unk16_0",
            "Unk17_0",
            "Unk18_0",
            "Unk19_0",
            "Unk20_0"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Name = r.ReadBytes(32);
            this.Number = r.ReadInt32();
            this.StartTime = r.ReadSingle();
            this.EndTime = r.ReadSingle();
            this.StartPhraseIterationId = r.ReadInt32();
            this.EndPhraseIterationId = r.ReadInt32();
            this.Unk12 = r.ReadInt32();
            this.Unk13 = r.ReadInt32();
            this.Unk14 = r.ReadInt32();
            this.Unk15 = r.ReadInt32();
            this.Unk16_0 = r.ReadInt32();
            this.Unk17_0 = r.ReadInt32();
            this.Unk18_0 = r.ReadInt32();
            this.Unk19_0 = r.ReadInt32();
            this.Unk20_0 = r.ReadInt32();
        }
    }

    public class SectionSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngSection[] Sections { get; set; }

        public string[] _order = {
            "Count",
            "Sections"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Sections = new SngSection[this.Count]; for (int i = 0; i < this.Count; i++) { SngSection obj = new SngSection(); obj.Read(r); this.Sections[i] = obj; }
        }
    }

    public class SngAnchor2014
    {
        public float StartBeatTime { get; set; }
        public float EndBeatTime { get; set; }
        public float Unk3_StartBeatTime { get; set; }
        public float Unk4_StartBeatTime { get; set; }
        public Int32 FretId { get; set; }
        public Int32 Width { get; set; }
        public Int32 PhraseIterationId { get; set; }

        public string[] _order = {
            "StartBeatTime",
            "EndBeatTime",
            "Unk3_StartBeatTime",
            "Unk4_StartBeatTime",
            "FretId",
            "Width",
            "PhraseIterationId"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.StartBeatTime = r.ReadSingle();
            this.EndBeatTime = r.ReadSingle();
            this.Unk3_StartBeatTime = r.ReadSingle();
            this.Unk4_StartBeatTime = r.ReadSingle();
            this.FretId = r.ReadInt32();
            this.Width = r.ReadInt32();
            this.PhraseIterationId = r.ReadInt32();
        }
    }

    public class AnchorSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngAnchor2014[] Anchors { get; set; }

        public string[] _order = {
            "Count",
            "Anchors"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Anchors = new SngAnchor2014[this.Count]; for (int i = 0; i < this.Count; i++) { SngAnchor2014 obj = new SngAnchor2014(); obj.Read(r); this.Anchors[i] = obj; }
        }
    }

    public class SngAnchorExtension
    {
        public Int32 BeatTime { get; set; }
        public Byte FretId { get; set; }
        public Int32 Unk2 { get; set; }
        public Int16 Unk3 { get; set; }
        public Byte Unk4 { get; set; }

        public string[] _order = {
            "BeatTime",
            "FretId",
            "Unk2",
            "Unk3",
            "Unk4"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.BeatTime = r.ReadInt32();
            this.FretId = r.ReadByte();
            this.Unk2 = r.ReadInt32();
            this.Unk3 = r.ReadInt16();
            this.Unk4 = r.ReadByte();
        }
    }

    public class AnchorExtensionSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngAnchorExtension[] AnchorExtensions { get; set; }

        public string[] _order = {
            "Count",
            "AnchorExtensions"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.AnchorExtensions = new SngAnchorExtension[this.Count]; for (int i = 0; i < this.Count; i++) { SngAnchorExtension obj = new SngAnchorExtension(); obj.Read(r); this.AnchorExtensions[i] = obj; }
        }
    }

    public class SngFingerprint
    {
        public Int32 ChordId { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float Unk3_StartTime { get; set; }
        public float Unk4_StartTime { get; set; }

        public string[] _order = {
            "ChordId",
            "StartTime",
            "EndTime",
            "Unk3_StartTime",
            "Unk4_StartTime"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.ChordId = r.ReadInt32();
            this.StartTime = r.ReadSingle();
            this.EndTime = r.ReadSingle();
            this.Unk3_StartTime = r.ReadSingle();
            this.Unk4_StartTime = r.ReadSingle();
        }
    }

    public class FingerprintSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngFingerprint[] Fingerprints { get; set; }

        public string[] _order = {
            "Count",
            "Fingerprints"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Fingerprints = new SngFingerprint[this.Count]; for (int i = 0; i < this.Count; i++) { SngFingerprint obj = new SngFingerprint(); obj.Read(r); this.Fingerprints[i] = obj; }
        }
    }

    public class SngNotes
    {
        public Int32[] _NoteMask = new Int32[2];
        public Int32[] NoteMask { get { return this._NoteMask; } set { _NoteMask = value; } }
        public Int32 Unk1 { get; set; }
        public float Time { get; set; }
        public Byte StringIndex { get; set; }
        public Byte[] _FretId = new Byte[2];
        public Byte[] FretId { get { return this._FretId; } set { _FretId = value; } }
        public Byte Unk3_4 { get; set; }
        public Int32 ChordId { get; set; }
        public Int32 ChordNotesId { get; set; }
        public Int32 PhraseId { get; set; }
        public Int32 PhraseIterationId { get; set; }
        public Int16[] _FingerPrintId = new Int16[2];
        public Int16[] FingerPrintId { get { return this._FingerPrintId; } set { _FingerPrintId = value; } }
        public Int16 Unk4 { get; set; }
        public Int16 Unk5 { get; set; }
        public Int16 Unk6 { get; set; }
        public Byte[] _FingerId = new Byte[4];
        public Byte[] FingerId { get { return this._FingerId; } set { _FingerId = value; } }
        public Byte PickDirection { get; set; }
        public Byte Slap { get; set; }
        public Byte Pluck { get; set; }
        public Int16 Vibrato { get; set; }
        public float Sustain { get; set; }
        public float MaxBend { get; set; }
        public BendDataSection BendData { get; set; }

        public string[] _order = {
            "NoteMask",
            "Unk1",
            "Time",
            "StringIndex",
            "FretId",
            "Unk3_4",
            "ChordId",
            "ChordNotesId",
            "PhraseId",
            "PhraseIterationId",
            "FingerPrintId",
            "Unk4",
            "Unk5",
            "Unk6",
            "FingerId",
            "PickDirection",
            "Slap",
            "Pluck",
            "Vibrato",
            "Sustain",
            "MaxBend",
            "BendData"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.NoteMask = new Int32[2]; for (int i=0; i<2; i++) this.NoteMask[i] = r.ReadInt32();
            this.Unk1 = r.ReadInt32();
            this.Time = r.ReadSingle();
            this.StringIndex = r.ReadByte();
            this.FretId = r.ReadBytes(2);
            this.Unk3_4 = r.ReadByte();
            this.ChordId = r.ReadInt32();
            this.ChordNotesId = r.ReadInt32();
            this.PhraseId = r.ReadInt32();
            this.PhraseIterationId = r.ReadInt32();
            this.FingerPrintId = new Int16[2]; for (int i=0; i<2; i++) this.FingerPrintId[i] = r.ReadInt16();
            this.Unk4 = r.ReadInt16();
            this.Unk5 = r.ReadInt16();
            this.Unk6 = r.ReadInt16();
            this.FingerId = r.ReadBytes(4);
            this.PickDirection = r.ReadByte();
            this.Slap = r.ReadByte();
            this.Pluck = r.ReadByte();
            this.Vibrato = r.ReadInt16();
            this.Sustain = r.ReadSingle();
            this.MaxBend = r.ReadSingle();
            this.BendData = new BendDataSection(); this.BendData.Read(r);
        }
    }

    public class NotesSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngNotes[] Notes { get; set; }

        public string[] _order = {
            "Count",
            "Notes"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Notes = new SngNotes[this.Count]; for (int i = 0; i < this.Count; i++) { SngNotes obj = new SngNotes(); obj.Read(r); this.Notes[i] = obj; }
        }
    }

    public class SngArrangement
    {
        public Int32 Difficulty { get; set; }
        public AnchorSection Anchors { get; set; }
        public AnchorExtensionSection AnchorExtensions { get; set; }
        public FingerprintSection Fingerprints1 { get; set; }
        public FingerprintSection Fingerprints2 { get; set; }
        public NotesSection Notes { get; set; }

        public Int32 PhraseCount { get; set; }
        // count = PhraseCount
        public float[] AverageNotesPerIteration { get; set; }
        public Int32 PhraseIterationCount1 { get; set; }
        // count = PhraseIterationCount1
        public Int32[] NotesInIteration1 { get; set; }
        public Int32 PhraseIterationCount2 { get; set; }
        // count = PhraseIterationCount2
        public Int32[] NotesInIteration2 { get; set; }

        public string[] _order = {
            "Difficulty",
            "Anchors",
            "AnchorExtensions",
            "Fingerprints1",
            "Fingerprints2",
            "Notes",
            "PhraseCount",
            "AverageNotesPerIteration",
            "PhraseIterationCount1",
            "NotesInIteration1",
            "PhraseIterationCount2",
            "NotesInIteration2"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Difficulty = r.ReadInt32();
            this.Anchors = new AnchorSection(); this.Anchors.Read(r);
            this.AnchorExtensions = new AnchorExtensionSection(); this.AnchorExtensions.Read(r);
            this.Fingerprints1 = new FingerprintSection(); this.Fingerprints1.Read(r);
            this.Fingerprints2 = new FingerprintSection(); this.Fingerprints2.Read(r);
            this.Notes = new NotesSection(); this.Notes.Read(r);
            this.PhraseCount = r.ReadInt32();
            this.AverageNotesPerIteration = new float[this.PhraseCount]; for (int i=0; i<this.PhraseCount; i++) this.AverageNotesPerIteration[i] = r.ReadSingle();
            this.PhraseIterationCount1 = r.ReadInt32();
            this.NotesInIteration1 = new Int32[this.PhraseIterationCount1]; for (int i=0; i<this.PhraseIterationCount1; i++) this.NotesInIteration1[i] = r.ReadInt32();
            this.PhraseIterationCount2 = r.ReadInt32();
            this.NotesInIteration2 = new Int32[this.PhraseIterationCount2]; for (int i=0; i<this.PhraseIterationCount2; i++) this.NotesInIteration2[i] = r.ReadInt32();
        }
    }

    public class ArrangementSection
    {
        public Int32 Count { get; set; }
        // count = Count
        public SngArrangement[] Arrangements { get; set; }

        public string[] _order = {
            "Count",
            "Arrangements"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.Count = r.ReadInt32();
            this.Arrangements = new SngArrangement[this.Count]; for (int i = 0; i < this.Count; i++) { SngArrangement obj = new SngArrangement(); obj.Read(r); this.Arrangements[i] = obj; }
        }
    }

    public class Metadata2014
    {
        public double MaxScore { get; set; }
        public double MaxNotesAndChords { get; set; }
        public double Unk3_MaxNotesAndChords { get; set; }
        public double PointsPerNote { get; set; }
        public float FirstBeatLength { get; set; }
        public float StartTime { get; set; }
        public Byte CapoFretId { get; set; }
        public Byte[] _LastConversionDateTime = new Byte[32];
        public Byte[] LastConversionDateTime { get { return this._LastConversionDateTime; } set { _LastConversionDateTime = value; } }
        public Int16 Part { get; set; }
        public float SongLength { get; set; }
        public Int32 StringCount { get; set; }
        // count = StringCount
        public Int16[] Tuning { get; set; }
        public float Unk11_FirstSectionStartTime { get; set; }
        public float Unk12_FirstSectionStartTime { get; set; }
        public Int32 MaxDifficulty { get; set; }

        public string[] _order = {
            "MaxScore",
            "MaxNotesAndChords",
            "Unk3_MaxNotesAndChords",
            "PointsPerNote",
            "FirstBeatLength",
            "StartTime",
            "CapoFretId",
            "LastConversionDateTime",
            "Part",
            "SongLength",
            "StringCount",
            "Tuning",
            "Unk11_FirstSectionStartTime",
            "Unk12_FirstSectionStartTime",
            "MaxDifficulty"
        };
        public string[] order { get { return this._order; } }

        public void Read(BinaryReader r) {
            this.MaxScore = r.ReadDouble();
            this.MaxNotesAndChords = r.ReadDouble();
            this.Unk3_MaxNotesAndChords = r.ReadDouble();
            this.PointsPerNote = r.ReadDouble();
            this.FirstBeatLength = r.ReadSingle();
            this.StartTime = r.ReadSingle();
            this.CapoFretId = r.ReadByte();
            this.LastConversionDateTime = r.ReadBytes(32);
            this.Part = r.ReadInt16();
            this.SongLength = r.ReadSingle();
            this.StringCount = r.ReadInt32();
            this.Tuning = new Int16[this.StringCount]; for (int i=0; i<this.StringCount; i++) this.Tuning[i] = r.ReadInt16();
            this.Unk11_FirstSectionStartTime = r.ReadSingle();
            this.Unk12_FirstSectionStartTime = r.ReadSingle();
            this.MaxDifficulty = r.ReadInt32();
        }
    }
}
