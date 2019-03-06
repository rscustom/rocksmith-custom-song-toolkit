using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using RocksmithToolkitLib.XmlRepository;
using SharpConfig;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;


namespace RocksmithToolkitGUI.DLCInlayCreator
{
    public partial class DLCInlayCreator : UserControl
    {
        #region Properties

        public static string GlobalTitlePlatform; // used by IntroScreensCreator

        private const string MESSAGEBOX_CAPTION = "DLC Inlay Creator";
        private string IconFile = String.Empty;
        private string InlayFile = String.Empty;

        private BackgroundWorker bwGenerate = new BackgroundWorker();
        private StringBuilder errorsFound;
        private string dlcSavePath;
        private string defaultDir;

        private string workDir
        {
            get { return Path.GetDirectoryName(Application.ExecutablePath); }
        }

        private string Author
        {
            get { return authorTextbox.Text; }
            set { authorTextbox.Text = value; }
        }

        private string InlayName
        {
            get { return inlayNameTextbox.Text; }
            set { inlayNameTextbox.Text = value; }
        }

        private bool Frets24
        {
            get { return Frets24Checkbox.Checked; }
            set { Frets24Checkbox.Checked = value; }
        }

        private bool Colored
        {
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
        }

        private void DLCInlayCreator_Load(object sender, EventArgs e)
        {
            try
            {
                platformPC.Checked = true;
                platformMAC.Checked = platformXBox360.Checked = platformPS3.Checked = false;
                PopulateInlayTypeCombo();
                PopulateAppIdCombo();
                PopulateInlayTemplateCombo();
                DefaultResourceToFile();
            }
            catch { /*For mono compatibility*/ }
        }

        private void PopulateInlayTypeCombo()
        {
            var enumList = Enum.GetNames(typeof(ModType)).ToList<string>();
            inlayTypeCombo.DataSource = enumList;
            inlayTypeCombo.SelectedIndex = 0;
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

        private void PopulateInlayTemplateCombo()
        {
            var templateList = Directory.EnumerateFiles(Path.Combine(workDir, "cgm"));
            inlayTemplateCombo.Items.Clear();
            inlayTemplateCombo.Items.Add("Select template");
            foreach (var template in templateList)
            {
                if (Path.GetExtension(template).ToLower() == ".cgm")
                {
                    inlayTemplateCombo.Items.Add(Path.GetFileNameWithoutExtension(template));
                }
            }
            inlayTemplateCombo.SelectedIndex = 0;
        }

        private void plataform_CheckedChanged(object sender, EventArgs e)
        {
            appIdCombo.Enabled = platformPC.Checked || platformMAC.Checked;
        }

        private void picIcon_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a valid PNG 512x512 image file";
                ofd.Filter = "Image file 512x512 (*.png)|*.png";
                ofd.FilterIndex = 1;
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                var fileName = ofd.FileName;
                if (fileName.IsValidImage())
                    picIcon.ImageLocation = IconFile = fileName;
                else
                    MessageBox.Show("The selected image is not valid or not supported." + Environment.NewLine +
                                        "MimeType doesn't match with file extension!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void picInlay_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a valid PNG 1024x512 image file";
                ofd.Filter = "Image file 1024x512 (*.png)|*.png";
                ofd.FilterIndex = 1;
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                var fileName = ofd.FileName;
                if (fileName.IsValidImage())
                    picInlay.ImageLocation = InlayFile = fileName;
                else
                    MessageBox.Show("The selected image is not valid or not supported." + Environment.NewLine +
                                        "MimeType doesn't match with file extension!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void inlayNameTextbox_Leave(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            textbox.Text = textbox.Text.Trim().GetValidInlayName(Frets24);
        }

        private void FlipX_Changed(object sender, EventArgs e)
        {
            ExternalApps.PngFlipX(InlayFile);
            picInlay.ImageLocation = InlayFile;
        }

        private void FlipY_Changed(object sender, EventArgs e)
        {
            ExternalApps.PngFlipY(InlayFile);
            picInlay.ImageLocation = InlayFile;
        }

        private void DefaultResourceToFile()
        {
            Author = ConfigRepository.Instance()["general_defaultauthor"];
            InlayName = ConfigRepository.Instance()["cgm_inlayname"];
            Frets24 = ConfigRepository.Instance().GetBoolean("cgm_24frets");
            Colored = ConfigRepository.Instance().GetBoolean("cgm_coloredinlay");

            defaultDir = Path.Combine(Path.GetTempPath(), InlayName);
            IconFile = Path.Combine(defaultDir, "icon.png");
            InlayFile = Path.Combine(defaultDir, "inlay.png");

            if (!Directory.Exists(defaultDir))
            {
                Directory.CreateDirectory(defaultDir);

                // Icon Resource
                using (var iconStream = new MemoryStream(RocksmithToolkitLib.Properties.Resources.cgm_default_icon))
                {
                    iconStream.WriteFile(IconFile);
                }

                // Inlay Resource
                using (var inlayStream = new MemoryStream(RocksmithToolkitLib.Properties.Resources.cgm_default_inlay))
                {
                    inlayStream.WriteFile(InlayFile);
                }
            }

            picIcon.ImageLocation = IconFile;
            picInlay.ImageLocation = InlayFile;
        }

        private void loadCGMButton_Click(object sender, EventArgs e)
        {
            var customCGM = String.Empty;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a CGM file";
                ofd.Filter = "CGM file (*.cgm)|*.cgm";
                ofd.FilterIndex = 1;
                ofd.InitialDirectory = Path.Combine(workDir, "cgm");
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                customCGM = ofd.FileName;
            }
            inlayTemplateCombo.SelectedIndex = 0;
            LoadCGM(customCGM);
        }

        private void inlayTemplateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inlayTemplateCombo.SelectedIndex != 0)
                LoadCGM(Path.Combine(workDir, "cgm", inlayTemplateCombo.SelectedItem + ".cgm"));
            else
            {
                DefaultResourceToFile();
            }
        }

