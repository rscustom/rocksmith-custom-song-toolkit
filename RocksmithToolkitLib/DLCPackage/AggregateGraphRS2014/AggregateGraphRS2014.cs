using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraphRS2014
{
    public class AggregateGraphRS2014
    {   
        public List<GraphItem> JsonDB { get; set; }
        public GraphItem HsanDB { get; set; }
        public List<GraphItemLLID> MusicgameSong { get; set; }
        public List<GraphItemLLID> SongXml { get; set; }
        public GraphItemLLID ShowlightXml { get; set; }
        public List<GraphItemLLID> AlbumArt { get; set; }
        public List<GraphItemLLID> Soundbank { get; set; }
        public GraphItem GameXblock { get; set; }

        public static AggregateGraphRS2014 LoadFromFile(string agregateGraphRS2014File) {
            AggregateGraphRS2014 aggregateGraph = new AggregateGraphRS2014(); ;
            var graphPartList = GraphPart.GetGraphParts(File.ReadAllLines(agregateGraphRS2014File));

            var json = GraphPart.WhereByValue(graphPartList, GraphTag.JsonDB.GetDescription());
            if (json.Count() > 0) {
                aggregateGraph.JsonDB = new List<GraphItem>();
                foreach (var j in json) {
                    aggregateGraph.JsonDB.Add(new GraphItem(j.UUID, GraphPart.WhereByUUID(graphPartList, j.UUID)));
                }
            }

            var hsan = GraphPart.SingleByValue(graphPartList, GraphTag.HsanDB.GetDescription());
            if (hsan != null)
                aggregateGraph.HsanDB = new GraphItem(hsan.UUID, GraphPart.WhereByUUID(graphPartList, hsan.UUID));

            var sng = GraphPart.WhereByValue(graphPartList, GraphTag.MusicgameSong.GetDescription());
            if (sng.Count() > 0) {
                aggregateGraph.MusicgameSong = new List<GraphItemLLID>();
                foreach (var s in sng)
                    aggregateGraph.MusicgameSong.Add(new GraphItemLLID(s.UUID, GraphPart.WhereByUUID(graphPartList, s.UUID)));
            }

            var xml = GraphPart.WhereByValue(graphPartList, GraphTag.XML.GetDescription());
            if (xml.Count() > 0) {
                foreach (var x in xml) {
                    aggregateGraph.SongXml = new List<GraphItemLLID>();
                    var graphList = GraphPart.WhereByUUID(graphPartList, x.UUID);
                    if (graphList.Exists(p => p.Value.Contains("showlights")))
                        aggregateGraph.ShowlightXml = new GraphItemLLID(x.UUID, graphList);
                    else
                        aggregateGraph.SongXml.Add(new GraphItemLLID(x.UUID, graphList));
                }
            }

            var dds = GraphPart.WhereByValue(graphPartList, GraphTag.Image.GetDescription());
            if (dds.Count() > 0) {
                aggregateGraph.AlbumArt = new List<GraphItemLLID>();
                foreach (var d in dds)
                    aggregateGraph.AlbumArt.Add(new GraphItemLLID(d.UUID, GraphPart.WhereByUUID(graphPartList, d.UUID)));
            }

            var bnk = GraphPart.WhereByValue(graphPartList, GraphTag.WwiseSoundBank.GetDescription());
            if (bnk.Count() > 0) {
                aggregateGraph.Soundbank = new List<GraphItemLLID>();
                foreach (var b in bnk)
                    aggregateGraph.Soundbank.Add(new GraphItemLLID(b.UUID, GraphPart.WhereByUUID(graphPartList, b.UUID)));
            }

            var xblock = GraphPart.SingleByValue(graphPartList, GraphTag.XWorld.GetDescription());
            aggregateGraph.GameXblock = new GraphItem(xblock.UUID, GraphPart.WhereByUUID(graphPartList, xblock.UUID));


            return aggregateGraph;
        }
    }
}
