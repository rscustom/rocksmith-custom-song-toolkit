using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ookii.Dialogs;
using RocksmithToolkitLib.DLCPackage;
using System.IO;

namespace RocksmithTookitGUI.DLCPackerUnpacker
{
    public partial class DLCPackerUnpacker : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "DLC Packer";

        public DLCPackerUnpacker()
        {
            InitializeComponent();

            SongAppId firstSong = null;
            foreach (var song in SongAppId.GetSongAppIds())
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
                Packer.Pack(sourcePath, saveFileName, useCryptography);
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

            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Custom DLC|*.dat";
                ofd.Title = "Select one or more DLC files to update";
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

                var unpackedDir = tmpDir + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(sourceFileName) + Packer.ADD_PC;
                var appIdFile = unpackedDir + Path.DirectorySeparatorChar + "APP_ID";
                File.WriteAllText(appIdFile, appId);

                Packer.Pack(unpackedDir, sourceFileName, useCryptography);
            }

            MessageBox.Show("APP ID update is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
