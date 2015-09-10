using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using Ookii.Dialogs;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;

namespace RocksmithToolkitGUI.DLCPackerUnpacker
{
    public partial class DLCPackerUnpacker : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "DLC Packer/Unpacker";
        private BackgroundWorker bwRepack = new BackgroundWorker();
        private BackgroundWorker bwUnpack = new BackgroundWorker();
        private StringBuilder errorsFound;
        private string savePath;

        public static ProgressBar UpdateProgress { get; set; }
        public static Label CurrentOperationLabel { get; set; }

        private bool decodeAudio
        {
            get { return decodeAudioCheckbox.Checked; }
        }

        private bool extractSongXml
        {
            get { return extractSongXmlCheckBox.Checked; }
        }

        private bool updateSng
        {
            get { return updateSngCheckBox.Checked; }
        }

        public DLCPackerUnpacker()
        {
            InitializeComponent();

            try
            {
                var gameVersionList = Enum.GetNames(typeof(GameVersion)).ToList<string>();
                gameVersionList.Remove("None");
                gameVersionCombo.DataSource = gameVersionList;
                gameVersionCombo.SelectedItem = ConfigRepository.Instance()["general_defaultgameversion"];
                GameVersion gameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), gameVersionCombo.SelectedItem.ToString());
                PopulateAppIdCombo(gameVersion);
            }
            catch { /*For mono compatibility*/ }

            // App ID updater worker
            bwRepack.DoWork += new DoWorkEventHandler(UpdateAppId);
            bwRepack.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            bwRepack.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            bwRepack.WorkerReportsProgress = true;

            // commented out bWorker aspects to test GlobalExtension ProgressBar function
            //// Upack worker
            //bwUnpack.DoWork += new DoWorkEventHandler(Unpack);
            //bwUnpack.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            //bwUnpack.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            //bwUnpack.WorkerReportsProgress = true;
        }

        private void cmbAppIds_SelectedValueChanged(object sender, EventArgs e)
        {
            if (appIdCombo.SelectedItem != null)
                AppIdTB.Text = ((SongAppId)appIdCombo.SelectedItem).AppId;
        }

        private void gameVersionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameVersion gameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), gameVersionCombo.SelectedItem.ToString());
            PopulateAppIdCombo(gameVersion); ;
        }

        private void PopulateAppIdCombo(GameVersion gameVersion)
        {
            appIdCombo.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
                appIdCombo.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select((gameVersion == GameVersion.RS2014) ? ConfigRepository.Instance()["general_defaultappid_RS2014"] : ConfigRepository.Instance()["general_defaultappid_RS2012"], gameVersion);
            appIdCombo.SelectedItem = songAppId;
            AppIdTB.Text = songAppId.AppId;
        }

        public void SetProgress(int progress, int maxValue)
        {
            var step = (int)Math.Round(1.0 / (maxValue + 1) * 100, 0);
            updateProgress.Value = (int)(progress / step);
        }

        private void packButton_Click(object sender, EventArgs e)
        {
            string sourcePath;
            string saveFileName;

            using (var fbd = new VistaFolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                sourcePath = fbd.SelectedPath;
            }

            using (var sfd = new SaveFileDialog())
            {
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                saveFileName = sfd.FileName;
            }
      
            GlobalExtension.UpdateProgress = this.updateProgress;
            GlobalExtension.CurrentOperationLabel = this.currentOperationLabel;
            Thread.Sleep(100); // give Globals a chance to initialize
            GlobalExtension.ShowProgress("Packing archive ...");            
            Application.DoEvents();

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                Packer.Pack(sourcePath, saveFileName, updateSng);
                sw.Stop();
                GlobalExtension.ShowProgress("Finished packing archive (elapsed time): " + sw.Elapsed, 100);
                MessageBox.Show("Packing is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\n{1}\n{2}", "Packing error!", ex.Message, ex.InnerException), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // prevents possible cross threading
            GlobalExtension.Dispose();
        }

        private void unpackButton_Click(object sender, EventArgs e)
        {
            string[] sourceFileNames;
            savePath = String.Empty;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileNames = ofd.FileNames;
            }

            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.SelectedPath = Path.GetDirectoryName(sourceFileNames[0]) + Path.DirectorySeparatorChar;
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                savePath = fbd.SelectedPath;
            }

            // commented out bWorker aspects to test GlobalExtension ProgressBar function
            //if (!bwUnpack.IsBusy && sourceFileNames.Length > 0)
            //{
            //    updateProgress.Value = 0;
            //    updateProgress.Visible = true;
            //    currentOperationLabel.Visible = true;
            unpackButton.Enabled = false;
            //    bwUnpack.RunWorkerAsync(sourceFileNames);
            //}
            //}

            //private void Unpack(object sender, DoWorkEventArgs e)
            //{
            //    var sourceFileNames = e.Argument as string[];
            errorsFound = new StringBuilder();
            //var step = (int)Math.Round(1.0 / sourceFileNames.Length * 100, 0);
            //int progress = 0;

            GlobalExtension.UpdateProgress = this.updateProgress;
            GlobalExtension.CurrentOperationLabel = this.currentOperationLabel;
            Thread.Sleep(100); // give Globals a chance to initialize

            foreach (string sourceFileName in sourceFileNames)
            {
                Application.DoEvents();
                Platform platform = Packer.GetPlatform(sourceFileName);
                // bwUnpack.ReportProgress(progress, String.Format("Unpacking '{0}'", Path.GetFileName(sourceFileName)));

                GlobalExtension.ShowProgress(String.Format("Unpacking '{0}'", Path.GetFileName(sourceFileName)));

                // remove this exception handler for testing
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Restart();
                    Packer.Unpack(sourceFileName, savePath, decodeAudio, extractSongXml);
                    sw.Stop();
                    GlobalExtension.ShowProgress("Finished unpacking archive (elapsed time): " + sw.Elapsed, 100);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error unpacking file '{0}': {1}", Path.GetFileName(sourceFileName), ex.Message));
                }

                // progress += step;
                // bwUnpack.ReportProgress(progress);
            }
            //  bwUnpack.ReportProgress(100);
            //  e.Result = "unpack";

            // add this message while bWorker is commented out
            if (errorsFound.Length <= 0)
                MessageBox.Show("Unpacking is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Unpacking is complete with errors. See below: " + Environment.NewLine + Environment.NewLine + errorsFound.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            unpackButton.Enabled = true;
            // prevents possible cross threading
            GlobalExtension.Dispose();
        }

        private void repackButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Custom Rocksmith/Rocksmith2014 DLC (*.dat;*.psarc)|*.dat;*.psarc";
                ofd.Title = "Select one or more DLC files to update";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                if (!bwRepack.IsBusy && ofd.FileNames.Length > 0)
                {
                    updateProgress.Value = 0;
                    updateProgress.Visible = true;
                    currentOperationLabel.Visible = true;
                    repackButton.Enabled = false;
                    bwRepack.RunWorkerAsync(ofd.FileNames);
                }
            }
        }

        private void UpdateAppId(object sender, DoWorkEventArgs e)
        {
            var sourceFileNames = e.Argument as string[];
            errorsFound = new StringBuilder();
            var step = (int)Math.Round(1.0 / sourceFileNames.Length * 100, 0);
            int progress = 0;

            var tmpDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
            var appId = AppIdTB.Text;

            foreach (string sourceFileName in sourceFileNames)
            {
                Application.DoEvents();
                var platform = sourceFileName.GetPlatform();
                bwRepack.ReportProgress(progress, String.Format("Updating '{0}'", Path.GetFileName(sourceFileName)));

                if (!platform.IsConsole)
                {
                    try
                    {
                        var unpackedDir = Packer.Unpack(sourceFileName, tmpDir);
                        var appIdFile = Path.Combine(unpackedDir, (platform.version == GameVersion.RS2012) ? "APP_ID" : "appid.appid");
                        File.WriteAllText(appIdFile, appId);
                        Packer.Pack(unpackedDir, sourceFileName, updateSng);
                    }
                    catch (Exception ex)
                    {
                        errorsFound.AppendLine(String.Format("Error trying repack file '{0}': {1}", Path.GetFileName(sourceFileName), ex.Message));
                    }

                    progress += step;
                    bwRepack.ReportProgress(progress);
                }
                else
                    errorsFound.AppendLine(String.Format("File '{0}' is not a valid package for desktop platform.", Path.GetFileName(sourceFileName)));
            }
            bwRepack.ReportProgress(100);
            e.Result = "repack";
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
            switch (Convert.ToString(e.Result))
            {
                case "repack":
                    if (errorsFound.Length <= 0)
                        MessageBox.Show("APP ID update is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("APP ID update is complete with errors. See below: " + Environment.NewLine + errorsFound.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    repackButton.Enabled = true;
                    break;
                case "unpack":
                    if (errorsFound.Length <= 0)
                        MessageBox.Show("Unpacking is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Unpacking is complete with errors. See below: " + Environment.NewLine + Environment.NewLine + errorsFound.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    unpackButton.Enabled = true;
                    break;
            }

            updateProgress.Visible = false;
            currentOperationLabel.Visible = false;
        }

        public void ShowCurrentOperation(string message)
        {
            currentOperationLabel.Text = message;
            currentOperationLabel.Refresh();
        }

        private void lowTuningBassFixButton_Click(object sender, EventArgs e)
        {
            dlcPackageCreatorControl.dlcLowTuningBassFix(sender, e, lowTuningBassFixButton, quickBassFixBox.Checked, deleteSourceFileCheckBox.Checked);
        }


    }
}
