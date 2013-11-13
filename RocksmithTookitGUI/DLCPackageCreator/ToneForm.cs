using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.Tone;
using System.Runtime.Serialization;
using System.Xml;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneForm : Form
    {
        private const string MESSAGEBOX_CAPTION = "DLC Package Creator";

        public bool Saved = false;
        public dynamic LoadedTone = null;
        private GameVersion CurrentGameVersion;

        private string CurrentOFDFilter {
            get {
                switch (CurrentGameVersion) {
                    case GameVersion.RS2014:
                        return "Rocksmith 2014 Tone Template (*.tone2014.xml)|*.tone2014.xml";
                    default:
                        return "Rocksmith Tone Template (*.tone.xml)|*.tone.xml";
                }
            }
        }

        public ToneForm(dynamic tone, GameVersion gameVersion)
        {
            CurrentGameVersion = gameVersion;
            InitializeComponent();
            RecreateToneControl(tone);
        }

        private void RecreateToneControl(dynamic tone) {
            // Recreate toneControl from CurrentGameVersion
            Controls.Remove(toneControl1);
            toneControl1 = new RocksmithToolkitGUI.DLCPackageCreator.ToneControl(CurrentGameVersion);
            this.toneControl1.Location = new System.Drawing.Point(10, 11);
            this.toneControl1.Name = "toneControl1";
            this.toneControl1.Size = new System.Drawing.Size(512, 268);
            this.toneControl1.TabIndex = 0;
            toneControl1.Tone = tone;
            Controls.Add(toneControl1); 
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Saved = true;
            Close();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            string toneSavePath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = CurrentOFDFilter;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneSavePath = ofd.FileName;
            }

            dynamic tone = null;
            try
            {
                switch (CurrentGameVersion) {
                    case GameVersion.RS2012:
                        tone = Tone.LoadFromFile(toneSavePath);
                        break;
                    case GameVersion.RS2014:
                        tone = Tone2014.LoadFromFile(toneSavePath);
                        break;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't load saved tone. \n\r" + ex.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            toneControl1.Tone = tone;
            LoadedTone = toneControl1.Tone;

            MessageBox.Show("Tone was loaded.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string toneSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = CurrentOFDFilter;
                ofd.AddExtension = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneSavePath = ofd.FileName;
            }

            var tone = toneControl1.Tone;
            tone.Serialize(toneSavePath);

            MessageBox.Show("Tone was saved.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
