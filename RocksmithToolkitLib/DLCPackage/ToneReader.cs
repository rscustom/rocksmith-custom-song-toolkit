using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using RocksmithToolkitLib.Sng;

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
            else {
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
