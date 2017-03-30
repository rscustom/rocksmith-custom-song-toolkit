using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NDesk.Options;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib;

namespace cdlcconverter
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
                { "sp|sourceplatform=", "Source platform (valid values: Pc, Mac, XBox360 or PS3)", v => outputArguments.SourcePlatform = GetPlatform(v) },
                { "tp|targetplatform=", "Target platform (valid values: Pc, Mac, XBox360 or PS3)", v => outputArguments.TargetPlatform = GetPlatform(v) },
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
                    ShowHelpfulError("Must specify a 'source platform', valid values: Pc|Mac|XBox360|PS3.");
                    return 1;
                }

                if (arguments.TargetPlatform == null || arguments.TargetPlatform.platform == GamePlatform.None)
                {
                    ShowHelpfulError("Must specify a 'target platform', valid values: Pc|Mac|XBox360|PS3.");
                    return 1;
                }

                if (string.IsNullOrEmpty(arguments.Input))
                {
                    ShowHelpfulError("Must specify an 'input' file or directory.");
                    return 1;
                }

                if ((!arguments.TargetPlatform.IsConsole) && String.IsNullOrEmpty(arguments.AppId))
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

                Console.WriteLine(String.Format("Found '{0}' DLCs in '{1}'", sourcePackages.Count(), arguments.Input));
                int dlcCount = 1;
                int dlcSuccessfulCount = 0;
                List<string> dlcErrorList = new List<string>();
                foreach (var sourcePackage in sourcePackages)
                {
                    try
                    {
                        Console.WriteLine("-----------------------------------------------------------------");
                        Console.WriteLine(String.Format("Processing DLC [" + dlcCount + " / " + sourcePackages.Count() + "] '{0}' ...", Path.GetFileName(sourcePackage)));

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
                        {
                            // This should not happen..
                            Console.WriteLine(output);
                        }
                        else
                        {
                            Console.WriteLine(String.Format("DLC {0} converted from '{1}' to '{2}'.", Path.GetFileName(sourcePackage), arguments.SourcePlatform.platform, arguments.TargetPlatform.platform));
                        }

                        dlcSuccessfulCount++;
                    }
                    catch (Exception e) {
                        Console.WriteLine(String.Format("ERROR: Couldn't convert DLC because of error '{0}' - skip file '{1}'", e.Message, Path.GetFileName(sourcePackage)));
                        dlcErrorList.Add(Path.GetFullPath(sourcePackage));
                    }
                    finally
                    {
                        Console.WriteLine("-----------------------------------------------------------------");
                        dlcCount++;
                    }
                }

                Console.WriteLine(String.Format("'{0}' DLCs successful processed.", dlcSuccessfulCount));
                if (dlcErrorList.Count > 0)
                {
                    Console.WriteLine(String.Format("'{0}' DLCs processed with errors:", dlcErrorList.Count));
                    dlcErrorList.ForEach(delegate(String fileName)
                    {
                        Console.WriteLine(fileName);
                    });
                }

            }
            catch (Exception ex)
            {
                ShowHelpfulError(ex.Message);
                return 1;
            }


            return 0;
        }

        static void ShowHelpfulError(string message)
        {
            Console.Write("dlcconverter: ");
            Console.WriteLine(message);
            Console.WriteLine("Try 'dlcconverter --help' for more information.");
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
