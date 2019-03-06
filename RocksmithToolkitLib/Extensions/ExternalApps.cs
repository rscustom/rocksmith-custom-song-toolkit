using System;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;

namespace RocksmithToolkitLib.Extensions
{
    public static class ExternalApps
    {
        // path fixed for unit testing compatiblity
        public static readonly string TOOLKIT_ROOT = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public static readonly string TOOLS_DIR = "tools";
        public static readonly string APP_TOPNG = Path.Combine(TOOLS_DIR, "topng.exe");
        public static readonly string APP_7Z = Path.Combine(TOOLS_DIR, "7za.exe");
        public static readonly string APP_NVDXT = Path.Combine(TOOLS_DIR, "nvdxt.exe");
        public static readonly string APP_OGGCUT = Path.Combine(TOOLS_DIR, "oggCut.exe");
        public static readonly string APP_OGGDEC = Path.Combine(TOOLS_DIR, "oggdec.exe");
        public static readonly string APP_OGGENC = Path.Combine(TOOLS_DIR, "oggenc.exe");
        public static readonly string APP_WW2OGG = Path.Combine(TOOLS_DIR, "ww2ogg.exe");
        public static readonly string APP_REVORB = Path.Combine(TOOLS_DIR, "revorb.exe");
        public static readonly string APP_CODEBOOKS = Path.Combine(TOOLS_DIR, "packed_codebooks.bin");
        public static readonly string APP_CODEBOOKS_603 = Path.Combine(TOOLS_DIR, "packed_codebooks_aoTuV_603.bin");
        public static readonly string APP_COREJAR = Path.Combine(TOOLS_DIR, "core.jar");
        public static readonly string DDC_DIR = "ddc";
        public static readonly string APP_DDC = Path.Combine(DDC_DIR, "ddc.exe");

