using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.Sng
{
    public class Ebeat
    {
        public  float Time { get; set; }
        public  Int16 Measure { get; set; }
        public  Int16 Beat { get; set; }
        public  Int32 IsFirstBeatInMeasure { get; set; } //?

        public Ebeat(BinaryReader br)
        {
            Time = br.ReadSingle();
            Measure = br.ReadInt16();
            Beat = br.ReadInt16();
            IsFirstBeatInMeasure = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time);
            bw.Write(Measure);
            bw.Write(Beat);
            bw.Write(IsFirstBeatInMeasure);
        }
    }

    public class Phrase
    {
        public byte Solo { get; set; }
        public byte Disparity { get; set; }
        public byte Ignore { get; set; }
        public byte Unknown { get; set; }
        public  Int32 MaxDifficulty { get; set; }
        public  Int32 PhraseIterationCount { get; set; }
        private  Byte[] _name; // len = 32
        public string Name { get { return _name.ToNullTerminatedAscii(); } }

        public Phrase(BinaryReader br)
        {
            Solo = br.ReadByte();
            Disparity = br.ReadByte();
            Ignore = br.ReadByte();
            Unknown = br.ReadByte();
            MaxDifficulty = br.ReadInt32();
            PhraseIterationCount = br.ReadInt32();
            _name = br.ReadBytes(32);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Disparity);
            bw.Write(MaxDifficulty);
            bw.Write(PhraseIterationCount);
            bw.Write(_name);
        }
    }

    public class PhraseIteration
    {
        public  Int32 Id { get; set; }
        public  float StartTime { get; set; }
        public  float EndTime { get; set; }

        public PhraseIteration(BinaryReader br)
        {
            Id = br.ReadInt32();
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(StartTime);
            bw.Write(EndTime);
        }
    }

    public class Control
    {
        public  float Time { get; set; }
        public  Byte[] _code; // len = 256
        public string Code { get { return _code.ToNullTerminatedAscii(); } }

        public Control(BinaryReader br)
        {
            Time = br.ReadSingle();
            _code = br.ReadBytes(256);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time);
            bw.Write(_code);
        }
    }

    public class SongEvent
    {
        public  float Time { get; set; }
        public  Byte[] _code; // len = 256
        public string Code { get { return _code.ToNullTerminatedAscii(); } }

        public SongEvent(BinaryReader br)
        {
            Time = br.ReadSingle();
            _code = br.ReadBytes(256);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time);
            bw.Write(_code);
        }
    }

    public class SongSection
    {
        public  Byte[] _name; // len = 32
        public  Int32 Number { get; set; }
        public  float StartTime { get; set; }
        public  float EndTime { get; set; }
        public  Int32 StartPhraseIteration { get; set; } //??
        public Int32 EndPhraseIteration { get; set; } // ??
        public Byte Bit1 { get; set; } // ??
        public Byte Bit2 { get; set; } // ??
        public Byte Bit3 { get; set; } // ??
        public Byte Bit4 { get; set; } // ??
        public Byte Bit5 { get; set; } // ??
        public Byte Bit6 { get; set; } // ??
        public Byte Bit7 { get; set; } // ??
        public Byte Bit8 { get; set; } // ??

        public string Name { get { return _name.ToNullTerminatedAscii(); } }

        public SongSection(BinaryReader br)
        {
            _name = br.ReadBytes(32);
            Number = br.ReadInt32();
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            StartPhraseIteration = br.ReadInt32();
            EndPhraseIteration = br.ReadInt32();
            Bit1 = br.ReadByte();
            Bit2 = br.ReadByte();
            Bit3 = br.ReadByte();
            Bit4 = br.ReadByte();
            Bit5 = br.ReadByte();
            Bit6 = br.ReadByte();
            Bit7 = br.ReadByte();
            Bit8 = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_name);
            bw.Write(Number);
            bw.Write(StartTime);
            bw.Write(EndTime);
            bw.Write(StartPhraseIteration);
            bw.Write(EndPhraseIteration);
            bw.Write(Bit1);
            bw.Write(Bit2);
            bw.Write(Bit3);
            bw.Write(Bit4);
            bw.Write(Bit5);
            bw.Write(Bit6);
            bw.Write(Bit7);
            bw.Write(Bit8);
        }
    }

    public class SongLevel
    {
        public  Int32 Difficulty { get; set; }
        public  Int32 AnchorCount; 
        public  Anchor[] Anchors { get; set; }
        public  Int32 SlideCount;
        public  Slide[] Slides { get; set; }
        public  Int32 HandShapeCount;
        public  HandShape[] HandShapes { get; set; }
        public  Int32 NoteCount;
        public  Note[] Notes { get; set; }
        public  Int32 PhraseCount;
        public  float[] AverageNotesPerPhrase { get; set; }
        public  Int32 PhraseIterationCount;
        public  Int32[] NotesPerIteration1 { get; set; }
        public Int32[] NotesPerIteration2 { get; set; }

        public SongLevel(BinaryReader br, int version)
        {
            Difficulty = br.ReadInt32();
            AnchorCount = br.ReadInt32();
            Anchors = new Anchor[AnchorCount];
            for (int i = 0; i < AnchorCount; ++i)
            {
                Anchors[i] = new Anchor(br);
            }

            SlideCount = br.ReadInt32();
            Slides = new Slide[SlideCount];
            for (int i = 0; i < SlideCount; ++i)
            {
                Slides[i] = new Slide(br);
            }

            HandShapeCount = br.ReadInt32();
            HandShapes = new HandShape[HandShapeCount];
            for (int i = 0; i < HandShapeCount; ++i)
            {
                HandShapes[i] = new HandShape(br);
            }

            NoteCount = br.ReadInt32();
            Notes = new Note[NoteCount];
            for (int i = 0; i < NoteCount; ++i)
            {
                Notes[i] = new Note(br, version);
            }

            PhraseCount = br.ReadInt32();
            AverageNotesPerPhrase = new float[PhraseCount];
            for (int i = 0; i < PhraseCount; ++i)
            {
                AverageNotesPerPhrase[i] = br.ReadSingle();
            }

            PhraseIterationCount = br.ReadInt32();
            NotesPerIteration1 = new Int32[PhraseIterationCount];
            for (int i = 0; i < NotesPerIteration1.Count(); ++i)
            {
                NotesPerIteration1[i] = br.ReadInt32();
            }

            PhraseIterationCount = br.ReadInt32();
            NotesPerIteration2 = new Int32[PhraseIterationCount];
            for (int i = 0; i < NotesPerIteration2.Count(); ++i)
            {
                NotesPerIteration2[i] = br.ReadInt32();
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Difficulty);
            bw.Write(AnchorCount);
            for (int i = 0; i < AnchorCount; ++i)
            {
                Anchors[i].Write(bw);
            }

            bw.Write(SlideCount);
            for (int i = 0; i < SlideCount; ++i)
            {
                Slides[i].Write(bw);
            }

            bw.Write(HandShapeCount);
            for (int i = 0; i < HandShapeCount; ++i)
            {
                HandShapes[i].Write(bw);
            }

            bw.Write(NoteCount);
            for (int i = 0; i < NoteCount; ++i)
            {
                Notes[i].Write(bw);
            }

            bw.Write(PhraseCount);
            for (int i = 0; i < PhraseCount; ++i)
            {
                bw.Write(AverageNotesPerPhrase[i]);
            }

            bw.Write(PhraseIterationCount);
            for (int i = 0; i < NotesPerIteration1.Count(); ++i)
            {
                bw.Write(NotesPerIteration1[i]);
            }

            bw.Write(PhraseIterationCount);
            for (int i = 0; i < NotesPerIteration2.Count(); ++i)
            {
                bw.Write(NotesPerIteration2[i]);
            }
        }
    }

    public class Anchor
    {
        public  float StartTime { get; set; }
        public  float EndTime { get; set; }
        public  float MidTime { get; set; } // ?
        public  Int32 Fret { get; set; }
        public  Int32 PhraseIteration { get; set; }

        public Anchor(BinaryReader br)
        {
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            MidTime = br.ReadSingle();
            Fret = br.ReadInt32();
            PhraseIteration = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(StartTime);
            bw.Write(EndTime);
            bw.Write(MidTime);
            bw.Write(Fret);
            bw.Write(PhraseIteration);
        }
    }

    public class Note
    {
        public  float Time { get; set; }
        public  Int32 String { get; set; }
        public  Int32 Fret { get; set; }
        public  Int32 ChordId { get; set; }
        public  Int32 Unknown1 { get; set; }
        public  float SustainTime { get; set; }
        public  Int32 Bend { get; set; }
        public  Int32 SlideTo { get; set; }
        public  byte Tremolo { get; set; }
        public  byte Harmonic { get; set; }
        public  byte PalmMute { get; set; }
        public  byte Hopo { get; set; }
        public  Int32 Slap { get; set; }
        public  Int32 Pluck { get; set; }
        public  byte HammerOn { get; set; }
        public  byte PullOff { get; set; }
        public  byte Ignore { get; set; }
        public  byte HighDensity { get; set; }
        public  Int32 Unknown2 { get; set; } //?
        public  Int32 IterationId { get; set; }
        public  Int32 PhraseId { get; set; }

        public Note(BinaryReader br, int version)
        {
            Time = br.ReadSingle();
            String = br.ReadInt32();
            Fret = br.ReadInt32();
            ChordId = br.ReadInt32();
            Unknown2 = br.ReadInt32();
            SustainTime = br.ReadSingle();
            Bend = br.ReadInt32();
            SlideTo = br.ReadInt32();
            Tremolo = br.ReadByte();
            Harmonic = br.ReadByte();
            PalmMute = br.ReadByte();
            Hopo = br.ReadByte();
            if (version == 51)
            {
                Slap = br.ReadInt32();
                Pluck = br.ReadInt32();
            }
            HammerOn = br.ReadByte();
            PullOff = br.ReadByte();
            Ignore = br.ReadByte();
            HighDensity = br.ReadByte();
            Unknown2 = br.ReadInt32();
            IterationId = br.ReadInt32();
            PhraseId = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            /*
            bw.Write(Time);
            bw.Write(String);
            bw.Write(Fret);
            bw.Write(Unknown1);
            bw.Write(Unknown2);
            bw.Write(SustainTime);
            bw.Write(Bend);
            bw.Write(Unknown3);
            bw.Write(Unknown4);
            bw.Write(PalmMute);
            bw.Write(Unknown5);
            bw.Write(Unknown6);
            bw.Write(Index);
            bw.Write(Unknown7);
             */
        }
    }

    public class Slide
    {
        public  float Time { get; set; }
        public  Int32 EndingFret { get; set; }

        public Slide(BinaryReader br)
        {
            Time = br.ReadSingle();
            EndingFret = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time);
            bw.Write(EndingFret);
        }
    }

    public class HandShape
    {
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float Unknown1 { get; set; } //?
        public float Unknown2 { get; set; } //?
        public Int32 ChordId { get; set; }
        public float FirstChordInHandShapeTime { get; set; }
        public float LastChordInHandShapeTime { get; set; }

        public HandShape(BinaryReader br)
        {
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            Unknown1 = br.ReadSingle();
            Unknown2 = br.ReadSingle();
            ChordId = br.ReadInt32();
            FirstChordInHandShapeTime = br.ReadSingle();
            LastChordInHandShapeTime = br.ReadSingle();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(StartTime);
            bw.Write(EndTime);
            bw.Write(Unknown1);
            bw.Write(Unknown2);
            bw.Write(ChordId);
            bw.Write(FirstChordInHandShapeTime);
            bw.Write(LastChordInHandShapeTime);
        }
    }


    public class ChordTemplate
    {
        public  Int32 Fret0 { get; set; }
        public  Int32 Fret1 { get; set; }
        public  Int32 Fret2 { get; set; }
        public  Int32 Fret3 { get; set; }
        public  Int32 Fret4 { get; set; }
        public Int32 Fret5 { get; set; }
        public Int32 Finger0 { get; set; }
        public Int32 Finger1 { get; set; }
        public Int32 Finger2 { get; set; }
        public Int32 Finger3 { get; set; }
        public Int32 Finger4 { get; set; }
        public Int32 Finger5 { get; set; }
        public Int32 Note0 { get; set; }
        public Int32 Note1 { get; set; }
        public Int32 Note2 { get; set; }
        public Int32 Note3 { get; set; }
        public Int32 Note4 { get; set; }
        public Int32 Note5 { get; set; }
        private  Byte[] _name; // len 32
        public string Name { get { return _name.ToNullTerminatedAscii(); } }

        public ChordTemplate(BinaryReader br)
        {
            Fret0 = br.ReadInt32();
            Fret1 = br.ReadInt32();
            Fret2 = br.ReadInt32();
            Fret3 = br.ReadInt32();
            Fret4 = br.ReadInt32();
            Fret5 = br.ReadInt32();
            Finger0 = br.ReadInt32();
            Finger1 = br.ReadInt32();
            Finger2 = br.ReadInt32();
            Finger3 = br.ReadInt32();
            Finger4 = br.ReadInt32();
            Finger5 = br.ReadInt32();
            Note0 = br.ReadInt32();
            Note1 = br.ReadInt32();
            Note2 = br.ReadInt32();
            Note3 = br.ReadInt32();
            Note4 = br.ReadInt32();
            Note5 = br.ReadInt32();
            _name = br.ReadBytes(32);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Fret0);
            bw.Write(Fret1);
            bw.Write(Fret2);
            bw.Write(Fret3);
            bw.Write(Fret4);
            bw.Write(Fret5);
            bw.Write(Finger0);
            bw.Write(Finger1);
            bw.Write(Finger2);
            bw.Write(Finger3);
            bw.Write(Finger4);
            bw.Write(Finger5);
            bw.Write(Note0);
            bw.Write(Note1);
            bw.Write(Note2);
            bw.Write(Note3);
            bw.Write(Note4);
            bw.Write(Note5);
            bw.Write(_name);
        }
    }

    public class Vocal
    {
        public  float Time { get; set; }
        public  Int32 Note { get; set; }
        public  float Length { get; set; }
        private  Byte[] _lyric; // len 32
        public string Lyric { get { return _lyric.ToNullTerminatedUTF8(); } }

        public Vocal(BinaryReader br)
        {
            Time = br.ReadSingle();
            Note = br.ReadInt32();
            Length = br.ReadSingle();
            _lyric = br.ReadBytes(32);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time);
            bw.Write(Note);
            bw.Write(Length);
            bw.Write(_lyric);
        }
    }

    public class Metadata
    {
        public  double MaxScore { get; set; }
        public  double TotalNotes { get; set; }
        public  double PointsPerNote { get; set; }
        public  float BeatTiming { get; set; }
        public  float FirstBeat { get; set; }

        private  Byte[] _lastConversion; // len = 32
        private  Byte[] _songTitle; // len = 64
        private  Byte[] _arrangement; // len = 32
        private  Byte[] _artist; // len = 32
        public  Int16 SongPart { get; set; }
        public  float Length { get; set; }

        public string LastConversion { get { return _lastConversion.ToNullTerminatedAscii(); } }
        public string SongTitle { get { return _songTitle.ToNullTerminatedAscii(); } }
        public string Arrangement { get { return _arrangement.ToNullTerminatedAscii(); } }
        public string Artist { get { return _artist.ToNullTerminatedAscii(); } }

        public int Tuning { get; set; }
        public float Difficulty { get; set; }
        public float Unknown1 { get; set; }
        public float Unknown2 { get; set; }
        public int MaxDifficulty { get; set; }
        public UnknownSection1[] UnknownSection1 { get; set; }
        public UnknownSection2[] UnknownSection2 { get; set; }

        public Metadata(BinaryReader br)
        {
            MaxScore = br.ReadDouble();
            TotalNotes = br.ReadDouble();
            PointsPerNote = br.ReadDouble();
            BeatTiming = br.ReadSingle();
            FirstBeat = br.ReadSingle();
            _lastConversion = br.ReadBytes(32);
            _songTitle = br.ReadBytes(64);
            _arrangement = br.ReadBytes(32);
            _artist = br.ReadBytes(32);
            SongPart = br.ReadInt16();
            Length = br.ReadSingle();
            Tuning = br.ReadInt32();
            Difficulty = br.ReadSingle();
            Unknown1 = br.ReadSingle();
            Unknown2 = br.ReadSingle();
            MaxDifficulty = br.ReadInt32();

            int len = br.ReadInt32();
            UnknownSection1 = new UnknownSection1[len];
            for (int i = 0; i < UnknownSection1.Count(); ++i)
            {
                UnknownSection1[i] = new UnknownSection1(br);
            }

            len = br.ReadInt32();
            UnknownSection2 = new UnknownSection2[len];
            for (int i = 0; i < UnknownSection2.Count(); ++i)
            {
                UnknownSection2[i] = new UnknownSection2(br);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_lastConversion);
            bw.Write(_songTitle);
            bw.Write(_arrangement);
            bw.Write(_artist);
            bw.Write(SongPart);
            bw.Write(Length);
        }

    }

    public class UnknownSection1
    {
        public byte[] Unknown1 { get; set; }
        public byte[] Unknown2 { get; set; }
        public int Unknown3 { get; set; }

        public UnknownSection1(BinaryReader br)
        {
            Unknown1 = br.ReadBytes(4);
            Unknown2 = br.ReadBytes(4);
            Unknown3 = br.ReadInt32();
        }
    }

    public class UnknownSection2
    {
        private byte[] _unknown1 {get; set;}
        public string Unknown1 { get { return _unknown1.ToNullTerminatedAscii(); } }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }

        public UnknownSection2(BinaryReader br)
        {
            _unknown1 = br.ReadBytes(64);
            Unknown2 = br.ReadInt32();
            Unknown3 = br.ReadInt32();
        }
    }

    public class PhraseProperty
    {
        public  Int32 PhraseId { get; set; }
        public  Int32 Difficulty { get; set; }
        public  Int32 Empty { get; set; } // ?
        public  Int16 LevelJump { get; set; } // ?
        public  Int16 Redundant { get; set; } // ?

        public PhraseProperty(BinaryReader br)
        {
            PhraseId = br.ReadInt32();
            Difficulty = br.ReadInt32();
            Empty = br.ReadInt32();
            LevelJump = br.ReadInt16();
            Redundant = br.ReadInt16();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(PhraseId);
            bw.Write(Difficulty);
            bw.Write(Empty);
            bw.Write(LevelJump);
            bw.Write(Redundant);
        }
    }

    public class LinkedDiff
    {
        public  Int32 ParentId { get; set; }
        public  Int32 ChildId { get; set; }
        public  Int32 Unknown { get; set; } // ?

        public LinkedDiff(BinaryReader br)
        {
            ParentId = br.ReadInt32();
            ChildId = br.ReadInt32();
            Unknown = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ParentId);
            bw.Write(ChildId);
            bw.Write(Unknown);
        }
    }

    public class SngFile
    {
        public  Int32 Version { get; set; } // ?
        private  Int32 _beatCount;
        public Ebeat[] Beats { get; set; }
        private  Int32 _PhraseCount;
        public Phrase[] Phrases { get; set; }
        private  Int32 _chordTemplateCount;
        public ChordTemplate[] ChordTemplates;
        private  Int32 _fretHandMuteTemplateCount;
        private  Int32 _vocalsCount;
        public Vocal[] _vocals { get; set; }
        public Int32 _phraseIterationCount;
        public PhraseIteration[] PhraseIterations { get; set; }
        public  Int32 PhrasePropertyCount;
        public  PhraseProperty[] PhraseProperties { get; set; }
        private  Int32 LinkedDiffCount;
        private  LinkedDiff[] LinkedDiffs { get; set; }
        public  Int32 ControlCount;
        public  Control[] Controls { get; set; }
        private  Int32 _songEventCount;
        public  SongEvent[] SongEvents { get; set; }
        public  Int32 SongSectionCount;
        public  SongSection[] SongSections { get; set; }
        private  Int32 _songLevelCount;
        public  SongLevel[] SongLevels { get; set; }
        public  Metadata Metadata { get; set; }
        public  Byte[] _unknown2 { get; set; } // len ??


        private  string _filePath;
        public override string ToString()
        {
            if (Metadata != null)
                return Metadata.SongTitle;

            if (!string.IsNullOrWhiteSpace(_filePath))
                return Path.GetFileName(_filePath);

            return "<Unknown>";
        }

        public SngFile(string file)
        {
            _filePath = file;

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader br = new BinaryReader(stream);
                Version = br.ReadInt32();
                _beatCount = br.ReadInt32();
                Beats = new Ebeat[_beatCount];
                for (int i = 0; i < _beatCount; ++i)
                {
                    Beats[i] = new Ebeat(br);
                }

                _PhraseCount = br.ReadInt32();
                Phrases = new Phrase[_PhraseCount];
                for (int i = 0; i < _PhraseCount; ++i)
                {
                    Phrases[i] = new Phrase(br);
                }

                _chordTemplateCount = br.ReadInt32();
                ChordTemplates = new ChordTemplate[_chordTemplateCount];
                for (int i = 0; i < _chordTemplateCount; ++i)
                {
                    ChordTemplates[i] = new ChordTemplate(br);
                }
                _fretHandMuteTemplateCount = br.ReadInt32(); // always 0?
                _vocalsCount = br.ReadInt32();
                _vocals = new Vocal[_vocalsCount];
                for (int i = 0; i < _vocalsCount; ++i)
                {
                    _vocals[i] = new Vocal(br);
                }

                _phraseIterationCount = br.ReadInt32();
                PhraseIterations = new PhraseIteration[_phraseIterationCount];
                for (int i = 0; i < _phraseIterationCount; ++i)
                {
                    PhraseIterations[i] = new PhraseIteration(br);
                }

                PhrasePropertyCount = br.ReadInt32();
                PhraseProperties = new PhraseProperty[PhrasePropertyCount];
                for (int i = 0; i < PhrasePropertyCount; ++i)
                {
                    PhraseProperties[i] = new PhraseProperty(br);
                }

                LinkedDiffCount = br.ReadInt32();
                LinkedDiffs = new LinkedDiff[LinkedDiffCount];
                for (int i = 0; i < LinkedDiffCount; ++i)
                {
                    LinkedDiffs[i] = new LinkedDiff(br);
                }

                ControlCount = br.ReadInt32();
                Controls = new Control[ControlCount];
                for (int i = 0; i < ControlCount; ++i)
                {
                    Controls[i] = new Control(br);
                }

                _songEventCount = br.ReadInt32();
                SongEvents = new SongEvent[_songEventCount];
                for (int i = 0; i < _songEventCount; ++i)
                {
                    SongEvents[i] = new SongEvent(br);
                }
                SongSectionCount = br.ReadInt32();
                SongSections = new SongSection[SongSectionCount];
                for (int i = 0; i < SongSectionCount; ++i)
                {
                    SongSections[i] = new SongSection(br);
                }

                _songLevelCount = br.ReadInt32();
                SongLevels = new SongLevel[_songLevelCount];
                for (int i = 0; i < _songLevelCount; ++i)
                {
                    SongLevels[i] = new SongLevel(br, Version);
                }

                Metadata = new Metadata(br);

                // Not sure what this junk is down here yet

                int endLength = (int)(br.BaseStream.Length - br.BaseStream.Position);
                _unknown2 = br.ReadBytes(endLength);

            }
        }

        public void Write(string file)
        {
            using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write(Version);
                bw.Write(_beatCount);
                for (int i = 0; i < _beatCount; ++i)
                {
                    Beats[i].Write(bw);
                }

                bw.Write(_PhraseCount);
                for (int i = 0; i < _PhraseCount; ++i)
                {
                    Phrases[i].Write(bw);
                }

                bw.Write(_chordTemplateCount);
                for (int i = 0; i < _chordTemplateCount; ++i)
                {
                    ChordTemplates[i].Write(bw);
                }
                bw.Write(_fretHandMuteTemplateCount); // always 0?
                bw.Write(_vocalsCount);
                for (int i = 0; i < _vocalsCount; ++i)
                {
                    _vocals[i].Write(bw);
                }

                bw.Write(_phraseIterationCount);
                for (int i = 0; i < _phraseIterationCount; ++i)
                {
                    PhraseIterations[i].Write(bw);
                }

                bw.Write(PhrasePropertyCount);
                for (int i = 0; i < PhrasePropertyCount; ++i)
                {
                    PhraseProperties[i].Write(bw);
                }

                bw.Write(LinkedDiffCount);
                for (int i = 0; i < LinkedDiffCount; ++i)
                {
                    LinkedDiffs[i].Write(bw);
                }

                bw.Write(ControlCount);
                for (int i = 0; i < ControlCount; ++i)
                {
                    Controls[i].Write(bw);
                }

                bw.Write(_songEventCount);
                for (int i = 0; i < _songEventCount; ++i)
                {
                    SongEvents[i].Write(bw);
                }
                bw.Write(SongSectionCount);
                for (int i = 0; i < SongSectionCount; ++i)
                {
                    SongSections[i].Write(bw);
                }

                bw.Write(_songLevelCount);
                for (int i = 0; i < _songLevelCount; ++i)
                {
                    SongLevels[i].Write(bw);
                }

                Metadata.Write(bw);

                // Not sure what this junk is down here yet
                bw.Write(_unknown2);

            }
        }
    }

    public class PhraseIterationInfo
    {
        public int PhraseId { get; set; }
        public int IterationId { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int MaxDifficulty { get; set; }
    }
}
