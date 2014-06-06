using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.Extensions
{
    public static class ExternalApps
    {
        private const string APP_TOPNG = "topng.exe";
        private const string APP_7Z = "7za.exe";
        private const string APP_NVDXT = "nvdxt.exe";
        private const string APP_PACKER = "packer.exe";


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
    }
}
