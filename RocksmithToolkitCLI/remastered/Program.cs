using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.PsarcLoader;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;

namespace remastered
{
    class Program
    {
        private static StringBuilder sbErrors = new StringBuilder();
        private static string fileExt = "psarc";
        private static bool optionOrg;
        private static bool optionPre;
        private static bool optionRen;

        private static int Main(string[] args)
        {
            Console.WindowWidth = 85;
            Console.WindowHeight = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

#if (DEBUG)
            // give the progie some dumby file to work on
            args = new string[] { "D:\\Temp\\Test\\The_Beatles-NowhereMan_p.psarc", "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            args = new string[] { "-org", "Tragically-Hip_Emperor-Penguin_v0_8_p.psarc" };
            args = new string[] { "-pre", "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            args = new string[] { "-ren", "D:\\Temp\\Test\\PeppaPig_p.psarc.org" };
            Console.WriteLine(@"Running in Debug Mode ... help is not available");
#endif

            // catch if there are no cmd line arguments
            if (args.GetLength(0) == 0) args = new string[] { "?" };
            if (args[0].Contains(@"?") || args[0].ToLower().Contains(@"help"))
            {
                Console.WriteLine(@"remastered.exe DropletApp for Rocksmith 2014 CDLC");
                Console.WriteLine();
                Console.WriteLine(@" - PC Version: " + ProjectVersion());
                Console.WriteLine(@"   Copyright (C) 2016 Toolkit Developers");
                Console.WriteLine();
                Console.WriteLine(@" - Purpose: Converts CDLC for use with Rocksmith 2014 Remastered Version.");
                Console.WriteLine(@"            Repairs the 100% bug issue and improves mastery function.");
                Console.WriteLine(@"            Original (*.psarc) files will be renamed as (*.psarc.org).");
                Console.WriteLine(@"            ODLC and CDLC that have already been repaired are skipped.");
                Console.WriteLine(@"            The CDLC version number is not changed by this tool.");
                Console.WriteLine(@"            Use the toolkit CDLC Creator feature to change version numbers.");
                Console.WriteLine();
                Console.WriteLine(@" - Now includes CDLC corruption and validation checking");
                Console.WriteLine();
                Console.WriteLine(@" - Usage: Drag/Drop CDLC song file(s) and/or folder(s) onto the executable.");
                Console.WriteLine(@"          There is typically a 132 character limit for Drag/Drop content.");
                Console.WriteLine();
                Console.WriteLine(@" - CLI Command Prompt Syntax:");
                Console.WriteLine(@"   remastered.exe [option] [option] [source1] [source2] [source3]..");
                Console.WriteLine(@"   option: [-org] to remaster files that have extension (.org)");
                Console.WriteLine(@"   option: [-pre] to repair CDLC that have not been played in");
                Console.WriteLine(@"           Rocksmith 2014 Remastered and preserve song stats.");
                Console.WriteLine(@"   source: [Pathname] of CDLC files or folders to be repaired]");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@" - WARNING: Do not use this DroplettApp for short jingle/riff CDLC.");
                Console.WriteLine(@"   CDLC with less than 30 seconds audio will not be synced properly.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(@"The more you Drag/Drop the longer it takes to process.  Coffee break ;)");
                Console.WriteLine();
                Console.WriteLine(@"Press any key to continue ...");
                Console.ReadKey();
                return 0;
            }

            Console.WriteLine(@"Initializing Remastered CLI ...");
            sbErrors.AppendLine("remastered.exe CLI failed to repair the following files:");

            var argsClean = args;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].Substring(0, 4).ToLower())
                {
                    case "-org":
                        fileExt = "org";
                        optionOrg = true;
                        argsClean = argsClean.Where((src, ndx) => ndx != i).ToArray();
                        Console.WriteLine(@"CLI option [-org] was found ...");
                        break;
                    case "-pre":
                        optionPre = true;
                        argsClean = argsClean.Where((src, ndx) => ndx != i).ToArray();
                        Console.WriteLine(@"CLI option [-pre] was found ...");
                        break;
                    case "-ren":
                        fileExt = "org";
                        optionRen = true;
                        argsClean = argsClean.Where((src, ndx) => ndx != i).ToArray();
                        Console.WriteLine(@"CLI option [-ren] was found ...");
                        break;
                }
            }

            if ((optionOrg || optionPre) && optionRen)
                ShowHelpfulError("Multiple CLI options may not be used with option [-ren]");

            args = argsClean;

