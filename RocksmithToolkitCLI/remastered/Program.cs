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
        private enum MessageType { Default, Info, Warning, Error, Other };
        private const ConsoleColor DefaultBackColor = ConsoleColor.Black;
        private const ConsoleColor DefaultForeColor = ConsoleColor.Green;
        private const string LogFileName = "remastered_error.log";
        private const string TKI_ARRID = "(Arrangement ID by CLI)";
        private const string TKI_REMASTER = "(Remastered by CLI)";
        private const string corExt = ".cor";
        private const string orgExt = ".org";
        private static List<string> cleanFolders = new List<string>();
        private static bool optionLog;
        private static bool optionOrg;
        private static bool optionPre;
        private static bool optionRen;
        private static bool optionSil;
        private static string platformExt;
        private static StringBuilder sbErrors = new StringBuilder();
        private static string workDirectory;

        private static int Main(string[] args)
        {
            Console.Title = AppDomain.CurrentDomain.FriendlyName;
            Console.WindowWidth = 85;
            Console.WindowHeight = 35;
            Console.BackgroundColor = DefaultBackColor;
            Console.ForegroundColor = DefaultForeColor;

#if (DEBUG)
            // give the progie some dumby file to work on
            // args = new string[] { "D:\\Temp\\Test\\The_Beatles-NowhereMan_p.psarc" };
            // args = new string[] { "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            args = new string[] { "D:\\Temp\\test" };
            // args = new string[] { "-org", "-pre", "D:\\Temp\\Test\\PeppaPig_p.psarc.org" };
            // args = new string[] { "-pre", "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            // args = new string[] { "-ren", "D:\\Temp\\Test\\PeppaPig_p.psarc.org" };
            // args = new string[] { "-log \"D:\\Temp\\Test\"", "D:\\Temp\\Test\\PeppaPig_p.psarc" };
            ShowMessage("Running in Debug Mode ... Help is not available");
#endif

            // catch if there are no cmd line arguments
            if (args.GetLength(0) == 0) args = new string[] { "?" };
            if (args[0].Contains(@"?") || args[0].ToLower().Contains(@"help"))
            {
                var helpMsg =
                    "remastered.exe DropletApp for Rocksmith 2014 CDLC" + Environment.NewLine + Environment.NewLine +
                    " - PC Version: " + ProjectVersion() + Environment.NewLine +
                    "   Copyright (C) 2016 Toolkit Developers" + Environment.NewLine + Environment.NewLine +
                    " - Purpose: Converts CDLC for use with Rocksmith 2014 Remastered Version." + Environment.NewLine +
                    "            Repairs the 100% bug issue and improves mastery function." + Environment.NewLine +
                    "            Original (*.psarc) files will be renamed as (*.psarc.org)." + Environment.NewLine +
                    "            ODLC and CDLC that have already been repaired are skipped." + Environment.NewLine +
                    "            The CDLC version number is not changed by this tool." + Environment.NewLine +
                    "            Use the toolkit CDLC Creator feature to change version numbers." + Environment.NewLine + Environment.NewLine +
                    " - Now includes CDLC corruption and validation checking" + Environment.NewLine + Environment.NewLine +
                    " - Usage: Drag/Drop CDLC song file(s) and/or folder(s) onto the executable." + Environment.NewLine +
                    "          There is typically a 132 character limit for Drag/Drop content." + Environment.NewLine + Environment.NewLine +
                    " - CLI Command Prompt Syntax:" + Environment.NewLine +
                    "   remastered.exe [option] [option] [source1] [source2] [source3].." + Environment.NewLine +
                    "   option: [-org] to remaster files that have extension (.org)" + Environment.NewLine +
                    "   option: [-pre] to repair CDLC that have not been played in" + Environment.NewLine +
                    "           Rocksmith 2014 Remastered and preserve song stats." + Environment.NewLine +
                    "   option: [-ren] to rename (.org) files to (.psarc) files" + Environment.NewLine +
                    "   option: [-log] [directory path] to specify a custom log path" + Environment.NewLine +
                    "           folder for CLI generated (.org), (.cor), and (.log) files" + Environment.NewLine +
                    "   source: [Pathname] of CDLC files or folders to be repaired]" + Environment.NewLine + Environment.NewLine;
                ShowMessage(helpMsg);

                var warningMsg =
                    " - WARNING: Do not use this DroplettApp for short jingle/riff CDLC." + Environment.NewLine +
                    "   CDLC with less than 30 seconds audio will not be synced properly." + Environment.NewLine + Environment.NewLine +
                    "   Make sure all CDLC with (.org) or (.cor) have been removed from" + Environment.NewLine +
                    "   the Rocksmith 2014 'dlc' folder prior to playing the game." + Environment.NewLine + Environment.NewLine;
                ShowMessage(warningMsg, MessageType.Warning);

                var footerMsg =
                    "The more you Drag/Drop the longer it takes to process.  Coffee break  + Environment.NewLine +)" + Environment.NewLine + Environment.NewLine +
                    "Press any key to continue ...";
                ShowMessage(footerMsg);
                if (!optionSil)
                    Console.ReadKey();
                return 0;
            }

            ShowMessage(@"Initializing Remastered CLI ...", prefixLine: true, postfixLine: true);

            // set and add the default workDirectory to cleanDirectories list so that no files get deleted
            workDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "REMASTERED_CLI");
            cleanFolders.Add(workDirectory);

            // detect, set and remove [option] from args
            var argsClean = args;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].Substring(0, 4).ToLower())
                {
                    case "-org":
                        optionOrg = true;
                        // TODO: move to code grave yard
                        // argsClean = argsClean.Where((src, ndx) => ndx != i).ToArray();
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        ShowMessage("CLI option [-org] was found ...");
                        break;
                    case "-pre":
                        optionPre = true;
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        ShowMessage("CLI option [-pre] was found ...");
                        break;
                    case "-ren":
                        optionRen = true;
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        ShowMessage("CLI option [-ren] was found ...");
                        break;
                    case "-log":
                        optionLog = true;
                        workDirectory = args[i].Substring(5).Trim().Trim('"');
                        // add the user specified workDirectory to cleanDirectories list
                        cleanFolders.Add(workDirectory);
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        ShowMessage("CLI option [-log] was found ...");
                        ShowMessage("Custom Log Path: " + workDirectory);
                        break;
                    case "-sil":
                        optionSil = true;
                        argsClean = argsClean.Where(x => x != args[i]).ToArray();
                        ShowMessage("CLI option [-sil] was found ...");
                        break;
                }
            }

            if ((optionOrg || optionPre) && optionRen)
            {
                ShowMessage("Multiple CLI options may not be used with option [-ren]", MessageType.Warning, prefixLine: true, postfixLine: true);
                ShowMessage("Press any key to continue ...");
                if (!optionSil)
                    Console.ReadKey();
                return 4;
            }

            if (!Directory.Exists(workDirectory))
                Directory.CreateDirectory(workDirectory);

            args = argsClean;
            foreach (var arg in args)
            {
                // throws a good error if directory/file is not found
                if (IsDirectory(arg))
                {
                    ShowMessage("Parsing folder: " + arg, prefixLine: true, postfixLine: true);

                    // enumerate files and skip empty folders
                    var allFiles = Directory.EnumerateFiles(arg, "*", SearchOption.AllDirectories).ToList();
                    if (!allFiles.Any())
                        continue;

                    if (optionRen)
                        RenameFiles(allFiles);
                    else
                    {
                        // cleanup the directory removing (.org) and (.cor) files
                        CleanArgFolder(arg);
                        RepairFiles(allFiles);
                    }
                }
                else // single files
                {
                    if (optionOrg && !arg.ToLower().Contains(orgExt))
                    {
                        ShowMessage(arg + ", Can not be repaired using CLI option [-org]", MessageType.Error, prefixLine: true, postfixLine: true);
                        continue;
                    }

                    if (optionRen)
                        RenameFiles(new string[] { arg });
                    else
                    {
                        // cleanup the directory removing (.org) and (.cor) files
                        CleanArgFolder(Path.GetDirectoryName(arg));
                        RepairFiles(new string[] { arg });
                    }
                }
            }

            var errorLogLines = Regex.Matches(sbErrors.ToString(), Environment.NewLine).Count;
            if (errorLogLines > 0)
            {
                // error log can be turned into CSV file
                sbErrors.Insert(0, "File Path, Error Message" + Environment.NewLine);
                sbErrors.Insert(0, DateTime.Now.ToString("MM-dd-yy HH:mm") + Environment.NewLine);
                var errorLogPath = Path.Combine(workDirectory, LogFileName);
                using (TextWriter tw = new StreamWriter(errorLogPath, true))
                {
                    tw.WriteLine(sbErrors + Environment.NewLine);
                    tw.Close();
                }
            }

