using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocksmithToolkitLib.XML;

namespace RocksmithToTabLib
{
    /// <summary>
    /// An intermediary representation of a score. The score keeps information about
    /// title, artist, etc. as well as the individual tracks contained in the score.
    /// A track in our context is a Rocksmith arrangement at a particular difficulty
    /// level.
    /// This intermediary format is primarily intended to convert the Rocksmith format
    /// into something with actual rhythmic information.
    /// </summary>
    public class Score
    {
        public Score()
        {
            Tracks = new List<Track>();
        }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtistSort { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Tabber { get; set; }
        public List<string> Comments { get; set; }

        public List<Track> Tracks { get; set; }


        /// <summary>
        /// Sorts the tracks in order lead, rhythm, bass. If there is a tie,
        /// we sort with the bonus arrangement property, then the track name.
        /// </summary>
        public void SortTracks()
        {
            Tracks.Sort();
        }
    }


    /// <summary>
    /// A single track in the score. Represents a single arrangement at a particular
    /// difficulty level. It identifies the instrument and keeps a list of bars.
    /// </summary>
    public class Track : IComparable<Track>
    {
        public Track()
        {
            Bars = new List<Bar>();
            ChordTemplates = new Dictionary<int, ChordTemplate>();
        }

        public int CompareTo(Track other)
        {
            if (Path == other.Path)
            {
                if (Bonus == other.Bonus)
                    return Name.CompareTo(other.Name);
                else
                    return Bonus.CompareTo(other.Bonus);
            }
            else
                return Path.CompareTo(other.Path);
        }

        public enum InstrumentType
        {
            Guitar,
            Bass,
            Vocals
        }

        public enum PathType
        {
            Lead,
            Rhythm,
            Bass,
        }

        public string Identifier { get; set; }
        public string Name { get; set; }
        public int[] Color { get; set; }  // used in Guitar Pro to distinguish the tracks by color
        public int DifficultyLevel { get; set; }
        public InstrumentType Instrument { get; set; }
        public int NumStrings { get; set; }  // some CDLCs feature Bass tracks with more than 4 strings
        public PathType Path { get; set; }
        public bool Bonus { get; set; }
        // Tuning stored as midi notes for each open string
        public int[] Tuning { get; set; }
        public int Capo { get; set; }
        public List<Bar> Bars { get; set; }
        public Dictionary<int, ChordTemplate> ChordTemplates { get; set; }

        public Single AverageBeatsPerMinute { get; set; }
    }


    /// <summary>
    /// A chord template. Used to generate chord diagrams and also to determine
    /// the actual notes in a chord referencing this template.
    /// </summary>
    public class ChordTemplate
    {
        public string Name { get; set; }
        public int[] Frets { get; set; }
        public int[] Fingers { get; set; }
        public int ChordId { get; set; }
    }


    /// <summary>
    /// A single bar in a track. Specifies time and tempo and contains the actual 
    /// notes in the bar.
    /// </summary>
    public class Bar
    {
        public Bar()
        {
            Chords = new List<Chord>();
            BeatTimes = new List<Single>();
        }

        public int BeatsPerMinute { get; set; }
        public int TimeDenominator { get; set; }
        public int TimeNominator { get; set; }

        public List<Chord> Chords { get; set; }

        // start and end times in Rocksmith
        public Single Start { get; set; }
        public Single End { get; set; }

        public List<Single> BeatTimes { get; set; }

        public bool ContainsTime(Single time)
        {
            return Start <= time && time < End;
        }

        /// <summary>
        /// Approximates the given absolute time length in terms of a note value.
        /// The note duration is represented as an int in multiples of
        /// 1/48 of a quarter note.
        /// </summary>
        public Single GetDuration(Single start, Single length)
        {
            // Since notes can be just a bit offbeat, duration recognition is actually
            // quite tricky. Taking into account the individual sub-beats helps a lot, 
            // since they might relay a bit of information about the amount the beats are
            // off from the metronome. So we split each note into the parts belonging to 
            // one sub-beat and calculate the note duration for that beat individually.
            Single duration = 0;
            for (int i = 0; i < BeatTimes.Count - 1; ++i)
            {
                if (start >= BeatTimes[i + 1])
                    continue;
                if (start + length < BeatTimes[i])
                    break;

                var beatLength = BeatTimes[i+1] - BeatTimes[i];
                var noteStart = Math.Max(start, BeatTimes[i]);
                var noteEnd = Math.Min(start + length, BeatTimes[i+1]);
                var beatDuration = (noteEnd - noteStart) / beatLength * 4 / TimeDenominator;
                duration += beatDuration;
            }

            return duration * 48;
        }

