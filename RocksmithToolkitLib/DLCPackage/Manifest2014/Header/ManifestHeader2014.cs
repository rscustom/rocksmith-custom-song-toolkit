using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest2014.Header
{
    public class ManifestHeader2014<T>
    {
        public Dictionary<string, Dictionary<string, T>> Entries { get; set; }
        public String ModelName { get; set; }
        public int? IterationVersion { get; set; }
        public String InsertRoot { get; set; }

        public ManifestHeader2014() { }

        public ManifestHeader2014(Platform platform, DLCPackageType dlcType = DLCPackageType.Song)
        {
            switch (dlcType) {
                case DLCPackageType.Song:
                    if (platform.IsConsole) {
                        ModelName = "RSEnumerable_Song_Header";
                        IterationVersion = 2;
                    }
                    InsertRoot = "Static.Songs.Headers";
                    Entries = new Dictionary<string, Dictionary<string, T>>();
                    break;
                case DLCPackageType.Lesson:
                    throw new NotImplementedException("Lesson package type not implemented yet :(");
                case DLCPackageType.Inlay:
                    if (platform.IsConsole) {
                        ModelName = "RSEnumerable_Guitar_Header";
                    }
                    InsertRoot = "Static.Guitars.Headers";
                    Entries = new Dictionary<string, Dictionary<string, T>>();
                    break;
            }
            
        }

        public void Serialize(Stream stream) 
        {
            var writer = new StreamWriter(stream);
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Formatting = Formatting.Indented;
            jss.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(this, jss);
            writer.Write(json);
            writer.Flush();
        }

        public static ManifestHeader2014<T> LoadFromFile(string manifestHeader2014FilePath)
        {
            using (var reader = new StreamReader(manifestHeader2014FilePath)) {
                return JsonConvert.DeserializeObject<ManifestHeader2014<T>>(reader.ReadToEnd());
            }
        }

        public void SaveToFile(string manifestHeader2014FilePath)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Formatting = Formatting.Indented;
            jss.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(this, jss);
            File.WriteAllText(manifestHeader2014FilePath, json);
        }

    }
}
