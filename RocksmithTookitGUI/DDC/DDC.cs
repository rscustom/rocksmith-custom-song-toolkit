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
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.XmlRepository;
using ProgressBarStyle = System.Windows.Forms.ProgressBarStyle;
using PsarcPackager = RocksmithToolkitLib.PsarcLoader.PsarcPackager;

namespace RocksmithToolkitGUI.DDC
{
    public partial class DDC : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "Dynamic Difficulty Creator";
        private const string ARRID_BY_DDC = "(Arrangement Identifcation revised by DDC)";

        internal BackgroundWorker bw;
        // 0 - fpath 1 - name
        internal Dictionary<string, string> DLCdb;
        internal Dictionary<string, string> RampMdlsDb;
        internal Dictionary<string, string> ConfigsDb;
        internal static string AppWD = AppDomain.CurrentDomain.BaseDirectory;
        internal static string DdcBD = Path.Combine(AppWD, "ddc");
        internal Color EnabledColor = Color.Black;
        internal Color DisabledColor = Color.Gray;

        internal bool isNDD { get; set; }

        internal bool CleanProcess
        {
            get
            {
                return chkOverwrite.Checked;
            }
            set
            {
                chkOverwrite.Checked = value;
            }
        }

        public bool KeepLog
        {
            get
            {
                return chkGenLogFile.Checked;
            }
            set
            {
                chkGenLogFile.Checked = value;
            }
        }

        internal string processOutput { get; set; }

        public DDC()
        {
            InitializeComponent();
            // Init fields
            bw = new BackgroundWorker();
            DLCdb = new Dictionary<string, string>();
            RampMdlsDb = new Dictionary<string, string>();
            ConfigsDb = new Dictionary<string, string>();
            // Setup worker
            this.bw.DoWork += bw_DoWork;
            this.bw.ProgressChanged += bw_ProgressChanged;
            this.bw.RunWorkerCompleted += bw_Completed;
            this.bw.WorkerReportsProgress = true;
        }

        private void DDC_Load(object sender, EventArgs e)
        {
            try
            {
                string ddcPath = Path.Combine(AppWD, "ddc", "ddc.exe");
                if (!this.DesignMode && File.Exists(ddcPath))
                {
                    var vi = FileVersionInfo.GetVersionInfo(ddcPath).ProductVersion;
                    ddcVersion.Text = String.Format("v{0}", vi);
                }
                PopMDLs();
                PopCFGs();
                SetDefaultFromConfig();
            }
            catch { /*For mono compatibility*/ }
        }

        private void SetDefaultFromConfig()
        {
            ramUpMdlsCbox.SelectedItem = ConfigRepository.Instance()["ddc_rampup"];
            ConfigFilesCbx.SelectedItem = ConfigRepository.Instance()["ddc_config"];
            phaseLenNum.Value = ConfigRepository.Instance().GetDecimal("ddc_phraselength");
            chkRemoveSustains.Checked = ConfigRepository.Instance().GetBoolean("ddc_removesustain");
        }

