using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Header
{
    public class ManifestHeader2014
    {
        public Dictionary<string, Dictionary<string, AttributesHeader2014>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }
        public String InsertRoot { get; set; }

        public ManifestHeader2014() { }

        public ManifestHeader2014(Platform platform)
        {
            Entries = new Dictionary<string, Dictionary<string, AttributesHeader2014>>();
            if (platform.IsConsole) {
                ModelName = "RSEnumerable_Song_Header";
                IterationVersion = 2;
            }
            InsertRoot = "Static.Songs.Headers";
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

        public static ManifestHeader2014 LoadFromFile(string manifestHeader2014FilePath) {
            using (var reader = new StreamReader(manifestHeader2014FilePath)) {
                var manifest = new ManifestHeader2014();
                manifest = JsonConvert.DeserializeObject<ManifestHeader2014>(reader.ReadToEnd());
                return manifest;
            }
        }
    }
}
