using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Header;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Functions
{
    public class ManifestFunctions
    {
        private Dictionary<string, string> SectionUINames { get; set; }

        public ManifestFunctions(GameVersion gameVersion)
        {
            switch (gameVersion)
            {
                case GameVersion.RS2012:
                    SectionUINames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    SectionUINames.Add("intro", "$[6005] Intro [1]");
                    SectionUINames.Add("outro", "$[6006] Outro [1]");
                    SectionUINames.Add("verse", "$[6007] Verse [1]");
                    SectionUINames.Add("chorus", "$[6008] Chorus [1]");
                    SectionUINames.Add("bridge", "$[6009] Bridge [1]");
                    SectionUINames.Add("solo", "$[6010] Solo [1]");
                    SectionUINames.Add("ambient", "$[6011] Ambient [1]");
                    SectionUINames.Add("breakdown", "$[6012] Breakdown [1]");
                    SectionUINames.Add("interlude", "$[6013] Interlude [1]");
                    SectionUINames.Add("prechorus", "$[6014] Pre Chorus [1]");
                    SectionUINames.Add("transition", "$[6015] Transition [1]");
                    SectionUINames.Add("postchorus", "$[6016] Post Chorus [1]");
                    SectionUINames.Add("hook", "$[6017] Hook [1]");
                    SectionUINames.Add("riff", "$[6018] Riff [1]");
                    SectionUINames.Add("fadein", "$[6077] Fade In [1]");
                    SectionUINames.Add("fadeout", "$[6078] Fade Out [1]");
                    SectionUINames.Add("buildup", "$[6079] Buildup [1]");
                    SectionUINames.Add("preverse", "$[6080] Pre Verse [1]");
                    SectionUINames.Add("modverse", "$[6081] Modulated Verse [1]");
                    SectionUINames.Add("postvs", "$[6082] Post Verse [1]");
                    SectionUINames.Add("variation", "$[6083] Variation [1]");
                    SectionUINames.Add("modchorus", "$[6084] Modulated Chorus [1]");
                    SectionUINames.Add("head", "$[6085] Head [1]");
                    SectionUINames.Add("modbridge", "$[6086] Modulated Bridge [1]");
                    SectionUINames.Add("melody", "$[6087] Melody [1]");
                    SectionUINames.Add("postbrdg", "$[6088] Post Bridge [1]");
                    SectionUINames.Add("prebrdg", "$[6089] Pre Bridge [1]");
                    SectionUINames.Add("vamp", "$[6090] Vamp [1]");
                    SectionUINames.Add("noguitar", "$[6091] No Guitar [1]");
                    SectionUINames.Add("silence", "$[6092] Silence [1]");
                    break;
                case GameVersion.RS2014:
                    SectionUINames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    SectionUINames.Add("fadein", "$[34276] Fade In [1]");
                    SectionUINames.Add("fadeout", "$[34277] Fade Out [1]");
                    SectionUINames.Add("buildup", "$[34278] Buildup [1]");
                    SectionUINames.Add("chorus", "$[34279] Chorus [1]");
                    SectionUINames.Add("hook", "$[34280] Hook [1]");
                    SectionUINames.Add("head", "$[34281] Head [1]");
                    SectionUINames.Add("bridge", "$[34282] Bridge [1]");
                    SectionUINames.Add("ambient", "$[34283] Ambient [1]");
                    SectionUINames.Add("breakdown", "$[34284] Breakdown [1]");
                    SectionUINames.Add("interlude", "$[34285] Interlude [1]");
                    SectionUINames.Add("intro", "$[34286] Intro [1]");
                    SectionUINames.Add("melody", "$[34287] Melody [1]");
                    SectionUINames.Add("modbridge", "$[34288] Modulated Bridge [1]");
                    SectionUINames.Add("modchorus", "$[34289] Modulated Chorus [1]");
                    SectionUINames.Add("modverse", "$[34290] Modulated Verse [1]");
                    SectionUINames.Add("outro", "$[34291] Outro [1]");
                    SectionUINames.Add("postbrdg", "$[34292] Post Bridge [1]");
                    SectionUINames.Add("postchorus", "$[34293] Post Chorus [1]");
                    SectionUINames.Add("postvs", "$[34294] Post Verse [1]");
                    SectionUINames.Add("prebrdg", "$[34295] Pre Bridge [1]");
                    SectionUINames.Add("prechorus", "$[34296] Pre Chorus [1]");
                    SectionUINames.Add("preverse", "$[34297] Pre Verse [1]");
                    SectionUINames.Add("riff", "$[34298] Riff [1]");
                    SectionUINames.Add("rifff", "$[34298] Riff [1]"); //incorrect name in some adverse cases
                    SectionUINames.Add("silence", "$[34299] Silence [1]");
                    SectionUINames.Add("shifts", "$[34308] Shifts [1]");
                    SectionUINames.Add("slides", "$[35872] Slides [1]");
                    SectionUINames.Add("solo", "$[34300] Solo [1]");
                    SectionUINames.Add("tapping", "$[34305] Tapping [1]");
                    SectionUINames.Add("taps", "$[34313] Taps [1]");
                    SectionUINames.Add("transition", "$[34301] Transition [1]");
                    SectionUINames.Add("vamp", "$[34302] Vamp [1]");
                    SectionUINames.Add("variation", "$[34303] Variation [1]");
                    SectionUINames.Add("verse", "$[34304] Verse [1]");
                    SectionUINames.Add("noguitar", "$[6091] No Guitar [1]");
                    break;
            }
        }

        public void GenerateTechniques(Attributes2014 attribute, Song2014 song)
        {
            // results from this method do not match ODLC but are workable
            //
            //"Techniques" : {
            //     "DiffLevelID" : {//used to display which techs are set at current lvl.
            //         "SectionNumber" : [// > 0
            //             TechID, //required base tech for extended tech(?)
            //             TechID
            //         ]
            //     },
            // }

            if (song.Sections == null)
                return;

            attribute.Techniques = new Dictionary<string, Dictionary<string, List<int>>>();
            for (int difficulty = 0; difficulty < song.Levels.Length; difficulty++)
            {
                var notes = song.Levels[difficulty].Notes;
                var sectionId = new Dictionary<string, List<int>>();
                var techId = new List<int>();

                for (int section = 0; section < song.Sections.Length; section++)
                {
                    var sectionNumber = song.Sections[section].Number;
                    var starTime = song.Sections[section].StartTime;
                    var endTime = song.Sections[Math.Min(section + 1, song.Sections.Length - 1)].StartTime;

                    // iterate through notes in section in the difficulty level
                    foreach (var note in notes)
                    {
                        if (note.Time >= starTime && note.Time < endTime) //in range
                        {
                            var noteTech = getNoteTech(note); // needs tweaking
                            techId.AddRange(noteTech);
                        }
                    }

                    if (techId.Count > 0)
                    {
                        // TODO: needs more tweaking
                        //  techId.Add(35); // try adding dumby data for now
                        List<int> distinctTechIds = techId.Distinct().OrderBy(x => x).ToList();
                        // sometimes sectionNumbers are not unique so duplicate key throws an error if not checked
                        if (sectionId.ContainsKey(sectionNumber.ToString()))
                        {
                            // get the current values and make sure all combined values are distinct
                            var techIdValue = sectionId[sectionNumber.ToString()];
                            techIdValue.AddRange(distinctTechIds);
                            distinctTechIds = techIdValue.Distinct().OrderBy(x => x).ToList();
                            sectionId.Remove(sectionNumber.ToString());
                        }

                        sectionId.Add(sectionNumber.ToString(), distinctTechIds);
                    }

                    techId = new List<int>();
                }
                /*
                "5": {
                   "1": [
                       13,
                       35 <- missing
                       ],
               */

                if (sectionId.Keys.Count > 0)
                    attribute.Techniques.Add(difficulty.ToString(), sectionId);
            }
        }

        static List<int> getNoteTech(SongNote2014 n)
        {
            // TODO: adjust these values

            var t = new List<int>();
            if (1 == n.Accent)
                t.Add(0);
            if (0 != n.Bend)
                t.Add(1);
            if (1 == n.Mute)
                t.Add(2);
            if (1 == n.HammerOn)
                t.Add(3);
            if (1 == n.Harmonic)
                t.Add(4);
            if (1 == n.HarmonicPinch)
                t.Add(5);
            if (1 == n.Hopo)
                t.Add(6);
            if (1 == n.PalmMute)
                t.Add(7);
            if (1 == n.Pluck)
                t.Add(8);
            if (1 == n.PullOff)
                t.Add(9);
            if (1 == n.Slap)
                t.Add(10);
            if (n.SlideTo > 0)
                t.Add(11);
            if (n.SlideUnpitchTo > 0)
                t.Add(12);
            if (n.Sustain > 0)
                t.Add(13);
            if (1 == n.Tap)
                t.Add(14);
            if (1 == n.Tremolo)
                t.Add(15);
            if (1 == n.Vibrato)
                t.Add(16);

            // TODO: determine other dependencies

            return t;
        }

        public void GeneratePhraseIterationsData(IAttributes attribute, dynamic song, GameVersion gameVersion)
        {
            if (song.PhraseIterations == null)
                return;

            for (int i = 0; i < song.PhraseIterations.Length; i++)
            {
                var phraseIteration = song.PhraseIterations[i];
                var phrase = song.Phrases[phraseIteration.PhraseId];
                var endTime = i >= song.PhraseIterations.Length - 1 ? song.SongLength : song.PhraseIterations[i + 1].Time;

                var phraseIt = new PhraseIteration();
                phraseIt.StartTime = phraseIteration.Time;
                phraseIt.EndTime = endTime;
                phraseIt.PhraseIndex = phraseIteration.PhraseId;
                phraseIt.Name = phrase.Name;
                phraseIt.MaxDifficulty = phrase.MaxDifficulty;

                if (gameVersion == GameVersion.RS2012)
                    phraseIt.MaxScorePerDifficulty = new List<float>();

                attribute.PhraseIterations.Add(phraseIt);
            }

            var noteCnt = 0;
            foreach (var y in attribute.PhraseIterations)
            {
                if (song.Levels[y.MaxDifficulty].Notes != null)
                {
                    if (gameVersion == GameVersion.RS2012)
                        noteCnt += GetNoteCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                    else
                        noteCnt += GetNoteCount2014(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                }
                if (song.Levels[y.MaxDifficulty].Chords != null)
                {
                    noteCnt += GetChordCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Chords);
                }
            }

            attribute.Score_MaxNotes = noteCnt;
            attribute.Score_PNV = ((float)attribute.TargetScore) / noteCnt;

            foreach (var y in attribute.PhraseIterations)
            {
                var phrase = song.Phrases[y.PhraseIndex];
                for (int ndx = 0; ndx <= phrase.MaxDifficulty; ndx++)
                {
                    var multiplier = ((float)(ndx + 1)) / (phrase.MaxDifficulty + 1);
                    var pnv = attribute.Score_PNV;
                    var noteCount = 0;

                    if (song.Levels[ndx].Chords != null)
                    {
                        if (gameVersion == GameVersion.RS2012)
                            noteCnt += GetNoteCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                        else
                            noteCnt += GetNoteCount2014(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                    }

                    if (song.Levels[ndx].Chords != null)
                        noteCount += GetChordCount(y.StartTime, y.EndTime, song.Levels[ndx].Chords);

                    if (gameVersion == GameVersion.RS2012)
                    {
                        var score = pnv * noteCount * multiplier;
                        y.MaxScorePerDifficulty.Add(score);
                    }
                }
            }
        }

        public static void GetSongDifficulty(AttributesHeader2014 attribute, Song2014 song)
        {
            var easyArray = new List<int>();
            var mediumArray = new List<int>();
            var hardArray = new List<int>();

            for (int i = 0; i < song.PhraseIterations.Length; i++)
            {
                var pt = song.PhraseIterations[i];
                var hard = song.Phrases[pt.PhraseId].MaxDifficulty;
                if (pt.HeroLevels != null)
                    foreach (var h in pt.HeroLevels)
                    {
                        switch (h.Hero)
                        {
                            case 1:
                                easyArray.Add(h.Difficulty);
                                break;
                            case 2:
                                mediumArray.Add(h.Difficulty);
                                break;
                            case 3:
                                hard = h.Difficulty;
                                break;
                        }
                        hardArray.Add(hard);
                    }
            }

            // Is not the way of official are calculated, but is a way to calculate unique values for custom
            // Could be rewritten with the correct way or a better way to get this value
            var itCount = song.PhraseIterations.Length;
            var ifAny = easyArray.Count > 0;

            // TODO: round to 9 decimal places and improve calculation
            attribute.SongDiffEasy = ifAny ? easyArray.Average() / itCount : 0;
            attribute.SongDiffMed = ifAny ? mediumArray.Average() / itCount : 0;
            attribute.SongDiffHard = ifAny ? hardArray.Average() / itCount : 0;
            attribute.SongDifficulty = attribute.SongDiffHard;
        }

        public void GenerateSectionData(IAttributes attribute, dynamic song)
        {
            if (song.Sections == null)
                return;

            for (int i = 0; i < song.Sections.Length; i++)
            {
                var section = song.Sections[i];
                var sect = new Section
                {
                    Name = section.Name,
                    Number = section.Number,
                    StartTime = section.StartTime,
                    EndTime = (i >= song.Sections.Length - 1) ? song.SongLength : song.Sections[i + 1].StartTime,
                    UIName = null
                };
                string[] sep = sect.Name.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);

                // process "<section><number>" used by official XML
                var numAlpha = new Regex("(?<Alpha>[a-zA-Z]*)(?<Numeric>[0-9]*)");
                var match = numAlpha.Match(sep[0]);
                if (match.Groups["Numeric"].Value != "")
                    sep = new string[] { match.Groups["Alpha"].Value, match.Groups["Numeric"].Value };

                if (sep.Length == 1)
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                        sect.UIName = uiName;
                    else
                        throw new InvalidDataException(String.Format("Unknown section name: {0}", sep[0]));
                }
                else
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                    {
                        try
                        {
                            if (Convert.ToInt32(sep[1]) != 0 || Convert.ToInt32(sep[1]) != 1)
                                uiName += String.Format("|{0}", sep[1]);
                        }
                        catch { }
                        sect.UIName = uiName;
                    }
                    else
                        throw new InvalidDataException(String.Format("Unknown section name: {0}", sep[0]));
                }
                var phraseIterStart = -1;
                var phraseIterEnd = 0;
                var isSolo = section.Name == "solo";
                if (song.PhraseIterations != null)
                {
                    for (int o = 0; o < song.PhraseIterations.Length; o++)
                    {
                        var phraseIter = song.PhraseIterations[o];
                        if (phraseIterStart == -1 && phraseIter.Time >= sect.StartTime)
                            phraseIterStart = o;
                        if (phraseIter.Time >= sect.EndTime)
                            break;
                        phraseIterEnd = o;
                        if (song.Phrases[phraseIter.PhraseId].Solo > 0)
                            isSolo = true;
                    }
                }
                sect.StartPhraseIterationIndex = phraseIterStart;
                sect.EndPhraseIterationIndex = phraseIterEnd;
                sect.IsSolo = isSolo;
                attribute.Sections.Add(sect);
            }
        }

        public void GeneratePhraseData(IAttributes attribute, dynamic song)
        {
            if (song.Phrases == null)
                return;

            var ind = 0;
            foreach (var y in song.Phrases)
            {
                attribute.Phrases.Add(new Phrase
                {
                    IterationCount = PhraseIterationCount(song, ind),
                    MaxDifficulty = y.MaxDifficulty,
                    Name = y.Name // TODO: validate phrase names here
                });
                ind++;
            }
        }

        private int PhraseIterationCount(Song2014 song, int ind)
        {
            return song.PhraseIterations.Count(z => z.PhraseId == ind);
        }

        private int PhraseIterationCount(Song song, int ind)
        {
            return song.PhraseIterations.Count(z => z.PhraseId == ind);
        }

        public void GenerateChordTemplateData(IAttributes attribute, dynamic song)
        {
            var ind = 0;
            if (song.ChordTemplates == null)
                return;

            foreach (var y in song.ChordTemplates)
                if (!String.IsNullOrEmpty(y.ChordName)) //Only add chords with name, checked in RS1 and RS14 packages
                    attribute.ChordTemplates.Add(new ChordTemplate
                    {
                        ChordId = ind++,
                        ChordName = y.ChordName,
                        Fingers = new List<int> { y.Finger0, y.Finger1, y.Finger2, y.Finger3, y.Finger4, y.Finger5 },
                        Frets = new List<int> { y.Fret0, y.Fret1, y.Fret2, y.Fret3, y.Fret4, y.Fret5 }
                    });
        }

        //TODO: investigate on values 0.9 and lower, spotted in DLC
        public void GenerateDynamicVisualDensity(IAttributes attribute, dynamic song, Arrangement arrangement, GameVersion version)
        {
            if (arrangement.ArrangementType == ArrangementType.Vocal)
            {
                if (version == GameVersion.RS2014)
                    attribute.DynamicVisualDensity = Enumerable.Repeat(2.0f, 20).ToList();
                else
                    attribute.DynamicVisualDensity = new List<float> {
                        4.5f, 4.3000001907348633f, 4.0999999046325684f, 3.9000000953674316f, 3.7000000476837158f,
                        3.5f, 3.2999999523162842f, 3.0999999046325684f, 2.9000000953674316f, 2.7000000476837158f,
                        2.5f, 2.2999999523162842f, 2.0999999046325684f,
                        2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f
                    };
            }
            else
            {
                const float floorLimit = 5f;
                attribute.DynamicVisualDensity = new List<float>(20);
                float endSpeed = Math.Min(45f, Math.Max(floorLimit, arrangement.ScrollSpeed)) / 10f;
                if (song.Levels.Length == 1)
                {
                    attribute.DynamicVisualDensity = Enumerable.Repeat(endSpeed, 20).ToList();
                }
                else
                {
                    double beginSpeed = 4.5d;
                    double maxLevel = Math.Min(song.Levels.Length, 20d) - 1;
                    double factor = maxLevel > 0 ? Math.Pow(endSpeed / beginSpeed, 1d / maxLevel) : 1d;
                    for (int i = 0; i < 20; i++)
                    {
                        if (i >= maxLevel)
                        {
                            attribute.DynamicVisualDensity.Add(endSpeed);
                        }
                        else
                        {
                            attribute.DynamicVisualDensity.Add((float)(beginSpeed * Math.Pow(factor, i)));
                        }
                    }
                }
            }
        }

        public int GetNoteCount(float startTime, float endTime, ICollection<SongNote> notes)
        {
            int count = 0;
            for (int i = 0; i < notes.Count; i++)
            {
                var n = notes.ElementAt(i).Time;
                if (n < endTime &&
                   n >= startTime)
                    count++;
            }
            return count;
        }

        public int GetNoteCount2014(float startTime, float endTime, ICollection<SongNote2014> notes)
        {
            int count = 0;
            for (int i = 0; i < notes.Count; i++)
            {
                var n = notes.ElementAt(i).Time;
                if (n < endTime &&
                   n >= startTime)
                    count++;
            }
            return count;
        }

        public int GetChordCount(float startTime, float endTime, ICollection<dynamic> chords)
        {
            int count = 0;
            for (int i = 0; i < chords.Count; i++)
            {
                var ch = chords.ElementAt(i).Time;
                if (ch < endTime &&
                   ch >= startTime)
                    count++;
            }
            return count;
        }

        public Int32 GetMaxDifficulty(Song2014 xml)
        {
            var max = 0;
            foreach (var phrase in xml.Phrases)
                if (max < phrase.MaxDifficulty)
                    max = phrase.MaxDifficulty;
            return max;
        }

        public void GenerateTuningData(Attributes2014 attribute, dynamic song)
        {
            if (song.Tuning == null)
                return;

            attribute.Tuning = song.Tuning;
            var tuning = new TuningDefinition();
            var tuningName = tuning.NameFromStrings(attribute.Tuning);

            if (tuningName == "E Standard")
                attribute.ArrangementProperties.StandardTuning = 1;
            else
                attribute.ArrangementProperties.StandardTuning = 0;
        }

        public void GenerateChords(Attributes2014 attribute, Song2014 song)
        {
            // Some ODLC contain JSON Chords errors, this method is producing workable results
            //
            // USING song.Levels[difficulty].HandShapes METHOD
            // the handshape data can be used to obtain chordIds 
            // (more efficient less data to iterate through)
            //
            //"Chords" : {
            //     "DiffLevelID" : {//used to display which chord is set at current lvl.
            //         "SectionID" : [// >= 0
            //             ChordID, 
            //             ChordID
            //         ]
            //     },
            // }

            if (song.Sections == null)
                return;

            attribute.Chords = new Dictionary<string, Dictionary<string, List<int>>>();
            for (int difficulty = 0; difficulty < song.Levels.Length; difficulty++)
            {
                var chords = song.Levels[difficulty].HandShapes;
                var sectionId = new Dictionary<string, List<int>>();
                var chordId = new List<int>();

                for (int section = 0; section < song.Sections.Length; section++)
                {
                    var sectionNumber = song.Sections[section].Number;
                    var starTime = song.Sections[section].StartTime;
                    var endTime = song.Sections[Math.Min(section + 1, song.Sections.Length - 1)].StartTime;

                    // iterate through chords in handshapes in the difficulty level
                    foreach (var chord in chords)
                    {
                        if (chord.StartTime >= starTime && chord.EndTime < endTime) //in range
                        {
                            chordId.Add(chord.ChordId);
                        }
                    }

                    if (chordId.Count > 0)
                    {
                        // always ordered in ODLC
                        List<int> distinctChordIds = chordId.Distinct().OrderBy(x => x).ToList();
                        sectionId.Add(section.ToString(), distinctChordIds);
                    }

                    chordId = new List<int>();
                }

                if (sectionId.Keys.Count > 0)
                    attribute.Chords.Add(difficulty.ToString(), sectionId);
            }

        }

    }
}

