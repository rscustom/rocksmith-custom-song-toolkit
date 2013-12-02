using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ookii.Dialogs;
using System.Diagnostics;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI.DDC
{
    public partial class DDC : UserControl
    {
        internal static string AppWD = Directory.GetCurrentDirectory();
        internal string ArrangementPath { get; set; }
        internal string rampupPath { get; set; }
        internal bool samefolder { get; set; }
        internal string newWD { get; set; }

        public DDC()
        {
            InitializeComponent();
            DescriptionDDC.TextAlign = ContentAlignment.MiddleCenter;
            DescriptionDDC.Text = "This is DDC - Dynamic Difficulty Creator by Chlipouni\r\n\r\n"+
                "If you are unfamiliar with it, please, read this help: http://ddcreator.wordpress.com \r\n";
            DescriptionDDC.LinkArea = new LinkArea(112, 31);
        }

        private void ProduceDDbt_Click(object sender, EventArgs e)
        {
            string msgtext;
            string ddcXML = "DDC_" + Path.GetFileName(ArrFilePathTB.Text);
            string sourceDDC = Path.Combine(Path.GetDirectoryName(ArrFilePathTB.Text), ddcXML);
            string destDDC = Path.Combine(newWD, ddcXML);
            string arrDDClog = Path.ChangeExtension(sourceDDC, ".log");
            string destLog = Path.GetDirectoryName(destDDC) + "\\ddc.log";
            string ddcLog = Path.GetDirectoryName(sourceDDC) + "\\ddc.log";
            if (newWD.Equals(Path.GetDirectoryName(ArrFilePathTB.Text))) samefolder = true;
            switch (GenerateDD())
            {
                case "0":
                    msgtext = "DD created succsesfully!";
                    if (File.Exists(sourceDDC)&(!samefolder)) { File.Delete(destDDC); File.Delete(destDDC.Replace(".xml", ".log")); File.Delete(destLog); }
                    if (!samefolder)
                    {
                        File.Move(sourceDDC, destDDC); File.Move(arrDDClog, destDDC.Replace(".xml", ".log"));
                        File.Move(ddcLog, destLog);
                    }
                    break;
                case "1":
                    msgtext = "System Error, check access rights."; break;
                case "2":
                    msgtext = "DDC error, see DDC.log"; break;
                default:
                    msgtext = "Unknown Error"; break;
            }
            MessageBox.Show(msgtext, "DDC generator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateDD()
        {
            using (var DDC = new Process())
            {
                DDC.StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(ArrFilePathTB.Text),
                    FileName = AppWD + @"\ddc\ddc.exe",
                    Arguments = String.Format("\"{0}\" -l {1} -s {2}{3}", Path.GetFileName(ArrFilePathTB.Text), (UInt16)phaseLenNum.Value, REMsus(), GetRampUpMdl()),
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    //WindowStyle = ProcessWindowStyle.Minimized,
                    //RedirectStandardOutput = true
                };

                DDC.Start();
                //Application.DoEvents();
                DDC.WaitForExit();

                return DDC.ExitCode.ToString();
            }
        }
        private string GetRampUpMdl()
        {
            if (remChordsCB.Checked)
                return String.Format(" -m {0}", Path.GetFullPath(AppWD + @"\ddc\ddc_chords_remover.xml"));
            if (RampUPcbbx.Text.Trim().Length > 0)
                return String.Format(" -m {0}", Path.GetFullPath(RampUPcbbx.Text));
            else return "";
        }

        private string REMsus()
        {
            if (delsustainsBT.Checked)
                return "Y";
            else return "N";
        }

        private void remChordsCB_CheckedChanged(object sender, EventArgs e)
        {
            if (remChordsCB.Checked) {
                rampUpBT.Enabled = false;
                RampUPcbbx.Enabled = false;
            }
            else {
                rampUpBT.Enabled = true;
                RampUPcbbx.Enabled = true;
            }
        }

        private void AddArrBT_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            using (var sfd = new VistaFolderBrowserDialog())
            {
                ofd.Filter = "Select RS arrangement|*.xml";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                ArrangementPath = ofd.FileName;
                sfd.SelectedPath = Path.GetDirectoryName(ArrangementPath)+"\\";
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                newWD = sfd.SelectedPath;
            }
            ArrFilePathTB.Text = ArrangementPath;
        }

        private void rampUpBT_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                rampupPath = ofd.FileName;
            }
            RampUPcbbx.Text = rampupPath;
        }

        private void DescriptionDDC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool done = false;
            Process[] processlist = Process.GetProcesses();
            foreach (Process browser in processlist)
            {                
                string[] Browsers = new string[]{
                    "chrome", "opera", "firefox"
                };
                foreach (var browserID in Browsers)
                {
                    if (browser.ProcessName.Equals(browserID))
                    {
                        browser.StartInfo.FileName = browser.MainModule.FileName;
                        browser.StartInfo.Arguments = "http://ddcreator.wordpress.com";
                        browser.Start();
                        done = true;
                        break;
                    }
                }
                if (done) break;
            }
            if (!done) System.Diagnostics.Process.Start("cmd", "/c start http://ddcreator.wordpress.com");
            this.DescriptionDDC.Links[DescriptionDDC.Links.IndexOf(e.Link)].Visited = true;
        }
    }
}
