// AUTO-GENERATED FILE, DO NOT MODIFY!
using System;
using System.IO;

namespace RocksmithToolkitLib.Sng2014HSL {
	public class Bpm {
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
		public void read(BinaryReader r) {
			this.Time = r.ReadSingle();
			this.Measure = r.ReadInt16();
			this.Beat = r.ReadInt16();
			this.PhraseIteration = r.ReadInt32();
			this.Mask = r.ReadInt32();
		}
	}
	public class BpmSection {
		public Int32 Count { get; set; }
		public Bpm[] BPMs { get; set; }

		public string[] _order = {
			"Count",
			"BPMs"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.BPMs = new Bpm[this.Count]; for (int i=0; i<this.Count; i++) { Bpm obj = new Bpm(); obj.read(r); this.BPMs[i] = obj; }
		}
	}
	public class Phrase {
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
		public void read(BinaryReader r) {
			this.Solo = r.ReadByte();
			this.Disparity = r.ReadByte();
			this.Ignore = r.ReadByte();
			this.Padding = r.ReadByte();
			this.MaxDifficulty = r.ReadInt32();
			this.PhraseIterationLinks = r.ReadInt32();
			this.Name = r.ReadBytes(32);
		}
	}
	public class PhraseSection {
		public Int32 Count { get; set; }
		public Phrase[] Phrases { get; set; }

		public string[] _order = {
			"Count",
			"Phrases"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Phrases = new Phrase[this.Count]; for (int i=0; i<this.Count; i++) { Phrase obj = new Phrase(); obj.read(r); this.Phrases[i] = obj; }
		}
	}
	public class Chord {
		public UInt32 Mask { get; set; }
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
		public void read(BinaryReader r) {
			this.Mask = r.ReadUInt32();
			this.Frets = r.ReadBytes(6);
			this.Fingers = r.ReadBytes(6);
			this.Notes = new Int32[6]; for (int i=0; i<6; i++) this.Notes[i] = r.ReadInt32();
			this.Name = r.ReadBytes(32);
		}
	}
	public class ChordSection {
		public Int32 Count { get; set; }
		public Chord[] Chords { get; set; }

		public string[] _order = {
			"Count",
			"Chords"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Chords = new Chord[this.Count]; for (int i=0; i<this.Count; i++) { Chord obj = new Chord(); obj.read(r); this.Chords[i] = obj; }
		}
	}
	public class BendData32 {
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
		public void read(BinaryReader r) {
			this.Time = r.ReadSingle();
			this.Step = r.ReadSingle();
			this.Unk3_0 = r.ReadInt16();
			this.Unk4_0 = r.ReadByte();
			this.Unk5 = r.ReadByte();
		}
	}
	public class BendData {
		public BendData32[] _BendData32 = new BendData32[32];
		public BendData32[] BendData32 { get { return this._BendData32; } set { _BendData32 = value; } }
		public Int32 UsedCount { get; set; }

		public string[] _order = {
			"BendData32",
			"UsedCount"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.BendData32 = new BendData32[32]; for (int i=0; i<32; i++) { BendData32 obj = new BendData32(); obj.read(r); this.BendData32[i] = obj; }
			this.UsedCount = r.ReadInt32();
		}
	}
	public class BendDataSection {
		public Int32 Count { get; set; }
		public BendData32[] BendData { get; set; }

		public string[] _order = {
			"Count",
			"BendData"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.BendData = new BendData32[this.Count]; for (int i=0; i<this.Count; i++) { BendData32 obj = new BendData32(); obj.read(r); this.BendData[i] = obj; }
		}
	}
	public class ChordNotes {
		public UInt32[] _NoteMask = new UInt32[6];
		public UInt32[] NoteMask { get { return this._NoteMask; } set { _NoteMask = value; } }
		public BendData[] _BendData = new BendData[6];
		public BendData[] BendData { get { return this._BendData; } set { _BendData = value; } }
		public Byte[] _SlideTo = new Byte[6];
		public Byte[] SlideTo { get { return this._SlideTo; } set { _SlideTo = value; } }
		public Byte[] _SlideUnpitchTo = new Byte[6];
		public Byte[] SlideUnpitchTo { get { return this._SlideUnpitchTo; } set { _SlideUnpitchTo = value; } }
		public Int16[] _Vibrato = new Int16[6];
		public Int16[] Vibrato { get { return this._Vibrato; } set { _Vibrato = value; } }

