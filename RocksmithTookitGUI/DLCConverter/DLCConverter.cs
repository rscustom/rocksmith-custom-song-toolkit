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
                    firstSong = song;
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

        private void platformTargetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppIdVisibilty();
        }

        private void appIdCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (appIdCombo.SelectedItem != null)
                AppId = ((SongAppId)appIdCombo.SelectedItem).AppId;
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            // VALIDATIONS
            if (SourcePlatform.Equals(TargetPlatform)) {
                MessageBox.Show("The source and target platform should be different.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // GET FILES
            string[] sourcePackages;

            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select one DLC for platform conversion";
                ofd.Multiselect = true;
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
                sourcePackages = ofd.FileNames;
            }

            // SOURCE
            var tmpDir = Path.GetTempPath();

            StringBuilder errorsFound = new StringBuilder();

            foreach (var sourcePackage in sourcePackages)
            {
                var alertMessage = String.Format("Source package '{0}' seems to be not {1} platform, the conversion can't be work.", Path.GetFileName(sourcePackage), SourcePlatform.platform);
                if (SourcePlatform.platform != GamePlatform.PS3)
                {
                    if (!Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(SourcePlatform.GetPathName()[2]))
                    {
                        errorsFound.AppendLine(alertMessage);
                        if (MessageBox.Show(String.Format(alertMessage + Environment.NewLine + "Force try to convert this package?", SourcePlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            continue;
                    }
                } else if (SourcePlatform.platform == GamePlatform.PS3) {
                    if (!(Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(SourcePlatform.GetPathName()[2] + ".psarc")))
                    {
                        errorsFound.AppendLine(alertMessage);
                        if (MessageBox.Show(String.Format(alertMessage + Environment.NewLine + "Force try to convert this package?", SourcePlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            continue;
                    }
                }

                var unpackedDir = Path.Combine(tmpDir, String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(sourcePackage), SourcePlatform.platform));

                if (Directory.Exists(unpackedDir))
                    Directory.Delete(unpackedDir, true);

                Packer.Unpack(sourcePackage, tmpDir, SourcePlatform.platform == GamePlatform.Pc);

                // DESTINATION
                var nameTemplate = (!TargetPlatform.IsConsole) ? "{0}{1}.psarc" : "{0}{1}";

                var packageName = Path.GetFileNameWithoutExtension(sourcePackage);
                if (packageName.EndsWith(new Platform(GamePlatform.Pc, GameVersion.None).GetPathName()[2]) ||
                        packageName.EndsWith(new Platform(GamePlatform.Mac, GameVersion.None).GetPathName()[2]) ||
                        packageName.EndsWith(new Platform(GamePlatform.XBox360, GameVersion.None).GetPathName()[2]) ||
                        packageName.EndsWith(new Platform(GamePlatform.PS3, GameVersion.None).GetPathName()[2] + ".psarc"))
                {
                    packageName = packageName.Substring(0, packageName.LastIndexOf("_"));
                }
                var targetFileName = Path.Combine(Path.GetDirectoryName(sourcePackage), String.Format(nameTemplate, Path.Combine(Path.GetDirectoryName(sourcePackage), packageName), TargetPlatform.GetPathName()[2]));

                var hasNoXmlSong = Directory.GetFiles(Path.Combine(unpackedDir, "songs", "arr"), "*.xml", SearchOption.AllDirectories).Length <= 1;

                // CONVERSION                
                if (NeedRebuildPackage) {
                    if (hasNoXmlSong) {
                        errorsFound.AppendLine(String.Format("Package {0} is not a custom song, you need a custom song to convert Rocksmith 2014 from non similiar platforms.", sourcePackage));
                        return;
                    }
                    ConvertPackageRebuilding(unpackedDir, targetFileName);
                } else
                    ConvertPackageForSimilarPlatform(unpackedDir, targetFileName);

                if (Directory.Exists(unpackedDir))
                    Directory.Delete(unpackedDir, true);
            }

            if (errorsFound.Length <= 0)
                MessageBox.Show(String.Format("DLC was converted from '{0}' to '{1}'.", SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show(String.Format("DLC was converted from '{0}' to '{1}' with erros. See below: " + Environment.NewLine + errorsFound.ToString(), SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // Recreates SNG because SNG have different keys in PC and Mac
            bool updateSNG = ((SourcePlatform.platform == GamePlatform.Pc && TargetPlatform.platform == GamePlatform.Mac) ||
                (SourcePlatform.platform == GamePlatform.Mac && TargetPlatform.platform == GamePlatform.Pc));

            // Packing
            Packer.Pack(unpackedDir, targetFileName, (TargetPlatform.platform == GamePlatform.Pc) ? true : false, updateSNG);

            if (Directory.Exists(unpackedDir))
                Directory.Delete(unpackedDir, true);
        }

        private void ConvertPackageRebuilding(string unpackedDir, string targetFileName) {
            //Load files
            var xmlSongs = Directory.GetFiles(Path.Combine(unpackedDir, "songs", "arr"), "*.xml", SearchOption.AllDirectories);
            
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

                if (attr.Phrases != null) {
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
                var xmlName = attr.SongXml.Split(':')[3];
                var aggXml = aggregateData.SongXml.SingleOrDefault(n => n.Name == xmlName);
                var xmlFile = unpackedDir + aggXml.RelPath.Replace("/", "\\");
                if (attr.Phrases != null)
                    data.Arrangements.Add(new Arrangement(attr, xmlFile));
                else
                {   // issue for vocal file
                    var voc = new Arrangement();
                    voc.ArrangementType = ArrangementType.Vocal;
                    voc.SongFile = new SongFile { File = "" };
                    voc.SongXml = new SongXML { File = xmlFile };
                    voc.ScrollSpeed = 20;
                    data.Arrangements.Add(voc);
                }
            }

            //Get Files
            var ddsFiles = Directory.GetFiles(unpackedDir, "*.dds", SearchOption.AllDirectories);
            if (ddsFiles.Length > 0)
                data.AlbumArtPath = ddsFiles[1];

            var sourceAudioFiles = Directory.GetFiles(unpackedDir, "*.wem", SearchOption.AllDirectories);
            
            var targetAudioFiles = new List<string>();
            foreach (var file in sourceAudioFiles)
            {
                var newFile = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                OggFile.ConvertWem(file, newFile);
                targetAudioFiles.Add(newFile);
            }

            if (targetAudioFiles.Count() <= 0)
                throw new InvalidDataException("Audio files not found.");
            
            string audioPath, audioPreviewPath;
            FileInfo a = new FileInfo(targetAudioFiles[0]);
            FileInfo b = a; // For packages that have not audio preview
            if (targetAudioFiles.Count() == 2)
                b = new FileInfo(targetAudioFiles[1]);

            if (a.Length > b.Length)
            {
                audioPath = a.FullName;
                audioPreviewPath = b.FullName;
            }
            else
            {
                audioPath = b.FullName;
                audioPreviewPath = a.FullName;
            }

            var audioPreview = Path.Combine(Path.GetDirectoryName(audioPath), String.Format("{0}_preview{1}", Path.GetFileNameWithoutExtension(audioPath), Path.GetExtension(audioPath)));
            switch (TargetPlatform.platform)
            {
                case GamePlatform.Pc:
                    data.OggPath = audioPath;
                    data.OggPreviewPath = audioPreview;
                    break;
                case GamePlatform.Mac:
                    data.OggMACPath = audioPath;
                    data.OggPreviewMACPath = audioPreview;
                    break;
                case GamePlatform.XBox360:
                    data.OggXBox360Path = audioPath;
                    data.OggPreviewXBox360Path = audioPreview;
                    break;
                case GamePlatform.PS3:
                    data.OggPS3Path = audioPath;
                    data.OggPreviewPS3Path = audioPreview;
                    break;
            }

            RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(targetFileName, data, new Platform(TargetPlatform.platform, GameVersion.RS2014));
        }
    }
}
