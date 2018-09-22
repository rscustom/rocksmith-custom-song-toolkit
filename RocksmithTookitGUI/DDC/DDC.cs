using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using Ookii.Dialogs;
using System.Diagnostics;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.XmlRepository;
using Control = System.Windows.Forms.Control;
using ProgressBarStyle = System.Windows.Forms.ProgressBarStyle;
using PsarcPackager = RocksmithToolkitLib.PsarcLoader.PsarcPackager;

namespace RocksmithToolkitGUI.DDC
{
    public partial class DDC : UserControl
    {
        #region Constants

        private const string MESSAGEBOX_CAPTION = "Dynamic Difficulty Creator";
        private const string TKI_ARRID = "(Arrangement ID by DDC)";
        private const string TKI_REMASTER = "(Remastered by DDC)";

        #endregion

        // key => fileName w/o Ext, value => filePath
        private Dictionary<string, string> ConfigDb;
        private string DDC_Path = Path.GetTempPath();
        private Color DisabledColor = Color.Gray;
        private Color EnabledColor = Color.Black;
        private Dictionary<string, string> FilesDb;
        private Dictionary<string, string> RampUpDb;
        private BackgroundWorker bw;

        public DDC()
        {
            InitializeComponent();

            // Init fields
            FilesDb = new Dictionary<string, string>();
            RampUpDb = new Dictionary<string, string>();
            ConfigDb = new Dictionary<string, string>();

            // Setup worker
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_Completed;
            bw.WorkerReportsProgress = true;
        }

        //
        private bool IsNDD { get; set; }
        private string ProcessOutput { get; set; }

