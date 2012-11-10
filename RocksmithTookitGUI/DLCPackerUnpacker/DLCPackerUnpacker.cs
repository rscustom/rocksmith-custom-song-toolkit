using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithDLCPackager;

namespace RocksmithTookitGUI.DLCPackerUnpacker
{
    public partial class DLCPackerUnpacker : UserControl
    {
        private readonly Packer packer;

        public DLCPackerUnpacker()
        {
            InitializeComponent();
            packer = new Packer();
        }

        private void packButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            var sourcePath = fbd.SelectedPath;
            var saveFileName = sfd.FileName;
            var useCryptography = useCryptographyCheckbox.Checked;

            packer.Pack(sourcePath, saveFileName, useCryptography);
        }

        private void unpackButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            var sourceFileName = ofd.FileName;
            var savePath = fbd.SelectedPath;
            var useCryptography = useCryptographyCheckbox.Checked;

            packer.Unpack(sourceFileName, savePath, useCryptography);
        }
    }
}
