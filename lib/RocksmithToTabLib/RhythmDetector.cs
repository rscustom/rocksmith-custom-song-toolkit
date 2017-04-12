using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocksmithToTabLib
{
    public class RhythmValue
    {
        public int Duration;
        public int NoteIndex;
    }


    public class RhythmDetector
    {
        public static List<RhythmValue> GetRhythm(List<float> noteDurations, int measureDuration, int beatDuration)
        {
            float sum = noteDurations.Sum();
            float scaling = measureDuration / sum;
            var noteEnds = new List<float>();
            float total = 0;
            for (int i = 0; i < noteDurations.Count; ++i)
            {
                noteDurations[i] *= scaling;
                total += noteDurations[i];
                noteEnds.Add(total);
            }

            MatchRhythm(noteEnds, 0, noteEnds.Count, 0, measureDuration, beatDuration);

            // determine final note values
            var ret = new List<RhythmValue>();
            float offset = 0;
            foreach (var end in noteEnds)
            {
                var rhythm = new RhythmValue()
                {
                    Duration = (int)Math.Round(end - offset),
                    NoteIndex = ret.Count
                };
                offset = end;
                ret.Add(rhythm);
            }
            SplitDurations(ret, measureDuration, beatDuration);
            return ret;
        }


        static void MatchRhythm(List<float> noteEnds, int start, int end, float offset, float length, int beatDuration)
        {
            //Console.WriteLine("MatchRhythm(start: {0}, end: {1}, offset: {2}, length: {3}, beatDuration: {4})", start, end, offset, length, beatDuration);
            //foreach (var ends in noteEnds)
            //    Console.Write("  {0:F2}", ends);
            //Console.WriteLine();
            // recursion condition: end if only one note is left in the current interval
            if (end - start <= 1)
                return;

            if (length <= 3)
            {
                // we can't divide this part any further, so all notes here need to be merged
                // i.e. all but the last note are set to a length of 0
                for (int i = start; i < end-1; ++i)
                {
                    noteEnds[i] = offset;
                }
                return;
            }

            int tripletBeat = beatDuration * 2 / 3;

            // we will now go through the note list and compare the end of each note with 
            // any multiple of the beat duration or the corresponding triplet. the closest
            // match will be taken, the note durations will be shifted accordingly, and then
            // the algorithm recurses left and right of the match.
            // the rationale behind the algorithm is as follows: even though every single note
            // will probably be slightly off in its length, in summary there is a good chance
            // to recognize the passing of e.g. two beats. So once we find that, we can look
            // deeper to approximately construct a fitting rhythm to the given note durations.
            const float PRECISION = 1.0f;
            int minMatchPos = start;
            float minMatchEnd = offset;
            float minMatchDiff = length+1;

            for (int i = start; i < end-1; ++i)
            {
                // try even rhythm
                float mult = (float)Math.Round(noteEnds[i] / beatDuration);
                float diff = Math.Abs(mult * beatDuration - noteEnds[i]);
                if (diff < minMatchDiff && mult * beatDuration >= offset && mult * beatDuration <= offset + length)
                {
                    minMatchPos = i;
                    minMatchEnd = mult * beatDuration;
                    minMatchDiff = diff;
                }

                // try the triplet variant
                mult = (float)Math.Round(noteEnds[i] / tripletBeat);
                diff = Math.Abs(mult * tripletBeat - noteEnds[i]);
                if (diff < minMatchDiff && mult * tripletBeat >= offset && mult * tripletBeat <= offset + length)
                {
                    minMatchPos = i;
                    minMatchEnd = mult * tripletBeat;
                    minMatchDiff = diff;
                }                
            }

            if (minMatchDiff < PRECISION || beatDuration <= 3)
            {
                // take the closest match and correct it to the determined value,
                // then rescale the other note ends accordingly and recurse
                //float originalLeftLength = noteEnds[minMatchPos] - offset;
                float correctedLeftLength = minMatchEnd - offset;
                //float originalRightLength = length - noteEnds[minMatchPos] + offset;
                float correctedRightLength = length - minMatchEnd + offset;
                //float leftScaling = correctedLeftLength / originalLeftLength;
                //float rightScaling = correctedRightLength / originalRightLength;
                //if (originalLeftLength == 0 || originalRightLength == 0)
                //    Console.WriteLine("  !!Warning: left or right length is 0.");
                //Console.WriteLine("Corrected note {0} to length {1}", minMatchPos, minMatchEnd);
                //for (int i = start; i < minMatchPos; ++i)
                //{
                //    // rescale left side
                //    noteEnds[i] = offset + (noteEnds[i] - offset) * leftScaling;
                //}
                //for (int i = minMatchPos + 1; i < end-1; ++i)
                //{
                //    // rescale right side
                //    noteEnds[i] = minMatchEnd + (noteEnds[i] - noteEnds[minMatchPos]) * rightScaling;
                //}
                noteEnds[minMatchPos] = minMatchEnd;
                // recurse left
                MatchRhythm(noteEnds, start, minMatchPos + 1, offset, correctedLeftLength, beatDuration);
                // recurse right
                MatchRhythm(noteEnds, minMatchPos + 1, end, minMatchEnd, correctedRightLength, beatDuration);
            }
            else
            {
                // no luck, try matching to a smaller beat value
                MatchRhythm(noteEnds, start, end, offset, length, beatDuration / 2);
            }
        }


        public static readonly int[] PrintableDurations = new int[] {2, 3, 4, 6, 8, 9, 12, 16, 18, 24, 32, 36, 48, 72, 96, 144, 192 };


        static void SplitDurations(List<RhythmValue> durations, int measureDuration, int beatLength)
        {
            // This function takes care of note values that cannot (or should not) be represented
            // by a single note, and it also tries to split up the rhythm in a more or less 
            // readable way. This means e.g. that triplet notes should not stand alone.
            // (Although the algorithm may not be perfect in prevent single triplets.)
            int curPos = 0;
            for (int i = 0; i < durations.Count; ++i)
            {
                if (durations[i].Duration <= 1)
                    continue;

                if (PrintableDurations.Contains(durations[i].Duration))
                {
                    curPos += durations[i].Duration;
                    continue;
                }

                //Console.WriteLine("  At pos {0}, trying to split duration {1}...", curPos, durations[i].Duration);

                bool done = false;

                int curBeat = beatLength;
                int noteEnd = curPos + durations[i].Duration;
                int n = 2;
                int d = 3;

                while (!done && curBeat >= 2)
                {
                    int maxMult = noteEnd / curBeat;
                    for (int j = maxMult; j >= 1; --j)
                    {
                        int remaining = noteEnd - j * curBeat;
                        if (remaining < 2 && remaining != 0)
                            break;
                        int duration = durations[i].Duration - remaining;
                        if (PrintableDurations.Contains(duration))
                        {
                            durations[i].Duration = duration;
                            if (remaining != 0)
                            {
                                durations.Insert(i + 1, new RhythmValue()
                                {
                                    Duration = remaining,
                                    NoteIndex = durations[i].NoteIndex
                                });
                            }
                            done = true;
                            break;
                        }
                    }

                    // try next smaller even / triplet beat
                    curBeat = curBeat * n / d;
                    if (n == 2)
                    {
                        n = 3;
                        d = 4;
                    }
                    else
                    {
                        n = 2;
                        d = 3;
                    }
                }

                if (!PrintableDurations.Contains(durations[i].Duration))
                {
                    Console.WriteLine("  Warning: Failed to split note duration {0} properly, splitting in half...", durations[i].Duration);
                    var newNote = new RhythmValue()
                    {
                        Duration = durations[i].Duration / 2,
                        NoteIndex = durations[i].NoteIndex
                    };
                    durations[i].Duration -= newNote.Duration;
                    durations.Insert(i + 1, newNote);
                    --i;
                }
                else
                {
                    curPos += durations[i].Duration;
                }
            }
        }

    }
}
