using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class Manifest
    {
        public List<Tone> Entries { get; set; }
        public string ModelName { get; set; }
        public int IterationVersion { get; set; }

        public Manifest()
        {
            Entries = new List<Tone>();
            ModelName = "GRTonePreset";
            IterationVersion = 2;
        }

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
