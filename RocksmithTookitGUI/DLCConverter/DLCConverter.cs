using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ookii.Dialogs;
using RocksmithToolkitLib.DLCPackage;
using System.IO;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;
using X360.STFS;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;

namespace RocksmithToolkitGUI.DLCConverter
{
    public partial class DLCConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "DLC Converter";

        public string AudioPath
        {
            get { return audioPathTB.Text; }
            set { audioPathTB.Text = value; }
        }

        public string AppId
        {
            get { return AppIdTB.Text; }
            set { AppIdTB.Text = value; }
        }


        public DLCConverter()
        {
            InitializeComponent();

            // Fill source combo            
            var sourcePlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
            sourcePlatform.Remove("None"); //Not compatible for source
            sourcePlatform.Remove("PS3"); //Not compatible for source
            platformSourceCombo.DataSource = sourcePlatform;
            platformSourceCombo.SelectedItem = GamePlatform.Pc.ToString();


            // Fill target combo
            var targetPlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
            targetPlatform.Remove("None");
            platformTargetCombo.DataSource = targetPlatform;
            platformTargetCombo.SelectedItem = GamePlatform.XBox360.ToString();
            
            // Fill App ID
            PopulateAppIdCombo(GameVersion.RS2014); //Supported game version
            AppIdVisibilty();
        }

        private void PopulateAppIdCombo(GameVersion gameVersion)
        {
            SongAppId firstSong = null;
            appIdCombo.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
            {
                appIdCombo.Items.Add(song);
                if (firstSong == null)
                {
                    firstSong = song;
                }
            }
            appIdCombo.SelectedItem = firstSong;
            AppId = firstSong.AppId;
        }

        private void AppIdVisibilty() {
            if (platformTargetCombo.SelectedItem != null)
            {
                var target = new Platform(platformTargetCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                var isPCorMac = target.platform == GamePlatform.Pc || target.platform == GamePlatform.Mac;
                appIdCombo.Visible = isPCorMac;
                AppIdTB.Visible = isPCorMac;
            }
        }

        private void platformSourceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void platformTargetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppIdVisibilty();
        }

