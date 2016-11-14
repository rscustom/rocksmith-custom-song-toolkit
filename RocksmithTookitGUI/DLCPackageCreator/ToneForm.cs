using System;
using System.IO;
using System.Windows.Forms;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneForm : Form
    {
        public bool Saved = false;
        public GameVersion CurrentGameVersion;
        public bool EditMode = false;

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
        {
            // removed limit ... set to 69 for testing
            //TODO: max 6 gears allowed, 2 necessary and 4 extra.
            switch (CurrentGameVersion)
            {
                case GameVersion.RS2012:
                    if (toneControl.Tone.PedalList.Count == 0) return;
                    if (toneControl.Tone.PedalList.Count > 6) return;
                    break;
                case GameVersion.RS2014:
                    if (toneControl.Tone.GearList.IsNull()) 
                        return;
                    if (toneControl.Tone.GearList.SlotsUsed() > 6)
                    {
                        MessageBox.Show("Using " + toneControl.Tone.GearList.SlotsUsed() + " Game Effects may crash game.   " + Environment.NewLine + Environment.NewLine +
                                        "Please report your test results to" + Environment.NewLine + "the Rocksmith Toolkit Developers.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // MessageBox.Show("Reached effects limit, Only 4 extra gears allowed.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // return;
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
                ofd.InitialDirectory = Globals.DefaultToneFile;
                ofd.Filter = CurrentOFDFilter;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneSavePath = Globals.DefaultToneFile = ofd.FileName;
            }
            LoadToneFile(toneSavePath);
        }

        public void LoadToneFile(string toneSavePath, bool verbose = true)
        {
            try
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2012:
                        toneControl.Tone = Tone.LoadFromXmlTemplateFile(toneSavePath);
                        break;
                    case GameVersion.RS2014:
                        toneControl.Tone = Tone2014.LoadFromXmlTemplateFile(toneSavePath);
                        break;
                }

            }
            catch (Exception ex)
            {
                toneControl.Tone = null;
                MessageBox.Show("Can't load saved tone. \n" + ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (verbose)
                MessageBox.Show("Tone was loaded.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string toneSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.InitialDirectory = Globals.DefaultToneFile;
                ofd.Filter = CurrentOFDFilter;
                ofd.AddExtension = true;
                if (CurrentGameVersion != GameVersion.RS2012)
                    ofd.FileName = String.Format("{0}.tone2014.xml", toneControl.Tone.Name);
                else
                    ofd.FileName = String.Format("{0}.tone.xml", toneControl.Tone.Name);

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                toneSavePath = Globals.DefaultToneFile = ofd.FileName;
            }

            var tone = toneControl.Tone;
            tone.Serialize(toneSavePath);

            MessageBox.Show("Tone was saved.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
