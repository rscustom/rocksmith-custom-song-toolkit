using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Extensions;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;

namespace RocksmithToolkitLib.Ogg
{
    public static class Wwise
    {
        public static void Convert2Wem(string sourcePath, string destinationPath, int audioQuality)
        {
            try
            {
                var wwiseCLIPath = GetWwisePath();
                var wwiseTemplateDir = LoadWwiseTemplate(sourcePath, audioQuality, wwiseCLIPath);
                ExternalApps.Wav2Wem(wwiseCLIPath, wwiseTemplateDir);
                GetWwiseFiles(destinationPath, wwiseTemplateDir);
            }
            catch (Exception ex)
            {
                //overrided ex, can't get real ex/msg, use log + throw;
                throw new Exception("Wwise audio file conversion failed: " + ex.Message);
            }
        }

        public static string GetWwisePath()
        {
            string wwisePath;

            // Audiokinect Wwise might not be installed in the default location ;<
            // added custom Wwise install path to toolkit configuration menu
            if (!String.IsNullOrEmpty(ConfigRepository.Instance()["general_wwisepath"]))
                wwisePath = ConfigRepository.Instance()["general_wwisepath"];
            else
                wwisePath = Environment.GetEnvironmentVariable("WWISEROOT");

            if (String.IsNullOrEmpty(wwisePath))
                throw new FileNotFoundException("Could not find Audiokinect Wwise installation." + Environment.NewLine + "Please confirm that Wwise v2013.2.x or v2014.1.x series are installed.");

            // No support for v2015.1.x yet, code is expandable
            if (!wwisePath.ToLower().Contains("v2013.2"))
                if (!wwisePath.ToLower().Contains("v2014.1"))
                    throw new FileNotFoundException("You have an incompatible version of Audiokinect Wwise installed." +
                    Environment.NewLine + "Install Wwise v2013.2.x or v2014.1.x series if you would like to use" +
                    Environment.NewLine + " the toolkit OGG/WAV audio to Wwise WEM audio auto convert features.");

            var pathWwiseCli = Directory.EnumerateFiles(wwisePath, "WwiseCLI.exe", SearchOption.AllDirectories).FirstOrDefault();

            if (!pathWwiseCli.Any())
                throw new FileNotFoundException("Could not find WwiseCLI.exe in " + wwisePath + Environment.NewLine + "Please confirm that Wwise v2013.2.x or v2014.1.x series is installed.");

            return pathWwiseCli;
        }

        public static string LoadWwiseTemplate(string sourcePath, int audioQuality, string wwisePath)
        {
            var appRootDir = Path.GetDirectoryName(Application.ExecutablePath);
            var packedTemplatePath2013 = Path.Combine(appRootDir, "Wwise2013.tar.bz2");
            var packedTemplatePath2014 = Path.Combine(appRootDir, "Wwise2014.tar.bz2");
            var templateDir = String.Empty;
            //Unpack required template here, based on wwise version installed.
            if (wwisePath.ToLower().Contains("v2013.2"))
            {
                ExtractTemplate(packedTemplatePath2013);
                templateDir = Path.Combine(appRootDir, "Wwise2013\\Template");
            }
            else if (wwisePath.ToLower().Contains("v2014.1"))
            {
                ExtractTemplate(packedTemplatePath2014);
                templateDir = Path.Combine(appRootDir, "Wwise2014\\Template");
            }
            // expandable to next new Wwise version here
            else
                throw new FileNotFoundException("Wwise path is incompatible.");

            string resString = String.Empty;
            var workUnitPath = Path.Combine(templateDir, "Interactive Music Hierarchy", "Default Work Unit.wwu");
            using (var sr = new StreamReader(File.OpenRead(workUnitPath)))
            {
                resString = sr.ReadToEnd();
            }

            resString = resString.Replace("%QF1%", Convert.ToString(audioQuality));
            resString = resString.Replace("%QF2%", "4");//preview

            using (TextWriter tw = new StreamWriter(workUnitPath, false))
            {
                tw.Write(resString);
                tw.Flush();
            }

            var orgSfxDir = Path.Combine(templateDir, "Originals\\SFX");
            if (!Directory.Exists(orgSfxDir))
                throw new FileNotFoundException("Could not find Wwise template originals SFX directory.\r\nReinstall CST to fix problem.");

            var vcache = Directory.EnumerateFiles(templateDir, "Template.*.validationcache").FirstOrDefault();
            if (File.Exists(vcache))
                File.Delete(vcache);

            var cache = Path.Combine(templateDir, ".cache");
            if (Directory.Exists(cache))
            {
                Directory.Delete(cache, true);
                // WwiseCLI requires full .cache path be present??? Usually not.
                Directory.CreateDirectory(Path.Combine(templateDir, ".cache\\Windows\\SFX"));
            }

            // cleanup gives new hex value to WEM files
            var bnk = Path.Combine(templateDir, "GeneratedSoundBanks");
            if (Directory.Exists(bnk))
                Directory.Delete(bnk, true);

            var dirName = Path.GetDirectoryName(sourcePath);
            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            var dirFileName = Path.Combine(dirName, fileName);
            var sourcePreviewWave = String.Format("{0}_{1}.wav", dirFileName, "preview");

            File.Copy(sourcePath, Path.Combine(orgSfxDir, "Audio.wav"), true);
            File.Copy(sourcePreviewWave, Path.Combine(orgSfxDir, "Audio_preview.wav"), true);

            return templateDir;
        }

        // about 800ms here, not that slow.
        public static void ExtractTemplate(string packedTemplatePath)
        {
            var appRootDir = Path.GetDirectoryName(Application.ExecutablePath);
            // speed up by only unpacking one time, also allows users to upgrade Wwise later
            var wwiseFolder = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(packedTemplatePath));
            if (Directory.Exists(Path.Combine(appRootDir, wwiseFolder)))
                return;

            using (var packedTemplate = File.OpenRead(packedTemplatePath))
            using (var bz2 = new BZip2InputStream(packedTemplate))
            using (var tar = TarArchive.CreateInputTarArchive(bz2))
            {
                tar.ExtractContents(appRootDir);
            }
        }

        public static void GetWwiseFiles(string destinationPath, string wwiseTemplateDir)
        {
            const string wemDir = @".cache\Windows\SFX";
            var wemPath = Path.Combine(wwiseTemplateDir, wemDir);
            var wemPathInfo = new DirectoryInfo(wemPath);

            if (!wemPathInfo.Exists)
                throw new FileNotFoundException("Could not find Wwise template .cache Windows SFX directory");

            var srcPaths = wemPathInfo.EnumerateFiles("*.wem", SearchOption.TopDirectoryOnly).ToArray();
            if (srcPaths.Length != 2)
                throw new Exception("Did not find converted Wem audio and preview files");

            //var destPreviewPath = Path.Combine(Path.GetDirectoryName(destinationPath), Path.GetFileNameWithoutExtension(destinationPath) + "_preview.wem");
            var destPreviewPath = string.Format("{0}_preview.wem", destinationPath.Substring(0, destinationPath.LastIndexOf(".")));
            foreach (var srcPath in srcPaths)
            {
                File.Copy(srcPath.FullName, srcPath.Name.Contains("_preview_") ? destPreviewPath : destinationPath, true);
            }
        }


    }
}