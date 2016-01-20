using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using NDesk.Options;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib;
using System.Reflection;
using X360.STFS;
using RocksmithToolkitLib.Xml;

namespace packer
{
    internal class Arguments
    {
        public bool ShowHelp;
        public string[] Input;
        public string Output;
        public string Template;
        public bool Pack;
        public bool Unpack;
        public bool Build;
        public bool DecodeOGG;
        public bool UpdateSng;
        public bool UpdateManifest;
        public bool OverwriteSongXml;
        public Platform Platform;
    }

    static class Program
    {
        private static Arguments DefaultArguments()
        {
            return new Arguments
            {
                DecodeOGG = true
            };
        }

        private static OptionSet GetOptions(Arguments outputArguments)
        {
            return new OptionSet
            {
                { "h|?|help", "Show this help message and exit", v => outputArguments.ShowHelp = v != null },
                { "p|pack", "Pack a song", v => { if (v != null) outputArguments.Pack = true; }},
                { "u|unpack", "Unpack a song", v => { if (v != null) outputArguments.Unpack = true; }},
                { "b|build", "Build a song package from 'Rocksmith DLC template' (*.dlc.xml)", v => outputArguments.Build = v != null },
                { "i|input=", "The input file or directory (multiple allowed, use ; to split paths)", v => outputArguments.Input = v.Split( new[]{';'}, 2) },
                { "o|output=", "The output file or directory", v => outputArguments.Output = v },
                { "t|template=", "The template file for building package", v => outputArguments.Template = v },
                { "f|platform=", "Platform to pack package [Pc, Mac, XBox360, PS3]", v => outputArguments.SetPlatform(v) },
                { "v|version=", "Version of the Rocksmith Game [RS2012 or RS2014]", v => outputArguments.SetVersion(v) },
                { "ogg|decodeogg", "Decode ogg file when unpack a song (default is true)", v => { if (v != null) outputArguments.DecodeOGG = true; }},
                { "sng|updatesng", "Recreate SNG files when pack a song (default is false)", v => { if (v != null) outputArguments.UpdateSng = true; }},
                { "jsn|updatejsn", "Updates manifest files when pack a song (default is false)", v => { if (v != null) outputArguments.UpdateManifest = true; }},
                { "xml|overwritexml", "Overwrite EOF XML files with XML from SNG files (default is false)", v => { if (v != null) outputArguments.OverwriteSongXml = true; }}
            };
        }

