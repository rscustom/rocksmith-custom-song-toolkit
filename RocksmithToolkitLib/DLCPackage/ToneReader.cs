using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace RocksmithToolkitLib.DLCPackage
{
    public class ToneReader
    {
        public static List<Tone.Tone> Read(string filePath)
        {
            List<Tone.Tone> tones = new List<Tone.Tone>();
            FileInfo fi = new FileInfo(filePath);
            switch (fi.Extension)
            {
                case ".json":
                    tones.Add(readManifest(filePath));
                    break;
                case ".dat":
                    return extractTones(filePath);
                default:
                    throw new NotSupportedException(String.Format("Unknown file extension exception '{0}'. File not supported.", fi.Extension));
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

        private static List<Tone.Tone> extractTones(string packagePath)
        {
            List<Tone.Tone> tones = new List<Tone.Tone>();
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Packer.Unpack(packagePath, appDir, true);
            FileInfo file = new FileInfo(packagePath);
            DirectoryInfo unpackedDir = new DirectoryInfo(Path.Combine(appDir, Path.GetFileNameWithoutExtension(file.Name) + Packer.ADD_PC));
            IEnumerable<FileInfo> fileList = unpackedDir.GetFiles("tone*.manifest.json", SearchOption.AllDirectories);
            foreach (FileInfo fi in fileList)
            {
                tones.Add(readManifest(fi.FullName));
            }
            unpackedDir.Delete(true);
            
            return tones;
        }
    }
}
