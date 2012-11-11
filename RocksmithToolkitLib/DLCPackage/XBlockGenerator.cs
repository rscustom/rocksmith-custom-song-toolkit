using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithDLCCreator;
using RocksmithDLCCreator.XBlock;

namespace RocksmithToolkitLib.DLCPackage
{
    public class XBlockGenerator
    {
        public static void Generate(string dlcName, Manifest.Manifest manifest, AggregateGraph.AggregateGraph aggregateGraph, Stream outStream)
        {
            var game = new Game();
            game.Entities = new List<Entity>();
            foreach (var x in manifest.Entries)
            {
                var entry = x.Value["Attributes"];
                var entity = new Entity();
                entity.Id = entry.PersistentID.ToLower();
                entity.Name = String.Format("GRSong_Asset_{0}_{1}", dlcName, entry.ArrangementName);
                entity.ModelName = "GRSong_Asset";
                entity.Iterations = 46;
                game.Entities.Add(entity);
                var properties = new List<Property>();
                var addProperty = new Action<string, object>((a, b) => properties.Add(CreateProperty(a, b.ToString())));
                addProperty("BinaryVersion", "51");
                addProperty("SongKey", entry.SongKey);
                addProperty("SongAsset", entry.SongAsset);
                addProperty("SongXml", entry.SongXml);
                addProperty("ForceUseXML", entry.ForceUseXML);
                addProperty("Shipping", entry.Shipping);
                addProperty("DisplayName", entry.DisplayName);
                
                addProperty("InputEvent", entry.InputEvent);
                addProperty("ArrangementName", entry.ArrangementName);
                addProperty("RepresentativeArrangement", entry.RepresentativeArrangement);
                if (entry.VocalsAssetId != "")
                {
                    addProperty("SongEvent", entry.SongEvent);
                    addProperty("VocalsAssetId", entry.VocalsAssetId.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    var dynVisDen = new List<object>();
                    foreach (var y in entry.DynamicVisualDensity)
                        dynVisDen.Add(y);
                    properties.Add(CreateMultiItemProperty("DynamicVisualDensity", dynVisDen));
                    addProperty("AverageTempo", entry.AverageTempo);//fix this
                    addProperty("RelativeDifficulty", entry.RelativeDifficulty);
                }
                addProperty("ArtistName", entry.ArtistName);
                addProperty("ArtistNameSort", entry.ArtistNameSort);
                addProperty("SongName", entry.SongName);
                addProperty("SongNameSort", entry.SongNameSort);
                addProperty("AlbumName", entry.AlbumName);
                addProperty("AlbumNameSort", entry.AlbumNameSort);
                addProperty("SongYear", entry.SongYear);
                addProperty("Sustain", entry.Sustain);
                addProperty("PluckedType", entry.PluckedType);
                addProperty("MasterID_Xbox360", entry.MasterID_Xbox360);
                addProperty("EffectChainName", entry.EffectChainName);
                addProperty("CrowdTempo", "Fast");//fix this
                addProperty("AlbumArt", entry.AlbumArt);
                addProperty("NonStandardChords", true);//fix this
                addProperty("DoubleStops", entry.DoubleStops);
                addProperty("PowerChords", entry.PowerChords);
                addProperty("OpenChords", entry.OpenChords);
                addProperty("PalmMutes", entry.PalmMutes);


                entity.Properties = properties;
            }
            var ent = new Entity() { Id = IdGenerator.Guid().ToString().Replace("-", ""), Name = "SoundScene0", Iterations = 1, ModelName = "SoundScene", Properties = new List<Property>() { CreateMultiItemProperty("SoundBanks", new string[1] { aggregateGraph.SoundBank.Name + ".bnk" }) } };
            game.Entities.Add(ent);
            game.Serialize(outStream);
        }
        private static Property CreateProperty(string name, string value)
        {
            return new Property() { Name = name, Set = new Set() { Value = value } };
        }
        private static Property CreateMultiItemProperty(string name, IEnumerable<object> values)
        {
            var prop = new Property() { Name = name };
            var multiItemSet = new MultiItemSet();
            multiItemSet.Values = new List<string>();
            foreach (var x in values)
                multiItemSet.Values.Add(x.ToString());
            prop.Set = multiItemSet;
            return prop;
        }
    }
}