/*
 * 
 * MAYBE USABLE FOR PHRASE NAMES
 * 
                string[] sep = sect.Name.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (sep.Length == 1)
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                        sect.UIName = uiName;
                    else
                        throw new InvalidDataException(String.Format("Unknown section name: {0}", sep[0]));
                }
                else
                {
                    string uiName;
                    if (SectionUINames.TryGetValue(sep[0], out uiName))
                    {
                        try
                        {
                            if (Convert.ToInt32(sep[1]) != 0 || Convert.ToInt32(sep[1]) != 1)
                                uiName += String.Format("|{0}", sep[1]);
                        }
                        catch { }
                        sect.UIName = uiName;
                    }
                    else
                        throw new InvalidDataException(String.Format("Unknown section name: {0}", sep[0]));
                }

*/

// CODE GRAVE YARD

//public void GenerateChords(Attributes2014 attribute, Song2014 song)
//{
// SAVE ... This method works for chords for which handshapes do not exists.
// Some ODLC contain JSON Chords errors, confirmed this method is accurate
// USING song.Levels[difficulty].Chords METHOD
//
//    //"Chords" : {
//    //     "DiffLevelID" : {//used to display which chord is set at current lvl.
//    //         "SectionID" : [// >= 0
//    //             ChordID, 
//    //             ChordID
//    //         ]
//    //     },
//    // }