        private void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            pbUpdateProgress.Value = 100;
            if (e.Result.Equals(0))
            {
                foreach (var file in DLCdb)
                {
                    switch (Path.GetExtension(file.Value))
                    {
                        case ".xml": // Arrangement
                            {
                                string filePath = Path.GetDirectoryName(file.Value),
                                ddcArrXML = Path.Combine(filePath, String.Format("DDC_{0}.xml", file.Key)),
                                srcShowlights = Path.Combine(filePath, String.Format("{0}_showlights.xml", file.Key)),
                                destShowlights = Path.Combine(filePath, String.Format("DDC_{0}_showlights.xml", file.Key));

                                if (!CleanProcess && !File.Exists(destShowlights) && File.Exists(srcShowlights) && File.Exists(ddcArrXML))
                                    File.Copy(srcShowlights, destShowlights, true);
                            }
                            break;
                        case ".psarc": // PC / Mac (RS2014)
                        case ".dat":   // PC (RS1)
                        case ".edat":  // PS3
                        case "":       // XBox 360
                            {
                                string filePath = file.Value,
                                newName = String.Format("{0}_{1}{2}",
                                file.Key.StripPlatformEndName().GetValidFileName().Replace("_DD", "").Replace("_NDD", ""), isNDD ? "_NDD" : "DD", filePath.GetPlatform().GetPathName()[2]);

                                if (CleanProcess && File.Exists(filePath) && !Path.GetFileNameWithoutExtension(filePath).GetValidFileName().Equals(newName))
                                    File.Delete(filePath);
                            }
                            break;
                    }

                    Invoke(new MethodInvoker(() => DelEntry(file.Value)));
                }

                DLCdb.Clear();
                MessageBox.Show(String.Format("Dynamic difficulty {0}!", isNDD ? "removed" : "generated"), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Result.Equals(1))
                MessageBox.Show("DDC error! System Error. See below: " + Environment.NewLine + processOutput, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (e.Result.Equals(2))
            {
                MessageBox.Show(String.Format("Dynamic difficulty {0} with errors! See below:{1}{2}", Environment.NewLine, isNDD ? "removed" : "generated", processOutput), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("DDC error! See ddc.log", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);

            ProduceDDbt.Enabled = true;
            pbUpdateProgress.Visible = false;
            pbUpdateProgress.MarqueeAnimationSpeed = 0;
            pbUpdateProgress.Style = ProgressBarStyle.Continuous;
            lblCurrentOperation.Visible = false;
            this.Focus();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbUpdateProgress.Value = e.ProgressPercentage;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            processOutput = String.Empty;
            // TODO: change progress reporting method so it is responsive for a single file
            var step = (int)Math.Round(1.0 / DLCdb.Count * 100, 0);
            int result = -1, progress = 0;
            string remSUS = String.Empty, rampPath = String.Empty, cfgPath = String.Empty;

            this.Invoke(new MethodInvoker(() =>
            {
                remSUS = IsREMsus();
                rampPath = GetRampUpMdl();
                cfgPath = GetConfig();
            }));

            bw.ReportProgress(progress);

            StringBuilder errorsFound = new StringBuilder();
            foreach (var file in DLCdb)
            {
                string consoleOutput = String.Empty;
                switch (Path.GetExtension(file.Value))
                {
                    case ".xml":   // Arrangement
                        result = ApplyDD(file.Value, remSUS, rampPath, cfgPath, out consoleOutput, CleanProcess, KeepLog);
                        errorsFound.AppendLine(consoleOutput);
                        break;
                    case ".psarc": // PC / Mac (RS2014)
                    case ".dat":   // PC (RS1)
                    case ".edat":  // PS3
                    case "":       // XBox 360
                        result = ApplyPackageDD(file.Value, remSUS, rampPath, cfgPath, out consoleOutput, KeepLog);
                        errorsFound.AppendLine(consoleOutput);
                        break;
                }
                if (!String.IsNullOrEmpty(errorsFound.ToString()))
                {
                    processOutput = errorsFound.ToString();
                }

                progress += step;
                bw.ReportProgress(progress);
            }

            e.Result = result;
        }

        private int ApplyDD(string file, string remSUS, string rampPath, string cfgPath, out string consoleOutput, bool cleanProcess = false, bool keepLog = false)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppWD, "ddc", "ddc.exe"),
                WorkingDirectory = Path.GetDirectoryName(file),
                Arguments = String.Format("\"{0}\" -l {1} -s {2}{3}{4}{5}{6}",
                    Path.GetFileName(file),
                    (UInt16)phaseLenNum.Value,
                    remSUS, rampPath, cfgPath,
                    cleanProcess ? " -p Y" : " -p N",
                    keepLog ? " -t Y" : " -t N"
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
                DDC.WaitForExit(1000 * 60 * 15); //wait 15 minutes
                return DDC.ExitCode;
            }
        }

        private int ApplyPackageDD(string filePath, string remSUS, string rampPath, string cfgPath, out string consoleOutputPkg, bool keepLog = false)
        {
            int singleResult = -1;
            consoleOutputPkg = String.Empty;
            var platform = filePath.GetPlatform();

            var newFilePath = Path.Combine(Path.GetDirectoryName(filePath), String.Format("{0}_{1}{2}",
                Path.GetFileNameWithoutExtension(filePath).StripPlatformEndName().GetValidFileName().Replace("_DD", "").Replace("_NDD", ""),
                isNDD ? "NDD" : "DD", platform.GetPathName()[2]));
            newFilePath = String.Format("{0}{1}", newFilePath, Path.GetExtension(filePath));

            DLCPackageData packageData;
            using (var psarcOld = new PsarcPackager())
                packageData = psarcOld.ReadPackage(filePath);

            // Update arrangement song info
            foreach (Arrangement arr in packageData.Arrangements)
            {
                // skip vocal and showlight arrangements
                if (arr.ArrangementType == ArrangementType.Vocal || arr.ArrangementType == ArrangementType.ShowLight)
                    continue;
                // apply DD to xml arrangments
                singleResult = ApplyDD(arr.SongXml.File, remSUS, rampPath, cfgPath, out consoleOutputPkg, true, keepLog);
                if (singleResult == 1)
                {
                    var errMsg = "DDC generated an error while processing arrangement:" + Environment.NewLine +
                        arr.SongXml.File + Environment.NewLine +
                        "for CDLC file: " + filePath + Environment.NewLine;

                    BetterDialog2.ShowDialog(errMsg, "DDC Generated Error", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                    return singleResult;
                }

                if (singleResult == 2)
                    consoleOutputPkg = String.Format("Arrangement file '{0}' => {1}", Path.GetFileNameWithoutExtension(arr.SongXml.File), consoleOutputPkg);

                var unpackedDir = Path.GetDirectoryName(Path.GetDirectoryName(arr.SongXml.File));
                var logFiles = Directory.EnumerateFiles(unpackedDir, "*.log", SearchOption.AllDirectories);

                if (keepLog)
                {
                    string clogDir = Path.Combine(Path.GetDirectoryName(newFilePath), "DDC_Log");
                    string plogDir = Path.Combine(clogDir, Path.GetFileNameWithoutExtension(newFilePath).StripPlatformEndName().Replace("_DD", "").Replace("_NDD", ""));

                    if (!Directory.Exists(clogDir)) Directory.CreateDirectory(clogDir);
                    DirectoryExtension.SafeDelete(plogDir);
                    Directory.CreateDirectory(plogDir);
                    foreach (var logFile in logFiles)
                        File.Copy(logFile, Path.Combine(plogDir, Path.GetFileName(logFile)));
                }

                if (chkGenArrIds.Checked)
                {
                    // generate new AggregateGraph
                    arr.SongFile = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongFile() { File = "" };

                    // generate new Arrangement IDs
                    arr.Id = IdGenerator.Guid();
                    arr.MasterId = RandomGenerator.NextInt();

                    // preserve existing xml comments
                    arr.XmlComments = Song2014.ReadXmlComments(arr.SongXml.File);
                    var isCommented = false;
                    var commentNodes = arr.XmlComments as List<XComment> ?? arr.XmlComments.ToList();
                    foreach (var commentNode in commentNodes)
                    {
                        if (commentNode.ToString().Contains(ARRID_BY_DDC))
                            isCommented = true;
                    }

                    // add comment to saved xml        
                    if (!isCommented)
                        Song2014.WriteXmlComments(arr.SongXml.File, commentNodes, true, ARRID_BY_DDC);

                    // add comment to ToolkitInfo to identify CDLC revised by DDC
                    var packageComment = packageData.PackageComment;

                    if (String.IsNullOrEmpty(packageComment))
                        packageComment = ARRID_BY_DDC;

                    if (!packageComment.Contains(ARRID_BY_DDC))
                        packageComment = packageComment + " " + ARRID_BY_DDC;

                    packageData.PackageComment = packageComment;
                }

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

                // resave the validated xml
                File.Delete(arr.SongXml.File);
                using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                    songXml.Serialize(stream, true);
            }

            // validate packageData (important)
            packageData.Name = packageData.Name.GetValidKey(); // DLC Key                 

            if (String.IsNullOrEmpty(packageData.PackageVersion))
                packageData.PackageVersion = "1";
            else
                packageData.PackageVersion = packageData.PackageVersion.GetValidVersion();

            Console.WriteLine(@" - Repackaging updated DDC content ...");

            // regenerates the SNG with the repair and repackages               
            using (var psarcNew = new PsarcPackager(true))
                psarcNew.WritePackage(newFilePath, packageData, filePath);

            return singleResult;
        }

        internal void FillDB()
        {
            int i = 0;
            DDCfilesDgw.Rows.Clear();
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
            else
                return "";
        }

        private string GetConfig()
        {
            if (ConfigFilesCbx.Text.Trim().Length > 0)
                return String.Format(" -c \"{0}\"", Path.GetFullPath(ConfigsDb[ConfigFilesCbx.Text]));
            else
                return "";
        }

        private string IsREMsus()
        {
            if (chkRemoveSustains.Checked)
                return "Y";
            else return "N";
        }

        private void ProduceDDbt_Click(object sender, EventArgs e)
        {

            if (!this.bw.IsBusy && DLCdb.Count > 0)
            {
                pbUpdateProgress.Style = ProgressBarStyle.Marquee;
                pbUpdateProgress.MarqueeAnimationSpeed = 60;
                pbUpdateProgress.Visible = true;
                lblCurrentOperation.Text = "Generating DD Content ...";
                lblCurrentOperation.Visible = true;
                this.Refresh();

                ProduceDDbt.Enabled = false;
                this.bw.RunWorkerAsync();
            }
        }

        private void AddArrBT_Click(object sender, EventArgs e)
        {
            using (var ofd = new VistaOpenFileDialog())
            using (var sfd = new VistaFolderBrowserDialog())
            {
                ofd.Filter = "Select Package or Arrangement (*.psarc;*.dat;*.edat;*.xml)|*.psarc;*.dat;*.edat;*.xml|" + "All files|*.*";
                ofd.FilterIndex = 0;
                ofd.Multiselect = true;
                ofd.ReadOnlyChecked = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                foreach (var file in ofd.FileNames)
                {
                    if (file.EndsWith("_showlights.xml") ||
                        file.EndsWith(".dlc.xml") ||
                        file.StartsWith("DDC_"))
                        continue;

                    if (!DLCdb.ContainsValue(file))
                        DLCdb.Add(Path.GetFileNameWithoutExtension(file), file);
                }
            }

            FillDB();
        }

        private void rampUpBT_Click(object sender, EventArgs e)
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

                foreach (var file in ofd.FileNames)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    Directory.CreateDirectory(@".\ddc\umdls\");
                    var path = String.Format(@".\ddc\umdls\user_{0}.xml", name);
                    if (!ramUpMdlsCbox.Items.Contains(name))
                    {
                        try
                        {
                            File.Copy(file, path, true);
                            ramUpMdlsCbox.Items.Add(name);
                        }
                        catch { }
                    }
                    ramUpMdlsCbox.SelectedIndex = ramUpMdlsCbox.FindStringExact(name);
                }
            }
        }

