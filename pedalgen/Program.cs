using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.Tone;

namespace pedalgen
{
    class Program
    {
        static void Main(string[] args)
        {

            var dir = args[0];
            var pedals = new List<Pedal>();
            foreach (string file in Directory.EnumerateFiles(dir))
            {
                try
                {
                    var pedalsJson = File.ReadAllText(file);
                    var junk = pedalsJson.IndexOf("\n}");
                    if (junk > -1)
                    {
                        pedalsJson = pedalsJson.Substring(0, junk + 2);
                    }
                    pedalsJson.Replace("TonePedalRTPCName", "Key");
                    var filePedals = JsonConvert.DeserializeObject<RsJsonFile<Pedal>>(pedalsJson);
                    if ("GRPedal".Equals(filePedals.ModelName))
                    {
                        pedals.AddRange(filePedals.Entries);
                    }
                }
                catch { }
            }
            var bad = pedals.Where(p => p.Type == null);
            var goodPedals = pedals.Where(p => p.Type != null);
            File.WriteAllText(Path.Combine(dir, "pedals.json"), JsonConvert.SerializeObject(goodPedals, Formatting.Indented));
        }


    }
}
