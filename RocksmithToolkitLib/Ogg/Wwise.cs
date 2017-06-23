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
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitLib.Ogg
{
    public static class Wwise
    {
        static OggFile.WwiseVersion Selected { get; set; }

        /// <summary>
        /// Covert Wav to Wem using WwiseCLI.exe
        /// for faster conversion, source path should be wav file
        /// </summary>
        /// <param name="wavSourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="audioQuality"></param>
        public static void Convert2Wem(string wavSourcePath, string destinationPath, int audioQuality)
        {
            try
            {
                var wwiseCLIPath = GetWwisePath();
                var wwiseTemplateDir = LoadWwiseTemplate(wavSourcePath, audioQuality);

                Console.WriteLine("WwiseCLI:\n\'" + wwiseCLIPath + "\'\n\nTemplate:\n\'" + wwiseTemplateDir + "\'");

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
            string wwiseCLIPath;

            // Audiokinetic Wwise might not be installed in the default location ;<
            if (!String.IsNullOrEmpty(ConfigRepository.Instance()["general_wwisepath"]))
                wwiseCLIPath = ConfigRepository.Instance()["general_wwisepath"];
            else
                wwiseCLIPath = Environment.GetEnvironmentVariable("WWISEROOT");

            if (String.IsNullOrEmpty(wwiseCLIPath))
                throw new FileNotFoundException("Could not find Audiokinetic Wwise installation." + Environment.NewLine + "Please confirm that either Wwise v2013.2.x v2014.1.x 2015.1.x or 2016.2.x series is installed.");

            //Wwise has default and x64 build's so make sure we've picked sufficient one (e.g x64 on x64 machine) win32 = 32bit x64 = 64bit
            Selected = OggFile.WwiseVersion.None;
            string pathWwiseCli;
            if (Environment.Is64BitOperatingSystem)
                pathWwiseCli = Directory.EnumerateFiles(wwiseCLIPath, "WwiseCLI.exe", SearchOption.AllDirectories).AsParallel().FirstOrDefault((arg) => arg.Contains("Authoring\\x64"));
            else
                pathWwiseCli = Directory.EnumerateFiles(wwiseCLIPath, "WwiseCLI.exe", SearchOption.AllDirectories).AsParallel().FirstOrDefault((arg) => arg.Contains("Authoring\\Win32"));
            if (pathWwiseCli == null)
                throw new FileNotFoundException("Could not find WwiseCLI.exe in " + wwiseCLIPath + Environment.NewLine + "Please confirm that either Wwise v2013.2.x v2014.1.x 2015.1.x or 2016.2.x series is installed.");

#if (DEBUG)
            // 32 bit wwise can run on 64 bit machine just fine, actually even better
            // this check is causing an error in the release build for some user on some machines
            Console.WriteLine("64bit = {0}", GeneralExtensions.IsPE64BitType(pathWwiseCli));
#endif

            var wwiseVersion = FileVersionInfo.GetVersionInfo(pathWwiseCli).ProductVersion;
            if (wwiseVersion.StartsWith("2013.2"))
                Selected = OggFile.WwiseVersion.Wwise2013;
            else if (wwiseVersion.StartsWith("2014.1"))
                Selected = OggFile.WwiseVersion.Wwise2014;
            else if (wwiseVersion.StartsWith("2015.1"))
                Selected = OggFile.WwiseVersion.Wwise2015;
            else if (wwiseVersion.StartsWith("2016.2"))
                Selected = OggFile.WwiseVersion.Wwise2016;

            // add support for new versions here, code is expandable
            //else if (wwiseVersion.StartsWith("xxxx.x"))
            //    Selected = OggFile.WwiseVersion.WwiseXXXX;

            if (Selected == OggFile.WwiseVersion.None)
                throw new FileNotFoundException("You have no compatible version of Audiokinetic Wwise installed." +
                Environment.NewLine + "Install supportend Wwise version, which are v2013.2.x || v2014.1.x || v2015.1.x || v2016.2.x series" +
                Environment.NewLine + " if you would like to use our Wwise autoconvert feature.");

            return pathWwiseCli;
        }

        /// <summary>
        /// Unpack\modify\load wwise template.
        /// </summary>
        /// <returns>Modified wwise template directory.</returns>
        /// <param name="sourcePath">Source path.</param>
        /// <param name="audioQuality">Audio quality (-2 to 10).</param>
        public static string LoadWwiseTemplate(string sourcePath, int audioQuality)
        {
            var appRootDir = Path.GetDirectoryName(Application.ExecutablePath);
            var templateDir = Path.Combine(appRootDir, "Template");

            //Unpack required template here, based on Wwise version installed.
            switch (Selected)
            {
                // add support for new versions of Wwise here
                case OggFile.WwiseVersion.Wwise2013:
                case OggFile.WwiseVersion.Wwise2014:
                case OggFile.WwiseVersion.Wwise2015:
                case OggFile.WwiseVersion.Wwise2016:
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

            //if nothing missing maybe just create one? what is prupouse of this?
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
            const string wemDir = @".cache\Windows\SFX"; //could be platform dependent like "Mac" or "PS3"
            var wemPath = Path.Combine(wwiseTemplateDir, wemDir);
            var wemPathInfo = new DirectoryInfo(wemPath);
            Console.WriteLine("Wwise '.cache': " + wemPath);

            if (!wemPathInfo.Exists)
                throw new FileNotFoundException("Could not find Wwise template .cache Windows SFX directory");

            var srcPaths = wemPathInfo.EnumerateFiles("*.wem", SearchOption.AllDirectories).ToArray();
            if (srcPaths.Length != 2)
                throw new Exception("Did not find converted Wem audio and preview files");

            //var destPreviewPath = Path.Combine(Path.GetDirectoryName(destinationPath), Path.GetFileNameWithoutExtension(destinationPath) + "_preview.wem");
            var destPreviewPath = string.Format("{0}_preview.wem", destinationPath.Substring(0, destinationPath.LastIndexOf(".", StringComparison.Ordinal)));
            foreach (var srcPath in srcPaths)
            {
                //fix headers for wwise v2016 wem's
                if (Selected == OggFile.WwiseVersion.Wwise2016)
                    OggFile.DowngradeWemVersion(srcPath.FullName, srcPath.Name.Contains("_preview_") ? destPreviewPath : destinationPath);
                else
                    File.Copy(srcPath.FullName, srcPath.Name.Contains("_preview_") ? destPreviewPath : destinationPath, true);
            }
        }


    }
}