        public static bool VerifyExternalApps()
        {
            try
            {
                var errMsg = new StringBuilder();

                // Verifying third party apps exist where toolkit expects to find them
                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_TOPNG)))
                    errMsg.AppendLine(APP_TOPNG);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_7Z)))
                    errMsg.AppendLine(APP_7Z);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_NVDXT)))
                    errMsg.AppendLine(APP_NVDXT);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_OGGDEC)))
                    errMsg.AppendLine(APP_OGGDEC);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_OGGCUT)))
                    errMsg.AppendLine(APP_OGGCUT);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_OGGENC)))
                    errMsg.AppendLine(APP_OGGENC);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_COREJAR)))
                    errMsg.AppendLine(APP_COREJAR);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_WW2OGG)))
                    errMsg.AppendLine(APP_WW2OGG);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_REVORB)))
                    errMsg.AppendLine(APP_REVORB);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_CODEBOOKS)))
                    errMsg.AppendLine(APP_CODEBOOKS);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_CODEBOOKS_603)))
                    errMsg.AppendLine(APP_CODEBOOKS_603);

                if (!File.Exists(Path.Combine(TOOLKIT_ROOT, APP_DDC)))
                    errMsg.AppendLine(APP_DDC);

                if (!String.IsNullOrEmpty(errMsg.ToString()))
                    throw new FileNotFoundException("<ERROR> Critical toolkit files not found:" + Environment.NewLine + errMsg.ToString());
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("<ERROR> Critical toolkit files not found:" + Environment.NewLine + ex.Message);
            }

            return true;
        }

        public static void Dds2Png(string sourcePath, string destinationPath = null)
        {
            var cmdArgs = String.Empty;
            if (destinationPath == null)
            {
                cmdArgs = String.Format(" -overwrite -out png \"{0}\"", sourcePath);
            }
            else
            {
                cmdArgs = String.Format(" -overwrite -out png -o \"{1}\" \"{0}\"", sourcePath, destinationPath);
            }
            GeneralExtension.RunExternalExecutable(APP_TOPNG, true, true, true, cmdArgs);
        }

        public static void PngFlipX(string sourcePath)
        {
            var cmdArgs = String.Format("-overwrite -xflip \"{0}\"", sourcePath);
            GeneralExtension.RunExternalExecutable(APP_TOPNG, true, true, true, cmdArgs);
        }

        public static void PngFlipY(string sourcePath)
        {
            var cmdArgs = String.Format("-overwrite -yflip \"{0}\"", sourcePath);
            GeneralExtension.RunExternalExecutable(APP_TOPNG, true, true, true, cmdArgs);
        }

        public static void Png2Dds(string sourcePath, string destinationPath, int xSize, int ySize)
        {
            var cmdArgs = String.Format(" -file \"{0}\" -prescale {2} {3} -quality_highest -max -dxt5 -nomipmap -alpha -overwrite -output \"{1}\"", sourcePath, destinationPath, xSize, ySize);
            GeneralExtension.RunExternalExecutable(APP_NVDXT, true, true, true, cmdArgs);
        }

        public static void UnpackPsarc(string sourcePath, string destinationPath, string targetPlatform)
        {
            var cmdArgs = String.Format(" --unpack --input=\"{0}\" --platform={2} --version=RS2014 --output=\"{1}\"", sourcePath, destinationPath, targetPlatform);
            GeneralExtension.RunExternalExecutable(Path.Combine(TOOLKIT_ROOT, "packer.exe"), true, true, true, cmdArgs);
        }

        public static void RepackPsarc(string sourcePath, string destinationPath, string targetPlatform)
        {
            var cmdArgs = String.Format(" --pack --input=\"{0}\" --platform={2} --version=RS2014 --output=\"{1}\"", sourcePath, destinationPath, targetPlatform);
            GeneralExtension.RunExternalExecutable(Path.Combine(TOOLKIT_ROOT, "packer.exe"), true, true, true, cmdArgs);
        }

        public static void InjectZip(string sourcePath, string destinationPath, bool recurseDir = false, bool filesOnly = false)
        {
            var cmdSwitch = String.Empty;
            if (recurseDir) cmdSwitch = " -r"; // do not remove space
            if (filesOnly) sourcePath = Path.Combine(sourcePath, "*");
            // CRITICAL spacing in cmdArgs
            var cmdArgs = String.Format(" a \"{0}\"{2} \"{1}\"", destinationPath, sourcePath, cmdSwitch);
            GeneralExtension.RunExternalExecutable(APP_7Z, true, true, true, cmdArgs);
        }

        public static void ExtractZip(string sourcePath, string destinationPath, bool overwriteExisting = true, bool runInBackground = true)
        {
            var cmdSwitch = String.Empty;
            if (overwriteExisting) cmdSwitch = " -aoa";

            // CRITICAL spacing in cmdArgs ... there can be no space after "-o" or it doesn't work
            var cmdArgs = String.Format(" x \"{0}\"{2} -o\"{1}\"", sourcePath, destinationPath, cmdSwitch);
            if (runInBackground)
            {
                GeneralExtension.RunExternalExecutable(APP_7Z, true, true, true, cmdArgs);
            }
            else
            {
                GeneralExtension.RunExternalExecutable(APP_7Z, true, false, true, cmdArgs);
            }
        }

        public static void Wav2Ogg(string sourcePath, string destinationPath, int qualityFactor)
        {
            if (destinationPath == null)
                destinationPath = String.Format("{0}", Path.ChangeExtension(sourcePath, "ogg"));
            // interestingly ODLC uses 44100 or 48000 interchangeably ... so resampling is not necessary
            var cmdArgs = String.Format(" -q {2} \"{0}\" -o \"{1}\"", sourcePath, destinationPath, Convert.ToString(qualityFactor));

            GeneralExtension.RunExternalExecutable(APP_OGGENC, true, false, true, cmdArgs);
        }

        /// <summary>
        /// Convert audio file to ogg
        /// </summary>
        /// <param name="sourcePath"> RAW Audio, WAV, AIFF, FLAC, OGG</param>
        /// <param name="destinationPath">OGG</param>
        /// <param name="qualityFactor"> 0 (low) to 10 (high)</param>
        /// <param name="sampleRate"> (Hz), defaults to same sample rate as source if not specified</param>
        public static void Audio2Ogg(string sourcePath, string destinationPath, int qualityFactor, int sampleRate = 0)
        {
            if (destinationPath == null)
                destinationPath = String.Format("{0}", Path.ChangeExtension(sourcePath, "ogg"));

            var cmdArgs = String.Format(" -q {2} \"{0}\" -o \"{1}\"", sourcePath, destinationPath, qualityFactor);

            if (sampleRate > 0)
                cmdArgs += String.Format(" --resample {0}\"", sampleRate);

            GeneralExtension.RunExternalExecutable(APP_OGGENC, true, false, true, cmdArgs);
        }

        public static void Ogg2Preview(string sourcePath, string destinationPath, long msLength = 30000, long msStart = 4000)
        {
            var cmdArgs = String.Format(" -s {2} -l {3} \"{0}\" \"{1}\"", sourcePath, destinationPath, msStart, msLength);

            GeneralExtension.RunExternalExecutable(APP_OGGCUT, true, false, true, cmdArgs);
        }

        public static void Ogg2Wav(string sourcePath, string destinationPath)
        {
            var cmdArgs = String.Format(" -o \"{1}\" \"{0}\"", sourcePath, destinationPath);

            GeneralExtension.RunExternalExecutable(APP_OGGDEC, true, false, true, cmdArgs);
        }

        public static void Preview2Wav(string sourcePath)
        {
            var dirName = Path.GetDirectoryName(sourcePath);
            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            var dirFileName = Path.Combine(dirName, fileName);
            var srcPath = String.Format("{0}_{1}.ogg", dirFileName, "preview");
            var destPath = Path.ChangeExtension(srcPath, ".wav");

            var cmdArgs = String.Format(" -o \"{1}\" \"{0}\"", srcPath, destPath);

            GeneralExtension.RunExternalExecutable(APP_OGGDEC, true, false, true, cmdArgs);
        }

        public static void Wav2Wem(string wwiseCLIPath, string wwiseTemplateDir, int magicDust = 0)
        {
            var templatePath = Path.Combine(wwiseTemplateDir, "Template.wproj");
            // -NoWwiseDat ignores cached wem's and will generate each time.
            // -ClearAudioFileCache force re-generate for wem's also deletes old and creates fresh new file.
            // -Save should help with updating project to new schema (may loose quality factor field)
            var cmdArgs = String.Format("\"{0}\" -GenerateSoundBanks -Platform Windows -Language English(US) -NoWwiseDat -ClearAudioFileCache -Save", templatePath);
            var output = GeneralExtension.RunExternalExecutable(wwiseCLIPath, true, false, true, cmdArgs);

            if (output.Contains("Error: Project migration needed") && magicDust > 0)
            {
                Debug.WriteLine("WwiseCLI.exe Conversion Failed ...");
                Debug.WriteLine("Applying Magic Dust #" + magicDust);
                magicDust--;
                Wav2Wem(wwiseCLIPath, wwiseTemplateDir, magicDust);
            }
        }


    }
}
