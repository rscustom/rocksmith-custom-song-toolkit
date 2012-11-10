using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RocksmithDLCCreator;

namespace RocksmithToolkitGUI
{
    public partial class ArrangementForm : Form
    {
        public ArrangementForm()
        {
            InitializeComponent();
        }
        public Arrangement Arragement { get; private set; }
        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "sng Files|*.sng";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SngFilePath.Text = ofd.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Xml Files|*.xml";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                XmlFilePath.Text = ofd.FileName;
        }

        private int readInt(string val)
        {
            int v;
            if (int.TryParse(val, out v) == false)
                return -1;
            return v;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var arrangement = new Arrangement();
            arrangement.IsVocal = IsVocal.Checked;
            arrangement.Name = arrangement.IsVocal ? "Vocals" : ArrangementName.Text;
            arrangement.AverageTempo = readInt(AverageTempo.Text);
            if (arrangement.AverageTempo == -1)
                return;
            arrangement.BarChords = BarChords.Checked;
            arrangement.DoubleStops = DoubleStops.Checked;
            arrangement.DropDPowerChords = DropDPowerChords.Checked;
            arrangement.FifthsAndOctaves = FifithsAndOctaves.Checked;
            arrangement.FretHandMutes = FretHandMutes.Checked;
            arrangement.OpenChords = OpenChords.Checked;
            arrangement.PowerChords = PowerChords.Checked;
            arrangement.PreBends = PreBends.Checked;
            arrangement.RelativeDifficulty = readInt(RelativeDifficulty.Text);
            if (arrangement.RelativeDifficulty == -1)
                return;
            arrangement.SlapAndPop = SlapAndPop.Checked;
            arrangement.SongDifficulty = readInt(SongDifficulty.Text);
            if (arrangement.SongDifficulty == -1)
                return;
            var songfilepath = SngFilePath.Text;
            var xmlfilepath = XmlFilePath.Text;
            if (!File.Exists(songfilepath) || !File.Exists(xmlfilepath))
                return;
            arrangement.SongFile = new SongFile() { File = songfilepath };
            arrangement.SongXml = new SongXML() { File = xmlfilepath };
            arrangement.Tuning = Tuning.Text;
            arrangement.Vibrato = Vibrato.Checked;
            Arragement = arrangement;
            this.Close();
        }
    }
}
