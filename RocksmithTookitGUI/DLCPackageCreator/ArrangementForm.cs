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
            foreach (var val in Enum.GetValues(typeof(ArrangementType)))
            {
                arrangementTypeCombo.Items.Add(val);
            }
            foreach (var tone in toneNames)
            {
                tonesCombo.Items.Add(tone);
            }
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
                ArrangementName.Text = arrangement.Name;
                arrangementTypeCombo.SelectedItem = arrangement.ArrangementType;
                BarChords.Checked = arrangement.BarChords;
                DoubleStops.Checked = arrangement.DoubleStops;
                DropDPowerChords.Checked = arrangement.DropDPowerChords;
                FifthsAndOctaves.Checked = arrangement.FifthsAndOctaves;
                FretHandMutes.Checked = arrangement.FretHandMutes;
                OpenChords.Checked = arrangement.OpenChords;
                PowerChords.Checked = arrangement.PowerChords;
                PreBends.Checked = arrangement.PreBends;
                RelativeDifficulty.Text = arrangement.RelativeDifficulty.ToString();
                SlapAndPop.Checked = arrangement.SlapAndPop;
                PreBends.Checked = arrangement.PreBends;
                Tuning.Text = arrangement.Tuning;
                Vibrato.Checked = arrangement.Vibrato;
                scrollSpeedTrackBar.Value = Math.Min(scrollSpeedTrackBar.Maximum, Math.Max(scrollSpeedTrackBar.Minimum, arrangement.ScrollSpeed));

                SngFilePath.Text = arrangement.SongFile.File;
                XmlFilePath.Text = arrangement.SongXml.File;
                tonesCombo.SelectedItem = arrangement.ToneName;
                if (tonesCombo.SelectedItem == null && tonesCombo.Items.Count > 0)
                {
                    tonesCombo.SelectedItem = tonesCombo.Items[0];
                }
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
            if (string.IsNullOrEmpty(ArrangementName.Text))
            {
                ArrangementName.Focus();
                return;
            }

            arrangement.ArrangementType = (ArrangementType)arrangementTypeCombo.SelectedItem;
            arrangement.Name = arrangement.ArrangementType == ArrangementType.Vocal ? "Vocals" : ArrangementName.Text;
            arrangement.BarChords = BarChords.Checked;
            arrangement.DoubleStops = DoubleStops.Checked;
            arrangement.DropDPowerChords = DropDPowerChords.Checked;
            arrangement.FifthsAndOctaves = FifthsAndOctaves.Checked;
            arrangement.FretHandMutes = FretHandMutes.Checked;
            arrangement.OpenChords = OpenChords.Checked;
            arrangement.PowerChords = PowerChords.Checked;
            arrangement.PreBends = PreBends.Checked;
            arrangement.RelativeDifficulty = readInt(RelativeDifficulty.Text);
            arrangement.SlapAndPop = SlapAndPop.Checked;
            arrangement.Tuning = Tuning.Text;
            arrangement.Vibrato = Vibrato.Checked;
            arrangement.ScrollSpeed = scrollSpeedTrackBar.Value;
            arrangement.SongFile.File = songfilepath;
            arrangement.SongXml.File = xmlfilepath;
            arrangement.ToneName = tonesCombo.SelectedItem.ToString();

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
    }
}
