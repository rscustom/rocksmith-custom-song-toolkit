using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;

namespace packagecreator
{
    // TODO: include functionality for RS2012
    internal class Arguments
    {
        public bool ShowHelp;
        public bool Package;
        public string[] Input;
        public string Output;
        public Platform Platform;
        public string AppId;
        public string Revision;
        public string Quality;
        public string Decibels;
    }

    static class Program
    {
        private static Arguments DefaultArguments()
        {
            return new Arguments
            {
                Package = true,
                Platform = new Platform(GamePlatform.Pc, GameVersion.RS2014),
                AppId = "248750",  // RS1 => 206102 not currently supported
                Revision = "1",
                Quality = "4", // not currently used
                Decibels = "-12"
            };
        }

        private static OptionSet GetOptions(Arguments outputArguments)
        {
            return new OptionSet
            {
                { "p|PackageCreator", "Usage: Drag/Drop a root directory " +
                  "that contains songname subfolders that each contain CDLC ready files onto the executable application icon:\r\n" +                 
                  "RS2014 *.json [lead, rhythm, combos, bass]\r\n"+
                  "RS2014 *.xml [lead, rhythm, combos, bass]\r\n"+
                  "RS2014 Vocals.xml and Vocals.json (optional)\r\nAlbumArt256.dds\r\n" + 
                  "Wwise 2013 Audio.wem\r\nWwise 2013 Audio_preview.wem\r\n", v => { if (v != null) outputArguments.Package = true; }},                
                { "-|--------------", "Alternate Command Line Usage is shown below:\r\n", v => { if (v != null) outputArguments.Package = true; }},                
                { "h|?|help", "Show this help message and exit", v => outputArguments.ShowHelp = v != null },
                { "i|input=", "Input directory (multiple allowed, use ; to split paths)", v => outputArguments.Input = v.Split( new[]{';'}, 2) },
                { "o|output=", "Output directory", v => outputArguments.Output = v },
                { "f|platform=", "Package Platform [Pc, Mac, XBox360, PS3]", v => outputArguments.SetPlatform(v) },
                { "v|version=", "Rocksmith Game Version [RS2014]", v => outputArguments.SetVersion(v) },
                { "a|appid=", "Rocksmith APP ID", v => { if (v != null) outputArguments.AppId = v; }},
                { "r|revision=", "CDLC Revision [1 to 9, or 1.0 to 9.9]", v => { if (v != null) outputArguments.Revision = v; }},
                { "q|quality=", "Audio Quality [4 to 9]", v => { if (v != null) outputArguments.Quality = v; }},
                { "d|decibels=", "Audio Volume [HIGHER -1, AVERAGE -12, -16 LOWER]", v => outputArguments.Output = v }
            };
        }

        private static void SetPlatform(this Arguments arguments, string platformString)
        {
            GamePlatform p;
            var validPlatform = Enum.TryParse(platformString, true, out p);
            if (!validPlatform)
            {
                ShowHelpfulError(String.Format("{0} is not a valid platform.", platformString));
                arguments.Platform.platform = GamePlatform.None;
            }
            arguments.Platform.platform = p;
        }

