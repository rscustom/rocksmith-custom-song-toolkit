using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
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
        public static string Convert(string sourcePackage, Platform sourcePlatform, Platform targetPlatform, string appId) {

            var needRebuildPackage = sourcePlatform.IsConsole != targetPlatform.IsConsole;
            var tmpDir = Path.GetTempPath();

            var fileName = Path.GetFileNameWithoutExtension(sourcePackage);
            if (sourcePlatform.platform == GamePlatform.PS3)
                if (fileName.Contains(".psarc"))
                    fileName = fileName.Substring(0, fileName.LastIndexOf("."));

            var unpackedDir = Packer.Unpack(sourcePackage, tmpDir, false, sourcePlatform);

            // DESTINATION
            var nameTemplate = (!targetPlatform.IsConsole) ? "{0}{1}.psarc" : "{0}{1}";

            var packageName = Path.GetFileNameWithoutExtension(sourcePackage);
            if (packageName.EndsWith(new Platform(GamePlatform.Pc, GameVersion.None).GetPathName()[2]) ||
                    packageName.EndsWith(new Platform(GamePlatform.Mac, GameVersion.None).GetPathName()[2]) ||
                    packageName.EndsWith(new Platform(GamePlatform.XBox360, GameVersion.None).GetPathName()[2]) ||
                    packageName.EndsWith(new Platform(GamePlatform.PS3, GameVersion.None).GetPathName()[2] + ".psarc"))
            {
                packageName = packageName.Substring(0, packageName.LastIndexOf("_"));
            }
            var targetFileName = Path.Combine(Path.GetDirectoryName(sourcePackage), String.Format(nameTemplate, Path.Combine(Path.GetDirectoryName(sourcePackage), packageName), targetPlatform.GetPathName()[2]));

            var noXML = Directory.GetFiles(unpackedDir, "*_*.xml", SearchOption.AllDirectories).Length <= 1;
            var noShowlights = Directory.GetFiles(unpackedDir, "*_showlights.xml", SearchOption.AllDirectories).Length < 1;
            
            // CONVERSION                
            if (needRebuildPackage)
            {
                if (noXML || noShowlights)
                    return String.Format("Package {0} is not a custom song, you need a custom song to convert Rocksmith 2014 from non similiar platforms.", sourcePackage);

                ConvertPackageRebuilding(unpackedDir, targetFileName, targetPlatform, appId);
            }
            else
                ConvertPackageForSimilarPlatform(unpackedDir, targetFileName, sourcePlatform, targetPlatform, appId);

            DirectoryExtension.SafeDelete(unpackedDir);

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
            var aggregateFile = Directory.GetFiles(unpackedDir, "*.nt", SearchOption.AllDirectories)[0];
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
                    DirectoryExtension.SafeDelete(newDir);
                    DirectoryExtension.Move(dir, newDir);
                }
                else if (dir.EndsWith(sourceDir1))
                {
                    var newDir = dir.Substring(0, dir.LastIndexOf(sourceDir1)) + targetDir1;
                    DirectoryExtension.SafeDelete(newDir);
                    DirectoryExtension.Move(dir, newDir);
                }
            }

            // Recreates SNG because SNG have different keys in PC and Mac
            bool updateSNG = ((sourcePlatform.platform == GamePlatform.Pc && targetPlatform.platform == GamePlatform.Mac) || (sourcePlatform.platform == GamePlatform.Mac && targetPlatform.platform == GamePlatform.Pc));

            // Packing
            var dirToPack = unpackedDir;
            if (sourcePlatform.platform == GamePlatform.XBox360)
                dirToPack = Directory.GetDirectories(Path.Combine(unpackedDir, Packer.ROOT_XBox360))[0];

            Packer.Pack(dirToPack, targetFileName, updateSNG, targetPlatform);
            DirectoryExtension.SafeDelete(unpackedDir);
        }

        private static void ConvertPackageRebuilding(string unpackedDir, string targetFileName, Platform targetPlatform, string appId)
        {
            var data = DLCPackageData.LoadFromFile(unpackedDir, targetPlatform);
            if (!targetPlatform.IsConsole)
                data.AppId = appId;

            //Build
            RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(targetFileName, data, new Platform(targetPlatform.platform, GameVersion.RS2014));
        }
    }
}
