using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using RocksmithToolkitLib.Extensions;
using System.IO;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraphRS2014 {
    public enum GraphTag {
        [Description("database")]
        Database,
        [Description("json-db")]
        JsonDB,
        [Description("hsan-db")]
        HsanDB,
        [Description("application")]
        Application,
        [Description("musicgame-song")]
        MusicgameSong,
        [Description("dds")]
        DDS,
        [Description("image")]
        Image,
        [Description("xml")]
        XML,
        [Description("audio")]
        Audio,
        [Description("wwise-sound-bank")]
        WwiseSoundBank,
        [Description("dx9")]
        DX9, //Only for Windows
        [Description("macos")]
        MacOS, //Only for MAC
        [Description("xbox360")]
        Xbox360, //Only for XBox 360
        [Description("ps3")]
        PS3, //Only for PS3
        [Description("emergent-world")]
        EmergentWorld,
        [Description("x-world")]
        XWorld
    };

    public class GraphItemLLID : GraphItem {
        public Guid LLID { get; set; }
        public string LogPath { get { return String.Format("{0}/{1}", LogPathDirectory, LogPathFile); } }
        public string LogPathDirectory { private get; set; }
        public string LogPathFile { private get; set; }

        public GraphItemLLID(Guid uuid, List<GraphPart> graphPartList)
            : base(uuid, graphPartList) {
            foreach (var graph in graphPartList) {
                var value = graph.Value.Split(new Char[] { '"' })[1];
                switch (graph.Type.Split(new Char[] { '/', '>' })[5]) {
                    case "llid":
                        LLID = Guid.Parse(value);
                        break;
                    case "logpath":
                        LogPathDirectory = Canonical;
                        LogPathFile = value.Substring(Canonical.Length + 1);
                        break;
                }
            }
        }
    }

    public class GraphItem {
        public Guid UUID { get; set; }
        public List<string> Tag { get; set; }
        public string Canonical { get; set; }
        public string Name { get; set; }
        public string RelPath { get { return String.Format("{0}/{1}", RelPathDirectory, RelPathFile); } }
        public string RelPathDirectory { private get; set; }
        public string RelPathFile { private get; set; }

        public GraphItem(Guid uuid, List<GraphPart> graphPartList) {
            UUID = uuid;
            Tag = new List<string>();

            foreach (var graph in graphPartList) {
                var value = graph.Value.Split(new Char[] { '"' })[1];
                switch (graph.Type.Split(new Char[] { '/', '>' })[5]) {
                    case "tag":
                        Tag.Add(value);
                        break;
                    case "canonical":
                        Canonical = value;
                        break;
                    case "name":
                        Name = value;
                        break;
                    case "relpath":
                        RelPathDirectory = Canonical;
                        RelPathFile = value.Substring(Canonical.Length + 1);
                        break;
                }
            }
        }
    }
}
