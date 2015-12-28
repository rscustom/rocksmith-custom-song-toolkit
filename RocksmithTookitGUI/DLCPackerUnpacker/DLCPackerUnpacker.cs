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
using RocksmithToolkitLib.DLCPackage.AggregateGraph2014;
using RocksmithToolkitLib.Extensions;


namespace RocksmithToolkitGUI.DLCPackerUnpacker
{
    public partial class DLCPackerUnpacker : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "CDLC Packer/Unpacker";
        private BackgroundWorker bwRepack = new BackgroundWorker();
        private StringBuilder errorsFound;
        private string savePath;

        public static Label CurrentOperationLabel { get; set; }
        public static ProgressBar UpdateProgress { get; set; }

        public DLCPackerUnpacker()
        {
            InitializeComponent();

            try
            {
                var gameVersionList = Enum.GetNames(typeof(GameVersion)).ToList<string>();
                gameVersionList.Remove("None");
                cmbGameVersion.DataSource = gameVersionList;
                cmbGameVersion.SelectedItem = ConfigRepository.Instance()["general_defaultgameversion"];
                GameVersion gameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), cmbGameVersion.SelectedItem.ToString());
                PopulateAppIdCombo(gameVersion);
            }
            catch { /*For mono compatibility*/ }

            // App ID updater worker
            bwRepack.DoWork += new DoWorkEventHandler(UpdateAppId);
            bwRepack.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            bwRepack.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            bwRepack.WorkerReportsProgress = true;
        }

        private bool decodeAudio
        {
            get { return chkDecodeAudio.Checked; }
        }

        private bool extractSongXml
        {
            get { return chkExtractSongXml.Checked; }
        }

        private bool updateSng
        {
            get { return chkUpdateSng.Checked; }
        }

        private void PopulateAppIdCombo(GameVersion gameVersion)
        {
            cmbAppId.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
                cmbAppId.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select((gameVersion == GameVersion.RS2014) ? ConfigRepository.Instance()["general_defaultappid_RS2014"] : ConfigRepository.Instance()["general_defaultappid_RS2012"], gameVersion);
            cmbAppId.SelectedItem = songAppId;
            txtAppId.Text = songAppId.AppId;
        }

        private void ShowCurrentOperation(string message)
        {
            lblCurrentOperation.Text = message;
            lblCurrentOperation.Refresh();
        }

        private void UnpackSongs(IEnumerable<string> sourceFileNames, string destPath, bool decode = false, bool extract = false)
        {
            btnSelectSongs.Enabled = false;
            errorsFound = new StringBuilder();
            GlobalExtension.UpdateProgress = this.pbUpdateProgress;
            GlobalExtension.CurrentOperationLabel = this.lblCurrentOperation;
            Thread.Sleep(100); // give Globals a chance to initialize
            Stopwatch sw = new Stopwatch();
            sw.Restart();

            foreach (string sourceFileName in sourceFileNames)
            {
                Application.DoEvents();
                Platform platform = Packer.GetPlatform(sourceFileName);
                GlobalExtension.ShowProgress(String.Format("Unpacking '{0}'", Path.GetFileName(sourceFileName)));

                try
                {
                    Packer.Unpack(sourceFileName, destPath, decode, extract);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error unpacking file '{0}': {1}", Path.GetFileName(sourceFileName), ex.Message));
                }
            }

            sw.Stop();
            GlobalExtension.ShowProgress("Finished unpacking archive (elapsed time): " + sw.Elapsed, 100);

            if (errorsFound.Length > 0)
                MessageBox.Show("Unpacking is complete with errors. See below: " + Environment.NewLine + Environment.NewLine + errorsFound.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Unpacking is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnSelectSongs.Enabled = true;
            // prevents possible cross threading
            GlobalExtension.Dispose();
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
                    btnRepackAppId.Enabled = true;
                    break;
                case "unpack":
                    if (errorsFound.Length <= 0)
                        MessageBox.Show("Unpacking is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Unpacking is complete with errors. See below: " + Environment.NewLine + Environment.NewLine + errorsFound.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnUnpack.Enabled = true;
                    break;
            }

            pbUpdateProgress.Visible = false;
            lblCurrentOperation.Visible = false;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= pbUpdateProgress.Maximum)
                pbUpdateProgress.Value = e.ProgressPercentage;
            else
                pbUpdateProgress.Value = pbUpdateProgress.Maximum;

            ShowCurrentOperation(e.UserState as string);
        }

        private void UpdateAppId(object sender, DoWorkEventArgs e)
        {
            var sourceFileNames = e.Argument as string[];
            errorsFound = new StringBuilder();
            var step = (int)Math.Round(1.0 / sourceFileNames.Length * 100, 0);
            int progress = 0;

            var tmpDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
            var appId = txtAppId.Text;

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

        private void btnLowTuningBassFix_Click(object sender, EventArgs e)
        {
            dlcPackageCreatorControl.dlcLowTuningBassFix(sender, e, btnLowTuningBassFix, chkQuickBassFix.Checked, chkDeleteSourceFile.Checked);
        }

        private void btnPackSongPack_Click(object sender, EventArgs e)
        {
            string sourcePath;
            string saveFileName;

            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.Description = "Select the Song Pack folder";
                fbd.SelectedPath = savePath;

                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                sourcePath = fbd.SelectedPath;
            }

            saveFileName = Path.GetFileName(sourcePath);

            //using (var sfd = new SaveFileDialog())
            //{
            //    sfd.FileName = Path.GetFileName(sourcePath);

            //    if (sfd.ShowDialog() != DialogResult.OK)
            //        return;

            //    saveFileName = sfd.FileName;
            //}

            GlobalExtension.UpdateProgress = this.pbUpdateProgress;
            GlobalExtension.CurrentOperationLabel = this.lblCurrentOperation;
            Thread.Sleep(100); // give Globals a chance to initialize
            GlobalExtension.ShowProgress("Packing archive ...");
            Application.DoEvents();

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Restart();

                var songPackDir = AggregateGraph2014.DoLikeSongPack(sourcePath, txtAppId.Text);
                var destFilePath = Path.Combine(Path.GetDirectoryName(sourcePath), String.Format("{0}_p.psarc", Path.GetFileName(sourcePath)));
                Packer.Pack(songPackDir, destFilePath, fixShowlights: false, predefinedPlatform: new Platform(GamePlatform.Pc, GameVersion.RS2014));

                // clean up now (song pack folder)
                if (Directory.Exists(songPackDir))
                    DirectoryExtension.SafeDelete(songPackDir);

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

        private void btnPack_Click(object sender, EventArgs e)
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

            GlobalExtension.UpdateProgress = this.pbUpdateProgress;
            GlobalExtension.CurrentOperationLabel = this.lblCurrentOperation;
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

        private void btnRepackAppId_Click(object sender, EventArgs e)
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
                    pbUpdateProgress.Value = 0;
                    pbUpdateProgress.Visible = true;
                    lblCurrentOperation.Visible = true;
                    btnRepackAppId.Enabled = false;
                    bwRepack.RunWorkerAsync(ofd.FileNames);
                }
            }
        }

        private void btnSelectSongs_Click(object sender, EventArgs e)
        {
            string[] sourceFileNames;
            savePath = String.Empty;


            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "All Files (*.psarc)|*.psarc";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileNames = ofd.FileNames;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Path.GetDirectoryName(sourceFileNames[0]) + Path.DirectorySeparatorChar;
                fbd.Description = "Select 'Make New Folder' then right click to 'Rename'" + Environment.NewLine + "the 'New Folder' to the desired Song Pack name." + Environment.NewLine + "Make sure the new Song Pack folder stays selected then click Ok.";

                if (fbd.ShowDialog() != DialogResult.OK) return;
                savePath = fbd.SelectedPath;
            }

            UnpackSongs(sourceFileNames, savePath);
        }

        private void btnUnpack_Click(object sender, EventArgs e)
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

            UnpackSongs(sourceFileNames, savePath, decodeAudio, extractSongXml);
        }

        private void cmbAppIds_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbAppId.SelectedItem != null)
                txtAppId.Text = ((SongAppId)cmbAppId.SelectedItem).AppId;
        }

        private void cmbGameVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameVersion gameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), cmbGameVersion.SelectedItem.ToString());
            PopulateAppIdCombo(gameVersion); ;
        }

     }
}
