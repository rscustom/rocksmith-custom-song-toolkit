using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.CGM
{
    /*
    Cozy1's custom code to create/delete/read/write/modify INI files
    */

    internal class clsINI // Read Write INI Files
    {
        private const string MESSAGEBOX_CAPTION = "Class INI of CGM";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
                                                             string key,
                                                             string val,
                                                             string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                                                          string key, string def,
                                                          StringBuilder retVal,
                                                          int size,
                                                          string filePath);

        // Write Data to the INI File
        public bool IniWrite(string iniFile, string Section, string Key,
                             string Value)
        {
            long lRet = WritePrivateProfileString(Section, Key, Value, iniFile);
            if (lRet > 0) return true;

            MessageBox.Show("There is a problem writing an INI file value.",
                            MESSAGEBOX_CAPTION, MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            return false;
        }

        // Read Data Value From the INI File
        public string IniRead(string iniFile, string Section, string Key)
        {
            StringBuilder Value = new StringBuilder(255);
            int sRet = GetPrivateProfileString(Section, Key, "", Value, 255,
                                               iniFile);

            if (sRet > 0) return Value.ToString();

            return "ERROR";
        }

        // Create an INI File
        public bool IniCreate(string iniFile, bool verBose)
        {
            if (File.Exists(iniFile))
            {
                if (verBose)
                {
                    if (
                            MessageBox.Show(
                                    "The INI file " + iniFile +
                                    " already exists." + Environment.NewLine +
                                    "Do you want to overwrite it?",
                                    MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information) ==
                            DialogResult.No) return false;
                }
            }
            File.Create(iniFile).Close();
            return true;
        }

        // Delete an INI File
        public bool IniDelete(string iniFile, bool verBose)
        {
            if (File.Exists(iniFile))
            {
                if (verBose)
                {
                    if (
                            MessageBox.Show(
                                    "The INI file " + iniFile + " exists." +
                                    Environment.NewLine +
                                    "Do you really want to delete it?",
                                    MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information) ==
                            DialogResult.No)
                    {
                        return false;
                    }
                }
                File.Delete(iniFile);
            }
            return true;
        }
    }
}