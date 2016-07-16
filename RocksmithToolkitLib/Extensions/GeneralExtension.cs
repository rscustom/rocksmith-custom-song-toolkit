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
using RocksmithToTabLib;

namespace RocksmithToolkitLib.Extensions
{
    public static class GeneralExtensions
    {
        private static readonly Random randomNumber = new Random();

        public static bool Contains(this String obj, char[] chars)
        {
            return (obj.IndexOfAny(chars) >= 0);
        }

        public static string GetDescription(this object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

        public static string[] SelectLines(this string[] content, string value)
        {
            return (from j in content
                    where j.Contains(value)
                    select j).ToArray<string>();
        }

        public static string ReadPackageVersion(string filePath)
        {
            string packageVersion = "1";
            using (var info = File.OpenText(filePath))
            {
                packageVersion = GetToolkitInfo(info).PackageVersion ?? "1";
            }
            return packageVersion;
        }

        public static DLCPackage.ToolkitInfo GetToolkitInfo(StreamReader reader)
        {
            if (reader == null)
                return null;

            var info = new DLCPackage.ToolkitInfo();
            string line = null;
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
                    info.ToolkitVersion = tokens[0];
                }
                if (tokens.Length == 2)
                {
                    // key/value attribute
                    var key = tokens[0].ToLower();
                    switch (key)
                    {
                        case "toolkit version":
                            info.ToolkitVersion = tokens[1]; break;
                        case "package author":
                            info.PackageAuthor = tokens[1]; break;
                        case "package version":
                            info.PackageVersion = tokens[1]; break;
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
            return info;
        }

        /// <summary>
        /// Strips non-printable ASCII characters
        /// </summary>
        /// <param name="filePath">Full path to the File</param>
        public static Stream StripIllegalXMLChars(this string filePath)
        {
            string tmpContents = File.ReadAllText(filePath);
            const string pattern = @"[\x01-\x08\x0B-\x0C\x0E-\x1F\x7F-\x84\x86-\x9F]"; // XML1.1

            tmpContents = Regex.Replace(tmpContents, pattern, "", RegexOptions.IgnoreCase);

            return new MemoryStream(new UTF8Encoding(false).GetBytes(tmpContents));
        }

        public static void PopFontPath(this RocksmithToolkitLib.Sng2014HSL.Sng2014File vox, string dlcname)
        {
            var path = String.Format("assets/ui/lyrics/{0}/lyrics_{0}.dds", dlcname);
            if (vox.Vocals != null)
                if (vox.Vocals.Count > 0 && vox.SymbolsTexture.Count > 0)
                {
                    RocksmithToolkitLib.Sng2014HSL.Sng2014FileWriter.readString(path, vox.SymbolsTexture.SymbolsTextures[0].Font);
                    vox.SymbolsTexture.SymbolsTextures[0].FontpathLength = path.Length;
                }
        }

        public static string GetValidVersion(this string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                Regex rgx = new Regex(@"^[\d\.]*");
                var match = rgx.Match(value);
                if (match.Success)
                    return match.Value.Trim();
            }
            return "1";
        }

        /// <summary>
        /// Format string as valid DLCKey (aka SongKey).
        /// </summary>
        /// <param name="value">DLCKey for verification</param>
        /// <param name="songTitle">optional varification comparison to DLCKey </param>
        /// <returns>valid DLCKey contains no spaces, no accents, or special characters but can begin with or be all numbers</returns>

        public static string GetValidDlcKey(this string value, string songTitle = "")
        {
            string dlcKey = String.Empty;
            if (!String.IsNullOrEmpty(value))
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]"); // removes spaces
                dlcKey = rgx.Replace(value, "");
                //Avoid SongKey==SongTitle case
                if (dlcKey == songTitle.Replace(" ", ""))
                    dlcKey = dlcKey + "Song";
            }
            // limit length to 30
            return dlcKey.Substring(0, Math.Min(30, dlcKey.Length));
        }

        public static string GetValidYear(this string value)
        {
            // check for valid four digit song year 
            if (!Regex.IsMatch(value, "^(19[0-9][0-9]|20[0-1][0-9])"))
                value = ""; // clear if not valid

            return value;
        }

        public static string GetValidTempo(this string value)
        {
            float tempo = 0;
            float.TryParse(value.Trim(), out tempo);
            int bpm = (int)Math.Round(tempo);
            // check for valid tempo
            if (bpm > 0 && bpm < 300)
                return bpm.ToString();

            return "120"; // default tempo
       }

        public static string GetValidSortName(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            if (value.ToUpperInvariant().StartsWith("THE "))
                return value.Remove(0, 4).GetValidName(true, true);

            return value.GetValidName(true, true);
        }

        public static string GetValidFileName(this string value)
        {
            return String.Concat(value.Split(Path.GetInvalidFileNameChars()));
        }

