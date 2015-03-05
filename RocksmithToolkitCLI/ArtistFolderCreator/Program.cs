using System;
using System.IO;
using System.Reflection;

namespace ArtistFolderCreator
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WindowWidth = 85;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

#if (DEBUG)
            // give the progie some dumby file to work on
            args = new string[] { "D:\\Temp\\Test" };
#endif

            // catch if there are no cmd line arguments
            if (args.GetLength(0) == 0) args = new string[] { "?" };
            if (args[0].Contains("?") || args[0].ToLower().Contains("help"))
            {
                Console.WriteLine(@"Artist Folder Creator DropletApp for Rocksmith 2014 CDLC");
                Console.WriteLine(@" - Version: " + ProjectVersion());
                Console.WriteLine(@"   Copywrite (C) 2015 CST Developers");
                Console.WriteLine();
                Console.WriteLine(@" - Purpose: Catalog CDLC songs by ArtistName into folders");
                Console.WriteLine(@"   Copies 'Artist-Name_Song-Name_v1_p.psarc' files to ArtistName folders.");
                Console.WriteLine();
                Console.WriteLine(@" - Usage: Drag/Drop CDLC song file folder onto the console executable icon.");
                Console.Read();
                return 0;
            }
            var errorMsg = String.Empty;

            if (args.GetLength(0) > 1)
                return ShowHelpfulError("Too many CDLC folders dropped onto the execuatable.\r\nOne at time ... please.");

            if (IsDirectory(args[0]))
            {
                Console.WriteLine(@"Initializing Artist Folder Creator CLI ...");
                Console.WriteLine();
                var srcDir = args[0];
                var rootDir = Path.GetDirectoryName(srcDir);
                const string destDir = "dlc";

                // iterate through files *.psarc
                var cdlcFiles = Directory.GetFiles(srcDir, "*.psarc", SearchOption.AllDirectories);
                if (cdlcFiles.Length <= 0)
                    return ShowHelpfulError("Can not find any CDLC *.psarc files");

                foreach (var cdlcFile in cdlcFiles)
                {
                    // get a single SongName
                    var artistName = Path.GetFileName(cdlcFile).Split('_')[0];
                    var artistDestDir = Path.Combine(rootDir, destDir, artistName);

                    // create new ArtistName folder for song files
                    if (!Directory.Exists(artistDestDir))
                        Directory.CreateDirectory(artistDestDir);

                    Console.WriteLine(@"Parsing folder: " + srcDir);
                    Console.WriteLine(@"Looking for songs by: " + artistName);

                    var artistNameFiles = Directory.GetFiles(srcDir, String.Format("{0}_*.psarc", artistName), SearchOption.AllDirectories);
                    if (artistNameFiles.Length <= 0)
                        return ShowHelpfulError("Can not find any artist named song files");

                    foreach (var artistNameFile in artistNameFiles)
                        if (!File.Exists(Path.Combine(artistDestDir, Path.GetFileName(artistNameFile))))
                            File.Copy(artistNameFile, Path.Combine(artistDestDir, Path.GetFileName(artistNameFile)));

                    Console.WriteLine(@"Copied all song files by: " + artistName);
                    Console.WriteLine(@"To: " + artistDestDir);
                    Console.WriteLine();

                }

                Console.WriteLine();
                Console.WriteLine(@"Done Processing CDLC Files ...");
                Console.WriteLine(@"ArtistName folders and CDLC songs saved to:");
                Console.WriteLine(Path.Combine(rootDir, destDir));
                Console.WriteLine();
            }

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
            Console.Write("Split Song Pack: ");
            Console.WriteLine(message);
            Console.ReadLine();
            return 0;
        }

    }

}

