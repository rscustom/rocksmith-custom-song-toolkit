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
        /// Format input string as valid SongKey name.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="songTitle"></param>
        /// <returns>SongKey valid name</returns>
        public static string GetValidSongName(this string value, string songTitle)
        {
            string songName = String.Empty;
            if (!String.IsNullOrEmpty(value))
            {
                Regex rgx = new Regex("[^a-zA-Z0-9\\-]");
                songName = rgx.Replace(value, "");
                //Avoid SongKey==SongTitle case
                if (songName == songTitle)
                    songName = songName + "Song";
            }
            return songName;
        }

        public static string GetValidSortName(this string value)
        {        
            if (String.IsNullOrEmpty(value))
                return value;
            if (value.ToUpperInvariant().StartsWith("THE "))
                return value.Remove(0, 4);
            return value;
        }

        public static string GetValidName(this string value, bool allowSpace = false, bool allowStartsWithNumber = false, bool underscoreSpace = false, bool frets24 = false)
        {
            string name = String.Empty;
            if (!String.IsNullOrEmpty(value))
            {
                Regex rgx = new Regex((allowSpace) ? "[^a-zA-Z0-9\\-_ ]" : "[^a-zA-Z0-9\\-_]");
                name = rgx.Replace(value, "");

                Regex rgx2 = new Regex(@"^[\d]*\s*");
                if (!allowStartsWithNumber) name = rgx2.Replace(name, "");           
               
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
                
                if (underscoreSpace) name = name.Replace(" ", "_");
            }
            
            return name.Trim();
        }

        public static string StripPlatformEndName(this string value)
        {
            if (value.EndsWith(GamePlatform.Pc     .GetPathName()[2]) ||
                value.EndsWith(GamePlatform.Mac    .GetPathName()[2]) ||
                value.EndsWith(GamePlatform.XBox360.GetPathName()[2]) ||
                value.EndsWith(GamePlatform.PS3    .GetPathName()[2]) ||
                value.EndsWith(GamePlatform.PS3    .GetPathName()[2] + ".psarc"))
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
                return String.Format(Format, Artist.GetValidName(true, true), Title.GetValidName(true, true), Version).Replace(" ", "-");
            return String.Format(Format, Artist.Acronym(), Title.GetValidName(true, true), Version).Replace(" ", "-");
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
                byte[] file = File.ReadAllBytes(fileName);

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
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
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
            if(File.Exists(file))
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
    }
}
