using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Ookii.Dialogs;
using X360.STFS;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;

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

        public Platform SourcePlatform {
            get {
                if (platformSourceCombo.Items.Count > 0)
                    return new Platform(platformSourceCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                else
                    return new Platform(GamePlatform.None, GameVersion.None);
            }
        }

        public Platform TargetPlatform {
            get {
                if (platformTargetCombo.Items.Count > 0)
                    return new Platform(platformTargetCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                else
                    return new Platform(GamePlatform.None, GameVersion.None);
            }

        }

        private bool NeedRebuildPackage {
            get { return SourcePlatform.IsConsole != TargetPlatform.IsConsole; }
        }

        public DLCConverter()
        {
            InitializeComponent();

            // Fill source combo            
            var sourcePlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
            sourcePlatform.Remove("None");
            platformSourceCombo.DataSource = sourcePlatform;
            platformSourceCombo.SelectedItem = GamePlatform.Pc.ToString();

            // Fill target combo
            var targetPlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
            targetPlatform.Remove("None");
            platformTargetCombo.DataSource = targetPlatform;
            platformTargetCombo.SelectedItem = GamePlatform.XBox360.ToString();

            // Fill App ID
            try
            {
                PopulateAppIdCombo(GameVersion.RS2014); //Supported game version
                AppIdVisibilty();
            }
            catch { }

            AudioPathVisibility();
        }

        private void PopulateAppIdCombo(GameVersion gameVersion)
        {
            SongAppId firstSong = null;
            appIdCombo.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
            {
                appIdCombo.Items.Add(song);
                if (firstSong == null)
                    firstSong = song;
            }
            appIdCombo.SelectedItem = firstSong;
            AppId = firstSong.AppId;
        }

        private void AudioPathVisibility() {
            audioPathTB.Visible = NeedRebuildPackage;
            openAudioButton.Visible = NeedRebuildPackage;
            previewMessageLabel.Visible = NeedRebuildPackage;
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

        private void platformSourceCombo_SelectedIndexChanged(object sender, EventArgs e) {
            AudioPathVisibility();
        }

        private void platformTargetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppIdVisibilty();
            AudioPathVisibility();
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
            // Validations
            if (SourcePlatform == TargetPlatform) {
                MessageBox.Show("The source and target platform should be different.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (NeedRebuildPackage)
                if (String.IsNullOrEmpty(AudioPath)) {
                    MessageBox.Show("The converted audio on Wwise 2013 for target platform should be selected.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            // Conversion
            string sourcePackage;

            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select one DLC for platform conversion";
                switch (SourcePlatform.platform) {
                    case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        ofd.Filter = "PC or Mac Rocksmith 2014 DLC (*.psarc)|*.psarc";
                        break;
                    case GamePlatform.XBox360:
                        ofd.Filter = "XBox 360 Rocksmith 2014 DLC (*.)|*.*";
                        break;
                    case GamePlatform.PS3:
                        ofd.Filter = "PS3 Rocksmith 2014 DLC (*.edat)|*.edat";
                        break;
                    default:
                        MessageBox.Show("The converted audio on Wwise 2013 for target platform should be selected.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourcePackage = ofd.FileName;
            }

            // SOURCE
            var tmpDir = Path.GetTempPath();
            var unpackedDir = Path.Combine(tmpDir, String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(sourcePackage), SourcePlatform.platform));

            if (Directory.Exists(unpackedDir))
                Directory.Delete(unpackedDir, true);

            Packer.Unpack(sourcePackage, tmpDir, SourcePlatform.platform == GamePlatform.Pc);            
            
            // DESTINATION
            var nameTemplate = (!TargetPlatform.IsConsole) ? "{0}_{1}.psarc" : "{0}_{1}";
            var targetFileName = Path.Combine(Path.GetDirectoryName(sourcePackage), String.Format(nameTemplate, Path.GetFileNameWithoutExtension(sourcePackage), TargetPlatform.platform.ToString()));
            
            // CONVERSION
            if (NeedRebuildPackage)
                ConvertPackageRebuilding(unpackedDir, targetFileName);
            else
                ConvertPackageForSimilarPlatform(unpackedDir, targetFileName);
        }

        private void ConvertPackageForSimilarPlatform(string unpackedDir, string targetFileName) {
            // Old and new paths
            var sourceDir0 = SourcePlatform.GetPathName()[0].ToLower();
            var sourceDir1 = SourcePlatform.GetPathName()[1].ToLower();
            var targetDir0 = TargetPlatform.GetPathName()[0].ToLower();
            var targetDir1 = TargetPlatform.GetPathName()[1].ToLower();

            // Replace aggregate graph values
            var aggregateFile = Directory.GetFiles(unpackedDir, "*.nt", SearchOption.TopDirectoryOnly)[0];
            var aggregateGraphText = File.ReadAllText(aggregateFile);
            // Tags
            aggregateGraphText = Regex.Replace(aggregateGraphText, GraphItem.GetPlatformTagDescription(SourcePlatform.platform), GraphItem.GetPlatformTagDescription(TargetPlatform.platform), RegexOptions.Multiline);
            // Paths
            aggregateGraphText = Regex.Replace(aggregateGraphText, sourceDir0, targetDir0, RegexOptions.Multiline);
            aggregateGraphText = Regex.Replace(aggregateGraphText, sourceDir1, targetDir1, RegexOptions.Multiline);
            File.WriteAllText(aggregateFile, aggregateGraphText);

            // Rename directories
            foreach (var dir in Directory.GetDirectories(unpackedDir, "*.*", SearchOption.AllDirectories)) {
                if (dir.EndsWith(sourceDir0))
                {
                    var newDir = dir.Replace(sourceDir0, targetDir0);
                    if (Directory.Exists(newDir))
                        Directory.Delete(newDir, true);
                    DirectoryExtension.Move(dir, newDir);
                }
                else if (dir.EndsWith(sourceDir1))
                {
                    var newDir = dir.Replace(sourceDir1, targetDir1);
                    if (Directory.Exists(newDir))
                        Directory.Delete(newDir, true);
                    DirectoryExtension.Move(dir, newDir);
                }
            }

            // Packing
            Packer.Pack(unpackedDir, targetFileName, (TargetPlatform.platform == GamePlatform.Pc) ? true : false, false);

            if (Directory.Exists(unpackedDir))
                Directory.Delete(unpackedDir, true);

            MessageBox.Show(String.Format("DLC was converted from '{0}' to '{1}'.", SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ConvertPackageRebuilding(string unpackedDir, string targetFileName) {
            //Load files
            var xmlSongs = Directory.GetFiles(Path.Combine(unpackedDir, "songs", "arr"), "*.xml", SearchOption.AllDirectories);
            if (xmlSongs.Length <= 1) {
                MessageBox.Show("The selected DLC is not a custom song, you need a custom song for Rocksmith 2014 to use this feature.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var jsonFiles = Directory.GetFiles(unpackedDir, "*.json", SearchOption.AllDirectories);
            var aggregateFile = Directory.GetFiles(unpackedDir, "*.nt", SearchOption.TopDirectoryOnly)[0];
            var aggregateData = AggregateGraph2014.LoadFromFile(aggregateFile);

            var data = new DLCPackageData();
            data.GameVersion = GameVersion.RS2014;
            if (!TargetPlatform.IsConsole)
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
            switch (TargetPlatform.platform) {
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

            RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(targetFileName, data, new Platform(TargetPlatform.platform, GameVersion.RS2014));
            MessageBox.Show(String.Format("DLC was converted from '{0}' to '{1}'.", SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