//    if (song.Sections == null)
//        return;

//    attribute.Chords = new Dictionary<string, Dictionary<string, List<int>>>();
//    for (int difficulty = 0; difficulty < song.Levels.Length; difficulty++)
//    {
//        var chords = song.Levels[difficulty].Chords;
//        var sectionId = new Dictionary<string, List<int>>();
//        var chordId = new List<int>();
//        Debug.WriteLine("ManifestFunctions difficulty level: " + difficulty);

//        for (int section = 0; section < song.Sections.Length; section++)
//        {
//            var starTime = song.Sections[section].StartTime;
//            var endTime = song.Sections[Math.Min(section + 1, song.Sections.Length - 1)].StartTime;
//            Debug.WriteLine("ManifestFunctions section: " + section);

//            // iterate through chords in section in the difficulty level
//            foreach (var chord in chords)
//            {
//                if (chord.Time >= starTime && chord.Time < endTime) //in range
//                {
//                    Debug.WriteLine("ManifestFunctions added chord.ChordId: " + chord.ChordId);
//                    chordId.Add(chord.ChordId);
//                }
//            }

//            if (chordId.Count > 0)
//            {
//                List<int> distinctChordIds = chordId.Distinct().OrderBy(x => x).ToList();
//                sectionId.Add(section.ToString(), distinctChordIds);
//            }

