using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class ManifestGeneric<T>
    {
        public Dictionary<string, Dictionary<string, T>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }
        public String InsertRoot { get; set; }

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

        public static ManifestGeneric<T> LoadFromFile(string manifestRS2012FilePath)
        {
            using (var reader = new StreamReader(manifestRS2012FilePath))
            {
                return JsonConvert.DeserializeObject<ManifestGeneric<T>>(reader.ReadToEnd());
            }
        }

        public void SaveToFile(string manifestRS2012FilePath)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Formatting = Formatting.Indented;
            jss.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(this, jss);
            File.WriteAllText(manifestRS2012FilePath, json);
        }
    }
}