        private static void SetVersion(this Arguments arguments, string versionString)
        {
            GameVersion v;
            var validVersion = Enum.TryParse(versionString, true, out v);
            if (!validVersion)
            {
                ShowHelpfulError(String.Format("{0} is not a valid game version.", versionString));
                arguments.Platform.version = GameVersion.None;
            }
            arguments.Platform.version = v;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Console.WindowWidth = 85;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

#if (DEBUG)
            // give the progie some dumby directory to work on for testing
            // args = new string[] { "--input=D:\\Temp\\Test", "--output=D:\\Temp" }; //, "platform=Pc", "version=RS2014" };
            args = new string[] { "D:\\Temp\\Test" };
#endif

            var arguments = DefaultArguments();
            var options = GetOptions(arguments);
            string[] srcDirs = null;
            options.Parse(args);

            try
            {
                // drag/drop a directory onto executable application
                if (arguments.Input == null && args.GetLength(0) != 0)
                {
                    try
                    {
                        if (args[0].IsDirectory())
                        {
                            srcDirs = args;
                            if (srcDirs.Length == 1)
                            {
                                srcDirs = Directory.GetDirectories(srcDirs[0]);
                                arguments.Output = Path.GetDirectoryName(srcDirs[0]);
                            }
                            else
                                arguments.Output = Path.GetDirectoryName(args[0]);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowHelpfulError(ex.Message + "  Check that RootFolder structure has SubFolder(s).");
                        return 1; // failure
                    }
                }
                else // command line error checking 
                {
                    if (arguments.ShowHelp || args.GetLength(0) == 0)
                    {
                        options.WriteOptionDescriptions(Console.Out);
                        Console.ReadLine();
                        return -1; // neither success or failure
                    }

                    if (!arguments.Package)
                    {
                        ShowHelpfulError("Must specify the primary command as 'package'");
                        return 1;
                    }

                    if (arguments.Package)
                    {
                        if (!arguments.Input[0].IsDirectory() || (arguments.Input == null && arguments.Input.Length <= 0))
                        {
                            ShowHelpfulError("Must specify and 'input' directory.");
                            return 1;
                        }

                        if (string.IsNullOrEmpty(arguments.Output))
                        {
                            ShowHelpfulError("Must specify an 'output' directory.");
                            return 1;
                        }
                    }

                    if ((arguments.Platform.platform == GamePlatform.None && arguments.Platform.version != GameVersion.None) || (arguments.Platform.platform != GamePlatform.None && arguments.Platform.version == GameVersion.None))
                    {
                        ShowHelpfulError("'platform' argument requires 'version' and vice-versa to define platform.\r\nUse this option only if you have problem with platform auto identifier");
                        return 1;
                    }

                    srcDirs = arguments.Input;
                }

                Console.WriteLine(@"Initializing Package Creator CLI ...");
                Console.WriteLine("");

                var songCount = srcDirs.Length;
                for (int i = 0; i < songCount; i++)
                {
                    Console.WriteLine(@"Parsing Input Directory (" + (i + 1) + @"/" + songCount + @") for CDLC Package Data: " + Path.GetFileName(srcDirs[i]));

                    try
                    {

                        // get package data
                        DLCPackageData packageData = DLCPackageData.LoadFromFolder(srcDirs[i], arguments.Platform, arguments.Platform);
                        packageData.AppId = arguments.AppId;
                        packageData.PackageVersion = arguments.Revision;
                        packageData.Name = Path.GetFileName(srcDirs[i]).GetValidName();
                        packageData.Volume = packageData.Volume == 0 ? Convert.ToInt16(arguments.Decibels) : packageData.Volume;
                        packageData.PreviewVolume = packageData.PreviewVolume == 0 ? Convert.ToInt16(arguments.Decibels) : packageData.PreviewVolume;

                        // check Album Artwork
                        if (arguments.Platform.version == GameVersion.RS2014)
                            CheckAlbumArt(srcDirs[i], packageData.Name);

                        // generate CDLC file name
                        var artist = packageData.SongInfo.ArtistSort;
                        var title = packageData.SongInfo.SongDisplayNameSort;
                        // var destDir = Path.Combine(arguments.Output, Path.GetFileName(srcDirs[i]).GetValidName());
                        var fileName = GeneralExtensions.GetShortName("{0}_{1}_v{2}", artist, title, arguments.Revision.Replace(".", "_"), ConfigRepository.Instance().GetBoolean("creator_useacronyms"));
                        var destPath = Path.Combine(arguments.Output, fileName);
                        var fullFileName = String.Format("{0}{1}.psarc", fileName, DLCPackageCreator.GetPathName(arguments.Platform)[2]);
                        Console.WriteLine(@"Packing: " + Path.GetFileName(fullFileName));
                        Console.WriteLine("");
                        // pack the data
                        DLCPackageCreator.Generate(destPath, packageData, new Platform(arguments.Platform.platform, arguments.Platform.version));
                        packageData.CleanCache();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(String.Format("Packaging error!\nDirectory: {0}\n{1}\n{2}", srcDirs[i], ex.Message, ex.InnerException));
                        Console.ReadLine();
                    }
                }

                Console.WriteLine(@"All Finished");
                Console.WriteLine(@"Press any key to continue ...");
                Console.ReadLine();
                return 0; // success
            }
            catch (Exception ex)
            {
                ShowHelpfulError(ex.Message);
                return 1; // failure
            }
        }

        static void ShowHelpfulError(string message)
        {
            Console.Write("packagecreator: ");
            Console.WriteLine(message);
            Console.WriteLine("Try 'packagecreator --help' for more information.");
            Console.ReadLine();
        }

        private static bool IsDirectory(this string path)
        {
            bool isDirectory = false;

            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    isDirectory = true;
            }
            catch (Exception ex)
            {
                ShowHelpfulError("Invalid directory." + Environment.NewLine + ex.Message);
            }

            return isDirectory;
        }