//            chordId = new List<int>();
//        }

//        if (sectionId.Keys.Count > 0)
//            attribute.Chords.Add(difficulty.ToString(), sectionId); // .Distinct().ToDictionary(x => x.Key, x => x.Value));
//    }

//}


///// <summary>
///// Generates the techniques.
///// </summary>
///// <remarks>
///// "Techniques" : {
/////     "DiffLevelID" : {//used to display which techs are set at current lvl.
/////         "SetionID" : [// > 0
/////             TechID, //required base tech for extended tech(?)
/////             TechID
/////         ]
/////     },
///// }
///// </remarks>
///// <param name="attribute">Attribute.</param>
///// <param name="song">Song.</param>
//public void GenerateTechniques(Attributes2014 attribute, Song2014 song)
//{
//    // TODO: fix ... results from this method are NOT accurate 
//    //
//    //"Techniques" : {
//    //     "DiffLevelID" : {//used to display which techs are set at current lvl.
//    //         "SectionID" : [// > 0
//    //             TechID, //required base tech for extended tech(?)
//    //             TechID
//    //         ]
//    //     },
//    // }

//    if (song.Sections == null)
//        return;

//    attribute.Techniques = new Dictionary<string, Dictionary<string, List<int>>>();
//    for (int difficulty = 0; difficulty < song.Levels.Length; difficulty++)
//    {
//        var notes = song.Levels[difficulty].Notes;
//        var sectionId = new Dictionary<string, List<int>>();
//        var techId = new List<int>();

