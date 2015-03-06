using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Reflection;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.Tone
{
    public class Tone {
        #region Tone Import Types

        //If-else block for detecting which key it is.
        private static SortedSet<string> Keys = new SortedSet<string>();
        private static int num { get; set; }

        #endregion

        public string BlockAsset { get; set; }
        public string Description { get; set; }
        public List<object> ExclusiveBuild { get; set; }
        public bool IsDLC { get; set; }
        public bool IsPreviewOnlyItem { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public Dictionary<string, Pedal> PedalList { get; set; }
        public string PersistentID { get; set; }
        public string UnlockKey { get; set; }
        public float Volume { get; set; }

        public Tone()
        {
            //fill with defauld amp\cab
            PedalList = new Dictionary<string, Pedal>();
            ExclusiveBuild = new List<object>();
            UnlockKey = "";
            IsDLC = true;
            IsPreviewOnlyItem = false;
            Volume = -12;
            Description = "$[-1] ";
        }

        public override string ToString()
        {
            return Name;
        }

        public void Serialize(string toneSavePath) {
            var serializer = new DataContractSerializer(typeof(Tone));
            using (var stm = XmlWriter.Create(toneSavePath, new XmlWriterSettings() { CheckCharacters = true, Indent = true })) {
                serializer.WriteObject(stm, this);
            }
        }

        public static Tone LoadFromXmlTemplateFile(string toneTemplateFilePath) {
            Tone tone = null;
            var serializer = new DataContractSerializer(typeof(Tone));
            using (var stm = new XmlTextReader(toneTemplateFilePath)) {
                tone = (Tone)serializer.ReadObject(stm);
            }
            return tone;
        }

        public static List<Tone> Import(string filePath) {
            var tones = new List<Tone>();

            var toneExtension = Path.GetExtension(filePath);
            switch (toneExtension) {
                case ".json":
                    tones.Add(ReadFromManifest(filePath));
                    break;
                case ".xml":
                    tones.Add(ReadFromRocksmithExportedXml(filePath));
                    break;
                default:
                    var platform = Packer.GetPlatform(filePath);
                    switch (platform.platform) {
                        case GamePlatform.Pc:
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

        #region Read From Rocksmith Tone Exported to Xml

        private static Tone ReadFromRocksmithExportedXml(string TonePathXML) {
            var manifest = new Manifest();
            var doc = XDocument.Load(TonePathXML);
            manifest.Entries.AddRange(
                from e in doc.Descendants("song")
                select new Tone {   //Cleaning
                    BlockAsset = null,
                    Key = null,
                    PersistentID = null,
                    UnlockKey = null,

                    Name = (string)e.Attribute("name"),
                    Volume = (float)e.Attribute("volume"),
                    PedalList = (//key = amp or cabinet or pedal; value = type of Tone.Pedal
                        from p in e.Elements("pedal")
                        select new Pedal {
                            PedalKey = (string)p.Attribute("name"), //string from pedal list
                            KnobValues = p.Descendants("rtpc").ToDictionary(r => r.Attribute("name").Value, r => Convert.ToDecimal(r
                                .Attribute("value").Value))
                        })
                        .ToDictionary(x => Transform(x.PedalKey.Split("_".ToCharArray())[0].ToString()))
                });
            Keys.Clear();
            num = 0;
            return manifest.Entries[0];
        }

        private static string Transform(string name) {
            if (name.Equals("Pedal") && num == 0 && !Keys.Contains("PostPedal3")) {
                num += 1;
                Keys.Add("PostPedal3");
                return "PostPedal3";
            }
            if (name.Equals("Pedal") && num == 1 && !Keys.Contains("Amp") && !Keys.Contains("Cabinet") && !Keys.Contains("PostPedal2")) {
                num += 1;
                Keys.Add("PostPedal2");
                return "PostPedal2";
            }
            if (name.Equals("Pedal") && num == 2 && !Keys.Contains("Amp") && !Keys.Contains("Cabinet") && !Keys.Contains("PostPedal1")) {
                num += 1;
                Keys.Add("PostPedal1");
                return "PostPedal1";
            }
            if (name.Equals("Cab")) {
                num += 1;
                Keys.Add("Cabinet");
                return "Cabinet";
            }
            if (name.Equals("Pedal") && num >= 1 && !Keys.Contains("Amp") && Keys.Contains("Cabinet") && !Keys.Contains("LoopPedal3")) {
                num += 1;
                Keys.Add("LoopPedal3");
                return "LoopPedal3";
            }
            if (name.Equals("Pedal") && num >= 2 && !Keys.Contains("Amp") && Keys.Contains("Cabinet") && !Keys.Contains("LoopPedal2")) {
                num += 1;
                Keys.Add("LoopPedal2");
                return "LoopPedal2";
            }
            if (name.Equals("Pedal") && num >= 3 && !Keys.Contains("Amp") && Keys.Contains("Cabinet") && !Keys.Contains("LoopPedal1")) {
                num += 1;
                Keys.Add("LoopPedal1");
                return "LoopPedal1";
            }
            if (name.Equals("Amp")) {
                num += 1;
                Keys.Add("Amp");
                return "Amp";
            }
            if (name.Equals("Pedal") && num >= 2 && Keys.Contains("Cabinet") && Keys.Contains("Amp") && !Keys.Contains("PrePedal3")) {
                num += 1;
                Keys.Add("PrePedal3");
                return "PrePedal3";
            }
            if (name.Equals("Pedal") && num >= 3 && Keys.Contains("Cabinet") && Keys.Contains("Amp") && !Keys.Contains("PrePedal2")) {
                num += 1;
                Keys.Add("PrePedal2");
                return "PrePedal2";
            }
            if (name.Equals("Pedal") && num >= 4 && Keys.Contains("Cabinet") && Keys.Contains("Amp") && !Keys.Contains("PrePedal1")) {
                num += 1;
                Keys.Add("PrePedal1");
                return "PrePedal1";
            } else { throw new Exception("You have choose invalid RS tone file or something else!"); }

        }

        #endregion

        #region Tone Manifest / Song Package

        private static Tone ReadFromManifest(string manifestFilePath) {
            var manifest = Manifest.LoadFromFile(manifestFilePath);

            manifest.Entries[0].BlockAsset = null;
            manifest.Entries[0].ExclusiveBuild = null;
            manifest.Entries[0].Key = null;
            manifest.Entries[0].PersistentID = null;
            manifest.Entries[0].UnlockKey = null;

            return manifest.Entries[0];
        }

        private static List<Tone> ReadFromPackage(string packagePath, Platform platform) {
            var tones = new List<Tone>();
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string unpackedDir = Packer.Unpack(packagePath, appDir, predefinedPlatform: platform);

            foreach (var file in Directory.EnumerateFiles(unpackedDir, "tone*.manifest.json", SearchOption.AllDirectories))
                tones.Add(ReadFromManifest(file));

            DirectoryExtension.SafeDelete(unpackedDir);

            return tones;
        }

        #endregion
    }
}
