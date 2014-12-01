using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class ManifestFunctions
    {
        private Dictionary<string, string> SectionUINames { get; set; }

        public ManifestFunctions(GameVersion gameVersion)
        {
            switch (gameVersion) {
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
                if (song.Levels[y.MaxDifficulty].Chords != null )
                {
                    noteCnt += GetChordCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Chords);
                }
            }

            attribute.Score_MaxNotes = noteCnt;
            attribute.Score_PNV = ((float)attribute.TargetScore) / noteCnt;

            foreach (var y in attribute.PhraseIterations)
            {
                var phrase = song.Phrases[y.PhraseIndex];
                for (int o = 0; o <= phrase.MaxDifficulty; o++)
                {
                    var multiplier = ((float)(o + 1)) / (phrase.MaxDifficulty + 1);
                    var pnv = attribute.Score_PNV;
                    var noteCount = 0;

                    if (song.Levels[o].Chords != null)
                    {
                        if (gameVersion == GameVersion.RS2012)
                            noteCnt += GetNoteCount(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                        else
                            noteCnt += GetNoteCount2014(y.StartTime, y.EndTime, song.Levels[y.MaxDifficulty].Notes);
                    }

                    if (song.Levels[o].Chords != null)
                        noteCount += GetChordCount(y.StartTime, y.EndTime, song.Levels[o].Chords);

                    if (gameVersion == GameVersion.RS2012)
                    {
                        var score = pnv * noteCount * multiplier;
                        y.MaxScorePerDifficulty.Add(score);
                    }
                }
            }
        }

        public static void GetSongDifficulty(AttributesHeader2014 attribute, Song2014 song) {
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
            // Can be rewrited with the correct way or a best way to get this value
            var itCount = song.PhraseIterations.Length;

            attribute.SongDiffEasy = (easyArray.Count > 0) ? easyArray.Average() / itCount : 0;
            attribute.SongDiffMed = (easyArray.Count > 0) ? mediumArray.Average() / itCount : 0;
            attribute.SongDiffHard = (easyArray.Count > 0) ? hardArray.Average() / itCount : 0;
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
                    } else
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
                    Name = y.Name
                });
                ind++;
            }
        }

        private int PhraseIterationCount(Song2014 song, int ind) {
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

        public void GenerateDynamicVisualDensity(IAttributes attribute, dynamic song, Arrangement arrangement, GameVersion version) {
            if (arrangement.ArrangementType == ArrangementType.Vocal)
            {
                if (version == GameVersion.RS2014)
                    attribute.DynamicVisualDensity = new List<float> {
                        2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f,
                        2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f
                    };
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
                attribute.DynamicVisualDensity = new List<float>(20);
                float endSpeed = Math.Min(45f, Math.Max(10f, arrangement.ScrollSpeed)) / 10f;
                if (song.Levels.Length == 1)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        attribute.DynamicVisualDensity.Add(endSpeed);
                    }
                }
                else
                {
                    double beginSpeed = 4.5d;
                    double maxLevel = Math.Min(song.Levels.Length, 16d) - 1;
                    double factor = maxLevel == 0 ? 1d : Math.Pow(endSpeed / beginSpeed, 1d / maxLevel);
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
                if(notes.ElementAt(i).Time < endTime)
                if (notes.ElementAt(i).Time >= startTime)
                    count++;
            }
            return count;
        }

        public int GetNoteCount2014(float startTime, float endTime, ICollection<SongNote2014> notes)
        {
            int count = 0;
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes.ElementAt(i).Time < endTime)
                    if (notes.ElementAt(i).Time >= startTime)
                        count++;
            }
            return count;
        }

        public int GetChordCount(float startTime, float endTime, ICollection<dynamic> chords)
        {
            int count = 0;
            for (int i = 0; i < chords.Count; i++)
            {
                if (chords.ElementAt(i).Time < endTime)
                    if (chords.ElementAt(i).Time >= startTime)
                        count++;
            }
            return count;
        }

        public Int32 GetMaxDifficulty(Song2014 xml) {
            var max = 0;
            foreach (var phrase in xml.Phrases)
                if (max < phrase.MaxDifficulty)
                    max = phrase.MaxDifficulty;
            return max;
        }
    }
}
