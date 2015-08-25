using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Ookii.Dialogs;
using X360.STFS;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;
using System.ComponentModel;

namespace RocksmithToolkitGUI.DLCConverter
{
    public partial class DLCConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "DLC Converter";
        private BackgroundWorker bwConvert;
        private StringBuilder errorsFound;

        public string AppId
        {
            get { return AppIdTB.Text; }
            set { AppIdTB.Text = value; }
        }

        public Platform SourcePlatform {
            get ; set;
        }
        void defineSourcePlatform()
        {
            if (platformSourceCombo.Items.Count > 0)
                SourcePlatform = new Platform(platformSourceCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
            else 
                SourcePlatform = new Platform(GamePlatform.None, GameVersion.None);
        }

        public Platform TargetPlatform {
            get ; set;
        }
        void defineTargetPlatform()
        {
            if (platformTargetCombo.Items.Count > 0)
                TargetPlatform = new Platform(platformTargetCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
            else 
                TargetPlatform = new Platform(GamePlatform.None, GameVersion.None);
        }

        public DLCConverter()
        {
            InitializeComponent();
            bwConvert = new BackgroundWorker { WorkerReportsProgress = true };
        }

        private void DLCConverter_Load(object sender, EventArgs e) {
            try {
                // Fill source combo
                var sourcePlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
                sourcePlatform.Remove("None");
                platformSourceCombo.DataSource = sourcePlatform;
                platformSourceCombo.SelectedItem = ConfigRepository.Instance()["converter_source"];

                // Fill target combo
                var targetPlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
                targetPlatform.Remove("None");
                platformTargetCombo.DataSource = targetPlatform;
                platformTargetCombo.SelectedItem = ConfigRepository.Instance()["converter_target"];

                // Fill App ID
                PopulateAppIdCombo(GameVersion.RS2014); //Supported game version
                AppIdVisibilty();
            } catch { /*For mono compatibility*/ }

            // Converter worker
            bwConvert.DoWork += doConvert;
            bwConvert.ProgressChanged += (se,ea) =>
            {
                if (ea.ProgressPercentage <= updateProgress.Maximum)
                    updateProgress.Value = ea.ProgressPercentage;
                else
                    updateProgress.Value = updateProgress.Maximum;
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
                            String.Format("DLC was converted from '{2}' to '{3}' with erros. See below: {0}{1}{0}", Environment.NewLine, errorsFound.ToString(), SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                    convertButton.Enabled = true;
                    Parent.Focus();
                break;
            }

            updateProgress.Visible = false;
            currentOperationLabel.Visible = false;
        }

        private void ShowCurrentOperation(string message)
        {
            currentOperationLabel.Text = message;
            currentOperationLabel.Refresh();
        }

        private void PopulateAppIdCombo(GameVersion gameVersion)
        {
            appIdCombo.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
                appIdCombo.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select(ConfigRepository.Instance()["general_defaultappid_RS2014"], gameVersion);
            appIdCombo.SelectedItem = songAppId;
            AppId = songAppId.AppId;
        }

        private void AppIdVisibilty() {
            if (platformTargetCombo.SelectedItem != null)
            {
                var target = new Platform(platformTargetCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                appIdCombo.Enabled = !target.IsConsole;
                AppIdTB.Enabled = !target.IsConsole;
            }
        }

        private void platformTargetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppIdVisibilty();
        }

        private void appIdCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (appIdCombo.SelectedItem != null)
                AppId = ((SongAppId)appIdCombo.SelectedItem).AppId;
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

                try {
                // CONVERT
                var output = DLCPackageConverter.Convert(sourcePackage, SourcePlatform, TargetPlatform, AppId);
                if(!String.IsNullOrEmpty(output))
                    errorsFound.AppendLine(output);
                }
                catch(Exception ex) {
                    errorsFound.AppendLine(String.Format("{0}\n{1}\n",ex.Message, ex.StackTrace));
                }
                progress += step;
                bwConvert.ReportProgress(progress);
            }
            bwConvert.ReportProgress(100);
            e.Result = "done";
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            defineSourcePlatform();
            defineTargetPlatform();
            // VALIDATIONS
            if (SourcePlatform.Equals(TargetPlatform)) {
                MessageBox.Show("The source and target platform should be different.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // GET FILES
            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select one DLC for platform conversion";
                ofd.Multiselect = true;
                switch (SourcePlatform.platform) {
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
                    updateProgress.Value = 0;
                    updateProgress.Visible = true;
                    currentOperationLabel.Visible = true;
                    convertButton.Enabled = false;
                    bwConvert.RunWorkerAsync(ofd.FileNames);
                }
            }
        }
    }
}
