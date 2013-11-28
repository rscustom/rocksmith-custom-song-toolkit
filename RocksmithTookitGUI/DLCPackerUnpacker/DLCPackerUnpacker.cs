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

        public DLCPackerUnpacker()
        {
            InitializeComponent();

            var gameVersionList = Enum.GetNames(typeof(GameVersion)).ToList<string>();
            gameVersionList.Remove("None");
            gameVersionCombo.DataSource = gameVersionList;
            gameVersionCombo.SelectedItem = GameVersion.RS2012;

            GameVersion gameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), gameVersionCombo.SelectedItem.ToString());
            PopulateAppIdCombo(gameVersion);
        }

        private void packButton_Click(object sender, EventArgs e)
        {
            string sourcePath;
            string saveFileName;
            var updateSng = updateSngCheckBox.Checked;

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
                string[] decodedAudioFiles = Directory.GetFiles(sourcePath, "*_fixed.ogg", SearchOption.AllDirectories);
                foreach (var file in decodedAudioFiles)
                    File.Delete(file);
                Packer.Pack(sourcePath, saveFileName, (platform.platform == GamePlatform.Pc) ? true : false, updateSng);
                MessageBox.Show("Packing is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\n\r{1}\n\r{2}", "Packing error!", ex.Message, ex.InnerException), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void unpackButton_Click(object sender, EventArgs e)
        {
            IList<string> sourceFileNames;
            string savePath;
            var decodeAudio = decodeAudioCheckbox.Checked;

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
                Packer.Unpack(sourceFileName, savePath, (platform.platform == GamePlatform.Pc) ? true : false); // Cryptography way is used only for PC in Rocksmith 1

                if (decodeAudio) {
                    try
                    {
                        var name = Path.GetFileNameWithoutExtension(sourceFileName);
                        if (platform.platform == GamePlatform.PS3)
                            name = name.Substring(0, name.LastIndexOf("."));
                        name += String.Format("_{0}", platform.platform.ToString());
                        var audioFiles = Directory.GetFiles(Path.Combine(savePath, name), "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wem"));

                        foreach (var file in audioFiles)
                        {
                            var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                            OggFile.Revorb(file, outputFileName, Path.GetDirectoryName(Application.ExecutablePath), Path.GetExtension(file).GetWwiseVersion());
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        errorsFound.AppendLine(ex.Message);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        errorsFound.AppendLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        errorsFound.AppendLine(ex.Message);
                    }
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
            var updateSng = updateSngCheckBox.Checked;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Custom Rocksmith DLC PC (*.dat;*.psarc)|*.dat;*.psarc";
                ofd.Title = "Select one or more PC DLC files to update";
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

                if (platform.platform == GamePlatform.Pc)
                {
                    var useCryptography = (platform.version == GameVersion.RS2012) ? true : false;
                    Packer.Unpack(sourceFileName, tmpDir, useCryptography);

                    var unpackedDir = tmpDir + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(sourceFileName) + String.Format("_{0}", platform.platform);

                    var appIdFile = Path.Combine(unpackedDir, (platform.version == GameVersion.RS2012) ? "APP_ID" : "appid.appid");

                    File.WriteAllText(appIdFile, appId);

                    Packer.Pack(unpackedDir, sourceFileName, useCryptography, updateSng);
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
            SongAppId firstSong = null;
            appIdCombo.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
            {
                appIdCombo.Items.Add(song);
                if (firstSong == null)
                {
                    firstSong = song;
                }
            }
            appIdCombo.SelectedItem = firstSong;
            AppIdTB.Text = firstSong.AppId;
        }
    }
}
