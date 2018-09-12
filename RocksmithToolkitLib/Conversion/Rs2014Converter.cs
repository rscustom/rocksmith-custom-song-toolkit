using System;
using System.Collections.Generic;
using System.Linq;
using RocksmithToolkitLib.SngToTab;
using RocksmithToolkitLib.XML;
using System.IO;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;

namespace RocksmithToolkitLib.Conversion
{
    public class Rs2014Converter : IDisposable
    {
        public void Dispose() { }

        #region Song2014 to XML

        /// <summary>
        /// Convert RS2014 (Song2014) to XML file
        /// </summary>
        /// <param name="rs2014Song"></param>
        /// <param name="outputPath"></param>
        /// <param name="overWrite"></param>
        /// <returns>RS2014 XML file path</returns>
        public string Song2014ToXml(Song2014 rs2014Song, string outputPath, bool overWrite)
        {
            if (File.Exists(outputPath) && overWrite)
                File.Delete(outputPath);
            else
            {
                var outputDir = Path.GetDirectoryName(outputPath);
                var outputFile = String.Format("{0}_{1}", rs2014Song.Title, rs2014Song.Arrangement);
                outputFile = String.Format("{0}{1}", outputFile.ToLower().GetValidFileName(), "_rs2014.xml");
                outputPath = Path.Combine(outputDir, outputFile);
            }

            using (var stream = File.OpenWrite(outputPath))
            {
                rs2014Song.Serialize(stream, false);
            }

            return outputPath;
        }

        #endregion

        #region XML file to Song2014

        /// <summary>
        /// Convert XML file to RS2014 (Song2014)
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns>Song</returns>
        public Song2014 XmlToSong2014(string xmlFilePath)
        {
            Song2014 song = Song2014.LoadFromFile(xmlFilePath);
            return song;
        }

        #endregion

        #region Song2014 to Song

        public Song Song2014ToSong(Song2014 rs2014Song)
        {
            var rs1Song = new Song();
            AddSongMetadata(rs2014Song, rs1Song);
            AddElements(rs2014Song, rs1Song);
            AddDifferences(rs2014Song, rs1Song);

            return rs1Song;
        }

        private void AddSongMetadata(Song2014 rs2014Song, Song rs1Song)
        {
            // for consistency apply old naming method ;(
            rs1Song.Title = String.Format("{0} - {1}", rs2014Song.Title, rs2014Song.Arrangement);
            if (rs2014Song.Part > 1)
                rs1Song.Title = String.Format("{0} {1}", rs1Song.Title, rs2014Song.Part);

            rs1Song.Arrangement = rs2014Song.Arrangement;
            rs1Song.Part = rs2014Song.Part;
            rs1Song.Offset = rs2014Song.Offset;
            rs1Song.SongLength = rs2014Song.SongLength;
            rs1Song.StartBeat = rs2014Song.StartBeat;
            rs1Song.AverageTempo = rs2014Song.AverageTempo;
            rs1Song.Tuning = rs2014Song.Tuning;
            rs1Song.ArtistName = rs2014Song.ArtistName;
            rs1Song.AlbumName = rs2014Song.AlbumName;
            rs1Song.AlbumYear = rs2014Song.AlbumYear;

            // use correct LastConversionDateTime format
            rs1Song.LastConversionDateTime = DateTime.Now.ToString("MM-dd-yy HH:mm");
        }

        private void AddElements(Song2014 rs2014Song, Song rs1Song)
        {
            // these elements have direct mapping
            rs1Song.Phrases = rs2014Song.Phrases;
            rs1Song.LinkedDiffs = rs2014Song.LinkedDiffs;
            rs1Song.PhraseProperties = rs2014Song.PhraseProperties;
            rs1Song.FretHandMuteTemplates = rs2014Song.FretHandMuteTemplates;
            rs1Song.Ebeats = rs2014Song.Ebeats;
            rs1Song.Sections = rs2014Song.Sections;
            rs1Song.Events = rs2014Song.Events;

        }

