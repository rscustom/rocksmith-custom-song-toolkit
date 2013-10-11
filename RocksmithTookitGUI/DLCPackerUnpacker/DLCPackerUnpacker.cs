using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ookii.Dialogs;
using RocksmithToolkitLib.DLCPackage;
using System.IO;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitGUI.DLCPackerUnpacker
{
    public partial class DLCPackerUnpacker : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "DLC Packer";

        public DLCPackerUnpacker()
        {
            InitializeComponent();

            SongAppId firstSong = null;
            foreach (var song in SongAppIdRepository.Instance().List)
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

        private void packButton_Click(object sender, EventArgs e)
        {
            string sourcePath;
            string saveFileName;
            var useCryptography = useCryptographyCheckbox.Checked;
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
                string[] decodedOGGFiles = Directory.GetFiles(sourcePath, "*_fixed.ogg", SearchOption.AllDirectories);
                foreach (var file in decodedOGGFiles)
                    File.Delete(file);
                Packer.Pack(sourcePath, saveFileName, useCryptography, updateSng);
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
            var useCryptography = useCryptographyCheckbox.Checked;
            var decodeOGG = decodeOGGCheckbox.Checked;

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

            foreach (string sourceFileName in sourceFileNames)
            {
                Application.DoEvents();
                Packer.Unpack(sourceFileName, savePath, useCryptography);

                if (decodeOGG) {
                    GamePlatform platform = Packer.GetPlatform(sourceFileName);
                    var name = Path.GetFileNameWithoutExtension(sourceFileName);
                    name += String.Format("_{0}", platform.ToString());
                    string[] oggFiles = Directory.GetFiles(Path.Combine(savePath, name), "*.ogg", SearchOption.AllDirectories);
                    foreach (var file in oggFiles)
                    {
                        var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                        OggConverter.OggConverter.Revorb(file, outputFileName);
                    }
                }
            }

            MessageBox.Show("Unpacking is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var useCryptography = useCryptographyCheckbox.Checked;
            var updateSng = updateSngCheckBox.Checked;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Custom DLC|*.dat";
                ofd.Title = "Select one or more PC DLC files to update";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileNames = ofd.FileNames;
            }

            var tmpDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
            var appId = AppIdTB.Text;

            foreach (string sourceFileName in sourceFileNames)
            {
                Application.DoEvents();
                Packer.Unpack(sourceFileName, tmpDir, useCryptography);

                var unpackedDir = tmpDir + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(sourceFileName) + String.Format("_{0}", GamePlatform.Pc);
                var appIdFile = unpackedDir + Path.DirectorySeparatorChar + "APP_ID";
                File.WriteAllText(appIdFile, appId);

                Packer.Pack(unpackedDir, sourceFileName, useCryptography, updateSng);
            }

            MessageBox.Show("APP ID update is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
