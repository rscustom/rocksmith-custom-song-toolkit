using System;
using System.IO;
using System.Linq;
using System.Reflection;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.PSARC;

namespace toneliberator
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Console.SetWindowSize(85, 40);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
            }
            catch {/* DO NOTHING */}

#if (DEBUG)
            // give the progie some dumby file to work on
            args = new string[] { "D:\\Temp\\PeppaPig_p.psarc" };
            Console.WriteLine("Running in Debug Mode ... help is not available");
#endif

            // catch if there are no cmd line arguments
            if (args.GetLength(0) == 0) args = new string[] { "?" };
            if (args[0].Contains("?") || args[0].ToLower().Contains("help"))
            {
                Console.WriteLine(@"Tone Liberator DropletApp for Rocksmith 2014 CDLC");
                Console.WriteLine();
                Console.WriteLine(@" - Version: " + ProjectVersion());
                Console.WriteLine(@"   Copyright (C) 2016 CST Developers, Cozy1");
                Console.WriteLine();
                Console.WriteLine(@" - Purpose: Extracts Reusable Tones from CDLC Archive Files");
                Console.WriteLine(@"   Creates 'SongName_Arrangement_ToneKey.tone2014.xml' files");
                Console.WriteLine(@"   and copies them to the 'dlc/toneliberator' output folder");
                Console.WriteLine();
                Console.WriteLine(@" - Usage: Drag/Drop CDLC archive file(s)");
                Console.WriteLine(@"   or folder(s) onto the console executable icon");
                Console.Read();
                return 0;
            }

            var errorMsg = String.Empty;
            var srcDir = args[0];
            var rootDir = Path.GetDirectoryName(srcDir);
            var destFolder = Path.Combine("dlc", "toneliberator");
            var destDir = Path.Combine(rootDir, destFolder);

            // create destination 'dlc/tones' directory
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            Console.WriteLine(@"Initializing Tone Liberator CLI ...");
            Console.WriteLine();

            // iterate through folders and/or files       
            foreach (var arg in args)
            {
                string[] filePaths;

                if (IsDirectory(arg))
                {
                    filePaths = Directory.GetFiles(arg, "*.psarc", SearchOption.AllDirectories).ToArray();
                    if (!filePaths.Any())
                    {
                        Console.WriteLine(@"<ERROR> No valid PSARC archives found: " + arg);
                        Console.WriteLine();
                        continue;
                    }
                }
                else if (arg.IsValidPSARC())
                {
                    filePaths = new string[1];
                    filePaths[0] = arg;
                }
                else
                {
                    Console.WriteLine(@"<ERROR> Invalid PSARC archive: " + arg);
                    Console.WriteLine();
                    continue;
                }

                foreach (var filePath in filePaths)
                {
                    Console.WriteLine(); 
                    Console.WriteLine(@"Liberating tones from: " + filePath);

                    using (var browser = new PsarcLoader(filePath, true))
                    {
                        var jsonEntries = browser.ExtractJsonManifests();
                        foreach (var manifest2014 in jsonEntries)
                        {
                            var preToneKey = String.Empty;
                            var attr = manifest2014.Entries.ToArray()[0].Value.ToArray()[0].Value;

                            for (int i = 0; i < attr.Tones.Count; i++)
                            {
                                var toneKey = attr.Tones[i].Key;
                                if (toneKey == preToneKey)                                   
                                    continue;
                                else
                                    preToneKey = toneKey;

                                var toneFileName = String.Format("{0}_{1}.tone2014.xml", attr.FullName, toneKey);
                                var toneFilePath = Path.Combine(destDir, toneFileName);
                                Console.WriteLine(@" - Writing tone: " + toneFilePath);

                                var tone = new Tone2014();
                                tone = attr.Tones[i];
                                tone.Serialize(toneFilePath);
                            }
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine(@"Done creating tone2014.xml files ...");
            Console.WriteLine(@" - Saved to directory: " + Path.Combine(rootDir, destDir) + @"\");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(@"Press any key to continue ...");
            Console.Read();
            return 0;
        }

        private static string ProjectVersion()
        {
            return String.Format("{0}.{1}.{2}",
                                 Assembly.GetExecutingAssembly().GetName().Version.Major,
                                 Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                 Assembly.GetExecutingAssembly().GetName().Version.Build);
        }

        private static bool IsDirectory(string path)
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
                Console.WriteLine(@"Invalid directory." + Environment.NewLine + ex.Message);
            }

            return isDirectory;
        }

        static int ShowHelpfulError(string message)
        {
            Console.Write("toneliberator: ");
            Console.WriteLine(message);
            Console.ReadLine();
            return 0;
        }

    }

}
