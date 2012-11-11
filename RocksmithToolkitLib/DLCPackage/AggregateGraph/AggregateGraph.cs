using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithDLCCreator;

namespace RocksmithToolkitLib.DLCPackage.AggregateGraph
{
    public class AggregateGraph
    {
        public List<SongFile> SongFiles { get; private set; }
        public List<SongXML> SongXMLs { get; private set; }
        public AlbumArt AlbumArt { get; set; }
        public SoundBank SoundBank { get; set; }
        public XBlockFile XBlock { get; set; }
        public AggregateGraph()
        {
            SongFiles = new List<SongFile>();
            SongXMLs = new List<SongXML>();
        }
        public void Write(string dlcName, Stream str)
        {
            StreamWriter writer = new StreamWriter(str);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"application\".", x.UUID);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"application\".", x.UUID);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"musicgame-song\".", x.UUID);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"xml\".", x.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"image\".", AlbumArt.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"dds\".", AlbumArt.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"audio\".", SoundBank.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"wwise-sound-bank\".", SoundBank.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"DX9\".", SoundBank.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"x-world\".", XBlock.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"emergent-world\".", XBlock.UUID);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/llid> \"{1}\".", x.UUID, x.LLID);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/llid> \"{1}\".", x.UUID, x.LLID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/llid> \"{1}\".", AlbumArt.UUID, AlbumArt.LLID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/llid> \"{1}\".", SoundBank.UUID, SoundBank.LLID);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"{1}\".", x.UUID, "/GRExports/Generic");
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"{1}\".", x.UUID, "/GR/Behaviors/Songs");
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"{1}\".", AlbumArt.UUID, "/GRAssets/AlbumArt");
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"{1}\".", SoundBank.UUID, "/Audio/Windows");
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"{1}\".", XBlock.UUID, "/Exports/Songs");
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", x.UUID, x.Name);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", x.UUID, x.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", AlbumArt.UUID, AlbumArt.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", SoundBank.UUID, SoundBank.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", XBlock.UUID, XBlock.Name);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{2}/GRExports/Generic/{1}.sng\".", x.UUID, x.Name, dlcName);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{2}/GR/Behaviors/Songs/{1}.xml\".", x.UUID, x.Name, dlcName);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{2}/GRAssets/AlbumArt/{1}.dds\".", AlbumArt.UUID, AlbumArt.Name, dlcName);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{2}/Audio/Windows/{1}.bnk\".", SoundBank.UUID, SoundBank.Name, dlcName);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{2}/Exports/Songs/{1}.xblock\".", XBlock.UUID, XBlock.Name, dlcName);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/grexports/{1}.sng\".", x.UUID, x.Name.ToLower());
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/gr/behaviors/songs/{1}.xml\".", x.UUID, x.Name.ToLower());
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/grassets/albumart/{1}.dds\".", AlbumArt.UUID, AlbumArt.Name.ToLower());
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/audio/windows/{1}.bnk\".", SoundBank.UUID, SoundBank.Name.ToLower());
            writer.Flush();
        }
    }
}