		public string[] _order = {
			"NoteMask",
			"BendData",
			"SlideTo",
			"SlideUnpitchTo",
			"Vibrato"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.NoteMask = new UInt32[6]; for (int i=0; i<6; i++) this.NoteMask[i] = r.ReadUInt32();
			this.BendData = new BendData[6]; for (int i=0; i<6; i++) { BendData obj = new BendData(); obj.read(r); this.BendData[i] = obj; }
			this.SlideTo = r.ReadBytes(6);
			this.SlideUnpitchTo = r.ReadBytes(6);
			this.Vibrato = new Int16[6]; for (int i=0; i<6; i++) this.Vibrato[i] = r.ReadInt16();
		}
	}
	public class ChordNotesSection {
		public Int32 Count { get; set; }
		public ChordNotes[] ChordNotes { get; set; }

		public string[] _order = {
			"Count",
			"ChordNotes"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.ChordNotes = new ChordNotes[this.Count]; for (int i=0; i<this.Count; i++) { ChordNotes obj = new ChordNotes(); obj.read(r); this.ChordNotes[i] = obj; }
		}
	}
	public class Vocal {
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
		public void read(BinaryReader r) {
			this.Time = r.ReadSingle();
			this.Note = r.ReadInt32();
			this.Length = r.ReadSingle();
			this.Lyric = r.ReadBytes(48);
		}
	}
	public class VocalSection {
		public Int32 Count { get; set; }
		public Vocal[] Vocals { get; set; }

		public string[] _order = {
			"Count",
			"Vocals"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Vocals = new Vocal[this.Count]; for (int i=0; i<this.Count; i++) { Vocal obj = new Vocal(); obj.read(r); this.Vocals[i] = obj; }
		}
	}
	public class SymbolsHeader {
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
		public void read(BinaryReader r) {
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
	public class SymbolsHeaderSection {
		public Int32 Count { get; set; }
		public SymbolsHeader[] SymbolsHeader { get; set; }

		public string[] _order = {
			"Count",
			"SymbolsHeader"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.SymbolsHeader = new SymbolsHeader[this.Count]; for (int i=0; i<this.Count; i++) { SymbolsHeader obj = new SymbolsHeader(); obj.read(r); this.SymbolsHeader[i] = obj; }
		}
	}
	public class SymbolsTexture {
		public Byte[] _Font = new Byte[128];
		public Byte[] Font { get { return this._Font; } set { _Font = value; } }
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Font = r.ReadBytes(128);
			this.FontpathLength = r.ReadInt32();
			this.Unk1_0 = r.ReadInt32();
			this.Width = r.ReadInt32();
			this.Height = r.ReadInt32();
		}
	}
	public class SymbolsTextureSection {
		public Int32 Count { get; set; }
		public SymbolsTexture[] SymbolsTextures { get; set; }

		public string[] _order = {
			"Count",
			"SymbolsTextures"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.SymbolsTextures = new SymbolsTexture[this.Count]; for (int i=0; i<this.Count; i++) { SymbolsTexture obj = new SymbolsTexture(); obj.read(r); this.SymbolsTextures[i] = obj; }
		}
	}
	public class Rect {
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.yMin = r.ReadSingle();
			this.xMin = r.ReadSingle();
			this.yMax = r.ReadSingle();
			this.xMax = r.ReadSingle();
		}
	}
	public class SymbolDefinition {
		public Byte[] _Text = new Byte[12];
		public Byte[] Text { get { return this._Text; } set { _Text = value; } }
		public Rect Rect_Outter { get; set; }
		public Rect Rect_Inner { get; set; }

		public string[] _order = {
			"Text",
			"Rect_Outter",
			"Rect_Inner"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Text = r.ReadBytes(12);
			this.Rect_Outter = new Rect(); this.Rect_Outter.read(r);
			this.Rect_Inner = new Rect(); this.Rect_Inner.read(r);
		}
	}
	public class SymbolDefinitionSection {
		public Int32 Count { get; set; }
		public SymbolDefinition[] SymbolDefinitions { get; set; }

