using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng2014HSL;
using Action = System.Action;
using ToolkitInfo = RocksmithToolkitLib.DLCPackage.ToolkitInfo;
using System.Threading;
using System.Runtime.InteropServices;

namespace RocksmithToolkitLib.Extensions
{
    public static class GeneralExtension
    {
        private static readonly Random randomNumber = new Random();

        public static bool IsWine()
        {
            if (Environment.GetEnvironmentVariable("WINE_INSTALLED") == "1")
                return true;

            return false;
        }

        public static bool Contains(this String obj, char[] chars)
        {
            return (obj.IndexOfAny(chars) >= 0);
        }

        public static T Copy<T>(T value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                dcs.WriteObject(stream, value);
                stream.Position = 0;
                return (T)dcs.ReadObject(stream);
            }
        }

        public static string CopyToTempFile(this string file, string extension = ".tmp")
        {
            var tmp = GetTempFileName(extension);
            if (File.Exists(file))
                File.Copy(file, tmp);
            return tmp;
        }

        public static T DeepCopy<T>(object value)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, value);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static string GetDescription(this object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

        public static string GetTempFileName(string extension = ".tmp")
        {
            string re = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp", Path.GetRandomFileName() + extension);
            // perma fix for album artwork tmp\*.dds 'Could not find file' error 
            IOExtension.MakeDirectory(Path.GetDirectoryName(re));

            return re;
        }

        public static ToolkitInfo GetToolkitInfo(StreamReader reader)
        {
            if (reader == null)
                return null;

            var tkInfo = new ToolkitInfo();
            string line = null;
            reader.BaseStream.Position = 0;
            while ((line = reader.ReadLine()) != null)
            {
                // we need to decipher what this line contains;
                // older toolkit versions just put a single line with the version number
                // newer versions put several lines in the format "key : value"
                var tokens = line.Split(new char[] { ':' });
                // trim all tokens of surrounding whitespaces
                for (int i = 0; i < tokens.Length; ++i)
                    tokens[i] = tokens[i].Trim();

                if (tokens.Length == 1)
                {
                    // this is probably just the version number
                    tkInfo.ToolkitVersion = tokens[0];
                }
                if (tokens.Length == 2)
                {
                    // key/value attribute
                    var key = tokens[0].ToLower();
                    switch (key)
                    {
                        case "toolkit version":
                            tkInfo.ToolkitVersion = tokens[1]; break;
                        case "package author":
                            tkInfo.PackageAuthor = tokens[1]; break;
                        case "package version":
                            tkInfo.PackageVersion = tokens[1]; break;
                        case "package comment":
                            tkInfo.PackageComment = tokens[1]; break;
                        case "package rating":
                            tkInfo.PackageRating = tokens[1]; break;
                        default:
                            Console.WriteLine("  Notice: Unknown key in toolkit.version: {0}", key);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("  Notice: Unrecognized line in toolkit.version: {0}", line);
                }
            }
            return tkInfo;
        }

        public static bool IsBetween(float testValue, float minValue, float maxValue)
        {
            if (testValue >= minValue && testValue <= maxValue)
                return true;

            return false;
        }
        /// <summary>
        /// Gets the type of the PE machine.
        /// (this extension method causes an error in the release build for some users on some machines)
        /// </summary>
        /// <returns>The PE machine type.</returns>
        /// <example>true if 64 Bit PE was provided</example>
        /// <param name="pecoffPath">Pecoff path.</param>
        /// <seealso cref="https://www.microsoft.com/whdc/system/platform/firmware/PECOFF.mspx"/>
        public static bool IsPE64BitType(string pecoffPath)
        {
            var f = new FileStream(pecoffPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var b = new BinaryReader(f);

            // Offset to PE header is always at 0x3C.
            f.Seek(0x3c, SeekOrigin.Begin);

            // The PE header starts with "PE\0\0" =  0x50 0x45 0x00 0x00, little-endian
            var peHeaderOffset = b.ReadInt32();
            f.Seek(peHeaderOffset, SeekOrigin.Begin);
            if (b.ReadUInt32() != 0x00004550)
                throw new Exception("Can't find PE header!");

            //Seek to OptionalHeader to find out what type
            f.Seek(0x118, SeekOrigin.Begin);
            if (b.ReadByte() != 0xB)
                throw new Exception("ROM type detected, you're doing it wrong!");
            return b.ReadByte() == 2;
        }

        public static bool IsValidPSARC(this string fileName)
        {
            //Supported DLC Package types
            var mimeByteHeaderList = new Dictionary<string, byte[]>
            {
                { ".psarc", Encoding.ASCII.GetBytes("PSAR") },
                { ".edat", Encoding.ASCII.GetBytes("NPD") },
                { "xbox", Encoding.ASCII.GetBytes("CON") }
            };
            string extension = Path.GetExtension(fileName);
            if (String.IsNullOrEmpty(extension))
                extension = fileName.Split('_').LastOrDefault();
            if (mimeByteHeaderList.ContainsKey(extension))
            {
                byte[] mime = mimeByteHeaderList[extension];
                byte[] file = new byte[] { 0, 0, 0, 0 };
                using (FileStream fs = File.OpenRead(fileName))
                    fs.Read(file, 0, mime.Length);

                bool r = file.Take(mime.Length).SequenceEqual(mime);
                if (!r)
                    File.Move(fileName, Path.ChangeExtension(fileName, ".invalid"));

                return r;
            }
            return false;
        }

        public static void PopFontPath(this Sng2014File vox, string dlcname)
        {
            var path = String.Format("assets/ui/lyrics/{0}/lyrics_{0}.dds", dlcname);
            if (vox.Vocals != null)
                if (vox.Vocals.Count > 0 && vox.SymbolsTexture.Count > 0)
                {
                    Sng2014FileWriter.readString(path, vox.SymbolsTexture.SymbolsTextures[0].Font);
                    vox.SymbolsTexture.SymbolsTextures[0].FontpathLength = path.Length;
                }
        }

        public static long RandomLong(long lMin, long lMax)
        {
            return lMin + randomNumber.Next() % (lMax - lMin);
        }

        public static string RandomName(int iLen)
        {
            var builder = new StringBuilder(iLen);

            for (int i = 0; i < iLen; i++)
                builder.Append((char)randomNumber.Next(0x61, 0x7A)); // Alpha Lower Case Only

            return builder.ToString();
        }

        public static ToolkitInfo ReadToolkitInfo(string filePath)
        {
            ToolkitInfo tkInfo;
            using (var info = File.OpenText(filePath))
                tkInfo = GetToolkitInfo(info);

            return tkInfo;
        }

        private static HelpForm cmdWin;
        public static string RunExternalExecutable(string exeFileName, bool toolkitRootFolder = true, bool runInBackground = false, bool waitToFinish = false, string arguments = null)
        {
            var output = string.Empty;
            var toolkitRootPath = AppDomain.CurrentDomain.BaseDirectory;
            var rootPath = toolkitRootFolder ? toolkitRootPath : Path.GetDirectoryName(exeFileName);

            // for Mac Mono/Wine use old process command window
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || IsWine())
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(rootPath, exeFileName),
                    WorkingDirectory = rootPath
                };

                if (runInBackground)
                {
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                }

                if (!String.IsNullOrEmpty(arguments))
                    startInfo.Arguments = arguments;

                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                if (waitToFinish)
                    process.WaitForExit();

                if (runInBackground)
                    output = process.StandardOutput.ReadToEnd();

                return output;
            }
            else
            {
                try
                {
                    // use custom Third Party Application Process window
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(rootPath, exeFileName),
                        WorkingDirectory = rootPath,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    if (!String.IsNullOrEmpty(arguments))
                        startInfo.Arguments = arguments;

                    var process = new Process();
                    var sb = new StringBuilder();
                    sb.AppendLine("Please wait ...");
                    sb.AppendLine("");
                    sb.AppendLine(startInfo.FileName + " " + startInfo.Arguments);

                    if (!runInBackground)
                    {
                        // setup a custom Command Window
                        cmdWin = new HelpForm();
                        cmdWin.Size = new Size(500, 500);
                        cmdWin.StartPosition = FormStartPosition.CenterScreen;
                        cmdWin.TopMost = true;
                        cmdWin.Text = "Toolkit Third Party Application Process Window ...";
                        cmdWin.okButton.Hide();
                        cmdWin.rtbBlank.BackColor = Color.Black;
                        cmdWin.rtbNotes.BackColor = Color.Black;
                        cmdWin.rtbNotes.ForeColor = Color.LimeGreen;
                        cmdWin.rtbNotes.ScrollBars = RichTextBoxScrollBars.None;
                        cmdWin.rtbNotes.Text = sb.ToString();
                        cmdWin.Show();
                        Application.DoEvents();
                    }

                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true;
                    process.Start();

                    if (!runInBackground && waitToFinish)
                    {
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            sb.AppendLine(line);
                            UpdateCmdWin(line);
                        }
                    }

                    var exitCode = -1; // waitToFinish is false
                    if (waitToFinish)
                    {
                        process.WaitForExit(); // WaitForExit blocks EventHandler UI Threading
                        exitCode = process.ExitCode; // sucess = 0, failure = 1

                        if (!runInBackground)
                        {
                            sb.AppendLine("Finished ...");
                            UpdateCmdWin("");
                            UpdateCmdWin("");
                            Thread.Sleep(3000);
                            cmdWin.Close();
                            cmdWin.Dispose();
                        }

                        process.Dispose();
                        process = null;
                    }

                    output = sb.ToString() + Environment.NewLine + "Exit Code: " + exitCode;
                }
                catch (Exception ex) // for Mac Wine/Mono compatiblity
                {
                    var errMsg = new StringBuilder();
                    errMsg.AppendLine("");
                    errMsg.AppendLine("RunExternalExecutable ...");
                    errMsg.AppendLine("If you are running toolkit on Mac Wine make sure Environmental Variable 'WINE_INSTALLED' is set to '1'");
                    errMsg.AppendLine(ex.Message);
                    throw new SystemException(errMsg.ToString() + Environment.NewLine);
                }
            }

            return output;
        }

        private static void UpdateCmdWin(string line)
        {
            try
            {
                InvokeIfRequired(cmdWin, a =>
                  {
                      cmdWin.rtbNotes.Text += Environment.NewLine + line;
                      cmdWin.rtbNotes.SelectionStart = cmdWin.rtbNotes.Text.Length;
                      cmdWin.rtbNotes.ScrollToCaret();
                      Application.DoEvents();
                  });
            }
            catch (Exception ex) // for Mac Wine/Mono compatiblity
            {
                var errMsg = new StringBuilder();
                errMsg.AppendLine("");
                errMsg.AppendLine("UpdateCmdWin ...");
                errMsg.AppendLine("If you are running toolkit on Mac Wine make sure Environmental Variable 'WINE_INSTALLED' is set to '1'");
                errMsg.AppendLine("");
                throw new SystemException(errMsg.ToString() + ex.Message + Environment.NewLine);
            }

            Debug.WriteLine(line);
        }

        public static string[] SelectLines(this string[] content, string value)
        {
            return (from j in content
                    where j.Contains(value)
                    select j).ToArray<string>();
        }

        public static byte[] ToByteArray(this string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                    .ToArray();
        }

        public static string ToHex(this string inputString)
        {
            byte[] bArray = Encoding.Default.GetBytes(inputString);
            var hexString = BitConverter.ToString(bArray);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        public static int ToInt32(this string value)
        {
            int v;
            if (!Int32.TryParse(value, out v))
                return -1;
            return v;
        }

        public static string ToLowerId(this Guid guid)
        {
            return guid.ToString().Replace("-", "").ToLower();
        }

        public static void WriteFile(this Stream memoryStream, string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, (int)memoryStream.Length);
                file.Write(bytes, 0, bytes.Length);
            }
        }

        public static void InvokeIfRequired<T>(this T c, Action<T> action) where T : Control
        {
            if (c.InvokeRequired)
                c.Invoke(new Action(() => action(c)));
            else
                action(c);
        }

        public static bool IsInDesignMode
        {
            get
            {
                var bVshostCheck = Process.GetCurrentProcess().ProcessName.IndexOf("vshost", StringComparison.OrdinalIgnoreCase) > -1 ? true : false;
                var bUsageCheck = LicenseManager.UsageMode == LicenseUsageMode.Designtime ? true : false;
                var bDevEnvCheck = Application.ExecutablePath.IndexOf("devenv", StringComparison.OrdinalIgnoreCase) > -1 ? true : false;
                var bDebuggerAttached = Debugger.IsAttached;

#if DEBUG
                var bModeCheck = true;
#else
                var bModeCheck = false;
#endif

                //MessageBox.Show("bVshostCheck = " + bVshostCheck + Environment.NewLine +
                //                "bModeCheck = " + bUsageCheck + Environment.NewLine +
                //                "bDevEnvCheck = " + bDevEnvCheck + Environment.NewLine +
                //                "bDebuggerAttached = " + bDebuggerAttached + Environment.NewLine +
                //                "bModeCheck = " + bModeCheck, "DEBUG ME");

                if (bDebuggerAttached || bDevEnvCheck || bUsageCheck || bVshostCheck || bModeCheck)
                    return true;

                return false;
            }
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [Obfuscation(Exclude = false, Feature = "-rename")]
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        public static float GetDisplayScalingFactor(Control control)
        {
            var dpi = GetDisplayDpi(control);
            float displayScalingFactor;

            if (dpi > 96)
            {
                displayScalingFactor = dpi / 96;
                return displayScalingFactor;
            }

            // this method is valid only for 96 DPI
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            displayScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return displayScalingFactor; // 1.25 = 125%
        }

        public static float GetDisplayDpi(Control control)
        {
            var dpi = control.CreateGraphics().DpiX;
            return dpi;
        }

        public static bool ValidateDisplaySettings(Form form, Control control, bool forceAdjustment = false, bool verbose = true)
        {
            // system settings can cause display issues for the application
            var dpi = GetDisplayDpi(control);
            var displayScalingFactor = GetDisplayScalingFactor(control);

            if (dpi != 96 || displayScalingFactor != 1.0 || forceAdjustment)
            {
                if (verbose)
                    MessageBox.Show(
                        " - System Display DPI Setting (" + dpi + ")" + Environment.NewLine +
                        " - System Display Screen Scale Factor (" + displayScalingFactor * 100 + "%)" + Environment.NewLine +
                        " - Adjusted AutoScaleDimensions, AutoScaleMode, and AutoSize" + Environment.NewLine + Environment.NewLine +
                        "If application does not display correctly then change system setting to:  " + Environment.NewLine +
                        "Control Panel>Appearance and Personalization>Display>Smaller - 100%  ", "Validate Display Settings ...", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                form.SuspendLayout();
                form.AutoScaleDimensions = new SizeF(6F, 13F); // assuming 96 DPI
                form.AutoScaleMode = AutoScaleMode.Font;
                control.AutoSize = true;
                form.ResumeLayout();

                return false;
            }

            return true;
        }


    }
}
