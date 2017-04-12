using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RocksmithToTabLib
{
    public class GP5File
    {
        private const string FILE_VERSION = "FICHIER GUITAR PRO v5.00";
 	    private static readonly string[] PAGE_SETUP_LINES = {
		    "%TITLE%",
		    "%SUBTITLE%",
		    "%ARTIST%",
		    "%ALBUM%",
		    "Words by %WORDS%",
		    "Music by %MUSIC%",
		    "Words & Music by %WORDSMUSIC%",
		    "Copyright %COPYRIGHT%",
		    "All Rights Reserved - International Copyright Secured",
		    "Page %N%/%P%",
    	};


        private BinaryWriter writer = null;
        private Score score = null;
        private List<bool[]> tieNotes = null;
        private List<int> prevChordId = null;
        private List<int> ports = null;
        private List<int> primaryChannels = null;
        private List<int> secondaryChannels = null;
        private Queue<int> channelTracks = null;

        
        public void ExportScore(Score score, string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Create))
            {
                ExportScore(score, file);
            }
        }

        public void ExportScore(Score score, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.GetEncoding(1252)))
            {
                ExportScore(score, writer);
            }

        }
             
        private void ExportScore(Score score, BinaryWriter writer)
        {
            this.writer = writer;
            this.score = score;

            WriteHeader();
            WriteScoreInfo();
            WriteLyrics();
            WritePageSetup();
            WriteTempo();
            WriteKey();
            WriteChannels();
            WriteMusicalDirections();
            writer.Write((Int32)0);  // master reverb setting
            WriteMeasuresAndTracks();

            this.score = null;
            this.writer = null;
        }



        private void WriteHeader()
        {
            writer.Write(FILE_VERSION);
            // need to pad version string to 30 bytes
            for (int i = FILE_VERSION.Length; i < 30; ++i)
                writer.Write((byte)0);
        }


        private void WriteScoreInfo()
        {
            WriteDoublePrefixedString(score.Title);
            WriteDoublePrefixedString("");  // subtitle
            WriteDoublePrefixedString(score.Artist);
            WriteDoublePrefixedString(score.Album);
            WriteDoublePrefixedString(score.Artist);  // words by
            WriteDoublePrefixedString(score.Artist);  // music by
            WriteDoublePrefixedString("");  // copyright
            WriteDoublePrefixedString(score.Tabber);  // tabber
            WriteDoublePrefixedString("");  // instructions
            if (score.Comments == null)
            {
                writer.Write((Int32)1);  // number of comments, followed by comments as strings
                WriteDoublePrefixedString("");
            }
            else
            {
                writer.Write((Int32)score.Comments.Count);
                foreach (var line in score.Comments)
                    WriteDoublePrefixedString(line + "\n");
            }
        }


        private void WriteLyrics()
        {
            // placeholder for now, just write empty data
            writer.Write((Int32)0);  // associated track
            for (int i = 0; i < 5; ++i)  // once for each lyrics line
            {
                writer.Write((Int32)0);  // starting from bar
                WriteIntPrefixedString("");  // lyrics string
            }
        }


        private void WritePageSetup()
        {
            writer.Write((Int32)210);  // page width
            writer.Write((Int32)297);  // page height
            writer.Write((Int32)10);  // left margin
            writer.Write((Int32)10);  // right margin
            writer.Write((Int32)15);  // top margin
            writer.Write((Int32)10);  // bottom margin
            writer.Write((Int32)100);  // score size percentage
            writer.Write((Byte)0xff);  // view flags
            writer.Write((Byte)0x01);  // view flags

            for (int i = 0; i < PAGE_SETUP_LINES.Length; ++i)
            {
                WriteDoublePrefixedString(PAGE_SETUP_LINES[i]);
            }
        }


        private void WriteTempo()
        {
            // first comes a string describing the tempo of the song
            WriteDoublePrefixedString("");
            // then actual BPM
            Int32 avgBPM = (score.Tracks.Count > 0) ? (Int32)score.Tracks[0].AverageBeatsPerMinute : 120;
            writer.Write(avgBPM);
        }


        private void WriteKey()
        {
            // these fields tell the key of the song. Since we don't know that, we just fill
            // them with 0
            writer.Write((Int32)0);
            writer.Write((Byte)0);
        }


        private void WriteChannels()
        {
            // this sets used program and volume / effects on each channel, we just
            // use some default values for guitar and bass
            int curChannel = 0;
            int curPort = 0;
            ports = new List<int>();
            primaryChannels = new List<int>();
            secondaryChannels = new List<int>();
            channelTracks = new Queue<int>();
            for (int i = 0; i < score.Tracks.Count; ++i)
            {
                ports.Add(curPort);
                primaryChannels.Add(curChannel++);
                if (curChannel == 9)  // reserved for drum tracks
                    ++curChannel;
                secondaryChannels.Add(curChannel++);
                if (curChannel == 9)
                    ++curChannel;

                int program = (score.Tracks[i].Instrument == Track.InstrumentType.Bass) ? 0x21 : 0x1d;
                channelTracks.Enqueue(program);
                channelTracks.Enqueue(program);

                if (curChannel >= 15)
                {
                    curChannel = 0;
                    ++curPort;
                }
            }

            for (int i = 0; i < 64; ++i)
            {
                int channel = 0x19;
                if (i % 16 != 9 && channelTracks.Count != 0)
                    channel = channelTracks.Dequeue();
                WriteChannel(channel);
            }
        }

        
        private void WriteChannel(Int32 program)
        {
            writer.Write(program);  // program
            writer.Write((Byte)15);  // volume (from 0 to 16)
            writer.Write((Byte)8);  // pan (from 0 to 16)
            writer.Write((Byte)0);  // chorus
            writer.Write((Byte)0);  // reverb
            writer.Write((Byte)0);  // phaser
            writer.Write((Byte)0);  // tremolo
            writer.Write((Byte)0);  // unused
            writer.Write((Byte)0);  // unused
        }


        private void WriteMusicalDirections()
        {
            // these tell where the musical symbols like code, fine etc. are placed
            // we are not using those, so we set them to unused (0xffff).
            for (int i = 0; i < 38; ++i)
            {
                writer.Write((Byte)0xff);
            }
        }


        private void WriteMeasuresAndTracks()
        {
            // write number of measures and number of tracks
            Int32 numBars = 0;
            if (score.Tracks.Count > 0)
                numBars = score.Tracks[0].Bars.Count;
            writer.Write(numBars);
            writer.Write((Int32)score.Tracks.Count);

            if (score.Tracks.Count > 0)
            {
                WriteMasterBars();

                for (int i = 0; i < score.Tracks.Count; ++i)
                {
                    var track = score.Tracks[i];
                    WriteTrack(track, i);
                }

                // padding
                writer.Write((short)0);

                // now for the actual contents of the measures
                var currentBPM = (int)score.Tracks[0].AverageBeatsPerMinute;
                tieNotes = new List<bool[]>();
                prevChordId = new List<int>();
                foreach (var track in score.Tracks)
                {
                    tieNotes.Add(new bool[] { false, false, false, false, false, false });
                    prevChordId.Add(-1);
                }


                for (int b = 0; b < score.Tracks[0].Bars.Count; ++b)
                {
                    bool changeTempo = (currentBPM != score.Tracks[0].Bars[b].BeatsPerMinute);
                    currentBPM = score.Tracks[0].Bars[b].BeatsPerMinute;
                    for (int i = 0; i < score.Tracks.Count; ++i)
                    {
                        var track = score.Tracks[i];
                        // it can happen that an arrangement has fewer bars than the first one.
                        // This usually only happens in malformed CDLCs. If it does happen, we add
                        // empty bars as necessary.
                        var bar = (b < track.Bars.Count) ? track.Bars[b] : CreateEmptyBar(score.Tracks[0].Bars[b]);
                        WriteBar(bar, i, track.NumStrings, changeTempo);
                        writer.Write((Byte)0);  // padding

                        changeTempo = false;
                    }
                }
            }
        }


        private Bar CreateEmptyBar(Bar template)
        {
            return new Bar();
        }


        private void WriteMasterBars()
        {
            const Byte KEY_CHANGE = 1 << 6;
            const Byte TIME_CHANGE = (1 << 0) | (1 << 1);
            var bars = score.Tracks[0].Bars;
            int timeNom = 0;
            int timeDenom = 0;
            for (int i = 0; i < bars.Count; ++i)
            {
                var bar = bars[i];

                if (i > 0)
                    writer.Write((Byte)0);  // padding between bars

                Byte flags = 0;
                if (bar == bars.First())
                    flags |= KEY_CHANGE;
                if (timeNom != bar.TimeNominator || timeDenom != bar.TimeDenominator)
                    flags |= TIME_CHANGE;
                timeNom = bar.TimeNominator;
                timeDenom = bar.TimeDenominator;

                writer.Write(flags);
                if ((flags & TIME_CHANGE) != 0)
                {
                    writer.Write((Byte)timeNom);
                    writer.Write((Byte)timeDenom);
                }
                if ((flags & KEY_CHANGE) != 0)
                {
                    // first bar needs to define a key signature. since we don't know that,
                    // we'll just set a default
                    writer.Write((short)0);
                }
                if ((flags & TIME_CHANGE) != 0)
                {
                    // write beam eighth notes
                    int eighthsInDenominator = 8 / timeDenom;
                    int total = eighthsInDenominator * timeNom;
                    Byte val = (Byte)(total / 4);
                    Byte missing = (Byte)(total - 4 * val);
                    Byte[] output = new Byte[] { val, val, val, val };
                    if (missing > 0)
                        output[0] += missing;

                    writer.Write(output);
                }

                writer.Write((Byte)0);  // triplet feel == NONE
                writer.Write((Byte)0);  // padding
            }
        }


        private void WriteTrack(Track track, int trackNumber)
        {
            Byte flags = 0;
            writer.Write(flags);
            writer.Write((Byte)(8 | flags));
            // track name padded to 40 bytes
            var trackName = track.Name; // +" Level " + track.DifficultyLevel;
            trackName = trackName.Substring(0, Math.Min(40, trackName.Length));
            writer.Write(trackName);
            for (int i = trackName.Length; i < 40; ++i)
                writer.Write((Byte)0);

            // tuning information
            int numStrings = track.NumStrings;
            writer.Write(numStrings);
            // apparently, we need to transpose bass tunings one octave down
            int tuningOffset = (track.Instrument == Track.InstrumentType.Bass) ? 12 : 0;
            for (int i = numStrings - 1; i >= 0; --i)
                writer.Write(track.Tuning[i] - tuningOffset);
            for (int i = numStrings; i < 7; ++i)
                writer.Write((UInt32)0xffffffff);  // padding to fill up to 7 strings

            // MIDI channel information
            writer.Write((Int32)(ports[trackNumber] + 1));  // port
            writer.Write((Int32)(primaryChannels[trackNumber] + 1));  // primary channel
            writer.Write((Int32)(secondaryChannels[trackNumber] + 1));  // secondary channel

            // number of frets, just set to 24 to be safe
            writer.Write((Int32)24);
            // capo position
            writer.Write((Int32)track.Capo);
            // track color in RGB0
            writer.Write((Byte)track.Color[0]);
            writer.Write((Byte)track.Color[1]);
            writer.Write((Byte)track.Color[2]);
            writer.Write((Byte)0);

            // unknown byte sequence, taken from TuxGuitar
            Byte[] fillData = new Byte[]{ 67, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 255, 3, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            writer.Write(fillData);
        }


        private void WriteBar(Bar bar, int trackNumber, int numStrings, bool changeTempo)
        {
            writer.Write((Int32)bar.Chords.Count);
            for (int i = 0; i < bar.Chords.Count; ++i)
            {
                var chord = bar.Chords[i];
                WriteBeat(chord, trackNumber, numStrings, changeTempo && (i == 0), bar.BeatsPerMinute);
            }

            // we also need to provide the second voice, however in our case it's going 
            // to be empty
            writer.Write((Int32)0);
        }


        private void WriteBeat(Chord chord, int trackNumber, int numStrings, bool changeTempo, int newTempo)
        {
            const Byte DOTTED_NOTE = 1;
            const Byte CHORD_DIAGRAM = (1 << 1);
            const Byte TEXT = (1 << 2);
            const Byte BEAT_EFFECTS = (1 << 3);
            const Byte MIX_TABLE = (1 << 4);
            const Byte TUPLET = (1 << 5);
            const Byte REST = (1 << 6);
            const short STRING_EFFECTS = (1 << 5);
            const Byte TAPPING = 1;
            const Byte SLAPPING = 2;
            const Byte POPPING = 3;

            // figure out beat duration
            bool dotted = false;
            bool triplet = false;
            SByte duration = 0;

            switch (chord.Duration)
            {
                case 192:
                    duration = -2;
                    break;
                case 144:
                    duration = -1;
                    dotted = true;
                    break;
                case 96:
                    duration = -1;
                    break;
                case 72:
                    duration = 0;
                    dotted = true;
                    break;
                case 48:
                    duration = 0;
                    break;
                case 36:
                    duration = 1;
                    dotted = true;
                    break;
                case 32:
                    duration = 0;
                    triplet = true;
                    break;
                case 24:
                    duration = 1;
                    break;
                case 18:
                    duration = 2;
                    dotted = true;
                    break;
                case 16:
                    duration = 1;
                    triplet = true;
                    break;
                case 12:
                    duration = 2;
                    break;
                case 9:
                    duration = 3;
                    dotted = true;
                    break;
                case 8:
                    duration = 2;
                    triplet = true;
                    break;
                case 6:
                    duration = 3;
                    break;
                case 4:
                    duration = 3;
                    triplet = true;
                    break;
                case 3:
                    duration = 4;
                    break;
                case 2:
                    duration = 4;
                    triplet = true;
                    break;
                default:
                    Console.WriteLine("  Warning: Rhythm Duration {0} not handled, defaulting to quarter note.", chord.Duration);
                    duration = 0;
                    break;
            }

            Byte flags = 0;
            if (chord.Notes.Count == 0)
                flags |= REST;
            if (dotted)
                flags |= DOTTED_NOTE;
            if (triplet)
                flags |= TUPLET;
            if (changeTempo)
                flags |= MIX_TABLE;
            if (chord.Section != null)
                flags |= TEXT;
            bool tapped = false;
            foreach (var kvp in chord.Notes)
            {
                if (kvp.Value.Tapped)
                    tapped = true;
            }
            if (chord.Popped || chord.Slapped || tapped)
                flags |= BEAT_EFFECTS;

            var chordTemplates = score.Tracks[trackNumber].ChordTemplates;
            if (chord.ChordId != -1 && chord.ChordId != prevChordId[trackNumber] &&
                chordTemplates.ContainsKey(chord.ChordId) &&
                chordTemplates[chord.ChordId].Name != string.Empty)
            {
                flags |= CHORD_DIAGRAM;
            }
            prevChordId[trackNumber] = chord.ChordId;

            writer.Write(flags);
            if (chord.Notes.Count == 0)
                writer.Write((Byte)2);  // 2 is an actual rest, 0 is silent
            writer.Write(duration);
            if (triplet)
                writer.Write((Int32)3);  // declare a triplet beat

            // chord diagram
            if ((flags & CHORD_DIAGRAM) != 0)
            {
                var chordTemplate = chordTemplates[chord.ChordId];
                WriteChordTemplate(chordTemplate);
            }
            // section names
            if ((flags & TEXT) != 0)
                WriteDoublePrefixedString(chord.Section);

            // beat effects
            if ((flags & BEAT_EFFECTS) != 0)
            {
                short effectsFlag = 0;
                if (chord.Popped || chord.Slapped || tapped)
                    effectsFlag |= STRING_EFFECTS;
                writer.Write(effectsFlag);
                if (tapped)
                    writer.Write(TAPPING);
                else if (chord.Slapped)
                    writer.Write(SLAPPING);
                else if (chord.Popped)
                    writer.Write(POPPING);
            }

            // mix table (used for changing tempo)
            if ((flags & MIX_TABLE) != 0)
            {
                for (int i = 0; i < 23; ++i)
                    writer.Write((Byte)0xff);
                WriteDoublePrefixedString("");  // tempo string
                writer.Write((Int32)newTempo);
                writer.Write((Byte)0);  // means new tempo takes effect immediately
                writer.Write((Byte)1);
                writer.Write((Byte)0xff);
            }

            // now write the actual notes. a flag indicates which strings are being played
            Byte stringFlags = 0;
            int stringOffset = 7 - numStrings;
            foreach (var kvp in chord.Notes)
                stringFlags |= (Byte)(1 << (kvp.Key+stringOffset));
            writer.Write(stringFlags);
            var notes = chord.Notes.Values.OrderByDescending(x => x.String);
            foreach (var note in notes)
            {
                WriteNote(note, trackNumber);
            }
            // there seem to be a few accidental ties set in the Rocksmith XMLs
            // so unset the tie status on any strings that weren't in the current chord.
            for (int i = 0; i < 6; ++i)
            {
                if (!chord.Notes.ContainsKey(i))
                    tieNotes[trackNumber][i] = false;
            }

            short noteTranspose = 0; 
            writer.Write(noteTranspose);
        }


        private void WriteChordTemplate(ChordTemplate template)
        {
            // basic default options (don't need to mess with this)
            writer.Write(new Byte[] { 1, 1, 0, 0, 0, 12, 0, 0, 255, 255, 255, 255, 0, 0, 0, 0, 0 });
            // chord name padded to 20 bytes
            var chordName = template.Name.Substring(0, Math.Min(20, template.Name.Length));
            writer.Write(chordName);
            for (int i = chordName.Length; i < 20; ++i)
                writer.Write((Byte)0);
            writer.Write((short)0);  // padding
            writer.Write((short)0);  // tonality of ninth/fifth
            writer.Write((Byte)0);  // tonality of eleventh
            
            // we need to determine at what base fret to start the chord diagram
            int minFret = 100;
            int maxFret = 0;
            for (int i = 0; i < 6; ++i)
            {
                if (template.Frets[i] > 0)
                {
                    minFret = Math.Min(template.Frets[i], minFret);
                    maxFret = Math.Max(template.Frets[i], maxFret);
                }
            }
            if (maxFret <= 5)
                minFret = 1; 
            writer.Write((Int32)minFret);
            // write the frets for each string
            for (int i = 5; i >= 0; --i)
                writer.Write((Int32)template.Frets[i]);
            writer.Write((Int32)(-1));  // 7-th string, not used
            
            //for (int i = 0; i < 32; ++i)
            //    writer.Write((Byte)0);
            // barre definitions
            writer.Write((Byte)0);
            for (int i = 0; i < 5; ++i)
            {
                writer.Write((Byte)0);
                writer.Write((Byte)0);
                writer.Write((Byte)0);
            }
            
            // whether the chord contains certain intervals, irrelevant
            writer.Write((Int32)0);
            writer.Write((Int32)0);
            
            // finger positions
            for (int i = 5; i >= 0; --i)
                writer.Write((Byte)template.Fingers[i]);
            writer.Write((Byte)255);  // 7-th string, not used
            writer.Write((Byte)1);  // display fingerings
        }


        private void WriteNote(Note note, int trackNumber)
        {
            const Byte NOTE_TYPE = (1 << 4);
            const Byte NOTE_DYNAMICS = (1 << 5);
            const Byte NOTE_EFFECTS = (1 << 3);
            const Byte FINGER_HINTS = (1 << 7);
            const Byte ACCENTUATED = (1 << 6);
            const Byte TYPE_NORMAL = 1;
            const Byte TYPE_TIED = 2;
            const Byte TYPE_DEAD = 3;
            const short BEND = 1;
            const short HOPO = (1 << 1);
            const short PALM_MUTE = (1 << 9);
            const short TREMOLO = (1 << 10);
            const short SLIDE = (1 << 11);
            const short HARMONIC = (1 << 12);
            const short VIBRATO = (1 << 14);
            const Byte NATURAL_HARMONIC = 1;
            const Byte PINCH_HARMONIC = 4;
            Byte flags = NOTE_TYPE | NOTE_DYNAMICS;

            if (note.LeftFingering >= 0 || note.RightFingering >= 0)
                flags |= FINGER_HINTS;
            if (note.Tremolo || note.Hopo || note.Slide != Note.SlideType.None)
                flags |= NOTE_EFFECTS;
            if (note.Harmonic || note.Vibrato || note.PalmMuted)
                flags |= NOTE_EFFECTS;
            if (note.Accent)
                flags |= ACCENTUATED;

            bool bend = false;
            foreach (var bendValue in note.BendValues)
            {
                // some notes are assigned non-sensical bend values of 0, we need to filter those out,
                // they look ugly in Guitar Pro otherwise.
                if (bendValue.Step > 0)
                    bend = true;
            }
            if (bend)
                flags |= NOTE_EFFECTS;

            writer.Write(flags);
            // first: note type
            if (note.Muted)
                writer.Write(TYPE_DEAD);
            else if (tieNotes[trackNumber][note.String])
                writer.Write(TYPE_TIED);
            else
                writer.Write((Byte)TYPE_NORMAL);
            tieNotes[trackNumber][note.String] = note.LinkNext && note.Slide == Note.SlideType.None;

            // dynamics
            int accent = (note.Accent) ? 1 : 0;
            writer.Write((Byte)(5 + accent));  // mezzo-forte

            writer.Write((Byte)note.Fret);

            if ((flags & FINGER_HINTS) != 0)
            {
                writer.Write((Byte)note.LeftFingering);
                writer.Write((Byte)note.RightFingering);
            }

            writer.Write((Byte)0);  // padding

            if ((flags & NOTE_EFFECTS) != 0)
            { 
                short effectFlags = 0;
                if (note.Tremolo)
                    effectFlags |= TREMOLO;
                if (note.Slide != Note.SlideType.None)
                    effectFlags |= SLIDE;
                if (note.Hopo)
                    effectFlags |= HOPO;
                if (note.Harmonic || note.PinchHarmonic)
                    effectFlags |= HARMONIC;
                if (note.Vibrato)
                    effectFlags |= VIBRATO;
                if (note.PalmMuted)
                    effectFlags |= PALM_MUTE;
                if (bend)
                    effectFlags |= BEND;

                writer.Write(effectFlags);

                if (bend)
                    WriteBend(note.BendValues, note.Vibrato);

                if (note.Tremolo)
                    writer.Write((Byte)2);  // picking speed for tremolo, can be 3, 2 or 1.
                if (note.Slide == Note.SlideType.ToNext)
                    writer.Write((Byte)2);  // legato slide to next note
                else if (note.Slide == Note.SlideType.UnpitchDown)
                    writer.Write((Byte)4);  // slide out downwards
                else if (note.Slide == Note.SlideType.UnpitchUp)
                    writer.Write((Byte)8);  // slide out upwards
                if (note.Harmonic)
                    writer.Write(NATURAL_HARMONIC);
                else if (note.PinchHarmonic)
                    writer.Write(PINCH_HARMONIC);
            }

        }


        private void WriteBend(List<Note.BendValue> bendValues, bool vibrato)
        {
            writer.Write((Byte)2);  // bend type; irrelevant, as it will be overwritten by the actual bend points
            writer.Write((Int32)0);  // max bend height; again, irrelevant

            // TODO: Guitar Pro might only support up to 30 bend points, do we need to check this?
            writer.Write((Int32)bendValues.Count);
            foreach (var bendValue in bendValues)
            {
                Int32 position = (Int32)(bendValue.RelativePosition * 60);
                // GP5 seems to have trouble if step is not a multiple of 25 (quarter bend), or at least
                // this helped prevent a few crashes with bends.
                // although I wonder why they even use a range from 0 to 100 if that is the case?
                Int32 step = (Int32)(Math.Round(bendValue.Step * 2) * 25);
                writer.Write(position);
                writer.Write(step);
                writer.Write((Byte)0); //(vibrato ? 2 : 0));
            }
        }




        private void WriteIntPrefixedString(string text)
        {
            writer.Write((Int32)text.Length);
            writer.Write(text.ToArray());
        }
        private void WriteDoublePrefixedString(string text)
        {
            // GP5 has a weird habit of doubly prefixing strings.
            if (text != null)
            {
                writer.Write((Int32)text.Length + 1);
                writer.Write(text);
            }
            else
            {
                writer.Write((Int32)1);
                writer.Write("");
            }
        }
    }
}
