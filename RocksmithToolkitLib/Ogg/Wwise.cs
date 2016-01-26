using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Extensions;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using System.Diagnostics;

namespace RocksmithToolkitLib.Ogg
{
    public static class Wwise
    {
        static OggFile.WwiseVersion Selected { get; set; }
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
                //overridden ex, can't get real ex/msg, use log + throw;
                throw new Exception("Wwise audio file conversion failed: " + ex.Message);
            }
        }

        public static string GetWwisePath()
        {
            string wwisePath;

            // Audiokinetic Wwise might not be installed in the default location ;<
            // added custom Wwise install path to toolkit configuration menu
            if (!String.IsNullOrEmpty(ConfigRepository.Instance()["general_wwisepath"]))
                wwisePath = ConfigRepository.Instance()["general_wwisepath"];
            else
                wwisePath = Environment.GetEnvironmentVariable("WWISEROOT");

            if (String.IsNullOrEmpty(wwisePath))
                throw new FileNotFoundException("Could not find Audiokinetic Wwise installation." + Environment.NewLine + "Please confirm that Wwise v2013.2.x v2014.1.x or 2015.1.x series is installed.");

            Selected = OggFile.WwiseVersion.None;
            var pathWwiseCli = Directory.EnumerateFiles(wwisePath, "WwiseCLI.exe", SearchOption.AllDirectories).FirstOrDefault();
            if (pathWwiseCli == null)
                throw new FileNotFoundException("Could not find WwiseCLI.exe in " + wwisePath + Environment.NewLine + "Please confirm that Wwise v2013.2.x v2014.1.x or 2015.1.x series is installed.");

            var wwiseVersion = FileVersionInfo.GetVersionInfo(pathWwiseCli).ProductVersion;
            if (wwiseVersion.StartsWith("2013.2"))
                Selected = OggFile.WwiseVersion.Wwise2013;
            if (wwiseVersion.StartsWith("2014.1"))
                Selected = OggFile.WwiseVersion.Wwise2014;
            if (wwiseVersion.StartsWith("2015.1"))
                Selected = OggFile.WwiseVersion.Wwise2015;
            // add support for new versions here, code is expandable
            //if (wwiseVersion.StartsWith("201x.x"))
            //    Selected = OggFile.WwiseVersion.Wwise201x;
            if (Selected == OggFile.WwiseVersion.None)
                throw new FileNotFoundException("You have no compatible version of Audiokinetic Wwise installed." +
                Environment.NewLine + "Install Wwise v2013.2.x or v2014.1.x or even v2015.1.x series if you would like to use" +
                Environment.NewLine + " the toolkit OGG/WAV audio to Wwise WEM audio auto convert features.");

            return pathWwiseCli;
        }

        public static string LoadWwiseTemplate(string sourcePath, int audioQuality, string wwisePath)
        {
            var appRootDir = Path.GetDirectoryName(Application.ExecutablePath);
            var templateDir = Path.Combine(appRootDir, "Template");
            //Unpack required template here, based on wwise version installed.
            switch (Selected)
            {
                // add support for new versions of Wwise here
                case OggFile.WwiseVersion.Wwise2013:
                case OggFile.WwiseVersion.Wwise2014:
                case OggFile.WwiseVersion.Wwise2015:
                    ExtractTemplate(Path.Combine(appRootDir, Selected + ".tar.bz2"));
                    break;
                default:
                    throw new FileNotFoundException("Wwise path is incompatible.");
            }
            var resString = String.Empty;
            var workUnitPath = Path.Combine(templateDir, "Interactive Music Hierarchy", "Default Work Unit.wwu");
            using (var sr = new StreamReader(File.OpenRead(workUnitPath)))
            {
                resString = sr.ReadToEnd(); sr.Close();
                resString = resString.Replace("%QF1%", Convert.ToString(audioQuality));
                resString = resString.Replace("%QF2%", "4"); //preview

                var tw = new StreamWriter(workUnitPath, false);
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
            var destPreviewPath = string.Format("{0}_preview.wem", destinationPath.Substring(0, destinationPath.LastIndexOf(".", StringComparison.Ordinal)));
            foreach (var srcPath in srcPaths)
            {
                File.Copy(srcPath.FullName, srcPath.Name.Contains("_preview_") ? destPreviewPath : destinationPath, true);
            }
        }


    }
}