        private void ConfigFilesBtn_Click(object sender, EventArgs e)
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

                foreach (var file in ofd.FileNames)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    Directory.CreateDirectory(@".\ddc\ucfg\");
                    var path = String.Format(@".\ddc\ucfg\user_{0}.cfg", name);
                    if (!ConfigFilesCbx.Items.Contains(name))
                    {
                        try { File.Copy(file, path, true); }
                        catch { }
                        ConfigFilesCbx.Items.Add(name);
                    }
                    ConfigFilesCbx.SelectedIndex = ConfigFilesCbx.FindStringExact(name);
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
                        Process.Start("microsoft-edge:"+link);
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

        private void PopMDLs()
        {
            if (Directory.Exists(DdcBD)) //@".\ddc\"
            {
                ramUpMdlsCbox.Items.Clear();
                RampMdlsDb.Clear();
                foreach (var mdl in Directory.EnumerateFiles(DdcBD, "*.xml", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(mdl);
                    if (name.StartsWith("user_")) name = name.Remove(0, 5);
                    ramUpMdlsCbox.Items.Add(name);
                    ramUpMdlsCbox.SelectedIndex = ramUpMdlsCbox.FindStringExact("ddc_default");
                    RampMdlsDb.Add(name, Path.GetFullPath(mdl));
                }
                ramUpMdlsCbox.Refresh();
            }
        }

        private void PopCFGs()
        {
            if (Directory.Exists(DdcBD))
            {
                ConfigFilesCbx.Items.Clear();
                ConfigsDb.Clear();
                foreach (var cfg in Directory.EnumerateFiles(DdcBD, "*.cfg", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(cfg);
                    if (name.StartsWith("user_")) name = name.Remove(0, 5);
                    ConfigFilesCbx.Items.Add(name);
                    ConfigFilesCbx.SelectedIndex = ConfigFilesCbx.FindStringExact("ddc_default");
                    ConfigsDb.Add(name, Path.GetFullPath(cfg));
                }
                ConfigFilesCbx.Refresh();
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
            if (DDCfilesDgw.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure to delete the selected file?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

                string file = e.Row.Cells["PathColnm"].Value.ToString();
                string value = Path.GetFileNameWithoutExtension(file);

                if (DLCdb != null) DLCdb.Remove(value);
            }
        }

        private void ramUpMdlsCbox_DropDown(object sender, EventArgs e)
        {
            PopMDLs();
        }

        private void ConfigFilesCbx_DropDown(object sender, EventArgs e)
        {
            PopCFGs();
        }

        private void deleteArrBT_Click(object sender, EventArgs e)
        {
            if (DDCfilesDgw.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure to delete the selected file?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

                foreach (DataGridViewRow row in DDCfilesDgw.SelectedRows)
                {
                    string file = row.Cells["PathColnm"].Value.ToString();
                    string value = Path.GetFileNameWithoutExtension(file);

                    if (DLCdb != null) DLCdb.Remove(value);
                }

                FillDB();
            }
        }

        private void Highlight_CheckStateChanged(object sender, EventArgs e)
        {
            chkRemoveSustains.ForeColor = chkRemoveSustains.Checked ? EnabledColor : DisabledColor;
            chkOverwrite.ForeColor = chkOverwrite.Checked ? EnabledColor : DisabledColor;
            chkGenArrIds.ForeColor = chkGenArrIds.Checked ? EnabledColor : DisabledColor;
            chkGenLogFile.ForeColor = chkGenLogFile.Checked ? EnabledColor : DisabledColor;
        }

        private void ramUpMdlsCbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            isNDD = ((ComboBox)sender).Text.Equals("ddc_dd_remover");
            ProduceDDbt.Text = (isNDD) ? "Remove DD" : "Generate DD";
        }

        private void DDCfilesDgw_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    if (!(Path.GetExtension(file) == ".xml" ||
                          Path.GetExtension(file) == ".dat" ||
                          Path.GetExtension(file) == ".psarc" ||
                          Path.GetExtension(file) == "" ||
                          Path.GetExtension(file) == ".edat"))
                        continue;

                    if (file.EndsWith("_showlights.xml") ||
                        file.EndsWith(".dlc.xml") ||
                        file.StartsWith("DDC_"))
                        continue;

                    if (!DLCdb.ContainsValue(file))
                        DLCdb.Add(Path.GetFileNameWithoutExtension(file), file);
                }

                FillDB();
            }
        }

