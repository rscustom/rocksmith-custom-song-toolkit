using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Tone;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib;
using System.Xml.XPath;
using System.Xml.Linq;
using RocksmithToolkitLib.Ogg;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public partial class DLCPackageCreator : UserControl
    {
        public DLCPackageCreator()
        {
            InitializeComponent();
            SongAppId firstSong = null;
            foreach (var song in SongAppId.GetSongAppIds())
            {
                cmbAppIds.Items.Add(song);
                if (firstSong == null)
                {
                    firstSong = song;
                }
            }
            cmbAppIds.SelectedItem = firstSong;
            AppIdTB.Text = firstSong.AppId;

            TonesLB.Items.Add(CreateNewTone());
        }

        private Tone CreateNewTone()
        {
            Tone tone = new Tone();
            var allPedals = GameData.GetPedalData();
            tone.Name = "Default";
            bool uniqueToneName = false;
            int ind = 0;
            do
            {
                uniqueToneName = null == TonesLB.Items.OfType<Tone>().FirstOrDefault(t => tone.Name.Equals(t.Name));
                if (!uniqueToneName)
                {
                    tone.Name = "Default " + (++ind);
                }
            } while (!uniqueToneName);

            tone.PedalList.Add("Amp", allPedals.First(p => p.Key == "Amp_Fusion").MakePedalSetting());
            tone.PedalList.Add("Cabinet", allPedals.First(p => p.Key == "Cab_2X12_Fusion_57_Cone").MakePedalSetting());
            return tone;
        }

        private IEnumerable<string> GetToneNames()
        {
            return TonesLB.Items.OfType<Tone>().Select(t => t.Name);
        }
        private string OggPath
        {
            get { return oggPathTB.Text; }
            set { oggPathTB.Text = value; }
        }
        private string OggXBox360Path {
            get { return oggXBox360PathTB.Text; }
            set { oggXBox360PathTB.Text = value; }
        }
        private string AlbumArtPath
        {
            get { return AlbumArtPathTB.Text; }
            set { AlbumArtPathTB.Text = value; }
        }

        private void arrangementAddButton_Click(object sender, EventArgs e)
        {
            Arrangement arrangement;
            using (var form = new ArrangementForm(GetToneNames()))
            {
                if (DialogResult.OK != form.ShowDialog())
                {
                    return;
                }
                arrangement = form.Arrangement;
            }
            if (arrangement == null)
                return;
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

        private void openOggXBox360Button_Click(object sender, EventArgs e) {
            using (var ofd = new OpenFileDialog()) {
                ofd.Filter = "Fixed WWise Files|*.ogg";
                if (ofd.ShowDialog() == DialogResult.OK)
                    OggXBox360Path = ofd.FileName;
            }
        }

        private void dlcGenerateButton_Click(object sender, EventArgs e)
        {
            var packageData = GetPackageData();
            if (packageData == null)
            {
                MessageBox.Show("One or more fields are missing information.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (platformPC.Checked)
                    OggFile.VerifyHeaders(OggPath);
                if (platformXBox360.Checked)
                    OggFile.VerifyHeaders(OggXBox360Path);
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(ex.Message, "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (platformPC.Checked && OggFile.getPlatform(OggPath) != GamePlatform.Pc)
            {
                MessageBox.Show("The Windows OGG is either invalid or for the wrong platform.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (platformXBox360.Checked && OggFile.getPlatform(OggXBox360Path) != GamePlatform.XBox360)
            {
                MessageBox.Show("The Xbox 360 OGG is either invalid or for the wrong platform.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string dlcSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = "Rocksmith DLC|*.dat";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }
            if (platformPC.Checked)
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, GamePlatform.Pc);
            if (platformXBox360.Checked)
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(Path.Combine(Path.GetDirectoryName(dlcSavePath), Path.GetFileNameWithoutExtension(dlcSavePath)), packageData, GamePlatform.XBox360);

            MessageBox.Show("Package was generated.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dlcSaveButton_Click(object sender, EventArgs e)
        {
            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            string dlcSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = "Rocksmith DLC Template|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }
            var path = new Uri(Path.GetDirectoryName(dlcSavePath) + Path.DirectorySeparatorChar);

            var packageData = GetPackageData();
            if (packageData == null)
            {
                MessageBox.Show("One or more fields are missing information.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Make the paths relative
            string albumPath = packageData.AlbumArtPath;
            if (!string.IsNullOrEmpty(albumPath) && Uri.IsWellFormedUriString(albumPath, UriKind.Absolute))
                packageData.AlbumArtPath = path.MakeRelativeUri(new Uri(albumPath)).ToString();

            string oggPath = packageData.OggPath;
            if (!String.IsNullOrEmpty(oggPath) && Uri.IsWellFormedUriString(oggPath, UriKind.Absolute))
                packageData.OggPath = path.MakeRelativeUri(new Uri(oggPath)).ToString();
            
            string oggXBox360Path = packageData.OggXBox360Path;
            if (!String.IsNullOrEmpty(oggXBox360Path) && Uri.IsWellFormedUriString(oggXBox360Path, UriKind.Absolute))
                packageData.OggXBox360Path = path.MakeRelativeUri(new Uri(oggXBox360Path)).ToString();
            
            foreach (var arr in packageData.Arrangements)
            {
                string songXmlFile = arr.SongXml.File;
                if (!String.IsNullOrEmpty(songXmlFile) && Uri.IsWellFormedUriString(songXmlFile, UriKind.Absolute))
                    arr.SongXml.File = path.MakeRelativeUri(new Uri(songXmlFile)).ToString();
            }
            var serializer = new DataContractSerializer(typeof(DLCPackageData));
            using (var stm = new XmlTextWriter(dlcSavePath, Encoding.Default))
            {
                serializer.WriteObject(stm, packageData);
            }

            //Re-absolutize the paths
            foreach (var arr in packageData.Arrangements)
            {
                string songXmlFile = arr.SongXml.File;
                if (!String.IsNullOrEmpty(songXmlFile) && Uri.IsWellFormedUriString(songXmlFile, UriKind.Relative))
                    arr.SongXml.File = MakeAbsolute(path, arr.SongXml.File);
            }
            
            MessageBox.Show("DLC Package template was saved.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dlcLoadButton_Click(object sender, EventArgs e)
        {
            string dlcSavePath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Rocksmith DLC Template|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }

            DLCPackageData info = null;

            var serializer = new DataContractSerializer(typeof(DLCPackageData));
            using (var stm = new XmlTextReader(dlcSavePath))
            {
                try {
                    info = (DLCPackageData)serializer.ReadObject(stm);
                } catch (SerializationException se) {
                    //Make compatible with previous version DLC saved
                    if (se.Message.IndexOf("ArrangementName") > -1 || se.Message.IndexOf("InstrumentTuning") > -1)
                    {
                        try {
                            info = (DLCPackageData)serializer.ReadObject(FixOldDlcPackage(dlcSavePath));
                        } catch (SerializationException se2) {
                            MessageBox.Show("Can't load saved DLC because is not compatible with new DLC save format. \n\r" + se2.Message, "DLCPackageCreator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }                    
                }
            }

            TonesLB.Items.Clear();
            ArrangementLB.Items.Clear();

            if (info.Tones == null)
            {
                info.Tones = new List<Tone>();
            }
            if (info.Tones.Count == 0)
            {
                info.Tones.Add(CreateNewTone());
            }

            var path = new Uri(Path.GetDirectoryName(dlcSavePath) + Path.DirectorySeparatorChar);

            // Song INFO
            DlcNameTB.Text = info.Name;
            AppIdTB.Text = info.AppId;
            AlbumTB.Text = info.SongInfo.Album;
            SongDisplayNameTB.Text = info.SongInfo.SongDisplayName;
            YearTB.Text = info.SongInfo.SongYear.ToString();
            ArtistTB.Text = info.SongInfo.Artist;
            AverageTempo.Text = info.SongInfo.AverageTempo.ToString();

            // Album art
            AlbumArtPath = MakeAbsolute(path, info.AlbumArtPath);

            // Ogg Windows file
            if (!String.IsNullOrEmpty(info.OggPath))
                OggPath = MakeAbsolute(path, info.OggPath);
            platformPC.Checked = !String.IsNullOrEmpty(OggPath);

            // Ogg XBox360 file
            if (!String.IsNullOrEmpty(info.OggXBox360Path))
                OggXBox360Path = MakeAbsolute(path, info.OggXBox360Path);
            platformXBox360.Checked = !String.IsNullOrEmpty(OggXBox360Path);

            foreach (var arrangement in info.Arrangements)
            {
                arrangement.SongXml.File = MakeAbsolute(path, arrangement.SongXml.File);
                if (arrangement.ToneName == null)
                {
                    arrangement.ToneName = info.Tones[0].Name;
                }
                ArrangementLB.Items.Add(arrangement);
            }

            foreach (var tone in info.Tones)
            {
                TonesLB.Items.Add(tone);
            }

            MessageBox.Show("DLC Package template was loaded.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private XmlTextReader FixOldDlcPackage(string xmlPath) {
            var doc = XDocument.Load(xmlPath);
            XNamespace dlcNamespace = "http://schemas.datacontract.org/2004/07/RocksmithToolkitLib.DLCPackage";
            XmlNamespaceManager nsArrangement = new XmlNamespaceManager(new NameTable());
            nsArrangement.AddNamespace("ns", dlcNamespace.NamespaceName);
            string arrangementXPah = "/ns:DLCPackageData/ns:Arrangements";

            bool missingArrangementType = false;

            for (int i = 1; i <= ((IEnumerable<XElement>)doc.XPathSelectElements(arrangementXPah + "/ns:Arrangement", nsArrangement)).Count(); i++)
            {
                XElement arrangementName = doc.XPathSelectElement(String.Format(arrangementXPah + "/ns:Arrangement[{0}]/ns:Name", i), nsArrangement);
                XElement arrangementType = doc.XPathSelectElement(String.Format(arrangementXPah + "/ns:Arrangement[{0}]/ns:ArrangementType", i), nsArrangement);
                XElement tuning = doc.XPathSelectElement(String.Format(arrangementXPah + "/ns:Arrangement[{0}]/ns:Tuning", i), nsArrangement);

                if (arrangementType == null)
                {
                    XElement arrangement = doc.XPathSelectElement(String.Format(arrangementXPah + "/ns:Arrangement[{0}]", i), nsArrangement);
                    arrangementType = new XElement(dlcNamespace + "ArrangementType", "Guitar");
                    arrangement.Add(arrangementType);
                    missingArrangementType = true;
                }

                //Fix arrangement name
                switch (arrangementType.Value) {
                    case "Bass":
                        arrangementName.Value = "Bass";
                        break;
                    case "Vocal":
                        arrangementName.Value = "Vocals";
                        break;
                    default:
                        arrangementName.Value = "Combo";
                        break;
                }
            }

            if (missingArrangementType)
                MessageBox.Show("Warning: One or more arrangement have no ArrangementType defined. Guitar has added by default, take a look.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return new XmlTextReader(new StringReader(doc.Document.ToString()));
        }

        private string MakeAbsolute(Uri baseUri, string path)
        {
            return new Uri(baseUri, path).AbsolutePath.Replace("%25", "%").Replace("%20", " ");
        }

        private DLCPackageData GetPackageData()
        {
            int year, tempo;
            if (string.IsNullOrEmpty(DlcNameTB.Text))
            {
                DlcNameTB.Focus();
                return null;
            }
            if (DlcNameTB.Text == SongDisplayNameTB.Text) {
                MessageBox.Show("Error: DLC name can't be the same of song name", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DlcNameTB.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(SongDisplayNameTB.Text))
            {
                SongDisplayNameTB.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(AlbumTB.Text))
            {
                AlbumTB.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(ArtistTB.Text))
            {
                ArtistTB.Focus();
                return null;
            }
            if (!int.TryParse(YearTB.Text, out year))
            {
                YearTB.Focus();
                return null;
            }
            if (!int.TryParse(AverageTempo.Text, out tempo))
            {
                AverageTempo.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(AppIdTB.Text))
            {
                AppIdTB.Focus();
                return null;
            }
            if (!platformPC.Checked && !platformXBox360.Checked && !platformPS3.Checked)
            {
                MessageBox.Show("Error: No game platform selected", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            if (platformPC.Checked && !File.Exists(OggPath))
            {
                oggPathTB.Focus();
                return null;
            }
            if (platformXBox360.Checked && !File.Exists(OggXBox360Path)) {
                oggXBox360PathTB.Focus();
                return null;
            }
            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            if (arrangements.Count(x => x.ArrangementType == ArrangementType.Bass) > 1) {
                MessageBox.Show("Error: Multiple Bass arrangement found", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            if (arrangements.Count(x => x.ArrangementType == ArrangementType.Vocal) > 1)
            {
                MessageBox.Show("Error: Multiple Vocals arrangement found", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            var tones = TonesLB.Items.OfType<Tone>().ToList();
            var data = new DLCPackageData
            {
                Name = DlcNameTB.Text.Replace(" ", "_"),
                AppId = AppIdTB.Text,
                SongInfo = new SongInfo
                {
                    SongDisplayName = SongDisplayNameTB.Text,
                    Album = AlbumTB.Text,
                    SongYear = year,
                    Artist = ArtistTB.Text,
                    AverageTempo = tempo
                },
                AlbumArtPath = AlbumArtPath,
                OggPath = OggPath,
                OggXBox360Path = OggXBox360Path,
                Arrangements = arrangements,
                Tones = tones
            };

            return data;
        }

        private void cmbAppIds_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbAppIds.SelectedItem != null)
            {
                AppIdTB.Text = ((SongAppId)cmbAppIds.SelectedItem).AppId;
            }
        }

        private void ArrangementLB_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            arrangementEditButton_Click(sender, e);
        }

        private void arrangementEditButton_Click(object sender, EventArgs e)
        {
            if (ArrangementLB.SelectedItem != null)
            {
                var arrangement = (Arrangement)ArrangementLB.SelectedItem;
                using (var form = new ArrangementForm(arrangement, GetToneNames()) { Text = "Edit Arrangement" })
                {
                    if (DialogResult.OK != form.ShowDialog())
                    {
                        return;
                    }
                }
                ArrangementLB.Items[ArrangementLB.SelectedIndex] = arrangement;
            }
        }


        private void toneAddButton_Click(object sender, EventArgs e)
        {
            Tone tone = CreateNewTone();
            using (var form = new ToneForm(tone))
            {
                form.ShowDialog();
                TonesLB.Items.Add(tone);
            }
        }

        private void toneRemoveButton_Click(object sender, EventArgs e)
        {
            if (TonesLB.SelectedItem != null && TonesLB.Items.Count > 1)
            {
                var tone = (Tone)TonesLB.SelectedItem;
                TonesLB.Items.Remove(TonesLB.SelectedItem);

                var firstTone = (Tone)TonesLB.Items[0];
                foreach (var item in ArrangementLB.Items.OfType<Arrangement>())
                {
                    if (tone.Name.Equals(item.ToneName))
                    {
                        item.ToneName = firstTone.Name;
                    }
                }
                ArrangementLB.Refresh();
            }
        }

        private void ToneLB_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            toneEditButton_Click(sender, e);
        }

        private void toneEditButton_Click(object sender, EventArgs e)
        {
            if (TonesLB.SelectedItem != null)
            {
                var tone = (Tone)TonesLB.SelectedItem;
                var toneName = tone.Name;
                using (var form = new ToneForm(tone))
                {
                    form.ShowDialog();
                }
                if (toneName != tone.Name)
                {
                    for(int i = 0; i <ArrangementLB.Items.Count; i++) {
                        var arrangement = (Arrangement)ArrangementLB.Items[i];
                        if (toneName.Equals(arrangement.ToneName))
                        {
                            arrangement.ToneName = tone.Name;
                            ArrangementLB.Items[i] = arrangement;
                        }
                    }
                    TonesLB.Items[TonesLB.SelectedIndex] = tone;
                }
            }
        }

        private void toneImportButton_Click(object sender, EventArgs e)
        {
            string toneImportFile;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a package or tone manifest file";
                ofd.Filter = "Package or Tone manifest|*.dat;tone*.manifest.json";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneImportFile = ofd.FileName;
            }

            try
            {
                Application.DoEvents();
                List<Tone> tones = ToneReader.Read(toneImportFile);
                foreach (Tone tone in tones)
                    TonesLB.Items.Add(tone);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't import tone(s). \n\r" + ex.Message, "DLCPackageCreator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Tone(s) was imported.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void plataform_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox platformCkb = ((CheckBox)sender);
            bool pChecked = platformCkb.Checked;
            if (platformCkb.Name.IndexOf("XBox360") > 0)
            {
                oggXBox360PathTB.Visible = pChecked;
                openOggXBox360Button.Visible = pChecked;
            }
            else
            {
                oggPathTB.Visible = pChecked;
                openOggButton.Visible = pChecked;
                AppIdTB.Visible = pChecked;
                cmbAppIds.Visible = pChecked;
            }
        }
    }
}