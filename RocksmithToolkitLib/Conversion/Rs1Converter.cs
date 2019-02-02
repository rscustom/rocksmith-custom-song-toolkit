using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.SngToTab;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.XmlRepository;


namespace RocksmithToolkitLib.Conversion
{
    public class Rs1Converter : IDisposable
    {
        #region Song XML file to Song Object
        /// <summary>
        /// Convert XML file to RS1 (Song)
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns>Song</returns>
        public Song XmlToSong(string xmlFilePath)
        {
            Song song = Song.LoadFromFile(xmlFilePath);
            return song;
        }
        #endregion

        #region Song Object to Song Xml file

        /// <summary>
        /// Convert RS1 Song Object to XML file
        /// </summary>
        /// <param name="rsSong"></param>
        /// <param name="outputPath"></param>
        /// <param name="overWrite"></param>
        /// <returns>RS Song XML file path</returns>
        public string SongToXml(Song rsSong, string outputDir, bool overWrite)
        {
            var outputFile = String.Format("{0}_{1}", rsSong.Title, rsSong.Arrangement);
            outputFile = String.Format("{0}{1}", outputFile.GetValidFileName(), "_rs1.xml");
            var outputPath = Path.Combine(outputDir, outputFile);

            if (File.Exists(outputPath) && overWrite)
                File.Delete(outputPath);

            using (var stream = File.OpenWrite(outputPath))
                rsSong.Serialize(stream, true);

            return outputPath;
        }
        #endregion

        #region Song Object to SngFile Object
        /// <summary>
        /// Converts RS1 Song Object to RS1 SngFile Object
        /// </summary>
        /// <param name="rs1Song"></param>
        /// <returns>SngFile</returns>
        public SngFile Song2SngFile(Song rs1Song, string outputDir)
        {
            var rs1SngPath = SongToSngFilePath(rs1Song, outputDir);
            SngFile sngFile = new SngFile(rs1SngPath);
            return sngFile;
        }
        #endregion

        #region Song to SngFilePath
        /// <summary>
        /// Converts RS1 Song Object to *.sng File
        /// </summary>
        /// <param name="rs1Song"></param>
        /// <param name="outputDir"></param>
        /// <returns>Path to binary *.sng file</returns>
        public string SongToSngFilePath(Song rs1Song, string outputDir)
        {
            string rs1XmlPath;
            using (var obj = new Rs1Converter())
                rs1XmlPath = obj.SongToXml(rs1Song, outputDir, true);

            ArrangementType arrangementType;
            if (rs1Song.Arrangement.ToLower() == "bass")
                arrangementType = ArrangementType.Bass;
            else
                arrangementType = ArrangementType.Guitar;

            var sngFilePath = Path.ChangeExtension(rs1XmlPath, ".sng");
            SngFileWriter.Write(rs1XmlPath, sngFilePath, arrangementType, new Platform(GamePlatform.Pc, GameVersion.None));

            if (File.Exists(rs1XmlPath)) File.Delete(rs1XmlPath);

            return sngFilePath;
        }
        #endregion

        #region SngFilePath to ASCII Tablature
        /// <summary>
        /// SngFilePath to ASCII Tablature
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDir"></param>
        /// <param name="allDif"></param>
        public void SngFilePathToAsciiTab(string inputFilePath, string outputDir, bool allDif)
        {
            using (var obj = new Sng2Tab())
                obj.Convert(inputFilePath, outputDir, allDif);
        }
        #endregion

        #region Song to Song2014