        private static void SetPlatform(this Arguments arguments, string platformString)
        {
            if (arguments.Platform == null)
                arguments.Platform = new Platform(GamePlatform.None, GameVersion.None);
            
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
            if (arguments.Platform == null)
                arguments.Platform = new Platform(GamePlatform.None, GameVersion.None);
            
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
            var arguments = DefaultArguments();
            var options = GetOptions(arguments);

            try
            {
                options.Parse(args);
                if (arguments.ShowHelp)
                {
                    options.WriteOptionDescriptions(Console.Out);
                    return 0;
                }

                if (!arguments.Pack && !arguments.Unpack && !arguments.Build)
                {
                    ShowHelpfulError("Must especify a primary command as 'pack', 'unpack', 'build'.");
                    return 1;
                }
                if (arguments.Build && arguments.Pack ||
                    arguments.Build && arguments.Unpack ||
                    arguments.Pack && arguments.Unpack)
                {
                    ShowHelpfulError("The primary command 'build', 'pack' and 'unpack' can't be used at same time.");
                    return 1;
                }
                if (arguments.Pack || arguments.Unpack)
                {
                    if (arguments.Input == null && arguments.Input.Length <= 0)
                    {
                        ShowHelpfulError("Must specify an 'input' file or directory.");
                        return 1;
                    }
                    if (string.IsNullOrEmpty(arguments.Output))
                    {
                        ShowHelpfulError("Must specified an 'output' file or directory.");
                        return 1;
                    }
                    if ((arguments.Platform.platform == GamePlatform.None && arguments.Platform.version != GameVersion.None) ||
                        (arguments.Platform.platform != GamePlatform.None && arguments.Platform.version == GameVersion.None)) {
                            ShowHelpfulError("'platform' argument require 'version' and vice-versa to define platform. Use this option only if you have problem with platform auto identifier");
                            return 1;
                    }
                }

                // BUILD PACKAGE FROM TEMPLATE
                if (arguments.Build)
                {
                    if (string.IsNullOrEmpty(arguments.Template)) {
                        ShowHelpfulError("Must specify an 'template' argument with path of the Rocksmith DLC template (*.dlc.xml).");
                        return 1;
                    }
                    if (string.IsNullOrEmpty(arguments.Output))
                    {
                        ShowHelpfulError("Must specified an 'output' file.");
                        return 1;
                    }
                    if (arguments.Output.IsDirectory())
                    {
                        ShowHelpfulError("The 'output' argument in 'build' command must be a file.");
                        return 1;
                    }

                    try
                    {
                        Console.WriteLine("Warning: You should load and save XML with 'RocksmithToolkitGUI 2.3.0.0' or above to make sure it is still valid and compatible with this feature!");

                        DLCPackageData info = null;
                        var serializer = new DataContractSerializer(typeof(DLCPackageData));
                        using (var stm = new XmlTextReader(arguments.Template))
                        {
                            info = (DLCPackageData)serializer.ReadObject(stm);
                        }

                        var gameVersion = info.GameVersion;
                        FixPaths(info, arguments.Template, gameVersion);

                        if (info.Pc)
                            DLCPackageCreator.Generate(arguments.Output, info, new Platform(GamePlatform.Pc, gameVersion));
                        if (gameVersion == GameVersion.RS2014)
                            if (info.Mac)
                                DLCPackageCreator.Generate(arguments.Output, info, new Platform(GamePlatform.Mac, gameVersion));
                        if (info.XBox360)
                            DLCPackageCreator.Generate(Path.Combine(Path.GetDirectoryName(arguments.Output), Path.GetFileNameWithoutExtension(arguments.Output)), info, new Platform(GamePlatform.XBox360, gameVersion));
                        if (info.PS3)
                            DLCPackageCreator.Generate(Path.Combine(Path.GetDirectoryName(arguments.Output), Path.GetFileNameWithoutExtension(arguments.Output)), info, new Platform(GamePlatform.PS3, gameVersion));

                        Console.WriteLine("Package was generated.");
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}\n{1}\n{2}", "Build error!", ex.Message, ex.InnerException));
                        return 1;
                    }
                }

                // PACK A FOLDER TO A PACKAGE
                if (arguments.Pack)
                {
                    if (!arguments.Input[0].IsDirectory())
                    {
                        ShowHelpfulError("The 'input' argument in 'pack' command must be a directory.");
                        return 1;
                    }

                    var srcFiles = arguments.Input;
                    foreach (string srcFileName in srcFiles)
                    {
                        try
                        {
                            if (arguments.Platform.platform != GamePlatform.None && arguments.Platform.version != GameVersion.None)
                                Packer.Pack(Path.GetFullPath(srcFileName), Path.GetFullPath(arguments.Output), arguments.UpdateSng, arguments.Platform, arguments.UpdateManifest);
                            else
                                Packer.Pack(Path.GetFullPath(srcFileName), Path.GetFullPath(arguments.Output), arguments.UpdateSng, updateManifest: arguments.UpdateManifest);

                                Console.WriteLine("Packing is complete.");
                                return 0;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(String.Format("Packing error!\nFile: {0}\n{1}\n{2}", srcFileName, ex.Message, ex.InnerException));
                                return 1;
                            }
                        }
                    }

                // UNPACK A PACKAGE FILE
                if (arguments.Unpack)
                {
                    if (!arguments.Output.IsDirectory())
                    {
                        ShowHelpfulError("The 'output' argument in 'unpack' command must be a directory.");
                        return 1;
                    }

                    var srcFiles = new List<string>();
                    
                    foreach (var name in arguments.Input) {
                        if(name.IsDirectory())
                            srcFiles.AddRange(Directory.EnumerateFiles(Path.GetFullPath(name), "*.psarc", SearchOption.AllDirectories));
                        if(File.Exists(name))
                            srcFiles.Add(name);
                    }

                    foreach (string srcFileName in srcFiles)
                    {
                        Platform platform = Packer.GetPlatform(srcFileName);
                        if (platform.platform == GamePlatform.None)
                        {
                            Console.WriteLine("Error: Platform not found or invalid 'input' file:" + srcFileName);
                            continue;
                        }

                        try
                        {
                            Packer.Unpack(Path.GetFullPath(srcFileName), Path.GetFullPath(arguments.Output), arguments.DecodeOGG, arguments.OverwriteSongXml);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(String.Format("Unpacking error!\nFile: {0}\n{1}\n{2}", srcFileName, ex.Message, ex.InnerException));
                            return 1;
                        }
                    }
                    Console.WriteLine("Unpacking is complete.");
                    return 0;
                }

            }
            catch (OptionException ex)
            {
                ShowHelpfulError(ex.Message);
                return 1;
            }

