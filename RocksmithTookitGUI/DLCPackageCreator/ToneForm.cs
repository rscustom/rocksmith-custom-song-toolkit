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

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneForm : Form
    {
        public bool Saved = false;
        public Tone LoadedTone = null;

        public ToneForm(Tone tone)
        {
            InitializeComponent();
            toneControl1.Tone = tone;
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
                ofd.Filter = "Rocksmith Tone (*.tone.xml)|*.tone.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneSavePath = ofd.FileName;
            }

            Tone tone = null;

            var serializer = new DataContractSerializer(typeof(Tone));
            using (var stm = new XmlTextReader(toneSavePath))
            {
                try
                {
                    tone = (Tone)serializer.ReadObject(stm);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't load saved tone. \n\r" + ex.Message, "DLCPackageCreator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            toneControl1.Tone = tone;
            LoadedTone = toneControl1.Tone;

            MessageBox.Show("Tone was loaded.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string toneSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = "Rocksmith DLC Template (*.tone.xml)|*.tone.xml";
                ofd.AddExtension = true;
                ofd.DefaultExt = "tone.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneSavePath = ofd.FileName;
            }

            var tone = toneControl1.Tone;
            var serializer = new DataContractSerializer(typeof(Tone));
            using (var stm = XmlWriter.Create(toneSavePath, new XmlWriterSettings() { CheckCharacters = true, Indent = true }))
            {
                serializer.WriteObject(stm, tone);
            }

            MessageBox.Show("Tone was saved.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
