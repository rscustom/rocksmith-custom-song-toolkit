using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MiscUtil.IO;
using MiscUtil.Conversion;
using System.Windows.Forms;


namespace transferprofile
{
    internal class Program
    {
        [STAThread] // required to prevent clipboard threading error
        private static int Main(string[] args)
        {
            Console.SetWindowPosition(0, 0);
            Console.SetWindowSize(85, 35);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            var backupExt = ".computer_b_org";

#if (DEBUG)
            // give the CLI some dumby args to use
            // args = new string[] { "read", "D:\\Temp\\LocalProfiles.json" };
            args = new string[] { "write", "00-00-00-00", "D:\\Temp\\LocalProfiles.json", "D:\\Temp\\TEST_PRFLDB" };
            // args = new string[] { "restore", "D:\\Temp\\LocalProfiles.json" + backupExt };
            Console.WriteLine("Running in Debug Mode ... help is not available");
#endif

            // catch if there are no cmd line arguments
            if (args.Length < 2)
                args = new string[] { "?" };
            if (args[0].Contains("?") || args[0].ToLower().Contains("help"))
            {
                Console.WriteLine();
                Console.WriteLine("User Profile ID (PID) Transfer DropletApp for Rocksmith 2014");
                Console.WriteLine();
                Console.WriteLine(" - Version: " + ProjectVersion());
                Console.WriteLine("   Copyright (C) 2016 CST Developers, Cozy1");
                Console.WriteLine();
                Console.WriteLine(" - Purpose: Transfer User Profiles from Computer A to Computer B.");
                Console.WriteLine();
                Console.WriteLine(" - Syntax:  transferprofile.exe [read] [file]");
                Console.WriteLine("            Reads the PID from the 'localprofiles.json' file on Computer B.");
                Console.WriteLine();
                Console.WriteLine(" - Syntax:  transferprofile.exe [write] [PID] [file_1] ... [file_n]");
                Console.WriteLine("            Writes the PID from Computer B to the Computer A");
                Console.WriteLine("            'localprofiles.json' and '_prfldb' files");
                Console.WriteLine("            PID must be input as a formatted string: '00-00-00-00'");
                Console.WriteLine();
                Console.WriteLine(" - Syntax:  transferprofile.exe [restore] [file]");
                Console.WriteLine("            Drag/Drop the 'localprofiles.json" + backupExt + "' onto");
                Console.WriteLine("            the 'CLI_RestorePID.bat' file");
                Console.WriteLine();
                Console.WriteLine(" - Usage:   1) Drag/Drop 'localprofiles.json' from Computer B onto 'CLI_ReadPID.bat'");
                Console.WriteLine("               file to read the PID from 'localprofiles.json' file.");
                Console.WriteLine("            2) Modify CLI_WritePID.bat with NotePad Editor and use Ctrl-V to paste");
                Console.WriteLine("               in the PID for Computer B from Clipboard.");
                Console.WriteLine("            3) Copy the 'localprofiles.json' and '_prfldb' files from");
                Console.WriteLine("               Computer A to Computer B.");
                Console.WriteLine("            4) Drag/Drop 'localprofiles.json' and '_prfldb' files onto");
                Console.WriteLine("               the 'CLI_WritePID.bat file to write the PID from Computer B");
                Console.WriteLine("               to the files that were copied from Computer A to Computer B.");
                Console.WriteLine("            5) Play the game using the transfered profile.");
                Console.WriteLine();
                Console.Read();
                return 0;
            }

            var action = args[0].ToLower();
            Console.WriteLine();

            if (args.Length == 2) // read or restore
            {
                if (action != "read" && action != "restore")
                    ShowHelpfulError("<ERROR> Invalid syntax usage." + Environment.NewLine + "Type 'transferprofile.exe help' for additional information ...");

                var srcPath = args[1];

                if (!srcPath.ToLower().Contains("localprofiles.json"))
                    ShowHelpfulError("<ERROR> [file] is not 'localprofiles.json'" + Environment.NewLine + "Type 'transferprofile.exe help' for additional information ...");

                if (action == "read")
                {
                    var destDir = Path.GetDirectoryName(srcPath);
                    var profileId = BitConverter.ToString(GetProfileId(srcPath));
                    var nullFileName = String.Format("PID {0}", profileId);
                    var nullFilePath = Path.Combine(destDir, nullFileName);
                    Clipboard.SetText(profileId);

                    using (TextWriter tw = new StreamWriter(nullFilePath, false))
                        tw.Close();

                    Console.WriteLine(" - User Profile File: " + srcPath);
                    Console.WriteLine("   The PID is: " + profileId);
                    Console.WriteLine("   The PID was saved to the Clipboard for later use.");
                    Console.WriteLine("   For reference created a null file named: " + nullFileName);

                    var destPath = String.Format("{0}{1}", srcPath, backupExt);
                    if (!File.Exists(destPath)) // make backup of original
                    {
                        File.Copy(srcPath, destPath);
                        Console.WriteLine(" - User Profile File: " + srcPath);
                        Console.WriteLine("   Copied To: " + destPath);
                    }
                }
                else if (action == "restore" && srcPath.ToLower().EndsWith(backupExt))
                {
                    var destPath = Path.Combine(Path.GetDirectoryName(srcPath), Path.GetFileNameWithoutExtension(srcPath));
                    File.Copy(srcPath, destPath, true);
                    Console.WriteLine(" - User Profile File: " + srcPath);
                    Console.WriteLine("   Restored To: " + destPath);
                }
                else
                    ShowHelpfulError("<ERROR> Invalid syntax usage." + Environment.NewLine + "Type 'transferprofile.exe help' for additional information ...");
            }
            else if (args.Length > 2) // write
            {
                if (action != "write")
                    ShowHelpfulError("<ERROR> Invalid 'write' syntax usage." + Environment.NewLine + "Type 'transferprofile.exe help' for additional information ...");

                if (args[1].Length != 11)
                    ShowHelpfulError("<ERROR> The PID must be input as a formatted string: '00-00-00-00' ...");

                var profileId = String2Byte(args[1]);
                var little = BigEndianBitConverter.Little;

                for (int i = 2; i < args.Length; i++)
                {
                    using (FileStream outStream = new FileStream(args[i], FileMode.Open, FileAccess.ReadWrite))
                    using (var bw = new EndianBinaryWriter(little, outStream))
                    {
                        bw.BaseStream.Position = 8;
                        bw.Write(profileId);
                    }

                    Console.WriteLine(" - Changed User Profile File: " + args[i]);
                    Console.WriteLine("   PID To: " + args[1]);

                    if (args.Length > 3 && i < args.Length - 1)
                        Console.WriteLine();
                }
            }
            else
                ShowHelpfulError("<ERROR> Encountered unexpected CLI syntax." + Environment.NewLine + "Type 'transferprofile.exe help' for additional information ...");

            Console.WriteLine();
            Console.WriteLine(@"Press 'Enter' to continue ...");
            Console.Read();
            return 0;
        }

        private static byte[] GetProfileId(string filePath)
        {
            var little = BigEndianBitConverter.Little;

            using (var inputFS = File.OpenRead(filePath))
            using (EndianBinaryReader br = new EndianBinaryReader(little, inputFS))
            {
                // move to the correct location
                br.ReadBytes(8);
                // read raw PID data
                var profileId1 = br.ReadBytes(4);
                return profileId1;
            }
        }


        private static string ProjectVersion()
        {
            return String.Format("{0}.{1}.{2}", Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor, Assembly.GetExecutingAssembly().GetName().Version.Build);
        }

        private static int ShowHelpfulError(string message)
        {
            Console.WriteLine(" - Transfer Profile:");
            Console.WriteLine(message);
            Console.Read();
            return 1;
        }

        /// <summary>
        /// input string like "00-00-00-00"
        /// returns byte data[0] ... data[3]
        /// </summary>
        private static byte[] String2Byte(string dataString)
        {
            if (dataString.Length != 11) return null;

            return dataString.Split('-').Select(x => Convert.ToByte(x, 16)).ToArray();
        }

    }
}