        /// <summary>
        /// Convert RS1 Song Object to RS2 Song2014 Object
        /// RS1 to RS2014 Mapping Method
        /// </summary>
        /// <param name="rsSong"></param>
        /// <param name="srcPath"></param>
        /// <returns>Song2014</returns>
        public Song2014 SongToSong2014(Song rsSong)
        {
            // Song to Song2014 Mapping
            Song2014 rsSong2014 = new Song2014();

            // NOTE: better more accurate/complete song info may be parsed and loaded
            // from the RS1 song.manifest.json file using RS1LoadFromFolder method
            rsSong2014.Version = "7";
            rsSong2014.Title = rsSong.Title;
            rsSong2014.Arrangement = rsSong.Arrangement;
            rsSong2014.Part = rsSong.Part;
            rsSong2014.Offset = rsSong.Offset;
            rsSong2014.CentOffset = "0";
            rsSong2014.SongLength = rsSong.SongLength;
            rsSong2014.LastConversionDateTime = DateTime.Now.ToString("MM-dd-yy HH:mm");
            rsSong2014.StartBeat = rsSong.Ebeats[0].Time;
            // if RS1 CDLC Song XML originates from EOF it may
            // already contain AverageTempo otherwise it gets calculated 
            rsSong2014.AverageTempo = rsSong.AverageTempo == 0 ? AverageBPM(rsSong) : rsSong.AverageTempo;
            // tuning parsed from RS1 song.manifest.json file by RS1LoadFromFolder
            rsSong2014.Tuning = rsSong.Tuning == null ? new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 } : rsSong.Tuning;
            rsSong2014.Capo = 0;
            rsSong2014.ArtistName = rsSong.ArtistName;
            rsSong2014.AlbumName = rsSong.AlbumName;
            rsSong2014.AlbumYear = rsSong.AlbumYear;
            rsSong2014.CrowdSpeed = "1";

            // RS1 before Bass Edition did not contain ArrangementProperties
            if (rsSong.ArrangementProperties == null)
                rsSong.ArrangementProperties = new SongArrangementProperties();

            // initialize arrangement properties with zero's
            rsSong2014.ArrangementProperties = new SongArrangementProperties2014
            {
                Represent = rsSong.ArrangementProperties.Represent,
                NonStandardChords = rsSong.ArrangementProperties.NonStandardChords,
                BarreChords = rsSong.ArrangementProperties.BarreChords,
                PowerChords = rsSong.ArrangementProperties.PowerChords,
                DropDPower = rsSong.ArrangementProperties.DropDPower,
                OpenChords = rsSong.ArrangementProperties.OpenChords,
                FingerPicking = rsSong.ArrangementProperties.FingerPicking,
                PickDirection = rsSong.ArrangementProperties.PickDirection,
                DoubleStops = rsSong.ArrangementProperties.DoubleStops,
                PalmMutes = rsSong.ArrangementProperties.PalmMutes,
                Harmonics = rsSong.ArrangementProperties.Harmonics,
                PinchHarmonics = rsSong.ArrangementProperties.PinchHarmonics,
                Hopo = rsSong.ArrangementProperties.Hopo,
                Tremolo = rsSong.ArrangementProperties.Tremolo,
                Slides = rsSong.ArrangementProperties.Slides,
                UnpitchedSlides = rsSong.ArrangementProperties.UnpitchedSlides,
                Bends = rsSong.ArrangementProperties.Bends,
                Tapping = rsSong.ArrangementProperties.Tapping,
                Vibrato = rsSong.ArrangementProperties.Vibrato,
                FretHandMutes = rsSong.ArrangementProperties.FretHandMutes,
                SlapPop = rsSong.ArrangementProperties.SlapPop,
                TwoFingerPicking = rsSong.ArrangementProperties.TwoFingerPicking,
                FifthsAndOctaves = rsSong.ArrangementProperties.FifthsAndOctaves,
                Syncopation = rsSong.ArrangementProperties.Syncopation,
                BassPick = rsSong.ArrangementProperties.BassPick,
                Sustain = rsSong.ArrangementProperties.Sustain
            };

            var tuning = new TuningDefinition();
            var tuningName = tuning.NameFromStrings(rsSong2014.Tuning);
            rsSong2014.ArrangementProperties.StandardTuning = tuningName == "E Standard" ? 1 : 0;
            rsSong2014.ArrangementProperties.Represent = 1;
            rsSong2014.ArrangementProperties.BonusArr = 0;

