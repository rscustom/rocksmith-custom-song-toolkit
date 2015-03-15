using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class Manifest
    {
        public Dictionary<string, Dictionary<string, Attributes>> Entries { get; set; }
        public String ModelName { get; set; }
        public int IterationVersion { get; set; }


        public static Manifest LoadFromFile(string manifestToneFile)
        {
            var manifest = new Manifest();
            using (var reader = new StreamReader(manifestToneFile))
            {
                manifest = JsonConvert.DeserializeObject<Manifest>(reader.ReadToEnd());
            }
            return manifest;
        }
    }
}
