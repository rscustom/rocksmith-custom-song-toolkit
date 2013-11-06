using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.ManifestRS2014
{
    public class ManifestRS2014
    {
        public Dictionary<string, Dictionary<string, AttributesRS2014>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }
        public String InsertRoot { get; set; }

        public ManifestRS2014()
        {
            ModelName = "RSEnumerable_Song";
            IterationVersion = 2;
            InsertRoot = "Static.Songs.Entries";
        }

        public static ManifestRS2014 LoadFromFile(string manifestRS2014FilePath) {
            using (var reader = new StreamReader(manifestRS2014FilePath)) {
                var manifest = new ManifestRS2014();
                manifest = JsonConvert.DeserializeObject<ManifestRS2014>(reader.ReadToEnd());
                return manifest;
            }
        }
    }
}
