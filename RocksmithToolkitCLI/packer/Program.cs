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
                { "i|input=", "The encrypted input file or directory (required, multiple allowed)", v => outputArguments.Input = v },
                { "o|output=", "The output directory (defaults to the input directory)", v => outputArguments.Output = v },
                { "t|template=", "The template file for building package", v => outputArguments.Template = v },
                { "pack", "Generate a song package", v => {if (v != null) outputArguments.Pack = true; }},
                { "unpack", "Unpack a song", v => {if (v != null) outputArguments.Unpack = true; }},
                { "build", "Build a song package", v => {if (v != null) outputArguments.Build = true; }},
                { "ogg|decodeogg", "Decode ogg file when unpack a song", v => {if (v != null) outputArguments.DecodeOGG = true; }},
                { "sng|updatesng", "Recreate SNG files when pack a song", v => {if (v != null) outputArguments.UpdateSng = true; }}
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
                if (arguments.Build) {
                    if (string.IsNullOrEmpty(arguments.Template) && string.IsNullOrEmpty(arguments.Output)) {
                        ShowHelpfulError("Missing 'template' file and/or 'output' file.");
                        return 1;
                    }
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
                    if (arguments.Output.IsDirectory())
                    {
                        ShowHelpfulError("The 'output' argument in 'build' command must be a file.");
                        return 1;
                    }
                    try
                    {
                        Console.WriteLine("Warning: You should load and save XML after toolkit upgrade to make sure it is still valid!");

                        DLCPackageData info = null;
                        var serializer = new DataContractSerializer(typeof(DLCPackageData));
                        using (var stm = new XmlTextReader(arguments.Template))
                        {
                            info = (DLCPackageData)serializer.ReadObject(stm);
                        }
                        RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(arguments.Output, info, GamePlatform.Pc, null);
                        Console.WriteLine("Package was generated.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}\n\r{1}\n\r{2}", "Build error!", ex.Message, ex.InnerException));
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}\n\r{1}\n\r{2}", "Packing error!", ex.Message, ex.InnerException));
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
                        GamePlatform platform = Packer.GetPlatform(sourceFileName);
                        if (platform == GamePlatform.None)
                        {
                            Console.WriteLine("Error: Platform not found or invalid 'input' file.");
                            return 1;
                        }

                        Packer.Unpack(sourceFileName, arguments.Output, true);

                        if (arguments.DecodeOGG)
                        {
                            var name = Path.GetFileNameWithoutExtension(sourceFileName);
                            name += String.Format("_{0}", platform.ToString());
                            string[] oggFiles = Directory.GetFiles(Path.Combine(arguments.Output, name), "*.ogg", SearchOption.AllDirectories);
                            foreach (var file in oggFiles)
                            {
                                var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                                OggFile.Revorb(file, outputFileName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                            }
                        }
                    }
                    Console.WriteLine("Unpacking is complete.");
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
            Console.WriteLine("Try 'packer /help' for more information.");
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