//        for (int section = 0; section < song.Sections.Length; section++)
//        {
//            var starTime = song.Sections[section].StartTime;
//            var endTime = song.Sections[Math.Min(section + 1, song.Sections.Length - 1)].StartTime;

//            // iterate through notes in section in the difficulty level
//            foreach (var note in notes)
//            {
//                if (note.Time >= starTime && note.Time < endTime) //in range
//                {
//                    var noteTech = getNoteTech(note);
//                    techId.AddRange(noteTech);
//                }
//            }

//            if (techId.Count > 0)
//            {
//                // TODO: needs more tweaking
//                //  techId.Add(35); // try adding dumby data for now
//                // order of usage in ODLC
//                List<int> distinctTechIds = techId.Distinct().ToList();
//                sectionId.Add((section + 1).ToString(), distinctTechIds);
//            }
//            /*
//                 "5": {
//                    "1": [
//                        13,
//                        35 <- missing
//                        ],
//                */



//            techId = new List<int>();
//        }

//        if (sectionId.Keys.Count > 0)
//        {
//            var sortedSectionIds = new Dictionary<string, List<int>>();
//            var keysSorted = sectionId.Keys.ToList();
//            keysSorted.Sort();
//            foreach (var key in keysSorted)
//            {
//                var keyValue = sectionId[key];
//                sortedSectionIds.Add(key, keyValue);
//            }
//            attribute.Techniques.Add(difficulty.ToString(), sortedSectionIds);
//        }
//    }
//}