		public string[] _order = {
			"Count",
			"SymbolDefinitions"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.SymbolDefinitions = new SymbolDefinition[this.Count]; for (int i=0; i<this.Count; i++) { SymbolDefinition obj = new SymbolDefinition(); obj.read(r); this.SymbolDefinitions[i] = obj; }
		}
	}
	public class PhraseIteration {
		public Int32 PhraseId { get; set; }
		public float StartTime { get; set; }
		public float NextPhraseTime { get; set; }
		public Int32[] _Difficulty = new Int32[3];
		public Int32[] Difficulty { get { return this._Difficulty; } set { _Difficulty = value; } }

		public string[] _order = {
			"PhraseId",
			"StartTime",
			"NextPhraseTime",
			"Difficulty"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.PhraseId = r.ReadInt32();
			this.StartTime = r.ReadSingle();
			this.NextPhraseTime = r.ReadSingle();
			this.Difficulty = new Int32[3]; for (int i=0; i<3; i++) this.Difficulty[i] = r.ReadInt32();
		}
	}
	public class PhraseIterationSection {
		public Int32 Count { get; set; }
		public PhraseIteration[] PhraseIterations { get; set; }

		public string[] _order = {
			"Count",
			"PhraseIterations"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.PhraseIterations = new PhraseIteration[this.Count]; for (int i=0; i<this.Count; i++) { PhraseIteration obj = new PhraseIteration(); obj.read(r); this.PhraseIterations[i] = obj; }
		}
	}
	public class PhraseExtraInfoByLevel {
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
		public void read(BinaryReader r) {
			this.PhraseId = r.ReadInt32();
			this.Difficulty = r.ReadInt32();
			this.Empty = r.ReadInt32();
			this.LevelJump = r.ReadByte();
			this.Redundant = r.ReadInt16();
			this.Padding = r.ReadByte();
		}
	}
	public class PhraseExtraInfoByLevelSection {
		public Int32 Count { get; set; }
		public PhraseExtraInfoByLevel[] PhraseExtraInfoByLevel { get; set; }

		public string[] _order = {
			"Count",
			"PhraseExtraInfoByLevel"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.PhraseExtraInfoByLevel = new PhraseExtraInfoByLevel[this.Count]; for (int i=0; i<this.Count; i++) { PhraseExtraInfoByLevel obj = new PhraseExtraInfoByLevel(); obj.read(r); this.PhraseExtraInfoByLevel[i] = obj; }
		}
	}
	public class NLinkedDifficulty {
		public Int32 LevelBreak { get; set; }
		public Int32 PhraseCount { get; set; }
		public Int32[] NLD_Phrase { get; set; }

		public string[] _order = {
			"LevelBreak",
			"PhraseCount",
			"NLD_Phrase"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.LevelBreak = r.ReadInt32();
			this.PhraseCount = r.ReadInt32();
			this.NLD_Phrase = new Int32[this.PhraseCount]; for (int i=0; i<this.PhraseCount; i++) this.NLD_Phrase[i] = r.ReadInt32();
		}
	}
	public class NLinkedDifficultySection {
		public Int32 Count { get; set; }
		public NLinkedDifficulty[] NLinkedDifficulties { get; set; }

		public string[] _order = {
			"Count",
			"NLinkedDifficulties"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.NLinkedDifficulties = new NLinkedDifficulty[this.Count]; for (int i=0; i<this.Count; i++) { NLinkedDifficulty obj = new NLinkedDifficulty(); obj.read(r); this.NLinkedDifficulties[i] = obj; }
		}
	}
	public class Action {
		public float Time { get; set; }
		public Byte[] _ActionName = new Byte[256];
		public Byte[] ActionName { get { return this._ActionName; } set { _ActionName = value; } }

		public string[] _order = {
			"Time",
			"ActionName"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Time = r.ReadSingle();
			this.ActionName = r.ReadBytes(256);
		}
	}
	public class ActionSection {
		public Int32 Count { get; set; }
		public Action[] Actions { get; set; }