        private void LoadCGM(string customCGM)
        {
            if (!String.IsNullOrEmpty(customCGM))
            {
                // Unpack CGM file (7z file format)
                var unpackedFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(customCGM));

                if (Directory.Exists(unpackedFolder))
                    IOExtension.DeleteDirectory(unpackedFolder);

                ExternalApps.ExtractZip(customCGM, unpackedFolder);

                var errorMessage = string.Format("Template {0} can't be loaded, maybe doesn't exists or is corrupted.", customCGM);
                if (!Directory.Exists(unpackedFolder))
                {
                    MessageBox.Show(errorMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Setup inlay pre-definition
                Frets24Checkbox.Checked = ColoredCheckbox.Checked = chkFlipX.Checked = chkFlipY.Checked = false;

                // Open the setup.smb INI file
                Configuration iniConfig = Configuration.LoadFromFile(Path.Combine(unpackedFolder, "setup.smb"));

                // switch to new sharpconfig.dll ini file format with [General] section
                // allow for backward compatibility with old *.cgm files
                try
                {
                    Author = iniConfig["General"]["author"].Value;
                    InlayName = iniConfig["General"]["inlayname"].Value;
                    Frets24 = iniConfig["General"]["24frets"].GetValue<bool>();
                    Colored = iniConfig["General"]["colored"].GetValue<bool>();
                }
                catch
                {
                    Author = iniConfig["Setup"]["creatorname"].Value;
                    InlayName = iniConfig["Setup"]["guitarname"].Value;
                    try
                    {
                        // skip exception for templates created with old v2.0.0.71b
                        Frets24 = iniConfig["Setup"]["24frets"].GetValue<bool>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: Incomplete INI file - " + e.Message);
                    }
                    Colored = iniConfig["Setup"]["coloredinlay"].GetValue<bool>();
                }

                // Convert the dds files to png
                var iconFile = Path.Combine(unpackedFolder, "icon.dds");
                ExternalApps.Dds2Png(iconFile);
                var inlayFile = Path.Combine(unpackedFolder, "inlay.dds");
                ExternalApps.Dds2Png(inlayFile);

                if (!File.Exists(iconFile) || !File.Exists(inlayFile))
                {
                    MessageBox.Show(errorMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load png into picBoxes and save paths
                picIcon.ImageLocation = IconFile = Path.ChangeExtension(iconFile, ".png");
                picInlay.ImageLocation = InlayFile = Path.ChangeExtension(inlayFile, ".png");
            }
        }

        private void saveCGMButton_Click(object sender, EventArgs e)
        {
            var saveFile = String.Empty;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Select a location to store your CGM file";
                sfd.Filter = "CGM file (*.cgm)|*.cgm";
                sfd.InitialDirectory = Path.Combine(workDir, "cgm");
                sfd.FileName = (InlayName.GetValidInlayName(Frets24)) + "_" + StringExtensions.GetValidAcronym(Author) + ".cgm".GetValidFileName();

                if (sfd.ShowDialog() != DialogResult.OK) return;
                saveFile = sfd.FileName;
            }

            // Create workDir folder
            var tmpWorkDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(saveFile));

            if (Directory.Exists(tmpWorkDir))
                IOExtension.DeleteDirectory(tmpWorkDir);

            if (!Directory.Exists(tmpWorkDir))
                Directory.CreateDirectory(tmpWorkDir);

            // Convert PNG to DDS
            ExternalApps.Png2Dds(IconFile, Path.Combine(tmpWorkDir, "icon.dds"), 512, 512);
            ExternalApps.Png2Dds(InlayFile, Path.Combine(tmpWorkDir, "inlay.dds"), 1024, 512);

            // Create setup.smb
            var iniFile = Path.Combine(tmpWorkDir, "setup.smb");
            Configuration iniCFG = new Configuration();

            // sharpconfig.dll automatically creates a new [General] section in the INI file
            iniCFG["General"].Add(new Setting("author", String.IsNullOrEmpty(Author) ? "CSC" : Author));
            iniCFG["General"].Add(new Setting("inlayname", String.IsNullOrEmpty(InlayName) ? "null" : InlayName));
            iniCFG["General"].Add(new Setting("24frets", Convert.ToString(Convert.ToInt32(Frets24))));
            iniCFG["General"].Add(new Setting("colored", Convert.ToString(Convert.ToInt32(Colored))));
            iniCFG["General"].Add(new Setting("cscvers", ToolkitVersion.RSTKGuiVersion.Replace("-00000000", "")));
            iniCFG["General"].Add(new Setting("modified", DateTime.Now.ToShortDateString()));
            iniCFG.Save(iniFile);

            // Pack file into a .cgm file (7zip file format)
            ExternalApps.InjectZip(tmpWorkDir, saveFile, false, true);

            // Delete temp work dir
            if (Directory.Exists(tmpWorkDir))
                IOExtension.DeleteDirectory(tmpWorkDir);

            if (MessageBox.Show("Inlay template was saved." + Environment.NewLine + "Would you like to open the folder?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Process.Start(Path.GetDirectoryName(saveFile));
            }

            if (Path.GetDirectoryName(saveFile) == Path.Combine(workDir, "cgm"))
            {
                inlayTemplateCombo.Items.Add(Path.GetFileNameWithoutExtension(saveFile));
                inlayTemplateCombo.SelectedIndex = (inlayTemplateCombo.Items.Count - 1);
            }
        }

        private void inlayGenerateButton_Click(object sender, EventArgs e)
        {
            // put inlays in proper directory
            if (Directory.Exists(ConfigRepository.Instance()["general_rs2014path"]))
            {
                dlcSavePath = Path.Combine(ConfigRepository.Instance()["general_rs2014path"], "dlc", "Inlays");
            }
            else
            {
                dlcSavePath = Path.Combine(workDir, "cgm", "dlc", "Inlays");
            }

            if (!Directory.Exists(dlcSavePath))
                Directory.CreateDirectory(dlcSavePath);

            using (var ofd = new SaveFileDialog())
            {
                ofd.FileName = (InlayName.GetValidInlayName(Frets24).ToLower()).GetValidFileName();
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
            packageData.Inlay.DLCSixName = GeneralExtension.RandomName(6);

            // CRITICAL - 24 fret inlays have naming dependencies
            if (Frets24) packageData.Inlay.DLCSixName = String.Format("24fret_{0}", packageData.Inlay.DLCSixName);

            packageData.Name = InlayName;
            packageData.AppId = appIdCombo.SelectedValue.ToString();

            // Saving for later
            ConfigRepository.Instance()["cgm_inlayname"] = InlayName;
            ConfigRepository.Instance()["cgm_24frets"] = Frets24.ToString();
            ConfigRepository.Instance()["cgm_coloredinlay"] = Colored.ToString();

            // Generate
            OverwriteExistingPackage();

            if (Path.GetFileName(dlcSavePath).Contains(" ") && platformPS3.Checked)
                if (!ConfigRepository.Instance().GetBoolean("creator_ps3pkgnamewarn"))
                {
                    MessageBox.Show(String.Format("PS3 package name can't support space character due to encryption limitation. {0} Spaces will be automatic removed for your PS3 package name.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ConfigRepository.Instance()["creator_ps3pkgnamewarn"] = true.ToString();
                }

            if (!bwGenerate.IsBusy && packageData != null)
            {
                updateProgress.Visible = true;
                currentOperationLabel.Visible = true;
                inlayGenerateButton.Enabled = false;
                bwGenerate.RunWorkerAsync(packageData);
            }
        }

        private void GeneratePackage(object sender, DoWorkEventArgs e)
        {
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
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating PC package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.Pc, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate PC package: {0}", ex.Message));
                }

            if (platformMAC.Checked)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating Mac package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.Mac, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate Mac package: {0}", ex.Message));
                }

            if (platformXBox360.Checked)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating XBox 360 package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.XBox360, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate XBox 360 package: {0}", ex.Message));
                }

            if (platformPS3.Checked)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating PS3 package");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.PS3, GameVersion.RS2014), DLCPackageType.Inlay);
                    progress += step;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate PS3 package: {0}. {1}PS3 package require 'JAVA x86' (32 bits) installed on your machine to generate properly.", ex.Message, Environment.NewLine));
                }

            // Cache cleanup so we don't serialize or reuse data that could be changed
            packageData.CleanCache();

            if (numPlatforms != 0) e.Result = "generate";
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= updateProgress.Maximum)
                updateProgress.Value = e.ProgressPercentage;
            else
                updateProgress.Value = updateProgress.Maximum;

            ShowCurrentOperation(e.UserState as string);
        }

