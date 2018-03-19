using Ookii.Dialogs;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Forms;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XML;
using System.Text.RegularExpressions;


namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class VocalsForm : Form
    {
        public VocalsForm(string fontSngPath, string lyricArtPath, bool isCustom, string vocalsXmlPath)
        {
            InitializeComponent();
            SngPath = fontSngPath;
            ArtPath = lyricArtPath;
            IsCustom = isCustom;
            VocalsPath = vocalsXmlPath;
            PopulateKey();
            PopulateRichText();
        }

        public string ArtPath
        {
            get { return txtVocalsDdsPath.Text; }
            set { txtVocalsDdsPath.Text = value; }
        }

        public bool IsCustom
        {
            get { return chkCustomFont.Checked; }
            set { chkCustomFont.Checked = value; }
        }

        public string SngPath
        {
            get { return txtVocalsSngPath.Text; }
            set { txtVocalsSngPath.Text = value; }
        }

        public string VocalsPath
        {
            get { return txtVocalsXmlPath.Text; }
            set { txtVocalsXmlPath.Text = value; }
        }

        private void PopulateKey()
        {
            lstKey.Items.Add("Rocksmith Lyric Characters");
            lstKey.Items.Add("");
            lstKey.Items.Add("'-' individually highlight syllables");
            lstKey.Items.Add("'--' show as seperate syllables/words");
            lstKey.Items.Add("'+' split over new line or add line");
            lstKey.Items.Add("'++' split over two new lines");
            lstKey.Items.Add("");
            lstKey.Items.Add("periods and commas are not used");
            lstKey.Items.Add("'=' used to produce '-' in game");
            lstKey.Items.Add("");
            lstKey.Items.Add("Submit additional keys to Developers");
        }

        private void PopulateRichText()
        {
            if (!String.IsNullOrEmpty(VocalsPath))
            {
                // EOF is using windows-1252 Encoding for extended (custom) vocals
                // Toolkit and game are using UTF-8 Encoding for vocals

                // confirmed this preserves and displays proper encoding
                using (var sr = new StreamReader(VocalsPath, new UTF8Encoding(false)))
                {
                    rtbVocals.Text = sr.ReadToEnd();
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(VocalsPath))
            {
                // perform rough validation of vocals xml
                // full validation is performed later if needed
                var vocalsStream = rtbVocals.Text.StripIllegalXMLChars();
                // confirmed this preserves and saves with proper encoding
                using (var reader = new StreamReader(vocalsStream, new UTF8Encoding(false)))
                using (var sw = new StreamWriter(VocalsPath, false, new UTF8Encoding(false)))
                {
                    var validString = reader.ReadToEnd();
                    // put back CRLF because RichTextBox removes them
                    validString = validString.RestoreCRLF();
                    sw.Write(validString);
                }
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnVocalsDdsPath_Click(object sender, EventArgs e)
        {

            // toolkit writes this artifact to: /assets/ui/lyrics/girigirichop/lyrics_SONGNAME.dds
            using (var f = new VistaOpenFileDialog())
            {
                f.FileName = ArtPath;
                f.Title = "Select a Custom Lyric Font DDS Texture (512x1024)";
                f.Filter = "Custom Lyrics Font Files (*.dds)|*.dds";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    ArtPath = f.FileName;
                }
            }
        }

        private void btnVocalsSngPath_Click(object sender, EventArgs e)
        {
            using (var f = new VistaOpenFileDialog())
            {
                f.FileName = SngPath;
                f.Filter = "Rocksmith SNG Custom Font Files (*.sng)|*.sng";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    ArtPath = f.FileName;
                    IsCustom = true; //possible jvocals and regular vocals, but supported only one custom font texture
                }
            }
        }

        private void btnVocalsXmlPath_Click(object sender, EventArgs e)
        {
            using (var f = new VistaOpenFileDialog())
            {
                f.FileName = VocalsPath;
                f.Filter = "Rocksmith XML Vocals or JVocals Files (*vocals.xml)|*vocals.xml";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    VocalsPath = f.FileName;
                    PopulateRichText();
                }
            }
        }

        private void txtVocalsXmlPath_TextChanged(object sender, EventArgs e)
        {
            ValidateGui();
        }

        private void chkCustomFont_CheckedChanged(object sender, EventArgs e)
        {
            ValidateGui();
        }

        private void ValidateGui()
        {
            // checking for custom fonts
            if (txtVocalsXmlPath.Text.ToLower().EndsWith("_jvocals.xml"))
            {
                btnVocalsDdsPath.Enabled = true;
                txtVocalsDdsPath.Enabled = true;
                chkCustomFont.Checked = true;
            }
            else
            {
                btnVocalsDdsPath.Enabled = false;
                txtVocalsDdsPath.Enabled = false;
            }

        }

    }
}