		public string[] _order = {
			"Count",
			"Actions"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Actions = new Action[this.Count]; for (int i=0; i<this.Count; i++) { Action obj = new Action(); obj.read(r); this.Actions[i] = obj; }
		}
	}
	public class Event {
		public float Time { get; set; }
		public Byte[] _EventName = new Byte[256];
		public Byte[] EventName { get { return this._EventName; } set { _EventName = value; } }

		public string[] _order = {
			"Time",
			"EventName"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Time = r.ReadSingle();
			this.EventName = r.ReadBytes(256);
		}
	}
	public class EventSection {
		public Int32 Count { get; set; }
		public Event[] Events { get; set; }

		public string[] _order = {
			"Count",
			"Events"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Events = new Event[this.Count]; for (int i=0; i<this.Count; i++) { Event obj = new Event(); obj.read(r); this.Events[i] = obj; }
		}
	}
	public class Tone {
		public float Time { get; set; }
		public Int32 ToneId { get; set; }

		public string[] _order = {
			"Time",
			"ToneId"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Time = r.ReadSingle();
			this.ToneId = r.ReadInt32();
		}
	}
	public class ToneSection {
		public Int32 Count { get; set; }
		public Tone[] Tones { get; set; }

		public string[] _order = {
			"Count",
			"Tones"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Tones = new Tone[this.Count]; for (int i=0; i<this.Count; i++) { Tone obj = new Tone(); obj.read(r); this.Tones[i] = obj; }
		}
	}
	public class Dna {
		public float Time { get; set; }
		public Int32 DnaId { get; set; }

		public string[] _order = {
			"Time",
			"DnaId"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Time = r.ReadSingle();
			this.DnaId = r.ReadInt32();
		}
	}
	public class DnaSection {
		public Int32 Count { get; set; }
		public Dna[] Dnas { get; set; }

		public string[] _order = {
			"Count",
			"Dnas"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Dnas = new Dna[this.Count]; for (int i=0; i<this.Count; i++) { Dna obj = new Dna(); obj.read(r); this.Dnas[i] = obj; }
		}
	}
	public class Section {
		public Byte[] _Name = new Byte[32];
		public Byte[] Name { get { return this._Name; } set { _Name = value; } }
		public Int32 Number { get; set; }
		public float StartTime { get; set; }
		public float EndTime { get; set; }
		public Int32 StartPhraseIterationId { get; set; }
		public Int32 EndPhraseIterationId { get; set; }
		public Byte[] _StringMask = new Byte[36];
		public Byte[] StringMask { get { return this._StringMask; } set { _StringMask = value; } }

		public string[] _order = {
			"Name",
			"Number",
			"StartTime",
			"EndTime",
			"StartPhraseIterationId",
			"EndPhraseIterationId",
			"StringMask"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Name = r.ReadBytes(32);
			this.Number = r.ReadInt32();
			this.StartTime = r.ReadSingle();
			this.EndTime = r.ReadSingle();
			this.StartPhraseIterationId = r.ReadInt32();
			this.EndPhraseIterationId = r.ReadInt32();
			this.StringMask = r.ReadBytes(36);
		}
	}
	public class SectionSection {
		public Int32 Count { get; set; }
		public Section[] Sections { get; set; }

