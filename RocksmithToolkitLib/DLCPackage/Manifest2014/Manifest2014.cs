using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class ManifestRS2014<T>
    {
        public Dictionary<string, Dictionary<string, T>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }
        public String InsertRoot { get; set; }

        public ManifestRS2014() {            
            ModelName = "RSEnumerable_Song";
            IterationVersion = 2;
            InsertRoot = "Static.Songs.Entries";
            Entries = new Dictionary<string, Dictionary<string, T>>();
        }

        public void Serialize(Stream stream) {
            StreamWriter writer = new StreamWriter(stream);
            //return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static ManifestRS2014<T> LoadFromFile(string manifestRS2014FilePath)
        {
            using (var reader = new StreamReader(manifestRS2014FilePath)) {
                var manifest = new ManifestRS2014<T>();
                manifest = JsonConvert.DeserializeObject<ManifestRS2014<T>>(reader.ReadToEnd());
                return manifest;
            }
        }
    }
}