        private void AddDifferences(Song2014 rs2014Song, Song rs1Song)
        {
            // there are slight mapping differences to account for in some elements
            for (var i = 0; i < rs2014Song.PhraseProperties.Count(); i++)
            {
                // TODO: Verify this element accuracy
                if (rs2014Song.PhraseProperties[i].Redundant == 256)
                    // there may be a second phraseProperties[].redudant 
                    rs1Song.PhraseProperties[i].Redundant = 0;
            }

            // add sufficient elements for phraseIterations
            var phrases = new List<SongPhraseIteration>();
            for (var i = 0; i < rs2014Song.PhraseIterations.Count(); i++)
            {
                phrases.Add(new SongPhraseIteration { PhraseId = i, Time = 0 });
            }

            if (rs2014Song.PhraseIterations.Count() > 0) // pop phraseIterations
            {
                rs1Song.PhraseIterations = phrases.ToArray();

                for (var i = 0; i < rs2014Song.PhraseIterations.Count(); i++)
                {
                    rs1Song.PhraseIterations[i].PhraseId = rs2014Song.PhraseIterations[i].PhraseId;
                    rs1Song.PhraseIterations[i].Time = rs2014Song.PhraseIterations[i].Time;
                }
            }

            // add sufficient elements for chordTemplates
            var chordTemplate = new List<SongChordTemplate>();
            for (var i = 0; i < rs2014Song.ChordTemplates.Count(); i++)
            {
                chordTemplate.Add(new SongChordTemplate
                {
                    ChordName = "C",
                    Finger0 = 0,
                    Finger1 = 0,
                    Finger2 = 0,
                    Finger3 = 0,
                    Finger4 = 0,
                    Finger5 = 0,
                    Fret0 = 0,
                    Fret1 = 0,
                    Fret2 = 0,
                    Fret3 = 0,
                    Fret4 = 0,
                    Fret5 = 0
                });
            }

            if (rs2014Song.ChordTemplates.Count() > 0) // pop chordTemplates
            {
                rs1Song.ChordTemplates = chordTemplate.ToArray();

                for (var i = 0; i < rs2014Song.ChordTemplates.Count(); i++)
                {
                    rs1Song.ChordTemplates[i].ChordName = rs2014Song.ChordTemplates[i].ChordName;
                    rs1Song.ChordTemplates[i].Finger0 = rs2014Song.ChordTemplates[i].Finger0;
                    rs1Song.ChordTemplates[i].Finger1 = rs2014Song.ChordTemplates[i].Finger1;
                    rs1Song.ChordTemplates[i].Finger2 = rs2014Song.ChordTemplates[i].Finger2;
                    rs1Song.ChordTemplates[i].Finger3 = rs2014Song.ChordTemplates[i].Finger3;
                    rs1Song.ChordTemplates[i].Finger4 = rs2014Song.ChordTemplates[i].Finger4;
                    rs1Song.ChordTemplates[i].Finger5 = rs2014Song.ChordTemplates[i].Finger5;
                    rs1Song.ChordTemplates[i].Fret0 = rs2014Song.ChordTemplates[i].Fret0;
                    rs1Song.ChordTemplates[i].Fret1 = rs2014Song.ChordTemplates[i].Fret1;
                    rs1Song.ChordTemplates[i].Fret2 = rs2014Song.ChordTemplates[i].Fret2;
                    rs1Song.ChordTemplates[i].Fret3 = rs2014Song.ChordTemplates[i].Fret3;
                    rs1Song.ChordTemplates[i].Fret4 = rs2014Song.ChordTemplates[i].Fret4;
                    rs1Song.ChordTemplates[i].Fret5 = rs2014Song.ChordTemplates[i].Fret5;
                }
            }

            // add sufficient elements for levels
            var levels = new List<SongLevel>();
            for (var i = 0; i < rs2014Song.Levels.Count(); i++)
            {
                levels.Add(new SongLevel
                {
                    Anchors = new SongAnchor[rs2014Song.Levels[i].Anchors.Length],
                    Chords = new SongChord[rs2014Song.Levels[i].Chords.Length],
                    Difficulty = i,
                    HandShapes = new SongHandShape[rs2014Song.Levels[i].HandShapes.Length],
                    Notes = new SongNote[rs2014Song.Levels[i].Notes.Length]
                });
            }

            if (rs2014Song.Levels.Count() > 0)
            {
                rs1Song.Levels = levels.ToArray();

                var anchors = new List<SongAnchor>();
                for (var i = 0; i < rs2014Song.Levels.Count(); i++)
                {
                    for (var j = 0; j < rs2014Song.Levels[i].Anchors.Count(); j++)
                    {
                        anchors.Add(new SongAnchor
                        {
                            Fret = 0,
                            Time = 0
                        });
                    }
                    rs1Song.Levels[i].Anchors = anchors.ToArray();
                    anchors.Clear();
                }
            }

            var chords = new List<SongChord>();
            for (var i = 0; i < rs2014Song.Levels.Count(); i++)
            {
                for (var j = 0; j < rs2014Song.Levels[i].Chords.Count(); j++)
                {
                    chords.Add(new SongChord
                    {
                        Time = 0,
                        HighDensity = 0x00,
                        Ignore = 0x00,
                        Strum = "down"
                    });
                }

                if (rs2014Song.Levels[i].Chords.Count() > 0)
                {
                    rs1Song.Levels[i].Chords = chords.ToArray();
                    chords.Clear();
                }
            }

            var handShape = new List<SongHandShape>();
            for (var i = 0; i < rs2014Song.Levels.Count(); i++)
            {
                for (var j = 0; j < rs2014Song.Levels[i].HandShapes.Count(); j++)
                {
                    handShape.Add(new SongHandShape
                    {
                        ChordId = 0,
                        StartTime = 0,
                        EndTime = 0
                    });
                }

                if (rs2014Song.Levels[i].HandShapes.Count() > 0)
                {
                    rs1Song.Levels[i].HandShapes = handShape.ToArray();
                    handShape.Clear();
                }
            }

            var notes = new List<SongNote>();
            for (var i = 0; i < rs2014Song.Levels.Count(); i++)
            {
                for (var j = 0; j < rs2014Song.Levels[i].Notes.Count(); j++)
                {
                    notes.Add(new SongNote
                    {
                        Time = 0,
                        Bend = 0,
                        Fret = 0,
                        HammerOn = 0x00,
                        Harmonic = 0x00,
                        Hopo = 0x00,
                        Ignore = 0x00,
                        PalmMute = 0x00,
                        Pluck = 0,
                        PullOff = 0x00,
                        Slap = 0x00,
                        SlideTo = 0,
                        String = 0,
                        Sustain = 0x00,
                        Tremolo = 0x00
                    });
                }

                if (rs2014Song.Levels[i].Notes.Count() > 0)
                {
                    rs1Song.Levels[i].Notes = notes.ToArray();
                    notes.Clear();
                }
            }


            // populate elements
            for (var i = 0; i < rs2014Song.Levels.Count(); i++)
            {
                rs1Song.Levels[i].Difficulty = rs2014Song.Levels[i].Difficulty;

                for (var j = 0; j < rs2014Song.Levels[i].Anchors.Count(); j++)
                {
                    rs1Song.Levels[i].Anchors[j].Time = rs2014Song.Levels[i].Anchors[j].Time;
                    rs1Song.Levels[i].Anchors[j].Fret = rs2014Song.Levels[i].Anchors[j].Fret;
                }

                for (var j = 0; j < rs2014Song.Levels[i].Chords.Count(); j++)
                {
                    rs1Song.Levels[i].Chords[j].Time = rs2014Song.Levels[i].Chords[j].Time;
                    rs1Song.Levels[i].Chords[j].ChordId = rs2014Song.Levels[i].Chords[j].ChordId;
                    rs1Song.Levels[i].Chords[j].HighDensity = rs2014Song.Levels[i].Chords[j].HighDensity;
                    rs1Song.Levels[i].Chords[j].Ignore = rs2014Song.Levels[i].Chords[j].Ignore;
                    rs1Song.Levels[i].Chords[j].Strum = rs2014Song.Levels[i].Chords[j].Strum;
                }

                for (var j = 0; j < rs2014Song.Levels[i].HandShapes.Count(); j++)
                {
                    rs1Song.Levels[i].HandShapes[j].ChordId = rs2014Song.Levels[i].HandShapes[j].ChordId;
                    rs1Song.Levels[i].HandShapes[j].StartTime = rs2014Song.Levels[i].HandShapes[j].StartTime;
                    rs1Song.Levels[i].HandShapes[j].EndTime = rs2014Song.Levels[i].HandShapes[j].EndTime;
                }

                for (var j = 0; j < rs2014Song.Levels[i].Notes.Count(); j++)
                {
                    rs1Song.Levels[i].Notes[j].Time = rs2014Song.Levels[i].Notes[j].Time;
                    rs1Song.Levels[i].Notes[j].Bend = (int)rs2014Song.Levels[i].Notes[j].Bend;
                    rs1Song.Levels[i].Notes[j].Fret = rs2014Song.Levels[i].Notes[j].Fret;
                    rs1Song.Levels[i].Notes[j].HammerOn = rs2014Song.Levels[i].Notes[j].HammerOn;
                    rs1Song.Levels[i].Notes[j].Harmonic = rs2014Song.Levels[i].Notes[j].Harmonic;
                    rs1Song.Levels[i].Notes[j].Hopo = rs2014Song.Levels[i].Notes[j].Hopo;
                    rs1Song.Levels[i].Notes[j].Ignore = rs2014Song.Levels[i].Notes[j].Ignore;
                    rs1Song.Levels[i].Notes[j].PalmMute = rs2014Song.Levels[i].Notes[j].PalmMute;
                    rs1Song.Levels[i].Notes[j].Pluck = rs2014Song.Levels[i].Notes[j].Pluck;
                    rs1Song.Levels[i].Notes[j].PullOff = rs2014Song.Levels[i].Notes[j].PullOff;
                    rs1Song.Levels[i].Notes[j].Slap = rs2014Song.Levels[i].Notes[j].Slap;
                    rs1Song.Levels[i].Notes[j].SlideTo = rs2014Song.Levels[i].Notes[j].SlideTo;
                    rs1Song.Levels[i].Notes[j].String = rs2014Song.Levels[i].Notes[j].String;
                    rs1Song.Levels[i].Notes[j].Sustain = rs2014Song.Levels[i].Notes[j].Sustain;
                    rs1Song.Levels[i].Notes[j].Tremolo = rs2014Song.Levels[i].Notes[j].Tremolo;
                }
            }

        }

