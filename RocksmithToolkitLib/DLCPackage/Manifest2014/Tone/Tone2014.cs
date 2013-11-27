using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    #region TONE DESCRIPTOR

    public enum ToneDescriptor {
        [Description("$[35715]BASS")]
        BASS = 35715,
        [Description("$[35716]OVERDRIVE")]
        OVERDRIVE = 35716,
        [Description("$[35718]VOCAL")]
        VOCAL = 35718,
        [Description("$[35719]OCTAVE")]
        OCTAVE = 35719,
        [Description("$[35720]CLEAN")]
        CLEAN = 35720,
        [Description("$[35721]ACOUSTIC")]
        ACOUSTIC = 35721,
        [Description("$[35722]DISTORTION")]
        DISTORTION = 35722,
        [Description("$[35723]CHORUS")]
        CHORUS = 35723,
        [Description("$[35724]LEAD")]
        LEAD = 35724,
        [Description("$[35725]ROTARY")]
        ROTARY = 35725,
        [Description("$[35726]REVERB")]
        REVERB = 35726,
        [Description("$[35727]TREMOLO")]
        TREMOLO = 35727,
        [Description("$[35728]VIBRATO")]
        VIBRATO = 35728,
        [Description("$[35729]FILTER")]
        FILTER = 35729,
        [Description("$[35730]PHASER")]
        PHASER = 35730,
        [Description("$[35731]FLANGER")]
        FLANGER = 35731,
        [Description("$[35732]LOW OUTPUT")]
        LOW_OUTPUT = 35732,
        [Description("$[35733]EFFECT")]
        EFFECT = 35733,
        [Description("$[35734]PROCESSED")]
        PROCESSED = 35734,
        [Description("$[35750]SPECIAL EFFECT")]
        SPECIAL_EFFECT = 35750,
        [Description("$[35751]MULTI-EFFECT")]
        MULTI_EFFECT = 35751,
        [Description("$[35752]DIRECT")]
        DIRECT = 35752,
        [Description("$[35753]DELAY")]
        DELAY = 35753,
        [Description("$[35754]ECHO")]
        ECHO = 35754,
        [Description("$[35755]HIGH GAIN")]
        HIGH_GAIN = 35755,
        [Description("$[35756]FUZZ")]
        FUZZ = 35756
    }

    #endregion

    public class Tone2014
    {
        public Gear2014 GearList { get; set; }
        public bool IsCustom { get; set; }
        public decimal Volume { get; set; }
        public List<string> ToneDescriptors { get; set; }
        public string Key { get; set; }
        public string NameSeparator { get; set; }
        public string Name { get; set; }
        public decimal SortOrder { get; set; }

        public Tone2014()
        {
            GearList = new Gear2014();
            IsCustom = true;
            Volume = -12;
            ToneDescriptors = new List<string>();
            NameSeparator = " - ";
            SortOrder = 0;
        }

        public override string ToString()
        {
            return Name;
        }

        public void Serialize(string toneSavePath) {
            var serializer = new DataContractSerializer(typeof(Tone2014));
            using (var stm = XmlWriter.Create(toneSavePath, new XmlWriterSettings() { CheckCharacters = true, Indent = true })) {
                serializer.WriteObject(stm, this);
            }
        }

        public static Tone2014 LoadFromXmlTemplateFile(string toneTemplateFilePath)
        {
            Tone2014 tone = null;
            var serializer = new DataContractSerializer(typeof(Tone2014));
            using (var stm = new XmlTextReader(toneTemplateFilePath)) {
                tone = (Tone2014)serializer.ReadObject(stm);
            }
            return tone;
        }

        #region Tone Import

        public static List<Tone2014> Import(string filePath)
        {
            List<Tone2014> tones = new List<Tone2014>();

            var toneExtension = Path.GetExtension(filePath);

            switch (toneExtension)
            {
                case ".json":
                    tones.AddRange(ReadFromManifest(filePath));
                    break;
                default:
                    var platform = Packer.GetPlatform(filePath);
                    switch (platform.platform)
                    {
                        case GamePlatform.Pc:
                        case GamePlatform.Mac:
                        case GamePlatform.XBox360:
                            return ReadFromPackage(filePath, platform);
                        case GamePlatform.PS3:
                            throw new InvalidOperationException("PS3 platform is not supported at this time :(");
                        default:
                            throw new NotSupportedException(String.Format("Unknown file extension exception '{0}'. File not supported.", toneExtension));
                    }
            }

            return tones;
        }

        #endregion

        #region Tone Manifest / Song Package

        private static List<Tone2014> ReadFromManifest(string manifestFilePath)
        {
            List<Tone2014> tones = new List<Tone2014>();

            Attributes2014 jsonManifestAttributes = Manifest2014<Attributes2014>.LoadFromFile(manifestFilePath).Entries.ToArray()[0].Value.ToArray()[0].Value;
            if (jsonManifestAttributes.ArrangementName != ArrangementName.Vocals.ToString() && jsonManifestAttributes.Tones != null)
                tones.AddRange(jsonManifestAttributes.Tones);

            return tones;
        }

        private static List<Tone2014> ReadFromPackage(string packagePath, Platform platform)
        {
            List<Tone2014> tones = new List<Tone2014>();
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Packer.Unpack(packagePath, appDir, (platform.platform == GamePlatform.Pc) ? true : false);
            string unpackedDir = Path.Combine(appDir, Path.GetFileNameWithoutExtension(packagePath) + String.Format("_{0}", platform.platform.ToString()));

            string[] toneManifestFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);

            foreach (var file in toneManifestFiles)
                tones.AddRange(ReadFromManifest(file));

            if (Directory.Exists(unpackedDir))
                Directory.Delete(unpackedDir, true);

            return tones;
        }

        #endregion
    }
}
