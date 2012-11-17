using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Tone;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public partial class DLCPackageCreator : UserControl
    {
        public DLCPackageCreator()
        {
            InitializeComponent();
        }

        private string OggPath
        {
            get { return OggPathTB.Text; }
            set { OggPathTB.Text = value; }
        }
        private string AlbumArtPath
        {
            get { return AlbumArtPathTB.Text; }
            set { AlbumArtPathTB.Text = value; }
        }

        private void arrangementAddButton_Click(object sender, EventArgs e)
        {
            Arrangement arrangement;
            using (var form = new ArrangementForm())
            {
                form.ShowDialog();
                arrangement = form.Arrangement;
            }
            if (arrangement == null)
                return;
            arrangement.Artist = ArtistTB.Text;
            arrangement.SongDisplayName = SongDisplayNameTB.Text;
            arrangement.SongYear = Convert.ToInt32(YearTB.Text);
            ArrangementLB.Items.Add(arrangement);
        }

        private void arrangementRemoveButton_Click(object sender, EventArgs e)
        {
            if (ArrangementLB.SelectedItem != null)
                ArrangementLB.Items.Remove(ArrangementLB.SelectedItem);
        }

        private void openOggButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Fixed WWise Files|*.ogg";
                if (ofd.ShowDialog() == DialogResult.OK)
                    OggPath = ofd.FileName;
            }
        }

        private void dlcGenerateButton_Click(object sender, EventArgs e)
        {
            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            if (arrangements.Count(x => x.IsVocal) > 1)
            {
                MessageBox.Show("Error: Multiple Vocals Found");
                return;
            }
            string dlcSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = "Rocksmith DLC|*.dat";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }

            var dlcName = DlcNameTB.Text.Replace(" ", "_");
            var info = new DLCPackageData
            {
                Name = dlcName,
                Album = AlbumTB.Text,
                AlbumArtPath = AlbumArtPath,
                OggPath = OggPath,
                Arrangements = arrangements,
                Tone = toneControl.Tone
            };
            RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, info);

            MessageBox.Show("Package was generated.", "DLC Package Creator");
        }

        private void albumArtButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "dds Files|*.dds";
                if (ofd.ShowDialog() == DialogResult.OK)
                    AlbumArtPath = ofd.FileName;
            }
        }
    }
}
