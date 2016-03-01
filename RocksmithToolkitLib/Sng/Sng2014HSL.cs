// AUTO-GENERATED FILE, DO NOT MODIFY!
using System;
using System.Diagnostics;
using System.IO;
using MiscUtil.IO;

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Bpm
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
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Time = r.ReadSingle();
            Measure = r.ReadInt16();
            Beat = r.ReadInt16();
            PhraseIteration = r.ReadInt32();
            Mask = r.ReadInt32();
        }
    }
    public class BpmSection
    {
        public Int32 Count { get; set; }
        public Bpm[] BPMs { get; set; }

        public string[] _order = {
			"Count",
			"BPMs"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            try
            {
                Count = r.ReadInt32();
                BPMs = new Bpm[Count];
                for (int i = 0; i < Count; i++)
                {
                    var obj = new Bpm();
                    obj.read(r);
                    BPMs[i] = obj;
                }
            }
            catch (Exception ex)
            {
                // incomplete song information causes exceptions during conversion
                // such as, "End of Stream reached with 4 bytes left to read" 

                throw new Exception("Corrupt CDLC ... Regenerating with Creator GUI may fix it." + Environment.NewLine +
                    "Make sure the song information is complete and correct, including Song Year and Avg Tempo information. (HINT)" + Environment.NewLine +
                    ex.Message + Environment.NewLine + Environment.NewLine);
            }
        }
    }
    public class Phrase
    {
        public Byte Solo { get; set; }
        public Byte Disparity { get; set; }
        public Byte Ignore { get; set; }
        public Byte Padding { get; set; }
        public Int32 MaxDifficulty { get; set; }
        public Int32 PhraseIterationLinks { get; set; }
        public Byte[] _Name = new Byte[32];
        public Byte[] Name { get { return _Name; } set { _Name = value; } }

        public string[] _order = {
			"Solo",
			"Disparity",
			"Ignore",
			"Padding",
			"MaxDifficulty",
			"PhraseIterationLinks",
			"Name"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Solo = r.ReadByte();
            Disparity = r.ReadByte();
            Ignore = r.ReadByte();
            Padding = r.ReadByte();
            MaxDifficulty = r.ReadInt32();
            PhraseIterationLinks = r.ReadInt32();
            Name = r.ReadBytes(32);
        }
    }
    public class PhraseSection
    {
        public Int32 Count { get; set; }
        public Phrase[] Phrases { get; set; }

        public string[] _order = {
			"Count",
			"Phrases"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Phrases = new Phrase[Count]; for (int i = 0; i < Count; i++) { var obj = new Phrase(); obj.read(r); Phrases[i] = obj; }
        }
    }
    public class Chord
    {
        public UInt32 Mask { get; set; }
        public Byte[] _Frets = new Byte[6];
        public Byte[] Frets { get { return _Frets; } set { _Frets = value; } }
        public Byte[] _Fingers = new Byte[6];
        public Byte[] Fingers { get { return _Fingers; } set { _Fingers = value; } }
        public Int32[] _Notes = new Int32[6];
        public Int32[] Notes { get { return _Notes; } set { _Notes = value; } }
        public Byte[] _Name = new Byte[32];
        public Byte[] Name { get { return _Name; } set { _Name = value; } }

        public string[] _order = {
			"Mask",
			"Frets",
			"Fingers",
			"Notes",
			"Name"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Mask = r.ReadUInt32();
            Frets = r.ReadBytes(6);
            Fingers = r.ReadBytes(6);
            Notes = new Int32[6]; for (int i = 0; i < 6; i++) Notes[i] = r.ReadInt32();
            Name = r.ReadBytes(32);
        }
    }
    public class ChordSection
    {
        public Int32 Count { get; set; }
        public Chord[] Chords { get; set; }

        public string[] _order = {
			"Count",
			"Chords"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Chords = new Chord[Count]; for (int i = 0; i < Count; i++) { var obj = new Chord(); obj.read(r); Chords[i] = obj; }
        }
    }
    public class BendData32
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
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Time = r.ReadSingle();
            Step = r.ReadSingle();
            Unk3_0 = r.ReadInt16();
            Unk4_0 = r.ReadByte();
            Unk5 = r.ReadByte();
        }
    }
    public class BendData
    {
        public BendData32[] _BendData32 = new BendData32[32];
        public BendData32[] BendData32 { get { return _BendData32; } set { _BendData32 = value; } }
        public Int32 UsedCount { get; set; }

        public string[] _order = {
			"BendData32",
			"UsedCount"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            BendData32 = new BendData32[32]; for (int i = 0; i < 32; i++) { var obj = new BendData32(); obj.read(r); BendData32[i] = obj; }
            UsedCount = r.ReadInt32();
        }
    }
    public class BendDataSection
    {
        public Int32 Count { get; set; }
        public BendData32[] BendData { get; set; }

        public string[] _order = {
			"Count",
			"BendData"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            BendData = new BendData32[Count]; for (int i = 0; i < Count; i++) { var obj = new BendData32(); obj.read(r); BendData[i] = obj; }
        }
    }
    public class ChordNotes
    {
        public UInt32[] _NoteMask = new UInt32[6];
        public UInt32[] NoteMask { get { return _NoteMask; } set { _NoteMask = value; } }
        public BendData[] _BendData = new BendData[6];
        public BendData[] BendData { get { return _BendData; } set { _BendData = value; } }
        public Byte[] _SlideTo = new Byte[6];
        public Byte[] SlideTo { get { return _SlideTo; } set { _SlideTo = value; } }
        public Byte[] _SlideUnpitchTo = new Byte[6];
        public Byte[] SlideUnpitchTo { get { return _SlideUnpitchTo; } set { _SlideUnpitchTo = value; } }
        public Int16[] _Vibrato = new Int16[6];
        public Int16[] Vibrato { get { return _Vibrato; } set { _Vibrato = value; } }

        public string[] _order = {
			"NoteMask",
			"BendData",
			"SlideTo",
			"SlideUnpitchTo",
			"Vibrato"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            NoteMask = new UInt32[6]; for (int i = 0; i < 6; i++) NoteMask[i] = r.ReadUInt32();
            BendData = new BendData[6]; for (int i = 0; i < 6; i++) { var obj = new BendData(); obj.read(r); BendData[i] = obj; }
            SlideTo = r.ReadBytes(6);
            SlideUnpitchTo = r.ReadBytes(6);
            Vibrato = new Int16[6]; for (int i = 0; i < 6; i++) Vibrato[i] = r.ReadInt16();
        }
    }
    public class ChordNotesSection
    {
        public Int32 Count { get; set; }
        public ChordNotes[] ChordNotes { get; set; }

        public string[] _order = {
			"Count",
			"ChordNotes"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            ChordNotes = new ChordNotes[Count]; for (int i = 0; i < Count; i++) { var obj = new ChordNotes(); obj.read(r); ChordNotes[i] = obj; }
        }
    }
    public class Vocal
    {
        public float Time { get; set; }
        public Int32 Note { get; set; }
        public float Length { get; set; }
        public Byte[] _Lyric = new Byte[48];
        public Byte[] Lyric { get { return _Lyric; } set { _Lyric = value; } }

        public string[] _order = {
			"Time",
			"Note",
			"Length",
			"Lyric"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Time = r.ReadSingle();
            Note = r.ReadInt32();
            Length = r.ReadSingle();
            Lyric = r.ReadBytes(48);
        }
    }
    public class VocalSection
    {
        public Int32 Count { get; set; }
        public Vocal[] Vocals { get; set; }

        public string[] _order = {
			"Count",
			"Vocals"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Vocals = new Vocal[Count]; for (int i = 0; i < Count; i++) { var obj = new Vocal(); obj.read(r); Vocals[i] = obj; }
        }
    }
    public class SymbolsHeader
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
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Unk1 = r.ReadInt32();
            Unk2 = r.ReadInt32();
            Unk3 = r.ReadInt32();
            Unk4 = r.ReadInt32();
            Unk5 = r.ReadInt32();
            Unk6 = r.ReadInt32();
            Unk7 = r.ReadInt32();
            Unk8 = r.ReadInt32();
        }
    }
    public class SymbolsHeaderSection
    {
        public Int32 Count { get; set; }
        public SymbolsHeader[] SymbolsHeader { get; set; }

        public string[] _order = {
			"Count",
			"SymbolsHeader"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            SymbolsHeader = new SymbolsHeader[Count]; for (int i = 0; i < Count; i++) { var obj = new SymbolsHeader(); obj.read(r); SymbolsHeader[i] = obj; }
        }
    }
    public class SymbolsTexture
    {
        public Byte[] _Font = new Byte[128];
        public Byte[] Font { get { return _Font; } set { _Font = value; } }
        public Int32 FontpathLength { get; set; }
        public Int32 Unk1_0 { get; set; }
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }

        public string[] _order = {
			"Font",
			"FontpathLength",
			"Unk1_0",
			"Width",
			"Height"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Font = r.ReadBytes(128);
            FontpathLength = r.ReadInt32();
            Unk1_0 = r.ReadInt32();
            Width = r.ReadInt32();
            Height = r.ReadInt32();
        }
    }
    public class SymbolsTextureSection
    {
        public Int32 Count { get; set; }
        public SymbolsTexture[] SymbolsTextures { get; set; }

        public string[] _order = {
			"Count",
			"SymbolsTextures"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            SymbolsTextures = new SymbolsTexture[Count]; for (int i = 0; i < Count; i++) { var obj = new SymbolsTexture(); obj.read(r); SymbolsTextures[i] = obj; }
        }
    }
    public class Rect
    {
        public float yMin { get; set; }
        public float xMin { get; set; }
        public float yMax { get; set; }
        public float xMax { get; set; }

        public string[] _order = {
			"yMin",
			"xMin",
			"yMax",
			"xMax"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            yMin = r.ReadSingle();
            xMin = r.ReadSingle();
            yMax = r.ReadSingle();
            xMax = r.ReadSingle();
        }
    }
    public class SymbolDefinition
    {
        public Byte[] _Text = new Byte[12];
        public Byte[] Text { get { return _Text; } set { _Text = value; } }
        public Rect Rect_Outter { get; set; }
        public Rect Rect_Inner { get; set; }

        public string[] _order = {
			"Text",
			"Rect_Outter",
			"Rect_Inner"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Text = r.ReadBytes(12);
            Rect_Outter = new Rect(); Rect_Outter.read(r);
            Rect_Inner = new Rect(); Rect_Inner.read(r);
        }
    }
    public class SymbolDefinitionSection
    {
        public Int32 Count { get; set; }
        public SymbolDefinition[] SymbolDefinitions { get; set; }

        public string[] _order = {
			"Count",
			"SymbolDefinitions"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            SymbolDefinitions = new SymbolDefinition[Count]; for (int i = 0; i < Count; i++) { var obj = new SymbolDefinition(); obj.read(r); SymbolDefinitions[i] = obj; }
        }
    }
    public class PhraseIteration
    {
        public Int32 PhraseId { get; set; }
        public float StartTime { get; set; }
        public float NextPhraseTime { get; set; }
        public Int32[] _Difficulty = new Int32[3];
        public Int32[] Difficulty { get { return _Difficulty; } set { _Difficulty = value; } }

        public string[] _order = {
			"PhraseId",
			"StartTime",
			"NextPhraseTime",
			"Difficulty"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            PhraseId = r.ReadInt32();
            StartTime = r.ReadSingle();
            NextPhraseTime = r.ReadSingle();
            Difficulty = new Int32[3]; for (int i = 0; i < 3; i++) Difficulty[i] = r.ReadInt32();
        }
    }
    public class PhraseIterationSection
    {
        public Int32 Count { get; set; }
        public PhraseIteration[] PhraseIterations { get; set; }

        public string[] _order = {
			"Count",
			"PhraseIterations"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            PhraseIterations = new PhraseIteration[Count]; for (int i = 0; i < Count; i++) { var obj = new PhraseIteration(); obj.read(r); PhraseIterations[i] = obj; }
        }
    }
    public class PhraseExtraInfoByLevel
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
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            PhraseId = r.ReadInt32();
            Difficulty = r.ReadInt32();
            Empty = r.ReadInt32();
            LevelJump = r.ReadByte();
            Redundant = r.ReadInt16();
            Padding = r.ReadByte();
        }
    }
    public class PhraseExtraInfoByLevelSection
    {
        public Int32 Count { get; set; }
        public PhraseExtraInfoByLevel[] PhraseExtraInfoByLevel { get; set; }

        public string[] _order = {
			"Count",
			"PhraseExtraInfoByLevel"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            PhraseExtraInfoByLevel = new PhraseExtraInfoByLevel[Count]; for (int i = 0; i < Count; i++) { var obj = new PhraseExtraInfoByLevel(); obj.read(r); PhraseExtraInfoByLevel[i] = obj; }
        }
    }
    public class NLinkedDifficulty
    {
        public Int32 LevelBreak { get; set; }
        public Int32 PhraseCount { get; set; }
        public Int32[] NLD_Phrase { get; set; }

        public string[] _order = {
			"LevelBreak",
			"PhraseCount",
			"NLD_Phrase"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            LevelBreak = r.ReadInt32();
            PhraseCount = r.ReadInt32();
            NLD_Phrase = new Int32[PhraseCount]; for (int i = 0; i < PhraseCount; i++) NLD_Phrase[i] = r.ReadInt32();
        }
    }
    public class NLinkedDifficultySection
    {
        public Int32 Count { get; set; }
        public NLinkedDifficulty[] NLinkedDifficulties { get; set; }

        public string[] _order = {
			"Count",
			"NLinkedDifficulties"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            NLinkedDifficulties = new NLinkedDifficulty[Count]; for (int i = 0; i < Count; i++) { var obj = new NLinkedDifficulty(); obj.read(r); NLinkedDifficulties[i] = obj; }
        }
    }
    public class Action
    {
        public float Time { get; set; }
        public Byte[] _ActionName = new Byte[256];
        public Byte[] ActionName { get { return _ActionName; } set { _ActionName = value; } }

        public string[] _order = {
			"Time",
			"ActionName"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Time = r.ReadSingle();
            ActionName = r.ReadBytes(256);
        }
    }
    public class ActionSection
    {
        public Int32 Count { get; set; }
        public Action[] Actions { get; set; }

        public string[] _order = {
			"Count",
			"Actions"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Actions = new Action[Count]; for (int i = 0; i < Count; i++) { var obj = new Action(); obj.read(r); Actions[i] = obj; }
        }
    }
    public class Event
    {
        public float Time { get; set; }
        public Byte[] _EventName = new Byte[256];
        public Byte[] EventName { get { return _EventName; } set { _EventName = value; } }

        public string[] _order = {
			"Time",
			"EventName"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Time = r.ReadSingle();
            EventName = r.ReadBytes(256);
        }
    }
    public class EventSection
    {
        public Int32 Count { get; set; }
        public Event[] Events { get; set; }

        public string[] _order = {
			"Count",
			"Events"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Events = new Event[Count]; for (int i = 0; i < Count; i++) { var obj = new Event(); obj.read(r); Events[i] = obj; }
        }
    }
    public class Tone
    {
        public float Time { get; set; }
        public Int32 ToneId { get; set; }

        public string[] _order = {
			"Time",
			"ToneId"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Time = r.ReadSingle();
            ToneId = r.ReadInt32();
        }
    }
    public class ToneSection
    {
        public Int32 Count { get; set; }
        public Tone[] Tones { get; set; }

        public string[] _order = {
			"Count",
			"Tones"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Tones = new Tone[Count]; for (int i = 0; i < Count; i++) { var obj = new Tone(); obj.read(r); Tones[i] = obj; }
        }
    }
    public class Dna
    {
        public float Time { get; set; }
        public Int32 DnaId { get; set; }

        public string[] _order = {
			"Time",
			"DnaId"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Time = r.ReadSingle();
            DnaId = r.ReadInt32();
        }
    }
    public class DnaSection
    {
        public Int32 Count { get; set; }
        public Dna[] Dnas { get; set; }

        public string[] _order = {
			"Count",
			"Dnas"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Dnas = new Dna[Count]; for (int i = 0; i < Count; i++) { var obj = new Dna(); obj.read(r); Dnas[i] = obj; }
        }
    }
    public class Section
    {
        public Byte[] _Name = new Byte[32];
        public Byte[] Name { get { return _Name; } set { _Name = value; } }
        public Int32 Number { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public Int32 StartPhraseIterationId { get; set; }
        public Int32 EndPhraseIterationId { get; set; }
        public Byte[] _StringMask = new Byte[36];
        public Byte[] StringMask { get { return _StringMask; } set { _StringMask = value; } }

        public string[] _order = {
			"Name",
			"Number",
			"StartTime",
			"EndTime",
			"StartPhraseIterationId",
			"EndPhraseIterationId",
			"StringMask"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Name = r.ReadBytes(32);
            Number = r.ReadInt32();
            StartTime = r.ReadSingle();
            EndTime = r.ReadSingle();
            StartPhraseIterationId = r.ReadInt32();
            EndPhraseIterationId = r.ReadInt32();
            StringMask = r.ReadBytes(36);
        }
    }
    public class SectionSection
    {
        public Int32 Count { get; set; }
        public Section[] Sections { get; set; }

        public string[] _order = {
			"Count",
			"Sections"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Sections = new Section[Count]; for (int i = 0; i < Count; i++) { var obj = new Section(); obj.read(r); Sections[i] = obj; }
        }
    }
    public class Anchor
    {
        public float StartBeatTime { get; set; }
        public float EndBeatTime { get; set; }
        public float Unk3_FirstNoteTime { get; set; }
        public float Unk4_LastNoteTime { get; set; }
        public Byte FretId { get; set; }
        public Byte[] _Padding = new Byte[3];
        public Byte[] Padding { get { return _Padding; } set { _Padding = value; } }
        public Int32 Width { get; set; }
        public Int32 PhraseIterationId { get; set; }

        public string[] _order = {
			"StartBeatTime",
			"EndBeatTime",
			"Unk3_FirstNoteTime",
			"Unk4_LastNoteTime",
			"FretId",
			"Padding",
			"Width",
			"PhraseIterationId"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            StartBeatTime = r.ReadSingle();
            EndBeatTime = r.ReadSingle();
            Unk3_FirstNoteTime = r.ReadSingle();
            Unk4_LastNoteTime = r.ReadSingle();
            FretId = r.ReadByte();
            Padding = r.ReadBytes(3);
            Width = r.ReadInt32();
            PhraseIterationId = r.ReadInt32();
        }
    }
    public class AnchorSection
    {
        public Int32 Count { get; set; }
        public Anchor[] Anchors { get; set; }

        public string[] _order = {
			"Count",
			"Anchors"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Anchors = new Anchor[Count]; for (int i = 0; i < Count; i++) { var obj = new Anchor(); obj.read(r); Anchors[i] = obj; }
        }
    }
    public class AnchorExtension
    {
        public float BeatTime { get; set; }
        public Byte FretId { get; set; }
        public Int32 Unk2_0 { get; set; }
        public Int16 Unk3_0 { get; set; }
        public Byte Unk4_0 { get; set; }

        public string[] _order = {
			"BeatTime",
			"FretId",
			"Unk2_0",
			"Unk3_0",
			"Unk4_0"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            BeatTime = r.ReadSingle();
            FretId = r.ReadByte();
            Unk2_0 = r.ReadInt32();
            Unk3_0 = r.ReadInt16();
            Unk4_0 = r.ReadByte();
        }
    }
    public class AnchorExtensionSection
    {
        public Int32 Count { get; set; }
        public AnchorExtension[] AnchorExtensions { get; set; }

        public string[] _order = {
			"Count",
			"AnchorExtensions"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            AnchorExtensions = new AnchorExtension[Count]; for (int i = 0; i < Count; i++) { var obj = new AnchorExtension(); obj.read(r); AnchorExtensions[i] = obj; }
        }
    }
    public class Fingerprint
    {
        public Int32 ChordId { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float Unk3_FirstNoteTime { get; set; }
        public float Unk4_LastNoteTime { get; set; }

        public string[] _order = {
			"ChordId",
			"StartTime",
			"EndTime",
			"Unk3_FirstNoteTime",
			"Unk4_LastNoteTime"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            ChordId = r.ReadInt32();
            StartTime = r.ReadSingle();
            EndTime = r.ReadSingle();
            Unk3_FirstNoteTime = r.ReadSingle();
            Unk4_LastNoteTime = r.ReadSingle();
        }
    }
    public class FingerprintSection
    {
        public Int32 Count { get; set; }
        public Fingerprint[] Fingerprints { get; set; }

        public string[] _order = {
			"Count",
			"Fingerprints"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Fingerprints = new Fingerprint[Count]; for (int i = 0; i < Count; i++) { var obj = new Fingerprint(); obj.read(r); Fingerprints[i] = obj; }
        }
    }
    public class Notes
    {
        public UInt32 NoteMask { get; set; }
        public UInt32 NoteFlags { get; set; }
        public UInt32 Hash { get; set; }
        public float Time { get; set; }
        public Byte StringIndex { get; set; }
        public Byte FretId { get; set; }
        public Byte AnchorFretId { get; set; }
        public Byte AnchorWidth { get; set; }
        public Int32 ChordId { get; set; }
        public Int32 ChordNotesId { get; set; }
        public Int32 PhraseId { get; set; }
        public Int32 PhraseIterationId { get; set; }
        public Int16[] _FingerPrintId = new Int16[2];
        public Int16[] FingerPrintId { get { return _FingerPrintId; } set { _FingerPrintId = value; } }
        public Int16 NextIterNote { get; set; }
        public Int16 PrevIterNote { get; set; }
        public Int16 ParentPrevNote { get; set; }
        public Byte SlideTo { get; set; }
        public Byte SlideUnpitchTo { get; set; }
        public Byte LeftHand { get; set; }
        public Byte Tap { get; set; }
        public Byte PickDirection { get; set; }
        public Byte Slap { get; set; }
        public Byte Pluck { get; set; }
        public Int16 Vibrato { get; set; }
        public float Sustain { get; set; }
        public float MaxBend { get; set; }
        public BendDataSection BendData { get; set; }

        public string[] _order = {
			"NoteMask",
			"NoteFlags",
			"Hash",
			"Time",
			"StringIndex",
			"FretId",
			"AnchorFretId",
			"AnchorWidth",
			"ChordId",
			"ChordNotesId",
			"PhraseId",
			"PhraseIterationId",
			"FingerPrintId",
			"NextIterNote",
			"PrevIterNote",
			"ParentPrevNote",
			"SlideTo",
			"SlideUnpitchTo",
			"LeftHand",
			"Tap",
			"PickDirection",
			"Slap",
			"Pluck",
			"Vibrato",
			"Sustain",
			"MaxBend",
			"BendData"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            NoteMask = r.ReadUInt32();
            NoteFlags = r.ReadUInt32();
            Hash = r.ReadUInt32();
            Time = r.ReadSingle();
            StringIndex = r.ReadByte();
            FretId = r.ReadByte();
            AnchorFretId = r.ReadByte();
            AnchorWidth = r.ReadByte();
            ChordId = r.ReadInt32();
            ChordNotesId = r.ReadInt32();
            PhraseId = r.ReadInt32();
            PhraseIterationId = r.ReadInt32();
            FingerPrintId = new Int16[2]; for (int i = 0; i < 2; i++) FingerPrintId[i] = r.ReadInt16();
            NextIterNote = r.ReadInt16();
            PrevIterNote = r.ReadInt16();
            ParentPrevNote = r.ReadInt16();
            SlideTo = r.ReadByte();
            SlideUnpitchTo = r.ReadByte();
            LeftHand = r.ReadByte();
            Tap = r.ReadByte();
            PickDirection = r.ReadByte();
            Slap = r.ReadByte();
            Pluck = r.ReadByte();
            Vibrato = r.ReadInt16();
            Sustain = r.ReadSingle();
            MaxBend = r.ReadSingle();
            BendData = new BendDataSection(); BendData.read(r);
        }
    }
    public class NotesSection
    {
        public Int32 Count { get; set; }
        public Notes[] Notes { get; set; }

        public string[] _order = {
			"Count",
			"Notes"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Notes = new Notes[Count]; for (int i = 0; i < Count; i++) { var obj = new Notes(); obj.read(r); Notes[i] = obj; }
        }
    }
    public class Arrangement
    {
        public Int32 Difficulty { get; set; }
        public AnchorSection Anchors { get; set; }
        public AnchorExtensionSection AnchorExtensions { get; set; }
        public FingerprintSection Fingerprints1 { get; set; }
        public FingerprintSection Fingerprints2 { get; set; }
        public NotesSection Notes { get; set; }
        public Int32 PhraseCount { get; set; }
        public float[] AverageNotesPerIteration { get; set; }
        public Int32 PhraseIterationCount1 { get; set; }
        public Int32[] NotesInIteration1 { get; set; }
        public Int32 PhraseIterationCount2 { get; set; }
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
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Difficulty = r.ReadInt32();
            Anchors = new AnchorSection(); Anchors.read(r);
            AnchorExtensions = new AnchorExtensionSection(); AnchorExtensions.read(r);
            Fingerprints1 = new FingerprintSection(); Fingerprints1.read(r);
            Fingerprints2 = new FingerprintSection(); Fingerprints2.read(r);
            Notes = new NotesSection(); Notes.read(r);
            PhraseCount = r.ReadInt32();
            AverageNotesPerIteration = new float[PhraseCount]; for (int i = 0; i < PhraseCount; i++) AverageNotesPerIteration[i] = r.ReadSingle();
            PhraseIterationCount1 = r.ReadInt32();
            NotesInIteration1 = new Int32[PhraseIterationCount1]; for (int i = 0; i < PhraseIterationCount1; i++) NotesInIteration1[i] = r.ReadInt32();
            PhraseIterationCount2 = r.ReadInt32();
            NotesInIteration2 = new Int32[PhraseIterationCount2]; for (int i = 0; i < PhraseIterationCount2; i++) NotesInIteration2[i] = r.ReadInt32();
        }
    }
    public class ArrangementSection
    {
        public Int32 Count { get; set; }
        public Arrangement[] Arrangements { get; set; }

        public string[] _order = {
			"Count",
			"Arrangements"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            Count = r.ReadInt32();
            Arrangements = new Arrangement[Count]; for (int i = 0; i < Count; i++) { var obj = new Arrangement(); obj.read(r); Arrangements[i] = obj; }
        }
    }
    public class Metadata
    {
        public double MaxScore { get; set; }
        public double MaxNotesAndChords { get; set; }
        public double MaxNotesAndChords_Real { get; set; }
        public double PointsPerNote { get; set; }
        public float FirstBeatLength { get; set; }
        public float StartTime { get; set; }
        public Byte CapoFretId { get; set; }
        public Byte[] _LastConversionDateTime = new Byte[32];
        public Byte[] LastConversionDateTime { get { return _LastConversionDateTime; } set { _LastConversionDateTime = value; } }
        public Int16 Part { get; set; }
        public float SongLength { get; set; }
        public Int32 StringCount { get; set; }
        public Int16[] Tuning { get; set; }
        public float Unk11_FirstNoteTime { get; set; }
        public float Unk12_FirstNoteTime { get; set; }
        public Int32 MaxDifficulty { get; set; }

        public string[] _order = {
			"MaxScore",
			"MaxNotesAndChords",
			"MaxNotesAndChords_Real",
			"PointsPerNote",
			"FirstBeatLength",
			"StartTime",
			"CapoFretId",
			"LastConversionDateTime",
			"Part",
			"SongLength",
			"StringCount",
			"Tuning",
			"Unk11_FirstNoteTime",
			"Unk12_FirstNoteTime",
			"MaxDifficulty"
		};
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            MaxScore = r.ReadDouble();
            MaxNotesAndChords = r.ReadDouble();
            MaxNotesAndChords_Real = r.ReadDouble();
            PointsPerNote = r.ReadDouble();
            FirstBeatLength = r.ReadSingle();
            StartTime = r.ReadSingle();
            CapoFretId = r.ReadByte();
            LastConversionDateTime = r.ReadBytes(32);
            Part = r.ReadInt16();
            SongLength = r.ReadSingle();
            StringCount = r.ReadInt32();
            Tuning = new Int16[StringCount]; for (int i = 0; i < StringCount; i++) Tuning[i] = r.ReadInt16();
            Unk11_FirstNoteTime = r.ReadSingle();
            Unk12_FirstNoteTime = r.ReadSingle();
            MaxDifficulty = r.ReadInt32();
        }
    }
    public class Sng
    {
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
        public Metadata Metadata { get; set; }

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
        public string[] order { get { return _order; } }
        public void read(EndianBinaryReader r)
        {
            BPMs = new BpmSection(); BPMs.read(r);
            Phrases = new PhraseSection(); Phrases.read(r);
            Chords = new ChordSection(); Chords.read(r);
            ChordNotes = new ChordNotesSection(); ChordNotes.read(r);
            Vocals = new VocalSection(); Vocals.read(r);
            SymbolsHeader = new SymbolsHeaderSection(); SymbolsHeader.read(r);
            SymbolsTexture = new SymbolsTextureSection(); SymbolsTexture.read(r);
            SymbolsDefinition = new SymbolDefinitionSection(); SymbolsDefinition.read(r);
            PhraseIterations = new PhraseIterationSection(); PhraseIterations.read(r);
            PhraseExtraInfo = new PhraseExtraInfoByLevelSection(); PhraseExtraInfo.read(r);
            NLD = new NLinkedDifficultySection(); NLD.read(r);
            Actions = new ActionSection(); Actions.read(r);
            Events = new EventSection(); Events.read(r);
            Tones = new ToneSection(); Tones.read(r);
            DNAs = new DnaSection(); DNAs.read(r);
            Sections = new SectionSection(); Sections.read(r);
            Arrangements = new ArrangementSection(); Arrangements.read(r);
            Metadata = new Metadata(); Metadata.read(r);
        }
    }
}
