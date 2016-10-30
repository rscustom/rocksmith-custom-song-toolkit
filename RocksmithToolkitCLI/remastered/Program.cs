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
        private static string workDirectory;
        private const string TKI_REMASTER = "(Remastered by CLI)";
        private const string TKI_ARRID = "(Arrangement ID by CLI)";
        private static StringBuilder sbErrors = new StringBuilder();
        private static string fileExt = "psarc";
        private static bool optionOrg;
        private static bool optionPre;
        private static bool optionRen;
        private static bool optionLog;

        private static int Main(string[] args)
        {
            Console.Title = "remastered.exe";
            Console.WindowWidth = 85;
            Console.WindowHeight = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

#if (DEBUG)
            // give the progie some dumby file to work on
            // args = new string[] { "D:\\Temp\\Test\\The_Beatles-NowhereMan_p.psarc" };
            args = new string[] { "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            // args = new string[] { "-org", "-pre", "D:\\Temp\\Test\\PeppaPig_p.psarc.org" };
            // args = new string[] { "-pre", "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            // args = new string[] { "-ren", "D:\\Temp\\Test\\PeppaPig_p.psarc.org" };
            // args = new string[] { "-log \"D:\\Temp\\Test\"", "D:\\Temp\\Test\\PeppaPig_p.psarc" };
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
                Console.WriteLine(@"   option: [-ren] to rename (.org) files to (.psarc) files");
                Console.WriteLine(@"   option: [-log] [directory path] to specify a custom log path");
                Console.WriteLine(@"           folder for CLI generated (.org), (.cor), and (.log) files");
                Console.WriteLine(@"   source: [Pathname] of CDLC files or folders to be repaired]");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@" - WARNING: Do not use this DroplettApp for short jingle/riff CDLC.");
                Console.WriteLine(@"   CDLC with less than 30 seconds audio will not be synced properly.");
                Console.WriteLine();
                Console.WriteLine(@"   Make sure all CDLC with (.org) or (.cor) have been removed from");
                Console.WriteLine(@"   the Rocksmith 2014 'dlc' folder prior to playing the game.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(@"The more you Drag/Drop the longer it takes to process.  Coffee break ;)");
                Console.WriteLine();
                Console.WriteLine(@"Press any key to continue ...");
                Console.ReadKey();
                return 0;
            }

            Console.WriteLine(@"Initializing Remastered CLI ...");
            Console.WriteLine();
            sbErrors.AppendLine(DateTime.Now.ToString("MM-dd-yy HH:mm") + " - remastered.exe CLI failed to repair the following files:");

            // detect, set and remove [option] from args
            var argsClean = args;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].Substring(0, 4).ToLower())
                {
                    case "-org":
                        fileExt = "org";
                        optionOrg = true;
                        // TODO: move to code grave yard
                        // argsClean = argsClean.Where((src, ndx) => ndx != i).ToArray();
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        Console.WriteLine(@"CLI option [-org] was found ...");
                        break;
                    case "-pre":
                        optionPre = true;
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        Console.WriteLine(@"CLI option [-pre] was found ...");
                        break;
                    case "-ren":
                        fileExt = "org";
                        optionRen = true;
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        Console.WriteLine(@"CLI option [-ren] was found ...");
                        break;
                    case "-log":
                        optionLog = true;
                        workDirectory = args[i].Substring(5).Trim().Trim('"');
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        Console.WriteLine(@"CLI option [-log] was found ...");
                        Console.WriteLine(@"Custom Log Path: " + workDirectory);
                        break;
                }
            }

            Console.WriteLine();
            if ((optionOrg || optionPre) && optionRen)
                ShowHelpfulError("Multiple CLI options may not be used with option [-ren]");

            if (!optionLog)
                workDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "REMASTERED_CLI");

            if (!Directory.Exists(workDirectory))
                Directory.CreateDirectory(workDirectory);

            args = argsClean;
            foreach (var arg in args)
            {
                if (IsDirectory(arg)) // this will throw an error if file is not found
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
                var errorLogPath = Path.Combine(workDirectory, "remastered_error.log");
                using (TextWriter tw = new StreamWriter(errorLogPath, true))
                    tw.WriteLine(sbErrors + Environment.NewLine);

                // File.WriteAllText(errorLogPath, sbErrors.ToString().TrimEnd('\r', '\n'));
            }

            CleanupLocalTemp();
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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@"   Log Path Folder used (.org), (.cor), and (.log) files:");
                Console.WriteLine(@"   " + workDirectory);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(@" - The original non-remastered CDLC have been renamed with");
                Console.WriteLine(@"   file extension (.org) and moved to the log path folder.");
                Console.WriteLine(@"   The (.org) backup files can be archived or deleted after");
                Console.WriteLine(@"   it is confirm that the Remastered CDLC are working.");
                Console.WriteLine();
                Console.WriteLine(@" - Corrupt CDLC have been renamed with file extension (.cor)");
                Console.WriteLine(@"   and moved to the log path folder.");
                Console.WriteLine(@"   Corrupt (non-repairable) CDLC should be submitted to");
                Console.WriteLine(@"   the original charter so they can be repaired.");
                Console.WriteLine();
                Console.WriteLine(@" - See the 'remastered_error.log' in the log path folder");
                Console.WriteLine(@"   for a list of non-repairable files.");
            }
            Console.WriteLine();
            Console.WriteLine(@"Press any key to continue ...");
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
                if (entryTkInfo.PackageAuthor.Equals("Ubisoft"))
                    return "Official";

            if (entryTkInfo != null && entryTkInfo.PackageComment != null)
                if (entryTkInfo.PackageComment.Contains("Remastered"))
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
                var backupPath = String.Format(@"{0}.org", Path.Combine(workDirectory, Path.GetFileName(filePath)));
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
                    if (!optionPre)
                    {
                        // generate new AggregateGraph
                        arr.SongFile = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongFile { File = "" };

                        // generate new Arrangement IDs
                        arr.Id = IdGenerator.Guid();
                        arr.MasterId = RandomGenerator.NextInt();
                    }

                    // skip vocal and showlight arrangements
                    if (arr.ArrangementType == ArrangementType.Vocal || arr.ArrangementType == ArrangementType.ShowLight)
                        continue;

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

                    // write updated xml arrangement
                    using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                        songXml.Serialize(stream, true);

                    // add comments back to xml arrangement   
                    Song2014.WriteXmlComments(arr.SongXml.File, arr.XmlComments);
                }

                if (!optionPre)
                {
                    // add comment to ToolkitInfo to identify CDLC
                    var arrIdComment = packageData.PackageComment;
                    if (String.IsNullOrEmpty(arrIdComment))
                        arrIdComment = TKI_ARRID;
                    else if (!arrIdComment.Contains(TKI_ARRID))
                        arrIdComment = arrIdComment + " " + TKI_ARRID;

                    packageData.PackageComment = arrIdComment;
                }

                // add comment to ToolkitInfo to identify CDLC
                var remasterComment = packageData.PackageComment;
                if (String.IsNullOrEmpty(remasterComment))
                    remasterComment = TKI_REMASTER;
                else if (!remasterComment.Contains(TKI_REMASTER))
                    remasterComment = remasterComment + " " + TKI_REMASTER;

                packageData.PackageComment = remasterComment;

                // add default package version if missing
                if (String.IsNullOrEmpty(packageData.PackageVersion))
                    packageData.PackageVersion = "1";
                else
                    packageData.PackageVersion = packageData.PackageVersion.GetValidVersion();

                // validate packageData (important)
                packageData.Name = packageData.Name.GetValidKey(); // DLC Key                 
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

                // copy (org) to corrupt (cor), delete backup (org), delete original
                var backupPath = String.Format(@"{0}.org", Path.Combine(workDirectory, Path.GetFileName(filePath)));
                if (File.Exists(backupPath))
                {
                    var corruptPath = String.Format(@"{0}.cor", Path.Combine(workDirectory, Path.GetFileName(filePath)));
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
                // copy and rename buggy original (.org) cdlc to (.psarc)
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
            //#if !DEBUG
            var di = new DirectoryInfo(Path.GetTempPath());

            // 'Local Settings\Temp' in WinXp
            // 'AppData\Local\Temp' in Win7
            // confirm this is the correct temp directory before deleting
            if (di.Parent != null)
            {
                if (di.Parent.Name.Contains("Local") && di.Name == "Temp")
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
            }
            //#endif
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

