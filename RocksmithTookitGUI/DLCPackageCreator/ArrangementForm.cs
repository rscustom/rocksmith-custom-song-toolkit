using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public partial class ArrangementForm : Form
    {
        private Arrangement arrangement;

        public ArrangementForm(IEnumerable<string> toneNames)
            : this(new Arrangement
            {
                SongFile = new SongFile { File = "" },
                SongXml = new SongXML { File = "" },
                ArrangementType = ArrangementType.Guitar,
                RelativeDifficulty = 1,
                ScrollSpeed = 20
            }, toneNames)
        {
        }

        public ArrangementForm(Arrangement arrangement, IEnumerable<string> toneNames)
        {
            InitializeComponent();
            
            foreach (var val in Enum.GetValues(typeof(InstrumentTuning))) {
                tuningComboBox.Items.Add(val);
            }
            foreach (var val in Enum.GetValues(typeof(ArrangementType)))
            {
                arrangementTypeCombo.Items.Add(val);
            }
            arrangementTypeCombo.SelectedValueChanged += (sender, e) => {
                // Selecting defaults
                ArrangementType selectedType = ((ArrangementType)((ComboBox)sender).SelectedItem);
                
                switch (selectedType) {
                    case ArrangementType.Bass:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Bass);
                        arrangementNameCombo.SelectedItem = ArrangementName.Bass;
                        break;
                    case ArrangementType.Vocal:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Vocals);
                        arrangementNameCombo.SelectedItem = ArrangementName.Vocals;
                        break;
                    default:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Combo);
                        arrangementNameCombo.Items.Add(ArrangementName.Lead);
                        arrangementNameCombo.Items.Add(ArrangementName.Rhythm);
                        arrangementNameCombo.SelectedItem = arrangement.Name;
                        break;
                }
                arrangementNameCombo.Enabled = selectedType == ArrangementType.Guitar;

                Picked.Checked = selectedType == ArrangementType.Bass ? false : true;
            };
            foreach (var tone in toneNames)
            {
                tonesCombo.Items.Add(tone);
            }
            scrollSpeedTrackBar.Scroll += (sender, e) =>
            {
                scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeedTrackBar.Value) / 10);
            };
            Arrangement = arrangement;
        }

        public Arrangement Arrangement
        {
            get
            {
                return arrangement;
            }
            private set
            {
                arrangement = value;
                
                //Arrangement details
                arrangementNameCombo.SelectedItem = arrangement.Name;
                arrangementTypeCombo.SelectedItem = arrangement.ArrangementType;
                
                InstrumentTuning tuning = InstrumentTuning.Standard;
                Enum.TryParse<InstrumentTuning>(arrangement.Tuning, true, out tuning);
                tuningComboBox.SelectedItem = tuning;

                tonesCombo.SelectedItem = arrangement.ToneName;
                if (tonesCombo.SelectedItem == null && tonesCombo.Items.Count > 0)
                {
                    tonesCombo.SelectedItem = tonesCombo.Items[0];
                }

                Picked.Checked = arrangement.PluckedType == PluckedType.Picked;
                RelativeDifficulty.Text = arrangement.RelativeDifficulty.ToString();

                int scrollSpeed = Math.Min(scrollSpeedTrackBar.Maximum, Math.Max(scrollSpeedTrackBar.Minimum, arrangement.ScrollSpeed));
                scrollSpeedTrackBar.Value = scrollSpeed;
                scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeed) / 10);

                //Song files
                SngFilePath.Text = arrangement.SongFile.File;
                XmlFilePath.Text = arrangement.SongXml.File;
                
                //Techniques
                PowerChords.Checked = arrangement.PowerChords;
                BarChords.Checked = arrangement.BarChords;
                OpenChords.Checked = arrangement.OpenChords;
                DoubleStops.Checked = arrangement.DoubleStops;
                DropDPowerChords.Checked = arrangement.DropDPowerChords;
                FretHandMutes.Checked = arrangement.FretHandMutes;
                Prebends.Checked = arrangement.Prebends;
                Vibrato.Checked = arrangement.Vibrato;
                //Bass techniques
                FifthsAndOctaves.Checked = arrangement.FifthsAndOctaves;
                TwoFingerPlucking.Checked = arrangement.TwoFingerPlucking;
                Syncopation.Checked = arrangement.Syncopation;
            }
        }

        private void songFileBrowseButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "sng Files|*.sng";
                if (ofd.ShowDialog() == DialogResult.OK)
                    SngFilePath.Text = ofd.FileName;
            }
        }

        private void songXmlBrowseButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Xml Files|*.xml";
                if (ofd.ShowDialog() == DialogResult.OK)
                    XmlFilePath.Text = ofd.FileName;
            }
        }

        private int readInt(string val)
        {
            int v;
            if (int.TryParse(val, out v) == false)
                return -1;
            return v;
        }

        private void addArrangementButton_Click(object sender, EventArgs e)
        {
            var songfilepath = SngFilePath.Text;
            var xmlfilepath = XmlFilePath.Text;
            if (!File.Exists(xmlfilepath))
            {
                XmlFilePath.Focus();
                return;
            }
            if (!File.Exists(songfilepath))
            {
                SngFilePath.Focus();
                return;
            }

            //Arrangment details
            arrangement.Name = (ArrangementName)arrangementNameCombo.SelectedItem;
            arrangement.ArrangementType = (ArrangementType)arrangementTypeCombo.SelectedItem;
            arrangement.Tuning = tuningComboBox.SelectedItem.ToString();
            arrangement.ToneName = tonesCombo.SelectedItem.ToString();
            arrangement.PluckedType = Picked.Checked ? PluckedType.Picked : PluckedType.NotPicked;
            arrangement.RelativeDifficulty = readInt(RelativeDifficulty.Text);
            arrangement.ScrollSpeed = scrollSpeedTrackBar.Value;

            //Song files
            arrangement.SongFile.File = songfilepath;
            arrangement.SongXml.File = xmlfilepath;
            
            //Techniques
            arrangement.PowerChords = PowerChords.Checked;
            arrangement.BarChords = BarChords.Checked;
            arrangement.OpenChords = OpenChords.Checked;
            arrangement.DoubleStops = DoubleStops.Checked;
            arrangement.DropDPowerChords = DropDPowerChords.Checked;
            arrangement.FretHandMutes = FretHandMutes.Checked;
            arrangement.Prebends = Prebends.Checked;
            arrangement.Vibrato = Vibrato.Checked;
            //Bas techniques            
            arrangement.FifthsAndOctaves = FifthsAndOctaves.Checked;
            arrangement.TwoFingerPlucking = TwoFingerPlucking.Checked;
            arrangement.Syncopation = Syncopation.Checked;
            
            
            if (arrangement.RelativeDifficulty == -1)
            {
                RelativeDifficulty.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void arrangementNameCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