        private void appIdCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (appIdCombo.SelectedItem != null)
                AppId = ((SongAppId)appIdCombo.SelectedItem).AppId;
        }

        private void openAudioButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select converted audio for Target Platform";
                ofd.Filter = "Wwise 2013 audio file (*.wem)|*.wem";
                if (ofd.ShowDialog() == DialogResult.OK)
                    AudioPath = ofd.FileName;
            }
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            // Conversion
            string sourcePackage = "";
            var sourcePlatform = (GamePlatform)Enum.Parse(typeof(GamePlatform), platformSourceCombo.SelectedItem.ToString());
            var targetPlatform = (GamePlatform)Enum.Parse(typeof(GamePlatform), platformTargetCombo.SelectedItem.ToString());

            // Validations
            if (sourcePlatform == targetPlatform)
            {
                MessageBox.Show("The source and target platform should be different.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (String.IsNullOrEmpty(AudioPath)) {
                MessageBox.Show("The converted audio on Wwise 2013 for target platform should be selected.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select one DLC for platform conversion";
                switch (sourcePlatform)
	            {
		            case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        ofd.Filter = "PC or Mac Rocksmith 2014 DLC (*.psarc)|*.psarc";
                        break;
                    case GamePlatform.XBox360:
                        ofd.Filter = "XBox 360 Rocksmith 2014 DLC (*.)|*.*";
                        break;
                    default:
                        MessageBox.Show("The converted audio on Wwise 2013 for target platform should be selected.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
	            }
                
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourcePackage = ofd.FileName;
            }

            var nameTemplate = (targetPlatform == GamePlatform.Pc || targetPlatform == GamePlatform.Mac) ? "{0}_{1}.psarc" : "{0}_{1}";
            var targetFileName = Path.Combine(Path.GetDirectoryName(sourcePackage), String.Format(nameTemplate, Path.GetFileNameWithoutExtension(sourcePackage), targetPlatform.ToString()));
            var packageData = GetPackageData(sourcePackage, sourcePlatform, targetPlatform);
            if (packageData == null)
                return;

            RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(targetFileName, packageData, new Platform(targetPlatform, GameVersion.RS2014));
            MessageBox.Show(String.Format("DLC was converted from '{0}' to '{1}'.", sourcePlatform, targetPlatform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private DLCPackageData GetPackageData(string sourcePackage, GamePlatform sourcePlatform, GamePlatform targetPlatform)
        {
            var tmpDir = Path.GetTempPath();
            Packer.Unpack(sourcePackage, tmpDir, false);
            var unpackedDir = Path.Combine(tmpDir, String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(sourcePackage), sourcePlatform));
            
            //Load files
            var xmlSongs = Directory.GetFiles(Path.Combine(unpackedDir, "songs", "arr"), "*.xml", SearchOption.AllDirectories);
            if (xmlSongs.Length <= 1) {
                MessageBox.Show("The selected DLC is not a custom song, you need a custom song for Rocksmith 2014 to use this feature.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            var aggregateFile = Directory.GetFiles(unpackedDir, "*.nt", SearchOption.TopDirectoryOnly)[0];
            var aggregateData = AggregateGraph2014.LoadFromFile(aggregateFile);
            
            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            if (targetPlatform == GamePlatform.Pc || targetPlatform == GamePlatform.Mac)
                data.AppId = AppId;
            data.SignatureType = PackageMagic.CON;
                    
            //Get Arrangements / Tones
            data.Arrangements = new List<Arrangement>();
            data.TonesRS2014 = new List<Tone2014>();

            foreach (var json in jsonFiles) {
                Attributes2014 attr = Manifest2014<Attributes2014>.LoadFromFile(json).Entries.ToArray()[0].Value.ToArray()[0].Value;

                if ((ArrangementType)attr.ArrangementType != ArrangementType.Vocal) {
                    if (data.SongInfo == null) {
                        // Fill Package Data
                        data.Name = attr.DLCKey;
                        data.Volume = attr.SongVolume;

                        // Fill SongInfo
                        data.SongInfo = new SongInfo();
                        data.SongInfo.SongDisplayName = attr.SongName;
                        data.SongInfo.SongDisplayNameSort = attr.SongNameSort;
                        data.SongInfo.Album = attr.AlbumName;
                        data.SongInfo.SongYear = Convert.ToInt32(attr.SongYear);
                        data.SongInfo.Artist = attr.ArtistName;
                        data.SongInfo.ArtistSort = attr.ArtistNameSort;
                        data.SongInfo.AverageTempo = (int)attr.SongAverageTempo;
                    }

                    // Adding Tones
                    foreach (var jsonTone in attr.Tones)
                        if (!data.TonesRS2014.OfType<Tone2014>().Any(t => t.Key == jsonTone.Key))
                            data.TonesRS2014.Add(jsonTone);
                }

                // Adding Arrangements
                var xmlName = attr.SongXml.Split(new char[] { ':' })[3];
                var aggXml = aggregateData.SongXml.SingleOrDefault(n => n.Name == xmlName);
                var xmlFile = unpackedDir + aggXml.RelPath.Replace("/", "\\");
                data.Arrangements.Add(new Arrangement(attr, xmlFile));
            }

            //Get Files
            var ddsFiles = Directory.GetFiles(unpackedDir, "*.dds", SearchOption.AllDirectories);
            if (ddsFiles.Length > 0)
                data.AlbumArtPath = ddsFiles[1];

            var audioPreview = Path.Combine(Path.GetDirectoryName(AudioPath), String.Format("{0}_preview{1}", Path.GetFileNameWithoutExtension(AudioPath), Path.GetExtension(AudioPath)));
            switch (targetPlatform)
            {
                case GamePlatform.Pc:
                    data.OggPath = AudioPath;
                    data.OggPreviewPath = audioPreview;
                    break;
                case GamePlatform.Mac:
                    data.OggMACPath = AudioPath;
                    data.OggPreviewMACPath = audioPreview;
                    break;
                case GamePlatform.XBox360:
                    data.OggXBox360Path = AudioPath;
                    data.OggPreviewXBox360Path = audioPreview;
                    break;
                case GamePlatform.PS3:
                    data.OggPS3Path = AudioPath;
                    data.OggPreviewPS3Path = audioPreview;
                    break;
            }

            return data;
        }
    }
}
