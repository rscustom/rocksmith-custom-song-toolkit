using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithToolkitLib.Sng;

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
        public void Write(string dlcName, string[] platformPathNames, GamePlatform platform, Stream str)
        {
            StreamWriter writer = new StreamWriter(str);

            if (platform == GamePlatform.XBox360)
            {
                foreach (var x in SongFiles)
                    writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"Xbox360\".", x.UUID);
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"Xbox360\".", SoundBank.UUID);
            }

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
            
            if (platform == GamePlatform.Pc)
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
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"/GRExports/{1}\".", x.UUID, platformPathNames[1]);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"/GR/Behaviors/Songs\".", x.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"/GRAssets/AlbumArt\".", AlbumArt.UUID);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"/Audio/{1}\".", SoundBank.UUID, platformPathNames[0]);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/canonical> \"/Exports/Songs\".", XBlock.UUID);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", x.UUID, x.Name);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", x.UUID, x.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", AlbumArt.UUID, AlbumArt.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", SoundBank.UUID, SoundBank.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/name> \"{1}\".", XBlock.UUID, XBlock.Name);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{1}/GRExports/{2}/{3}.sng\".", x.UUID, dlcName, platformPathNames[1], x.Name);
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{1}/GR/Behaviors/Songs/{2}.xml\".", x.UUID, dlcName, x.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{1}/GRAssets/AlbumArt/{2}.dds\".", AlbumArt.UUID, dlcName, AlbumArt.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{1}/Audio/{2}/{3}.bnk\".", SoundBank.UUID, dlcName, platformPathNames[0], SoundBank.Name);
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/relpath> \"/ContentMounts/{1}/Exports/Songs/{2}.xblock\".", XBlock.UUID, dlcName, XBlock.Name);
            foreach (var x in SongFiles)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/grexports/{1}.sng\".", x.UUID, x.Name.ToLower());
            foreach (var x in SongXMLs)
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/gr/behaviors/songs/{1}.xml\".", x.UUID, x.Name.ToLower());
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/grassets/albumart/{1}.dds\".", AlbumArt.UUID, AlbumArt.Name.ToLower());
            writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/logpath> \"/audio/{1}/{2}.bnk\".", SoundBank.UUID, platformPathNames[0].ToLower(), SoundBank.Name.ToLower());
            writer.Flush();
        }
    }
}
