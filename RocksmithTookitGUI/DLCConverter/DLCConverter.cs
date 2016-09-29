using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;
using System.ComponentModel;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitGUI.DLCConverter
{
    public partial class DLCConverter : UserControl
    {
        #region Constants

        private const string MESSAGEBOX_CAPTION = "CDLC Converter";

        #endregion

        private BackgroundWorker bwConvert;
        private StringBuilder errorsFound;

        public DLCConverter()
        {
            InitializeComponent();
            bwConvert = new BackgroundWorker { WorkerReportsProgress = true };
        }

        public string AppId
        {
            get { return txtAppId.Text; }
            set { txtAppId.Text = value; }
        }

        public Platform SourcePlatform { get; set; }
        public Platform TargetPlatform { get; set; }

        private void AppIdVisibilty()
        {
            if (cmbTargetPlatform.SelectedItem != null)
            {
                var target = new Platform(cmbTargetPlatform.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                cmbAppId.Enabled = !target.IsConsole;
                txtAppId.Enabled = !target.IsConsole;
            }
        }

        private void PopulateAppIdCombo(GameVersion gameVersion)
        {
            cmbAppId.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
                cmbAppId.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select(ConfigRepository.Instance()["general_defaultappid_RS2014"], gameVersion);
            cmbAppId.SelectedItem = songAppId;
            AppId = songAppId.AppId;
        }

        private void SelectComboAppId(string appId)
        {
            var songAppId = SongAppIdRepository.Instance().Select(appId, GameVersion.RS2014);
            if (SongAppIdRepository.Instance().List.Any<SongAppId>(a => a.AppId == appId))
                cmbAppId.SelectedItem = songAppId;
            else
            {
                if (!appId.IsAppIdSixDigits())
                    MessageBox.Show("Please enter a valid six digit  " + Environment.NewLine + "App ID before continuing.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    MessageBox.Show("User entered an unknown AppID." + Environment.NewLine + Environment.NewLine + "Toolkit will use the AppID that  " + Environment.NewLine + "was entered manually but it can  " + Environment.NewLine + "not assess its validity.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowCurrentOperation(string message)
        {
            lblCurrentOperation.Text = message;
            lblCurrentOperation.Refresh();
        }

        private void ToggleUIControls(bool enable)
        {
            picLogo.Select();
            picLogo.Focus();
            btnConvert.Enabled = enable;
            cmbAppId.Enabled = enable;
            cmbSourcePlatform.Enabled = enable;
            cmbTargetPlatform.Enabled = enable;
            txtAppId.Enabled = enable;
        }

        private void defineSourcePlatform()
        {
            if (cmbSourcePlatform.Items.Count > 0)
                SourcePlatform = new Platform(cmbSourcePlatform.SelectedItem.ToString(), GameVersion.RS2014.ToString());
            else
                SourcePlatform = new Platform(GamePlatform.None, GameVersion.None);
        }

        private void defineTargetPlatform()
        {
            if (cmbTargetPlatform.Items.Count > 0)
                TargetPlatform = new Platform(cmbTargetPlatform.SelectedItem.ToString(), GameVersion.RS2014.ToString());
            else
                TargetPlatform = new Platform(GamePlatform.None, GameVersion.None);
        }

        private void DLCConverter_Load(object sender, EventArgs e)
        {
            try
            {
                // Fill source combo
                var sourcePlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
                sourcePlatform.Remove("None");
                cmbSourcePlatform.DataSource = sourcePlatform;
                cmbSourcePlatform.SelectedItem = ConfigRepository.Instance()["converter_source"];

                // Fill target combo
                var targetPlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
                targetPlatform.Remove("None");
                cmbTargetPlatform.DataSource = targetPlatform;
                cmbTargetPlatform.SelectedItem = ConfigRepository.Instance()["converter_target"];

                // Fill AppID
                PopulateAppIdCombo(GameVersion.RS2014); //Supported game version
                AppIdVisibilty();
            }
            catch { /*For mono compatibility*/ }

            // Converter worker
            bwConvert.DoWork += doConvert;
            bwConvert.ProgressChanged += (se, ea) =>
            {
                if (ea.ProgressPercentage <= pbUpdateProgress.Maximum)
                    pbUpdateProgress.Value = ea.ProgressPercentage;
                else
                    pbUpdateProgress.Value = pbUpdateProgress.Maximum;
                ShowCurrentOperation(ea.UserState as string);
            };
            bwConvert.RunWorkerCompleted += ProcessCompleted;
        }

        private void ProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            switch (Convert.ToString(e.Result))
            {
                case "done":
                    if (errorsFound.Length <= 0)
                        MessageBox.Show(
                            String.Format("DLC was converted from '{0}' to '{1}'.\n", SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information
                        );
                    else
                        MessageBox.Show(
                            String.Format("DLC was converted from '{2}' to '{3}' with errors. See below: {0}{1}{0}", Environment.NewLine, errorsFound.ToString(), SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );

                    break;
            }

            ToggleUIControls(true);
            pbUpdateProgress.Visible = false;
            lblCurrentOperation.Visible = false;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            defineSourcePlatform();
            defineTargetPlatform();
            // VALIDATIONS
            if (SourcePlatform.Equals(TargetPlatform))
            {
                MessageBox.Show("The source and target platform should be different.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // GET FILES
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select one DLC for platform conversion";
                ofd.Multiselect = true;
                switch (SourcePlatform.platform)
                {
                    case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        ofd.Filter = "PC or Mac Rocksmith 2014 DLC (*.psarc)|*.psarc";
                        break;
                    case GamePlatform.XBox360:
                        ofd.Filter = "XBox 360 Rocksmith 2014 DLC (*.)|*.*";
                        break;
                    case GamePlatform.PS3:
                        ofd.Filter = "PS3 Rocksmith 2014 DLC (*.edat)|*.edat";
                        break;
                    default:
                        MessageBox.Show("The converted audio on Wwise 2013 for target platform should be selected.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                if (!bwConvert.IsBusy && ofd.FileNames.Length > 0)
                {
                    pbUpdateProgress.Value = 0;
                    pbUpdateProgress.Visible = true;
                    lblCurrentOperation.Visible = true;
                    ToggleUIControls(false);
                    bwConvert.RunWorkerAsync(ofd.FileNames);
                }
            }
        }

        private void cmbAppId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAppId.SelectedItem != null)
                AppId = ((SongAppId)cmbAppId.SelectedItem).AppId;
        }

        private void doConvert(object sender, DoWorkEventArgs e)
        {
            // SOURCE
            var sourceFileNames = e.Argument as string[];
            errorsFound = new StringBuilder();
            var step = (int)Math.Round(1.0 / sourceFileNames.Length * 100, 0);
            int progress = 0;
            foreach (var sourcePackage in sourceFileNames)
            {
                bwConvert.ReportProgress(progress, String.Format("Converting '{0}' to {1} platform.", Path.GetFileName(sourcePackage), TargetPlatform.platform.GetPathName()[0]));
                if (!sourcePackage.IsValidPSARC())
                {
                    errorsFound.AppendLine(String.Format("File '{0}' isn't valid. File extension was changed to '.invalid'", Path.GetFileName(sourcePackage)));

                    return;
                }

                var alertMessage = String.Format("Source package '{0}' seems to be not {1} platform, the conversion impossible.", Path.GetFileName(sourcePackage), SourcePlatform);
                var haveCorrectName = Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(SourcePlatform.GetPathName()[2]);
                if (SourcePlatform.platform == GamePlatform.PS3)
                    haveCorrectName = Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(SourcePlatform.GetPathName()[2] + ".psarc");

                if (!haveCorrectName)
                {
                    errorsFound.AppendLine(alertMessage);
                    if (MessageBox.Show(alertMessage + Environment.NewLine + "Force try to convert this package?", MESSAGEBOX_CAPTION, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        continue;
                }

                try
                {
                    // CONVERT
                    var output = DLCPackageConverter.Convert(sourcePackage, SourcePlatform, TargetPlatform, AppId);
                    if (!String.IsNullOrEmpty(output))
                        errorsFound.AppendLine(output);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("{0}\n{1}\n", ex.Message, ex.StackTrace));
                }
                progress += step;
                bwConvert.ReportProgress(progress);
            }
            bwConvert.ReportProgress(100);
            e.Result = "done";
        }

        private void platformTargetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppIdVisibilty();
        }

        private void txtAppId_Validating(object sender, CancelEventArgs e)
        {
            var appId = ((TextBox)sender).Text.Trim();
            SelectComboAppId(appId);
        }

    }
}
