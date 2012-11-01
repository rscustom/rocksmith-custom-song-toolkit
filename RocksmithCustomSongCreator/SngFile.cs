using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RocksmithSngCreator
{
    public static class ByteArrayExtension
    {
        public static string ToNullTerminatedAscii(this Byte[] bytes)
        {
            return System.Text.Encoding.ASCII.GetString(bytes).TrimEnd('\0');
        }
    }

    public class Ebeat
    {
        public readonly float Time;
        public readonly Int16 Measure;
        public readonly Int16 Beat; // 0 base
        public readonly Int32 Unknown; //?

        public Ebeat(BinaryReader br)
        {
            Time = br.ReadSingle();
            Measure = br.ReadInt16();
            Beat = br.ReadInt16();
            Unknown = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time);
            bw.Write(Measure);
            bw.Write(Beat);
            bw.Write(Unknown);
        }
    }

    public class Phrase
    {
        public readonly Int32 Disparity;
        public readonly Int32 MaxDifficulty;
        public readonly Int32 PhraseIterationCount;
        Byte[] _name; // len = 32
        public string Name { get { return _name.ToNullTerminatedAscii(); } }

        public Phrase(BinaryReader br)
        {
            Disparity = br.ReadInt32();
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
        public readonly Int32 Id;
        public readonly float StartTime;
        public readonly float EndTime;

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
        public readonly float Time;
        public readonly Byte[] _code; // len = 256
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
        public readonly float Time;
        public readonly Byte[] _code; // len = 256
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
        public readonly Byte[] _name; // len = 32
        public readonly Int32 Number;
        public readonly float StartTime;
        public readonly float EndTime;
        public readonly Int32 StartNote; //??
        public readonly Int32 EndNote; // ??
        public readonly Byte Bit1; // ??
        public readonly Byte Bit2; // ??
        public readonly Byte Bit3; // ??
        public readonly Byte Bit4; // ??
        public readonly Int32 Padding; // ??

        public string Name { get { return _name.ToNullTerminatedAscii(); } }

        public SongSection(BinaryReader br)
        {
            _name = br.ReadBytes(32);
            Number = br.ReadInt32();
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            StartNote = br.ReadInt32();
            EndNote = br.ReadInt32();
            Bit1 = br.ReadByte();
            Bit1 = br.ReadByte();
            Bit1 = br.ReadByte();
            Bit1 = br.ReadByte();
            Padding = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_name);
            bw.Write(Number);
            bw.Write(StartTime);
            bw.Write(EndTime);
            bw.Write(StartNote);
            bw.Write(EndNote);
            bw.Write(Bit1);
            bw.Write(Bit1);
            bw.Write(Bit1);
            bw.Write(Bit1);
            bw.Write(Padding);
        }
    }

    public struct SongLevel
    {
        public readonly Int32 Difficulty;
        public readonly Int32 AnchorCount; //?
        public readonly Anchor[] Anchors;
        public readonly Int32 ChordCount;
        public readonly Chord[] Chords;
        public readonly Int32 HandShapeCount; //?
        public readonly HandShape[] HandShapes; //?
        public readonly Int32 NoteCount; // ?
        public readonly Note[] Notes;
        public readonly Int32 Unknown1Count;
        public readonly Int32[] Unknown1s; // each is len 1
        public readonly Int32 Unknown2Count;
        public readonly Int32[] Unknown2s; // each is len 2
        public readonly Int32 Unknown3;

        public SongLevel(BinaryReader br)
        {
            Difficulty = br.ReadInt32();
            AnchorCount = br.ReadInt32();
            Anchors = new Anchor[AnchorCount];
            for (int i = 0; i < AnchorCount; ++i)
            {
                Anchors[i] = new Anchor(br);
            }

            ChordCount = br.ReadInt32();
            Chords = new Chord[ChordCount];
            for (int i = 0; i < ChordCount; ++i)
            {
                Chords[i] = new Chord(br);
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
                Notes[i] = new Note(br);
            }

            Unknown1Count = br.ReadInt32();
            Unknown1s = new Int32[Unknown1Count];
            for(int i = 0; i < Unknown1Count; ++i)
            {
                Unknown1s[i] = br.ReadInt32();
            }

            Unknown2Count = br.ReadInt32();
            Unknown2s = new Int32[Unknown2Count * 2]; // each is len 2
            for (int i = 0; i < Unknown2s.Count(); ++i)
            {
                Unknown2s[i] = br.ReadInt32();
            }

            Unknown3 = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Difficulty);
            bw.Write(AnchorCount);
            for (int i = 0; i < AnchorCount; ++i)
            {
                Anchors[i].Write(bw);
            }

            bw.Write(ChordCount);
            for (int i = 0; i < ChordCount; ++i)
            {
                Chords[i].Write(bw);
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

            bw.Write(Unknown1Count);
            for (int i = 0; i < Unknown1Count; ++i)
            {
                bw.Write(Unknown1s[i]);
            }

            bw.Write(Unknown2Count);
            for (int i = 0; i < Unknown2s.Count(); ++i)
            {
                bw.Write(Unknown2s[i]);
            }

            bw.Write(Unknown3);
        }
    }

    public struct Anchor
    {
        public readonly float StartTime;
        public readonly float EndTime;
        public readonly float MidTime; // ?
        public readonly Int32 Fret;
        public readonly Int32 Unknown; // ?

        public Anchor(BinaryReader br)
        {
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            MidTime = br.ReadSingle();
            Fret = br.ReadInt32();
            Unknown = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(StartTime);
            bw.Write(EndTime);
            bw.Write(MidTime);
            bw.Write(Fret);
            bw.Write(Unknown);
        }
    }

    public class Note
    {
        public readonly float Time;
        public readonly Int32 String;
        public readonly Int32 Fret;
        public readonly Int32 Unknown1; //?
        public readonly Int32 Unknown2; //?
        public readonly float SustainTime;
        public readonly Int32 Bend; //?
        public readonly Int32 Unknown3; //?
        public readonly Int16 Unknown4; //?  \
        public readonly Int16 PalmMute; //? /
        public readonly Int32 Unknown5; //?
        public readonly Int32 Unknown6; //?
        public readonly Int32 Index;
        public readonly Int32 Unknown7;

        public Note(BinaryReader br)
        {
            Time = br.ReadSingle();
            String = br.ReadInt32();
            Fret = br.ReadInt32();
            Unknown1 = br.ReadInt32();
            Unknown2 = br.ReadInt32();
            SustainTime = br.ReadSingle();
            Bend = br.ReadInt32();
            Unknown3 = br.ReadInt32();
            Unknown4 = br.ReadInt16();
            PalmMute = br.ReadInt16();
            Unknown5 = br.ReadInt32();
            Unknown6 = br.ReadInt32();
            Index = br.ReadInt32();
            Unknown7 = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
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
        }
    }

    public class Chord
    {
        public readonly float Time;
        public readonly Int32 Unknown; // Almost definitely a bitfield combining chord Id, "highDensity", "ignore", and "strum="down"

        public Chord(BinaryReader br)
        {
            Time = br.ReadSingle();
            Unknown = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time);
            bw.Write(Unknown);
        }
    }

    public class HandShape
    {
        public readonly float StartTime; //?
        public readonly float EndTime; //?
        public readonly float StartTime2; //?
        public readonly float EndTime2; //?
        public readonly float ChordId;
        public readonly float Unknown1;
        public readonly float Unknown2;

        public HandShape(BinaryReader br)
        {
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            StartTime2 = br.ReadSingle();
            EndTime2 = br.ReadSingle();
            ChordId = br.ReadSingle();
            Unknown1 = br.ReadSingle();
            Unknown2 = br.ReadSingle();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(StartTime);
            bw.Write(EndTime);
            bw.Write(StartTime2);
            bw.Write(EndTime2);
            bw.Write(ChordId);
            bw.Write(Unknown1);
            bw.Write(Unknown2);
        }
    }


    public class ChordTemplate
    {
        public readonly Int32 Fret0;
        public readonly Int32 Fret1;
        public readonly Int32 Fret2;
        public readonly Int32 Fret3;
        public readonly Int32 Fret4;
        public readonly Int32 Fret5;
        public readonly Int32 Finger0;
        public readonly Int32 Finger1;
        public readonly Int32 Finger2;
        public readonly Int32 Finger3;
        public readonly Int32 Finger4;
        public readonly Int32 Finger5;
        public Byte[] Unknown; // len 24;
        Byte[] _name; // len 32

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
            Unknown = br.ReadBytes(24);
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
            bw.Write(Unknown);
            bw.Write(_name);
        }
    }

    public class Vocal
    {
        public readonly float Time;
        public readonly Int32 Note;
        public readonly float Length;
        Byte[] _lyric; // len 32

        public string Lyric { get { return _lyric.ToNullTerminatedAscii(); } }

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
        Byte[] _lastConversion; // len = 32
        Byte[] _songTitle; // len = 64
        Byte[] _arrangement; // len = 32
        Byte[] _artist; // len = 32
        public readonly Int16 SongPart;
        public readonly float Length;

        public string LastConversion { get { return _lastConversion.ToNullTerminatedAscii(); } }
        public string SongTitle { get { return _songTitle.ToNullTerminatedAscii(); } }
        public string Arrangement { get { return _arrangement.ToNullTerminatedAscii(); } }
        public string Artist { get { return _artist.ToNullTerminatedAscii(); } }

        public Metadata(BinaryReader br)
        {
            _lastConversion = br.ReadBytes(32);
            _songTitle = br.ReadBytes(64);
            _arrangement = br.ReadBytes(32);
            _artist = br.ReadBytes(32);
            SongPart = br.ReadInt16();
            Length = br.ReadSingle();
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

    public class PhraseProperty
    {
        Int32 PhraseId;
        Int32 Difficulty;
        Int32 Empty; // ?
        Int16 LevelJump; // ?
        Int16 Redundant; // ?
        
        public PhraseProperty(BinaryReader br)
        {
            PhraseId = br.ReadInt32();
            Difficulty= br.ReadInt32();
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
        Int32 ParentId;
        Int32 ChildId;
        Int32 Unknown; // ?
        
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
        public readonly Int32 _version; // ?
        Int32 _beatCount;
        public Ebeat[] _beats;
        Int32 _PhraseCount;
        public Phrase[] _phrases;
        Int32 _chordTemplateCount; 
        ChordTemplate[] _chordTemplates;
        Int32 _fretHandMuteTemplateCount; 
        Int32 _vocalsCount;
        public Vocal[] _vocals;
        public Int32 _phraseIterationCount;
        public PhraseIteration[] _phraseIterations;
        public readonly Int32 PhrasePropertyCount;
        public readonly PhraseProperty[] PhraseProperties;
        Int32 LinkedDiffCount;
        LinkedDiff[] LinkedDiffs;
        public readonly Int32 ControlCount;
        public readonly Control[] Controls;
        Int32 _songEventCount;
        public readonly SongEvent[] SongEvents;
        public readonly Int32 SongSectionCount;
        public readonly SongSection[] SongSections;
        Int32 _songLevelCount;
        public readonly SongLevel[] SongLevels;
        public Byte[] _unknown; // len 32
        public readonly Metadata Metadata;
        public readonly Byte[] _unknown2; // len ??


        private string _filePath;
        public override string ToString()
        {
            if (Metadata != null)
            {
                return Metadata.SongTitle;
            }

            if (!string.IsNullOrWhiteSpace(_filePath))
            {
                return Path.GetFileName(_filePath);
            }
            else
            {
                return "<Unknown>";
            }
        }

        public SngFile(string file)
        {
            _filePath = file;

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader br = new BinaryReader(stream);
                _version = br.ReadInt32();
                _beatCount = br.ReadInt32();
                _beats = new Ebeat[_beatCount];
                for (int i = 0; i < _beatCount; ++i)
                {
                    _beats[i] = new Ebeat(br);
                }

                _PhraseCount = br.ReadInt32();
                _phrases = new Phrase[_PhraseCount];
                for (int i = 0; i < _PhraseCount; ++i)
                {
                    _phrases[i] = new Phrase(br);
                }

                _chordTemplateCount = br.ReadInt32();
                _chordTemplates = new ChordTemplate[_chordTemplateCount];
                for (int i = 0; i < _chordTemplateCount; ++i)
                {
                    _chordTemplates[i] = new ChordTemplate(br);
                }
                _fretHandMuteTemplateCount = br.ReadInt32(); // always 0?
                _vocalsCount = br.ReadInt32();
                _vocals = new Vocal[_vocalsCount];
                for (int i = 0; i < _vocalsCount; ++i)
                {
                    _vocals[i] = new Vocal(br);
                }

                _phraseIterationCount = br.ReadInt32();
                _phraseIterations = new PhraseIteration[_phraseIterationCount];
                for (int i = 0; i < _phraseIterationCount; ++i)
                {
                    _phraseIterations[i] = new PhraseIteration(br);
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

                _songLevelCount =  br.ReadInt32();
                SongLevels = new SongLevel[_songLevelCount];
                for (int i = 0; i < _songLevelCount; ++i)
                {
                    SongLevels[i] = new SongLevel(br);
                }

                _unknown = br.ReadBytes(32);

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
                bw.Write(_version);
                bw.Write(_beatCount);
                for (int i = 0; i < _beatCount; ++i)
                {
                    _beats[i].Write(bw);
                }

                bw.Write(_PhraseCount);
                for (int i = 0; i < _PhraseCount; ++i)
                {
                    _phrases[i].Write(bw);
                }

                bw.Write(_chordTemplateCount);
                for (int i = 0; i < _chordTemplateCount; ++i)
                {
                    _chordTemplates[i].Write(bw);
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
                    _phraseIterations[i].Write(bw);
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

                bw.Write(_unknown);

                Metadata.Write(bw);

                // Not sure what this junk is down here yet
                bw.Write(_unknown2);

            }
        }
    }

    

}
