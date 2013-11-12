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
        public String InsertRoot { get; set; }

        public ManifestHeader2014()
        {
            Entries = new Dictionary<string, Dictionary<string, AttributesHeader2014>>();
            InsertRoot = "Static.Songs.Headers";
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
