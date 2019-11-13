using Ookii.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class VocalsForm : Form
    {
        public VocalsForm(string lyricArtPath, string vocalsXmlPath)
        {
            InitializeComponent();
            ArtPath = lyricArtPath;
            VocalsPath = vocalsXmlPath;
            PopulateKey();
            PopulateRichText();
        }

        public string ArtPath
        {
            get { return txtVocalsDdsPath.Text; }
            set { txtVocalsDdsPath.Text = value; }
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
            // The Toolkit writes this artifact to: /assets/ui/lyrics/[dlcName]/lyrics_[dlcName].dds
            using (var f = new VistaOpenFileDialog())
            {
                f.InitialDirectory = Path.GetDirectoryName(VocalsPath);
                f.FileName = ArtPath;
                f.Title = "Select a Custom Lyric Font DDS Texture";
                f.Filter = "Custom Lyrics Font Files (*.dds)|*.dds";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    ArtPath = f.FileName;
                }
            }
        }

        private void btnVocalsXmlPath_Click(object sender, EventArgs e)
        {
            using (var f = new VistaOpenFileDialog())
            {
                f.InitialDirectory = Path.GetDirectoryName(VocalsPath);
                f.FileName = VocalsPath;
                f.Filter = "Rocksmith XML Vocals or JVocals Files (*vocals.xml)|*vocals.xml";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    VocalsPath = f.FileName;
                    PopulateRichText();
                }
            }
        }

        private void btnRemoveFont_Click(object sender, EventArgs e)
        {
            ArtPath = null;
        }
    }
}

