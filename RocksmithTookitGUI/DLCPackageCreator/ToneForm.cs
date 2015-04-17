using System;
using System.Windows.Forms;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;



namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneForm : Form
    {
        public bool Saved = false;
        public GameVersion CurrentGameVersion;
        private DLCPackageCreator parentControl = null;


        private string CurrentOFDFilter
        {
            get
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2014:
                        return "Rocksmith 2014 Tone Template (*.tone2014.xml)|*.tone2014.xml";
                    default:
                        return "Rocksmith Tone Template (*.tone.xml)|*.tone.xml";
                }
            }
        }

        public ToneForm()
        {
            InitializeComponent();

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            switch (CurrentGameVersion)
            {
                case GameVersion.RS2012:
                    if (toneControl1.Tone.PedalList.Count == 0) return;
                    break;
                case GameVersion.RS2014:
                    if (toneControl1.Tone.GearList.IsNull()) return;
                    break;
            }
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
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2012:
                        tone = Tone.LoadFromXmlTemplateFile(toneSavePath);
                        break;
                    case GameVersion.RS2014:
                        tone = Tone2014.LoadFromXmlTemplateFile(toneSavePath);
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't load saved tone. \n" + ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            toneControl1.Tone = tone;

            MessageBox.Show("Tone was loaded.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            MessageBox.Show("Tone was saved.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