        public Single GetDurationLength(Single start, int duration)
        {
            Single quarterNoteLength = (End - Start) / TimeNominator * TimeDenominator / 4;
            return duration / 48.0f * quarterNoteLength;
        }

        public int GetBeatDuration()
        {
            if (TimeDenominator == 8)
                return 24;
            else
                return 48;
        }

        public int GetBarDuration()
        {
            return GetBeatDuration() * TimeNominator;
        }

        /// <summary>
        /// Requires that Start, End and TimeNominator have been set. Will try to figure out a
        /// fitting TimeDenominator and BPM.
        /// </summary>
        /// <param name="averageBPM">Average BPM in the track.</param>
        public void GuessTimeAndBPM(Single averageBPM)
        {
            var length = End - Start;
            var avgTimePerBeat = length / TimeNominator;
            if (Math.Abs(averageBPM - 60.0 / avgTimePerBeat)
                < Math.Abs(averageBPM - 30.0 / avgTimePerBeat))
            {
                // we are closer to the score's average BPM if we assume each
                // beat in this measure is a quarter note long.
                TimeDenominator = 4;
            }
            else
            {
                // in this case, eighth notes are a better match.
                TimeDenominator = 8;
            }
            // these are all the possibilities we consider. anything else is just too
            // weird, I think.

            BeatsPerMinute = (int)Math.Round(4.0/TimeDenominator * 60.0 / avgTimePerBeat);

        }
    }


    public class Chord
    {
        public Chord()
        {
            Notes = new Dictionary<int, Note>();
            ChordId = -1;
            BrushDirection = BrushType.None;
            Section = null;
        }

        public int ChordId { get; set; }
        // Duration is set as a multiple of 1/48th of a quarter note,
        // j.e., 48 = quarter, 24 = eighth, 16 = eighth triplet etc.
        public int Duration { get; set; }
        // index a note by its string
        public Dictionary<int, Note> Notes { get; set; }
        public bool Tremolo { get; set; }
        public bool Slapped { get; set; }
        public bool Popped { get; set; }
        public string Section { get; set; }

        public enum BrushType
        {
            None,
            Down,
            Up
        }
        public BrushType BrushDirection { get; set; }

        // start time in Rocksmith
        public Single Start { get; set; }
        public Single End { get; set; }
    }


    public class Note
    {
        public Single Start { get; set; }
        public int String { get; set; }
        public int Fret { get; set; }
        public bool PalmMuted { get; set; }
        public bool Muted { get; set; }
        public bool Hopo { get; set; }
        public bool Vibrato { get; set; }
        public bool LinkNext { get; set; }
        public bool Accent { get; set; }
        public bool Harmonic { get; set; }
        public bool PinchHarmonic { get; set; }
        public bool Tremolo { get; set; }
        public bool Tapped { get; set; }
        public bool Slapped { get; set; }
        public bool Popped { get; set; }
        public int LeftFingering { get; set; }
        public int RightFingering { get; set; }
        public float Sustain { get; set; }

        public enum SlideType
        {
            None,
            ToNext,
            UnpitchDown,
            UnpitchUp
        }
        public SlideType Slide { get; set; }
        public int SlideTarget { get; set; }

        public class BendValue
        {
            public float Start;
            public float RelativePosition;
            public float Step;
        }
        public List<BendValue> BendValues { get; set; }


        public bool _Extended;  // internal marker that this note is a linked note created for sustain


        public Note()
        {
            BendValues = new List<BendValue>();
            _Extended = false;
        }
    }
}
