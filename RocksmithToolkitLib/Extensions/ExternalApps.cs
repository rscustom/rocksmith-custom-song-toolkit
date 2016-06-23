using System;
using System.IO;
using System.Windows.Forms;

namespace RocksmithToolkitLib.Extensions
{
    public static class ExternalApps
    {
        private const string APP_TOPNG = "topng.exe";
        private const string APP_7Z = "7za.exe";
        private const string APP_NVDXT = "nvdxt.exe";
        private const string APP_PACKER = "packer.exe";
        private const string APP_OGGCUT = "oggCut.exe";
        private const string APP_OGGDEC = "oggdec.exe";
        private const string APP_OGGENC = "oggenc.exe";

        public static void VerifyExternalApps()
        {
            // Verifying if third party app is in root application directory
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_TOPNG)))
                throw new FileNotFoundException("topng.exe not found!");

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_7Z)))
                throw new FileNotFoundException("7za.exe not found!");

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_NVDXT)))
                throw new FileNotFoundException("nvdxt.exe not found!");

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_PACKER)))
                throw new FileNotFoundException("packer.exe not found!");

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_OGGCUT)))
                throw new FileNotFoundException("oggCut.exe not found!");

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_OGGDEC)))
                throw new FileNotFoundException("oggdec.exe not found!");

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_OGGENC)))
                throw new FileNotFoundException("oggenc.exe not found!");
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
            GeneralExtensions.RunExternalExecutable(APP_TOPNG, true, true, true, cmdArgs);
        }

        public static void PngFlipX(string sourcePath)
        {
            var cmdArgs = String.Format("-overwrite -xflip \"{0}\"", sourcePath);
            GeneralExtensions.RunExternalExecutable(APP_TOPNG, true, true, true, cmdArgs);
        }

        public static void PngFlipY(string sourcePath)
        {
            var cmdArgs = String.Format("-overwrite -yflip \"{0}\"", sourcePath);
            GeneralExtensions.RunExternalExecutable(APP_TOPNG, true, true, true, cmdArgs);
        }

        public static void Png2Dds(string sourcePath, string destinationPath, int xSize, int ySize)
        {
            var cmdArgs = String.Format(" -file \"{0}\" -prescale {2} {3} -quality_highest -max -dxt5 -nomipmap -alpha -overwrite -output \"{1}\"", sourcePath, destinationPath, xSize, ySize);
            GeneralExtensions.RunExternalExecutable(APP_NVDXT, true, true, true, cmdArgs);
        }

        public static void UnpackPsarc(string sourcePath, string destinationPath, string targetPlatform)
        {
            var cmdArgs = String.Format(" --unpack --input=\"{0}\" --platform={2} --version=RS2014 --output=\"{1}\"", sourcePath, destinationPath, targetPlatform);
            GeneralExtensions.RunExternalExecutable(APP_PACKER, true, true, true, cmdArgs);
        }
        //TODO: faster to use direct lib calls?? If this works lib calls will too.
        public static void RepackPsarc(string sourcePath, string destinationPath, string targetPlatform)
        {
            var cmdArgs = String.Format(" --pack --input=\"{0}\" --platform={2} --version=RS2014 --output=\"{1}\"", sourcePath, destinationPath, targetPlatform);
            GeneralExtensions.RunExternalExecutable(APP_PACKER, true, true, true, cmdArgs);
        }

        public static void InjectZip(string sourcePath, string destinationPath, bool recurseDir = false, bool filesOnly = false)
        {
            var cmdSwitch = String.Empty;
            if (recurseDir) cmdSwitch = " -r"; // do not remove space
            if (filesOnly) sourcePath = Path.Combine(sourcePath, "*");
            // CRITICAL spacing in cmdArgs
            var cmdArgs = String.Format(" a \"{0}\"{2} \"{1}\"", destinationPath, sourcePath, cmdSwitch);
            GeneralExtensions.RunExternalExecutable(APP_7Z, true, true, true, cmdArgs);
        }

        public static void ExtractZip(string sourcePath, string destinationPath, bool overwriteExisting = true, bool runInBackground = true)
        {
            var cmdSwitch = String.Empty;
            if (overwriteExisting) cmdSwitch = " -aoa";

            // CRITICAL spacing in cmdArgs ... there can be no space after "-o" or it doesn't work
            var cmdArgs = String.Format(" x \"{0}\"{2} -o\"{1}\"", sourcePath, destinationPath, cmdSwitch);
            if (runInBackground)
            {
                GeneralExtensions.RunExternalExecutable(APP_7Z, true, true, true, cmdArgs);
            }
            else
            {
                GeneralExtensions.RunExternalExecutable(APP_7Z, true, false, true, cmdArgs);
            }
        }

        public static void Wav2Ogg(string sourcePath, string destinationPath, int qualityFactor)
        {
            if (destinationPath == null)
                destinationPath = String.Format("{0}", Path.ChangeExtension(sourcePath, "ogg"));
            // interestingly ODLC uses 44100 or 48000 interchangeably ... so resampling is not necessary
            var cmdArgs = String.Format(" -q {2} \"{0}\" -o \"{1}\"", sourcePath, destinationPath, Convert.ToString(qualityFactor));

            GeneralExtensions.RunExternalExecutable(APP_OGGENC, true, false, true, cmdArgs);
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

            GeneralExtensions.RunExternalExecutable(APP_OGGENC, true, false, true, cmdArgs);
        }


        public static void Ogg2Preview(string sourcePath, string destinationPath, long msLength = 30000, long msStart = 4000)
        {
            var cmdArgs = String.Format(" -s {2} -l {3} \"{0}\" \"{1}\"", sourcePath, destinationPath, msStart, msLength);

            GeneralExtensions.RunExternalExecutable(APP_OGGCUT, true, false, true, cmdArgs);
        }

        public static void Ogg2Wav(string sourcePath, string destinationPath)
        {
            var cmdArgs = String.Format(" -o \"{1}\" \"{0}\"", sourcePath, destinationPath);

            GeneralExtensions.RunExternalExecutable(APP_OGGDEC, true, false, true, cmdArgs);
        }

        public static void Preview2Wav(string sourcePath)
        {
            var dirName = Path.GetDirectoryName(sourcePath);
            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            var dirFileName = Path.Combine(dirName, fileName);
            var srcPath = String.Format("{0}_{1}.ogg", dirFileName, "preview");
            var destPath = Path.ChangeExtension(srcPath, ".wav");

            var cmdArgs = String.Format(" -o \"{1}\" \"{0}\"", srcPath, destPath);

            GeneralExtensions.RunExternalExecutable(APP_OGGDEC, true, false, true, cmdArgs);
        }

        public static void Wav2Wem(string wwiseCLIPath, string wwiseTemplateDir)
        {
            var templatePath = Path.Combine(wwiseTemplateDir, "Template.wproj");
            var cmdArgs = String.Format(" \"{0}\" -GenerateSoundBanks -Platform Windows", templatePath);

            GeneralExtensions.RunExternalExecutable(wwiseCLIPath, true, false, true, cmdArgs);
        }

    }
}
