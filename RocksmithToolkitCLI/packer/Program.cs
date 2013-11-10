using System;
using System.Collections.Generic;
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

namespace PackerConsole
{
    internal class Arguments
    {
        public bool ShowHelp;
        public string Input;
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
                { "i|input=", "The input file or directory (multiple allowed)", v => outputArguments.Input = v },
                { "o|output=", "The output file or directory", v => outputArguments.Output = v },
                { "t|template=", "The template file for building package", v => outputArguments.Template = v },
                { "pack", "Pack a song", v => {if (v != null) outputArguments.Pack = true; }},
                { "unpack", "Unpack a song", v => {if (v != null) outputArguments.Unpack = true; }},
                { "build", "Build a song package from 'Rocksmith DLC template' (*.dlc.xml)", v => {if (v != null) outputArguments.Build = true; }},
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
                    if (string.IsNullOrEmpty(arguments.Input))
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
                        Console.WriteLine("Warning: You should load and save XML with 'RocksmithToolkitGUI 1.4.0.0' or above to make sure it is still valid and compatible with this feature!");

                        DLCPackageData info = null;
                        var serializer = new DataContractSerializer(typeof(DLCPackageData));
                        using (var stm = new XmlTextReader(arguments.Template))
                        {
                            info = (DLCPackageData)serializer.ReadObject(stm);
                        }

                        if (!String.IsNullOrEmpty(info.OggPath))
                            DLCPackageCreator.Generate(arguments.Output, info, new Platform(GamePlatform.Pc, GameVersion.None));
                        if (!String.IsNullOrEmpty(info.OggXBox360Path))
                            DLCPackageCreator.Generate(Path.Combine(Path.GetDirectoryName(arguments.Output), Path.GetFileNameWithoutExtension(arguments.Output)), info, new Platform(GamePlatform.XBox360, GameVersion.None));
                        if (!String.IsNullOrEmpty(info.OggPS3Path))
                            DLCPackageCreator.Generate(Path.Combine(Path.GetDirectoryName(arguments.Output), Path.GetFileNameWithoutExtension(arguments.Output)), info, new Platform(GamePlatform.PS3, GameVersion.None));

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
                    if (!arguments.Input.IsDirectory())
                    {
                        ShowHelpfulError("The 'input' argument in 'pack' command must be a directory.");
                        return 1;
                    }

                    try
                    {
                        string[] decodedOGGFiles = Directory.GetFiles(arguments.Input, "*_fixed.ogg", SearchOption.AllDirectories);
                        foreach (var file in decodedOGGFiles)
                            File.Delete(file);

                        Packer.Pack(arguments.Input, arguments.Output, true, arguments.UpdateSng);
                        Console.WriteLine("Packing is complete.");
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}\n\r{1}\n\r{2}", "Packing error!", ex.Message, ex.InnerException));
                        return 1;
                    }
                }
                if (arguments.Unpack)
                {
                    if (!arguments.Output.IsDirectory())
                    {
                        ShowHelpfulError("The 'output' argument in 'unpack' command must be a directory.");
                        return 1;
                    }

                    var sourceFiles = (arguments.Input.IsDirectory()) ? Directory.EnumerateFiles(arguments.Input) : new string[] { arguments.Input };

                    foreach (string sourceFileName in sourceFiles)
                    {
                        Platform platform = Packer.GetPlatform(sourceFileName);
                        if (platform.platform == GamePlatform.None)
                        {
                            Console.WriteLine("Error: Platform not found or invalid 'input' file:" + sourceFileName);
                            continue;
                        }

                        try
                        {
                            Packer.Unpack(sourceFileName, arguments.Output, true);

                            if (arguments.DecodeOGG)
                            {
                                var name = Path.GetFileNameWithoutExtension(sourceFileName);
                                name += String.Format("_{0}", platform.platform.ToString());

                                string[] audioFiles = Directory.GetFiles(Path.Combine(arguments.Output, name), (platform.GetWwiseVersion() == OggFile.WwiseVersion.Wwise2010) ? "*.ogg" : "*.wem", SearchOption.AllDirectories);

                                if (audioFiles != null && audioFiles.Length > 0)
                                {
                                    foreach (var file in audioFiles)
                                    {
                                        var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), ".ogg"));
                                        OggFile.Revorb(file, outputFileName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), platform.GetWwiseVersion());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(String.Format("{0}\n\r{1}\n\r{2}", "Unpacking error!", ex.Message, ex.InnerException));
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

        public static bool IsDirectory(this string path)
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
    }
}
