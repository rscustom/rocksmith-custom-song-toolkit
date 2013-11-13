using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class Manifest2014<T>
    {
        public Dictionary<string, Dictionary<string, T>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }
        public String InsertRoot { get; set; }

        public Manifest2014() {            
            ModelName = "RSEnumerable_Song";
            IterationVersion = 2;
            InsertRoot = "Static.Songs.Entries";
            Entries = new Dictionary<string, Dictionary<string, T>>();
        }

        public void Serialize(Stream stream) {
            var writer = new StreamWriter(stream);
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Formatting = Formatting.Indented;
            jss.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(this, jss);
            writer.Write(json);
            writer.Flush();
        }

        public static Manifest2014<T> LoadFromFile(string manifestRS2014FilePath)
        {
            using (var reader = new StreamReader(manifestRS2014FilePath)) {
                var manifest = new Manifest2014<T>();
                manifest = JsonConvert.DeserializeObject<Manifest2014<T>>(reader.ReadToEnd());
                return manifest;
            }
        }
    }
}
