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
                { "pack", "Pack a song", v => {if (v != null) outputArguments.Pack = true; }},
                { "unpack", "Unpack a song", v => {if (v != null) outputArguments.Unpack = true; }},
                { "build", "Build a song package from 'Rocksmith DLC template' (*.dlc.xml)", v => outputArguments.Build = v != null },
                { "i|input=", "The input file or directory (multiple allowed, use ; to split paths)", v => outputArguments.Input = v.Split(new[]{';'}, 2) },
                { "o|output=", "The output file or directory", v => outputArguments.Output = v },
                { "t|template=", "The template file for building package", v => outputArguments.Template = v },
                { "ogg|decodeogg", "Decode ogg file when unpack a song (default is true)", v => {if (v != null) outputArguments.DecodeOGG = true; }},
                { "sng|updatesng", "Recreate SNG files when pack a song (default is false)", v => {if (v != null) outputArguments.UpdateSng = true; }}
            };
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
                	if (string.IsNullOrEmpty(arguments.Input[0]))
                    {
                        ShowHelpfulError("Must specify an 'input' file or directory.");
                        return 1;
                    }
                    if (string.IsNullOrEmpty(arguments.Output))
                    {
                        ShowHelpfulError("Must specified an 'output' file or directory.");
                        return 1;
                    }
                }

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

                        var gameVersion = GameVersion.RS2012;
                        if (info.GameVersion != null)
                            gameVersion = info.GameVersion;

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
                        Console.WriteLine(String.Format("{0}\n\r{1}\n\r{2}", "Build error!", ex.Message, ex.InnerException));
                        return 1;
                    }
                }
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
							Packer.DeleteFixedAudio(srcFileName);
							Packer.Pack(Path.GetFullPath(srcFileName), Path.GetFullPath(arguments.Output), arguments.UpdateSng);
							Console.WriteLine("Packing is complete.");
	                        return 0;
	                    }
	                    catch (Exception ex)
	                    {
							Console.WriteLine(String.Format("Packing error!\n\rFile: {0}\n\r{1}\n\r{2}", 
							                                srcFileName, ex.Message, ex.InnerException));
	                        return 1;
	                    }
					}
                }
                if (arguments.Unpack)
                {
                    if (!arguments.Output.IsDirectory())
                    {
                        ShowHelpfulError("The 'output' argument in 'unpack' command must be a directory.");
                        return 1;
                    }

                    var srcFiles = new List<string>();
                    foreach (var name in arguments.Input){
                    	if(name.IsDirectory()) srcFiles.AddRange(Directory.EnumerateFiles(Path.GetFullPath(name), "*.psarc", SearchOption.AllDirectories));
                    	if(File.Exists(name)) srcFiles.Add(name);}

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
                            Packer.Unpack(Path.GetFullPath(srcFileName), Path.GetFullPath(arguments.Output));

                            if (arguments.DecodeOGG)
                            {
                                var name = Path.GetFileNameWithoutExtension(srcFileName);
                                name += String.Format("_{0}", platform.platform.ToString());

                                var audioFiles = Directory.GetFiles(Path.Combine(arguments.Output, name), "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wem"));

                                foreach (var file in audioFiles)
                                {
                                    var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                                    OggFile.Revorb(Path.GetFullPath(file), Path.GetFullPath(outputFileName), Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.GetExtension(file).GetWwiseVersion());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
							Console.WriteLine(String.Format("Unpacking error!\n\rFile: {0}\n\r{1}\n\r{2}", 
							                                srcFileName, ex.Message, ex.InnerException));
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
