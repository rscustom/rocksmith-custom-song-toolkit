using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NDesk.Options;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng2014HSL;
using MiscUtil.Conversion;
using RocksmithToolkitLib;

namespace sng2014
{
    internal class Arguments {
        public bool ShowHelp;
        public bool Pack;
        public bool Unpack;
        public string[] Input;
        public GamePlatform Platform;
    }

    static class Program
    {
        private static Arguments DefaultArguments() {
            return new Arguments {
                Platform = GamePlatform.Pc
            };
        }

        private static OptionSet GetOptions(Arguments outputArguments) {
            return new OptionSet
            {
                { "h|?|help", "Show this help message and exit", v => outputArguments.ShowHelp = v != null },
                { "p|pack", "Pack and encrypt a SNG file (RS2014)", v => { if (v != null) outputArguments.Pack = true; }},
                { "u|unpack", "Unpack and decrypt a SNG file (RS2014)", v => { if (v != null) outputArguments.Unpack = true; }},
                { "i|input=", "The input file or directory (multiple allowed, use ; to split paths)", v => outputArguments.Input = v.Split( new[]{';'}, 2) },
                { "p|platform=", "Platform to pack/unpack SNG [Pc, Mac, XBox360, PS3]", v => outputArguments.SetPlatform(v) }
            };
        }

        private static void SetPlatform(this Arguments arguments, string platformString) {
            if (String.IsNullOrEmpty(platformString))
                arguments.Platform = GamePlatform.Pc;

            GamePlatform p;
            var validPlatform = Enum.TryParse(platformString, true, out p);
            if (!validPlatform) {
                ShowHelpfulError(String.Format("{0} is not a valid platform.", platformString));
                arguments.Platform = GamePlatform.None;
            }
            arguments.Platform = p;
        }

        static int Main(string[] args)
        {
            var arguments = DefaultArguments();
            var options = GetOptions(arguments);

            try {
                options.Parse(args);

                if (arguments.ShowHelp) {
                    options.WriteOptionDescriptions(Console.Out);
                    return 0;
                }

                if (!arguments.Pack && !arguments.Unpack) {
                    ShowHelpfulError("Must especify a primary command as 'pack' or 'unpack'.");
                    return 1;
                }

                if (arguments.Input == null && arguments.Input.Length <= 0) {
                    ShowHelpfulError("Must specify at least one input file.");
                    return 1;
                }

                var srcFiles = new List<string>();
                foreach (var name in arguments.Input) {
                    if(name.IsDirectory())
                        srcFiles.AddRange(Directory.EnumerateFiles(Path.GetFullPath(name), "*.sng", SearchOption.AllDirectories));

                    if(File.Exists(name))
                        srcFiles.Add(name);
                }

                var errorCount = 0;
                foreach (string inputFile in srcFiles) {
                    if (!File.Exists(inputFile)) {
                        Console.WriteLine(String.Format("File '{0}' doesn't exists.", inputFile));
                        continue;
                    }

                    var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), String.Format("{0}_{1}.sng", Path.GetFileNameWithoutExtension(inputFile), (arguments.Unpack) ? "decrypted" : "encrypted"));

                    using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    using (FileStream outputStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {                        
                        if (arguments.Pack)
                            Sng2014File.UnpackSng(inputStream, outputStream, new Platform(arguments.Platform, GameVersion.RS2014));
                        //else if (arguments.Unpack)
                        //  Sng2014File.PackSng(inputStream, outputStream, new Platform(arguments.Platform, GameVersion.RS2014));
                    }
                }

                if (errorCount == 0)
                    Console.WriteLine("Process sucessfully completed!");
                else if (errorCount > 0 && errorCount < srcFiles.Count)
                    Console.WriteLine("Process completed with errors!");
                else
                    Console.WriteLine("An error ocurred!");

            } catch (OptionException ex) {
                ShowHelpfulError(ex.Message);
                return 1;
            }

            return 0;
        }

        private static bool IsDirectory(this string path) {
            bool isDirectory = false;

            try {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    isDirectory = true;
            } catch (Exception ex) {
                ShowHelpfulError("Invalid file or directory." + Environment.NewLine + ex.Message);
            }

            return isDirectory;
        }

        static void ShowHelpfulError(string message) {
            Console.Write("sng2014: ");
            Console.WriteLine(message);
            Console.WriteLine("Try 'sng2014 --help' for more information.");
        }
    }
}
