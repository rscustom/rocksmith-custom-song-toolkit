using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using Ookii.Dialogs;
using SharpConfig;
using RocksmithToolkitGUI.Properties;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI.DLCInlayCreator
{
    public partial class DLCInlayCreator : UserControl {
        #region Properties

        private const string MESSAGEBOX_CAPTION = "Custom Guitar Maker";
        private const string APP_TOPNG = "topng.exe";
        private const string APP_7Z = "7za.exe";
        private const string APP_NVDXT = "nvdxt.exe";

        private string Author = String.Empty;
        private string IconFile = String.Empty;
        private string InlayFile = String.Empty;
        
        private BackgroundWorker bwGenerate = new BackgroundWorker();
        private StringBuilder errorsFound;
        private string dlcSavePath;
        
        private string workDir {
            get { return Path.GetDirectoryName(Application.ExecutablePath); }
        }

        private string InlayName {
            get { return inlayNameTextbox.Text; }
            set { inlayNameTextbox.Text = value; }
        }

        private bool Frets24 {
            get { return Frets24Checkbox.Checked; }
            set { Frets24Checkbox.Checked = value; }
        }

        private bool Colored {
            get { return ColoredCheckbox.Checked; }
            set { ColoredCheckbox.Checked = value; }
        }

        #endregion

        public DLCInlayCreator()
        {
            InitializeComponent();

            // Generate package worker
            bwGenerate.DoWork += new DoWorkEventHandler(GeneratePackage);
            bwGenerate.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            bwGenerate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            bwGenerate.WorkerReportsProgress = true;

            picIcon.Image = Image.FromStream(new MemoryStream(RocksmithToolkitLib.Properties.Resources.cgm_default_icon));
            picInlay.Image = Image.FromStream(new MemoryStream(RocksmithToolkitLib.Properties.Resources.cgm_default_inlay));
        }

        private void ctrlCGM_Load(object sender, EventArgs e)
        {
            try
            {
                PopulateInlayTypeCombo();
                PopulateAppIdCombo();
                InlayName = ConfigRepository.Instance()["cgm_inlayname"];
                Frets24 = ConfigRepository.Instance().GetBoolean("cgm_24frets");
                Colored = ConfigRepository.Instance().GetBoolean("cgm_coloredinlay");
            } catch { /*For mono compatibility*/ }
        }

        private void PopulateInlayTypeCombo() {
            var enumList = Enum.GetNames(typeof(InlayType)).ToList<string>();
            inlayTypeCombo.DataSource = enumList;
        }

        private void PopulateAppIdCombo()
        {
            var appIdList = SongAppIdRepository.Instance().Select(GameVersion.RS2014).ToArray();
            appIdCombo.DataSource = appIdList;
            appIdCombo.DisplayMember = "DisplayName";
            appIdCombo.ValueMember = "AppId";

            var songAppId = SongAppIdRepository.Instance().Select(ConfigRepository.Instance()["general_defaultappid_RS2014"], GameVersion.RS2014);
            appIdCombo.SelectedValue = songAppId.AppId;
        }

        private void plataform_CheckedChanged(object sender, EventArgs e) {
            appIdCombo.Enabled = platformPC.Checked || platformMAC.Checked;
        }

        private void platformTargetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppIdVisibilty();
        }

        private void AppIdVisibilty()
        {
            if (platformPC.Checked || platformMAC.Checked)
                appIdCombo.Enabled = true;
            else if (!platformPC.Checked && !platformMAC.Checked)
                appIdCombo.Enabled = false;
        }

        private void picIcon_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select a valid PNG 512x512 image file";
                ofd.Filter = "Image file 512x512 (*.png)|*.png";
                ofd.FilterIndex = 1;
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                picIcon.ImageLocation = IconFile = ofd.FileName;
            }
        }

        private void picInlay_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select a valid PNG 1024x512 image file";
                ofd.Filter = "Image file 1024x512 (*.png)|*.png";
                ofd.FilterIndex = 1;
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                picInlay.ImageLocation = InlayFile = ofd.FileName;
            }
        }

        private void inlayNameTextbox_Leave(object sender, EventArgs e) {
            TextBox textbox = (TextBox)sender;
            textbox.Text = textbox.Text.Trim().GetValidName();
        }

        private void FlipX_Changed(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(InlayFile))
                DefaultResourceToFile();

            GeneralExtensions.RunExternalExecutable(APP_TOPNG, true, true, true, String.Format("-overwrite -xflip \"{0}\"", InlayFile));
            picInlay.ImageLocation = InlayFile;
        }

        private void FlipY_Changed(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(InlayFile))
                DefaultResourceToFile();

            GeneralExtensions.RunExternalExecutable(APP_TOPNG, true, true, true, String.Format("-overwrite -yflip \"{0}\"", InlayFile));
            picInlay.ImageLocation = InlayFile;
        }

        private void DefaultResourceToFile() {
            // Icon
            using (var iconStream = new MemoryStream(RocksmithToolkitLib.Properties.Resources.cgm_default_icon))
            {
                IconFile = GeneralExtensions.GetTempFileName(".png");
                iconStream.WriteFile(IconFile);
            }

            // Inlay
            using (var iconStream = new MemoryStream(RocksmithToolkitLib.Properties.Resources.cgm_default_inlay))
            {
                InlayFile = GeneralExtensions.GetTempFileName(".png");
                iconStream.WriteFile(InlayFile);
            }
        }

        private void loadCGMButton_Click(object sender, EventArgs e) {
            var customCGM = String.Empty;

            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select a CGM file";
                ofd.Filter = "CGM file (*.cgm)|*.cgm";
                ofd.FilterIndex = 1;
                ofd.InitialDirectory = Path.Combine(workDir, "cgm");
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                customCGM = ofd.FileName;
            }

            if (!String.IsNullOrEmpty(customCGM)) {
                // Unpack CGM file (7z file format)
                var unpackedCGMTmpFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(customCGM));
                var args = String.Format(" x \"{0}\" -o\"{1}\"", customCGM, unpackedCGMTmpFolder);
                GeneralExtensions.RunExternalExecutable(APP_7Z, true, true, true, args);

                // Open the setup.smb INI file
                Configuration iniConfig = Configuration.Load(Path.Combine(unpackedCGMTmpFolder, "setup.smb"), ParseFlags.IgnoreComments);

                Author = iniConfig["Setup"]["creatorname"].Value;
                InlayName = iniConfig["Setup"]["guitarname"].Value;
                Frets24 = iniConfig["Setup"]["24frets"].GetValue<bool>();
                Colored = iniConfig["Setup"]["coloredinlay"].GetValue<bool>();

                // Setup inlay pre-definition
                Frets24Checkbox.Checked = ColoredCheckbox.Checked = chkFlipX.Checked = chkFlipY.Checked = false;
                Frets24Checkbox.Checked = Frets24;
                ColoredCheckbox.Checked = Colored;

                // Convert the dds files to png
                var iconFile = Path.Combine(unpackedCGMTmpFolder, "icon.dds");
                GeneralExtensions.RunExternalExecutable(APP_TOPNG, true, true, true, String.Format(" -out png \"{0}\"", iconFile));
                var inlayFile = Path.Combine(unpackedCGMTmpFolder, "inlay.dds");
                GeneralExtensions.RunExternalExecutable(APP_TOPNG, true, true, true, String.Format(" -out png \"{0}\"", inlayFile));

                // Load png into picBoxes and save paths
                picIcon.ImageLocation = IconFile = Path.ChangeExtension(iconFile, ".png");
                picInlay.ImageLocation = InlayFile = Path.ChangeExtension(inlayFile, ".png");
            }
        }

        private void saveCGMButton_Click(object sender, EventArgs e) {
            var saveFile = String.Empty;

            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select a location to store your CGM file";
                ofd.Filter = "CGM file (*.cgm)|*.cgm";
                ofd.FilterIndex = 1;
                ofd.InitialDirectory = Path.Combine(workDir, "cgm");
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                saveFile = ofd.FileName;
            }

            // Create workDir folder
            var workCGMDir = Path.GetFileNameWithoutExtension(saveFile);
            if (Directory.Exists(workCGMDir))
                DirectoryExtension.SafeDelete(workCGMDir);

            if (!Directory.Exists(workCGMDir))
                Directory.CreateDirectory(workCGMDir);

            // Convert PNG to DDS
            var iconArgs = String.Format(" -file \"{0}\" -prescale 512 512 -quality_highest -max -32 dxt5 -dxt5 -overwrite -alpha -output \"{1}\"", IconFile, Path.ChangeExtension(IconFile, ".dds"));
            GeneralExtensions.RunExternalExecutable(APP_NVDXT, true, true, true, iconArgs);
            var inlayArgs = String.Format(" -file \"{0}\" -prescale 1024 512 -quality_highest -max -32 dxt5 -dxt5 -overwrite -alpha -output \"{1}\"", InlayFile, Path.ChangeExtension(InlayFile, ".dds"));
            GeneralExtensions.RunExternalExecutable(APP_NVDXT, true, true, true, inlayArgs);
            
            // Create setup.smb
            var iniFile = Path.Combine(workCGMDir, "setup.smb");

            Configuration iniCFG = new Configuration();
            iniCFG.Categories.Add(new SettingCategory("Setup"));

            var author = ConfigRepository.Instance()["general_defaultauthor"];
            iniCFG.Categories["Setup"].Settings.Add(new Setting("creatorname", String.IsNullOrEmpty(author) ? "CustomSongCreator" : author));
            iniCFG.Categories["Setup"].Settings.Add(new Setting("guitarname", InlayName));
            iniCFG.Categories["Setup"].Settings.Add(new Setting("24frets", Convert.ToString(Convert.ToInt32(Frets24))));
            iniCFG.Categories["Setup"].Settings.Add(new Setting("coloredinlay", Convert.ToString(Convert.ToInt32(Colored))));
            iniCFG.Categories["Setup"].Settings.Add(new Setting("cgmvers", "0.2.0.0"));
            iniCFG.Categories["Setup"].Settings.Add(new Setting("modified", DateTime.Now.ToShortDateString()));
            iniCFG.Save(iniFile);

            // Pack file into a .cgm file (7zip file format)
            var zipArgs = String.Format(" a \"{0}\" \"{1}\"", saveFile, workCGMDir);
            GeneralExtensions.RunExternalExecutable(APP_7Z, true, true, true, zipArgs);

            if (MessageBox.Show("CGM template was saved." + Environment.NewLine + "You want to open the folder in which the template was saved?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                Process.Start(Path.GetDirectoryName(Path.GetDirectoryName(saveFile)));
            }
        }

        private void inlayGenerateButton_Click(object sender, EventArgs e) {
            dlcSavePath = workDir;
            using (var ofd = new SaveFileDialog()) {
                ofd.FileName = InlayName.GetValidName();
                ofd.Filter = "Custom Inlay DLC (*.*)|*.*";
                ofd.InitialDirectory = dlcSavePath;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }

            DLCPackageData packageData = new DLCPackageData();
            packageData.Inlay = new InlayData();
            packageData.Inlay.InlayPath = InlayFile;
            packageData.Inlay.IconPath = IconFile;
            packageData.Inlay.Frets24 = Frets24;
            packageData.Inlay.Colored = Colored;
            packageData.Name = InlayName;

            // Saving for later
            ConfigRepository.Instance()["cgm_inlayname"] = InlayName;
            ConfigRepository.Instance()["cgm_24frets"] = Frets24.ToString();
            ConfigRepository.Instance()["cgm_coloredinlay"] = Colored.ToString();

            // Generate
            if (Path.GetFileName(dlcSavePath).Contains(" ") && platformPS3.Checked)
                if (!ConfigRepository.Instance().GetBoolean("creator_ps3pkgnamewarn")) {
                    MessageBox.Show(String.Format("PS3 package name can't support space character due to encryption limitation. {0} Spaces will be automatic removed for your PS3 package name.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } else {
                    ConfigRepository.Instance()["creator_ps3pkgnamewarn"] = true.ToString();
                }

            if (!bwGenerate.IsBusy && packageData != null) {
                updateProgress.Visible = true;
                currentOperationLabel.Visible = true;
                inlayGenerateButton.Enabled = false;
                bwGenerate.RunWorkerAsync(packageData);
            }
        }

        private void GeneratePackage(object sender, DoWorkEventArgs e) {
            var packageData = e.Argument as DLCPackageData;
            errorsFound = new StringBuilder();

            var numPlatforms = 0;
            if (platformPC.Checked)
                numPlatforms++;
            if (platformMAC.Checked)
                numPlatforms++;
            if (platformXBox360.Checked)
                numPlatforms++;
            if (platformPS3.Checked)
                numPlatforms++;

            var step = (int)Math.Round(1.0 / numPlatforms * 100, 0);
            int progress = 0;

            if (platformPC.Checked)
                try {
                    bwGenerate.ReportProgress(progress, "Generating PC package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.Pc, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                } catch (Exception ex) {
                    errorsFound.AppendLine(String.Format("Error generate PC package: {0}", ex.Message));
                }

            if (platformMAC.Checked)
                try {
                    bwGenerate.ReportProgress(progress, "Generating Mac package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.Mac, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                } catch (Exception ex) {
                    errorsFound.AppendLine(String.Format("Error generate Mac package: {0}", ex.Message));
                }

            if (platformXBox360.Checked)
                try {
                    bwGenerate.ReportProgress(progress, "Generating XBox 360 package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.XBox360, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                } catch (Exception ex) {
                    errorsFound.AppendLine(String.Format("Error generate XBox 360 package: {0}", ex.Message));
                }

            if (platformPS3.Checked)
                try {
                    bwGenerate.ReportProgress(progress, "Generating PS3 package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.PS3, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                } catch (Exception ex) {
                    errorsFound.AppendLine(String.Format("Error generate PS3 package: {0}. {1}PS3 package require 'JAVA x86' (32 bits) installed on your machine to generate properly.", ex.Message, Environment.NewLine));
                }

            // Cache cleanup so we don't serialize or reuse data that could be changed
            packageData.CleanCache();
            e.Result = "generate";
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.ProgressPercentage <= updateProgress.Maximum)
                updateProgress.Value = e.ProgressPercentage;
            else
                updateProgress.Value = updateProgress.Maximum;

            ShowCurrentOperation(e.UserState as string);
        }

        private void ProcessCompleted(object sender, RunWorkerCompletedEventArgs e) {
            switch (Convert.ToString(e.Result)) {
                case "generate":
                    var message = "Package was generated.";
                    if (errorsFound.Length > 0)
                        message = String.Format("Package was generated with errors! See below: " + Environment.NewLine + errorsFound.ToString());

                    message += Environment.NewLine + "You want to open the folder in which the package was generated?";

                    if (MessageBox.Show(message, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                        Process.Start(Path.GetDirectoryName(dlcSavePath));
                    }

                    this.Focus();
                    inlayGenerateButton.Enabled = true;
                    break;
            }

            updateProgress.Visible = false;
            currentOperationLabel.Visible = false;
        }

        private void ShowCurrentOperation(string message) {
            currentOperationLabel.Text = message;
            currentOperationLabel.Refresh();
        }
    }
}