		public string[] _order = {
			"Count",
			"Sections"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Sections = new Section[this.Count]; for (int i=0; i<this.Count; i++) { Section obj = new Section(); obj.read(r); this.Sections[i] = obj; }
		}
	}
	public class Anchor {
		public float StartBeatTime { get; set; }
		public float EndBeatTime { get; set; }
		public float Unk3_FirstNoteTime { get; set; }
		public float Unk4_LastNoteTime { get; set; }
		public Byte FretId { get; set; }
		public Byte[] _Padding = new Byte[3];
		public Byte[] Padding { get { return this._Padding; } set { _Padding = value; } }
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.StartBeatTime = r.ReadSingle();
			this.EndBeatTime = r.ReadSingle();
			this.Unk3_FirstNoteTime = r.ReadSingle();
			this.Unk4_LastNoteTime = r.ReadSingle();
			this.FretId = r.ReadByte();
			this.Padding = r.ReadBytes(3);
			this.Width = r.ReadInt32();
			this.PhraseIterationId = r.ReadInt32();
		}
	}
	public class AnchorSection {
		public Int32 Count { get; set; }
		public Anchor[] Anchors { get; set; }

		public string[] _order = {
			"Count",
			"Anchors"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Anchors = new Anchor[this.Count]; for (int i=0; i<this.Count; i++) { Anchor obj = new Anchor(); obj.read(r); this.Anchors[i] = obj; }
		}
	}
	public class AnchorExtension {
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.BeatTime = r.ReadSingle();
			this.FretId = r.ReadByte();
			this.Unk2_0 = r.ReadInt32();
			this.Unk3_0 = r.ReadInt16();
			this.Unk4_0 = r.ReadByte();
		}
	}
	public class AnchorExtensionSection {
		public Int32 Count { get; set; }
		public AnchorExtension[] AnchorExtensions { get; set; }

		public string[] _order = {
			"Count",
			"AnchorExtensions"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.AnchorExtensions = new AnchorExtension[this.Count]; for (int i=0; i<this.Count; i++) { AnchorExtension obj = new AnchorExtension(); obj.read(r); this.AnchorExtensions[i] = obj; }
		}
	}
	public class Fingerprint {
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.ChordId = r.ReadInt32();
			this.StartTime = r.ReadSingle();
			this.EndTime = r.ReadSingle();
			this.Unk3_FirstNoteTime = r.ReadSingle();
			this.Unk4_LastNoteTime = r.ReadSingle();
		}
	}
	public class FingerprintSection {
		public Int32 Count { get; set; }
		public Fingerprint[] Fingerprints { get; set; }

		public string[] _order = {
			"Count",
			"Fingerprints"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Fingerprints = new Fingerprint[this.Count]; for (int i=0; i<this.Count; i++) { Fingerprint obj = new Fingerprint(); obj.read(r); this.Fingerprints[i] = obj; }
		}
	}
	public class Notes {
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
		public Int16[] FingerPrintId { get { return this._FingerPrintId; } set { _FingerPrintId = value; } }
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.NoteMask = r.ReadUInt32();
			this.NoteFlags = r.ReadUInt32();
			this.Hash = r.ReadUInt32();
			this.Time = r.ReadSingle();
			this.StringIndex = r.ReadByte();
			this.FretId = r.ReadByte();
			this.AnchorFretId = r.ReadByte();
			this.AnchorWidth = r.ReadByte();
			this.ChordId = r.ReadInt32();
			this.ChordNotesId = r.ReadInt32();
			this.PhraseId = r.ReadInt32();
			this.PhraseIterationId = r.ReadInt32();
			this.FingerPrintId = new Int16[2]; for (int i=0; i<2; i++) this.FingerPrintId[i] = r.ReadInt16();
			this.NextIterNote = r.ReadInt16();
			this.PrevIterNote = r.ReadInt16();
			this.ParentPrevNote = r.ReadInt16();
			this.SlideTo = r.ReadByte();
			this.SlideUnpitchTo = r.ReadByte();
			this.LeftHand = r.ReadByte();
			this.Tap = r.ReadByte();
			this.PickDirection = r.ReadByte();
			this.Slap = r.ReadByte();
			this.Pluck = r.ReadByte();
			this.Vibrato = r.ReadInt16();
			this.Sustain = r.ReadSingle();
			this.MaxBend = r.ReadSingle();
			this.BendData = new BendDataSection(); this.BendData.read(r);
		}
	}
	public class NotesSection {
		public Int32 Count { get; set; }
		public Notes[] Notes { get; set; }

		public string[] _order = {
			"Count",
			"Notes"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Notes = new Notes[this.Count]; for (int i=0; i<this.Count; i++) { Notes obj = new Notes(); obj.read(r); this.Notes[i] = obj; }
		}
	}
	public class Arrangement {
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Difficulty = r.ReadInt32();
			this.Anchors = new AnchorSection(); this.Anchors.read(r);
			this.AnchorExtensions = new AnchorExtensionSection(); this.AnchorExtensions.read(r);
			this.Fingerprints1 = new FingerprintSection(); this.Fingerprints1.read(r);
			this.Fingerprints2 = new FingerprintSection(); this.Fingerprints2.read(r);
			this.Notes = new NotesSection(); this.Notes.read(r);
			this.PhraseCount = r.ReadInt32();
			this.AverageNotesPerIteration = new float[this.PhraseCount]; for (int i=0; i<this.PhraseCount; i++) this.AverageNotesPerIteration[i] = r.ReadSingle();
			this.PhraseIterationCount1 = r.ReadInt32();
			this.NotesInIteration1 = new Int32[this.PhraseIterationCount1]; for (int i=0; i<this.PhraseIterationCount1; i++) this.NotesInIteration1[i] = r.ReadInt32();
			this.PhraseIterationCount2 = r.ReadInt32();
			this.NotesInIteration2 = new Int32[this.PhraseIterationCount2]; for (int i=0; i<this.PhraseIterationCount2; i++) this.NotesInIteration2[i] = r.ReadInt32();
		}
	}
	public class ArrangementSection {
		public Int32 Count { get; set; }
		public Arrangement[] Arrangements { get; set; }

		public string[] _order = {
			"Count",
			"Arrangements"
		};
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.Count = r.ReadInt32();
			this.Arrangements = new Arrangement[this.Count]; for (int i=0; i<this.Count; i++) { Arrangement obj = new Arrangement(); obj.read(r); this.Arrangements[i] = obj; }
		}
	}
	public class Metadata {
		public double MaxScore { get; set; }
		public double MaxNotesAndChords { get; set; }
		public double MaxNotesAndChords_Real { get; set; }
		public double PointsPerNote { get; set; }
		public float FirstBeatLength { get; set; }
		public float StartTime { get; set; }
		public Byte CapoFretId { get; set; }
		public Byte[] _LastConversionDateTime = new Byte[32];
		public Byte[] LastConversionDateTime { get { return this._LastConversionDateTime; } set { _LastConversionDateTime = value; } }
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.MaxScore = r.ReadDouble();
			this.MaxNotesAndChords = r.ReadDouble();
			this.MaxNotesAndChords_Real = r.ReadDouble();
			this.PointsPerNote = r.ReadDouble();
			this.FirstBeatLength = r.ReadSingle();
			this.StartTime = r.ReadSingle();
			this.CapoFretId = r.ReadByte();
			this.LastConversionDateTime = r.ReadBytes(32);
			this.Part = r.ReadInt16();
			this.SongLength = r.ReadSingle();
			this.StringCount = r.ReadInt32();
			this.Tuning = new Int16[this.StringCount]; for (int i=0; i<this.StringCount; i++) this.Tuning[i] = r.ReadInt16();
			this.Unk11_FirstNoteTime = r.ReadSingle();
			this.Unk12_FirstNoteTime = r.ReadSingle();
			this.MaxDifficulty = r.ReadInt32();
		}
	}
	public class Sng {
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
		public string[] order { get { return this._order; } }
		public void read(BinaryReader r) {
			this.BPMs = new BpmSection(); this.BPMs.read(r);
			this.Phrases = new PhraseSection(); this.Phrases.read(r);
			this.Chords = new ChordSection(); this.Chords.read(r);
			this.ChordNotes = new ChordNotesSection(); this.ChordNotes.read(r);
			this.Vocals = new VocalSection(); this.Vocals.read(r);
			this.SymbolsHeader = new SymbolsHeaderSection(); this.SymbolsHeader.read(r);
			this.SymbolsTexture = new SymbolsTextureSection(); this.SymbolsTexture.read(r);
			this.SymbolsDefinition = new SymbolDefinitionSection(); this.SymbolsDefinition.read(r);
			this.PhraseIterations = new PhraseIterationSection(); this.PhraseIterations.read(r);
			this.PhraseExtraInfo = new PhraseExtraInfoByLevelSection(); this.PhraseExtraInfo.read(r);
			this.NLD = new NLinkedDifficultySection(); this.NLD.read(r);
			this.Actions = new ActionSection(); this.Actions.read(r);
			this.Events = new EventSection(); this.Events.read(r);
			this.Tones = new ToneSection(); this.Tones.read(r);
			this.DNAs = new DnaSection(); this.DNAs.read(r);
			this.Sections = new SectionSection(); this.Sections.read(r);
			this.Arrangements = new ArrangementSection(); this.Arrangements.read(r);
			this.Metadata = new Metadata(); this.Metadata.read(r);
		}
	}
}
