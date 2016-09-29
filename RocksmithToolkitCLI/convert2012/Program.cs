using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.XmlRepository;

namespace convert2012
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WindowWidth = 85;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

#if (DEBUG)
            // give the progie some dumby file to work on
            args = new string[] { "D:\\Temp\\Test" };
#endif

            // catch if there are no cmd line arguments
            if (args.GetLength(0) == 0) args = new string[] { "?" };
            if (args[0].Contains("?") || args[0].ToLower().Contains("help"))
            {
                Console.WriteLine(@"Rocksmith 2012 CDCL Converter DropletApp");
                Console.WriteLine(@" - Version: " + ProjectVersion());
                Console.WriteLine(@"   Copyright (C) 2015 CST Developers");
                Console.WriteLine();
                Console.WriteLine(@" - Purpose: Converts RS2012 CDLC dat file to RS2014 CDLC psarc file");
                Console.WriteLine();
                Console.WriteLine(@" - Usage: Drag/Drop folder with *.dat files onto the console executable icon.");
                Console.Read();
                return 0;
            }

            Console.WriteLine(@"Initializing Rocksmith 2012 CDLC Converter CLI ...");
            Console.WriteLine();
            var srcDir = args[0];

            // iterate through cdlc folders and find *.dat files
            var cdlcFilesPaths = Directory.GetFiles(srcDir, "*.dat", SearchOption.AllDirectories);
            var cdlcSaveDir = Path.Combine(Path.GetDirectoryName(cdlcFilesPaths[0]), "Converted CDLC");

            if (!Directory.Exists(cdlcSaveDir))
                Directory.CreateDirectory(cdlcSaveDir);

            foreach (var cdlcFilePath in cdlcFilesPaths)
            {
                // Unpack
                Console.WriteLine(@"Unpacking: " + Path.GetFileName(cdlcFilePath));
                var unpackedDirPath = Path.Combine(Path.GetDirectoryName(cdlcFilePath), String.Format("{0}_Pc", Path.GetFileNameWithoutExtension(cdlcFilePath)));
                var unpackedDest = Path.GetDirectoryName(cdlcFilePath);

                if (Directory.Exists(unpackedDirPath))
                    DirectoryExtension.SafeDelete(unpackedDirPath);

                try
                {
                    Packer.Unpack(cdlcFilePath, unpackedDest, true);

                    // Load Package Data
                    Console.WriteLine(@"Converting RS2012 CDLC to RS2014 CDLC ...");
                    DLCPackageData info = new DLCPackageData(); // DLCPackageData specific to RS2
                    info = DLCPackageData.RS1LoadFromFolder(unpackedDirPath, new Platform(GamePlatform.Pc, GameVersion.RS2014), true);

                    // Convert Audio to Wem Format
                    info = ConvertAudio(info);

                    // Update Album Art
                    info = ConvertAlbumArt(cdlcFilePath, info);

                    foreach (var arr in info.Arrangements)
                    {
                        Console.WriteLine(@"Converting XML Arrangement: " + arr);
                        arr.SongFile.File = "";

                        if (arr.ArrangementType != ArrangementType.Vocal)
                            UpdateXml(arr, info);
                    }

                    // Repack
                    var cdlcVersion = "c1"; // conversion 1
                    var cdlcFileName = StringExtensions.GetValidShortFileName(info.SongInfo.Artist, info.SongInfo.SongDisplayName, cdlcVersion, ConfigRepository.Instance().GetBoolean("creator_useacronyms"));
                    var cdlcSavePath = Path.Combine(cdlcSaveDir, cdlcFileName);
                    Console.WriteLine(@"Repacking as RS2014 CDLC: " + cdlcFileName + @".psarc");
                    Console.WriteLine("");
                    DLCPackageCreator.Generate(cdlcSavePath, info, new Platform(GamePlatform.Pc, GameVersion.RS2014), pnum: 1);
                    DirectoryExtension.SafeDelete(unpackedDirPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(@"Conversion could not be completed: " + ex.Message);
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine(@"Done Processing CDLC Folder ...");
            Console.WriteLine(@"Converted CDLC Saved To: " + cdlcSaveDir);
            Console.WriteLine(@"Remember ... CDLC Arrangements, Tones, Volumes, etc");
            Console.WriteLine(@"can be modified using the toolkit GUI, Creator tab");
            Console.WriteLine();
            Console.WriteLine(@"Press any key to continue ...");
            Console.Read();
            return 0;
        }

        public static void UpdateXml(Arrangement arr, DLCPackageData info)
        {
            // update xml with user modified DLCPackageData info
            var songXml = Song2014.LoadFromFile(arr.SongXml.File);
            arr.SongFile = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongFile { File = "" };
            arr.Id = IdGenerator.Guid();
            arr.MasterId = RandomGenerator.NextInt();

            songXml.AlbumName = info.SongInfo.Album;
            songXml.AlbumYear = info.SongInfo.SongYear.ToString();
            songXml.ArtistName = info.SongInfo.Artist;
            songXml.ArtistNameSort = info.SongInfo.ArtistSort;
            songXml.AverageTempo = info.SongInfo.AverageTempo;
            songXml.Title = info.SongInfo.SongDisplayName;
            songXml.ToneBase = arr.ToneBase;
            songXml.ToneA = arr.ToneA;
            songXml.ToneB = arr.ToneB;
            songXml.ToneC = arr.ToneC;
            songXml.ToneD = arr.ToneD;

            File.Delete(arr.SongXml.File);
            using (var stream = File.OpenWrite(arr.SongXml.File))
            {
                songXml.Serialize(stream, true);
            }
        }

        private static DLCPackageData ConvertAudio(DLCPackageData info)
        {
            var audioPath = info.OggPath;
            Console.WriteLine(@"Converting audio using: " + Path.GetFileName(audioPath));
            var audioPathNoExt = Path.Combine(Path.GetDirectoryName(audioPath), Path.GetFileNameWithoutExtension(audioPath));
            var oggPath = String.Format(audioPathNoExt + ".ogg");
            var wavPath = String.Format(audioPathNoExt + ".wav");
            var wemPath = String.Format(audioPathNoExt + ".wem");
            var oggPreviewPath = String.Format(audioPathNoExt + "_preview.ogg");
            var wavPreviewPath = String.Format(audioPathNoExt + "_preview.wav");
            var wemPreviewPath = String.Format(audioPathNoExt + "_preview.wem");
            var audioPreviewPath = wemPreviewPath;

            //RS1 old ogg was actually wwise
            if (audioPath.Substring(audioPath.Length - 4).ToLower() == ".ogg")
            {
                ExternalApps.Ogg2Wav(audioPath, wavPath);
                if (!File.Exists(oggPreviewPath))
                {
                    ExternalApps.Ogg2Preview(audioPath, oggPreviewPath);
                    ExternalApps.Ogg2Wav(oggPreviewPath, wavPreviewPath);
                }
                audioPath = wavPath;
            }

            if (audioPath.Substring(audioPath.Length - 4).ToLower() == ".wav")
            {
                if (!File.Exists(wavPreviewPath))
                {
                    ExternalApps.Wav2Ogg(audioPath, oggPath, 4);
                    ExternalApps.Ogg2Preview(oggPath, oggPreviewPath);
                    ExternalApps.Ogg2Wav(oggPreviewPath, wavPreviewPath);
                }
                Wwise.Convert2Wem(audioPath, wemPath, 4); // default audio quality = 4
                audioPath = wemPath;
            }

            info.OggPath = audioPath;
            info.OggPreviewPath = audioPreviewPath;

            return info;
        }

        private static DLCPackageData ConvertAlbumArt(string cdlcFilePath, DLCPackageData info)
        {
            var unpackedDirPath = Path.Combine(Path.GetDirectoryName(cdlcFilePath), String.Format("{0}_Pc", Path.GetFileNameWithoutExtension(cdlcFilePath)));
            //D:\Temp\Test\RS001SONG0000005_Pc\MoreThanAFeeling\GRAssets\AlbumArt
            // iterate through unpacked cdlc folder and find artwork
            var ddsFilesPath = Directory.GetFiles(unpackedDirPath, "*.dds", SearchOption.AllDirectories);

            if (!ddsFilesPath.Any())
            {
                Console.WriteLine(@"Using default album artwork");
                Console.ReadLine();
                return info;
            }

            try
            {
                var albumArtPath = ddsFilesPath[0];
                Console.WriteLine(@"Converting album artwork using: " + Path.GetFileName(albumArtPath));
                var ddsFiles = new List<DDSConvertedFile>();

                ddsFiles.Add(new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                ddsFiles.Add(new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                ddsFiles.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });

                // Convert to correct dds file sizes
                DLCPackageCreator.ToDDS(ddsFiles);

                var albumArtDir = Path.GetDirectoryName(albumArtPath);
                var albumArtName = String.Format("album_{0}", info.Name.ToLower().Replace("_", "").GetValidFileName());
                var ddsPartialPath = Path.Combine(albumArtDir, albumArtName);

                foreach (var dds in ddsFiles)
                {
                    var destAlbumArtPath = String.Format("{0}_{1}.dds", ddsPartialPath, dds.sizeX);
                    if (!File.Exists(dds.destinationFile))
                        Console.WriteLine(@"Could not convert: " + destAlbumArtPath);

                    File.Copy(dds.destinationFile, destAlbumArtPath);
                    // delete temp artwork file
                    File.Delete(dds.destinationFile);
                    dds.destinationFile = destAlbumArtPath;
                }

                // update package info with album art files
                info.ArtFiles = ddsFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Could not convert album artwork in " + Path.GetFileName(cdlcFilePath) + @": " + ex.Message);
            }

            return info;
        }

        private static string ProjectVersion()
        {
            return String.Format("{0}.{1}.{2}",
                                 Assembly.GetExecutingAssembly().GetName().Version.Major,
                                 Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                 Assembly.GetExecutingAssembly().GetName().Version.Build);
        }


    }

}

