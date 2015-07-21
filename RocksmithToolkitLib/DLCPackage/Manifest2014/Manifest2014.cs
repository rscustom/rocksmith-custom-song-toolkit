using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest2014
{
    public class Manifest2014<T>
    {
        public Dictionary<string, Dictionary<string, T>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }
        public String InsertRoot { get; set; }

        public Manifest2014(DLCPackageType dlcType = DLCPackageType.Song) {
            switch (dlcType) {
                case DLCPackageType.Song:
                    ModelName = "RSEnumerable_Song";
                    IterationVersion = 2;
                    InsertRoot = "Static.Songs.Entries";
                    Entries = new Dictionary<string, Dictionary<string, T>>();
                    break;
                case DLCPackageType.Lesson:
                    throw new NotImplementedException("Lesson package type not implemented yet :(");
                case DLCPackageType.Inlay:
                    ModelName = "RSEnumerable_Guitars";
                    IterationVersion = 2;
                    InsertRoot = "Static.Guitars.Entries";
                    Entries = new Dictionary<string, Dictionary<string, T>>();
                    break;
            }
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
                return JsonConvert.DeserializeObject<Manifest2014<T>>(reader.ReadToEnd());
            }
        }

        public void SaveToFile(string manifestRS2014FilePath) {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Formatting = Formatting.Indented;
            jss.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(this, jss);
            File.WriteAllText(manifestRS2014FilePath, json);
        }
    }
}