        #endregion

        #region Song2014 Xml File to Song XML File

        public string Song2014File2SongFile(string songFilePath, bool overWrite)
        {
            Song song;
            using (var obj = new Rs2014Converter())
                song = obj.Song2014ToSong(Song2014.LoadFromFile(songFilePath));

            if (!overWrite)
            {
                var srcDir = Path.GetDirectoryName(songFilePath);
                var srcName = Path.GetFileNameWithoutExtension(songFilePath);
                var backupSrcPath = String.Format("{0}_{1}.xml", Path.Combine(srcDir, srcName), "RS2014");

                // backup original RS1 file
                File.Copy(songFilePath, backupSrcPath);
            }

            // write converted RS1 file
            using (FileStream stream = new FileStream(songFilePath, FileMode.Create))
                song.Serialize(stream, true);

            return songFilePath;
        }

        #endregion

        #region Song2014 to ASCII Tablature

        /// <summary>
        /// Song2014 to ASCII Tablature
        /// </summary>
        /// <param name="rs2014Song"></param>
        /// <param name="outputDir"></param>
        /// <param name="allDif"></param>
        public void Song2014ToAsciiTab(Song2014 rs2014Song, string outputDir, bool allDif)
        {
            if (rs2014Song == null || String.IsNullOrEmpty(outputDir)) return;
            // convert to Song
            Song rs1Song;
            using (var obj = new Rs2014Converter())
                rs1Song = obj.Song2014ToSong(rs2014Song);
            Console.WriteLine("Converted Song2014 To Song");

            //if (false) // write Xml files for debugging 
            //{
            //    using (Rs2014Converter obj = new Rs2014Converter())
            //        obj.Song2014ToXml(rs2014Song, outputDir, true);
            //    using (Rs1Converter obj = new Rs1Converter())
            //        obj.SongToXml(rs1Song, outputDir, true);
            //}

            // convert to SngFile
            string rs1SngFilePath;
            using (var obj = new Rs1Converter())
                rs1SngFilePath = obj.SongToSngFilePath(rs1Song, outputDir);
            Console.WriteLine("Converted Song To SngFile");

            // convert to AsciiTab
            using (var s2Tab = new Sng2Tab())
                s2Tab.Convert(rs1SngFilePath, outputDir, allDif);
            Console.WriteLine("Converted SngFile To AsciiTab");

            if (File.Exists(rs1SngFilePath)) File.Delete(rs1SngFilePath);
        }

