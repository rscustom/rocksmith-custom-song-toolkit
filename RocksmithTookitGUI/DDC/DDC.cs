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
        internal BackgroundWorker bw = new BackgroundWorker();
        // 0 - fpath 1 - name
        internal Dictionary<string, string> DLCdb = new Dictionary<string,string>();
        internal Dictionary<string, string> RampMdlsDb = new Dictionary<string,string>();
        internal static string AppWD = Application.StartupPath;

        internal bool samefolder { get; set; }
        internal string newWD { get; set; }
        internal string oldWD { get; set; }

        public DDC()
        {
            InitializeComponent();
            this.bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            this.bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Completed);
            this.bw.WorkerReportsProgress = true;
        }

        private void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            DDprogress.Value = 100;
            if (e.Result.Equals(0))
            {
                string newPath = String.Empty;
                this.Invoke(new MethodInvoker(() =>
                { newPath = newWD; }));
                    foreach (var file in DLCdb)
                    {
                        if (newPath == String.Empty) newPath = Path.GetDirectoryName(file.Value);
                        string ddcArrXML = "DDC_" + file.Key+".xml", /*DDC_ArrangementName.xml*/
                        oldPath = Path.GetDirectoryName(file.Value),
                        srcDDCArrXML = Path.Combine(oldPath, ddcArrXML), /*<oldPath>\DDC_ArrName.xml*/
                        destDDCArrXML = Path.Combine(newPath, ddcArrXML), /*<newPath>\DDC_ArrName.xml*/
                        arrDDClog = Path.ChangeExtension(srcDDCArrXML, ".log"), /*<oldPath>\DDC_ArrName.log*/
                        srcLog = oldPath + "\\ddc.log",
                        destLog = newPath + "\\ddc.log",
                        srcShowlights = Path.Combine(oldPath, file.Key + "_showlights.xml"),
                        destShowlights = Path.Combine(newPath, "DDC_" + file.Key + "_showlights.xml");

                        samefolder = oldPath.Equals(newPath) ? true : false;
                        if (!samefolder && File.Exists(srcDDCArrXML))
                        {
                            if (File.Exists(destDDCArrXML))
                            { File.Delete(destDDCArrXML); File.Delete(destDDCArrXML.Replace(".xml", ".log")); File.Delete(destLog);
                            File.Delete(destShowlights); }
                            File.Move(srcDDCArrXML, destDDCArrXML); File.Move(arrDDClog, destDDCArrXML.Replace(".xml", ".log"));
                            File.Move(srcLog, destLog);
                        }
                        if (!File.Exists(destShowlights) && File.Exists(srcShowlights)) File.Copy(srcShowlights, destShowlights);
                        Invoke(new MethodInvoker(() => { DelEntry(file.Value); }));
                    }
                DLCdb.Clear();
                MessageBox.Show("DD generated!");
            }
            else if (e.Result.Equals(1))
                MessageBox.Show("DD generation error! System Error.");
            else MessageBox.Show("DD generation error! DDC error, see ddc.log");

            ProduceDDbt.Enabled = true;
            DDprogress.Visible = false;
            this.Focus();
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DDprogress.Value = e.ProgressPercentage;
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            bw.ReportProgress(0);
            int result = -1;
            int i = 0;
            int progress = 0;
            foreach (var file in DLCdb)
            {
                i++;
                progress = (int)Math.Round(1.0 / DLCdb.Count() * 100,0);
                string remSUS = String.Empty;
                string rampPath = String.Empty;
                this.Invoke(new MethodInvoker(() =>
                {
                    remSUS = IsREMsus();
                    rampPath = GetRampUpMdl();
                }));
                using (var DDC = new Process())
                {
                    DDC.StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = Path.GetDirectoryName(file.Value),
                        FileName = AppWD + @"\ddc\ddc.exe",
                        Arguments = String.Format("\"{0}\" -l {1} -s {2}{3}", Path.GetFileName(file.Value), 
                        (UInt16)phaseLenNum.Value, remSUS, rampPath),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    DDC.Start();
                    DDC.WaitForExit(30000);

                    result = DDC.ExitCode;
                }
                bw.ReportProgress(progress);
            }
            e.Result = result;
        }

        internal void FillDB()
        {
            int i = 0;
            foreach (var rowFile in DLCdb)
            {
                if (DDCfilesDgw.Rows.Count <= i && i < DLCdb.Count) DDCfilesDgw.Rows.Add();
                DDCfilesDgw.Rows[i].Cells["PathColnm"].Value = rowFile.Value;
                DDCfilesDgw.Rows[i].Cells["TypeColnm"].Value = Path.GetExtension(rowFile.Value);
                i++;
            }
            DDCfilesDgw.Update();
        }
        private string GetRampUpMdl()
        {
            if (ramUpMdlsCbox.Text.Trim().Length > 0)
                return String.Format(" -m \"{0}\"", Path.GetFullPath(RampMdlsDb[ramUpMdlsCbox.Text]));
            else return "";
        }
        private string IsREMsus()
        {
            if (delsustainsBT.Checked)
                return "Y";
            else return "N";
        }

        private void ProduceDDbt_Click(object sender, EventArgs e)
        {
            if (!this.bw.IsBusy)
            {
                if(DLCdb.Count > 1) DDprogress.Visible = true;
                ProduceDDbt.Enabled = false;
                this.bw.RunWorkerAsync(); 
            }
        }
        private void AddArrBT_Click(object sender, EventArgs e)
        {
            using (var ofd = new VistaOpenFileDialog())
            using (var sfd = new FolderBrowserDialog())
            {
                ofd.Filter = "Select DLC or Arrangement|*.psarc;*.dat;*.xml;*.edat|" + "All files|*.*";
                ofd.FilterIndex = 0;
                ofd.Multiselect = true;
                ofd.ReadOnlyChecked = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                foreach (var i in ofd.FileNames)
                {
                    if (i.EndsWith("_showlights.xml")
                        || i.EndsWith(".dlc.xml")
                        || i.IndexOf("DDC_")>0) continue;
                    if (!DLCdb.ContainsValue(i))
                        DLCdb.Add(Path.GetFileNameWithoutExtension(i), i);
                }

                if (DestPathCbx.Checked)
                {

                    {
                        if (sfd.ShowDialog() != DialogResult.OK)
                            return;
                        newWD = sfd.SelectedPath;
                    }
                }
            }
            newWD = String.Empty;
            FillDB();
        }
        private void rampUpBT_Click(object sender, EventArgs e)
        {
            using (var ofd = new VistaOpenFileDialog())
            {
                ofd.Filter = "DDC Ramp-Up model|*.xml";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = true;
                ofd.ReadOnlyChecked = true;
                ofd.RestoreDirectory = true;
                
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                foreach (var i in ofd.FileNames)
                {                    
                    var name = Path.GetFileNameWithoutExtension(i);
                    Directory.CreateDirectory(@".\ddc\umdls\");
                    var path = @".\ddc\umdls\user_" + name + ".xml";
                    if (!ramUpMdlsCbox.Items.Contains(name))
                    {
                        try { File.Copy(i, path, true); }
                        catch { }
                        ramUpMdlsCbox.Items.Add(name);
                    }
                    ramUpMdlsCbox.SelectedIndex = ramUpMdlsCbox.FindStringExact(name);
                }
            }
        }
        /// <summary>
        /// Optimized for Opera, Google Chrome and Firefox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescriptionDDC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool done = false;
            string link = "http://ddcreator.wordpress.com";
            string arg0 = "";
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
                        if(browserID.IndexOf("opera") >0) arg0 = "-newwindow ";

                        browser.StartInfo.FileName = browser.MainModule.FileName;
                        browser.StartInfo.Arguments = String.Format("{0}{1}", arg0, link);
                        browser.Start();
                        done = true;
                        break;
                    }
                }
                if (done) break;
            }
            if (!done) System.Diagnostics.Process.Start(link);
            this.DescriptionDDC.Links[DescriptionDDC.Links.IndexOf(e.Link)].Visited = true;
        }
        private void DDC_Load(object sender, EventArgs e)
        {
            PopMDLs();
        }
        private void PopMDLs()
        {
            if (Directory.Exists(@".\ddc\"))
            {
                ramUpMdlsCbox.Items.Clear();
                RampMdlsDb.Clear();
                foreach (var mdl in Directory.EnumerateFiles(@".\ddc\", "*.xml", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(mdl);
                    if (name.StartsWith("user_")) name = name.Substring(5, name.Length - 5);
                    ramUpMdlsCbox.Items.Add(name);
                    ramUpMdlsCbox.SelectedIndex = ramUpMdlsCbox.FindStringExact("ddc_default");
                    RampMdlsDb.Add(name, Path.GetFullPath(mdl));
                }
                ramUpMdlsCbox.Refresh();
            }
        }
        private void DelEntry(string path)
        {
            for (int i = DDCfilesDgw.RowCount - 1; i >= 0; i--)
            {
                if (DDCfilesDgw.Rows[i].Cells["PathColnm"].Value.Equals(path))
                { DDCfilesDgw.Rows.RemoveAt(i); return; }
            }
        }
        private void DDCfilesDgw_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string i = String.Empty;
            try
            {
                i = e.Row.Cells["PathColnm"].Value.ToString();
            }
            catch { }
            string value = Path.GetFileNameWithoutExtension(i);
            if (DLCdb != null)
                DLCdb.Remove(value);
        }

        private void ramUpMdlsCbox_DropDown(object sender, EventArgs e)
        {
            PopMDLs();
        }
    }
}