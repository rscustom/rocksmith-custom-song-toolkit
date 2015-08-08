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
            using (var reader = new StreamReader(manifestToneFile))
            {
                return JsonConvert.DeserializeObject<Manifest>(reader.ReadToEnd());
            }
        }
    }
}