        private string tmpWorkDir
        {
            get { return Path.Combine(Path.GetTempPath()); }
        }

        #endregion

        #region RS2014 Tone to RS1 Tone

        public Tone Tone2014toTone(Tone2014 rs2014Tone)
        {
            Tone tone = new Tone();
            Pedal amp = new Pedal();
            Pedal cabinet = new Pedal();
            Pedal loopPedal = new Pedal();
            Pedal prePedal = new Pedal();
            Pedal postPedal = new Pedal();
            // use Tone Key for better conversion
            tone.Name = rs2014Tone.Name ?? "Default";
            tone.Key = rs2014Tone.Key ?? "DEFAULT";
            tone.Volume = rs2014Tone.Volume;
            tone.IsDLC = true;
            tone.Description = "$[-1] ";

            // setup some possible tone approximation conversions
            // no direct mapping for RS2 -> RS1 Tones
            // so check IEnumerable<ToneDescriptor> List()
            // TODO: figure out better method for tone mapping
            if (tone.Key.ToUpper().Contains("COMBO"))
                tone.Key = "Combo_OD";

            if (tone.Key.ToUpper().Contains("OD"))
            {
                amp.PedalKey = "Cab_4X12_RangerVintageGreen_Tube_Cone";
                cabinet.PedalKey = "Amp_Ranger65";
                prePedal.PedalKey = "Pedal_CustomFuzz";
                prePedal.KnobValues = new Dictionary<string, decimal>(){
                    { "Pedal_CustomFuzz_Fuzz", (decimal) 25.0 }, 
                    { "Pedal_CustomFuzz_Lo", (decimal) 60.0 }, 
                    { "Pedal_CustomFuzz_Hi", (decimal) 0.0 }, 
                    { "Pedal_CustomFuzz_Rectify", (decimal) 1.0 }};

                tone.PedalList = new Dictionary<string, Pedal>()
                {
                    {"Amp", amp},
                    {"Cabinet", cabinet},
                    // pedals have three slots (Pedal3 to Pedal1)
                    // so this is just faking a close approximation of the truth
                    {"PrePedal3", prePedal}
                };
            }
            else if (tone.Key.ToUpper().Contains("LEAD"))
            {
                amp.PedalKey = "Amp_SoloTenor";
                amp.KnobValues = new Dictionary<string, decimal>()
                {
                    {"Amp_SoloTenor_Bass", (decimal) 45.0},
                    {"Amp_SoloTenor_Treble", (decimal) 75.0},
                    {"Amp_SoloTenor_Volume", (decimal) 48.0}
                };

                cabinet.PedalKey = "Cab_1X12_Tenor_Condenser_Cone";
                postPedal.PedalKey = "Pedal_HolyCow";
                postPedal.KnobValues = new Dictionary<string, decimal>()
                {
                    { "Pedal_HolyCow_Mix", (decimal) 32.0},
                    {"Pedal_HolyCow_Reverb", (decimal) 54.0}
                };
                tone.PedalList = new Dictionary<string, Pedal>()
                {
                    {"Amp", amp}, 
                    {"Cabinet", cabinet}, 
                    {"PostPedal3", postPedal}
                };
            }
            else if (tone.Key.ToUpper().Contains("DIS"))
            {
                amp.PedalKey = "Amp_MegaDuel";
                amp.KnobValues = new Dictionary<string, decimal>()
                {
                    { "Amp_MegaDuel_Low", (decimal)  95.0},
                    { "Amp_MegaDuel_Mid", (decimal)  55.0},
                    { "Amp_MegaDuel_Hi", (decimal)  25.0},
                    { "Amp_MegaDuel_Gain", (decimal)  72.0}
                };

                cabinet.PedalKey = "Cab_4X10_Baritone_Condenser_Cone";
                prePedal.PedalKey = "Pedal_EQ6";
                prePedal.KnobValues = new Dictionary<string, decimal>()
                {
                    { "Pedal_EQ6_200", (decimal)  1.0},
                    { "Pedal_EQ6_400", (decimal)  0.0},
                    { "Pedal_EQ6_1600", (decimal)  0.0},
                    { "Pedal_EQ6_3200", (decimal)  -3.0},
                    { "Pedal_EQ6_800", (decimal)  1.0},
                    { "Pedal_EQ6_100", (decimal)  2.0}
                };

                postPedal.PedalKey = "Pedal_EQ7";
                postPedal.KnobValues = new Dictionary<string, decimal>()
                {
                    { "Pedal_EQ7_1600", (decimal)  1.0},
                    { "Pedal_EQ7_800", (decimal)  1.0},
                    { "Pedal_EQ7_200", (decimal)  1.0},
                    { "Pedal_EQ7_3200", (decimal)  -4.0},
                    { "Pedal_EQ7_100", (decimal)  0.0},
                    { "Pedal_EQ7_400", (decimal)  0.0},
                    { "Pedal_EQ7_6400", (decimal)  -6.0}
                };

                tone.PedalList = new Dictionary<string, Pedal>()
                {
                    {"Amp", amp},
                    {"Cabinet", cabinet},
                    {"PrePedal3", prePedal},
                    {"PostPedal3", postPedal}
                };
            }
            else if (tone.Key.ToUpper().Contains("CLEAN"))
            {
                amp.PedalKey = "Amp_Hitone";
                amp.KnobValues = new Dictionary<string, decimal>()
                {
                    { "Amp_Hitone_Bright", (decimal)  65.0},
                    { "Amp_Hitone_Hi", (decimal)  65.0},
                    { "Amp_Hitone_Lo", (decimal)  55.0},
                    { "Amp_Hitone_Volume", (decimal)  90.0}
                };

                cabinet.PedalKey = "Cab_4X12_Hitone_Condenser_Cone";
                tone.PedalList = new Dictionary<string, Pedal>()
                {
                    {"Amp", amp},
                    {"Cabinet", cabinet}
                };
            }
            else if (tone.Key.ToUpper().Contains("ACOU"))
            {
                amp.PedalKey = "Amp_Ranger81";
                amp.KnobValues = new Dictionary<string, decimal>() 
                {
                    {"Amp_Ranger81_Gain", (decimal)  100.0}, 
                    {"Amp_Ranger81_Treble", (decimal)  100.0}, 
                    {"Amp_Ranger81_Bass", (decimal)  84.0},
                    {"Amp_Ranger81_Mid", (decimal)  100.0}
                };

                cabinet.PedalKey = "Cab_4X12_Ranger_Condenser_Cone";
                loopPedal.PedalKey = "Pedal_EQ7";
                loopPedal.KnobValues = new Dictionary<string, decimal>()
                {
                    { "Pedal_EQ7_1600", (decimal)  2.0},
                    { "Pedal_EQ7_800", (decimal)  -2.0},
                    { "Pedal_EQ7_200", (decimal)  2.0},
                    { "Pedal_EQ7_3200", (decimal)  0.0},
                    { "Pedal_EQ7_100", (decimal)  -1.0},
                    { "Pedal_EQ7_400", (decimal)  -1.0},
                    { "Pedal_EQ7_6400", (decimal)  2.0}
                };
                tone.PedalList = new Dictionary<string, Pedal>()
                {
                    {"Amp", amp},
                    {"Cabinet", cabinet},
                    {"LoopPedal3", loopPedal}
                };
            }
            else if (tone.Key.ToUpper().Contains("BASS"))
            {
                amp.PedalKey = "Amp_Bass_NPN500";
                amp.KnobValues = new Dictionary<string, decimal>(){
                    { "Amp_Bass_NPN500_HiBoost", (decimal) 1.0},
                    { "Amp_Bass_NPN500_Volume", (decimal) 25.0},
                    { "Amp_Bass_NPN500_HiMid", (decimal) 60.0},
                    { "Amp_Bass_NPN500_Low", (decimal) 100.0},
                    { "Amp_Bass_NPN500_LoCut", (decimal) 0.0},
                    { "Amp_Bass_NPN500_LoMid", (decimal) 40.0},
                    { "Amp_Bass_NPN500_High", (decimal) 60.0},
                    { "Amp_Bass_NPN500_MidContour", (decimal) 0.0}};

                cabinet.PedalKey = "Cab_Bass_2X10_HF_RE20_Edge";
                prePedal.PedalKey = "Pedal_EQ7";
                prePedal.KnobValues = new Dictionary<string, decimal>(){
                    {  "Pedal_EQ7_1600", (decimal) 4.0},
                    {  "Pedal_EQ7_800", (decimal) 2.0},
                    {  "Pedal_EQ7_200", (decimal) -1.0},
                    {  "Pedal_EQ7_3200", (decimal) 3.0},
                    {  "Pedal_EQ7_100", (decimal) 5.0},
                    {  "Pedal_EQ7_400", (decimal) 0.0},
                    {  "Pedal_EQ7_6400", (decimal) -3.0}};
                tone.PedalList = new Dictionary<string, Pedal>()
                {
                    {"Amp", amp},
                    {"Cabinet", cabinet},
                    // pedals have three slots (Pedal3 to Pedal1)
                    // so this is just faking a close approximation of the truth
                    {"PrePedal3", prePedal}, 
                };
            }
            else // set a default tone
            {
                // this is fix for unknown RS2 CDLC tones
                amp.PedalKey = "Amp_Baritone";
                amp.KnobValues = new Dictionary<string, decimal>(){
                    {"Amp_Baritone_Volume", (decimal) 30.0},
                    {"Amp_Baritone_Bass", (decimal) 60.0},
                    {"Amp_Baritone_Treble", (decimal) 60.0}};
               
                cabinet.PedalKey = "Cab_1X15_Tweed_57_Back";
                tone.PedalList = new Dictionary<string, Pedal>()
                {
                    {"Amp", amp},
                    {"Cabinet", cabinet}
                };
            }


            return tone;
        }

        #endregion


    }
}
