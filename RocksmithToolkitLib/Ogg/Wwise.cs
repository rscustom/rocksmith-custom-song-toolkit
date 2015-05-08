using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Extensions;
using System.Linq;

namespace RocksmithToolkitLib.Ogg
{
    public static class Wwise
    {
        public static void Convert2Wem(string sourcePath, string destinationPath, int audioQuality)
        {
            try
            {
                LoadWwiseTemplate(sourcePath, audioQuality);
                ExternalApps.Wav2Wem(GetWwisePath());
                GetWwiseFiles(destinationPath);
            }
            catch (Exception ex)
            {
                //overrided ex, can't get real ex/msg, use log + throw;
                throw new Exception("Wwise audio file conversion failed: " + ex.Message);
            }
        }

        public static string GetWwisePath()
        {
            // Audiokinect Wwise might not be installed in the default location ;<
            // so added Wwise location to toolkit configuration menu
            if (!String.IsNullOrEmpty(ConfigRepository.Instance()["general_wwisepath"]))
                return ConfigRepository.Instance()["general_wwisepath"];

            try
            {
                var programsDir = String.Empty;

                if (Environment.OSVersion.Version.Major >= 6)
                    programsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Audiokinetic");
                else
                    programsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Audiokinetic");

                var pathWwiseCli = Directory.EnumerateFiles(programsDir, "WwiseCLI.exe", SearchOption.AllDirectories).FirstOrDefault();

                if (String.IsNullOrEmpty(Path.GetFileName(pathWwiseCli)))
                    throw new FileNotFoundException("Could not find WwiseCLI.exe in " + programsDir + Environment.NewLine + "Please confirm that Build 4828 is installed.");

                return pathWwiseCli;
            }
            catch (Exception ex)
            {
                MessageBox.Show(new Form { TopMost = true }, @"Could not find WwiseCLI.exe or Audiokinetic directory.  " + Environment.NewLine + @"Please confirm that it is installed and selected in the CST configuration menu.", @"Exception: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
                Environment.Exit(-1);
                return null;
            }
        }

        public static void LoadWwiseTemplate(string sourcePath, int audioQuality)
        {//TODO: use packed template
            var appRootDir = Path.GetDirectoryName(Application.ExecutablePath);
            var templateDir = Path.Combine(appRootDir, "Wwise\\Template");
            var orgSfxDir = Path.Combine(appRootDir, templateDir, "Originals\\SFX");
            //Unpack template here

            if (!Directory.Exists(orgSfxDir))
                throw new FileNotFoundException("Could not find Wwise template originals SFX directory.\r\nReinstall Midi2RsXml to fix problem.");

            if (File.Exists(Path.Combine(templateDir, "Template.Administrator.validationcache")))
                File.Delete(Path.Combine(templateDir, "Template.Administrator.validationcache"));

            if (Directory.Exists(Path.Combine(templateDir, ".cache")))
            {
                Directory.Delete(Path.Combine(templateDir, ".cache"), true);
                // TODO: fix this for newer versions
                // Wwise requires full .cache path be present???
                Directory.CreateDirectory(Path.Combine(templateDir, ".cache\\Windows\\SFX"));
            }

            // cleanup gives new hex value to WEM files
            if (Directory.Exists(Path.Combine(templateDir, "GeneratedSoundBanks")))
                Directory.Delete(Path.Combine(templateDir, "GeneratedSoundBanks"), true);

            var dirName = Path.GetDirectoryName(sourcePath);
            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            var dirFileName = Path.Combine(dirName, fileName);
            var sourcePreviewWave = String.Format("{0}_{1}.wav", dirFileName, "preview");

            File.Copy(sourcePath, Path.Combine(orgSfxDir, "Audio.wav"), true);
            File.Copy(sourcePreviewWave, Path.Combine(orgSfxDir, "Audio_preview.wav"), true);


            string resString = String.Empty;
            const string resName = "RocksmithToolkitLib.Resources.QF Default Work Unit.wwu";
            Assembly assem = Assembly.GetExecutingAssembly();
            string[] names = assem.GetManifestResourceNames();
            var stream = assem.GetManifestResourceStream(resName);

            if (stream != null)
            {
                var reader = new StreamReader(stream);
                resString = reader.ReadToEnd();
            }
            else
                throw new Exception("Can not find Audio Quality Factor resource");

            var workUnitPath = Path.Combine(templateDir, "Interactive Music Hierarchy", "Default Work Unit.wwu");
            resString = resString.Replace("%QF1%", Convert.ToString(audioQuality));
            resString = resString.Replace("%QF2%", "4");//preview
            using (TextWriter tw = new StreamWriter(workUnitPath, false))
            {
                tw.Write(resString);
                tw.Flush();
            }
        }


        public static void GetWwiseFiles(string destinationPath)
        {
            const string wemDir = @"Wwise\Template\.cache\Windows\SFX";
            var appRootDir = Path.GetDirectoryName(Application.ExecutablePath);
            var wemPath = Path.Combine(appRootDir, wemDir);
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
                if (srcPath.Name.Contains("_preview_"))
                    File.Copy(srcPath.FullName, destPreviewPath, true);
                else
                    File.Copy(srcPath.FullName, destinationPath, true);
            }
        }


    }
}