            // initial SWAG based on RS1 arrangement element           
            rsSong2014.ArrangementProperties.RouteMask = rsSong.Arrangement.ToLower().Contains("lead") ? 1
                : (rsSong.Arrangement.ToLower().Contains("rhythm") ? 2
                : (rsSong.Arrangement.ToLower().Contains("combo") ? 2 // may not always be true 
                : (rsSong.Arrangement.ToLower().Contains("bass") ? 4 : 1))); //  but ok for now
            rsSong2014.ArrangementProperties.PathLead = rsSong2014.ArrangementProperties.RouteMask == 1 ? 1 : 0;
            rsSong2014.ArrangementProperties.PathRhythm = rsSong2014.ArrangementProperties.RouteMask == 1 ? 1 : 0;
            rsSong2014.ArrangementProperties.PathBass = rsSong2014.ArrangementProperties.RouteMask == 1 ? 1 : 0;

            // set tone defaults used to produce RS2014 CDLC
            rsSong2014.ToneBase = "Default";
            rsSong2014.ToneA = "";
            rsSong2014.ToneB = "";
            rsSong2014.ToneC = "";
            rsSong2014.ToneD = "";

            // these elements have direct mappings
            rsSong2014.Phrases = rsSong.Phrases;
            rsSong2014.FretHandMuteTemplates = rsSong.FretHandMuteTemplates;
            rsSong2014.Ebeats = rsSong.Ebeats;
            rsSong2014.Sections = rsSong.Sections;
            rsSong2014.Events = rsSong.Events;
            // these prevent in game hanging
            rsSong2014.LinkedDiffs = new SongLinkedDiff[0];
            rsSong2014.PhraseProperties = new SongPhraseProperty[0];

            // these elements have no direct mapping, processing order is important
            rsSong2014 = ConvertChordTemplates(rsSong, rsSong2014);
            rsSong2014 = ConvertLevels(rsSong, rsSong2014);
            rsSong2014 = ConvertPhraseIterations(rsSong, rsSong2014);
            // these prevent in game hanging
            rsSong2014.Tones = new SongTone2014[0];
            rsSong2014.NewLinkedDiff = new SongNewLinkedDiff[0];
            // tested ... not the source of in game hangs
            // rsSong2014.TranscriptionTrack = TranscriptionTrack2014.GetDefault();

            // tested ... confirmed this is a source of in game hangs
            // check alignment of Sections time with Ebeats first beat of measure time 
            // TODO: use LINQ
            float fbomTime = 0;
            float nfbomTime = 0;
            foreach (var section in rsSong2014.Sections)
            {
                foreach (var ebeat in rsSong2014.Ebeats)
                {
                    // save Ebeats first beat of measure time
                    if (ebeat.Measure != -1)
                        fbomTime = ebeat.Time;
                    else
                        nfbomTime = ebeat.Time;

                    if (section.Name.ToLower().Contains("noguitar") && Math.Abs(ebeat.Time - section.StartTime) < 0.001)
                    {
                        // CRITICAL - fix Section noguitar time (matches EOF output)
                        if (ebeat.Measure != -1)
                        {
                            section.StartTime = nfbomTime;
                            Console.WriteLine("Applied fix to RS1->RS2 Section StartTime for: " + section.Name);
                        }

                        break;
                    }

                    // found a valid Section time
                    if (ebeat.Measure != -1 && Math.Abs(ebeat.Time - section.StartTime) < 0.001)
                        break;

                    // fix invalid Section time
                    if (ebeat.Measure == -1 && ebeat.Time > section.StartTime)
                    {
                        section.StartTime = fbomTime;
                        Console.WriteLine("Applied fix to RS1->RS2 Section StartTime for: " + section.Name);
                        break;
                    }
                }
            }

            return rsSong2014;
        }

        public float AverageBPM(dynamic song)
        {
            // a rough approximation of BPM based on ebeats and time
            float beats = song.Ebeats.Length;
            float endTimeMins = song.Ebeats[song.Ebeats.Length - 1].Time / 60;
            // float endTimeMins = rsSong.SongLength / 60;
            float avgBPM = (float)Math.Round(beats / endTimeMins, 1);

            return avgBPM;
        }

