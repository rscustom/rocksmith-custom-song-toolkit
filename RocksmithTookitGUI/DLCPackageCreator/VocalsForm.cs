using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Ookii.Dialogs;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Tone;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class VocalsForm : Form
    {
        public string SngPath { get; set; }
        public string ArtPath { get; set; }

        public VocalsForm( string fontSng, string lyricArtPath )
        {
            InitializeComponent();
            SngPathCTB.Text = SngPath = fontSng;
            ArtPathCTB.Text = ArtPath = lyricArtPath;
        }
        
        void OkButton_Click(object sender, EventArgs e)
        {
            if(File.Exists(SngPath) && File.Exists(ArtPath))
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else {
                MessageBox.Show("One of required files are missing, please select both required files and try again.\r\n", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
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
                    ArtPath = f.FileName;
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
                    ArtPath = f.FileName;
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
    }
}
