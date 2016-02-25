using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.SngToTab;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class TuningFrequency
    {
        private static double A440 { get { return 440D; } }
        private static double CentsInOctave { get { return 1200D; } }

        private static double Ratio2Cents(double Ratio)
        {
            return CentsInOctave * Math.Log(Ratio) / Math.Log(2);
        }

        public static double Cents2Frequency(this double Cents)
        {
            return Math.Round(A440 * Math.Pow(Math.Pow(2, 1D / 1200D), Cents));
        }

        public static double Cents2Frequency(this double? Cents)
        {
            if (Cents != null)
                return Convert.ToDouble(Cents).Cents2Frequency();
            else
                return 0;
        }

        // Gets cents for frequency based on A440.
        public static double Frequency2Cents(this double Freq, out double Cents)
        {
            double Ratio = Freq / A440;
            Cents = Math.Round(Ratio2Cents(Ratio));
            return Cents;
        }

        public static double Frequency2Note(double frequency, out string note)
        {
            var lnote = (Math.Log(frequency) - Math.Log(261.626)) / Math.Log(2) + 4.0;
            decimal oct = Math.Floor((decimal)lnote);
            var cents = CentsInOctave * (lnote - (double)oct);

            var noteTable = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            string noteName = "";
            if (cents < 50)
            {
                noteName = "C";
            }
            else if (cents >= 1150)
            {
                noteName = "C";
                cents -= CentsInOctave;
                oct++;
            }
            else
            {
                var offset = 50.0;
                for (var j = 1; j <= 11; j++)
                {
                    if (cents >= offset && cents < (offset + 100))
                    {
                        noteName = noteTable[j];
                        cents -= (j * 100);
                        break;
                    }
                    offset += 100;
                }
            }

            note = noteName + oct;
            return Math.Round(cents);
        }

        public static double Frequency2Note(double frequency)
        {
            string dummy;
            return TuningFrequency.Frequency2Note(frequency, out dummy);
        }

        public static double Frequency2Cents(double frequency)
        {
            double dummy;
            return TuningFrequency.Frequency2Cents(frequency, out dummy);
        }


        // only for RS2014
        public static bool ApplyBassFix(Arrangement arr)
        {
            if (arr.TuningPitch.Equals(220.0))
            {
                // MessageBox.Show("This song is already at 220Hz pitch (bass fixed applied already?)", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Song2014 songXml = Song2014.LoadFromFile(arr.SongXml.File);
            // Force 220Hz
            arr.TuningPitch = 220.0;
            songXml.CentOffset = "-1200.0";
            // Octave up for each string
            Int16[] strings = arr.TuningStrings.ToArray();
            for (int s = 0; s < strings.Length; s++)
            {
                if (strings[s] != 0)
                    strings[s] += 12;
            }

            //Detect tuning
            var tuning = TuningDefinitionRepository.Instance().SelectAny(new TuningStrings(strings), GameVersion.RS2014);
            if (tuning == null)
            {
                tuning = new TuningDefinition();
                tuning.Tuning = new TuningStrings(strings);
                tuning.Name = tuning.NameFromStrings(tuning.Tuning);
                tuning.UIName = tuning.Name = String.Format("{0} Fixed", tuning.Name);
                tuning.Custom = true;
                tuning.GameVersion = GameVersion.RS2014;
                TuningDefinitionRepository.Instance().Add(tuning, true);
            }

            arr.TuningStrings = tuning.Tuning;
            arr.Tuning = tuning.Name;
            songXml.Tuning = tuning.Tuning;

            File.Delete(arr.SongXml.File);
            using (var stream = File.OpenWrite(arr.SongXml.File))
            {
                songXml.Serialize(stream, true);
            }

            return true;
        }

 

    }
}