        public static string GetValidName(this string value, bool allowSpace = false, bool allowStartsWithNumber = false, bool underscoreSpace = false, bool frets24 = false)
        {
            // valid characters developed from actually reviewing ODLC artist, title, album names
            string name = String.Empty;

            if (!String.IsNullOrEmpty(value))
            {
                // ODLC artist, title, album character use allows these but not these
                // allow use of accents Über ñice \\p{L}
                // allow use of unicode punctuation \\p{P\\{S} not currently implimented
                // may need to be escaped \t\n\f\r#$()*+.?[\^{|  ... '-' needs to be escaped if not at the beginning or end of regex sequence
                // allow use of only these special characters \\-_ /&.:',!?()\"#
                // allow use of alphanumerics a-zA-Z0-9
                // tested and working ... Üuber!@#$%^&*()_+=-09{}][":';<>.,?/ñice

                Regex rgx = new Regex((allowSpace) ? "[^a-zA-Z0-9\\-_ /&.:',!?()\"#\\p{L}]" : "[^a-zA-Z0-9\\-_/&.:',!?()\"#\\p{L} ]");
                name = rgx.Replace(value, "");

                Regex rgx2 = new Regex(@"^[\d]*\s*");
                if (!allowStartsWithNumber)
                    name = rgx2.Replace(name, "");

                // prevent names from starting with special characters -_* etc
                Regex rgx3 = new Regex("^[^A-Za-z0-9]*");
                     name = rgx3.Replace(name, "");

                if (frets24)
                {
                    if (name.Contains("24"))
                    {
                        name = name.Replace("_24_", "_");
                        name = name.Replace("_24", "");
                        name = name.Replace("24_", "");
                        name = name.Replace(" 24 ", " ");
                        name = name.Replace("24 ", " ");
                        name = name.Replace(" 24", " ");
                        name = name.Replace("24", "");
                    }
                    name = name.Trim() + " 24";
                }

                if (underscoreSpace)
                    name = name.Replace(" ", "_");
            }

            return name.Trim();
        }

        public static string StripPlatformEndName(this string value)
        {
            if (value.EndsWith(GamePlatform.Pc.GetPathName()[2]) ||
                value.EndsWith(GamePlatform.Mac.GetPathName()[2]) ||
                value.EndsWith(GamePlatform.XBox360.GetPathName()[2]) ||
                value.EndsWith(GamePlatform.PS3.GetPathName()[2]) ||
                value.EndsWith(GamePlatform.PS3.GetPathName()[2] + ".psarc"))
            {
                return value.Substring(0, value.LastIndexOf("_"));
            }

            return value;
        }

        public static string ToNullTerminatedAscii(this Byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
        }

        public static string ToNullTerminatedUTF8(this Byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        }

        public static string Acronym(this string value)
        {
            var v = Regex.Split(value, @"[\W\s]+").Where(r => !string.IsNullOrEmpty(r)).ToArray();
            if (v.Length > 1)
                return string.Join(string.Empty, v.Select(s => s[0])).ToUpper();
            return value.GetValidName(false, true);
        }

        public static string GetShortName(string Format, string Artist, string Title, string Version, bool Acronym)
        {
            if (!Acronym)
                return String.Format(Format, Artist.GetValidName(true, true), Title.GetValidName(true, true), Version).Replace(" ", "-").GetValidFileName();
            return String.Format(Format, Artist.Acronym(), Title.GetValidName(true, true), Version).Replace(" ", "-").GetValidFileName();
        }

        public static bool IsAppId6Digits(this string value)
        {
            // check for valid six digit AppID that begins with 2 , e.g. 248750
            return Regex.IsMatch(value, "^[2]\\d{5}$");  // "^[0-9]{6}$");
        }

        public static bool IsValidPSARC(this string fileName)
        {
            //Supported DLC Package types
            var mimeByteHeaderList = new Dictionary<string, byte[]>();
            mimeByteHeaderList.Add(".psarc", Encoding.ASCII.GetBytes("PSAR"));
            mimeByteHeaderList.Add(".edat", Encoding.ASCII.GetBytes("NPD"));
            mimeByteHeaderList.Add("xbox", Encoding.ASCII.GetBytes("CON"));

            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
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

        public static string RunExternalExecutable(string exeFileName, bool toolkitRootFolder = true, bool runInBackground = false, bool waitToFinish = false, string arguments = null)
        {
            string toolkitRootPath = Path.GetDirectoryName(Application.ExecutablePath);

            var rootPath = (toolkitRootFolder) ? toolkitRootPath : Path.GetDirectoryName(exeFileName);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = (toolkitRootFolder) ? Path.Combine(rootPath, exeFileName) : exeFileName;
            startInfo.WorkingDirectory = rootPath;

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

            var output = String.Empty;

            if (runInBackground)
                output = process.StandardOutput.ReadToEnd();

            return output;
        }

        public static string RandomName(int iLen)
        {
            var builder = new StringBuilder(iLen);

            for (int i = 0; i < iLen; i++)
                builder.Append((char)randomNumber.Next(0x61, 0x7A)); // Alpha Lower Case Only

            return builder.ToString();
        }

        public static long RandomLong(long lMin, long lMax)
        {
            return lMin + randomNumber.Next() % (lMax - lMin);
        }

        public static int ToInt32(this string value)
        {
            int v;
            if (!int.TryParse(value, out v))
                return -1;
            return v;
        }

        public static string ToHex(this string inputString)
        {
            byte[] bArray = Encoding.Default.GetBytes(inputString);
            var hexString = BitConverter.ToString(bArray);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        public static byte[] ToByteArray(this string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                    .ToArray();
        }

        public static string ToLowerId(this Guid guid)
        {
            return guid.ToString().Replace("-", "").ToLower();
        }

        public static byte[] ImageToBytes(this Image image, ImageFormat format)
        {
            byte[] xret = null;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                xret = ms.ToArray();
            }
            return xret;
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

        public static string GetTempFileName(string extension = ".tmp")
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + extension);
        }

        public static string CopyToTempFile(this string file, string extension = ".tmp")
        {
            var tmp = GetTempFileName(extension);
            if (File.Exists(file))
                File.Copy(file, tmp);
            return tmp;
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

        public static bool IsBetween(float testValue, float minValue, float maxValue)
        {
            if (testValue >= minValue && testValue <= maxValue)
                return true;

            return false;
        }

    }
}
