using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.XBlock;

namespace RocksmithToolkitLib.DLCPackage
{
    public class ToneGenerator // RS1 only
    {
        public static void Generate(string toneKey, Tone tone, Stream outManifest, Stream outXblock, Stream aggregateGraph)
        {
            var id = IdGenerator.Guid().ToString().Replace("-", "");
            if (string.IsNullOrEmpty(tone.Name))
                tone.Name = toneKey;
            tone.Key = toneKey;
            tone.PersistentID = id;
            tone.BlockAsset = String.Format("urn:emergent-world:DLC_Tone_{0}", toneKey);

            generateManifest(outManifest, tone);
            generateXBlock(toneKey, outXblock, tone);
            generateAggregateGraph(toneKey, aggregateGraph);
        }

        private static void generateManifest(Stream outManifest, Tone tone)
        {
            var manifest = new Manifest.Tone.Manifest();
            manifest.Entries.Add(tone);
            var data = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var writer = new StreamWriter(outManifest);
            writer.Write(data);
            writer.Flush();
            outManifest.Seek(0, SeekOrigin.Begin);
        }

        private static void generateXBlock(string toneKey, Stream outXblock, Tone tone)
        {
            var game = new GameXblock<Entity>();
            var entity = new Entity
            {
                Name = String.Format("GRTonePreset_{0}", toneKey),
                Iterations = 1,
                ModelName = "GRTonePreset",
                Id = tone.PersistentID.ToLower()
            };
            var properties = entity.Properties = new List<Property>();
            var addProperty = new Action<string, object>((a, b) => properties.Add(CreateProperty(a, b.ToString())));
            addProperty("Key", tone.Key);
            addProperty("Name", tone.Name);
            game.EntitySet = new List<Entity> {entity};
            game.Serialize(outXblock);
        }

        private static void generateAggregateGraph(string toneKey, Stream aggregateGraph)
        {
            var writer = new StreamWriter(aggregateGraph);
            var guid = IdGenerator.Guid();
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"x-world\".", guid);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"emergent-world\".", guid);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"/Exports/Pedals\".", guid);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"DLC_Tone_{1}\".", guid, toneKey);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/DLC_Tone_{1}/Exports/Pedals/DLC_Tone_{1}.xblock\".", guid, toneKey);
            writer.Flush();
            aggregateGraph.Seek(0, SeekOrigin.Begin);
        }

        private static Property CreateProperty(string name, string value)
        {
            return new Property { Name = name, Set = new Set { Value = value } };
        }
    }
}
