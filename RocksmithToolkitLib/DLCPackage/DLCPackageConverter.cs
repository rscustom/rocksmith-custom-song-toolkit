using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.AggregateGraph2014;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using X360.STFS;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class DLCPackageConverter
    {
        /// <summary>
        /// Converts CDLC packages between platforms
        /// </summary>
        /// <param name="sourcePackage"></param>
        /// <param name="sourcePlatform"></param>
        /// <param name="targetPlatform"></param>
        /// <param name="appId"></param>
        /// <returns>Errors if any</returns>
        public static string Convert(string sourcePackage, Platform sourcePlatform, Platform targetPlatform, string appId)
        {

            var needRebuildPackage = sourcePlatform.IsConsole != targetPlatform.IsConsole;
            var tmpDir = Path.GetTempPath();

            // key to good conversion is to overwriteSongXml
            var unpackedDir = Packer.Unpack(sourcePackage, tmpDir, sourcePlatform, false, true);

            // DESTINATION
            var nameTemplate = (!targetPlatform.IsConsole) ? "{0}{1}.psarc" : "{0}{1}";
            var packageName = Path.GetFileNameWithoutExtension(sourcePackage).StripPlatformEndName();
            packageName = packageName.Replace(".", "_");
            var targetFileName = String.Format(nameTemplate, Path.Combine(Path.GetDirectoryName(sourcePackage), packageName), targetPlatform.GetPathName()[2]);

            // force package rebuilding for testing
            // needRebuildPackage = true;
            // CONVERSION
            if (needRebuildPackage) 
                ConvertPackageRebuilding(unpackedDir, targetFileName, sourcePlatform, targetPlatform, appId);
            else
                ConvertPackageForSimilarPlatform(unpackedDir, targetFileName, sourcePlatform, targetPlatform, appId);

            IOExtension.DeleteDirectory(unpackedDir);

            return String.Empty;
        }

        private static void ConvertPackageForSimilarPlatform(string unpackedDir, string targetFileName, Platform sourcePlatform, Platform targetPlatform, string appId)
        {
            // Old and new paths
            var sourceDir0 = sourcePlatform.GetPathName()[0].ToLower();
            var sourceDir1 = sourcePlatform.GetPathName()[1].ToLower();
            var targetDir0 = targetPlatform.GetPathName()[0].ToLower();
            var targetDir1 = targetPlatform.GetPathName()[1].ToLower();

            if (!targetPlatform.IsConsole)
            {
                // Replace AppId
                var appIdFile = Path.Combine(unpackedDir, "appid.appid");
                File.WriteAllText(appIdFile, appId);
            }

            // Replace aggregate graph values
            var aggregateFile = Directory.EnumerateFiles(unpackedDir, "*.nt", SearchOption.AllDirectories).FirstOrDefault();
            var aggregateGraphText = File.ReadAllText(aggregateFile);
            // Tags
            aggregateGraphText = Regex.Replace(aggregateGraphText, GraphItem.GetPlatformTagDescription(sourcePlatform.platform), GraphItem.GetPlatformTagDescription(targetPlatform.platform), RegexOptions.Multiline);
            // Paths
            aggregateGraphText = Regex.Replace(aggregateGraphText, sourceDir0, targetDir0, RegexOptions.Multiline);
            aggregateGraphText = Regex.Replace(aggregateGraphText, sourceDir1, targetDir1, RegexOptions.Multiline);
            File.WriteAllText(aggregateFile, aggregateGraphText);

            // Rename directories
            foreach (var dir in Directory.GetDirectories(unpackedDir, "*.*", SearchOption.AllDirectories))
            {
                if (dir.EndsWith(sourceDir0))
                {
                    var newDir = dir.Substring(0, dir.LastIndexOf(sourceDir0)) + targetDir0;
                    IOExtension.DeleteDirectory(newDir);
                    IOExtension.MoveDirectory(dir, newDir);
                }
                else if (dir.EndsWith(sourceDir1))
                {
                    var newDir = dir.Substring(0, dir.LastIndexOf(sourceDir1)) + targetDir1;
                    IOExtension.DeleteDirectory(newDir);
                    IOExtension.MoveDirectory(dir, newDir);
                }
            }

            // Recreates SNG because SNG have different keys in PC and Mac
            bool updateSNG = ((sourcePlatform.platform == GamePlatform.Pc && targetPlatform.platform == GamePlatform.Mac) || (sourcePlatform.platform == GamePlatform.Mac && targetPlatform.platform == GamePlatform.Pc));

            // Packing
            var dirToPack = unpackedDir;
            if (sourcePlatform.platform == GamePlatform.XBox360)
                dirToPack = Directory.GetDirectories(Path.Combine(unpackedDir, Packer.ROOT_XBOX360))[0];

            Packer.Pack(dirToPack, targetFileName, targetPlatform, updateSNG);
            IOExtension.DeleteDirectory(unpackedDir);
        }

        private static void ConvertPackageRebuilding(string unpackedDir, string targetFileName, Platform sourcePlatform, Platform targetPlatform, string appId)
        {
            var data = DLCPackageData.LoadFromFolder(unpackedDir, targetPlatform, sourcePlatform);
            // Update AppID
            if (!targetPlatform.IsConsole)
                data.AppId = appId;

            // Build
            DLCPackageCreator.Generate(targetFileName, data, new Platform(targetPlatform.platform, GameVersion.RS2014));
        }
    }
}