        private Song2014 ConvertChordTemplates(Song rsSong, Song2014 rsSong2014)
        {
            // add chordTemplates elements
            var chordTemplate = new List<SongChordTemplate2014>();
            foreach (var songChordTemplate in rsSong.ChordTemplates)
            {
                // tested ... not the source of game hangs
                //if (String.IsNullOrEmpty(songChordTemplate.ChordName))
                //    continue;

                chordTemplate.Add(new SongChordTemplate2014 { ChordName = songChordTemplate.ChordName, DisplayName = songChordTemplate.ChordName, Finger0 = (sbyte)songChordTemplate.Finger0, Finger1 = (sbyte)songChordTemplate.Finger1, Finger2 = (sbyte)songChordTemplate.Finger2, Finger3 = (sbyte)songChordTemplate.Finger3, Finger4 = (sbyte)songChordTemplate.Finger4, Finger5 = (sbyte)songChordTemplate.Finger5, Fret0 = (sbyte)songChordTemplate.Fret0, Fret1 = (sbyte)songChordTemplate.Fret1, Fret2 = (sbyte)songChordTemplate.Fret2, Fret3 = (sbyte)songChordTemplate.Fret3, Fret4 = (sbyte)songChordTemplate.Fret4, Fret5 = (sbyte)songChordTemplate.Fret5 });
            }

            // tested ... not the source of game hangs
            // get rid of duplicate chords if any
            // chordTemplate = chordTemplate.Distinct().ToList();

            // tested ... could be source of game hangs
            if (rsSong.ChordTemplates == null)
            {
                Console.WriteLine("Applied fix to RS1->RS2 ChordTemplates conversion");
                rsSong2014.ChordTemplates = new SongChordTemplate2014[0];
            }
            else
                rsSong2014.ChordTemplates = chordTemplate.ToArray();

            return rsSong2014;
        }

        private Song2014 ConvertLevels(Song rsSong, Song2014 rsSong2014)
        {
            // add levels elements
            var levels = new List<SongLevel2014>();

            foreach (var songLevel in rsSong.Levels)
            {
                var anchors = new List<SongAnchor2014>();
                var notes = new List<SongNote2014>();
                var chords = new List<SongChord2014>();
                var handShapes = new List<SongHandShape>();

                for (int anchorIndex = 0; anchorIndex < songLevel.Anchors.Length; anchorIndex++)
                {
                    var anchor = songLevel.Anchors[anchorIndex];
                    anchors.Add(new SongAnchor2014 { Fret = anchor.Fret, Time = anchor.Time, Width = 4 });
                }

                for (int noteIndex = 0; noteIndex < songLevel.Notes.Length; noteIndex++)
                {
                    var songNote = songLevel.Notes[noteIndex];
                    notes.Add(GetNoteInfo(songNote));
                }

                for (int chordIndex = 0; chordIndex < songLevel.Chords.Length; chordIndex++)
                {
                    // RS1 does not contain chordNotes so need to make them from chordtemplate
                    List<SongNote2014> chordNotes = new List<SongNote2014>();
                    var zChord = songLevel.Chords[chordIndex];
                    var zChordId = zChord.ChordId;
                    var zChordTemplate = rsSong.ChordTemplates[zChordId];

                    // this is ok no code crash
                    //if (String.IsNullOrEmpty(zChordTemplate.ChordName))
                    //    continue;

                    if (zChordTemplate.Finger0 != -1) // finger > -1 is a string played
                        chordNotes.Add(DecodeChordTemplate(zChord, 0, zChordTemplate.Fret0));

                    if (zChordTemplate.Finger1 != -1)
                        chordNotes.Add(DecodeChordTemplate(zChord, 1, zChordTemplate.Fret1));

                    if (zChordTemplate.Finger2 != -1)
                        chordNotes.Add(DecodeChordTemplate(zChord, 2, zChordTemplate.Fret2));

                    if (zChordTemplate.Finger3 != -1)
                        chordNotes.Add(DecodeChordTemplate(zChord, 3, zChordTemplate.Fret3));

                    if (zChordTemplate.Finger4 != -1)
                        chordNotes.Add(DecodeChordTemplate(zChord, 4, zChordTemplate.Fret4));

                    if (zChordTemplate.Finger5 != -1)
                        chordNotes.Add(DecodeChordTemplate(zChord, 5, zChordTemplate.Fret5));

                    if (chordNotes.Any())
                    {
                        chords.Add(new SongChord2014 { ChordId = zChord.ChordId, ChordNotes = chordNotes.ToArray(), HighDensity = zChord.HighDensity, Ignore = zChord.Ignore, Strum = zChord.Strum, Time = zChord.Time });
                        // add chordNotes to songNotes for compatibility
                        notes.AddRange(chordNotes);
                    }
                }

                // tested ... not the source of game hangs
                // get rid of duplicate notes if any
                // notes = notes.Distinct().ToList();

                for (int shapeIndex = 0; shapeIndex < songLevel.HandShapes.Length; shapeIndex++)
                {
                    var handshape = songLevel.HandShapes[shapeIndex];
                    handShapes.Add(new SongHandShape { ChordId = handshape.ChordId, EndTime = handshape.EndTime, StartTime = handshape.StartTime });
                }

                levels.Add(new SongLevel2014 { Anchors = anchors.ToArray(), Chords = chords.ToArray(), Difficulty = songLevel.Difficulty, HandShapes = handShapes.ToArray(), Notes = notes.ToArray() });
            }

            rsSong2014.Levels = levels.ToArray();

            return rsSong2014;
        }

