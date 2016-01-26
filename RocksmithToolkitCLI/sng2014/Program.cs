using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NDesk.Options;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.Sng2014HSL;
using MiscUtil.Conversion;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng;

namespace sng2014
{
    internal class Arguments {
        public bool ShowHelp;
        public bool Pack;
        public bool Unpack;
        public bool Xml2Sng;
        public bool Sng2Xml;
        public string[] Input;
        public string[] Manifest;
        public ArrangementType ArrangementType;
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
                { "p|pack", "Pack and encrypt a SNG file", v => { if (v != null) outputArguments.Pack = true; }},
                { "u|unpack", "Unpack and decrypt a SNG file", v => { if (v != null) outputArguments.Unpack = true; }},
                { "x|sng2xml", "Generate a song Xml from a SNG file", v => { if (v != null) outputArguments.Sng2Xml = true; }},
                { "s|xml2sng", "Generate a song file (SNG) from a Xml file", v => { if (v != null) outputArguments.Xml2Sng = true; }},
                { "i|input|sng=", "The input SNG file(s) or directory [*.sng] (multiple allowed, use ; to split paths)", v => outputArguments.Input = v.Split( new[]{';'}, 2) },
                { "m|manifest=", "The input manifest arrangement file [*.json] (multiple allowed, use ; to split paths in same order of input (SNG) files)", v => outputArguments.Manifest = v.Split( new[]{';'}, 2) },
                { "a|type|arrangement=", "Arrangement type of the SNG [Guitar, Bass, Vocal]", v => outputArguments.SetArrangementType(v) },
                { "f|platform=", "Platform to pack/unpack SNG [Pc, Mac, XBox360, PS3]", v => outputArguments.SetPlatform(v) }
            };
        }

        private static void SetArrangementType(this Arguments arguments, string arrangementType) {
            if (String.IsNullOrEmpty(arrangementType))
                arguments.ArrangementType = ArrangementType.Guitar;

            ArrangementType arr;
            var validPlatform = Enum.TryParse <ArrangementType>(arrangementType, true, out arr);
            if (!validPlatform) {
                ShowHelpfulError(String.Format("{0} is not a valid platform.", arrangementType));
                arguments.ArrangementType = ArrangementType.Guitar;
            }
            else arguments.ArrangementType = arr;
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
            else arguments.Platform = p;
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

                if (!arguments.Pack && !arguments.Unpack && !arguments.Sng2Xml && !arguments.Xml2Sng) {
                    ShowHelpfulError("Must especify a primary command as 'pack', 'unpack', 'sng2xml' or 'xml2sng'.");
                    return 1;
                }

                if (arguments.Input == null && arguments.Input.Length <= 0) {
                    ShowHelpfulError("Must specify at least one input file.");
                    return 1;
                }

                if (arguments.Sng2Xml && arguments.Manifest == null && arguments.Manifest.Length <= 0) {
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

                    if (arguments.Unpack || arguments.Sng2Xml) {
                        if (Path.GetExtension(inputFile) != ".sng") {
                            Console.WriteLine(String.Format("File '{0}' is not support. \nOnly *.sng are supported on this command.", inputFile));
                            continue;
                        }
                    }

                    if (arguments.Pack || arguments.Unpack) {
                        var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), String.Format("{0}_{1}.sng", Path.GetFileNameWithoutExtension(inputFile), (arguments.Unpack) ? "decrypted" : "encrypted"));

                        using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                        using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite)) {
                            if (arguments.Pack)
                                Sng2014File.PackSng(inputStream, outputStream, new Platform(arguments.Platform, GameVersion.RS2014));
                            else if (arguments.Unpack)
                                Sng2014File.UnpackSng(inputStream, outputStream, new Platform(arguments.Platform, GameVersion.RS2014));
                        }
                    } else if (arguments.Sng2Xml) {
                        Attributes2014 att = null;
                        if (arguments.ArrangementType != ArrangementType.Vocal && arguments.Manifest != null && arguments.Manifest.Length > indexCount)
                            att = Manifest2014<Attributes2014>.LoadFromFile(arguments.Manifest[indexCount]).Entries.ToArray()[0].Value.ToArray()[0].Value;

                        var sng = Sng2014File.LoadFromFile(inputFile, new Platform(arguments.Platform, GameVersion.RS2014));

                        var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), String.Format("{0}.xml", Path.GetFileNameWithoutExtension(inputFile)));
                        using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite))
                        {
                            dynamic xml = null;

                            if (arguments.ArrangementType == ArrangementType.Vocal)
                                xml = new Vocals(sng);
                            else
                                xml = new Song2014(sng, att ?? null);

                            xml.Serialize(outputStream);
                        }
                    } else if (arguments.Xml2Sng) {
                        var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), String.Format("{0}.sng", Path.GetFileNameWithoutExtension(inputFile)));

                        using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite)) {
                            Sng2014File sng = Sng2014File.ConvertXML(inputFile, arguments.ArrangementType);
                            sng.WriteSng(outputStream, new Platform(arguments.Platform, GameVersion.RS2014));
                        }
                    }
                }

                if (errorCount == 0)
                    Console.WriteLine("Process successfully completed!");
                else if (errorCount > 0 && errorCount < srcFiles.Count)
                    Console.WriteLine("Process completed with errors!");
                else
                    Console.WriteLine("An error occurred!");

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
