using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public partial class ArrangementForm : Form
    {
        public ArrangementForm()
        {
            InitializeComponent();
        }
        public Arrangement Arrangement { get; private set; }
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
            if (!IsVocal.Checked && string.IsNullOrEmpty(ArrangementName.Text))
            {
                ArrangementName.Focus();
                return;
            }

            var arrangement = new Arrangement
            {
                Name = IsVocal.Checked ? "Vocals" : ArrangementName.Text,
                IsVocal = IsVocal.Checked,
                BarChords = BarChords.Checked,
                DoubleStops = DoubleStops.Checked,
                DropDPowerChords = DropDPowerChords.Checked,
                FifthsAndOctaves = FifithsAndOctaves.Checked,
                FretHandMutes = FretHandMutes.Checked,
                OpenChords = OpenChords.Checked,
                PowerChords = PowerChords.Checked,
                PreBends = PreBends.Checked,
                RelativeDifficulty = readInt(RelativeDifficulty.Text),
                SlapAndPop = SlapAndPop.Checked,
                Tuning = Tuning.Text,
                Vibrato = Vibrato.Checked,
                SongFile = new SongFile {File = songfilepath},
                SongXml = new SongXML {File = xmlfilepath}
            };
            if (arrangement.RelativeDifficulty == -1)
            {
                RelativeDifficulty.Focus();
                return;
            }
            Arrangement = arrangement;
            Close();
        }
    }
}