        private Song2014 ConvertPhraseIterations(Song rsSong, Song2014 rsSong2014)
        {
            var phraseIterations = new List<SongPhraseIteration2014>();
            foreach (var songPhraseIteration in rsSong.PhraseIterations)
            {
                // HeroLevels set to null -> prevent some hangs
                phraseIterations.Add(new SongPhraseIteration2014 { PhraseId = songPhraseIteration.PhraseId, HeroLevels = null, Time = songPhraseIteration.Time });
            }
            rsSong2014.PhraseIterations = phraseIterations.ToArray();

            return rsSong2014;
        }

        private SongNote2014 GetNoteInfo(SongNote songNote)
        {
            SongNote2014 songNote2014 = new SongNote2014();
            songNote2014.Bend = (float)songNote.Bend;

            // tested ... BendValue time causing in game hangs if off by 0.001f
            if (songNote.Bend > 0)
            {
                var bendValues = new List<BendValue>();
                // CRITICAL CALCULATION - DO NOT CHANGE - MULTIPLIER VALUE MUST BE 0.3333 TO ACHEIVE PROPER ACCURACY AND MATCH EOF OUTPUT
                bendValues.Add(new BendValue { Step = songNote.Bend, Time = (float)Math.Round((songNote.Sustain * 0.3333 / songNote.Bend) + songNote.Time, 3), Unk5 = 0 });
                songNote2014.BendValues = bendValues.ToArray();
            }

            // RS1
            // <note time="73.047" bend="0" fret="5" hammerOn="0" harmonic="0" hopo="0" ignore="0" palmMute="0" pullOff="0" slideTo="-1" string="1" sustain="0" tremolo="0"/>
            songNote2014.Time = (float)songNote.Time;
            songNote2014.Bend = (float)songNote.Bend;
            songNote2014.Fret = (sbyte)songNote.Fret;
            songNote2014.HammerOn = (byte)songNote.HammerOn;
            songNote2014.Harmonic = (byte)songNote.Harmonic;
            songNote2014.Hopo = (byte)songNote.Hopo;
            songNote2014.Ignore = (byte)songNote.Ignore;
            songNote2014.PalmMute = (byte)songNote.PalmMute;
            songNote2014.PullOff = (byte)songNote.PullOff;
            songNote2014.SlideTo = (sbyte)songNote.SlideTo;
            songNote2014.String = (byte)songNote.String;
            songNote2014.Sustain = (float)songNote.Sustain;
            songNote2014.Tremolo = (byte)songNote.Tremolo;
            // initialize elements not present in RS1
            songNote2014.Slap = 0; //  EOF is non-compliant     
            songNote2014.Pluck = 0; //  EOF is non-compliant   
            songNote2014.LinkNext = 0;
            songNote2014.Accent = 0;
            songNote2014.LeftHand = -1;
            songNote2014.Mute = 0;
            songNote2014.HarmonicPinch = 0;
            songNote2014.PickDirection = 0;
            songNote2014.RightHand = -1;
            songNote2014.SlideUnpitchTo = -1;
            songNote2014.Tap = 0;
            songNote2014.Vibrato = 0;

            return songNote2014;
        }

