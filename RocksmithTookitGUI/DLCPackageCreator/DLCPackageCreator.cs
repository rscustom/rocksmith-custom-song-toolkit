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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using X360.STFS;
using System.Diagnostics;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.ToolkitTone;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class DLCPackageCreator : UserControl
    {
        private const string MESSAGEBOX_CAPTION =  "DLC Package Creator";
        
        private GameVersion CurrentGameVersion {
            get {
                if (RS2014.Checked)
                    return GameVersion.RS2014;
                else
                    return GameVersion.RS2012; //Default
            }
        }

        private string CurrentOFDAudioFileFilter
        {
            get
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2014:
                        return "Wwise 2013 audio files (*.wem)|*.wem";
                    default:
                        return "Wwise 2010 audio files (*.ogg)|*.ogg"; 
                }
            }
        }

        private string CurrentOFDToneImportFilter
        {
            get
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2014:
                        return CurrentRocksmithTitle + " Song Package or Song Manifest (*.*,*.psarc,*.json)|*.*;*.psarc;*.json";
                    default:
                        return CurrentRocksmithTitle + " Song Package, Tone Manifest or Rocksmith Tone Xml (*.*,*.dat,*.manifest.json,*.xml)|*.*;*.xml;*.dat;tone*.manifest.json"; 
                }
            }
        }

        private string CurrentRocksmithTitle
        {
            get
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2014:
                        return "Rocksmith 2014";
                    default:
                        return "Rocksmith";
                }
            }
        }

        //Song Information
        public string DLCName
        {
            get { return DlcNameTB.Text; }
            set { DlcNameTB.Text = GetValidName(value); }
        }
        public string SongTitle
        {
            get { return SongDisplayNameTB.Text; }
            set { SongDisplayNameTB.Text = value; }
        }
        public string SongTitleSort
        {
            get { return SongDisplayNameSortTB.Text; }
            set { SongDisplayNameSortTB.Text = value; }
        }
        public string Album
        {
            get { return AlbumTB.Text; }
            set { AlbumTB.Text = value; }
        }
        public string Artist
        {
            get { return ArtistTB.Text; }
            set { ArtistTB.Text = value; }
        }
        public string ArtistSort
        {
            get { return ArtistSortTB.Text; }
            set { ArtistSortTB.Text = value; }
        }
        public string AlbumYear
        {
            get { return YearTB.Text; }
            set { YearTB.Text = value; }
        }
        public string AppId
        {
            get { return AppIdTB.Text; }
            set { AppIdTB.Text = value; }
        }
        public string AverageTempo
        {
            get { return AverageTempoTB.Text; }
            set
            {
                int tempo = 0;
                string stringTempo = value;
                int pointIndex = value.IndexOf(".");
                if (pointIndex > -1)
                    stringTempo = stringTempo.Substring(0, pointIndex);
                if (int.TryParse(stringTempo, out tempo))
                    AverageTempoTB.Text = tempo.ToString();
            }
        }

        //Tones
        private IEnumerable<string> GetToneNames()
        {
            if (CurrentGameVersion == GameVersion.RS2014)
                return TonesLB.Items.OfType<Tone2014>().Select(t => t.Name);
            else
                return TonesLB.Items.OfType<Tone>().Select(t => t.Name);
        }

        //Files
        private string AlbumArtPath // 512 (RS1)
        {
            get { return AlbumArtPathTB.Text; }
            set { AlbumArtPathTB.Text = value; }
        }
        private string AlbumArt256Path; // RS2014
        private string AlbumArt128Path; // RS2014
        private string AlbumArt64Path; // RS2014

        private string OggPCPath
        {
            get { return oggPcPathTB.Text; }
            set { oggPcPathTB.Text = value; }
        }
        private string OggMACPath
        {
            get { return oggMacPathTB.Text; }
            set { oggMacPathTB.Text = value; }
        }
        private string OggXBox360Path {
            get { return oggXBox360PathTB.Text; }
            set { oggXBox360PathTB.Text = value; }
        }
        private string OggPS3Path
        {
            get { return oggPS3PathTB.Text; }
            set { oggPS3PathTB.Text = value; }
        }

        public DLCPackageCreator()
        {
            InitializeComponent();
            PopulateAppIdCombo();
            PopulateTonesLB();
        }

        private void PopulateTonesLB()
        {
            TonesLB.Items.Clear();
            TonesLB.Items.Add(CreateNewTone());
        }

        private dynamic CreateNewTone()
        {
            var name = "Default";
            bool uniqueToneName = false;
            int ind = 0;
            do
            {
                uniqueToneName = !TonesLB.Items.Contains(name);
                if (!uniqueToneName)
                {
                    name = "Default " + (++ind);
                }
            } while (!uniqueToneName);

            if (CurrentGameVersion == GameVersion.RS2014)
                return new Tone2014() { Name = name };
            else
                return new Tone() { Name = name };
        }

        private void arrangementAddButton_Click(object sender, EventArgs e)
        {
            Arrangement arrangement;
            using (var form = new ArrangementForm(GetToneNames(), (DLCPackageCreator)this, CurrentGameVersion))
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
                ofd.Filter = "PC " + CurrentOFDAudioFileFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    OggPCPath = ofd.FileName;
            }
        }

        private void openOggMacButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "MAC " + CurrentOFDAudioFileFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    OggMACPath = ofd.FileName;
            }
        }

        private void openOggXBox360Button_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "XBOX 360 " + CurrentOFDAudioFileFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    OggXBox360Path = ofd.FileName;
            }
        }

        private void openOggPS3Button_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PS3 " + CurrentOFDAudioFileFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    OggPS3Path = ofd.FileName;
            }
        }

        private void dlcGenerateButton_Click(object sender, EventArgs e)
        {
            var packageData = GetPackageData();
            if (packageData == null)
            {
                MessageBox.Show("One or more fields are missing information.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CurrentGameVersion == GameVersion.RS2012)
            {
                try
                {
                    if (platformPC.Checked)
                        OggFile.VerifyHeaders(OggPCPath);
                    if (platformMAC.Checked)
                        OggFile.VerifyHeaders(OggMACPath);
                    if (platformXBox360.Checked)
                        OggFile.VerifyHeaders(OggXBox360Path);
                    if (platformPS3.Checked)
                        OggFile.VerifyHeaders(OggPS3Path);
                }
                catch (InvalidDataException ex)
                {
                    MessageBox.Show(ex.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (platformPC.Checked && OggFile.getPlatform(OggPCPath).platform != GamePlatform.Pc)
                {
                    MessageBox.Show("The Windows OGG is either invalid or for the wrong platform.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (platformXBox360.Checked && OggFile.getPlatform(OggXBox360Path).platform != GamePlatform.XBox360)
                {
                    MessageBox.Show("The Xbox 360 OGG is either invalid or for the wrong platform.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string dlcSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = CurrentRocksmithTitle + " DLC (*.*)|*.*";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }

            if (platformPC.Checked)
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.Pc, CurrentGameVersion));
            if (platformMAC.Checked)
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData, new Platform(GamePlatform.Mac, CurrentGameVersion));
            if (platformXBox360.Checked)
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(Path.Combine(Path.GetDirectoryName(dlcSavePath), Path.GetFileNameWithoutExtension(dlcSavePath)), packageData, new Platform(GamePlatform.XBox360, CurrentGameVersion));
            if (platformPS3.Checked)
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(Path.Combine(Path.GetDirectoryName(dlcSavePath), Path.GetFileNameWithoutExtension(dlcSavePath)), packageData, new Platform(GamePlatform.PS3, CurrentGameVersion));

            MessageBox.Show("Package was generated.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void albumArtButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DlcNameTB.Text)) {
                MessageBox.Show("Fill the 'DLC Name' field first.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DlcNameTB.Focus();
                return;
            }
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Album Art File (*.dds)|*.dds";
                if (ofd.ShowDialog() == DialogResult.OK)
                    AlbumArtPath = ofd.FileName;
            }
            if (CurrentGameVersion == GameVersion.RS2014)
            {
                GenerateAlbumArtFiles();
            }
        }

        private void GenerateAlbumArtFiles()
        {
            // Rename file
            var oldName = Path.GetFileNameWithoutExtension(AlbumArtPath);
            var newName = String.Format("album_{0}", DLCName);

            var aArtDirBase = Path.GetDirectoryName(AlbumArtPath);

            var aArt512JpgTmp = Path.Combine(aArtDirBase, String.Format("{0}.jpg", oldName));
            var aArt256JpgTmp = Path.Combine(aArtDirBase, String.Format("{0}_256.jpg", newName));
            var aArt128JpgTmp = Path.Combine(aArtDirBase, String.Format("{0}_128.jpg", newName));
            var aArt64JpgTmp = Path.Combine(aArtDirBase, String.Format("{0}_64.jpg", newName));

            // Convert input DDS to JPG
            ConvertAlbumArtFile("dds2jpg.exe", AlbumArtPath);

            if (!File.Exists(aArt512JpgTmp))
                MessageBox.Show("Can't generate new .dds file. An error ocurred!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Generate new rezised images
            ImageHandler.Save(aArt512JpgTmp, 256, 256, 100, aArt256JpgTmp);
            ImageHandler.Save(aArt512JpgTmp, 128, 128, 100, aArt128JpgTmp);
            ImageHandler.Save(aArt512JpgTmp, 64, 64, 100, aArt64JpgTmp);

            // Convert new sizes JPG to DDS
            ConvertAlbumArtFile("jpg2dds.exe", aArt256JpgTmp);
            ConvertAlbumArtFile("jpg2dds.exe", aArt128JpgTmp);
            ConvertAlbumArtFile("jpg2dds.exe", aArt64JpgTmp);

            // Get new DDS files path
            AlbumArt256Path = Path.Combine(aArtDirBase, Path.GetFileNameWithoutExtension(aArt256JpgTmp) + ".dds");
            AlbumArt128Path = Path.Combine(aArtDirBase, Path.GetFileNameWithoutExtension(aArt128JpgTmp) + ".dds");
            AlbumArt64Path = Path.Combine(aArtDirBase, Path.GetFileNameWithoutExtension(aArt64JpgTmp) + ".dds");

            // Delete jpg tmp files
            if (File.Exists(aArt512JpgTmp))
                File.Delete(aArt512JpgTmp);
            if (File.Exists(aArt256JpgTmp))
                File.Delete(aArt256JpgTmp);
            if (File.Exists(aArt128JpgTmp))
                File.Delete(aArt128JpgTmp);
            if (File.Exists(aArt64JpgTmp))
                File.Delete(aArt64JpgTmp);
        }

        private void ConvertAlbumArtFile(string converter, string imageFilePath) {
            string workDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ddstool");

            using (Process dds2jpgProcess = new Process())
            {
                dds2jpgProcess.StartInfo.FileName = Path.Combine(workDir, converter);
                dds2jpgProcess.StartInfo.WorkingDirectory = workDir;
                dds2jpgProcess.StartInfo.Arguments = String.Format("-c \"{0}\" \"{1}\"", imageFilePath, Path.GetDirectoryName(imageFilePath));
                dds2jpgProcess.StartInfo.UseShellExecute = false;
                dds2jpgProcess.StartInfo.CreateNoWindow = true;

                dds2jpgProcess.Start();
                dds2jpgProcess.WaitForExit();
            }
        }

        private void dlcSaveButton_Click(object sender, EventArgs e)
        {
            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            string dlcSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = CurrentRocksmithTitle + " DLC Template (*.dlc.xml)|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }
            var path = new Uri(Path.GetDirectoryName(dlcSavePath) + Path.DirectorySeparatorChar);

            var packageData = GetPackageData();
            if (packageData == null)
            {
                MessageBox.Show("One or more fields are missing information.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Make the paths relative
            string albumPath = packageData.AlbumArtPath;
            if (!string.IsNullOrEmpty(albumPath) && Uri.IsWellFormedUriString(albumPath, UriKind.Absolute))
                packageData.AlbumArtPath = path.MakeRelativeUri(new Uri(albumPath)).ToString();

            if (CurrentGameVersion == GameVersion.RS2014){
                string album256Path = packageData.AlbumArtPath;
                if (!string.IsNullOrEmpty(album256Path) && Uri.IsWellFormedUriString(album256Path, UriKind.Absolute))
                    packageData.AlbumArt256 = path.MakeRelativeUri(new Uri(album256Path)).ToString();

                string album128Path = packageData.AlbumArtPath;
                if (!string.IsNullOrEmpty(album128Path) && Uri.IsWellFormedUriString(album128Path, UriKind.Absolute))
                    packageData.AlbumArt128 = path.MakeRelativeUri(new Uri(album128Path)).ToString();

                string album64Path = packageData.AlbumArtPath;
                if (!string.IsNullOrEmpty(album64Path) && Uri.IsWellFormedUriString(album64Path, UriKind.Absolute))
                    packageData.AlbumArt64 = path.MakeRelativeUri(new Uri(album64Path)).ToString();
            }

            if (CurrentGameVersion == GameVersion.RS2014)
            {
                string albumPath128 = packageData.AlbumArt128;
                if (!string.IsNullOrEmpty(albumPath128) && Uri.IsWellFormedUriString(albumPath128, UriKind.Absolute))
                    packageData.AlbumArt128 = path.MakeRelativeUri(new Uri(albumPath128)).ToString();

                string albumPath64 = packageData.AlbumArt64;
                if (!string.IsNullOrEmpty(albumPath64) && Uri.IsWellFormedUriString(albumPath64, UriKind.Absolute))
                    packageData.AlbumArt64 = path.MakeRelativeUri(new Uri(albumPath64)).ToString();
            }

            string oggPath = packageData.OggPath;
            if (!String.IsNullOrEmpty(oggPath) && Uri.IsWellFormedUriString(oggPath, UriKind.Absolute))
                packageData.OggPath = path.MakeRelativeUri(new Uri(oggPath)).ToString();

            if (CurrentGameVersion == GameVersion.RS2014)
            {
                string oggMacPath = packageData.OggMACPath;
                if (!String.IsNullOrEmpty(oggMacPath) && Uri.IsWellFormedUriString(oggMacPath, UriKind.Absolute))
                    packageData.OggMACPath = path.MakeRelativeUri(new Uri(oggMacPath)).ToString();
            }

            string oggXBox360Path = packageData.OggXBox360Path;
            if (!String.IsNullOrEmpty(oggXBox360Path) && Uri.IsWellFormedUriString(oggXBox360Path, UriKind.Absolute))
                packageData.OggXBox360Path = path.MakeRelativeUri(new Uri(oggXBox360Path)).ToString();

            string oggPS3Path = packageData.OggPS3Path;
            if (!String.IsNullOrEmpty(oggPS3Path) && Uri.IsWellFormedUriString(oggPS3Path, UriKind.Absolute))
                packageData.OggPS3Path = path.MakeRelativeUri(new Uri(oggPS3Path)).ToString();
            
            foreach (var arr in packageData.Arrangements)
            {
                string songXmlFile = arr.SongXml.File;
                if (!String.IsNullOrEmpty(songXmlFile) && Uri.IsWellFormedUriString(songXmlFile, UriKind.Absolute))
                    arr.SongXml.File = path.MakeRelativeUri(new Uri(songXmlFile)).ToString();
            }
            var serializer = new DataContractSerializer(typeof(DLCPackageData));
            using (var stm = XmlWriter.Create(dlcSavePath, new XmlWriterSettings() { CheckCharacters = true, Indent = true }))
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

            MessageBox.Show(CurrentRocksmithTitle + " DLC Package template was saved.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dlcLoadButton_Click(object sender, EventArgs e)
        {
            string dlcSavePath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = CurrentRocksmithTitle + " DLC Template (*.dlc.xml)|*.dlc.xml";
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
                    //Make compatible with previous version saved DLC
                    if (se.Message.IndexOf("ArrangementName") > -1 || se.Message.IndexOf("InstrumentTuning") > -1)
                    {
                        try {
                            info = (DLCPackageData)serializer.ReadObject(FixOldDlcPackage(dlcSavePath));
                        } catch (SerializationException se2) {
                            MessageBox.Show("Can't load saved DLC because is not compatible with new DLC save format. \n\r" + se2.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }                    
                }
            }

            TonesLB.Items.Clear();
            ArrangementLB.Items.Clear();

            if (info == null)
            {
                MessageBox.Show("Can't load saved DLC. An error ocurred.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RS2012.Checked = info.GameVersion == GameVersion.RS2012;
            RS2014.Checked = info.GameVersion == GameVersion.RS2014;

            switch (CurrentGameVersion)
            {
                case GameVersion.RS2012:
                    if (info.Tones == null)
                    info.Tones = new List<Tone>();
                    if (info.Tones.Count == 0)
                        info.Tones.Add(CreateNewTone());

                    foreach (var tone in info.Tones)
                        TonesLB.Items.Add(tone);
                    break;
                case GameVersion.RS2014:
                    if (info.TonesRS2014 == null)
                    info.TonesRS2014 = new List<Tone2014>();
                    if (info.TonesRS2014.Count == 0)
                        info.TonesRS2014.Add(CreateNewTone());

                    foreach (var toneRS2014 in info.TonesRS2014)
                        TonesLB.Items.Add(toneRS2014);
                    break;
            }              

            var path = new Uri(Path.GetDirectoryName(dlcSavePath) + Path.DirectorySeparatorChar);

            // Song INFO
            DlcNameTB.Text = info.Name;

            PopulateAppIdCombo();
            Application.DoEvents();
            AppIdTB.Text = info.AppId;
            SelectComboAppId(info.AppId);

            AlbumTB.Text = info.SongInfo.Album;
            SongDisplayNameTB.Text = info.SongInfo.SongDisplayName;
            SongDisplayNameSortTB.Text = info.SongInfo.SongDisplayNameSort;
            YearTB.Text = info.SongInfo.SongYear.ToString();
            ArtistTB.Text = info.SongInfo.Artist;
            ArtistSortTB.Text = info.SongInfo.ArtistSort;
            AverageTempoTB.Text = info.SongInfo.AverageTempo.ToString();

            // Album art
            AlbumArtPath = MakeAbsolute(path, info.AlbumArtPath);
            AlbumArt256Path = MakeAbsolute(path, info.AlbumArt256);
            AlbumArt128Path = MakeAbsolute(path, info.AlbumArt128);
            AlbumArt64Path = MakeAbsolute(path, info.AlbumArt64);

            // Windows audio file
            if (!String.IsNullOrEmpty(info.OggPath))
                OggPCPath = MakeAbsolute(path, info.OggPath);
            platformPC.Checked = !String.IsNullOrEmpty(OggPCPath);

            // Mac audio file
            if (!String.IsNullOrEmpty(info.OggMACPath))
                OggMACPath = MakeAbsolute(path, info.OggMACPath);
            platformMAC.Checked = !String.IsNullOrEmpty(OggMACPath);

            // XBox360 audio file
            if (!String.IsNullOrEmpty(info.OggXBox360Path))
                OggXBox360Path = MakeAbsolute(path, info.OggXBox360Path);
            platformXBox360.Checked = !String.IsNullOrEmpty(OggXBox360Path);

            // PS3 audio file
            if (!String.IsNullOrEmpty(info.OggPS3Path))
                OggPS3Path = MakeAbsolute(path, info.OggPS3Path);
            platformPS3.Checked = !String.IsNullOrEmpty(OggPS3Path);

            volumeBox.Value = info.Volume;

            //if (platformXBox360.Checked)
            //    rbuttonSignatureLIVE.Checked = info.SignatureType == PackageMagic.LIVE;

            foreach (var arrangement in info.Arrangements)
            {
                arrangement.SongXml.File = MakeAbsolute(path, arrangement.SongXml.File);
                if (arrangement.ToneBase == null)
                    arrangement.ToneBase = info.Tones[0].Name;

                if (CurrentGameVersion == GameVersion.RS2014) {
                    //Defining a default gameplay path if all is false
                    if (arrangement.PathLead == false && arrangement.PathRhythm == false && arrangement.PathBass == false && arrangement.ArrangementType != ArrangementType.Vocal) {
                        switch (arrangement.ArrangementType)
                        {
                            case ArrangementType.Guitar:
                                switch (arrangement.Name)
	                            {
                                    case ArrangementName.Lead:
                                        arrangement.PathLead = true;
                                        break;
                                    case ArrangementName.Rhythm:
                                        arrangement.PathRhythm = true;
                                        break;
                                    case ArrangementName.Combo:
                                        arrangement.PathLead = true;
                                        arrangement.PathRhythm = true;
                                        break;
	                            }
                                break;
                            case ArrangementType.Bass:
                                arrangement.PathBass = true;
                                break;
                        }
                    }
                }

                ArrangementLB.Items.Add(arrangement);
            }

            MessageBox.Show(CurrentRocksmithTitle + " DLC Template was loaded.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Warning: One or more arrangement have no ArrangementType defined. Guitar has added by default, take a look.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return new XmlTextReader(new StringReader(doc.Document.ToString()));
        }

        private string MakeAbsolute(Uri baseUri, string path)
        {
            return new Uri(baseUri, path).AbsolutePath.Replace("%25", "%").Replace("%20", " ");
        }

        private DLCPackageData GetPackageData()
        {
            if (CurrentGameVersion == GameVersion.RS2014)
            {
                if (!platformPC.Checked && !platformMAC.Checked && !platformXBox360.Checked && !platformPS3.Checked)
                {
                    MessageBox.Show("Error: No game platform selected", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            else
            {
                if (!platformPC.Checked && !platformXBox360.Checked && !platformPS3.Checked)
                {
                    MessageBox.Show("Error: No game platform selected", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            int year, tempo;
            if (String.IsNullOrEmpty(DLCName))
            {
                DlcNameTB.Focus();
                return null;
            }
            if (DLCName == SongTitle) {
                MessageBox.Show("Error: DLC name can't be the same of song name", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DlcNameTB.Focus();
                return null;
            }
            if (String.IsNullOrEmpty(SongTitle))
            {
                SongDisplayNameTB.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(Album))
            {
                AlbumTB.Focus();
                return null;
            }
            if (String.IsNullOrEmpty(Artist))
            {
                ArtistTB.Focus();
                return null;
            }
            if (!Int32.TryParse(AlbumYear, out year))
            {
                YearTB.Focus();
                return null;
            }
            if (!Int32.TryParse(AverageTempo, out tempo))
            {
                AverageTempoTB.Focus();
                return null;
            }
            if (String.IsNullOrEmpty(AppId))
            {
                AppIdTB.Focus();
                return null;
            }

            //Album Art validation (alert only)
            if (String.IsNullOrEmpty(AlbumArtPath) && !File.Exists(AlbumArtPath))
            {
                var diagResult = MessageBox.Show("Warning: Album Art file not found!" + Environment.NewLine +
                                                 "If you click 'Yes' default album art will be defined." + Environment.NewLine +
                                                 "Else you click 'No' you want to select the Album Art File.",
                                                 MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                switch (diagResult)
	            {
		            case DialogResult.No:
                        AlbumArtPathTB.Focus();
                        return null;
                    default:
                        break;
	            }
            }

            if (CurrentGameVersion == GameVersion.RS2014 && !String.IsNullOrEmpty(AlbumArtPath)) {
                if (String.IsNullOrEmpty(AlbumArt256Path) || String.IsNullOrEmpty(AlbumArt128Path) || String.IsNullOrEmpty(AlbumArt64Path))
                    MessageBox.Show("Warning: Album Art 256, 128 or 64 size not found!" + Environment.NewLine +
                                    "Package will generated with default art.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            string oggPreviewPCPath = null;
            if (platformPC.Checked)
            {
                if (!File.Exists(OggPCPath))
                {
                    oggPcPathTB.Focus();
                    return null;
                }
                oggPreviewPCPath = Path.Combine(Path.GetDirectoryName(OggPCPath), String.Format(Path.GetFileNameWithoutExtension(OggPCPath) + "_preview" + Path.GetExtension(OggPCPath)));
                if (!File.Exists(oggPreviewPCPath))
                {
                    if (MessageBox.Show("Warning: Song Preview not found!" + Environment.NewLine +
                                        "File: " + oggPreviewPCPath + Environment.NewLine +
                                        "If you click 'Yes' the song file will be used for the song preview." + Environment.NewLine +
                                        "Else you click 'No' you could fix the problem before package generation.", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        oggPcPathTB.Focus();
                        return null;
                    }
                }
            }

            string oggPreviewMACPath = null;
            if (CurrentGameVersion == GameVersion.RS2014) {
                
                if (platformMAC.Checked)
                {
                    if (!File.Exists(OggMACPath))
                    {
                        oggMacPathTB.Focus();
                        return null;
                    }
                    oggPreviewMACPath = Path.Combine(Path.GetDirectoryName(OggMACPath), String.Format(Path.GetFileNameWithoutExtension(OggMACPath) + "_preview" + Path.GetExtension(OggMACPath)));
                    if (!File.Exists(oggPreviewMACPath))
                    {
                        if (MessageBox.Show("Warning: Song Preview not found!" + Environment.NewLine +
                                            "File: " + oggPreviewMACPath + Environment.NewLine +
                                            "If you click 'Yes' the song file will be used for the song preview." + Environment.NewLine +
                                            "Else you click 'No' you could fix the problem before package generation.", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            oggMacPathTB.Focus();
                            return null;
                        }
                    }
                }
            }

            string oggPreviewXBox360Path = null;
            if (platformXBox360.Checked)
            {
                if (!File.Exists(OggXBox360Path))
                {
                    oggXBox360PathTB.Focus();
                    return null;
                }
                oggPreviewXBox360Path = Path.Combine(Path.GetDirectoryName(OggXBox360Path), String.Format(Path.GetFileNameWithoutExtension(OggXBox360Path) + "_preview" + Path.GetExtension(OggXBox360Path)));
                if (!File.Exists(oggPreviewXBox360Path))
                {
                    if (MessageBox.Show("Warning: Song Preview not found!" + Environment.NewLine +
                                        "File: " + oggPreviewXBox360Path + Environment.NewLine +
                                        "If you click 'Yes' the song file will be used for the song preview." + Environment.NewLine +
                                        "Else you click 'No' you could fix the problem before package generation.", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        oggXBox360PathTB.Focus();
                        return null;
                    }
                }
            }

            string oggPreviewPS3Path = null;
            if (platformPS3.Checked)
            {
                if (!File.Exists(OggPS3Path))
                {
                    oggPS3PathTB.Focus();
                    return null;
                }
                oggPreviewPS3Path = Path.Combine(Path.GetDirectoryName(OggPS3Path), String.Format(Path.GetFileNameWithoutExtension(OggPS3Path) + "_preview" + Path.GetExtension(OggPS3Path)));
                if (!File.Exists(oggPreviewPS3Path))
                {
                    if (MessageBox.Show("Warning: Song Preview not found!" + Environment.NewLine +
                                        "File: " + oggPreviewPS3Path + Environment.NewLine +
                                        "If you click 'Yes' the song file will be used for the song preview." + Environment.NewLine +
                                        "Else you click 'No' you could fix the problem before package generation.", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        oggPS3PathTB.Focus();
                        return null;
                    }
                }
            }

            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            if (arrangements.Count(x => x.ArrangementType == ArrangementType.Vocal) > 1)
            {
                MessageBox.Show("Error: Multiple Vocals arrangement found", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            foreach (var arr in arrangements)
            {
                if (!File.Exists(arr.SongXml.File))
                {
                    MessageBox.Show("Error: Song Xml File doesn't exist: ".Insert(31, arr.SongXml.File), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

            }

            List<Tone> tones = new List<Tone>();
            if (CurrentGameVersion == GameVersion.RS2012)
                tones = TonesLB.Items.OfType<Tone>().ToList();

            List<Tone2014> tonesRS2014 = new List<Tone2014>();
            if (CurrentGameVersion == GameVersion.RS2014)
                tonesRS2014 = TonesLB.Items.OfType<Tone2014>().ToList();

            //string liveSignatureID = xboxLicense0IDTB.Text.Trim();
            //if (rbuttonSignatureLIVE.Checked && String.IsNullOrEmpty(liveSignatureID))
            //{
            //    MessageBox.Show("Error: If LIVE signature is selected, your LIVE signature ID is required (in HEX format)", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    xboxLicense0IDTB.Focus();
            //    return null;
            //}
            //if (rbuttonSignatureLIVE.Checked && !new Regex("([A-Fa-f0-9]{2})+$").IsMatch(liveSignatureID))
            //{
            //    MessageBox.Show("Error: LIVE signature ID seems to be not valid, need a HEX value", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    xboxLicense0IDTB.Focus();
            //    return null;
            //}

            //List<XBox360License> licenses = new List<XBox360License>();
            //if (rbuttonSignatureLIVE.Checked)
            //{
            //    licenses.Add(new XBox360License() { ID = Convert.ToInt64(xboxLicense0IDTB.Text.Trim(), 16), Bit = 1, Flag = 1 });
            //}
            
            var data = new DLCPackageData
            {
                GameVersion = CurrentGameVersion,
                Name = DlcNameTB.Text,
                AppId = AppIdTB.Text,

                SongInfo = new SongInfo
                {
                    SongDisplayName = SongDisplayNameTB.Text,
                    SongDisplayNameSort = String.IsNullOrEmpty(SongDisplayNameSortTB.Text.Trim()) ? SongDisplayNameTB.Text : SongDisplayNameSortTB.Text,
                    Album = AlbumTB.Text,
                    SongYear = year,
                    Artist = ArtistTB.Text,
                    ArtistSort = String.IsNullOrEmpty(ArtistSortTB.Text.Trim()) ? ArtistTB.Text : ArtistSortTB.Text,
                    AverageTempo = tempo
                },

                AlbumArtPath = AlbumArtPath,
                AlbumArt256 = AlbumArt256Path,
                AlbumArt128 = AlbumArt128Path,
                AlbumArt64 = AlbumArt64Path,
                OggPath = OggPCPath,
                OggPreviewPath = oggPreviewPCPath,
                OggMACPath = OggMACPath,
                OggPreviewMACPath = oggPreviewMACPath,
                OggXBox360Path = OggXBox360Path,
                OggPreviewXBox360Path = oggPreviewXBox360Path,
                OggPS3Path = OggPS3Path,
                OggPreviewPS3Path = oggPreviewPS3Path,
                Arrangements = arrangements,
                Tones = tones,
                TonesRS2014 = tonesRS2014,
                Volume = volumeBox.Value,
                SignatureType = PackageMagic.CON
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
                using (var form = new ArrangementForm(arrangement, GetToneNames(), (DLCPackageCreator)this, CurrentGameVersion) { Text = "Edit Arrangement" })
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
            var tone = CreateNewTone();
            using (var form = new ToneForm(tone, CurrentGameVersion))
            {
                form.ShowDialog();

                if (form.Saved)
                {
                    if (form.LoadedTone != null)
                        tone = form.LoadedTone;

                    TonesLB.Items.Add(tone);
                }
            }
        }

        private void toneRemoveButton_Click(object sender, EventArgs e)
        {
            if (TonesLB.SelectedItem != null && TonesLB.Items.Count > 1)
            {
                dynamic tone = TonesLB.SelectedItem;
                TonesLB.Items.Remove(TonesLB.SelectedItem);

                dynamic firstTone = TonesLB.Items[0];
                foreach (var item in ArrangementLB.Items.OfType<Arrangement>())
                {
                    if (tone.Name.Equals(item.ToneBase))
                        item.ToneBase = firstTone.Name;
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
                dynamic tone = TonesLB.SelectedItem;
                dynamic newTone = Copy(tone);
                var toneName = newTone.Name;
                using (var form = new ToneForm(newTone, CurrentGameVersion))
                {
                    form.ShowDialog();
                    
					if (form.Saved)
                    {
                        if (form.LoadedTone != null)
                            tone = form.LoadedTone;
                        
                        tone = newTone;
                        TonesLB.Items[TonesLB.SelectedIndex] = tone;
                    }
                }
                if (toneName != tone.Name)
                {
                    for(int i = 0; i <ArrangementLB.Items.Count; i++) {
                        var arrangement = (Arrangement)ArrangementLB.Items[i];
                        if (toneName.Equals(arrangement.ToneBase))
                        {
                            arrangement.ToneBase = tone.Name;
                            ArrangementLB.Items[i] = arrangement;
                        }
                    }                    
                }
            }
        }

        private void toneImportButton_Click(object sender, EventArgs e)
        {
            string toneImportFile;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select DLC Song Package or Tone File";
                ofd.Filter = CurrentOFDToneImportFilter;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneImportFile = ofd.FileName;
            }

            try
            {
                Application.DoEvents();
                if (CurrentGameVersion == GameVersion.RS2014) {
                    List<Tone2014> tones = Tone2014.Import(toneImportFile);
                    foreach (Tone2014 tone in tones)
                        if (!TonesLB.Items.OfType<Tone2014>().Any(t => t.Key == tone.Key))
                            TonesLB.Items.Add(tone);
                }
                else
                {
                    List<Tone> tones = Tone.Import(toneImportFile);
                    foreach (Tone tone in tones)
                        TonesLB.Items.Add(tone);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't import tone(s). \n\r" + ex.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Tone(s) was imported.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void plataform_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox platformCkb = ((CheckBox)sender);
            bool pChecked = platformCkb.Checked;
            if (platformCkb.Name.IndexOf("MAC") > 0)
            {
                oggMacPathTB.Enabled = pChecked;
                openOggMacButton.Enabled = pChecked;
            }
            else if (platformCkb.Name.IndexOf("XBox360") > 0)
            {
                oggXBox360PathTB.Enabled = pChecked;
                openOggXBox360Button.Enabled = pChecked;
                //panelXBox360SignatureType.Visible = pChecked;
            }
            else if (platformCkb.Name.IndexOf("PS3") > 0)
            {
                oggPS3PathTB.Enabled = pChecked;
                openOggPS3Button.Enabled = pChecked;
            }
            else
            {
                oggPcPathTB.Enabled = pChecked;
                openOggPcButton.Enabled = pChecked;
                AppIdTB.Visible = pChecked;
                cmbAppIds.Visible = pChecked;
            }
        }

        private void AppIdTB_TextChanged(object sender, EventArgs e) {
            var appId = ((TextBox)sender).Text.Trim();
            SelectComboAppId(appId);
        }

        private void SelectComboAppId(string appId) {
            SongAppId songAppId = SongAppIdRepository.Instance().Select(appId, CurrentGameVersion);
            if (SongAppIdRepository.Instance().List.Any<SongAppId>(a => a.AppId == appId))
                cmbAppIds.SelectedItem = songAppId;
        }

        private T Copy<T>(T value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                dcs.WriteObject(stream, value);
                stream.Position = 0;
                return (T)dcs.ReadObject(stream);
            }
        }

        private void DlcNameTB_Leave(object sender, EventArgs e)
        {
            TextBox dlcName = (TextBox)sender;
            dlcName.Text = GetValidName(dlcName.Text.Trim());
        }

        private string GetValidName(string value) {
            string name = String.Empty;
            if (!String.IsNullOrEmpty(value))
            {
                Regex rgx = new Regex("[^a-zA-Z0-9\\-]");
                name = rgx.Replace(value, "");
                if (name == SongTitle)
                    name = name + "Song";
            }
            return name;
        }

        //private void rbuttonSignature_CheckedChanged(object sender, EventArgs e)
        //{
        //    xboxLicense0IDTB.Visible = rbuttonSignatureLIVE.Checked;
        //}

        private void ListBox_KeyDown(object sender, KeyEventArgs e) {
            var control = (ListBox)sender;

            object item = control.SelectedItem;
            int index = control.SelectedIndex;
            int newIndex = index;

            control.Items.RemoveAt(index);
            
            switch ((Keys)e.KeyValue) {
                case Keys.Up:
                case Keys.PageUp:
                case Keys.Add:
                    newIndex--;
                    break;
                case Keys.Down:
                case Keys.PageDown:
                case Keys.Subtract:
                    newIndex++;
                    break;
            }

            if (newIndex >= 0 && newIndex <= control.Items.Count) {
                control.Items.Insert(newIndex, item);
                control.SelectedIndex = index;
            } else {
                control.Items.Insert(index, item);
                control.SelectedIndex = index;
            }

            control.Refresh();
        }

        private void GameVersion_CheckedChanged(object sender, EventArgs e)
        {
            PopulateAppIdCombo();
            PopulateTonesLB();

            // MAC RS2014 only
            platformMAC.Enabled = CurrentGameVersion == GameVersion.RS2014;
            platformMAC.Checked = false;
        }

        private void PopulateAppIdCombo()
        {
            SongAppId firstSong = null;
            cmbAppIds.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(CurrentGameVersion))
            {
                cmbAppIds.Items.Add(song);
                if (firstSong == null)
                {
                    firstSong = song;
                }
            }
            cmbAppIds.SelectedItem = firstSong;
            cmbAppIds.Refresh();
            AppIdTB.Text = firstSong.AppId;
            AppIdTB.Refresh();
        }
    }
}