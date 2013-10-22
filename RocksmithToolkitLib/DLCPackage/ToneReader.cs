using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

using Newtonsoft.Json;
using RocksmithToolkitLib.Sng;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage
{
    public class ToneReader
    {
        public static List<Tone.Tone> Read(string filePath)
        {
            List<Tone.Tone> tones = new List<Tone.Tone>();
            FileInfo fi = new FileInfo(filePath);
            if (fi.Extension == ".json")
                tones.Add(readManifest(filePath));
            else if (fi.Extension == ".xml")
                //throw new NotSupportedException(String.Format("Unknown file extension exception '{0}'. File not supported.", fi.Extension));
                tones.Add(readRSToneXml(filePath));
            else
            {
                var platform = Packer.GetPlatform(fi.FullName);
                switch (platform)
                {
                    case GamePlatform.Pc:
                    case GamePlatform.XBox360:
                        return extractTones(filePath, platform);
                    case GamePlatform.PS3:
                        throw new InvalidOperationException("PS3 platform is not supported at this time :(");
                    case GamePlatform.None:
                    default:
                        throw new NotSupportedException(String.Format("Unknown file extension exception '{0}'. File not supported.", fi.Extension));
                }
            }
            return tones;
        }

        private static Tone.Tone readRSToneXml(string TonePathXML)
        {
            var manifest = new Tone.Manifest();
            var doc = XDocument.Load(TonePathXML);
            manifest.Entries.AddRange (
                from e in doc.Descendants("song")
                select new Tone.Tone
                {   //Cleaning
                    BlockAsset = null,
                    Key = null,
                    PersistentID = null,
                    UnlockKey = null,

                    Name = (string)e.Attribute("name"),
                    Volume = (decimal)e.Attribute("volume"),
                    PedalList = (//key = amp or cabinet or pedal; value = type of Tone.Pedal
                        from p in e.Elements("pedal")
                        select new Tone.Pedal
                        {
                            PedalKey = (string)p.Attribute("name"), //string from pedal list
                            KnobValues = p.Descendants("rtpc").ToDictionary(r => r.Attribute("name").Value, r => Convert.ToDecimal(r
                                .Attribute("value").Value))
                        })//через лист и число ключей в PedalList
                    	.ToDictionary(x => Transform(x.PedalKey.Split("_".ToCharArray())[0].ToString()))
                });
            Keys.Clear();
            num = 0;
            return manifest.Entries[0];
        }
        //If-else block for detecting which key it is.
        private static SortedSet<string> Keys = new SortedSet<string>();
        private static int num {get;set;}
        private static string Transform(string name)
        {
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
            }
            else
            { throw new Exception("You have choose invalid RS tone file or something else!"); }
        	
        }
        private static Tone.Tone readManifest(string manifestPath)
        {
            using (var reader = new StreamReader(manifestPath))
            {
                var manifest = new Tone.Manifest();
                manifest = JsonConvert.DeserializeObject<Tone.Manifest>(reader.ReadToEnd());
                // Remove unecessary information
                manifest.Entries[0].BlockAsset = null;
                manifest.Entries[0].ExclusiveBuild = null;
                manifest.Entries[0].Key = null;
                manifest.Entries[0].PersistentID = null;
                manifest.Entries[0].UnlockKey = null;
                return manifest.Entries[0];
            }
        }

        private static List<Tone.Tone> extractTones(string packagePath, GamePlatform platform)
        {
            List<Tone.Tone> tones = new List<Tone.Tone>();
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Packer.Unpack(packagePath, appDir, true);
            string unpackedDir = Path.Combine(appDir, Path.GetFileNameWithoutExtension(packagePath) + String.Format("_{0}", platform.ToString()));
            string[] toneManifestFiles = Directory.GetFiles(unpackedDir, "tone*.manifest.json", SearchOption.AllDirectories);
            
            foreach (var file in toneManifestFiles)
            {
                tones.Add(readManifest(file));
            }
            if (Directory.Exists(unpackedDir))
                Directory.Delete(unpackedDir, true);
            
            return tones;
        }
    }
}
