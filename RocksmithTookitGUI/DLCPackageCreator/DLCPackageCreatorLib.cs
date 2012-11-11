using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RocksmithDLCCreator;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public static class DLCPackageCreatorLib
    {
        public static void GenerateDLCPackage(string packagePath, DLCPackageData info)
        {
            var dlcName = info.Name;
            var soundBankName = String.Format("Song_{0}", dlcName);
            var songPsarc = new PSARC.PSARC();
            var packPsarc = new PSARC.PSARC();
            using (var appId = new MemoryStream())
            using (var packageList = new MemoryStream())
            using (var aggregateGraph = new MemoryStream())
            using (var manifest = new MemoryStream())
            using (var xblock = new MemoryStream())
            using (var soundbank = new MemoryStream())
            using (var packageId = new MemoryStream())
            using (var pack = new MemoryStream())
            using (var toneManifestStream = new MemoryStream())
            using (var toneXblockStream = new MemoryStream())
            using (var toneAggregateGraph = new MemoryStream())
            using (var packIdStr = new MemoryStream())
            using (var toneStr = new MemoryStream())
            using (var finishedPack = new MemoryStream())
            using (var albumArt = File.OpenRead(info.AlbumArtPath))
            {
                var writer = new StreamWriter(appId);
                writer.Write("206113");
                writer.Flush();
                appId.Seek(0, SeekOrigin.Begin);
                packPsarc.AddEntry("APP_ID", appId);

                writer = new StreamWriter(packageList);
                writer.WriteLine(dlcName);
                writer.WriteLine("DLC_Tone_{0}", dlcName);
                writer.Flush();
                packageList.Seek(0, SeekOrigin.Begin);
                packPsarc.AddEntry("PackageList.txt", packageList);

                var manifestBuilder = new ManifestBuilder
                {
                    AlbumName = info.Album,
                    AggregateGraph = new AggregateGraph
                    {
                        SoundBank = new SoundBank { File = soundBankName + ".bnk" },
                        AlbumArt = new AlbumArt { File = info.AlbumArtPath }
                    }
                };

                foreach (var x in info.Arrangements)
                {
                    manifestBuilder.AggregateGraph.SongFiles.Add(x.SongFile);
                    manifestBuilder.AggregateGraph.SongXMLs.Add(x.SongXml);
                }
                manifestBuilder.AggregateGraph.XBlock = new XBlockFile { File = dlcName + ".xblock" };

                manifestBuilder.AggregateGraph.Write(dlcName, aggregateGraph);
                aggregateGraph.Flush();
                aggregateGraph.Seek(0, SeekOrigin.Begin);
                var manifestData = manifestBuilder.GenerateManifest(dlcName, info.Arrangements);
                writer = new StreamWriter(manifest);
                writer.Write(manifestData);
                writer.Flush();
                manifest.Seek(0, SeekOrigin.Begin);

                XBlockGenerator.Generate(dlcName, manifestBuilder.Manifest, manifestBuilder.AggregateGraph, xblock);
                xblock.Flush();
                xblock.Seek(0, SeekOrigin.Begin);

                var soundFileName = SoundBankGenerator.GenerateSoundBank("Sound_" + dlcName, File.OpenRead(info.OggPath), soundbank);
                soundbank.Flush();
                soundbank.Seek(0, SeekOrigin.Begin);

                writer = new StreamWriter(packageId);
                writer.Write(dlcName);
                writer.Flush();
                packageId.Seek(0, SeekOrigin.Begin);

                songPsarc.AddEntry("PACKAGE_ID", packageId);
                songPsarc.AddEntry("AggregateGraph.nt", aggregateGraph);
                songPsarc.AddEntry("Manifests/songs.manifest.json", manifest);
                songPsarc.AddEntry(String.Format("Exports/Songs/{0}.xblock", dlcName), xblock);
                songPsarc.AddEntry(String.Format("Audio/Windows/{0}.bnk", soundBankName), soundbank);
                songPsarc.AddEntry(String.Format("Audio/Windows/{0}.ogg", soundFileName), File.OpenRead(info.OggPath));

                songPsarc.AddEntry(String.Format("GRAssets/AlbumArt/{0}.dds", manifestBuilder.AggregateGraph.AlbumArt.Name), albumArt);

                foreach (var x in info.Arrangements)
                {
                    songPsarc.AddEntry(String.Format("GR/Behaviors/Songs/{0}.xml", Path.GetFileNameWithoutExtension(x.SongXml.File)), File.OpenRead(x.SongXml.File));
                    songPsarc.AddEntry(String.Format("GRExports/Generic/{0}.sng", Path.GetFileNameWithoutExtension(x.SongFile.File)), File.OpenRead(x.SongFile.File));
                }
                songPsarc.Write(pack);
                pack.Flush();
                pack.Seek(0, SeekOrigin.Begin);
                packPsarc.AddEntry(String.Format("{0}.psarc", dlcName), pack);

                writer = new StreamWriter(packIdStr);
                writer.WriteLine("DLC_Tone_{0}", dlcName);
                writer.Flush();
                packIdStr.Seek(0, SeekOrigin.Begin);
                ToneGenerator.Generate(dlcName, toneManifestStream, toneXblockStream, toneAggregateGraph);
                var tonePsarc = new PSARC.PSARC();
                tonePsarc.AddEntry(String.Format("Exports/Pedals/DLC_Tone_{0}", dlcName), toneXblockStream);
                tonePsarc.AddEntry("Manifests/tone.manifest.json", toneManifestStream);
                tonePsarc.AddEntry("AggregateGraph.nt", toneAggregateGraph);
                tonePsarc.AddEntry("PACKAGE_ID", packIdStr);
                tonePsarc.Write(toneStr);
                packPsarc.AddEntry(String.Format("DLC_Tone_{0}.psarc", dlcName), toneStr);

                packPsarc.Write(finishedPack);
                finishedPack.Flush();
                finishedPack.Seek(0, SeekOrigin.Begin);
                using (var fl = File.Create(packagePath))
                using (var rij = CreateRijndael())
                    Crypto(finishedPack, fl, rij.CreateEncryptor());
            }
        }

        private static RijndaelManaged CreateRijndael()
        {
            return new RijndaelManaged
            {
                Padding = PaddingMode.None,
                Mode = CipherMode.ECB,
                BlockSize = 128,
                IV = new byte[16],
                Key = new byte[]
                {
                    0xFA, 0x6F, 0x4F, 0x42, 0x3E, 0x66, 0x9F, 0x9E,
                    0x6A, 0xD2, 0x3A, 0x2F, 0x8F, 0xE5, 0x81, 0x88,
                    0x63, 0xD9, 0xB8, 0xFD, 0xED, 0xDF, 0xFE, 0xBD,
                    0x12, 0xB2, 0x7F, 0x76, 0x80, 0xD1, 0x51, 0x41
                }
            };
        }

        private static void Crypto(Stream input, Stream output, ICryptoTransform transform)
        {
            var buffer = new byte[1024];
            using (var cs = new CryptoStream(output, transform, CryptoStreamMode.Write))
            {
                for (long i = 0; i < input.Length; i += buffer.Length)
                {
                    int sz = (int)Math.Min(input.Length - input.Position, buffer.Length);
                    input.Read(buffer, 0, sz);
                    cs.Write(buffer, 0, sz);
                }
                int pad = buffer.Length - (int)(input.Length % buffer.Length);
                if (pad > 0)
                    cs.Write(new byte[pad], 0, pad);
                cs.Flush();
                output.Flush();
                output.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