        private int ApplyPackageDD(string srcPath, int phraseLen, bool removeSus, string rampPath, string cfgPath, out string consoleOutput, bool overWrite = false, bool keepLog = false)
        {
            int result = 0; // Ends normally with no error
            DLCPackageData packageData;
            consoleOutput = String.Empty;

            try
            {
                using (var psarcOld = new PsarcPackager())
                    packageData = psarcOld.ReadPackage(srcPath);
            }
            catch (Exception ex)
            {
                consoleOutput = "Error Reading : " + srcPath + Environment.NewLine + ex.Message;
                return -1; // Read Error
            }

            // Update arrangement song info
            foreach (Arrangement arr in packageData.Arrangements)
            {
                if (chkGenArrIds.Checked)
                {
                    // generate new AggregateGraph
                    arr.SongFile = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongFile() { File = "" };

                    // generate new Arrangement IDs
                    arr.Id = IdGenerator.Guid();
                    arr.MasterId = RandomGenerator.NextInt();
                }

                // skip vocal and showlight arrangements
                if (arr.ArrangementType == ArrangementType.Vocal || arr.ArrangementType == ArrangementType.ShowLight)
                    continue;

                // validate (QC) RS2014 packageData
                if (packageData.GameVersion == GameVersion.RS2014)
                {
                    // validate existing SongInfo
                    var songXml = Song2014.LoadFromFile(arr.SongXml.File);
                    songXml.AlbumYear = packageData.SongInfo.SongYear.ToString().GetValidYear();
                    songXml.ArtistName = packageData.SongInfo.Artist.GetValidAtaSpaceName();
                    songXml.Title = packageData.SongInfo.SongDisplayName.GetValidAtaSpaceName();
                    songXml.AlbumName = packageData.SongInfo.Album.GetValidAtaSpaceName();
                    songXml.ArtistNameSort = packageData.SongInfo.ArtistSort.GetValidSortableName();
                    songXml.SongNameSort = packageData.SongInfo.SongDisplayNameSort.GetValidSortableName();
                    songXml.AlbumNameSort = packageData.SongInfo.AlbumSort.GetValidSortableName();
                    songXml.AverageTempo = Convert.ToSingle(packageData.SongInfo.AverageTempo.ToString().GetValidTempo());

                    // write updated xml arrangement
                    using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                        songXml.Serialize(stream, false);

                    // restore arrangment comments 
                    Song2014.WriteXmlComments(arr.SongXml.File, arr.XmlComments);
                }

                // apply DD to xml arrangments... 0 = Ends normally with no error
                result = DDCreator.ApplyDD(arr.SongXml.File, phraseLen, removeSus, rampPath, cfgPath, out consoleOutput, true, keepLog);
                if (result == 0) // Ends normally with no error
                { /* DO NOTHING */}
                else if (result == 1) // Ends with system error
                {
                    consoleOutput = "DDC System Error: " + Environment.NewLine +
                                    "Arrangment file: " + Path.GetFileName(arr.SongXml.File) + Environment.NewLine +
                                    "CDLC file: " + srcPath;
                    return result;
                }
                else if (result == 2) // Ends with application error
                {
                    consoleOutput = "DDC Application Error: " + Environment.NewLine +
                                    "Arrangment file: " + Path.GetFileName(arr.SongXml.File) + Environment.NewLine +
                                    "CDLC file: " + srcPath;
                    return result;
                }

                if (keepLog)
                {
                    var unpackedDir = Path.GetDirectoryName(Path.GetDirectoryName(arr.SongXml.File));
                    var logFiles = Directory.EnumerateFiles(unpackedDir, "*.log", SearchOption.AllDirectories);
                    var clogDir = Path.Combine(Path.GetDirectoryName(srcPath), "DDC_Log");
                    var plogDir = Path.Combine(clogDir, Path.GetFileNameWithoutExtension(srcPath).StripPlatformEndName().Replace("_DD", "").Replace("_NDD", ""));

                    if (!Directory.Exists(clogDir))
                        Directory.CreateDirectory(clogDir);

                    IOExtension.DeleteDirectory(plogDir);
                    Directory.CreateDirectory(plogDir);

                    foreach (var logFile in logFiles)
                        File.Copy(logFile, Path.Combine(plogDir, Path.GetFileName(logFile)));
                }

                // put arrangment comments in correct order
                Song2014.WriteXmlComments(arr.SongXml.File);
            }

            if (chkGenArrIds.Checked)
            {
                // add comment to ToolkitInfo to identify CDLC
                var arrIdComment = packageData.ToolkitInfo.PackageComment;
                if (String.IsNullOrEmpty(arrIdComment))
                    arrIdComment = TKI_ARRID;
                else if (!arrIdComment.Contains(TKI_ARRID))
                    arrIdComment = arrIdComment + " " + TKI_ARRID;

                packageData.ToolkitInfo.PackageComment = arrIdComment;
            }

            // add comment to ToolkitInfo to identify CDLC
            var remasterComment = packageData.ToolkitInfo.PackageComment;
            if (String.IsNullOrEmpty(remasterComment))
                remasterComment = TKI_REMASTER;
            else if (!remasterComment.Contains(TKI_REMASTER))
                remasterComment = remasterComment + " " + TKI_REMASTER;

            packageData.ToolkitInfo.PackageComment = remasterComment;

            // add default package version if missing
            if (String.IsNullOrEmpty(packageData.ToolkitInfo.PackageVersion))
                packageData.ToolkitInfo.PackageVersion = "1";
            else
                packageData.ToolkitInfo.PackageVersion = packageData.ToolkitInfo.PackageVersion.GetValidVersion();

            // validate packageData (important)
            packageData.Name = packageData.Name.GetValidKey(); // DLC Key

            var destPath = srcPath;
            if (!overWrite)
                destPath = GenerateDdcFilePath(srcPath);

            try
            {
                // regenerates the archive with DDC changes and repairs
                using (var psarcNew = new PsarcPackager(true))
                    psarcNew.WritePackage(destPath, packageData, srcPath);
            }
            catch (Exception ex)
            {
                consoleOutput = "Error Writing: " + srcPath + Environment.NewLine + ex.Message;
                result = -2; // Write Error
            }

            return result;
        }

