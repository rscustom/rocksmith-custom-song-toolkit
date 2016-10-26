using System;
using System.IO;
using RocksmithToolkitLib.Xml;
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

        // TODO: apply before generate, like metronome arrangements does
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
                if (strings[s] < 0)
                    strings[s] += 12;
            }

            //Detect tuning
            var tuning = TuningDefinitionRepository.Instance.Detect(new TuningStrings(strings), GameVersion.RS2014, true);
            arr.Tuning = tuning.UIName = tuning.Name = String.Format("{0} Fixed", tuning.Name);// bastartd bass hack, huh?
            arr.TuningStrings = songXml.Tuning = tuning.Tuning;
            TuningDefinitionRepository.Instance.Save(true);

            var xmlComments = Song2014.ReadXmlComments(arr.SongXml.File);
            using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                songXml.Serialize(stream, true);

            Song2014.WriteXmlComments(arr.SongXml.File, xmlComments);

            return true;
        }

 

    }
}
