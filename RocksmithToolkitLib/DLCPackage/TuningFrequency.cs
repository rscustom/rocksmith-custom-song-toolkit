using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class TuningFrequency
    {
        public static double Frequency2Note(double frequency) {
            string dummy;
            return TuningFrequency.Frequency2Note(frequency, out dummy);
        }

        public static double Frequency2Note(double frequency, out string note)
        {
            var lnote = (Math.Log(frequency) - Math.Log(261.626)) / Math.Log(2) + 4.0;
            decimal oct = Math.Floor((decimal)lnote);
            var cents = 1200 * (lnote - (double)oct);
            
            var noteTable = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            string noteName = "";
            if (cents < 50) {
                noteName = "C";
            } else if (cents >= 1150) {
                noteName = "C";
                cents -= 1200;
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
    }
}
