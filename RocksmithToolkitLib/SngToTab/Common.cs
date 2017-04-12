using RocksmithToolkitLib.Sng;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.SngToTab
{
    public class Common
    {
        public static string TimeToString(Single time)
        {
            int timeInSeconds = (int)time;
            string minutes = (timeInSeconds / 60).ToString();
            string seconds = (timeInSeconds % 60).ToString();
            if (seconds.Length == 1)
                seconds = "0" + seconds;
            return minutes + ":" + seconds;
        }

        public static string ReplaceAt(string str, int position, string replace, int length = -1)
        {
            if (length < 0)
                length = replace.Length;
            StringBuilder sb = new StringBuilder(str);
            sb.Remove(position, length);
            sb.Insert(position, replace);
            return sb.ToString();
        }

        public static float ApproximateGCD(float[] input, float tolerance)
        {
            if (input.Length == 1)
                return input[0];

            List<float> remaining = new List<float>(input);

            do
            {
                List<float> differences = new List<float>();
                for (int i = 1; i < remaining.Count; i++)
                    differences.Add(remaining[i] - remaining[i - 1]);
                differences.Sort();

                float last = differences[0];
                List<float> bucket = new List<float>();
                bucket.Add(last);
                List<float> nextDifferences = new List<float>();

                for (int i = 1; i < differences.Count; i++)
                {
                    float n = differences[i];
                    float d = n - last;
                    if (d < tolerance)
                        bucket.Add(n);
                    else
                    {
                        nextDifferences.Add(bucket.Average());
                        bucket.Clear();
                        bucket.Add(n);
                    }

                    last = n;
                }
                nextDifferences.Add(bucket.Average());

                remaining = nextDifferences;
            }
            while (remaining.Count() > 1);

            return remaining[0];
        }

        public static int[] GetSlots(float[] input, float tolerance)
        {
            if (input.Length == 1)
                return new int[1] { 0 };

            float agcd = ApproximateGCD(input, tolerance);
            
            float first = input[0];
            List<int> slots = new List<int>();
            foreach (float n in input)
            {
                int d = (int)Math.Round((n - first) / agcd);
                slots.Add(d);
            }
            return slots.ToArray();
        }


        public const int MAX_DIFFICULTY_ONLY = -1;
        public const int ALL_DIFFICULTIES = -2;

        public static int getMaxDifficulty(SngFile sngFile)
        {
            int maxDifficulty = 0;
            foreach (PhraseIteration pi in sngFile.PhraseIterations)
            {
                Phrase p = sngFile.Phrases[pi.Id];
                if (p.MaxDifficulty > maxDifficulty)
                    maxDifficulty = p.MaxDifficulty;
            }

            return maxDifficulty;
        }

    }
}