        private void FillDB()
        {
            int i = 0;
            DDCfilesDgw.Rows.Clear();

            foreach (var rowFile in FilesDb)
            {
                if (DDCfilesDgw.Rows.Count <= i && i < FilesDb.Count) DDCfilesDgw.Rows.Add();
                DDCfilesDgw.Rows[i].Cells["PathColnm"].Value = rowFile.Value;
                DDCfilesDgw.Rows[i].Cells["TypeColnm"].Value = Path.GetExtension(rowFile.Value);
                i++;
            }

            DDCfilesDgw.Update();
        }

        private string GenerateDdcFilePath(string filePath)
        {
            var platform = filePath.GetPlatform();
            var ddcFilePath = Path.Combine(Path.GetDirectoryName(filePath), String.Format("{0}{1}{2}",
                Path.GetFileNameWithoutExtension(filePath).StripPlatformEndName().GetValidFileName().Replace("_DD", "").Replace("_NDD", ""),
                IsNDD ? "_NDD" : "_DD", Packer.GetPlatformEndName(platform)));

            return ddcFilePath;
        }

        private string GetConfig()
        {
            if (cmbConfigFile.Text.Trim().Length > 0)
                return String.Format("{0}", Path.GetFullPath(ConfigDb[cmbConfigFile.Text]));

            return "";
        }

        private string GetRampUpMdl()
        {
            if (cmbRampUp.Text.Trim().Length > 0)
                return String.Format("{0}", Path.GetFullPath(RampUpDb[cmbRampUp.Text]));

            return "";
        }

        private void PopCFGs()
        {
            cmbConfigFile.Items.Clear();
            ConfigDb.Clear();
            var filePaths = Directory.EnumerateFiles(DDC_Path, "*.cfg", SearchOption.AllDirectories).ToList();

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.StartsWith("user_")) fileName = fileName.Remove(0, 5);
                cmbConfigFile.Items.Add(fileName);
                cmbConfigFile.SelectedIndex = cmbConfigFile.FindStringExact("ddc_default");
                ConfigDb.Add(fileName, Path.GetFullPath(filePath));
            }

