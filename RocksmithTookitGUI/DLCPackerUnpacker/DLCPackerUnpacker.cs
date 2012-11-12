using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ookii.Dialogs;
using RocksmithToolkitLib.DLCPackage;

namespace RocksmithTookitGUI.DLCPackerUnpacker
{
    public partial class DLCPackerUnpacker : UserControl
    {
        public DLCPackerUnpacker()
        {
            InitializeComponent();
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

            Packer.Pack(sourcePath, saveFileName, useCryptography);

            MessageBox.Show("Packing is complete.", "DLC Packer");
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

            MessageBox.Show("Unpacking is complete.", "DLC Packer");
        }
    }
}