#if (!DEBUG)
            CleanLocalTemp();
#endif

            if (optionRen)
            {
                var renMsg =
                    "Done renaming CDLC (.org) to (.psarc) ..." + Environment.NewLine +
                    "Now you can use the CLI with option [-pre] to" + Environment.NewLine +
                    "preserve original song stats and redo the repair.";
                ShowMessage(renMsg, prefixLine: true, postfixLine: true);
                //
                var infoMsg =
                    "The [-pre] option does not work for CDLC that have" + Environment.NewLine +
                    "been played in Rocksmith 2014 Remastered because the" + Environment.NewLine +
                    "100% bug is already stored to the game save song stats.";
                ShowMessage(infoMsg, MessageType.Info, postfixLine: true);
            }
            else
            {
                ShowMessage("Done applying Remastered Repair to CDLC ...", prefixLine: true, postfixLine: true);
                ShowMessage(" - 'Notice: Unrecognized line in toolkit.version'", MessageType.Info);
                ShowMessage("   is an informational message.  This indicates the");
                ShowMessage("   original CDLC was produced with an older version");
                ShowMessage("   of the toolkit. The Remastered CDLC has been");
                ShowMessage("   updated to the current version of the toolkit.", postfixLine: true);
                //
                ShowMessage("   Log Path Folder used (.org), (.cor), and (.log) files:", MessageType.Info);
                ShowMessage("   " + workDirectory, MessageType.Info, postfixLine: true);
                //
                ShowMessage(" - The original non-remastered CDLC have been renamed with");
                ShowMessage("   file extension (.org) and moved to the log path folder.");
                ShowMessage("   The (.org) backup files can be archived or deleted after");
                ShowMessage("   it is confirm that the Remastered CDLC are working.", postfixLine: true);
                //
                ShowMessage(" - Corrupt CDLC have been renamed with file extension (.cor)");
                ShowMessage("   and moved to the log path folder.");
                ShowMessage("   Corrupt (non-repairable) CDLC should be submitted to");
                ShowMessage("   the original charter so they can be repaired.", postfixLine: true);
                //
                ShowMessage(" - See the 'remastered_error.log' in the log path folder");
                ShowMessage("   for a list of non-repairable files.", postfixLine: true);
            }
            ShowMessage("Press any key to continue ...");
            if (!optionSil)
                Console.ReadKey();
            return 0;
        }

        private static void CleanArgFolder(string argFolderPath)
        {
            // only clean a directory once to save time
            if (cleanFolders.Contains(argFolderPath))
                return;

            // remove (.org) and (.cor) files from dlc folder and subfolders
            ShowMessage("Cleaning arg folder and subfolders ...");
            bool neededCleaning = false;

            string[] extensions = { orgExt, corExt };
            foreach (var extension in extensions)
            {
                var filter = String.Format("*{0}*", extension);
                var filterFilePaths = Directory.EnumerateFiles(argFolderPath, filter, SearchOption.AllDirectories).ToList();
                foreach (var filterFilePath in filterFilePaths)
                {
                    neededCleaning = true;
                    var destFilePath = Path.Combine(workDirectory, Path.GetFileName(filterFilePath));
                    try
                    {
                        File.SetAttributes(filterFilePath, FileAttributes.Normal);
                        if (!File.Exists(destFilePath))
                        {
                            File.Copy(filterFilePath, destFilePath, true);
                            ShowMessage("Moved file: " + Path.GetFileName(filterFilePath));
                        }
                        else
                            ShowMessage("Deleted duplicate file: " + Path.GetFileName(filterFilePath));

                        // this could throw an error if file is "Read-Only" or does not exist
                        File.Delete(filterFilePath);
                    }
                    catch (IOException ex)
                    {
                        ShowMessage(filterFilePath + ", " + ex.Message, MessageType.Error, prefixLine: true, postfixLine: true);
                    }
                }
            }

            if (neededCleaning)
                ShowMessage("Finished cleaning: " + argFolderPath + " ...", postfixLine: true);
            else
                ShowMessage(argFolderPath + " didn't need cleaning ...", postfixLine: true);

            cleanFolders.Add(argFolderPath);
        }

        private static void CleanLocalTemp()
        {
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
        }

        private static bool CreateBackup(string srcFilePath)
        {
            if (srcFilePath.ToLower().Contains(orgExt))
                return false;

            ShowMessage("File: " + Path.GetFileName(srcFilePath), MessageType.Info, prefixLine: true);
            ShowMessage(" - Making a backup copy ...");
            try
            {
                var properExt = Path.GetExtension(srcFilePath);
                var orgFilePath = String.Format(@"{0}{1}{2}", Path.Combine(workDirectory, Path.GetFileNameWithoutExtension(srcFilePath)), orgExt, properExt).Trim();
                if (!File.Exists(orgFilePath))
                {
                    File.SetAttributes(srcFilePath, FileAttributes.Normal);
                    File.Copy(srcFilePath, orgFilePath, false);
                    ShowMessage(" - Sucessfully created backup ...");
                }
                else
                    ShowMessage(" - Backup already exists ...");
            }
            catch (Exception ex)
            {
                ShowMessage(" - Backup failed ...", MessageType.Warning); // a bad thing
                ShowMessage(ex.Message, MessageType.Warning);
                ShowMessage(srcFilePath + ", Backup Failed", MessageType.Error, prefixLine: true, postfixLine: true);
                return false;
            }
            return true;
        }

        private static ConsoleColor GetMessageColor(MessageType msgType)
        {
            switch (msgType)
            {
                case MessageType.Default:
                    return ConsoleColor.Green;
                case MessageType.Info:
                    return ConsoleColor.Cyan;
                case MessageType.Warning:
                    return ConsoleColor.Yellow;
                case MessageType.Error:
                    return ConsoleColor.Red;
            }
            return ConsoleColor.Green;
        }

        private static int GetMessageReturnValue(MessageType msgType)
        {
            switch (msgType)
            {
                case MessageType.Default:
                    return 0;
                case MessageType.Info:
                    return 1;
                case MessageType.Warning:
                    return 2;
                case MessageType.Error:
                    return 3;
            }
            return 4;
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
                if (ex.Message.Contains("find file"))
                    ShowMessage(path + ", File Not Found", MessageType.Warning, true);
                else
                    ShowMessage(path + ", " + ex.Message, MessageType.Warning, true);
            }
            return isDirectory;
        }

        private static string OfficialOrRepaired(string filePath)
        {
            ToolkitInfo entryTkInfo;
            try
            {
                using (var browser = new PsarcLoader(filePath, true))
                    entryTkInfo = browser.ExtractToolkitInfo();
            }
            catch (Exception ex)
            {
                ShowMessage(SplitString(ex.Message, 55), prefixLine: true, postfixLine: true);
                return null;
            }


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

        private static string ProjectVersion()
        {
            return String.Format(@"{0}.{1}.{2}",
                                 Assembly.GetExecutingAssembly().GetName().Version.Major,
                                 Assembly.GetExecutingAssembly().GetName().Version.Minor,
                                 Assembly.GetExecutingAssembly().GetName().Version.Build);
        }

        private static void RemasterSong(string srcFilePath)
        {
            try
            {
                // SNG's needs to be regenerated
                // ArrangmentIDs are stored in multiple place and all need to be updated
                // therefore we are going to unpack, apply repair, and repack
                ShowMessage(" - Extracting CDLC artifacts ...");
                DLCPackageData packageData;

                using (var psarcOld = new PsarcPackager())
                    packageData = psarcOld.ReadPackage(srcFilePath);

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
                ShowMessage(" - Repackaging remastered CDLC ...");

                // regenerates the SNG with the repair and repackages               
                using (var psarcNew = new PsarcPackager(true))
                    psarcNew.WritePackage(srcFilePath, packageData);

                ShowMessage(" - Repair was sucessful ...");
            }
            catch (Exception ex)
            {
                ShowMessage(" - Repair failed ... " + SplitString(ex.Message, 55), MessageType.Warning);
                ShowMessage(" - See 'remastered_error.log' file ... ", MessageType.Warning);
                ShowMessage(srcFilePath + ", Corrupt File", MessageType.Error, postfixLine: true);

                // copy (org) to corrupt (cor), delete backup (org), delete original
                var properExt = Path.GetExtension(srcFilePath);
                var orgFilePath = String.Format(@"{0}{1}{2}", Path.Combine(workDirectory, Path.GetFileNameWithoutExtension(srcFilePath)), orgExt, properExt).Trim();
                if (File.Exists(orgFilePath))
                {
                    var corFilePath = String.Format(@"{0}{1}{2}", Path.Combine(workDirectory, Path.GetFileNameWithoutExtension(srcFilePath)), corExt, properExt).Trim();
                    File.Copy(orgFilePath, corFilePath, true);
                    File.Delete(orgFilePath);
                    File.Delete(srcFilePath);
                }
            }
        }

        private static List<string> RenameFiles(IEnumerable<string> srcFilePaths)
        {
            var renamedFilePaths = new List<string>();

            foreach (var srcFilePath in srcFilePaths)
            {
                var destFilePath = srcFilePath;
                if (srcFilePath.ToLower().Contains(orgExt))
                    destFilePath = srcFilePath.Replace(orgExt, "");
                if (srcFilePath.ToLower().Contains(corExt))
                    destFilePath = srcFilePath.Replace(corExt, "");

                try
                {
                    File.SetAttributes(srcFilePath, FileAttributes.Normal);
                    if (!File.Exists(destFilePath))
                    {
                        File.Copy(srcFilePath, destFilePath, true);
                        renamedFilePaths.Add(destFilePath);
                        ShowMessage("Moved file: " + Path.GetFileName(srcFilePath));
                    }

                    // this could throw an error if file is "Read-Only" or does not exist
                    File.Delete(srcFilePath);
                    ShowMessage("Deleted file: " + Path.GetFileName(srcFilePath));
                }
                catch (IOException ex)
                {
                    ShowMessage(srcFilePath + ", " + ex.Message, MessageType.Error, prefixLine: true, postfixLine: true);
                }
            }

            return renamedFilePaths;
        }

        private static void RepairFiles(IEnumerable<string> srcFilePaths)
        {
            foreach (var srcFilePath in srcFilePaths)
            {
                // srcFilePath may have been removed by CleanArgFolder
                if (!File.Exists(srcFilePath))
                    continue;

                if (!srcFilePath.IsValidPSARC())
                    ShowMessage(srcFilePath + ", Is not a valid CDLC", MessageType.Error, prefixLine: true, postfixLine: true);
                else
                {
                    var officialOrRepaired = OfficialOrRepaired(srcFilePath);
                    if (String.IsNullOrEmpty(officialOrRepaired))
                    {
                        if (CreateBackup(srcFilePath))
                            RemasterSong(srcFilePath);
                    }
                    else
                    {
                        if (officialOrRepaired.Contains(@"Official"))
                            ShowMessage("Skipped ODLC File: " + Path.GetFileName(srcFilePath), MessageType.Warning);
                        else if (officialOrRepaired.Contains(@"Remastered"))
                            ShowMessage("Skipped Remastered File: " + Path.GetFileName(srcFilePath), MessageType.Warning);
                    }
                }
            }
        }

        private static int ShowMessage(string message, MessageType msgType = MessageType.Default, bool writeLog = false, bool prefixLine = false, bool postfixLine = false)
        {
            if (prefixLine)
                Console.WriteLine();

            Console.ForegroundColor = GetMessageColor(msgType);
            Console.WriteLine(message);
            Console.ForegroundColor = DefaultForeColor;

            if (postfixLine)
                Console.WriteLine();

            if (writeLog || msgType == MessageType.Error)
                sbErrors.AppendLine(message);

            return GetMessageReturnValue(msgType);
        }

        /// <summary>
        /// Splits a text string so that it wraps to specified line length
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="lineLength"></param>
        /// <param name="splitOnSpace"></param>
        /// <returns></returns>
        private static string SplitString(string inputText, int lineLength, bool splitOnSpace = true)
        {
            var finalString = String.Empty;

            if (splitOnSpace)
            {
                var delimiters = new[] { " " }; // , "\\" };
                var stringSplit = inputText.Split(delimiters, StringSplitOptions.None);
                var charCounter = 0;

                for (int i = 0; i < stringSplit.Length; i++)
                {
                    finalString += stringSplit[i] + " ";
                    charCounter += stringSplit[i].Length;

                    if (charCounter > lineLength)
                    {
                        finalString += Environment.NewLine;
                        charCounter = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < inputText.Length; i += lineLength)
                {
                    if (i + lineLength > inputText.Length)
                        lineLength = inputText.Length - i;

                    finalString += inputText.Substring(i, lineLength) + Environment.NewLine;
                }
                finalString = finalString.TrimEnd(Environment.NewLine.ToCharArray());
            }

            return finalString;
        }

    }
}

/* Graveyard
            // determine platform from the first file name in directory
            var di = new DirectoryInfo(srcFolderPath);
            var fileName = di.EnumerateFiles().Select(f => f.Name).FirstOrDefault();
            // check if folder is empty
            if (String.IsNullOrEmpty(fileName))
                return;

            var filePlatform = fileName.GetPlatform();
            var arcExt = filePlatform.platform;

 * 
 *             string[] extensions = { orgExt, corExt };
            var extFilePaths = Directory.EnumerateFiles(argFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(fi => extensions.Any(fi.ToLower().Contains)).ToList();

*/