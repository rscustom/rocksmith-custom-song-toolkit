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

namespace RocksmithToolkitLib.Extensions
{
    public static class GeneralExtensions
    {
        private static readonly Random randomNumber = new Random();

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
            Directory.CreateDirectory(Path.GetDirectoryName(re));

            return re;
        }

        public static ToolkitInfo GetToolkitInfo(StreamReader reader)
        {
            if (reader == null)
                return null;

            var tkInfo = new ToolkitInfo();
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

        public static string RunExternalExecutable(string exeFileName, bool toolkitRootFolder = true, bool runInBackground = false, bool waitToFinish = false, string arguments = null)
        {
            string toolkitRootPath = AppDomain.CurrentDomain.BaseDirectory; //Path.GetDirectoryName(Application.ExecutablePath);

            var rootPath = (toolkitRootFolder) ? toolkitRootPath : Path.GetDirectoryName(exeFileName);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.Combine(rootPath, exeFileName);
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

    }
}
