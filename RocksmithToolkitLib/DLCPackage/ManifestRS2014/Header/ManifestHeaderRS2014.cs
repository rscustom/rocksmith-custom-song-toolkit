using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Header
{
    public class ManifestHeaderRS2014
    {
        public Dictionary<string, Dictionary<string, AttributesHeaderRS2014>> Entries { get; set; }
        public String InsertRoot { get; set; }

        public ManifestHeaderRS2014()
        {
            Entries = new Dictionary<string, Dictionary<string, AttributesHeaderRS2014>>();
            InsertRoot = "Static.Songs.Headers";
        }

        public static ManifestHeaderRS2014 LoadFromFile(string manifestHeaderRS2014FilePath) {
            using (var reader = new StreamReader(manifestHeaderRS2014FilePath)) {
                var manifest = new ManifestHeaderRS2014();
                manifest = JsonConvert.DeserializeObject<ManifestHeaderRS2014>(reader.ReadToEnd());
                return manifest;
            }
        }
    }
}