            return 0;
        }

        static void ShowHelpfulError(string message)
        {
            Console.Write("packer: ");
            Console.WriteLine(message);
            Console.WriteLine("Try 'packer --help' for more information.");
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
            catch (Exception ex) {
                ShowHelpfulError("Invalid file or directory." + Environment.NewLine + ex.Message);
            }

            return isDirectory;
        }

        private static void FixPaths(DLCPackageData info, string templateDir, GameVersion gameVersion)
        {
            foreach (var arr in info.Arrangements) {
                arr.SongXml.File = Path.Combine(Path.GetDirectoryName(templateDir), arr.SongXml.File);
                if (gameVersion == GameVersion.RS2014)
                    UpdateTones(arr);
            }
            info.AlbumArtPath = Path.Combine(Path.GetDirectoryName(templateDir), info.AlbumArtPath);
            if (!String.IsNullOrEmpty(info.OggPath))
                info.OggPath = Path.Combine(Path.GetDirectoryName(templateDir), info.OggPath);
            if (!String.IsNullOrEmpty(info.OggPreviewPath))
                info.OggPreviewPath = Path.Combine(Path.GetDirectoryName(templateDir), info.OggPreviewPath);
        }

        private static void UpdateTones(Arrangement arrangement)
        {
            // template may not reflect current XML state, update tone slots
            if (arrangement.ArrangementType != ArrangementType.Vocal)
            {
                var xml = Song2014.LoadFromFile(arrangement.SongXml.File);

                if (xml.ToneBase != null)
                    arrangement.ToneBase = xml.ToneBase;

                // A (ID 0)
                if (xml.ToneA != null)
                {
                    if (xml.ToneA != xml.ToneBase)
                        // SNG convertor expects ToneA to be ID 0
                        throw new InvalidDataException(String.Format("Invalid tone definition detected in {0}, ToneA (ID 0) is expected to be same as ToneBase.", arrangement.SongXml.File));
                    arrangement.ToneA = xml.ToneA;
                }
                else
                    arrangement.ToneA = null;
                // B (ID 1)
                if (xml.ToneB != null)
                    arrangement.ToneB = xml.ToneB;
                else
                    arrangement.ToneB = null;
                // C (ID 2)
                if (xml.ToneC != null)
                    arrangement.ToneC = xml.ToneC;
                else
                    arrangement.ToneC = null;
                // D (ID 3)
                if (xml.ToneD != null)
                    arrangement.ToneD = xml.ToneD;
                else
                    arrangement.ToneD = null;
            }
        }
    }
}
