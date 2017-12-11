using System;
using System.Diagnostics;
using System.IO;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitLib.DLCPackage
{
    public static class DDCreator
    {
        internal static string AppDir = AppDomain.CurrentDomain.BaseDirectory;
        internal static string DdcDir = Path.Combine(AppDir, "ddc");

        /// <summary>
        /// Apply Dynamic Difficulty (DD) to an arrangement xml file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="phraseLen"></param>
        /// <param name="removeSus"></param>
        /// <param name="rampPath"></param>
        /// <param name="cfgPath"></param>
        /// <param name="consoleOutput"></param>
        /// <param name="overWrite"></param>
        /// <param name="keepLog"></param>
        /// <returns>0 => ends normally, 1 => ends with system error, 2 => ends with application error</returns>
        public static int ApplyDD(string filePath, int phraseLen, bool removeSus, string rampPath, string cfgPath, out string consoleOutput, bool overWrite = false, bool keepLog = false)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(DdcDir, "ddc.exe"),
                WorkingDirectory = Path.GetDirectoryName(filePath),
                Arguments = String.Format("\"{0}\" -l {1} -s {2} -m \"{3}\" -c \"{4}\" -p {5} -t {6}",
                    Path.GetFileName(filePath), (UInt16)phraseLen, removeSus ? "Y" : "N",
                    rampPath, cfgPath, overWrite ? "Y" : "N", keepLog ? "Y" : "N"
                    ),
                UseShellExecute = false,
                CreateNoWindow = true,  // hide command window
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var DDC = new Process())
            {
                DDC.StartInfo = startInfo;
                DDC.Start();
                consoleOutput = DDC.StandardOutput.ReadToEnd();
                consoleOutput += DDC.StandardError.ReadToEnd();
                DDC.WaitForExit(1000 * 60 * 15); //wait for 15 minutes, crunchy solution for AV-sandboxing issues
                return DDC.ExitCode;
            }
        }
    }

    public class DDCSettings
    {
        public int PhraseLen { get; private set; }
        public bool RemoveSus { get; private set; }
        public string RampPath { get; private set; }
        public string CfgPath { get; private set; }

        private static DDCSettings _instance;
        public static DDCSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DDCSettings();
                return _instance;
            }
        }

        public void LoadConfigXml()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var ddcDir = Path.Combine(baseDir, "ddc");
            var configFile = ConfigRepository.Instance()["ddc_config"] + ".cfg";
            var rampupFile = ConfigRepository.Instance()["ddc_rampup"] + ".xml";

            PhraseLen = (int)ConfigRepository.Instance().GetDecimal("ddc_phraselength");
            RemoveSus = ConfigRepository.Instance().GetBoolean("ddc_removesustain");
            RampPath = Path.Combine(ddcDir, rampupFile);
            CfgPath = Path.Combine(ddcDir, configFile);

            if (!File.Exists(RampPath) || !File.Exists(CfgPath))
                throw new FileNotFoundException("DDC support files are missing");

            //    // -m "D:\Documents and Settings\Administrator\My Documents\Visual Studio 2010\Projects\rocksmith-custom-song-toolkit\RocksmithTookitGUI\bin\Debug\ddc\ddc_default.xml"
            //    // -c "D:\Documents and Settings\Administrator\My Documents\Visual Studio 2010\Projects\rocksmith-custom-song-toolkit\RocksmithTookitGUI\bin\Debug\ddc\ddc_default.cfg"
        }
    }

}
