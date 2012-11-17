using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.XBlock;

namespace RocksmithToolkitLib.DLCPackage
{
    public class ToneGenerator
    {
        public static void Generate(string dlcName, Tone.Tone tone, Stream outManifest, Stream outXblock, Stream aggregateGraph)
        {
            var id = IdGenerator.Guid().ToString().Replace("-", "").ToUpper();
            tone.Key = dlcName;
            tone.PersistentID = id;
            tone.BlockAsset = String.Format("urn:emergent-world:DLC_Tone_{0}", dlcName);

            generateManifest(outManifest, tone);
            generateXBlock(dlcName, outXblock, tone);
            generateAggregateGraph(dlcName, aggregateGraph);
        }

        private static void generateManifest(Stream outManifest, Tone.Tone tone)
        {
            var manifest = new Tone.Manifest();
            manifest.Entries.Add(tone);
            var data = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var writer = new StreamWriter(outManifest);
            writer.Write(data);
            writer.Flush();
            outManifest.Seek(0, SeekOrigin.Begin);
        }

        private static void generateXBlock(string dlcName, Stream outXblock, Tone.Tone tone)
        {
            var game = new Game();
            var entity = new Entity
            {
                Name = String.Format("GRTonePreset_{0}", dlcName),
                Iterations = 1,
                ModelName = "GRTonePreset",
                Id = tone.PersistentID.ToLower()
            };
            var properties = entity.Properties = new List<Property>();
            var addProperty = new Action<string, object>((a, b) => properties.Add(CreateProperty(a, b.ToString())));
            addProperty("Key", tone.Key);
            addProperty("Name", tone.Name);
            game.Entities = new List<Entity> {entity};
            game.Serialize(outXblock);
        }

        private static void generateAggregateGraph(string dlcName, Stream aggregateGraph)
        {
            var writer = new StreamWriter(aggregateGraph);
            var guid = IdGenerator.Guid();
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"x-world\".", guid);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"emergent-world\".", guid);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"/Exports/Pedals\".", guid);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"DLC_Tone_{1}\".", guid, dlcName);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/DLC_Tone_{1}/Exports/Pedals/DLC_Tone_{1}.xblock\".", guid, dlcName);
            writer.Flush();
            aggregateGraph.Seek(0, SeekOrigin.Begin);
        }

        private static Property CreateProperty(string name, string value)
        {
            return new Property { Name = name, Set = new Set { Value = value } };
        }
    }
}
