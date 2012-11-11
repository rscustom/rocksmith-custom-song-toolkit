using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

            using (var fbd = new FolderBrowserDialog())
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
        }

        private void unpackButton_Click(object sender, EventArgs e)
        {
            string sourceFileName;
            string savePath;
            var useCryptography = useCryptographyCheckbox.Checked;

            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileName = ofd.FileName;
            }
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                savePath = fbd.SelectedPath;
            }

            Packer.Unpack(sourceFileName, savePath, useCryptography);
        }
    }
}