            cmbConfigFile.Refresh();
        }

        private void PopMDLs()
        {
            cmbRampUp.Items.Clear();
            RampUpDb.Clear();
            var filePaths = Directory.EnumerateFiles(DDC_Path, "*.xml", SearchOption.AllDirectories).ToList();

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.StartsWith("user_")) fileName = fileName.Remove(0, 5);
                cmbRampUp.Items.Add(fileName);
                cmbRampUp.SelectedIndex = cmbRampUp.FindStringExact("ddc_default");
                RampUpDb.Add(fileName, Path.GetFullPath(filePath));
            }

            cmbRampUp.Refresh();
        }

        private void RemoveEntry(string path)
        {
            for (int i = DDCfilesDgw.RowCount - 1; i >= 0; i--)
            {
                if (DDCfilesDgw.Rows[i].Cells["PathColnm"].Value.Equals(path))
                {
                    DDCfilesDgw.Rows.RemoveAt(i);
                    return;
                }
            }
        }

        private void SetDefaultFromConfig()
        {
            cmbRampUp.SelectedItem = ConfigRepository.Instance().GetString("ddc_rampup");
            cmbConfigFile.SelectedItem = ConfigRepository.Instance().GetString("ddc_config");
            cmbPhraseLen.Value = ConfigRepository.Instance().GetDecimal("ddc_phraselength");
            chkRemoveSustains.Checked = ConfigRepository.Instance().GetBoolean("ddc_removesustain");
        }

        private void DDC_Load(object sender, EventArgs e)
        {
            // EH fires on initial load or after any configuration changes  ... just like we want
            try
            {
                if (!this.DesignMode) // prevents tab menu VS IDE errors
                {
                    // set DDC version info
                    var ver = FileVersionInfo.GetVersionInfo(Path.Combine(ExternalApps.TOOLKIT_ROOT, ExternalApps.APP_DDC)).ProductVersion;
                    if (ddcVersion.InvokeRequired)
                        ddcVersion.Invoke(new Action(() => ddcVersion.Text = String.Format("v{0}", ver)));
                    else
                        ddcVersion.Text = String.Format("v{0}", ver);

                    // set the DDC_Path 
                    DDC_Path = Path.Combine(ExternalApps.TOOLKIT_ROOT, ExternalApps.DDC_DIR);

                    // populate comboboxes
                    PopMDLs();
                    PopCFGs();
                    SetDefaultFromConfig();
                }
            }
            catch { /*For mono compatibility*/ }
        }

        private void DDCfilesDgw_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var filePath in filePaths)
                {
                    if (!(Path.GetExtension(filePath) == ".xml" ||
                          Path.GetExtension(filePath) == ".dat" ||
                          Path.GetExtension(filePath) == ".psarc" ||
                          Path.GetExtension(filePath) == "" ||
                          Path.GetExtension(filePath) == ".edat"))
                        continue;

                    if (filePath.EndsWith("_showlights.xml") ||
                        filePath.EndsWith(".dlc.xml") ||
                        filePath.StartsWith("DDC_"))
                        continue;

                    if (!FilesDb.ContainsValue(filePath))
                        FilesDb.Add(Path.GetFileNameWithoutExtension(filePath), filePath);
                }

                FillDB();
            }
        }

        private void DDCfilesDgw_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void DDCfilesDgw_UserRemovingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (DDCfilesDgw.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure to remove the selected file?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

                string file = e.Row.Cells["PathColnm"].Value.ToString();
                string value = Path.GetFileNameWithoutExtension(file);

                if (FilesDb != null) FilesDb.Remove(value);
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
            string arg1 = "";
            var link_control = (System.Windows.Forms.LinkLabel)sender;
            var link = link_control.Text; //"https://ddcreator.wordpress.com";
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Process.Start(link);
            }
            else
            {
                string[] browsers = { "chrome", "opera", "firefox", "browser", "MicrosoftEdge" };
                foreach (var name in browsers)
                {
                    var browsera = Process.GetProcessesByName(name);
                    if (!browsera.Any())
                        continue;
                    var browser = browsera[0];
                    if (name.Contains("opera"))
                        arg1 = "-newwindow ";

                    if (name.Contains("MicrosoftEdge"))
                    {
                        Process.Start("microsoft-edge:" + link);
                        done = true;
                        break;
                    }

                    browser.StartInfo.FileName = browser.MainModule.FileName;
                    browser.StartInfo.Arguments = String.Format("{0}{1}", arg1, link);
                    browser.Start();
                    done = true;
                    break;
                }
                if (!done)
                    Process.Start(link);
            }

            this.DescriptionDDC.Links[DescriptionDDC.Links.IndexOf(e.Link)].Visited = true;
        }

        private void Highlight_CheckStateChanged(object sender, EventArgs e)
        {
            chkRemoveSustains.ForeColor = chkRemoveSustains.Checked ? EnabledColor : DisabledColor;
            chkOverwrite.ForeColor = chkOverwrite.Checked ? EnabledColor : DisabledColor;
            chkGenArrIds.ForeColor = chkGenArrIds.Checked ? EnabledColor : DisabledColor;
            chkGenLogFile.ForeColor = chkGenLogFile.Checked ? EnabledColor : DisabledColor;
        }

        private void btnAddArr_Click(object sender, EventArgs e)
        {
            using (var ofd = new VistaOpenFileDialog())
            {
                ofd.Title = "Select Package or Arrangement ...";
                ofd.Filter = "Filtered files (*.psarc;*.dat;*.edat;*.xml)|*.psarc;*.dat;*.edat;*.xml|" + "All files (*.*)|*.*";
                ofd.FilterIndex = 0;
                ofd.Multiselect = true;
                ofd.ReadOnlyChecked = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string[] filePaths = ofd.FileNames;

                foreach (var filePath in filePaths)
                {
                    if (filePath.EndsWith("_showlights.xml") ||
                        filePath.EndsWith(".dlc.xml") ||
                        filePath.StartsWith("DDC_"))
                        continue;

                    if (!FilesDb.ContainsValue(filePath))
                        FilesDb.Add(Path.GetFileNameWithoutExtension(filePath), filePath);
                }
            }

            FillDB();
        }

        private void btnConfigFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new VistaOpenFileDialog())
            {
                ofd.Filter = "DDC Config file (*.cfg)|*.cfg";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = true;
                ofd.ReadOnlyChecked = true;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string[] filePaths = ofd.FileNames;

                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    Directory.CreateDirectory(@".\ddc\ucfg\");
                    var path = String.Format(@".\ddc\ucfg\user_{0}.cfg", fileName);
                    if (!cmbConfigFile.Items.Contains(fileName))
                    {
                        try { File.Copy(filePath, path, true); }
                        catch { }
                        cmbConfigFile.Items.Add(fileName);
                    }
                    cmbConfigFile.SelectedIndex = cmbConfigFile.FindStringExact(fileName);
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

            if (!this.bw.IsBusy && FilesDb.Count > 0)
            {
                pbUpdateProgress.Style = ProgressBarStyle.Marquee;
                pbUpdateProgress.MarqueeAnimationSpeed = 60;
                pbUpdateProgress.Visible = true;
                lblCurrentOperation.Text = (IsNDD) ? "Removing DD Content ..." : "Generating DD Content ...";
                lblCurrentOperation.Visible = true;
                lblStatus.Visible = true;
                this.Refresh();
                btnGenerate.Enabled = false;
                this.bw.RunWorkerAsync();
            }
        }

        private void btnRampUp_Click(object sender, EventArgs e)
        {
            using (var ofd = new VistaOpenFileDialog())
            {
                ofd.Filter = "DDC Ramp-Up model (*.xml)|*.xml";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = true;
                ofd.ReadOnlyChecked = true;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string[] filePaths = ofd.FileNames;

                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    Directory.CreateDirectory(@".\ddc\umdls\");
                    var path = String.Format(@".\ddc\umdls\user_{0}.xml", fileName);
                    if (!cmbRampUp.Items.Contains(fileName))
                    {
                        try
                        {
                            File.Copy(filePath, path, true);
                            cmbRampUp.Items.Add(fileName);
                        }
                        catch { }
                    }

                    cmbRampUp.SelectedIndex = cmbRampUp.FindStringExact(fileName);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (DDCfilesDgw.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure to remove the selected file?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

                foreach (DataGridViewRow row in DDCfilesDgw.SelectedRows)
                {
                    var filePath = row.Cells["PathColnm"].Value.ToString();
                    var fileName = Path.GetFileNameWithoutExtension(filePath);

                    if (FilesDb != null) FilesDb.Remove(fileName);
                }

                FillDB();
            }
        }

        private void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            var debugMe = e.Result;
            // file overwriting is done here as last step
            pbUpdateProgress.Value = 100;

            foreach (var file in FilesDb)
            {
                switch (Path.GetExtension(file.Value))
                {
                    case ".xml": // Arrangement
                        {
                            var fileDir = Path.GetDirectoryName(file.Value);
                            var ddcArrXML = Path.Combine(fileDir, String.Format("DDC_{0}.xml", file.Key));
                            var srcShowlights = Path.Combine(fileDir, String.Format("{0}_showlights.xml", file.Key));
                            var destShowlights = Path.Combine(fileDir, String.Format("DDC_{0}_showlights.xml", file.Key));

                            if (!chkOverwrite.Checked && !File.Exists(destShowlights) && File.Exists(srcShowlights) && File.Exists(ddcArrXML))
                                File.Copy(srcShowlights, destShowlights, true);
                        }
                        break;

                    case ".psarc": // PC / Mac (RS2014)
                    case ".dat":   // PC (RS1)
                    case ".edat":  // PS3
                    case "":       // XBox 360
                        if (chkOverwrite.Checked)
                        {
                            var filePath = file.Value;
                            var ddcFilePath = GenerateDdcFilePath(filePath);
                            if (!ddcFilePath.Equals(filePath))
                            {
                                // File.Move is prone to exceptions
                                File.Copy(ddcFilePath, filePath, true);
                                File.Delete(ddcFilePath);
                            }
                        }
                        break;
                }

                Invoke(new MethodInvoker(() => RemoveEntry(file.Value)));
            }

            FilesDb.Clear();

            if (e.Result.Equals(0))
                MessageBox.Show(String.Format("Dynamic difficulty {0} sucessfully.", IsNDD ? "removed" : "generated"), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(String.Format("Dynamic difficulty {0} with errors: [" + e.Result + "]" + Environment.NewLine + "See DDC and Toolkit logs for details.", IsNDD ? "removed" : "generated"), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);

            btnGenerate.Enabled = true;
            pbUpdateProgress.Visible = false;
            pbUpdateProgress.MarqueeAnimationSpeed = 0;
            pbUpdateProgress.Style = ProgressBarStyle.Continuous;
            lblCurrentOperation.Visible = false;
            lblStatus.Visible = false;
            this.Focus();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessOutput = String.Empty;
            var rampPath = String.Empty;
            var cfgPath = String.Empty;

            this.Invoke(new MethodInvoker(() =>
                {
                    rampPath = GetRampUpMdl();
                    cfgPath = GetConfig();
                }));

            var errorsFound = new StringBuilder();
            var totalCount = FilesDb.Count;
            var currentCount = 0;
            var errorCount = 0;
            // TODO: change progress reporting method so it is responsive for a single file
            var step = (int)Math.Round(1.0 / FilesDb.Count * 100, 0);
            var progress = 0;
            bw.ReportProgress(progress);

            foreach (var file in FilesDb)
            {
                var consoleOutput = String.Empty;
                currentCount++;
                int count = currentCount;
                GeneralExtensions.InvokeIfRequired(lblStatus, delegate
                    {
                        lblStatus.Text = String.Format("Processing file {0} of {1} ... Please wait.", count, totalCount);
                    });

                switch (Path.GetExtension(file.Value))
                {
                    case ".xml":   // Arrangement
                        DDCreator.ApplyDD(file.Value, (int)cmbPhraseLen.Value, chkRemoveSustains.Checked, rampPath, cfgPath, out consoleOutput, chkOverwrite.Checked, chkGenLogFile.Checked);
                        break;
                    case ".psarc": // PC / Mac (RS2014)
                    case ".dat":   // PC (RS1)
                    case ".edat":  // PS3
                    case "":       // XBox 360
                        ApplyPackageDD(file.Value, (int)cmbPhraseLen.Value, chkRemoveSustains.Checked, rampPath, cfgPath, out consoleOutput, chkOverwrite.Checked, chkGenLogFile.Checked);
                        break;
                }

                if (!String.IsNullOrEmpty(consoleOutput))
                {
                    errorsFound.AppendLine(consoleOutput);
                    errorCount++;
                }

                progress += step;
                bw.ReportProgress(progress);
            }

            if (!String.IsNullOrEmpty(errorsFound.ToString()))
                ProcessOutput = errorsFound.ToString();

            GeneralExtensions.InvokeIfRequired(lblStatus, delegate
                {
                    lblStatus.Text = String.Format("Sucessfully processed {0} of {1} files ...", totalCount - errorCount, totalCount);
                });

            e.Result = errorCount; // No Errors = 0
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbUpdateProgress.Value = e.ProgressPercentage;
        }

        private void cmbConfigFiles_DropDown(object sender, EventArgs e)
        {
            PopCFGs();
        }

        private void cmbRamUp_DropDown(object sender, EventArgs e)
        {
            PopMDLs();
        }

        private void cmbRampUp_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsNDD = ((ComboBox)sender).Text.Equals("ddc_dd_remover");
            btnGenerate.Text = (IsNDD) ? "Remove DD" : "Generate DD";
        }
    }
}

