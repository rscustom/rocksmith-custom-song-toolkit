using Ookii.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class VocalsForm : Form
    {
        public string SngPath { get; set; }
        public string ArtPath { get; set; }
        public bool IsCustom { get; set; }

        public VocalsForm( string fontSng, string lyricArtPath, bool isCustom )
        {
            InitializeComponent();
            SngPathCTB.Text = SngPath = fontSng;
            ArtPathCTB.Text = ArtPath = lyricArtPath;
            isCustomCB.Checked = IsCustom = isCustom;
        }

        void OkButton_Click(object sender, EventArgs e)
        {
            if(File.Exists(SngPath) && File.Exists(ArtPath) || !IsCustom)
            {
                if (!IsCustom)
                    SngPath = "";
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else {
                MessageBox.Show("One of required files are missing, please select both required files and try again.\r\n", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void SngpathFD_Click(object sender, EventArgs e)
        {
            using (var f = new VistaOpenFileDialog())
            {
                f.FileName = SngPath;
                f.Filter = "Rocksmith Song Files (*.sng)|*.sng";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    SngPathCTB.Text = ArtPath = f.FileName;
                    isCustomCB.Checked = IsCustom = true; //possible jvocals and regular vocals, but supported only one custom font texture
                }
            }
        }

        void ArtpathFD_Click(object sender, EventArgs e)
        {
            using (var f = new VistaOpenFileDialog())
            {
                f.FileName = ArtPath;
                f.Filter = "Rocksmith DDS Art Files (*.dds)|*.dds";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    ArtPathCTB.Text = ArtPath = f.FileName;
                }
            }
        }

        void SngPathCTB_TextChanged(object sender, EventArgs e)
        {
            var name = (TextBox)sender;
            SngPath = name.Text;
        }

        void ArtPathCTB_TextChanged(object sender, EventArgs e)
        {
            var name = (TextBox)sender;
            ArtPath = name.Text;
        }

        void IsCustomCB_CheckedChanged(object sender, EventArgs e)
        {
            IsCustom = isCustomCB.Checked;
        }
    }
}
