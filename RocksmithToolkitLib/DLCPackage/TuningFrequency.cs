using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class TuningFrequency
    {
        private static double A440 { get{ return 440D; } }
        private static double CentsInOctave { get { return 1200D; } }

        private static double Ratio2Cents(double Ratio)
        {
            return CentsInOctave * Math.Log(Ratio) / Math.Log(2);
        }

        public static double Cents2Frequency(this double Cents)
        {
            return Math.Round(A440 * Math.Pow(Math.Pow(2, 1D / 1200D), Cents), 3);
        }

        public static double Cents2Frequency(this double? Cents) {
            if (Cents != null)
                return Convert.ToDouble(Cents).Cents2Frequency();
            else
                return 0;
        }

        // Gets cents for frequency based on A440.
        public static double Frequency2Cents(this double Freq, out double Cents)
        {
            double Ratio = Freq / A440;
            Cents = Ratio2Cents(Ratio);
            return Math.Round(Cents, 2);
        }

        public static double Frequency2Note(double frequency, out string note)
        {
            var lnote = (Math.Log(frequency) - Math.Log(261.626)) / Math.Log(2) + 4.0;
            decimal oct = Math.Floor((decimal)lnote);
            var cents = CentsInOctave * (lnote - (double)oct);

            var noteTable = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            string noteName = "";
            if (cents < 50) {
                noteName = "C";
            } else if (cents >= 1150) {
                noteName = "C";
                cents -= CentsInOctave;
                oct++;
            } else {
                var offset = 50.0;
                for (var j = 1; j <= 11; j++) {
                    if (cents >= offset && cents < (offset + 100)) {
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
    }
}