        private void DDCfilesDgw_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }
}

// CODE Grave Yard

/*
 * 
        private int ApplyPackageDD(string file, string remSUS, string rampPath, string cfgPath, out string consoleOutputPkg, bool keepLog = false)
        {
            int singleResult = -1;
            bool exitedByError = false;
            consoleOutputPkg = String.Empty;
            var tmpDir = Path.GetTempPath();
            var platform = file.GetPlatform();
            var unpackedDir = Packer.Unpack(file, tmpDir);

            var xmlFiles = Directory.EnumerateFiles(unpackedDir, "*.xml", SearchOption.AllDirectories);
            foreach (var xml in xmlFiles)
            {
                if (Path.GetFileNameWithoutExtension(xml).ToUpperInvariant().Contains("VOCAL"))
                    continue;
                if (Path.GetFileNameWithoutExtension(xml).ToUpperInvariant().Contains("SHOWLIGHT"))
                    continue;

                singleResult = ApplyDD(xml, remSUS, rampPath, cfgPath, out consoleOutputPkg, true, keepLog);
                if (singleResult == 1)
                {
                    exitedByError = true;
                    break;
                }
                else if (singleResult == 2)
                    consoleOutputPkg = String.Format("Arrangement file '{0}' => {1}", Path.GetFileNameWithoutExtension(xml), consoleOutputPkg);
            }

            if (!exitedByError)
            {
                var logFiles = Directory.EnumerateFiles(unpackedDir, "*.log", SearchOption.AllDirectories);
                var newName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(file).StripPlatformEndName().GetValidFileName().Replace("_DD", "").Replace("_NDD", ""),
                    isNDD ? "NDD" : "DD", platform.GetPathName()[2]));
                if (keepLog)
                {
                    string clogDir = Path.Combine(Path.GetDirectoryName(newName), "DDC_Log");
                    string plogDir = Path.Combine(clogDir, Path.GetFileNameWithoutExtension(newName).StripPlatformEndName().Replace("_DD", "").Replace("_NDD", ""));

                    if (!Directory.Exists(clogDir)) Directory.CreateDirectory(clogDir);
                    DirectoryExtension.SafeDelete(plogDir); Directory.CreateDirectory(plogDir);
                    foreach (var logFile in logFiles)
                    {
                        File.Move(logFile, Path.Combine(plogDir, Path.GetFileName(logFile)));
                    }
                }
                else
                {
                    foreach (var logFile in logFiles.Where(File.Exists))
                    {
                        File.Delete(logFile);
                    }
                }

                Packer.Pack(unpackedDir, newName, true, platform, true);
                DirectoryExtension.SafeDelete(unpackedDir);
            }
            return singleResult;
        }

 */