        private static void CheckAlbumArt(string srcDir, string dlcName)
        {
            // iterate through unpacked cdlc src folder and find artwork
            var ddsFilesPath = Directory.GetFiles(srcDir, "album_*.dds", SearchOption.AllDirectories);

            if (!ddsFilesPath.Any())
            {
                Console.WriteLine(@"Did not find any album artwork in:" + Environment.NewLine + srcDir);
                Console.WriteLine("");
                Console.ReadLine();
            }

            try
            {
                bool is64 = false, is128 = false, is256 = false;
                string albumArtPath = String.Empty;

                foreach (var ddsFile in ddsFilesPath)
                {
                    if (ddsFile.Contains("_64"))
                        is64 = true;
                    if (ddsFile.Contains("_128"))
                        is128 = true;
                    if (ddsFile.Contains("_256"))
                    {
                        is256 = true;
                        albumArtPath = ddsFile;
                    }
                }

                // do not update psarc if album artwork if already valid
                if (is64 && is128 && is256)
                {
                    Console.WriteLine(@"Artwork is valid.");
                    Console.WriteLine("");
                    return;
                }

                if (String.IsNullOrEmpty(albumArtPath))
                    albumArtPath = ddsFilesPath[0];

                Console.WriteLine(@"Repairing album artwork using: " + Path.GetFileName(albumArtPath));
                var ddsFiles = new List<DDSConvertedFile>();

                if (!albumArtPath.Contains("_64"))
                    ddsFiles.Add(new DDSConvertedFile() { sizeX = 64, sizeY = 64, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                if (!albumArtPath.Contains("_128"))
                    ddsFiles.Add(new DDSConvertedFile() { sizeX = 128, sizeY = 128, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });
                if (!albumArtPath.Contains("_256"))
                    ddsFiles.Add(new DDSConvertedFile() { sizeX = 256, sizeY = 256, sourceFile = albumArtPath, destinationFile = GeneralExtensions.GetTempFileName(".dds") });

                // Convert to correct dds file sizes
                DLCPackageCreator.ToDDS(ddsFiles);

                var albumArtDir = Path.GetDirectoryName(albumArtPath);
                var albumArtName = String.Format("album_{0}", dlcName.ToLower().Replace("_", "").GetValidName());
                var ddsPartialPath = Path.Combine(albumArtDir, albumArtName);

                foreach (var dds in ddsFiles)
                {
                    var destAlbumArtPath = String.Format("{0}_{1}.dds", ddsPartialPath, dds.sizeX);
                    if (!File.Exists(dds.destinationFile))
                        Console.WriteLine(@"Could not repair: " + destAlbumArtPath);

                    File.Copy(dds.destinationFile, destAlbumArtPath);
                    // delete temp artwork file
                    File.Delete(dds.destinationFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.ReadLine();
            }
        }



    }
}