        private SongNote2014 DecodeChordTemplate(SongChord songChord, int gString, int fret)
        {
            // RS2014
            //<chord time="83.366" linkNext="0" accent="0" chordId="19" fretHandMute="0" highDensity="0" ignore="0" palmMute="0" hopo="0" strum="down">
            //  <chordNote time="83.366" linkNext="0" accent="0" bend="0" fret="3" hammerOn="0" harmonic="0" hopo="0" ignore="0" leftHand="-1" mute="0" palmMute="0" pluck="-1" pullOff="0" slap="-1" slideTo="-1" string="4" sustain="0.000" tremolo="0" harmonicPinch="0" pickDirection="0" rightHand="-1" slideUnpitchTo="-1" tap="0" vibrato="0"/>
            //  <chordNote time="83.366" linkNext="0" accent="0" bend="0" fret="3" hammerOn="0" harmonic="0" hopo="0" ignore="0" leftHand="-1" mute="0" palmMute="0" pluck="-1" pullOff="0" slap="-1" slideTo="-1" string="5" sustain="0.000" tremolo="0" harmonicPinch="0" pickDirection="0" rightHand="-1" slideUnpitchTo="-1" tap="0" vibrato="0"/>
            //</chord>

            // RS1
            //<chord time="83.366" chordId="1" highDensity="0" ignore="0" strum="down"/>
            //<chordTemplate chordName="A" finger0="-1" finger1="0" finger2="1" finger3="1" finger4="1" finger5="-1" fret0="-1" fret1="0" fret2="2" fret3="2" fret4="2" fret5="-1"/>

            // finger > -1 is actual string

            SongNote2014 songNote2014 = new SongNote2014();
            songNote2014.Time = songChord.Time;
            songNote2014.LinkNext = 0;
            songNote2014.Accent = 0;
            songNote2014.Bend = 0;
            songNote2014.Fret = (sbyte)fret;
            songNote2014.HammerOn = 0;
            songNote2014.Hopo = 0;
            songNote2014.Ignore = songChord.Ignore;
            songNote2014.LeftHand = -1;
            songNote2014.Mute = 0;
            songNote2014.PalmMute = 0;
            songNote2014.Pluck = -1;
            songNote2014.PullOff = 0;
            songNote2014.Slap = -1;
            songNote2014.SlideTo = -1;
            songNote2014.String = (byte)gString;
            songNote2014.Sustain = 0.000f;
            songNote2014.Tremolo = 0;
            songNote2014.HarmonicPinch = 0;
            songNote2014.PickDirection = 0;
            songNote2014.RightHand = -1;
            songNote2014.SlideUnpitchTo = -1;
            songNote2014.Tap = 0;
            songNote2014.Vibrato = 0;

            return songNote2014;
        }
        # endregion

        #region Song Xml File to Song2014 XML File

        public string SongFile2Song2014File(string songFilePath, bool overWrite)
        {
            Song2014 song2014;
            using (var obj = new Rs1Converter())
                song2014 = obj.SongToSong2014(Song.LoadFromFile(songFilePath));

            if (!overWrite)
            {
                var srcDir = Path.GetDirectoryName(songFilePath);
                var srcName = Path.GetFileNameWithoutExtension(songFilePath);
                var backupSrcPath = String.Format("{0}_{1}.xml", Path.Combine(srcDir, srcName), "RS1");

                // backup original RS1 file
                File.Copy(songFilePath, backupSrcPath);
            }

            // write converted RS2014 file
            using (FileStream stream = new FileStream(songFilePath, FileMode.Create))
                song2014.Serialize(stream, true);

            return songFilePath;
        }

        #endregion