            foreach (var arg in args)
            {
                if (IsDirectory(arg)) // this will throw an error if file not found
                {
                    Console.WriteLine();
                    Console.WriteLine(@"Parsing folder: " + arg);
                    Console.WriteLine();

                    // enumerate files *.psarc
                    var enumExt = String.Format("*.{0}", fileExt);
                    var cdlcFiles = Directory.EnumerateFiles(arg, enumExt, SearchOption.TopDirectoryOnly);
                    if (!cdlcFiles.Any())
                        return ShowHelpfulError("Can not find any CDLC files with extension (" + enumExt + ")");

                    RepairFiles(cdlcFiles);
                }
                else
                {
                    if (fileExt == "org" && Path.GetExtension(arg).ToLower() != ".org")
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File: " + arg);
                        Console.WriteLine("Can not be repaired using CLI option [-org]");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine();
                        sbErrors.AppendLine(arg);

                        continue;
                    }

                    RepairFiles(new string[] { arg });
                }
            }

            var errorLogLines = Regex.Matches(sbErrors.ToString(), Environment.NewLine).Count;
            if (errorLogLines > 1)
            {
                var errorLogPath = Path.Combine(Path.GetDirectoryName(args[0]), "remastered_error.log");
                File.WriteAllText(errorLogPath, sbErrors.ToString().TrimEnd('\r', '\n'));
            }

            Console.WriteLine();
            
            if (optionRen)
            {
                Console.WriteLine(@"Done renaming CDLC (.org) to (.psarc) ...");
                Console.WriteLine(@"Now you can use the CLI with option [-pre] to");
                Console.WriteLine(@"preserve original song stats and redo the repair.");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@"The [-pre] option does not work for CDLC that have");
                Console.WriteLine(@"been played in Rocksmith 2014 Remastered because the");
                Console.WriteLine(@"100% bug is already stored to the game save song stats.");
                Console.ForegroundColor = ConsoleColor.Green;
            }

            else
            {
                Console.WriteLine(@"Done applying Remastered Repair to CDLC ...");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@" - 'Notice: Unrecognized line in toolkit.version'");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"   is an informational message.  This indicates the");
                Console.WriteLine(@"   original CDLC was produced with an older version");
                Console.WriteLine(@"   of the toolkit. The Remastered CDLC has been");
                Console.WriteLine(@"   updated to the current version of the toolkit.");
                Console.WriteLine();
                Console.WriteLine(@" - Remember the original (buggy) CDLC has been renamed");
                Console.WriteLine(@"   and it now has a backup file extension of (.org).");
                Console.WriteLine(@"   The (.org) files can be archived or deleted after");
                Console.WriteLine(@"   it is confirm that the Remastered CDLC is working.");
                Console.WriteLine();
                Console.WriteLine(@" - Any corrupt CDLC have been renamed and now has a");
                Console.WriteLine(@"   (.cor) file extension.  These should be submitted to");
                Console.WriteLine(@"   the original charter so they can be repaired.  See");
                Console.WriteLine(@"   'remastered_error.log' for a list of corrupt files.");
            }
            Console.WriteLine();
            Console.WriteLine(@"Press any key to continue ...");
            CleanupLocalTemp();
            Console.ReadKey();
            return 0;
        }

        private static string OfficialOrRepaired(string filePath)
        {
            ToolkitInfo entryTkInfo;
            using (var browser = new PsarcLoader(filePath, true))
                entryTkInfo = browser.ExtractToolkitInfo();

            if (entryTkInfo == null)
                return "Official";

            if (entryTkInfo != null && entryTkInfo.PackageAuthor != null)
                if (entryTkInfo.PackageAuthor.Equals(@"Ubisoft"))
                    return "Official";

            if (entryTkInfo != null && entryTkInfo.PackageComment != null)
                if (entryTkInfo.PackageComment.Contains(@"Remastered"))
                    return "Remastered";

            return null;
        }

        private static void RemasterSong(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            // backup original cdlc
            try
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@"File: " + fileName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@" - Making a backup copy ...");
                var backupPath = String.Format(@"{0}.org", filePath);
                if (!File.Exists(backupPath))
                {
                    File.Copy(filePath, backupPath, false);
                    Console.WriteLine(@" - Sucessfully created backup ...");
                }
                else
                    Console.WriteLine(@" - Backup already exists ...");
            }
            catch (Exception ex)
            {
                // it is critical that backup of originals was successful before proceeding
                throw new Exception(@"Backup of: " + filePath + " ... FAILED" + Environment.NewLine +
                    ex.Message + Environment.NewLine +
                    "Please correct the issue and make sure you have" + Environment.NewLine +
                    "backup copies of your original CDLC files.");
            }

            try
            {
                // TODO: repair in memory without extracting the files
                // SNG's needs to be regenerated
                // ArrangmentIDs are stored in multiple place and all need to be updated
                // therefore we are going to unpack, apply repair, and repack
                Console.WriteLine(@" - Extracting CDLC artifacts ...");
                DLCPackageData packageData;

                using (var psarcOld = new PsarcPackager())
                    packageData = psarcOld.ReadPackage(filePath);

                // Update arrangement song info
                foreach (Arrangement arr in packageData.Arrangements)
                {
                    // skip vocal and showlight arrangements
                    if (arr.ArrangementType == ArrangementType.Vocal || arr.ArrangementType == ArrangementType.ShowLight)
                        continue;

                    if (!optionPre)
                    {
                        // generate new AggregateGraph
                        arr.SongFile = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongFile { File = "" };

                        // generate new Arrangement IDs
                        arr.Id = IdGenerator.Guid();
                        arr.MasterId = RandomGenerator.NextInt();
                    }

                    // preserve existing xml comments
                    arr.XmlComments = Song2014.ReadXmlComments(arr.SongXml.File);
                    var isCommented = false;
                    var commentNodes = arr.XmlComments as List<XComment> ?? arr.XmlComments.ToList();
                    foreach (var commentNode in commentNodes)
                    {
                        if (commentNode.ToString().Contains(@"Remastered"))
                            isCommented = true;
                    }

                    // validate SongInfo
                    var songXml = Song2014.LoadFromFile(arr.SongXml.File);
                    songXml.AlbumYear = packageData.SongInfo.SongYear.ToString().GetValidYear();
                    songXml.ArtistName = packageData.SongInfo.Artist.GetValidAtaSpaceName();
                    songXml.Title = packageData.SongInfo.SongDisplayName.GetValidAtaSpaceName();
                    songXml.AlbumName = packageData.SongInfo.Album.GetValidAtaSpaceName();
                    songXml.ArtistNameSort = packageData.SongInfo.ArtistSort.GetValidSortableName();
                    songXml.SongNameSort = packageData.SongInfo.SongDisplayNameSort.GetValidSortableName();
                    songXml.AlbumNameSort = packageData.SongInfo.AlbumSort.GetValidSortableName();
                    songXml.AverageTempo = Convert.ToSingle(packageData.SongInfo.AverageTempo.ToString().GetValidTempo());

                    // resave the validated xml
                    File.Delete(arr.SongXml.File);
                    using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                        songXml.Serialize(stream, true);

                    // add "Remastered" comment to saved xml to be able to identify repaired CDLC        
                    if (!isCommented)
                        Song2014.WriteXmlComments(arr.SongXml.File, commentNodes, true, String.Format(@"Remastered by CLI"));
                }

                // validate packageData (important)
                packageData.Name = packageData.Name.GetValidKey(); // DLC Key                 

                if (String.IsNullOrEmpty(packageData.PackageVersion))
                    packageData.PackageVersion = "1";
                else
                    packageData.PackageVersion = packageData.PackageVersion.GetValidVersion();

                // add comment to ToolkitInfo to identify Remastered CDLC
                var packageComment = packageData.PackageComment;

                if (String.IsNullOrEmpty(packageComment))
                    packageComment = "(Remastered by CLI)";

                if (!packageComment.Contains(@"Remastered"))
                    packageComment = packageComment + " " + "(Remastered by CLI)";

                packageData.PackageComment = packageComment;
                Console.WriteLine(@" - Repackaging remastered CDLC ...");

                // regenerates the SNG with the repair and repackages               
                using (var psarcNew = new PsarcPackager(true))
                    psarcNew.WritePackage(filePath, packageData);

                Console.WriteLine(@" - Repair was sucessful ...");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@" - Repair failed ... " + ex.Message);
                Console.WriteLine(@" - See 'remastered_error.log' file ... ");
                Console.ForegroundColor = ConsoleColor.Green;

                // delete backup, restore original and rename corrupt (cor)
                var backupPath = String.Format(@"{0}.org", filePath);
                if (File.Exists(backupPath))
                {
                    var corruptPath = String.Format(@"{0}.cor", filePath);
                    File.Copy(backupPath, corruptPath, true);
                    File.Delete(backupPath);
                    File.Delete(filePath);
                }

                sbErrors.AppendLine(filePath);
            }

            Console.WriteLine();
        }

        private static void RepairFiles(IEnumerable<string> cdlcFiles)
        {
            foreach (var cdlcFile in cdlcFiles)
            {
                if (optionRen)
                // rename buggy original (.org) cdlc to (.psarc)
                {
                    if (File.Exists(cdlcFile) && Path.GetExtension(cdlcFile).ToLower() == ".org")
                    {
                        var psarcPath = cdlcFile.Substring(0, cdlcFile.Length - 4);
                        File.Copy(cdlcFile, psarcPath, true);
                    }

                    continue;
                }

                if (fileExt == "psarc")
                {
                    if (!cdlcFile.IsValidPSARC())
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(@"File is not a valid CDLC: " + Path.GetFileName(cdlcFile));
                        Console.ForegroundColor = ConsoleColor.Green;
                        continue;
                    }

                    var officialOrRepaired = OfficialOrRepaired(cdlcFile);
                    if (!String.IsNullOrEmpty(officialOrRepaired))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;

                        if (officialOrRepaired.Contains(@"Official"))
                            Console.WriteLine(@"Skipped ODLC File: " + Path.GetFileName(cdlcFile));
                        else if (officialOrRepaired.Contains(@"Remastered"))
                            Console.WriteLine(@"Skipped Remastered File: " + Path.GetFileName(cdlcFile));

                        Console.ForegroundColor = ConsoleColor.Green;
                        continue;
                    }

                    RemasterSong(cdlcFile);
                }
                else
                {
                    var orgPath = cdlcFile;
                    var psarcPath = cdlcFile.Substring(0, cdlcFile.Length - 4);

                    // restore buggy original (.org) cdlc so it can be remastered
                    if (File.Exists(orgPath))
                    {
                        File.Copy(orgPath, psarcPath, true);
                        // File.Delete(orgPath);      
                    }
                    else
                    {
                        // Error is handled by IsDirectory() so commented out
                        // (.org) can not be found
                        //Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine(@" - File not found: " + orgPath);
                        //Console.WriteLine();
                        //Console.ForegroundColor = ConsoleColor.Green;
                        //sbErrors.AppendLine(String.Format("File not found: {0}", orgPath));

                        continue;
                    }

                    RemasterSong(psarcPath);
                }
            }
        }

        private static int ShowHelpfulError(string message)
        {
            sbErrors.AppendLine(String.Format("{0}", message));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(@"remastered: ");
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Green;
            CleanupLocalTemp();
            Console.ReadKey();
            return 1;
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
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;

                if (ex.Message.Contains("find file"))
                    Console.WriteLine("File Not Found: " + path);
                else
                    Console.WriteLine(ex.Message);

                Console.ForegroundColor = ConsoleColor.Green;
                sbErrors.AppendLine(path);

            }

            return isDirectory;
        }

        private static void CleanupLocalTemp()
        {
#if !DEBUG
            var di = new DirectoryInfo(Path.GetTempPath());

            // confirm this is the 'Local Settings\Temp' directory
            if (di.Parent != null)
                if (di.Parent.Name == "Local Settings" && di.Name == "Temp")
                {
                    foreach (FileInfo file in di.GetFiles())
                        try
                        {
                            file.Delete();
                        }
                        catch { /*Don't worry just skip locked file*/ }

                    foreach (DirectoryInfo dir in di.GetDirectories())
                        try
                        {
                            dir.Delete(true);
                        }
                        catch { /*Don't worry just skip locked directory*/ }
                }
#endif
        }

        private static string ProjectVersion()
        {
            return String.Format(@"{0}.{1}.{2}",
                                 Assembly.GetExecutingAssembly().GetName().Version.Major,
                                 Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                 Assembly.GetExecutingAssembly().GetName().Version.Build);
        }


    }
}

// CODE Grave Yard
//
//var dropIsDirectory = false;
//var dropCount = args.GetLength(0);
//if (dropCount > 0)
//{
//    for (int i = 0; i < dropCount; i++)
//    {
//        if (IsDirectory(args[i]))
//            dropIsDirectory = true;

//        if (!IsDirectory(args[i]) && dropIsDirectory)
//            return ShowHelpfulError(@"Please drop all files or all folders onto the executable.");
//    }
//}
//else
//    return ShowHelpfulError(@"Please drop either all files or all folders onto the executable.");

