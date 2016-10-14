using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;

namespace remastered
{
    class Program
    {
        private static int Main(string[] args)
        {
            Console.WindowWidth = 80;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

#if (DEBUG)
            // give the progie some dumby file to work on
            args = new string[] { "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            Console.WriteLine(@"Running in Debug Mode ... help is not available");
#endif

            // catch if there are no cmd line arguments
            if (args.GetLength(0) == 0) args = new string[] { "?" };
            if (args[0].Contains(@"?") || args[0].ToLower().Contains(@"help"))
            {
                Console.WriteLine(@"remastered DropletApp for Rocksmith 2014 CDLC");
                Console.WriteLine();
                Console.WriteLine(@" - Mac/PC Version: " + ProjectVersion());
                Console.WriteLine(@"   Copyright (C) 2016 Toolkit Developers");
                Console.WriteLine();
                Console.WriteLine(@" - Purpose: Converts CDLC for use with Rocksmith 2014 Remastered Version.");
                Console.WriteLine(@"            Repairs the 100% bug issue and improves mastery function.");
                Console.WriteLine(@"            Original (*.psarc) files will be renamed as (*.psarc.org).");
                Console.WriteLine(@"            ODLC and CDLC that have already been repaired are skipped.");
                Console.WriteLine(@"            The CDLC version number is not changed by this tool.");
                Console.WriteLine(@"            User the toolkit CDLC Creator to change a version number.");
                Console.WriteLine();
                Console.WriteLine(@" - Usage: Drag/Drop CDLC song file(s) and/or folder(s) onto the executable.");
                Console.WriteLine(@"          Dragged/Dropped content typically has a 132 character limit.");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@" - WARNING: Do not use this DroplettApp for short jingle/riff CDLC.");
                Console.WriteLine(@"   Audio with less than 30 seconds duration will not be synced properly.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(@"The more you Drag/Drop the longer it takes to process.  Coffee break ;)");
                Console.Read();
                return 0;
            }

            Console.WriteLine(@"Initializing Remastered CLI ...");

            foreach (var arg in args)
            {
                if (IsDirectory(arg))
                {
                    Console.WriteLine();
                    Console.WriteLine(@"Parsing folder: " + arg);
                    Console.WriteLine();

                    // enumerate files *.psarc
                    var cdlcFiles = Directory.EnumerateFiles(arg, "*.psarc", SearchOption.TopDirectoryOnly);
                    if (!cdlcFiles.Any())
                        return ShowHelpfulError(@"Can not find any CDLC files (*.psarc)");

                    RepairFiles(cdlcFiles);
                }
                else
                    RepairFiles(new string[] { arg });
            }

            Console.WriteLine();
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
            Console.WriteLine();
            Console.WriteLine(@" - The (.org) files can be archived or deleted after");
            Console.WriteLine(@"   it is confirm that the Remastered CDLC is working.");
            Console.WriteLine();
            Console.WriteLine(@"Press any key to continue ...");
            CleanupLocalTemp();
            Console.Read();
            return 0;
        }

        private static string OfficialOrRepaired(string filePath)
        {
            ToolkitInfo entryTkInfo;
            using (var browser = new PsarcBrowser(filePath, true))
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
                {
                    packageData = psarcOld.ReadPackage(filePath);

                    // Update arrangement song info
                    foreach (Arrangement arr in packageData.Arrangements)
                    {
                        // skip vocal and showlight arrangements
                        if (arr.ArrangementType == ArrangementType.Vocal || arr.ArrangementType == ArrangementType.ShowLight)
                            continue;

                        // generate new Arrangement IDs
                        arr.Id = IdGenerator.Guid();
                        arr.MasterId = RandomGenerator.NextInt();
                        arr.ClearCache();
                    }

                    if (String.IsNullOrEmpty(packageData.PackageVersion))
                        packageData.PackageVersion = "1";

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
                }

                Console.WriteLine(@" - Repair was sucessful ...");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@" - Repair failed ... " + ex.Message);
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine();
        }

        private static void RepairFiles(IEnumerable<string> cdlcFiles)
        {
            foreach (var cdlcFile in cdlcFiles)
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
        }

        private static int ShowHelpfulError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(@"remastered: ");
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Green;
            CleanupLocalTemp();
            Console.Read();
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
                Console.WriteLine(@"Error: " + ex.Message);
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

// commented out since we are not modifying any xml files
// preserve existing xml comments
//arr.XmlComments = Song2014.ReadXmlComments(arr.SongXml.File);
// add "Remastered" comment to XML to be able to identify repaired CDLC 
//var isCommented = false;
//var commentNodes = arr.XmlComments as List<XComment> ?? arr.XmlComments.ToList();
//foreach (var commentNode in commentNodes)
//{
//    if (commentNode.ToString().Contains(@"Remastered"))
//        isCommented = true;
//}
//if (!isCommented)
//    Song2014.WriteXmlComments(arr.SongXml.File, commentNodes, true, String.Format(@"Remastered"));