        #region RS1 Tone to RS2 Tone2014

        public Tone2014 ToneToTone2014(Tone rs1Tone)
        {
            Tone2014 tone2014 = new Tone2014();
            Pedal2014 amp = new Pedal2014();
            Pedal2014 cabinet = new Pedal2014();
            Pedal2014 prepedal1 = new Pedal2014();
            Pedal2014 rack1 = new Pedal2014();
            Pedal2014 rack2 = new Pedal2014();
            tone2014.ToneDescriptors = new List<string>();
            // use Tone Key for better conversion
            tone2014.Name = rs1Tone.Name ?? "Default";
            tone2014.Key = rs1Tone.Key ?? "DEFAULT";
            tone2014.Volume = rs1Tone.Volume;
            tone2014.IsCustom = true;
            tone2014.NameSeparator = " - ";
            tone2014.SortOrder = 0;

            // setup some possible tone approximation conversions
            // no direct mapping for RS1 -> RS2 Tones
            // so check IEnumerable<ToneDescriptor> List()
            // TODO: figure out better method for tone mapping
            if (tone2014.Key.ToUpper().Contains("COMBO"))
                tone2014.Key = "Combo_OD";

            if (tone2014.Key.ToUpper().Contains("OD"))
            {
                tone2014.ToneDescriptors.Add("$[35716]OVERDRIVE");
                amp.Type = "Amps";
                amp.Category = "Amp";
                amp.PedalKey = "Amp_MarshallJTM45";
                cabinet.Type = "Cabinets";
                cabinet.Category = "Dynamic_Cone";
                cabinet.PedalKey = "Cab_Marshall1936_57_Cone";
                rack1.Type = "Racks";
                rack1.Category = "Filter";
                rack1.PedalKey = "Rack_StudioEQ";
                rack2.Type = "Racks";
                rack2.Category = "Reverb";
                rack2.PedalKey = "Rack_StudioVerb";
                prepedal1.Type = "Pedals";
                prepedal1.Category = "Distortion";
                prepedal1.PedalKey = "Pedal_SuperDrive";

                tone2014.GearList = new Gear2014()
                {
                    Amp = amp,
                    Cabinet = cabinet,
                    Rack1 = rack1,
                    Rack2 = rack2,
                    PrePedal1 = prepedal1
                };
            }
            else if (tone2014.Key.ToUpper().Contains("LEAD"))
            {
                tone2014.ToneDescriptors.Add("$[35724]LEAD");
                amp.Type = "Amps";
                amp.Category = "Amp";
                amp.PedalKey = "Amp_AT120";
                cabinet.Type = "Cabinets";
                cabinet.Category = "Dynamic_Cone";
                cabinet.PedalKey = "Cab_OrangePPC412_57_Cone";
                rack1.Type = "Racks";
                rack1.Category = "Filter";
                rack1.PedalKey = "Rack_StudioEQ";
                prepedal1.Type = "Pedals";
                prepedal1.Category = "Distortion";
                prepedal1.PedalKey = "Pedal_GermaniumDrive";

                tone2014.GearList = new Gear2014()
                {
                    Amp = amp,
                    Cabinet = cabinet,
                    Rack1 = rack1,
                    PrePedal1 = prepedal1
                };
            }
            else if (tone2014.Key.ToUpper().Contains("DIS"))
            {
                tone2014.ToneDescriptors.Add("$[35722]DISTORTION");
                amp.Type = "Amps";
                amp.Category = "Amp";
                amp.PedalKey = "Amp_GB100";
                cabinet.Type = "Cabinets";
                cabinet.Category = "Dynamic_Cone";
                cabinet.PedalKey = "Cab_GB412CMKIII_57_Cone";
                rack1.Type = "Racks";
                rack1.Category = "Filter";
                rack1.PedalKey = "Rack_StudioEQ";
                rack2.Type = "Racks";
                rack2.Category = "Reverb";
                rack2.PedalKey = "Rack_StudioVerb";
                prepedal1.Type = "Pedals";
                prepedal1.Category = "Distortion";
                prepedal1.PedalKey = "Pedal_GermaniumDrive";

                tone2014.GearList = new Gear2014()
                {
                    Amp = amp,
                    Cabinet = cabinet,
                    Rack1 = rack1,
                    Rack2 = rack2,
                    PrePedal1 = prepedal1
                };
            }
            else if (tone2014.Key.ToUpper().Contains("CLEAN"))
            {
                tone2014.ToneDescriptors.Add("$[35720]CLEAN");
                amp.Type = "Amps";
                amp.Category = "Amp";
                amp.PedalKey = "Amp_TW40";
                cabinet.Type = "Cabinets";
                cabinet.Category = "Dynamic_Cone";
                cabinet.PedalKey = "Cab_TW112C_57_Cone";
                rack1.Type = "Racks";
                rack1.Category = "Filter";
                rack1.PedalKey = "Rack_StudioEQ";

                tone2014.GearList = new Gear2014()
                {
                    Amp = amp,
                    Cabinet = cabinet,
                    Rack1 = rack1
                };
            }
            else if (tone2014.Key.ToUpper().Contains("ACOU"))
            {
                tone2014.ToneDescriptors.Add("$[35721]ACOUSTIC");
                amp.Type = "Amps";
                amp.Category = "Amp";
                amp.PedalKey = "Amp_TW40";
                cabinet.Type = "Cabinets";
                cabinet.Category = "Dynamic_Cone";
                cabinet.PedalKey = "Cab_GB412CMKIII_57_Cone";
                rack1.Type = "Racks";
                rack1.Category = "Filter";
                rack1.PedalKey = "Rack_StudioEQ";
                rack2.Type = "Racks";
                rack2.Category = "Dynamics";
                rack2.PedalKey = "Rack_StudioCompressor";
                prepedal1.Type = "Pedals";
                prepedal1.Category = "Filter";
                prepedal1.PedalKey = "Pedal_AcousticEmulator";

                tone2014.GearList = new Gear2014()
                {
                    Amp = amp,
                    Cabinet = cabinet,
                    Rack1 = rack1,
                    Rack2 = rack2,
                    PrePedal1 = prepedal1
                };
            }
            else if (tone2014.Key.ToUpper().Contains("BASS"))
            {
                tone2014.ToneDescriptors.Add("$[35715]BASS");
                amp.Type = "Amps";
                amp.Category = "Amp";
                amp.PedalKey = "Bass_Amp_CH300B";
                cabinet.Type = "Cabinets";
                cabinet.Category = "Dynamic_Cone";
                cabinet.PedalKey = "Bass_Cab_BT410BC_57_Cone";
                rack1.Type = "Racks";
                rack1.Category = "Filter";
                rack1.PedalKey = "Rack_StudioEQ";

                tone2014.GearList = new Gear2014() { Amp = amp, Cabinet = cabinet, Rack1 = rack1 };
            }
            else // default acoustic is better than nothing
            {
                // this is fix for bad RS1 CDLC tones
                tone2014.Key = "DEFAULT";
                //
                tone2014.ToneDescriptors.Add("$[35721]ACOUSTIC");
                amp.Type = "Amps";
                amp.Category = "Amp";
                amp.PedalKey = "Amp_TW40";
                cabinet.Type = "Cabinets";
                cabinet.Category = "Dynamic_Cone";
                cabinet.PedalKey = "Cab_GB412CMKIII_57_Cone";
                rack1.Type = "Racks";
                rack1.Category = "Filter";
                rack1.PedalKey = "Rack_StudioEQ";
                rack2.Type = "Racks";
                rack2.Category = "Dynamics";
                rack2.PedalKey = "Rack_StudioCompressor";
                prepedal1.Type = "Pedals";
                prepedal1.Category = "Filter";
                prepedal1.PedalKey = "Pedal_AcousticEmulator";

                tone2014.GearList = new Gear2014()
                {
                    Amp = amp,
                    Cabinet = cabinet,
                    Rack1 = rack1,
                    Rack2 = rack2,
                    PrePedal1 = prepedal1
                };
            }

            return tone2014;
        }

        #endregion

        public void Dispose() { }

    }
}
