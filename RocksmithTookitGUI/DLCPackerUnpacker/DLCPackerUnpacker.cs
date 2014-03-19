using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ookii.Dialogs;
using RocksmithToolkitLib.DLCPackage;
using System.IO;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;

namespace RocksmithToolkitGUI.DLCPackerUnpacker
{
    public partial class DLCPackerUnpacker : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "DLC Packer/Unpacker";

        private bool decodeAudio {
            get { return decodeAudioCheckbox.Checked; }
        }

        private bool extractSongXml {
            get { return extractSongXmlCheckBox.Checked; }
        }

        private bool updateSng {
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
                gameVersionCombo.SelectedItem = ConfigRepository.Instance()["creator_gameversion"];
                GameVersion gameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), gameVersionCombo.SelectedItem.ToString());
                PopulateAppIdCombo(gameVersion);
            }
            catch { /*For mono compatibility*/ }
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

            try
            {
                var platform = sourcePath.GetPlatform();
                Packer.Pack(sourcePath, saveFileName, updateSng);
                MessageBox.Show("Packing is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\n{1}\n{2}", "Packing error!", ex.Message, ex.InnerException), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void unpackButton_Click(object sender, EventArgs e)
        {
            IList<string> sourceFileNames;
            string savePath;
            
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileNames = ofd.FileNames;
            }
            using (var fbd = new VistaFolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                savePath = fbd.SelectedPath;
            }

            StringBuilder errorsFound = new StringBuilder();

            foreach (string sourceFileName in sourceFileNames)
            {
                Application.DoEvents();
                Platform platform = Packer.GetPlatform(sourceFileName);
                
                try {
                    Packer.Unpack(sourceFileName, savePath, decodeAudio, extractSongXml);
                }
                catch (Exception ex) {
                    errorsFound.AppendLine(ex.Message);
                }
            }

            if (errorsFound.Length <= 0)
                MessageBox.Show("Unpacking is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Unpacking is complete with errors. See below: " + Environment.NewLine + errorsFound.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void cmbAppIds_SelectedValueChanged(object sender, EventArgs e)
        {
            if (appIdCombo.SelectedItem != null)
            {
                AppIdTB.Text = ((SongAppId)appIdCombo.SelectedItem).AppId;
            }
        }

        private void repackButton_Click(object sender, EventArgs e)
        {
            IList<string> sourceFileNames;
            
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Custom Rocksmith/Rocksmith2014 DLC (*.dat;*.psarc)|*.dat;*.psarc";
                ofd.Title = "Select one or more DLC files to update";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileNames = ofd.FileNames;
            }

            var tmpDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
            var appId = AppIdTB.Text;

            StringBuilder errorsFound = new StringBuilder();

            foreach (string sourceFileName in sourceFileNames)
            {
                Application.DoEvents();
                var platform = sourceFileName.GetPlatform();

                if (platform.platform == GamePlatform.Pc || platform.platform == GamePlatform.Mac)
                {
                    var unpackedDir = Packer.Unpack(sourceFileName, tmpDir);

                    var appIdFile = Path.Combine(unpackedDir, (platform.version == GameVersion.RS2012) ? "APP_ID" : "appid.appid");

                    File.WriteAllText(appIdFile, appId);

                    Packer.Pack(unpackedDir, sourceFileName, updateSng);
                }
                else
                {
                    errorsFound.AppendLine(String.Format("File '{0}' is not PC platform and have no App Id to change.", sourceFileName));
                }
            }

            if (errorsFound.Length <= 0)
                MessageBox.Show("APP ID update is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("APP ID update is complete with errors. See below: " + Environment.NewLine + errorsFound.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            
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
    }
}