//       public void GenerateTechniques(Attributes2014 attribute, Song2014 song)
//        {
//            // TODO: improve this method
//            if (song.Sections == null)
//                return;

//            attribute.Techniques = new Dictionary<string, Dictionary<string, List<int>>>();
//            for( int l = 0, s = 0, sectionsL = song.Sections.Length, levelsL = song.Levels.Length; l < levelsL; l++, s = 0 )
//            {
////                var shapes = song.Levels[l].HandShapes;
////                var chords = song.Levels[l].Chords;
//                var notes = song.Levels[l].Notes;
//                var t = new List<int>();
//                var techs = new Dictionary<string, List<int>>();

//                foreach( var n in notes )
//                {//note should be in section range
//                    var starTime = song.Sections[s].StartTime;
//                    var endTime = song.Sections[Math.Min(s + 1, sectionsL - 1)].StartTime;

//                    if(n.Time > starTime && n.Time <= endTime)//in range
//                    {
//                        t.AddRange(getNoteTech(n));
//                    }
//                    else if(n.Time > endTime)//at next section
//                    {
//                        s++;
//                        if(t.Count > 0){
//                            techs.Add(s.ToString(), t.Distinct().ToList());
//                            t = new List<int>();
//                        }
//                    }
//                }
//                if(techs.Values.Count > 0)
//                    attribute.Techniques.Add(l.ToString(), techs.Distinct().ToDictionary(x => x.Key, x => x.Value));
//            }
//        }

//if (sectionId.Keys.Count > 0)
//{
//    var sortedSectionIds = new Dictionary<string, List<int>>();
//    var keysSorted = sectionId.Keys.ToList();
//    keysSorted.Sort();

//    foreach (var key in keysSorted)
//    {
//        var keyValue = sectionId[key];
//        sortedSectionIds.Add(key, keyValue);
//    }

//    attribute.Techniques.Add(difficulty.ToString(), sortedSectionIds);
//}
