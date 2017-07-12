using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class TuningFrequency
    {
        private static double A440 { get { return 440D; } }
        private static double CentsInOctave { get { return 1200D; } }

        private static double Ratio2Cents(double ratio)
        {
            return CentsInOctave * Math.Log(ratio) / Math.Log(2);
        }

        public static double Cents2Frequency(this double cents)
        {
            return Math.Round(A440 * Math.Pow(Math.Pow(2, 1D / 1200D), cents), 2);
        }

        public static double Cents2Frequency(this double? cents)
        {
            return cents != null ? Convert.ToDouble(cents).Cents2Frequency() : 0;
        }

        // Gets cents for frequency based on A440.
        public static double Frequency2Cents(this double freq, out double cents)
        {
            var ratio = freq / A440;
            cents = Math.Round(Ratio2Cents(ratio));
            return cents;
        }

        public static double Frequency2Note(double frequency, out string note)
        {
            var lnote = (Math.Log(frequency) - Math.Log(261.626)) / Math.Log(2) + 4.0;
            var oct = Math.Floor((decimal)lnote);
            var cents = CentsInOctave * (lnote - (double)oct);

            var noteTable = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            var noteName = "";
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

        public static string Frequency2Note(this double frequency)
        {
            string note;
            Frequency2Note(frequency, out note);
            return note;
        }

        public static double Frequency2Cents(this double frequency)
        {
            double dummy;
            return Frequency2Cents(frequency, out dummy);
        }

        // only for RS2014
        public static bool ApplyBassFix(Arrangement arr, bool saveTuningDefinition = false)
        {
            var debubMe = arr;

            // get the latest comments from the XML
            var xmlComments = Song2014.ReadXmlComments(arr.SongXml.File);
            var isBassFixed = xmlComments.Any(xComment => xComment.ToString().Contains("Low Bass Tuning Fixed")) || arr.TuningPitch.Equals(220.0);

            if (isBassFixed)
            {
                Console.WriteLine("Low bass tuning may already be fixed: " + arr.SongXml.File);
                // return false;
            }

            // TODO: check guitar compatibility
            // Octave up for each string
            Int16[] strings = arr.TuningStrings.ToArray();
            for (int s = 0; s < strings.Length; s++)
            {
                if (strings[s] < 0)
                    strings[s] += 12;
            }

            // update XML arrangement
            Song2014 songXml = Song2014.LoadFromFile(arr.SongXml.File);
            songXml.CentOffset = "-1200.0"; // Force 220Hz
            songXml.Tuning = new TuningStrings(strings);

            // bass tuning definition gets auto added/saved to repository
            if (saveTuningDefinition)
            {
                var tuningDef = TuningDefinitionRepository.Instance.Detect(songXml.Tuning, GameVersion.RS2014, false);

                if (!tuningDef.Name.Contains("Fixed"))
                {
                    var tuningUiName = String.Format("{0} Fixed", tuningDef.UIName);
                    var bassTuning = new TuningDefinition
                        {
                            Custom = true,
                            GameVersion = GameVersion.RS2014,
                            Name = tuningUiName.Replace(" ", ""),
                            Tuning = songXml.Tuning,
                            UIName = tuningUiName
                        };

                    TuningDefinitionRepository.SaveUnique(bassTuning);
                }
            }

            using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                songXml.Serialize(stream, true);

            // write xml comments back to fixed bass arrangement
            if (!isBassFixed)
                Song2014.WriteXmlComments(arr.SongXml.File, xmlComments, customComment: "Low Bass Tuning Fixed");

            return true;
        }

    }
}
