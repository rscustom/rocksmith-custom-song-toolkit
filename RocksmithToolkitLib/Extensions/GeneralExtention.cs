using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage;
using System.Windows.Forms;
using System.Diagnostics;

namespace RocksmithToolkitLib.Extensions
{
    public static class GeneralExtensions
    {
        public static bool Contains(this String obj, char[] chars)
        {
            return (obj.IndexOfAny(chars) >= 0);
        }

        public static string GetDescription(this object value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string[] SelectLines(this string[] content, string value) {
            return (from j in content
                    where j.Contains(value)
                    select j).ToArray<string>();
        }

        public static int ToInt32(this string value)
        {
            int v;
            if (int.TryParse(value, out v) == false)
                return -1;
            return v;
        }

        public static string GetValidSongName(this string value, string songTitle)
        {
            string name = String.Empty;
            if (!String.IsNullOrEmpty(value))
            {
                Regex rgx = new Regex("[^a-zA-Z0-9\\-]");
                name = rgx.Replace(value, "");
                if (name == songTitle)
                    name = name + "Song";
            }
            return name;
        }

        public static string GetValidName(this string value, bool allowSpace = true)
        {
            string name = String.Empty;
            if (!String.IsNullOrEmpty(value))
            {
                Regex rgx = new Regex((allowSpace) ? "[^a-zA-Z0-9\\-_ ]" : "[^a-zA-Z0-9\\-_]");
                name = rgx.Replace(value, "");
            }
            return name;
        }

        public static string StripPlatformEndName(this string value) {
            if (value.EndsWith(new Platform(GamePlatform.Pc, GameVersion.None).GetPathName()[2]) ||
                value.EndsWith(new Platform(GamePlatform.Mac, GameVersion.None).GetPathName()[2]) ||
                value.EndsWith(new Platform(GamePlatform.XBox360, GameVersion.None).GetPathName()[2]) ||
                value.EndsWith(new Platform(GamePlatform.PS3, GameVersion.None).GetPathName()[2]) ||
                value.EndsWith(new Platform(GamePlatform.PS3, GameVersion.None).GetPathName()[2] + ".psarc"))
            {
                return value.Substring(0, value.LastIndexOf("_"));
            }

            return value;
        }

        public static string ToNullTerminatedAscii(this Byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
        }

        public static string Acronym(this string value)
        {
            return string.Join(string.Empty, value.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]));
        }

        public static string GetShortName(string Format, string Artist, string Title, string Version, bool Acronym = false)
        {
            if (!Acronym)
                return String.Format(Format, Artist.GetValidName(true), Title.GetValidName(true), Version.GetValidName(true)).Replace(" ", "-");
            else
                return String.Format(Format, Artist.Acronym(), Title.GetValidName(true), Version.GetValidName(true)).Replace(" ", "-");
        }

        public static bool IsValidPSARC(this string fileName)
        {
            //Supported DLC Package types
            var mimeByteHeaderList = new Dictionary<string, byte[]>();
            mimeByteHeaderList.Add(".psarc", ASCIIEncoding.ASCII.GetBytes("PSAR"));
            mimeByteHeaderList.Add(".edat", ASCIIEncoding.ASCII.GetBytes("NPD"));
            mimeByteHeaderList.Add("xbox", ASCIIEncoding.ASCII.GetBytes("CON"));

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
            else
                return false;
        }

        public static void OpenExecutable(string exeFileName, bool toolkitRootFolder = true) {
            string toolkitRootPath = Path.GetDirectoryName(Application.ExecutablePath);

            var rootPath = (toolkitRootFolder) ? toolkitRootPath : Path.GetDirectoryName(exeFileName);

            Process updaterProcess = new Process();
            updaterProcess.StartInfo.FileName = (toolkitRootFolder) ? Path.Combine(rootPath, exeFileName) : exeFileName;
            updaterProcess.StartInfo.WorkingDirectory = rootPath;
            updaterProcess.Start();
        }
    }
}
