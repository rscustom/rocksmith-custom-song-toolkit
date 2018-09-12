using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.XBlock;
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

        public void Write(string dlcName, string[] platformPathNames, Platform platform, Stream str)
        {
            StreamWriter writer = new StreamWriter(str);

            if (platform.IsConsole)
            {
                foreach (var x in SongFiles)
                    writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"{1}\".", x.UUID, platformPathNames[0]);
                writer.WriteLine("<urn:uuid:{0}> <http://emergent.net/aweb/1.0/tag> \"{1}\".", SoundBank.UUID, platformPathNames[0]);
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

            if (platform.platform == GamePlatform.Pc)
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


        public static List<AgGraphNt> ReadFromFile(string ntFilePath)
        {
            //<urn:uuid:5394311c-b576-4a65-9d0a-f5737d270c63> <http://emergent.net/aweb/1.0/llid> "292f0d67-0000-0000-0000-000000000000".
            List<AgGraphNt> agGraphNt = new List<AgGraphNt>();

            using (StreamReader reader = new StreamReader(ntFilePath))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;

                    var ntParts = line.Split(new Char[] { ' ' });
                    if (ntParts.Length != 3) // spaces in a song file name
                        for (int i = 3; i < ntParts.Length; i++)
                            ntParts[2] = String.Format("{0} {1}", ntParts[2], ntParts[i]);

                    var agUrn = ntParts[0].Split(new Char[] { ':', '>' })[2];
                    var agType = ntParts[1].Split(new Char[] { '/', '>' })[5];
                    var agValue = ntParts[2].Split(new Char[] { '"', '"' })[1];

                    agGraphNt.Add(new AgGraphNt()
                    {
                        AgUrn = agUrn,
                        AgType = agType,
                        AgValue = agValue
                    });
                }
            }
            return agGraphNt;
        }

        // this may hurt your brain
        public static List<AgGraphMap> ProjectMap(List<AgGraphNt> agGraphNt, XblockX songsXblock, Manifest.Tone.Manifest toneManifest)
        {
            List<AgGraphMap> agGraphRef = new List<AgGraphMap>();

            foreach (var xmlFile in agGraphNt)
            {
                if (xmlFile.AgType.ToLower() != "logpath")
                    continue;

                var valueXmlFile = xmlFile.AgValue;
                var valueUuid = xmlFile.AgUrn;

                foreach (var id in agGraphNt) // aggregateGraph
                {
                    if (id.AgType == "llid" && id.AgUrn == valueUuid)
                    {
                        var tonesList = new List<string>();
                        var expandedLLID = String.Format("{0}{1}", "urn:llid:", id.AgValue);

                        foreach (var entity in songsXblock.entitySet) // songsXblock
                        {
                            string songXmlLLID = String.Empty;
                            string effectChainName = String.Empty;

                            foreach (var property in entity.property) // songsXblock
                            {
                                if (property.name == "SongXml")
                                    songXmlLLID = property.set.value;
                                if (property.name == "EffectChainName")
                                    effectChainName = property.set.value;
                                if (String.IsNullOrEmpty(effectChainName) || String.IsNullOrEmpty(songXmlLLID))
                                    continue;

                                if (songXmlLLID == expandedLLID)
                                {
                                    foreach (var entry in toneManifest.Entries) // toneManifest                                   
                                        if (entry.Key == effectChainName)
                                            tonesList.Add(entry.Key);

                                    if (!tonesList.Any())
                                        tonesList.Add("Default");

                                    agGraphRef.Add(new AgGraphMap()
                                        {
                                            UUID = id.AgUrn,
                                            LLID = id.AgValue.Split(new Char[] { '-' })[0],
                                            SongXmlPath = valueXmlFile,
                                            Tones = tonesList // newer RS1 may have multipble tones (guitar and bass)
                                        });
                                  
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (agGraphRef == null)
                Console.WriteLine("<ERROR> Did not find AgType 'logpath' ...");

            return agGraphRef;
        }
    }

    public class AgGraphNt
    {
        public string AgUrn { get; set; }
        public string AgType { get; set; }
        public string AgValue { get; set; }
    }

    public class AgGraphMap
    {
        public string UUID { get; set; }
        public string LLID { get; set; }
        public string SongXmlPath { get; set; }
        public List<string> Tones { get; set; }
    }

}
