using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using NDesk.Options;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng;
using System.Text;

namespace dlcconverter
{
    internal class Arguments
    {
        public bool ShowHelp;
        public Platform SourcePlatform;
        public Platform TargetPlatform;
        public string Input;
        public string AppId;
    }

    static class Program
    {
        private static OptionSet GetOptions(Arguments outputArguments)
        {
            return new OptionSet
            {
                { "h|?|help", "Show this help message and exit", v => outputArguments.ShowHelp = v != null },
                { "sp|sourceplatform", "Source platform (valid values: Pc, Mac, XBox360 or PS3)", v => outputArguments.SourcePlatform = GetPlatform(v) },
                { "tp|targetplatform", "Target platform (valid values: Pc, Mac, XBox360 or PS3)", v => outputArguments.TargetPlatform = GetPlatform(v) },
                { "i|input=", "The input file or directory (multiple allowed)", v => outputArguments.Input = v },
                { "appid=", "AppId (required for Pc and Mac platforms)", v => outputArguments.AppId = v }                
            };
        }

        private static Platform GetPlatform(string platformString) {
            GamePlatform p;
            var validPlatform = Enum.TryParse(platformString, true, out p);
            if (!validPlatform)
            {
                ShowHelpfulError(String.Format("{0} is not a valid platform.", platformString));
                return new Platform(GamePlatform.None, GameVersion.None);
            }
            return new Platform(p, GameVersion.RS2014);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            var arguments = new Arguments();
            var options = GetOptions(arguments);

            try
            {
                options.Parse(args);
                if (arguments.ShowHelp)
                {
                    options.WriteOptionDescriptions(Console.Out);
                    return 0;
                }

                // VALIDATIONS
                if (arguments.SourcePlatform == null || arguments.SourcePlatform.platform == GamePlatform.None)
                {
                    ShowHelpfulError("Must specified a 'source platform', valid values: Pc|Mac|XBox360|PS3.");
                    return 1;
                }

                if (arguments.TargetPlatform == null || arguments.TargetPlatform.platform == GamePlatform.None)
                {
                    ShowHelpfulError("Must specified a 'target platform', valid values: Pc|Mac|XBox360|PS3.");
                    return 1;
                }

                if (string.IsNullOrEmpty(arguments.Input))
                {
                    ShowHelpfulError("Must specify an 'input' file or directory.");
                    return 1;
                }

                if ((arguments.TargetPlatform.platform == GamePlatform.Pc || arguments.TargetPlatform.platform == GamePlatform.Mac) && String.IsNullOrEmpty(arguments.AppId))
                {
                    ShowHelpfulError("'appid' is required for 'Pc' or 'Mac' target platform.");
                    return 1;
                }              

                // CONVERSION
                var packageFilter = "*.psarc";
                if (arguments.SourcePlatform.platform == GamePlatform.XBox360)
                    packageFilter = "*.*";
                else if (arguments.SourcePlatform.platform == GamePlatform.PS3)
                    packageFilter = "*.edat";

                var sourcePackages = (arguments.Input.IsDirectory()) ? Directory.EnumerateFiles(arguments.Input, packageFilter, SearchOption.TopDirectoryOnly) : new string[] { arguments.Input };
                
                foreach (var sourcePackage in sourcePackages)
                {
                    var alertMessage = String.Format("Source package '{0}' seems to be not {1} platform, the conversion can't be work.", Path.GetFileName(sourcePackage), arguments.SourcePlatform.platform);
                    if (arguments.SourcePlatform.platform != GamePlatform.PS3)
                    {
                        if (!Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(arguments.SourcePlatform.GetPathName()[2]))
                        {
                            Console.WriteLine(alertMessage);
                            Console.WriteLine("Force try to convert this package? [Y] Yes, [N] No.");
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Y)
                                Console.WriteLine("Ok, trying convert...");
                            else
                                continue;
                        }
                    }
                    else if (arguments.SourcePlatform.platform == GamePlatform.PS3)
                    {
                        if (!(Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(arguments.SourcePlatform.GetPathName()[2] + ".psarc")))
                        {
                            Console.WriteLine(alertMessage);
                            Console.WriteLine("Force try to convert this package? [Y] Yes, [N] No.");
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Y)
                                Console.WriteLine("Ok, trying convert...");
                            else
                                continue;
                        }
                    }

                    // CONVERT
                    var output = DLCPackageConverter.Convert(sourcePackage, arguments.SourcePlatform, arguments.TargetPlatform, arguments.AppId);
                    if (!String.IsNullOrEmpty(output))
                        Console.WriteLine(output);
                }

                Console.WriteLine(String.Format("DLC was converted from '{0}' to '{1}'.", arguments.SourcePlatform.platform, arguments.TargetPlatform.platform));
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
            catch (Exception ex)
            {
                ShowHelpfulError("Invalid file or directory." + Environment.NewLine + ex.Message);
            }

            return isDirectory;
        }
    }
}
