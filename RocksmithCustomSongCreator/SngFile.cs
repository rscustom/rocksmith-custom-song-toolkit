using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RocksmithSngCreator
{
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
    }

    public class Phrase
    {
        public readonly Int32 Disparity;
        public readonly Int32 MaxDifficulty;
        public readonly Int32 PhraseIterationCount;
        Byte[] _name; // len = 32
        public string Name
        {
            get
            {
                return System.Text.Encoding.ASCII.GetString(_name).Split('\0')[0];
            }
        }

        public Phrase(BinaryReader br)
        {
            Disparity = br.ReadInt32();
            MaxDifficulty = br.ReadInt32();
            PhraseIterationCount = br.ReadInt32();
            _name = br.ReadBytes(32);
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
    }

    public class Control
    {
        public readonly float Time;
        public readonly Byte[] _code; // len = 256

        public string Code
        {
            get
            {
                return System.Text.Encoding.ASCII.GetString(_code).Split('\0')[0];
            }
        }


        public Control(BinaryReader br)
        {
            Time = br.ReadSingle();
            _code = br.ReadBytes(256);
        }
    }

    public class SongEvent
    {
        public readonly float Time;
        public readonly Byte[] _code; // len = 256

        public string Code
        {
            get
            {
                return System.Text.Encoding.ASCII.GetString(_code).Split('\0')[0];
            }
        }

        public SongEvent(BinaryReader br)
        {
            Time = br.ReadSingle();
            _code = br.ReadBytes(256);
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

        public string Name
        {
            get
            {
                return System.Text.Encoding.ASCII.GetString(_name).Split('\0')[0];
            }

        }
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

            int testCount = br.ReadInt32();
            for (int i = 0; i < testCount; ++i)
            {
                br.ReadInt32();
            }
            testCount = br.ReadInt32();
            for (int i = 0; i < testCount; ++i)
            {
                br.ReadInt32();
                br.ReadInt32();
            }

            br.ReadInt32();
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
    }

    public struct Note
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
    }

    public class HandShape
    {
        public readonly float StartTime; //?
        public readonly float EndTime; //?
        public readonly float StartTime2; //?
        public readonly float EndTime2; //?
        public readonly float ChordId;

        public HandShape(BinaryReader br)
        {
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            StartTime2 = br.ReadSingle();
            EndTime2 = br.ReadSingle();
            ChordId = br.ReadSingle();
            br.ReadSingle();
            br.ReadSingle();
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

        public string Name
        {
            get
            {
                return System.Text.Encoding.ASCII.GetString(_name).TrimEnd('\0');
            }
        }

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
    }

    public class Vocal
    {
        public readonly float Time;
        public readonly Int32 Note;
        public readonly float Length;
        Byte[] _lyric; // len 32

        public string Lyric
        {
            get
            {
                return System.Text.Encoding.ASCII.GetString(_lyric).TrimEnd('\0');
            }
        }

        public Vocal(BinaryReader br)
        {
            Time = br.ReadSingle();
            Note = br.ReadInt32();
            Length = br.ReadSingle();
            _lyric = br.ReadBytes(32);
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
        Int32 _propertyCount; //?
        // properties
        Int32 _linkedDiffCount; //?
        // linkedDiffs
        public readonly Int32 ControlCount;
        public readonly Control[] Controls;
        Int32 _songEventCount;
        public readonly SongEvent[] SongEvents;
        public readonly Int32 SongSectionCount;
        public readonly SongSection[] SongSections;
        Int32 _padding7; //?
        Int32 _padding8; //?
        Int32 _padding9; //?
        Int32 _padding10; //?
        Int32 _padding11; //?
        Int32 _padding12; //?
        Int32 _songLevelCount;
        SongLevel[] SongLevels;


        public SngFile(string file)
        {
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

                int propCount = br.ReadInt32();
                for (int i = 0; i < propCount; ++i)
                {
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                }
                int linkCount = br.ReadInt32();
                for (int i = 0; i < linkCount; ++i)
                {
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
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
            }
        }
    }

    

}
