using System;
using System.Collections.Generic;
using System.Linq;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph2014 {
    public class GraphPart {
        private const string URN_TEMPLATE = "<urn:uuid:{0}>";
        private const string TYPE_TEMPLATE = "<http://" + "emergent.net/aweb/1.0/{0}>";
        private const string VALUE_TEMPLATE = "\"{0}\".";
        
        public string Urn { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public Guid UUID {
            get {
                return Guid.Parse(Urn.Split(new Char[] { ':', '>' })[2]);
            }
        }

        public GraphPart(string graphLine) {
            var values = graphLine.Split(new Char[] { ' ' });
            Urn = values[0];
            Type = values[1];
            Value = values[2];
        }

        public static GraphPart SingleByValue(List<GraphPart> graphPartList, string value) {
            return graphPartList.SingleOrDefault<GraphPart>(g => g.Type == String.Format(TYPE_TEMPLATE, "tag") && g.Value == String.Format(VALUE_TEMPLATE, value));
        }

        public static List<GraphPart> WhereByUUID(List<GraphPart> graphPartList, Guid uuid) {
            return graphPartList.Where<GraphPart>(g => g.Urn == String.Format(URN_TEMPLATE, uuid)).ToList<GraphPart>();
        }

        public static List<GraphPart> WhereByValue(List<GraphPart> graphPartList, string value) {
            return graphPartList.Where<GraphPart>(g => g.Type == String.Format(TYPE_TEMPLATE, "tag") && g.Value == String.Format(VALUE_TEMPLATE, value)).ToList<GraphPart>();
        }

        public static List<GraphPart> WhereByType(List<GraphPart> graphPartList, string value) {
            return graphPartList.Where<GraphPart>(g => g.Type == String.Format(TYPE_TEMPLATE, value)).ToList<GraphPart>();
        }

        public static List<GraphPart> GetGraphParts(string[] graphLineArray) {
            var graphPartList = new List<GraphPart>();
            foreach (var g in graphLineArray)
                if (!String.IsNullOrEmpty(g))
                    graphPartList.Add(new GraphPart(g));
            return graphPartList;
        }
    }
}
