using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RocksmithDLCPackager
{
    public partial class PackerForm : Form
    {
        private readonly Packer packer;

        public PackerForm()
        {
            InitializeComponent();
            packer = new Packer();
        }

        private void PackButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            var sourcePath = fbd.SelectedPath;
            var saveFileName = sfd.FileName;
            var useCryptography = UseCryptographyCheckbox.Checked;

            packer.Pack(sourcePath, saveFileName, useCryptography);
        }

        private void UnpackButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            var sourceFileName = ofd.FileName;
            var savePath = fbd.SelectedPath;
            var useCryptography = UseCryptographyCheckbox.Checked;

            packer.Unpack(sourceFileName, savePath, useCryptography);
        }
    }
}
