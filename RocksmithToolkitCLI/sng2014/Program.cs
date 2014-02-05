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
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Xml;

namespace sng2014
{
    internal class Arguments {
        public bool ShowHelp;
        public bool Pack;
        public bool Unpack;
        public bool Xml;
        public string[] Input;
        public string[] Manifest;
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
                { "x|xml", "Generate a song Xml from a SNG file (RS2014)", v => { if (v != null) outputArguments.Xml = true; }},
                { "i|input|sng=", "The input SNG file(s) or directory [*.sng] (multiple allowed, use ; to split paths)", v => outputArguments.Manifest = v.Split( new[]{';'}, 2) },
                { "m|manifest=", "The input manifest arrangement file [*.json] (multiple allowed, use ; to split paths in same order of input (SNG) files)", v => outputArguments.Input = v.Split( new[]{';'}, 2) },
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

                if (arguments.Xml && arguments.Manifest == null && arguments.Manifest.Length <= 0) {
                    Console.WriteLine("No manifest file was entered. The song xml file will be generated without song informations like song title, album, artist, tone names, etc.");
                }

                var srcFiles = new List<string>();
                foreach (var name in arguments.Input) {
                    if(name.IsDirectory())
                        srcFiles.AddRange(Directory.EnumerateFiles(Path.GetFullPath(name), "*.sng", SearchOption.AllDirectories));

                    if(File.Exists(name))
                        srcFiles.Add(name);
                }

                var errorCount = 0;
                var indexCount = 0;
                foreach (string inputFile in srcFiles) {
                    if (!File.Exists(inputFile)) {
                        Console.WriteLine(String.Format("File '{0}' doesn't exists.", inputFile));
                        continue;
                    }

                    if (arguments.Unpack || arguments.Xml) {
                        if (Path.GetExtension(inputFile) != ".sng") {
                            Console.WriteLine(String.Format("File '{0}' is not support. \nOnly *.sng are supported on this command.", inputFile));
                            continue;
                        }
                    }

                    
                    if (arguments.Pack || arguments.Unpack) {
                        var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), String.Format("{0}_{1}.sng", Path.GetFileNameWithoutExtension(inputFile), (arguments.Unpack) ? "decrypted" : "encrypted"));
                        
                        using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                        using (FileStream outputStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                            if (arguments.Pack)
                                Sng2014File.UnpackSng(inputStream, outputStream, new Platform(arguments.Platform, GameVersion.RS2014));
                            else if (arguments.Unpack)
                                Console.WriteLine("Not supported at this time :(");
                            //  Sng2014File.PackSng(inputStream, outputStream, new Platform(arguments.Platform, GameVersion.RS2014));
                        }
                    } else if (arguments.Xml) {
                        Attributes2014 att = null;
                        if (arguments.Manifest != null && arguments.Manifest.Length > indexCount)
                            att = Manifest2014<Attributes2014>.LoadFromFile(arguments.Manifest[indexCount]).Entries.ToArray()[0].Value.ToArray()[0].Value;

                        var sng = Sng2014File.LoadFromFile(inputFile, new Platform(arguments.Platform, GameVersion.RS2014));

                        var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), String.Format("{0}.xml", Path.GetFileNameWithoutExtension(inputFile)));
                        using (FileStream outputStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            var song = new Song2014(sng, att ?? null);
                            song.Serialize(outputStream);
                        }
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
