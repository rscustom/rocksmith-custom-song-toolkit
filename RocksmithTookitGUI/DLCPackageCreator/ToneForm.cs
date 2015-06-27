using System;
using System.Windows.Forms;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneForm : Form
    {
        public bool Saved = false;
        public GameVersion CurrentGameVersion;
        //private DLCPackageCreator parentControl = null;


        private string CurrentOFDFilter
        {
            get
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2014:
                        return "Rocksmith 2014 Tone Template(*.tone2014.xml)|*.tone2014.xml|All XML Files (*.xml)|*.xml";
                    default:
                        return "Rocksmith Tone Template (*.tone.xml)|*.tone.xml|All XML Files (*.xml)|*.xml";
                }
            }
        }

        public ToneForm()
        {
            InitializeComponent();

        }

        private void okButton_Click(object sender, EventArgs e)
        {//TODO: max 6 gears allowed, 2 nessesary and 4 extra.
            switch (CurrentGameVersion)
            {
                case GameVersion.RS2012:
                    if (toneControl1.Tone.PedalList.Count == 0) return;
                    if (toneControl1.Tone.PedalList.Count > 6) return;
                    break;
                case GameVersion.RS2014:
                    if (toneControl1.Tone.GearList.IsNull()) return;
                    if (toneControl1.Tone.GearList.SlotsUsed() > 6)
                    {
                        MessageBox.Show("Reached effects limit, Only 4 extra gears allowed.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
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

            try
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2012:
                        toneControl1.Tone = Tone.LoadFromXmlTemplateFile(toneSavePath);
                        break;
                    case GameVersion.RS2014:
                        toneControl1.Tone = Tone2014.LoadFromXmlTemplateFile(toneSavePath);
                        break;
                }

            }
            catch (Exception ex)
            {
                toneControl1.Tone = null;
                MessageBox.Show("Can't load saved tone. \n" + ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Tone was loaded.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string toneSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = CurrentOFDFilter;
                ofd.AddExtension = true;
                if (CurrentGameVersion != GameVersion.RS2012)
                    ofd.FileName = String.Format("{0}.tone2014.xml", toneControl1.Tone.Name);
                else
                    ofd.FileName = String.Format("{0}.tone.xml", toneControl1.Tone.Name);

                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneSavePath = ofd.FileName;
            }

            var tone = toneControl1.Tone;
            tone.Serialize(toneSavePath);

            MessageBox.Show("Tone was saved.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
