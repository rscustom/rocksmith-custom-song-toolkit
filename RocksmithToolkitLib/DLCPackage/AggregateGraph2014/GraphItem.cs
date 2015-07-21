using System;
using System.Collections.Generic;
using System.ComponentModel;
using RocksmithToolkitLib.Extensions;
using System.IO;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph2014 {
    public enum TagType
    {
        tag,
        llid,
        canonical,
        name,
        relpath,
        logpath
    };

    public enum TagValue {
        [Description("database")]
        Database,
        [Description("json-db")]
        JsonDB,
        [Description("hson-db")]
        HsonDB,
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
        XWorld,
        [Description("gamebryo-scenegraph")]
        GamebryoSceneGraph
    };

    public class GraphItemLLID : GraphItem {
        [Description("llid")]
        public Guid LLID { get; set; }
        [Description("logpath")]
        public string LogPath { get { return String.Format("{0}/{1}", LogPathDirectory, LogPathFile); } }
        public string LogPathDirectory { private get; set; }
        public string LogPathFile { private get; set; }

        public GraphItemLLID() {}

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

        public override void Write(StreamWriter writer, bool lastLine = false) {
            var uuid = UUID.ToString().ToLower();

            base.Write(writer, lastLine);//hope it's ok to pass lastLine here.
            writer.WriteLine(GRAPHLINETEMPLATE, uuid, TagType.llid, LLID);

            var line = String.Format(GRAPHLINETEMPLATE, uuid, TagType.logpath, LogPath);
            if (!lastLine)
                writer.WriteLine(line);
            else
                writer.Write(line); //Xbox crashes if this file have a blank line in the end
        }
    }

    public class GraphItem {
        public static readonly string GRAPHLINETEMPLATE = "<urn:uuid:{0}> <http://" + "emergent.net/aweb/1.0/{1}> \"{2}\".";

        public Guid UUID { get; set; }
        [Description("tag")]
        public List<string> Tag { get; set; }
        [Description("canonical")]
        public string Canonical { get; set; }
        [Description("name")]
        public string Name { get; set; }
        [Description("relpath")]
        public string RelPath { get { return String.Format("{0}/{1}", RelPathDirectory, RelPathFile); } }
        public string RelPathDirectory { get; set; }
        public string RelPathFile { get; set; }

        public GraphItem() {}

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

        public virtual void Write(StreamWriter writer, bool lastLine = false) {
            var uuid = UUID.ToString().ToLower();

            foreach (var tag in Tag)
                writer.WriteLine(GRAPHLINETEMPLATE, uuid, TagType.tag, tag);

            writer.WriteLine(GRAPHLINETEMPLATE, uuid, TagType.canonical, Canonical);
            writer.WriteLine(GRAPHLINETEMPLATE, uuid, TagType.name, Name);

            var line = String.Format(GRAPHLINETEMPLATE, uuid, TagType.relpath, RelPath);
            if (!lastLine)
                writer.WriteLine(line);
            else
                writer.Write(line);
        }

        public static string GetPlatformTagDescription(GamePlatform platform)
        {
            switch (platform)
            {
                case GamePlatform.Pc:
                    return TagValue.DX9.GetDescription();
                case GamePlatform.Mac:
                    return TagValue.MacOS.GetDescription();
                case GamePlatform.XBox360:
                    return TagValue.Xbox360.GetDescription();
                case GamePlatform.PS3:
                    return TagValue.PS3.GetDescription();
                default:
                    throw new InvalidOperationException("Unexpected game platform value");
            }
        }
    }
}