        private void ProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateProgress.Visible = false;
            currentOperationLabel.Visible = false;

            switch (Convert.ToString(e.Result))
            {
                case "generate":
                    var message = "Package was generated.";
                    if (errorsFound.Length > 0)
                        message = String.Format("Package was generated with errors! See below: " + Environment.NewLine + errorsFound.ToString());

                    message += Environment.NewLine + "Would you like to open the folder?";

                    if (MessageBox.Show(message, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start(Path.GetDirectoryName(dlcSavePath));
                    }

                    this.Focus();
                    inlayGenerateButton.Enabled = true;
                    break;
            }
            this.Focus();
            inlayGenerateButton.Enabled = true;
        }

        private void ShowCurrentOperation(string message)
        {
            currentOperationLabel.Text = message;
            currentOperationLabel.Refresh();
        }

        public void DLCInlayCreator_Dispose(object sender, EventArgs e)
        {
            if (Directory.Exists(defaultDir))
                IOExtension.DeleteDirectory(defaultDir);
        }

        private void helpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // inlay creator help link
            string link = "http://goo.gl/pJxMuz";
            Process.Start(link);
        }

        private void OverwriteExistingPackage()
        {
            var packageArray = new string[,]
                {
                    {platformPC.Checked.ToString(), String.Format("{0}_p.psarc", dlcSavePath)},
                    {platformMAC.Checked.ToString(), String.Format("{0}_m.psarc", dlcSavePath)},
                    {platformXBox360.Checked.ToString(), String.Format("{0}_xbox", dlcSavePath)},
                    {platformPS3.Checked.ToString(), String.Format("{0}_ps3", dlcSavePath)}
                };

            for (int i = 0; i < packageArray.GetUpperBound(0); i++)
            {
                if (packageArray[i, 0] == "True")
                {
                    if (File.Exists(packageArray[i, 1]))
                    {
                        if (MessageBox.Show("The package \"" + Path.GetFileName(packageArray[i, 1]) + "\" already exists." + Environment.NewLine + "Would you like to overwrite the file?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            switch (i)
                            {
                                case 0:
                                    platformPC.Checked = false;
                                    break;
                                case 1:
                                    platformMAC.Checked = false;
                                    break;
                                case 2:
                                    platformXBox360.Checked = false;
                                    break;
                                case 3:
                                    platformPS3.Checked = false;
                                    break;
                            }

                        }
                    }
                }
            }
        }

        private void inlayTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (inlayTypeCombo.SelectedIndex)
            {
                case 0: // Guitar Inlays
                    gbRbPlatform.Visible = false;
                    introScreensCreator1.Visible = false;
                    break;

                // TODO: add Xbox360 and PS3 support
                case 1: // Intro Screens 
                    introScreensCreator1.LoadIntroScreens();
                    introScreensCreator1.Location = new Point(0, 45);
                    introScreensCreator1.Size = new Size(507, 450);
                    gbRbPlatform.Location = new Point(201, 0);
                    rbPc.Checked = true; // initializes GlogalTitlePlatform
                    gbRbPlatform.Visible = true;
                    introScreensCreator1.Visible = true;
                    break;
            }
        }

        private void radioPlatform_CheckedChanged(object sender, EventArgs e)
        {
            GlobalTitlePlatform = null;
            if (rbXbox360.Checked || rbPs3.Checked)
            {
                MessageBox.Show("Only PC and Mac platforms are supported at this time." + Environment.NewLine +
                   "Beta testers needed for Xbox360 and PS3 platforms." + Environment.NewLine +
                "Contact us through http://goo.gl/pJxMuz.", "Custom Intro Screens", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                rbPs3.Checked = false;
                rbXbox360.Checked = false;
                return;
            }
            if (rbPc.Checked) GlobalTitlePlatform = "Pc";
            if (rbMac.Checked) GlobalTitlePlatform = "Mac";
        }

    }
}