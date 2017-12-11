using System;
using System.IO;
using System.Linq;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitGUI.DDC
{
    [Obsolete("Depricated, please use RocksmithToolkitLib.DDCSettings.", true)]
    public class SettingsDDC
    {
        public int PhraseLen { get; private set; }
        public bool RemoveSus { get; private set; }
        public string RampPath { get; private set; }
        public string CfgPath { get; private set; }

        // not used for now
        // public bool CleanProcess { get; set; }
        // public bool KeepLog { get; set; }

        private static SettingsDDC _instance;
        public static SettingsDDC Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsDDC();
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