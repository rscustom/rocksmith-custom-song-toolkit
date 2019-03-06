using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Ookii.Dialogs;
using RocksmithToolkitGUI.DDC;
using RocksmithToolkitLib.DLCPackage.Manifest.Functions;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.XmlRepository;
using X360.STFS;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.XML;
using Control = System.Windows.Forms.Control;
using ProgressBarStyle = System.Windows.Forms.ProgressBarStyle;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib.Conversion;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using RocksmithToolkitLib.ToolkitTone;
using MakePedalSetting = RocksmithToolkitLib.ToolkitTone.ToolkitPedal;


namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class DLCPackageCreator : UserControl
    {
        #region Constants

        private const string TKI_ARRID = "(Arrangement ID by CDLC Creator)";
        private const string TKI_REMASTER = "(Remastered by CDLC Creator)";
        private const string TKI_DDC = "(DDC by CDLC Creator)"; // used to identify config condition
        private const string TKI_RS1 = "(RS1 by CDLC Creator)";

        #endregion

        public static readonly string MESSAGEBOX_CAPTION = "CDLC Package Creator";
        private BackgroundWorker bwGenerate = new BackgroundWorker();
        private StringBuilder errorsFound;
        private bool fixLowBass;
        private bool fixMultiTone;
        private string packageComment;
        // prevents multiple tool tip appearance and gives better action
        private ToolTip tt = new ToolTip();

        public DLCPackageCreator()
        {
            InitializeComponent();

            // it is better to be hidden initially and then unhide when needed
            if (GeneralExtension.IsInDesignMode)
                btnDevUse.Visible = true;

            lstArrangements.AllowDrop = true;
            numAudioQuality.MouseEnter += AudioQuality_MouseEnter;
            rbConvert.MouseEnter += rbConvert_MouseEnter;
            rbRs2014.MouseEnter += rbRs2014_MouseEnter;
            numVolSong.MouseEnter += Volume_MouseEnter;
            numVolPreview.MouseEnter += Volume_MouseEnter;
            // using MouseUp event may result in known VS double clicking glitch
            // when one control is over another control and the topmost
            // control is double clicked then the background control can
            // inadvertantly intercept the second click as a MouseUp event 
            // in this case it clears the GUI and produces undesired results
            rbRs2012.MouseClick += GameVersion_MouseClick;
            rbRs2014.MouseClick += GameVersion_MouseClick;
            rbConvert.MouseClick += GameVersion_MouseClick;
            gbGameVersion.MouseEnter += GameVersion_MouseEnter;

            // Generate package worker
            bwGenerate.DoWork += new DoWorkEventHandler(GeneratePackage);
            bwGenerate.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            bwGenerate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            bwGenerate.WorkerReportsProgress = true;

            AddValidationEventHandlers();

            // this sequence gets done everytime config changes
            try
            {
                ReadConfigSettings();
                PopulateAppIdCombo();
                PopulateTonesLB();
            }
            catch
            {
                /*For mono compatibility*/
            }
        }

        //dirty implementation, it's always true, consider undo\redo manager for actions made+logging maybe?
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public bool IsDirty { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string UnpackedDir { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string DestPath { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public bool PlatformPC
        {
            get { return chkPlatformPC.Checked; }
            set { chkPlatformPC.Checked = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public bool PlatformMAC
        {
            get { return chkPlatformMAC.Checked; }
            set { chkPlatformMAC.Checked = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public bool PlatformXBox360
        {
            get { return chkPlatformXBox360.Checked; }
            set { chkPlatformXBox360.Checked = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public bool PlatformPS3
        {
            get { return chkPlatformPS3.Checked; }
            set { chkPlatformPS3.Checked = value; }
        }

        public string Album
        {
            get { return txtAlbum.Text; }
            set { txtAlbum.Text = value.GetValidAtaSpaceName(); }
        }

        public string AlbumSort
        {
            get { return txtAlbumSort.Text; }
            set { txtAlbumSort.Text = String.IsNullOrEmpty(value) ? Album.GetValidSortableName() : value.GetValidSortableName(); }
        }

        public string AlbumYear
        {
            get { return txtYear.Text; }
            set { txtYear.Text = value.GetValidYear(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string AppId
        {
            get { return txtAppId.Text; }
            set { txtAppId.Text = value.GetValidAppIdSixDigits(); }
        }

        public string Artist
        {
            get { return txtArtist.Text; }
            set { txtArtist.Text = value.GetValidAtaSpaceName(); }
        }

        public string ArtistSort
        {
            get { return txtArtistSort.Text; }
            set { txtArtistSort.Text = String.IsNullOrEmpty(value) ? Artist.GetValidSortableName() : value.GetValidSortableName(); }
        }

        public string AverageTempo
        {
            get { return txtTempo.Text; }
            set { txtTempo.Text = value.GetValidTempo(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public GameVersion PreviousGameVersion { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public GameVersion CurrentGameVersion
        {
            get
            {
                if (rbRs2014.Checked)
                    return GameVersion.RS2014;
                if (rbRs2012.Checked)
                    return GameVersion.RS2012;
                if (rbConvert.Checked)
                    return GameVersion.None;

                throw new InvalidDataException("Game version is missing or is not selected.");
            }
            set
            {
                switch (value)
                {
                    case GameVersion.RS2014:
                        rbRs2014.Checked = true;
                        break;
                    case GameVersion.RS2012:
                        rbRs2012.Checked = true;
                        break;
                    case GameVersion.None:
                        rbConvert.Checked = true;
                        break;
                }
            }
        }

        public string CurrentOFDPackageFilter
        {
            get
            {
                string filter;
                switch (CurrentGameVersion)
                {
                    // case GameVersion.None:
                    case GameVersion.RS2014:
                        filter = CurrentRocksmithTitle + " PC/Mac Package (*.psarc)|*.psarc|";
                        filter += CurrentRocksmithTitle + " XBox360 Package (*_xbox, *.*)|*_xbox; *.*|";
                        filter += CurrentRocksmithTitle + " PS3 Package (*.edat)|*.edat|";
                        filter += CurrentRocksmithTitle + " All Files (*.*)|*.*";
                        break;
                    default:
                        filter = CurrentRocksmithTitle + " PC Package (*.dat)|*.dat|";
                        filter += CurrentRocksmithTitle + " XBox360 Package (*_xbox, *.*)|*_xbox; *.*|";
                        // TODO: add support for RS1 PS3    
                        // filter + CurrentRocksmithTitle + " PS3 Package (*.edat)|*.edat|";
                        filter += CurrentRocksmithTitle + " All Files (*.*)|*.*";
                        break;
                }
                return filter;
            }
        }

        public string DLCKey // appears to be interchageble with SongKey
        {
            get { return txtDlcKey.Text; }
            set { txtDlcKey.Text = value.GetValidKey(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public bool JavaBool { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string LyricArtPath { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string ToolkitVers { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string PackageAuthor { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string PackageRating { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        public string PackageComment
        {
            get
            {
                var tkiComment = CurrentGameVersion == GameVersion.RS2012 ? TKI_RS1 : TKI_REMASTER;

                // add any ToolkitInfo comment
                if (String.IsNullOrEmpty(packageComment))
                    return tkiComment;

                if (!packageComment.Contains("Remastered") || !packageComment.Contains("RS1"))
                    return packageComment + " " + tkiComment;

                return packageComment;
            }
            set { packageComment = value; }
        }

        public string PackageVersion
        {
            get { return txtVersion.Text; }
            set { txtVersion.Text = String.IsNullOrEmpty(value) ? "" : value.GetValidVersion(); }
        }

        public string JapaneseArtistName
        {
            get { return txtJapaneseArtistName.Text; }
            set { txtJapaneseArtistName.Text = value; }
        }

        public string JapaneseSongTitle
        {
            get { return txtJapaneseSongTitle.Text; }
            set { txtJapaneseSongTitle.Text = value; }
        }

        public string SongTitle
        {
            get { return txtSongTitle.Text; }
            set { txtSongTitle.Text = value.GetValidAtaSpaceName(); }
        }

        public string SongTitleSort
        {
            get { return txtSongTitleSort.Text; }
            set { txtSongTitleSort.Text = String.IsNullOrEmpty(value) ? SongTitle.GetValidSortableName() : value.GetValidSortableName(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        private string AlbumArtPath // 512 (RS1)
        {
            get { return txtAlbumArtPath.Text; }
            set { txtAlbumArtPath.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // perma fix to prevent creating a property value in designer
        private string AudioPath
        {
            get { return txtAudioPath.Text; }
            set { txtAudioPath.Text = value; }
        }

        private string CurrentOFDAudioFileFilter
        {
            get
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.None:
                    case GameVersion.RS2014:
                        return "All Supported Files|*.wem;*.ogg;*.wav|Wwise 2013 audio files (*.wem)|*.wem|Ogg Vorbis audio files (*.ogg)|*.ogg|Wave audio files (*.wav)|*.wav";
                    default:
                        return "All Supported Files|*.ogg;*.wav|Wwise 2010 audio files (*.ogg)|*.ogg|Ogg Vorbis audio files (*.ogg)|*.ogg|Wave audio files (*.wav)|*.wav";
                }
            }
        }

        private string CurrentOFDToneImportFilter
        {
            get
            {
                var filter = CurrentOFDPackageFilter + "|";
                filter += CurrentRocksmithTitle + " Song Manifest (*.json)|*.json";
                if (rbRs2012.Checked)
                    filter += "|Rocksmith 2012 Game Save Profile (*_profile)|*_profile";
                else
                    filter += "|Rocksmith 2014 Game Save Profile (*_prfldb)|*_prfldb";

                return filter;
            }
        }

        private string CurrentRocksmithTitle
        {
            get
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.None:
                    case GameVersion.RS2014:
                        return "Rocksmith 2014";
                    default:
                        return "Rocksmith";
                }
            }
        }

        public dynamic CreateNewTone(string toneName = "Default")
        {
            var name = GetUniqueToneName(toneName);
            var allPedals = ToolkitPedal.LoadFromResource(CurrentGameVersion);

            // create usable new tone, not just empty tone name/key
            if (CurrentGameVersion != GameVersion.RS2012)
            {
                //return new Tone2014() { Name = name, Key = name };
                var tone2014 = new Tone2014();
                tone2014.Name = name.GetValidAtaSpaceName();
                tone2014.Key = name.GetValidKey(isTone: true);
                tone2014.GearList.Amp = allPedals.First(p => p.Key == "Amp_OrangeAD50").MakePedalSetting(GameVersion.RS2014);
                tone2014.GearList.Cabinet = allPedals.First(p => p.Key == "Cab_OrangeJimmyBean_57_Cone").MakePedalSetting(GameVersion.RS2014);
                tone2014.ToneDescriptors.Add("$[35720]CLEAN");
                return tone2014;
            }

            // return new Tone { Name = name, Key = name };
            var tone = new Tone();
            tone.Name = name.GetValidAtaSpaceName();
            tone.Key = name.GetValidKey(isTone: true);
            tone.PedalList.Add("Amp", allPedals.First(p => p.Key == "Amp_Fusion").MakePedalSetting(GameVersion.RS2012));
            tone.PedalList.Add("Cabinet", allPedals.First(p => p.Key == "Cab_2X12_Fusion_57_Cone").MakePedalSetting(GameVersion.RS2012));
            return tone;
        }

        public Arrangement GenMetronomeArr(Arrangement arr)
        {
            var mArr = GeneralExtension.Copy(arr);
            var songXml = Song2014.LoadFromFile(mArr.SongXml.File);
            var newXml = Path.GetTempFileName();
            mArr.SongXml = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongXML { File = newXml };
            mArr.SongFile = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongFile { File = "" };
            mArr.ClearCache();
            mArr.BonusArr = true;
            mArr.Id = IdGenerator.Guid();
            mArr.MasterId = RandomGenerator.NextInt();
            mArr.Metronome = Metronome.Itself;
            songXml.ArrangementProperties.Metronome = (int)Metronome.Itself;

            var ebeats = songXml.Ebeats;
            var songEvents = new RocksmithToolkitLib.XML.SongEvent[ebeats.Length];
            for (var i = 0; i < ebeats.Length; i++)
            {
                songEvents[i] = new RocksmithToolkitLib.XML.SongEvent
                    {
                        Code = ebeats[i].Measure == -1 ? "B1" : "B0",
                        Time = ebeats[i].Time
                    };
            }

            songXml.Events = songXml.Events.Union(songEvents, new EqSEvent()).OrderBy(x => x.Time).ToArray();
            using (var stream = File.OpenWrite(mArr.SongXml.File))
                songXml.Serialize(stream, true);

            return mArr;
        }

        public void LoadTemplateFile(string templatePath)
        {
            // Make the paths relative
            var BasePath = Path.GetDirectoryName(templatePath);
            if (String.IsNullOrEmpty(UnpackedDir))
                UnpackedDir = BasePath;

            DLCPackageData info = null;

            try
            {
                if (CurrentGameVersion == GameVersion.RS2014) // rbRs2014.Checked)
                {
                    // check and fix the template compatibility if necessary
                    var templateString = File.ReadAllText(templatePath);

                    if (templateString.Contains("Manifest.Tone\">"))
                    {
                        templateString = templateString.Replace("Manifest.Tone\">", "Manifest2014.Tone\">");
                        File.WriteAllText(templatePath, templateString, Encoding.UTF8);
                    }
                }

                using (var stm = new XmlTextReader(templatePath))
                    info = new DataContractSerializer(typeof(DLCPackageData)).ReadObject(stm) as DLCPackageData;

                if (info == null) throw new InvalidDataException("CDLC Template is null");

                // use AppId to determine GameVersion of dlc.xml template
                CurrentGameVersion = (Convert.ToInt32(info.AppId) < 230000) ? GameVersion.RS2012 : GameVersion.RS2014;
            }
            catch (Exception se)
            {
                MessageBox.Show("Can not load CDLC Template. \n" + se.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var arr in info.Arrangements)
            {
                // load xml comments
                arr.XmlComments = Song2014.ReadXmlComments(Path.Combine(BasePath, arr.SongXml.File));

                // apply bassfix to template info in case arrangement was changed in EOF
                if (arr.ArrangementType == ArrangementType.Bass)
                {
                    // fix old toolkit behavior
                    if (arr.TuningStrings == null)
                    {
                        arr.TuningStrings = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                        continue;
                    }

                    if (rbRs2012.Checked)
                        continue;

                    var bassFix = false;
                    //Low tuning fix for bass, If lowest string is B and bass fix not applied 
                    if (arr.TuningStrings.String0 < -4 && arr.TuningPitch != 220.00)
                        if (fixLowBass)
                            bassFix = true;
                        else
                            bassFix |= MessageBox.Show(@"The bass tuning may be too low.  Apply Low Bass Tuning Fix?" + Environment.NewLine + @"Note: The fix may revert if bass Arrangement is re-saved in EOF.  ", @"Warning ... Low Bass Tuning", MessageBoxButtons.YesNo) == DialogResult.Yes;

                    // Fix Low Bass Tuning
                    if (bassFix && TuningFrequency.ApplyBassFix(arr, fixLowBass))
                    {
                        arr.TuningStrings = Song2014.LoadFromFile(Path.Combine(BasePath, arr.SongXml.File)).Tuning;
                        arr.TuningPitch = 220.00;
                        arr.Tuning = TuningDefinitionRepository.Instance.Detect(arr.TuningStrings, GameVersion.RS2014, false).UIName;

                        //MessageBox.Show("It appears the bass arrangement has been modified in EOF." + Environment.NewLine +
                        //                "It is recommended that you manually 'Add' all the project" + Environment.NewLine +
                        //                "files to the toolkit rather than to use 'Load Template'," + Environment.NewLine +
                        //                "because some template files have been modified in EOF.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            FillPackageCreatorForm(info, templatePath);

            // RS2014 ONLY FIELDS
            chkPlatformMAC.Enabled = CurrentGameVersion != GameVersion.RS2012;
            numVolPreview.Enabled = CurrentGameVersion != GameVersion.RS2012;
            chkJapaneseTitle.Enabled = CurrentGameVersion != GameVersion.RS2012;
            chkShowlights.Enabled = CurrentGameVersion != GameVersion.RS2012;

            if (!ConfigGlobals.IsUnitTest)
                Parent.Focus();
        }

        public string SaveTemplateFile(string templateDir = "", bool validate = true)
        {
            // FIXME: RS1 tone data not being saved (reseting to default)

            var templatePath = String.Empty;
            var fileName = String.Empty;
            var packageDataError = String.Empty;
            DLCPackageData packageData = GetPackageData(validate, out packageDataError);

            try
            {
                fileName = StringExtensions.GetValidShortFileName(ArtistSort, SongTitleSort, CurrentGameVersion.ToString(), ConfigRepository.Instance().GetBoolean("creator_useacronyms"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("CDLC template can not be autosaved." + Environment.NewLine +
                    ex.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!String.IsNullOrEmpty(templateDir) && !String.IsNullOrEmpty(fileName))
            {
                templatePath = Path.Combine(templateDir, fileName + ".dlc.xml");
            }
            else
            {
                using (var ofd = new SaveFileDialog())
                {
                    if (!String.IsNullOrEmpty(UnpackedDir))
                        ofd.InitialDirectory = UnpackedDir;
                    else
                    {
                        try
                        {
                            // use EOF project directory if it exists
                            ofd.InitialDirectory = Path.GetDirectoryName(packageData.Arrangements[0].SongXml.File);
                        }
                        catch
                        {
                            // ignore exeption if the EOF project directory does not exists
                        }
                    }

                    ofd.Title = "Save CDLC Template File as ...";
                    ofd.SupportMultiDottedExtensions = true;
                    ofd.Filter = CurrentRocksmithTitle + " CDLC Template (*.dlc.xml)|*.dlc.xml";
                    ofd.FileName = fileName.GetValidFileName();
                    if (DialogResult.OK != ofd.ShowDialog())
                        return String.Empty;

                    templatePath = ofd.FileName;
                }
            }

            //Make the paths relative
            var BasePath = Path.GetDirectoryName(templatePath);
            if (String.IsNullOrEmpty(UnpackedDir))
                UnpackedDir = BasePath;

            if (!string.IsNullOrEmpty(packageData.AlbumArtPath))
                packageData.AlbumArtPath = packageData.AlbumArtPath.RelativeTo(BasePath);

            string audioPath = packageData.OggPath;
            string audioPreviewPath = packageData.OggPreviewPath;
            if (!String.IsNullOrEmpty(audioPath))
                packageData.OggPath = audioPath.RelativeTo(BasePath);
            if (!String.IsNullOrEmpty(audioPreviewPath))
                packageData.OggPreviewPath = audioPreviewPath.RelativeTo(BasePath);
            if (!String.IsNullOrEmpty(packageData.LyricArtPath))
                packageData.LyricArtPath = packageData.LyricArtPath.RelativeTo(BasePath);

            foreach (var arr in packageData.Arrangements)
            {
                if (String.IsNullOrEmpty(arr.SongXml.File))
                    continue;
                arr.SongXml.File = arr.SongXml.File.RelativeTo(BasePath);
                arr.SongFile.File = "";
                if (!String.IsNullOrEmpty(arr.FontSng))
                    arr.FontSng = arr.FontSng.RelativeTo(BasePath);
            }
            try
            {
                using (var stm = XmlWriter.Create(templatePath, new XmlWriterSettings { CheckCharacters = true, Indent = true }))
                {
                    new DataContractSerializer(typeof(DLCPackageData)).WriteObject(stm, packageData);
                    IsDirty = false;
                }
            }
            catch
            {
                //Re-absolutize the paths
                foreach (var arr in packageData.Arrangements)
                {
                    if (!String.IsNullOrEmpty(arr.SongXml.File))
                        arr.SongXml.File = arr.SongXml.File.AbsoluteTo(BasePath);
                    if (!String.IsNullOrEmpty(arr.SongFile.File))
                        arr.SongFile.File = arr.SongFile.File.AbsoluteTo(BasePath);
                    if (!String.IsNullOrEmpty(arr.FontSng))
                        arr.FontSng = arr.FontSng.AbsoluteTo(BasePath);
                }

                if (!String.IsNullOrEmpty(packageData.LyricArtPath))
                    packageData.LyricArtPath = packageData.LyricArtPath.AbsoluteTo(BasePath);
            }

            //Re-absolutize the paths
            foreach (var arr in packageData.Arrangements)
            {
                if (!String.IsNullOrEmpty(arr.SongXml.File))
                    arr.SongXml.File = arr.SongXml.File.AbsoluteTo(BasePath);
                if (!String.IsNullOrEmpty(arr.SongFile.File))
                    arr.SongFile.File = arr.SongFile.File.AbsoluteTo(BasePath);
                if (!String.IsNullOrEmpty(arr.FontSng))
                    arr.FontSng = arr.FontSng.AbsoluteTo(BasePath);
            }

            if (!String.IsNullOrEmpty(packageData.LyricArtPath))
                packageData.LyricArtPath = packageData.LyricArtPath.AbsoluteTo(BasePath);

            if (String.IsNullOrEmpty(templateDir)) //if in GUI mode.
                MessageBox.Show(CurrentRocksmithTitle + " CDLC template was saved.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return templatePath;
        }

        /// <summary>
        /// Update song XML file (guitar or bass) with user modified DLCPackageData info
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="info"></param>
        public void UpdateSongXml(Arrangement arr, DLCPackageData info)
        {
            if (arr.ArrangementType == ArrangementType.Vocal || arr.ArrangementType == ArrangementType.ShowLight)
                return;

            var song2014 = Song2014.LoadFromFile(arr.SongXml.File);
            song2014.AlbumYear = info.SongInfo.SongYear.ToString();
            song2014.ArtistName = info.SongInfo.Artist;
            song2014.Title = info.SongInfo.SongDisplayName;
            song2014.AlbumName = info.SongInfo.Album;
            song2014.ArtistNameSort = info.SongInfo.ArtistSort;
            song2014.SongNameSort = info.SongInfo.SongDisplayNameSort;
            song2014.AlbumNameSort = info.SongInfo.AlbumSort;
            song2014.AverageTempo = info.SongInfo.AverageTempo;
            song2014.Tuning = arr.TuningStrings;
            song2014.Capo = (byte)arr.CapoFret;
            // all other ArrangementProperties in the xml are set by EOF and not changed by Toolkit (currently)            
            song2014.ArrangementProperties = arr.ArrangementPropeties != null ? arr.ArrangementPropeties : new SongArrangementProperties2014();
            song2014.ArrangementProperties.Represent = arr.Represent ? 1 : 0;
            song2014.ArrangementProperties.BonusArr = arr.BonusArr ? 1 : 0;
            song2014.ArrangementProperties.PathLead = Convert.ToInt32(arr.RouteMask == RouteMask.Lead);
            song2014.ArrangementProperties.PathRhythm = Convert.ToInt32(arr.RouteMask == RouteMask.Rhythm);
            song2014.ArrangementProperties.PathBass = Convert.ToInt32(arr.RouteMask == RouteMask.Bass);
            song2014.ArrangementProperties.RouteMask = (int)arr.RouteMask;
            song2014.ArrangementProperties.StandardTuning = arr.Tuning == "E Standard" ? 1 : 0;
            song2014.ArrangementProperties.BassPick = arr.ArrangementType == ArrangementType.Bass ? (int)arr.PluckedType : 0;

            // TODO: monitor this change
            // Commented out - EOF now properly sets the bonus/represent elements
            // represent is set to "1" by default, if there is a bonus then set represent to "0"
            //songXml.ArrangementProperties.Represent = arr.BonusArr ? 0 : 1;
            // for alternate arrangement then both represent and bonus are set to "0"
            //if (songXml.Part > 1 && !arr.BonusArr)
            //   songXml.ArrangementProperties.Represent = 0;

            if (CurrentGameVersion != GameVersion.RS2012)
            {
                //TODO: before this, check somewhere if autotone present, like update arrangement info in GetPackageData section.
                bool updTones = song2014.Tones != null;
                if (!String.IsNullOrEmpty(arr.ToneBase)) song2014.ToneBase = arr.ToneBase;
                if (!String.IsNullOrEmpty(arr.ToneA))
                {
                    if (updTones)
                        foreach (var t in song2014.Tones)
                            if (t.Name == song2014.ToneA)
                            {
                                t.Name = arr.ToneA;
                                t.Id = 0;
                            }
                    song2014.ToneA = arr.ToneA;
                }
                if (!String.IsNullOrEmpty(arr.ToneB))
                {
                    if (updTones)
                        foreach (var t in song2014.Tones)
                            if (t.Name == song2014.ToneB)
                            {
                                t.Name = arr.ToneB;
                                t.Id = 1;
                            }
                    song2014.ToneB = arr.ToneB;
                }
                if (!String.IsNullOrEmpty(arr.ToneC))
                {
                    if (updTones)
                        foreach (var t in song2014.Tones)
                            if (t.Name == song2014.ToneC)
                            {
                                t.Name = arr.ToneC;
                                t.Id = 2;
                            }
                    song2014.ToneC = arr.ToneC;
                }
                if (!String.IsNullOrEmpty(arr.ToneD))
                {
                    if (updTones)
                        foreach (var t in song2014.Tones)
                            if (t.Name == song2014.ToneD)
                            {
                                t.Name = arr.ToneD;
                                t.Id = 3;
                            }
                    song2014.ToneD = arr.ToneD;
                }
                // write updated xml arrangement
                using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                    song2014.Serialize(stream, true);
            }

            if (CurrentGameVersion == GameVersion.RS2012)
            {
                // detect and maintain old RS1 XML versions
                if (String.IsNullOrEmpty(song2014.Version) || song2014.Version == "4")
                {
                    var song = Song.LoadFromFile(arr.SongXml.File);
                    // write updated xml arrangement as old RS1 XML version
                    using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                        song.Serialize(stream, true);
                }
            }
        }

        private void AddArrangementsQuick(string[] filePaths)
        {
            foreach (var filePath in filePaths)
                AddArrangement(filePath);
        }

        private void AddArrangement(string xmlFilePath = "")
        {
            Arrangement arrangement = null;
            using (var form = new ArrangementForm(this, CurrentGameVersion))
            {
                if (form.IsAlreadyAdded(xmlFilePath))
                    return;

                form.EditMode = false;

                if (!String.IsNullOrEmpty(xmlFilePath))
                {
                    if (!form.AddXmlArrangement(xmlFilePath))
                        return;

                    form.LoadArrangementData(xmlFilePath);
                }
                else
                {
                    if (form.ShowDialog() != DialogResult.OK)
                        return;
                }

                arrangement = form.Arrangement;
            }

            if (arrangement == null)
                return;

            lstArrangements.Items.Add(arrangement);
            IsDirty = true;
        }

        private void AddValidationEventHandlers()
        {
            txtArtist.Validating += ValidateName;
            txtArtistSort.Validating += ValidateSortName;
            txtSongTitle.Validating += ValidateName;
            txtSongTitleSort.Validating += ValidateSortName;
            txtAlbum.Validating += ValidateName;
            txtAlbumSort.Validating += ValidateSortName;
            txtYear.Validating += ValidateYear;
            txtVersion.Validating += ValidateVersion;
            txtDlcKey.Validating += ValidateDlcKey;
            txtVersion.Validating += ClickedInputControl;
            txtTempo.Validating += ValidateTempo;
            btnArrangementAdd.Click += ClickedInputControl;
            btnArrangementEdit.Click += ClickedInputControl;
            btnArrangementRemove.Click += ClickedInputControl;
            btnToneAdd.Click += ClickedInputControl;
            btnToneEdit.Click += ClickedInputControl;
            btnToneRemove.Click += ClickedInputControl;
            btnToneDuplicate.Click += ClickedInputControl;
            btnToneImport.Click += ClickedInputControl;
        }

        private void DuplicateTone()
        {
            if (lstTones.SelectedItem != null && lstTones.Items.Count > 0)
            {
                dynamic tone = lstTones.SelectedItem;
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2012:
                        tone = GeneralExtension.Copy<Tone>((Tone)lstTones.SelectedItem);
                        break;
                    case GameVersion.None:
                    case GameVersion.RS2014:
                        tone = GeneralExtension.Copy<Tone2014>((Tone2014)lstTones.SelectedItem);
                        break;
                }

                var name = GetUniqueToneName(lstTones.Text);
                tone.Name = name;
                tone.Key = name.GetValidKey(isTone: true);
                lstTones.Items.Add(tone);

                dynamic firstTone = lstTones.Items[0];
                foreach (var item in lstArrangements.Items.OfType<Arrangement>())
                    if (tone.Name.Equals(item.ToneBase))
                        item.ToneBase = firstTone.Name;

                lstArrangements.Refresh();
                IsDirty = true;
            }
        }

        public void FillPackageCreatorForm(DLCPackageData info, string filesBaseDir)
        {
            PlatformPC = info.Pc;
            PlatformMAC = info.Mac;
            PlatformXBox360 = info.XBox360;
            PlatformPS3 = info.PS3;

            if (info.ToolkitInfo == null)
                info.ToolkitInfo = new ToolkitInfo { PackageVersion = "1" };

            ToolkitVers = info.ToolkitInfo.ToolkitVersion;
            PackageVersion = info.ToolkitInfo.PackageVersion;
            PackageComment = info.ToolkitInfo.PackageComment;
            PackageAuthor = info.ToolkitInfo.PackageAuthor;
            PackageRating = info.ToolkitInfo.PackageRating; // new for future

            lstTones.Items.Clear();
            switch (CurrentGameVersion)
            {
                case GameVersion.RS2012:
                    if (info.Tones == null)
                        info.Tones = new List<Tone>();
                    if (info.Tones.Count == 0)
                        info.Tones.Add(CreateNewTone());

                    foreach (var tone in info.Tones)
                    {
                        if (String.IsNullOrEmpty(tone.Key)) // JIC should never happen 
                            tone.Key = tone.Name.GetValidKey(isTone: true);

                        lstTones.Items.Add(tone);
                    }
                    break;
                case GameVersion.None:
                case GameVersion.RS2014:
                    if (info.TonesRS2014 == null)
                        info.TonesRS2014 = new List<Tone2014>();
                    if (info.TonesRS2014.Count == 0)
                        info.TonesRS2014.Add(CreateNewTone());

                    foreach (var toneRS2014 in info.TonesRS2014)
                    {
                        if (String.IsNullOrEmpty(toneRS2014.Key))
                            toneRS2014.Key = toneRS2014.Name.GetValidKey(isTone: true);

                        lstTones.Items.Add(toneRS2014);
                    }
                    break;
            }

            var BasePath = Path.GetDirectoryName(filesBaseDir);

            // Song INFO
            txtDlcKey.Text = info.Name;

            // do in case CurrentGameVersion changed
            PopulateAppIdCombo();

            // Update AppID unless it is locked
            if (!ConfigRepository.Instance().GetBoolean("general_lockappid"))
            {
                if (String.IsNullOrEmpty(info.AppId))
                {
                    // get GeneralConfig default AppID
                    var songAppId = SongAppIdRepository.Instance().Select((CurrentGameVersion == GameVersion.RS2014) ? ConfigRepository.Instance()["general_defaultappid_RS2014"] : ConfigRepository.Instance()["general_defaultappid_RS2012"], CurrentGameVersion);
                    if (!String.IsNullOrEmpty(songAppId.AppId))
                        AppId = songAppId.AppId;
                    else
                        AppId = "248750"; // JIC use hardcoded default
                }
                else
                    AppId = info.AppId;
            }
            else
                AppId = info.AppId;

            SelectComboAppId(AppId);

            // validate on-load to address old CDLC issues
            txtAlbum.Text = info.SongInfo.Album;
            txtAlbumSort.Text = info.SongInfo.AlbumSort.GetValidSortableName();
            txtJapaneseSongTitle.Text = info.SongInfo.JapaneseSongName;
            txtJapaneseArtistName.Text = info.SongInfo.JapaneseArtistName;
            chkJapaneseTitle.Checked = !string.IsNullOrEmpty(txtJapaneseSongTitle.Text) || !string.IsNullOrEmpty(txtJapaneseArtistName.Text);
            txtSongTitle.Text = info.SongInfo.SongDisplayName;
            txtSongTitleSort.Text = info.SongInfo.SongDisplayNameSort.GetValidSortableName();
            txtYear.Text = info.SongInfo.SongYear.ToString();
            txtArtist.Text = info.SongInfo.Artist;
            txtArtistSort.Text = info.SongInfo.ArtistSort.GetValidSortableName();
            txtTempo.Text = info.SongInfo.AverageTempo.ToString();

            // fill in the new AlbumSort textbox if it is empty
            if (String.IsNullOrEmpty(txtAlbumSort.Text))
            {
                var useDefaultAuthor = ConfigRepository.Instance().GetBoolean("creator_usedefaultauthor");
                // use default author for AlbumSort or generate
                if (useDefaultAuthor) // && CurrentGameVersion == GameVersion.RS2014)
                    AlbumSort = ConfigRepository.Instance()["general_defaultauthor"].Trim().GetValidSortableName();
                else
                    AlbumSort = Album.GetValidSortableName();
            }

            // Album art
            if (!String.IsNullOrEmpty(info.AlbumArtPath))
            {
                AlbumArtPath = info.AlbumArtPath.AbsoluteTo(BasePath);
                info.ArtFiles = null; // force ArtFiles array to be generated from the AlbumArtPath                
            }

            // Lyric art
            if (!String.IsNullOrEmpty(info.LyricArtPath))
                LyricArtPath = info.LyricArtPath.AbsoluteTo(BasePath);

            // Audio file
            if (!String.IsNullOrEmpty(info.OggPath))
                AudioPath = info.OggPath.AbsoluteTo(BasePath);

            numVolSong.Value = Decimal.Round((decimal)info.Volume, 2);
            numVolPreview.Value = info.PreviewVolume != null ? Decimal.Round((decimal)info.PreviewVolume, 2) : numVolSong.Value;

            if (info.OggQuality > 1 && info.OggQuality < 10)
                numAudioQuality.Value = info.OggQuality;
            else
                numAudioQuality.Value = 4;

            lstArrangements.Items.Clear();
            foreach (var arrangement in info.Arrangements)
            {
                arrangement.SongXml.File = arrangement.SongXml.File.AbsoluteTo(BasePath);

                if (!String.IsNullOrEmpty(arrangement.FontSng))
                    arrangement.FontSng = arrangement.FontSng.AbsoluteTo(BasePath);

                arrangement.ClearCache();

                if (arrangement.Metronome == Metronome.Itself)
                    continue;

                if (arrangement.ToneBase == null)
                {
                    switch (CurrentGameVersion)
                    {
                        case GameVersion.RS2012:
                            arrangement.ToneBase = info.Tones[0].Name;
                            break;
                        case GameVersion.None:
                        case GameVersion.RS2014:
                            arrangement.ToneBase = info.TonesRS2014[0].Name;
                            break;
                    }
                }

                if (arrangement.ArrangementType == ArrangementType.Bass || arrangement.ArrangementType == ArrangementType.Guitar)
                {
                    try
                    {
                        // Load tuning name from Arrangement
                        var add = (arrangement.ArrangementPropeties.PathBass != 1);
                        var version = CurrentGameVersion == GameVersion.RS2012 ? GameVersion.RS2012 : GameVersion.RS2014;
                        var tuning = TuningDefinitionRepository.Instance.Detect(arrangement.TuningStrings, version, add);

                        // Populate Arrangement tuning info
                        arrangement.Tuning = tuning.UIName;
                        arrangement.TuningStrings = tuning.Tuning;
                    }
                    catch
                    {
                        /* ignore old types of *.dlc.xml */
                    }
                }

                lstArrangements.Items.Add(arrangement);
            }

            // forces RS1 XML to be updated
            IsDirty = CurrentGameVersion != GameVersion.RS2014;
        }

        string errorMsg;
        private DLCPackageData GetPackageData(bool validate, out string errorMsg)
        {
            errorMsg = String.Empty;

            if (validate)
            {
                if (CurrentGameVersion != GameVersion.RS2012)
                {
                    if (!PlatformPC && !PlatformMAC && !PlatformXBox360 && !PlatformPS3)
                    {
                        errorMsg = "No game platform selected ...";
                        return null;
                    }
                }
                else
                {
                    if (!PlatformPC && !PlatformXBox360 && !PlatformPS3)
                    {
                        errorMsg = "No game platform selected ...";
                        return null;
                    }
                }

                if (String.IsNullOrEmpty(DLCKey))
                {
                    txtDlcKey.Focus();
                    errorMsg = "DLCKey is missing ...";
                    return null;
                }

                // CRITICAL: RS1 DLCKey CAN NOT be same as SongTitle w/o spaces, causes hanging after tuning
                // Some RS2 ODLC do have same DLCKey as SongTitle w/o spaces and no hanging ... WEIRD
                if (DLCKey == SongTitle.GetValidKey() && CurrentGameVersion == GameVersion.RS2012)
                {
                    DLCKey = SongTitle.GetValidKey(SongTitle); // fix the condition and move on
                    // errorMsg = "<ERROR> DLC Key can't be the same as song name";
                    //txtDlcKey.Focus();
                    //return null;
                }

                /* //ignore since optional
                if (txtJapaneseSongTitle.Enabled && String.IsNullOrEmpty(JapaneseSongTitle))
                {
                    txtJapaneseSongTitle.Focus();
                    errorMsg = "JapaneseSongTitle is missing ...";
                    return null;
                }
                if (txtJapaneseArtist.Enabled && String.IsNullOrEmpty(JapaneseArtist))
                {
                    txtJapaneseArtist.Focus();
                    errorMsg = "JapaneseArtist is missing ...";
                    return null;
                }*/

                if (String.IsNullOrEmpty(SongTitle))
                {
                    txtSongTitle.Focus();
                    errorMsg = "SongTitle is missing ...";
                    return null;
                }
                if (string.IsNullOrEmpty(Album))
                {
                    txtAlbum.Focus();
                    errorMsg = "Album is missing ...";
                    return null;
                }

                if (String.IsNullOrEmpty(Artist))
                {
                    txtArtist.Focus();
                    errorMsg = "Artist is missing ...";
                    return null;
                }

                if (string.IsNullOrEmpty(ArtistSort))
                {
                    txtArtistSort.Focus();
                    errorMsg = "ArtistSort is missing ...";
                    return null;
                }

                if (String.IsNullOrEmpty(SongTitleSort))
                {
                    txtSongTitleSort.Focus();
                    errorMsg = "SongTitleSort is missing ...";
                    return null;
                }

                if (String.IsNullOrEmpty(AlbumSort))
                {
                    txtAlbumSort.Focus();
                    errorMsg = "AlbumSort is missing ...";
                    return null;
                }

                if (String.IsNullOrEmpty(AppId))
                {
                    txtAppId.Focus();
                    errorMsg = "AppId is missing ...";
                    return null;
                }

                if (String.IsNullOrEmpty(PackageVersion))
                {
                    // force user to make entry rather than defaulting
                    // PackageVersion = "1";
                    errorMsg = "PackageVersion is missing ...";
                    txtVersion.Focus();
                    return null;
                }

                if (!PackageVersion.Equals(PackageVersion.GetValidVersion()))
                {
                    errorMsg = String.Format("Package version field contain invalid characters!" + Environment.NewLine +
                                             "Please replace this: {0}" + Environment.NewLine +
                                             "with something like this: 1 or 2.1 or 2.2.1", PackageVersion);
                    txtVersion.Focus();
                    return null;
                }

                //Album Art validation (alert only)
                if (String.IsNullOrEmpty(AlbumArtPath) || !File.Exists(AlbumArtPath))
                {
                    txtAlbumArtPath.Focus();
                    errorMsg = "AlbumArtPath file could not be found ...";
                    return null;
                }

                // NOTE: CDLC produced with older versions of toolkit may not
                // have an audio preview file so it is auto regenerate
                if (!File.Exists(AudioPath))
                {
                    txtAudioPath.Focus();
                    errorMsg = "Audio file could not be found ...";
                    return null;
                }

            } // end of validation

            int year;
            if (!Int32.TryParse(AlbumYear, out year))
            {
                txtYear.Focus();

                if (validate)
                {
                    errorMsg = "Invalid Year ...";
                    return null;
                }
            }

            int tempo;
            if (!Int32.TryParse(AverageTempo, out tempo))
            {
                txtTempo.Focus();

                if (validate)
                {
                    errorMsg = "Invalid AverageTempo ...";
                    return null;
                }
            }

            var arrangements = lstArrangements.Items.OfType<Arrangement>().ToList();

            if (arrangements.Count(x => x.ArrangementType == ArrangementType.Vocal && x.ArrangementName == ArrangementName.Vocals) > 1)
            {
                errorMsg = "<ERROR> Multiple Vocals arrangement found.  " + Environment.NewLine +
                           "Please remove any multiples and retry.";
                return null;
            }

            if (arrangements.Count(x => x.ArrangementType == ArrangementType.Vocal && x.ArrangementName == ArrangementName.JVocals) > 1)
            {
                errorMsg = "<ERROR> Multiple JVocals arrangement found.  " + Environment.NewLine +
                           "Please remove any multiples and retry.";
                return null;
            }

            if (arrangements.Count(x => x.ArrangementType == ArrangementType.ShowLight && x.ArrangementName == ArrangementName.ShowLights) > 1)
            {
                errorMsg = "<ERROR> Multiple Showlights arrangements found.  " + Environment.NewLine +
                           "Please remove any multiples and retry.";
                return null;
            }

            // theoretically the code below should not be called if imported CDLC is properly formed/valid
            // DEVNOTE: CDLC that end up here may be responsible for cross platform conversion failures

            int chorusTime = 4000;
            int previewLength = 30000;

            foreach (Arrangement arr in arrangements)
            {
                if (!File.Exists(arr.SongXml.File))
                {
                    errorMsg = "<ERROR> Song Arrangement Xml file doesn't exist: " + arr.SongXml.File;
                    return null;
                }

                // clear the archive file name
                arr.SongFile.File = "";

                if (arr.ArrangementType == ArrangementType.Bass || arr.ArrangementType == ArrangementType.Guitar)
                {
                    if (chorusTime != 4000)
                        break; // success ... found a better chorusTime

                    var songLength = Song2014.LoadFromFile(arr.SongXml.File).SongLength;
                    if (songLength < 30.0f)
                    {
                        previewLength = (int)(songLength * 1000f) - chorusTime;
                        break; // don't bother with next arrangement for short riffs
                    }

                    if (arr.Sng2014 == null) // should always be true
                    {
                        var sections = Song2014.LoadFromFile(arr.SongXml.File).Sections;
                        if (!sections.Any())
                            continue; // try next arrangement

                        if (sections.Any(x => x.Name.ToLower() == "chorus"))
                            chorusTime = (int)(sections.First(x => x.Name.ToLower() == "chorus").StartTime * 1000f);
                        else
                            chorusTime = (int)(sections.First().StartTime * 1000f);

                        if (chorusTime + 30000 > (int)(sections.Last().StartTime * 1000f))
                            chorusTime = (int)((sections.Last().StartTime - 30) * 1000f);
                    }
                    else // in theory this branch may never get used
                    {
                        var sections = arr.Sng2014.Sections.Sections;
                        if (!sections.Any())
                            continue; // try next arrangement

                        if (sections.Any(x => x.Name.ToString().ToLower() == "chorus"))
                            chorusTime = (int)(sections.First(x => x.Name.ToString().ToLower() == "chorus").StartTime * 1000f);
                        else
                            chorusTime = (int)(sections.First().StartTime * 1000f);

                        if (chorusTime + 30000 > (int)(sections.Last().StartTime * 1000f))
                            chorusTime = (int)((sections.Last().StartTime - 30) * 1000f);
                    }
                }
            }

            try
            {
                if (CurrentGameVersion == GameVersion.RS2012 || Path.GetExtension(AudioPath) == "*.wem")
                {
                    AudioPath.VerifyHeaders();
                }
            }
            catch (InvalidDataException ex)
            {
                errorMsg = ex.Message;
                return null;
            }

            string audioPreviewPath = String.Empty;
            if (CurrentGameVersion != GameVersion.RS2012 && validate)
            {
                // implement reusable audio to WEM conversion code
                AudioPath = OggFile.Convert2Wem(AudioPath, (int)numAudioQuality.Value, previewLength, chorusTime);
                var audioPathNoExt = Path.Combine(Path.GetDirectoryName(AudioPath), Path.GetFileNameWithoutExtension(AudioPath));
                audioPreviewPath = String.Format(audioPathNoExt + "_preview.wem");
            }

            List<Tone> tones = new List<Tone>();
            if (CurrentGameVersion == GameVersion.RS2012)
                tones = lstTones.Items.OfType<Tone>().ToList();

            List<Tone2014> tonesRS2014 = new List<Tone2014>();
            if (CurrentGameVersion != GameVersion.RS2012)
                tonesRS2014 = lstTones.Items.OfType<Tone2014>().ToList();

            var data = new DLCPackageData
                {
                    GameVersion = CurrentGameVersion,
                    Pc = PlatformPC,
                    Mac = PlatformMAC,
                    XBox360 = PlatformXBox360,
                    PS3 = PlatformPS3,
                    Name = txtDlcKey.Text,
                    AppId = txtAppId.Text,

                    SongInfo = new SongInfo
                        {
                            JapaneseArtistName = this.JapaneseArtistName,
                            JapaneseSongName = this.JapaneseSongTitle,
                            SongDisplayName = this.SongTitle,
                            SongDisplayNameSort = this.SongTitleSort,
                            Album = this.Album,
                            AlbumSort = this.AlbumSort,
                            SongYear = year,
                            Artist = this.Artist,
                            ArtistSort = this.ArtistSort,
                            AverageTempo = tempo
                        },

                    ToolkitInfo = new ToolkitInfo
                        {
                            ToolkitVersion = ToolkitVers,
                            PackageAuthor = PackageAuthor,
                            PackageRating = PackageRating,
                            PackageVersion = PackageVersion.GetValidVersion(),
                            PackageComment = PackageComment
                        },

                    AlbumArtPath = AlbumArtPath,
                    LyricArtPath = LyricArtPath,
                    OggPath = AudioPath,
                    OggPreviewPath = audioPreviewPath,
                    OggQuality = numAudioQuality.Value,
                    Arrangements = arrangements,
                    Tones = tones,
                    TonesRS2014 = tonesRS2014,
                    Volume = (float)numVolSong.Value,
                    PreviewVolume = (float)numVolPreview.Value,
                    SignatureType = PackageMagic.CON
                };

            return data;
        }

        private IEnumerable<string> GetToneNames()
        {
            // legacy code not used
            if (CurrentGameVersion != GameVersion.RS2012)
                return lstTones.Items.OfType<Tone2014>().Select(t => t.Name);
            return lstTones.Items.OfType<Tone>().Select(t => t.Name);
        }

        private string GetUniqueToneName(string toneName)
        {
            var uniqueName = toneName;
            bool isUnique = false;
            int ind = 1;

            do
            {
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2012:
                        isUnique = lstTones.Items.OfType<Tone>().All(n => n.Name != uniqueName);
                        break;
                    case GameVersion.None:
                    case GameVersion.RS2014:
                        isUnique = lstTones.Items.OfType<Tone2014>().All(n => n.Name != uniqueName);
                        break;
                }

                if (!isUnique)
                {
                    uniqueName = toneName + (++ind);
                }
            } while (!isUnique);

            return uniqueName;
        }

        private void ImportTone(string toneImportFile)
        {
            // hook up the progress bar
            GlobalExtension.UpdateProgress = pbUpdateProgress;
            GlobalExtension.CurrentOperationLabel = lblCurrentOperation;
            Thread.Sleep(100); // give Globals a chance to initialize
            Application.DoEvents();

            try
            {
                if (CurrentGameVersion != GameVersion.RS2012)
                {
                    var tones2014 = Tone2014.Import(toneImportFile);

                    //Popup ToneImportForm if tones > 1
                    if (tones2014.Count > 1)
                        using (var importForm = new ToneImportForm())
                        {
                            importForm.Tone2014 = tones2014;
                            importForm.PopList();
                            if (importForm.ShowDialog() == DialogResult.OK)
                            {
                                foreach (var tone in importForm.Tone2014.Where(t => !t.GearList.IsNull()))
                                    lstTones.Items.Add(tone);
                            }
                        }
                    else if (tones2014.Count < 2)
                        lstTones.Items.Add(tones2014.FirstOrDefault(t => !t.GearList.IsNull()));
                }
                else
                {
                    List<Tone> tones = Tone.Import(toneImportFile);
                    //Popup ToneImportForm if tones > 1
                    if (tones.Count > 1)
                        using (var importForm = new ToneImportForm())
                        {
                            importForm.Tone = tones;
                            importForm.PopList();
                            if (importForm.ShowDialog() == DialogResult.OK)
                            {
                                foreach (var tone in importForm.Tone.Where(t => t.PedalList.Count != 0))
                                    lstTones.Items.Add(tone);
                            }
                        }
                    else if (tones.Count < 2)
                        lstTones.Items.Add(tones.FirstOrDefault(t => t.PedalList.Count != 0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not import tone(s) from: " + toneImportFile + Environment.NewLine + ex.Message.StripCRLF("  "), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Continuous;
            GlobalExtension.ShowProgress("Tone Data Loaded ...", 100);
            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Marquee;
            GlobalExtension.Dispose();

            Parent.Focus();
        }

        private void PopulateAppIdCombo()
        {
            var currentGameVersion = CurrentGameVersion;
            if (rbConvert.Checked)
                currentGameVersion = GameVersion.RS2014;

            cmbAppIds.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(currentGameVersion))
                cmbAppIds.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select((currentGameVersion == GameVersion.RS2014) ? ConfigRepository.Instance()["general_defaultappid_RS2014"] : ConfigRepository.Instance()["general_defaultappid_RS2012"], currentGameVersion);
            cmbAppIds.SelectedItem = songAppId;
            AppId = songAppId.AppId;
        }

        private void PopulateArrangements(GameVersion oldGameVersion)
        {
            switch (CurrentGameVersion)
            {
                case GameVersion.None:
                case GameVersion.RS2012:
                    if (oldGameVersion != GameVersion.RS2012)
                    {
                        lstArrangements.Items.Clear();
                    }
                    break;
                default:
                    if (oldGameVersion == GameVersion.RS2012)
                    {
                        lstArrangements.Items.Clear();
                    }
                    break;
            }
        }

        private void PopulateAudioTB(GameVersion oldGameVersion)
        {
            // AudioTB
            switch (CurrentGameVersion)
            {
                case GameVersion.None:
                case GameVersion.RS2014:
                    if (oldGameVersion == GameVersion.RS2012)
                        txtAudioPath.Text = "";
                    if (!PlatformMAC && !PlatformPS3 && !PlatformXBox360)
                    {//PC only
                        txtAudioPath.Cue = "Audio to Wwise 2013 converter for Windows (*.wem, *.ogg, *.wav)";
                        label2.Text = @"Song preview is generated automatically if not provided in format 'filename_preview.wem'";
                    }
                    else
                    {
                        txtAudioPath.Cue = "Converted audio on Wwise 2013 for Windows, Mac, XBox360 or PS3 (*.wem)";
                        label2.Text = @"Song preview must have the same file name with '_preview' in the end, eg. 'filename_preview.wem'";
                    }
                    break;
                case GameVersion.RS2012:
                    if (oldGameVersion != GameVersion.RS2012)
                        txtAudioPath.Text = "";

                    txtAudioPath.Cue = "Converted audio on Wwise 2010 for Windows, XBox360 or PS3 (*.ogg)";
                    label2.Text = "";
                    break;
            }
        }

        private void PopulateTonesLB(GameVersion oldGameVersion)
        {
            if (CurrentGameVersion == oldGameVersion)
                return;

            PopulateTonesLB();
        }

        private void PopulateTonesLB()
        {
            lstTones.Items.Clear();

            // check if user has assigned default tone and it exists
            if (!String.IsNullOrEmpty(ConfigGlobals.DefaultToneFile) && File.Exists(ConfigGlobals.DefaultToneFile))
            {
                var tone = CreateNewTone();
                using (var form = new ToneForm())
                {
                    form.EditMode = false;
                    form.CurrentGameVersion = CurrentGameVersion;
                    form.toneControl.CurrentGameVersion = CurrentGameVersion;
                    form.toneControl.Init();
                    form.toneControl.Tone = GeneralExtension.Copy(tone);
                    form.LoadToneFile(ConfigGlobals.DefaultToneFile, false);
                    lstTones.Items.Add(form.toneControl.Tone);
                }
            }
            else
                lstTones.Items.Add(CreateNewTone("Default"));
        }

        private void RemoveTone()
        {
            if (lstTones.Items.Count == 1)
            {
                MessageBox.Show("There must always be at least one tone.  " + Environment.NewLine +
                    "This tone can not be removed.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure to remove the selected tone?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (lstTones.SelectedItem != null && lstTones.Items.Count > 1)
            {
                dynamic tone = lstTones.SelectedItem;
                lstTones.Items.Remove(lstTones.SelectedItem);

                dynamic firstTone = lstTones.Items[0];
                foreach (var item in lstArrangements.Items.OfType<Arrangement>())
                    if (tone.Name.Equals(item.ToneBase) && String.IsNullOrEmpty(item.ToneB))
                        item.ToneBase = firstTone.Name;

                lstArrangements.Refresh();
                IsDirty = true;
            }

            var debubMe = txtAppId.Text;
        }

        private void SelectComboAppId(string appId)
        {
            var songAppId = SongAppIdRepository.Instance().Select(appId, CurrentGameVersion);

            if (SongAppIdRepository.Instance().List.Any<SongAppId>(a => a.AppId == appId))
            {
                cmbAppIds.SelectedItem = songAppId;
            }
            else
            {
                if (!appId.IsAppIdSixDigits())
                    MessageBox.Show("Please enter a valid six digit  " + Environment.NewLine + "App ID before continuing.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                {
                    cmbAppIds.Items.Insert(0, new SongAppId() { Name = "Unknown AppID", AppId = appId });
                    cmbAppIds.SelectedIndex = 0;
                    MessageBox.Show("<WARNING> Unknown AppID ..." + Environment.NewLine +
                                    "This AppID may or may not be valid.  " + Environment.NewLine + Environment.NewLine +
                                    "Please ensure the AppID is valid.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ReadConfigSettings()
        {
            try
            {
                numAudioQuality.Value = ConfigRepository.Instance().GetDecimal("creator_qualityfactor");
                fixLowBass = ConfigRepository.Instance().GetBoolean("creator_fixlowbass");
                fixMultiTone = ConfigRepository.Instance().GetBoolean("creator_fixmultitone");
                ConfigGlobals.DefaultProjectFolder = ConfigRepository.Instance()["creator_defaultproject"];
                ConfigGlobals.DefaultToneFile = ConfigRepository.Instance()["creator_defaulttone"];
                CurrentGameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), ConfigRepository.Instance()["general_defaultgameversion"]);
                var defaultPlatform = (GamePlatform)Enum.Parse(typeof(GamePlatform), ConfigRepository.Instance()["general_defaultplatform"]);

                switch (defaultPlatform)
                {
                    case GamePlatform.Pc:
                        PlatformPC = true;
                        break;
                    case GamePlatform.Mac:
                        PlatformMAC = true;
                        break;
                    case GamePlatform.XBox360:
                        PlatformXBox360 = true;
                        break;
                    case GamePlatform.PS3:
                        PlatformPS3 = true;
                        break;
                }
            }
            catch (Exception)
            {
                throw new FileLoadException("RocksmithToolkitLib.Config.xml is corrupt.  Please reload fresh installation.");
            }
        }

        private void ShowCurrentOperation(string message)
        {
            lblCurrentOperation.Text = message;
            lblCurrentOperation.Refresh();
        }

        private void ValidateVersion(object sender, CancelEventArgs e)
        {
            var dlcVersion = sender as CueTextBox;
            dlcVersion.Text = dlcVersion.Text.Trim().GetValidVersion();
            IsDirty = true;
        }

        private void btnArrangementAdd_Click(object sender, EventArgs e)
        {
            AddArrangement();
        }

        private void btnPackageGenerate_Click(object sender, EventArgs e)
        {
            var diaMsg = String.Empty;
            var wwisePath = Wwise.GetWwisePath();
            var wwiseVersion = FileVersionInfo.GetVersionInfo(wwisePath).ProductVersion;
            if (CurrentGameVersion == GameVersion.RS2012 && !wwiseVersion.StartsWith("2010.3"))
            {
                diaMsg = "Configuration Wwise Path is not set properly for RS1 ...";
                BetterDialog2.ShowDialog(diaMsg, "Generate Button", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                return;
            }

            if (CurrentGameVersion != GameVersion.RS2012 && wwiseVersion.StartsWith("2010"))
            {
                diaMsg = "Configuration Wwise Path is not set properly for RS2014 or Conversions ...";
                BetterDialog2.ShowDialog(diaMsg, "Generate Button", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                return;
            }

            if (String.IsNullOrEmpty(ArtistSort) || String.IsNullOrEmpty(SongTitleSort) || String.IsNullOrEmpty(PackageVersion) || String.IsNullOrEmpty(DLCKey))
            {
                diaMsg = "Can not 'Generate' a package quite yet ..." + Environment.NewLine + Environment.NewLine +
                         "One or more fields are missing information," + Environment.NewLine +
                         "or contain invalid data.";
                BetterDialog2.ShowDialog(diaMsg, "Generate Button", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                var versionPrefix = String.Empty;
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2012:
                        versionPrefix = "r"; // RS2012 CDLC
                        break;
                    case GameVersion.None:
                        versionPrefix = "c"; // Converted RS1 CDLC
                        break;
                    default:
                        versionPrefix = "v"; // RS2014 CDLC
                        break;
                }

                var packageVersion = String.Format("{0}{1}", versionPrefix, PackageVersion);
                var fileName = StringExtensions.GetValidShortFileName(ArtistSort, SongTitleSort, packageVersion, ConfigRepository.Instance().GetBoolean("creator_useacronyms"));
                sfd.FileName = fileName.GetValidFileName();
                sfd.Filter = CurrentRocksmithTitle + " CDLC (*.*)|*.*";

                if (sfd.ShowDialog(this) != DialogResult.OK) // 'this' ensures sfd is topmost
                    return;

                DestPath = sfd.FileName;
            }

            PackageGenerate();
        }

        public DLCPackageData PackageGenerate()
        {
            var packageDataError = String.Empty;
            var packageData = GetPackageData(true, out packageDataError);
            // TODO: may want to change this to a MessageBox with return instead of throwing exception
            if (packageData == null)
                throw new InvalidDataException("<ERROR> DLCPackageData is null, " + packageDataError + Environment.NewLine + Environment.NewLine);

            var playableArrCount = packageData.Arrangements.Count(arr => arr.ArrangementType == ArrangementType.Guitar || arr.ArrangementType == ArrangementType.Bass);
            if (playableArrCount > 5) // may crash RS14R
            {
                var errMsg = "This CDLC will likely crash if it is played in Rocksmith 2014 Remastered." + Environment.NewLine + "The combined number of guitar and bass arrangements" + Environment.NewLine + "(including bonus arrangements) is " + playableArrCount + ", which exceeds the limit of 5." + Environment.NewLine + Environment.NewLine + "Do you still want to package this CDLC?";
                if (MessageBox.Show(errMsg, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                    return null;
            }

            // TODO: monitor this change
            var invalidRepresent = packageData.Arrangements.Any(ar => ar.Represent && ar.BonusArr);
            var hasRepresent = packageData.Arrangements.Any(ar => ar.Represent == true);

            // TODO: confirm before releasing
            // check for duplicate represent/bonus conditions within each RouteMask groups
            //var routeGroup = packageData.Arrangements.GroupBy(x => x.RouteMask).Where(grp => grp.Count() > 1).ToList();
            //foreach (var route in routeGroup)
            //{
            //    var isDefault = false;
            //    var isBonus = false;
            //    var isAlt = false;

            //    foreach (var arrangement in route)
            //    {
            //        if (arrangement.RouteMask == RouteMask.None)
            //            continue;

            //        if (arrangement.Represent && !arrangement.BonusArr)
            //        {
            //            if (isDefault)
            //                illegalRepresent = true;
            //            else
            //                isDefault = true;
            //        }
            //        else if (!arrangement.Represent && arrangement.BonusArr)
            //        {
            //            if (isBonus)
            //                illegalRepresent = true;
            //            else
            //                isBonus = true;
            //        }
            //        else if (!arrangement.Represent && !arrangement.BonusArr)
            //        {
            //            if (isAlt)
            //                illegalRepresent = true;
            //            else
            //                isAlt = true;
            //        }
            //    }
            //}

            if (invalidRepresent && !hasRepresent)
            {
                var diaMsg = "Invalid Arrangement Default/Bonus/Alternate Conditon ..." + Environment.NewLine + Environment.NewLine +
                             "Reauthor using EOF, or open each arrangement seperately" + Environment.NewLine +
                             "using toolkit Edit Arrangement and make necessary changes.  ";
                if (DialogResult.No == BetterDialog2.ShowDialog(diaMsg, "<WARNING> Arrangement Represent", null, "Ignore", "Abort", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning", 150, 150))
                    return null;
            }

            // showlights cause in game hanging for some RS1-RS2 conversions
            // and/or can be defaulted to a minimum set by devs if required
            packageData.DefaultShowlights = chkShowlights.Checked;

            //Generate metronome arrangements
            var mArr = new List<Arrangement>();
            foreach (var arr in packageData.Arrangements)
            {
                if (arr.Metronome == Metronome.Generate)
                    mArr.Add(GenMetronomeArr(arr));
            }

            packageData.Arrangements.AddRange(mArr);

            // Update XML arrangements song info
            bool updateArrangmentID = false;

            if (IsDirty && !ConfigGlobals.IsUnitTest)
            {
                var diaMsg = "The song information has been changed ..." + Environment.NewLine +
                             "Do you want to update 'Arrangement Identification'?  " + Environment.NewLine + Environment.NewLine +
                             "Answering 'Yes' will reduce the risk of in-game" + Environment.NewLine +
                             "hanging and the song stats will be reset to zero.";
                var result = BetterDialog2.ShowDialog(diaMsg, MESSAGEBOX_CAPTION, "Yes", "No", "Abort", Bitmap.FromHicon(SystemIcons.Hand.Handle), "ReadMe ...", 150, 150);

                if (result == DialogResult.Cancel)
                    return null;

                if (result == DialogResult.OK)
                {

                    updateArrangmentID = true;

                    // add TKI_ARRID comment
                    var arrIdComment = packageData.ToolkitInfo.PackageComment;
                    if (String.IsNullOrEmpty(arrIdComment))
                        arrIdComment = TKI_ARRID;
                    else if (!arrIdComment.Contains(TKI_ARRID))
                        arrIdComment = arrIdComment + " " + TKI_ARRID;

                    packageData.ToolkitInfo.PackageComment = arrIdComment;
                }
            }

            // fire up a fake progress bar to show app is alive and well
            var step = (int)Math.Round(1.0 / playableArrCount * 100, 3);
            var progress = 0;
            pbUpdateProgress.Visible = true;
            lblCurrentOperation.Visible = true;
            btnPackageGenerate.Enabled = false;
            pbUpdateProgress.Style = ProgressBarStyle.Continuous;
            pbUpdateProgress.Value = 10;
            ShowCurrentOperation("Updating package data  ...");
            Application.DoEvents();

            // (RS2014 ONLY) 
            if (ConfigRepository.Instance().GetBoolean("ddc_autogen") && packageData.GameVersion != GameVersion.RS2012)
            {
                // add TKI_DDC comment
                var ddcComment = packageData.ToolkitInfo.PackageComment;
                if (String.IsNullOrEmpty(ddcComment))
                    ddcComment = TKI_DDC;
                else if (!ddcComment.Contains(TKI_DDC))
                    ddcComment = ddcComment + " " + TKI_DDC;

                packageData.ToolkitInfo.PackageComment = ddcComment;
            }

            // add TKI_REMASTER or TKI_RS1 comment
            var tkiComment = packageData.GameVersion == GameVersion.RS2012 ? TKI_RS1 : TKI_REMASTER;
            var remasterComment = packageData.ToolkitInfo.PackageComment;
            if (String.IsNullOrEmpty(remasterComment))
                remasterComment = tkiComment;
            else if (!remasterComment.Contains(tkiComment))
                remasterComment = remasterComment + " " + tkiComment;

            packageData.ToolkitInfo.PackageComment = remasterComment;

            // declare one time for DDC generation   
            var consoleOutput = String.Empty;
            DDCSettings.Instance.LoadConfigXml();
            var phraseLen = DDCSettings.Instance.PhraseLen;
            // removeSus may be depricated in latest DDC but left here for comptiblity
            var removeSus = DDCSettings.Instance.RemoveSus;
            var rampPath = DDCSettings.Instance.RampPath;
            var cfgPath = DDCSettings.Instance.CfgPath;

            foreach (var arr in packageData.Arrangements)
            {
                if (updateArrangmentID)
                {
                    // generate new AggregateGraph
                    arr.SongFile = new RocksmithToolkitLib.DLCPackage.AggregateGraph.SongFile() { File = "" };
                    // generate new Arrangement IDs
                    arr.Id = IdGenerator.Guid();
                    arr.MasterId = RandomGenerator.NextInt();
                }

                // showlight and vocal arrangements
                if (arr.ArrangementType == ArrangementType.ShowLight)
                {
                    Song2014.WriteXmlComments(arr.SongXml.File, arr.XmlComments);
                    continue;
                }

                if (arr.ArrangementType == ArrangementType.Vocal)
                {
                    if (packageData.GameVersion == GameVersion.RS2012)
                        continue;

                    // only validate lyrics that do not use a custom font (RS2014 ONLY)
                    if (!arr.CustomFont)
                    {
                        var oldXml = GeneralExtension.CopyToTempFile(arr.SongXml.File);
                        using (var outputStream = new FileStream(arr.SongXml.File, FileMode.Create, FileAccess.ReadWrite))
                        {
                            var vocals2014 = RocksmithToolkitLib.Sng2014HSL.Sng2014FileWriter.ReadVocals(oldXml);
                            // validate lyrics
                            var xmlContent = new Vocals(vocals2014, true);
                            xmlContent.Serialize(outputStream);
                        }
                    }

                    Song2014.WriteXmlComments(arr.SongXml.File, arr.XmlComments);
                    continue;
                }

                progress += step;
                pbUpdateProgress.Value = (progress > 100 ? 100 : progress);

                if (IsDirty)
                    UpdateSongXml(arr, packageData);

                // restore arrangement comments before applying DD
                Song2014.WriteXmlComments(arr.SongXml.File, arr.XmlComments);

                // add DDC to arrangement (RS2014 ONLY)
                if (ConfigRepository.Instance().GetBoolean("ddc_autogen") && packageData.GameVersion != GameVersion.RS2012)
                {
                    var hasDD = false;
                    var songXml = Song2014.LoadFromFile(arr.SongXml.File);
                    var mf = new ManifestFunctions(GameVersion.RS2014);
                    if (mf.GetMaxDifficulty(songXml) != 0)
                        hasDD = true;

                    if (!hasDD)
                    {
                        // apply DD to xml arrangment and add DDC comment
                        var result = DDCreator.ApplyDD(arr.SongXml.File, phraseLen, removeSus, rampPath, cfgPath, out consoleOutput, true);
                        if (result == 1)
                        {
                            var errMsg = "DDC generated an error while processing arrangement:" + Environment.NewLine + arr.SongXml.File + Environment.NewLine;
                            BetterDialog2.ShowDialog(errMsg, "DDC Generated Error", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                        }

                        if (result == 2)
                        {
                            consoleOutput = String.Format("Arrangement file '{0}' => {1}", Path.GetFileNameWithoutExtension(arr.SongXml.File), consoleOutput);
                            BetterDialog2.ShowDialog(consoleOutput, "DDC Generation Info", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                        }
                    }
                    else // Don't overwrite existing DDC by CDLC author
                    {
                        // commented out ... don't nag user with this message
                        // MessageBox.Show("Existing DD content in arrangement: " + arr.Name + " was not changed", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Debug.WriteLine("Existing DD content in arrangement: " + arr.ArrangementName + " was not changed");

                        // add custom xml comment if needed
                        var hasComment = arr.XmlComments.Any(x => x.ToString().Contains("DDC"));
                        if (!hasComment)
                            Song2014.WriteXmlComments(arr.SongXml.File, arr.XmlComments, customComment: "DDC by CDLC author");
                    }
                }

                // put arrangement comments in correct order
                Song2014.WriteXmlComments(arr.SongXml.File);
            }

            if (Path.GetFileName(DestPath).Contains(" ") && PlatformPS3)
            {
                if (!ConfigRepository.Instance().GetBoolean("creator_ps3pkgnamewarn"))
                    MessageBox.Show(String.Format("PS3 package name can't support space character due to encryption limitation. {0} Spaces will be automatic removed for your PS3 package name.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    ConfigRepository.Instance()["creator_ps3pkgnamewarn"] = true.ToString();
            }

            // unit test does not behave well with async background worker
            if (!ConfigGlobals.IsUnitTest)
            {
                if (!bwGenerate.IsBusy && packageData != null)
                {
                    pbUpdateProgress.Style = ProgressBarStyle.Marquee;
                    pbUpdateProgress.Visible = true;
                    lblCurrentOperation.Visible = true;
                    btnPackageGenerate.Enabled = false;
                    bwGenerate.RunWorkerAsync(packageData);
                }
            }

            // used for debugging
            //var jsonObj = JsonConvert.SerializeObject(packageData, Formatting.Indented);
            //File.WriteAllText(String.Format("{0}{1}.txt", @"D:\Temp\dlc\", Path.GetFileNameWithoutExtension(DestPath)), jsonObj);
            return packageData;
        }

        private void btnPackageImport_Click(object sender, EventArgs e)
        {
            string srcPath;
            string destDir;

            // GET PATH
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a CDLC package (archive) to import";
                ofd.Filter = CurrentOFDPackageFilter;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                srcPath = ofd.FileName;
            }

            if (CurrentGameVersion == GameVersion.RS2014)
                if (!srcPath.IsValidPSARC())
                {
                    MessageBox.Show(String.Format("Invalid File Exception:  File '{0}' can not be used by current process.", Path.GetFileName(srcPath)), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.Description = "Select folder to save project artifacts";
                fbd.UseDescriptionForTitle = true;
                fbd.SelectedPath = Path.GetDirectoryName(srcPath) + Path.DirectorySeparatorChar;
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                destDir = fbd.SelectedPath;
            }

            PackageImport(srcPath, destDir);
        }

        /// <summary>
        /// Import a song archive into CDLC Creator GUI
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="destDir"></param>
        /// <returns>DLCPackageData</returns>
        public DLCPackageData PackageImport(string srcPath, string destDir)
        {
            GlobalExtension.UpdateProgress = pbUpdateProgress;
            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Continuous;
            GlobalExtension.CurrentOperationLabel = lblCurrentOperation;
            Thread.Sleep(100); // give Globals a chance to initialize
            Application.DoEvents();

            // UNPACK            
            var srcPlatform = srcPath.GetPlatform();
            if (Path.GetFileNameWithoutExtension(srcPath).EndsWith("_xbox"))
                srcPlatform.version = CurrentGameVersion;

            UnpackedDir = Packer.Unpack(srcPath, destDir, srcPlatform, true);

            // REORGANIZE
            GlobalExtension.ShowProgress("Reorganizing Package Data ...", 35);
            var structured = ConfigRepository.Instance().GetBoolean("creator_structured");
            if (structured && CurrentGameVersion == GameVersion.RS2014)
                DLCPackageData.DoLikeProject(UnpackedDir);

            // LOAD DATA
            GlobalExtension.ShowProgress("Loading Package Data ...", 70);
            DLCPackageData info = null; // DLCPackageData specific to RS2
            if (CurrentGameVersion == GameVersion.RS2014)
                info = DLCPackageData.LoadFromFolder(UnpackedDir, srcPlatform, srcPlatform, fixMultiTone, fixLowBass);
            else
                info = DLCPackageData.RS1LoadFromFolder(UnpackedDir, srcPlatform, rbConvert.Checked);

            switch (srcPlatform.platform)
            {
                case GamePlatform.Pc:
                    info.Pc = true;
                    break;
                case GamePlatform.Mac:
                    info.Mac = true;
                    break;
                case GamePlatform.XBox360:
                    info.XBox360 = true;
                    break;
                case GamePlatform.PS3:
                    info.PS3 = true;
                    break;
            }

            // FILL PACKAGE CREATOR FORM
            FillPackageCreatorForm(info, UnpackedDir);
            GlobalExtension.ShowProgress("Import Package Finished ...", 100);

            if (!ConfigGlobals.IsUnitTest)
                MessageBox.Show(CurrentRocksmithTitle + " CDLC package was imported.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);

            // prevents possible cross threading
            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Marquee;
            GlobalExtension.Dispose();

            // AUTO SAVE CDLC TEMPLATE (as requested by users) for RS2014 ONLY
            if (!rbConvert.Checked)
                IsDirty = true;

            if (!ConfigGlobals.IsUnitTest)
                Parent.Focus();

            return info;
        }

        private void btnTemplateLoad_Click(object sender, EventArgs e)
        {
            //TODO: issue with gameversion
            string dlcTemplatePath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = ConfigGlobals.DefaultProjectFolder;
                ofd.SupportMultiDottedExtensions = true;
                ofd.Filter = CurrentRocksmithTitle + " CDLC Template (*.dlc.xml)|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                dlcTemplatePath = ConfigGlobals.DefaultProjectFolder = ofd.FileName;
            }

            UnpackedDir = Path.GetDirectoryName(dlcTemplatePath);
            LoadTemplateFile(dlcTemplatePath);
        }

        private void btnToneAdd_Click(object sender, EventArgs e)
        {
            var tone = CreateNewTone();
            using (var form = new ToneForm() { Text = "Add Tone" })
            {
                form.EditMode = false;
                form.CurrentGameVersion = CurrentGameVersion;
                form.toneControl.CurrentGameVersion = CurrentGameVersion;
                form.toneControl.Init();
                form.toneControl.Tone = GeneralExtension.Copy(tone);
                form.ShowDialog();

                if (form.Saved)
                {
                    lstTones.Items.Add(form.toneControl.Tone);
                    IsDirty = true;
                }
            }
        }

        private void AudioQuality_MouseEnter(object sender, EventArgs e)
        {
            // Default 4 ~ 128kbps
            tt.IsBalloon = true;
            tt.InitialDelay = 0;
            tt.ShowAlways = true;
            tt.SetToolTip(numAudioQuality, "High Quality 6 ... ODLC Quality 4" +
                                           Environment.NewLine + "Leave audio quality set to (4)" +
                                           Environment.NewLine + "if source audio quality is unknown");
        }

        private void ClickedInputControl(object sender, EventArgs e)
        {
            // well maybe user changed ;)
            //IsDirty = true;
        }

        private void GameVersion_MouseEnter(object sender, EventArgs e)
        {
            PreviousGameVersion = CurrentGameVersion;
        }

        private void GameVersion_MouseClick(object sender, MouseEventArgs e)
        {
            // GameVersion_CheckedChanged usage comes with problems
            // everytime the value of checked is changed the event handler fires

            // GameVersion_MouseUp has known VS glitch when one control is on top of another control

            // DO NOT ResetPackageCreatorForm if converting RS2014 => RS1
            if (PreviousGameVersion == GameVersion.None || PreviousGameVersion == GameVersion.RS2012)
                ResetPackageCreatorForm(PreviousGameVersion);

            // ======== Convert Song2014 XML to Song XML ======== 
            if (!String.IsNullOrEmpty(DLCKey))
            {
                var packageDataError = String.Empty;
                DLCPackageData packageData = GetPackageData(true, out packageDataError);
                foreach (var arr in packageData.Arrangements)
                {
                    if (arr.ArrangementType == ArrangementType.Vocal)
                        continue;
                    if (arr.ArrangementType == ArrangementType.ShowLight)
                    {
                        // remove showlights from arrangement listbox
                        var showlight = lstArrangements.Items.OfType<Arrangement>().First(x => x.ArrangementType == ArrangementType.ShowLight);
                        lstArrangements.SelectedItems.Add(showlight);
                        lstArrangements.Items.Remove(lstArrangements.SelectedItem);
                        continue;
                    }

                    var songXmlPath = arr.SongXml.File;
                    using (var obj = new Rs2014Converter())
                        obj.Song2014File2SongFile(songXmlPath, true);
                }

                // Convert Tones2014 to Tone 
                var tonesRS2014 = lstTones.Items.OfType<Tone2014>().ToList();
                lstTones.Items.Clear();

                foreach (var tone2014 in tonesRS2014)
                {
                    using (var obj1 = new Rs2014Converter())
                    {
                        var tone = obj1.Tone2014toTone(tone2014);
                        lstTones.Items.Add(tone);
                    }
                }

                // Convert RS2014 to RS1 Audio 
                var wwisePath = Wwise.GetWwisePath();
                var wwiseVersion = FileVersionInfo.GetVersionInfo(wwisePath).ProductVersion;
                if (!wwiseVersion.StartsWith("2010.3"))
                    throw new Exception("<ERROR> Configuration Wwise Path is not set properly for RS1 ..." + Environment.NewLine);

                var wem2014 = AudioPath;
                var toolkitFolderPath = Path.GetDirectoryName(wem2014);
                var eofFolderPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(wem2014)), "EOF");
                var fixedOggFile = String.Format("{0}_fixed.ogg", Path.GetFileNameWithoutExtension(wem2014));
                var fixedOggPath = Path.Combine(eofFolderPath, fixedOggFile);
                var wavFile = String.Format("{0}.wav", Path.GetFileNameWithoutExtension(wem2014));
                var wavPath = Path.Combine(eofFolderPath, wavFile);
                var wem2010File = Path.ChangeExtension(Path.GetFileName(wem2014), ".ogg");
                var wem2010Path = Path.Combine(toolkitFolderPath, wem2010File);
                ExternalApps.Ogg2Wav(fixedOggPath, wavPath);
                Wwise.Wav2Wem(wavPath, wem2010Path, (int)numAudioQuality.Value);

                txtAudioPath.Clear();
                txtAudioPath.Text = wem2010Path;

                // add toolkit conversion comment
                packageData.ToolkitInfo.PackageComment += "(RS2014 to RS1 by CDLC Creator)";

                // ======== End Convert ======== 
            }
            else
            {
                PopulateTonesLB(PreviousGameVersion);
                PopulateArrangements(PreviousGameVersion);
                PopulateAudioTB(PreviousGameVersion);
            }

            PopulateAppIdCombo();

            // RS2014 ONLY FIELDS
            chkPlatformMAC.Enabled = CurrentGameVersion != GameVersion.RS2012;
            numVolPreview.Enabled = CurrentGameVersion != GameVersion.RS2012;
            chkJapaneseTitle.Enabled = CurrentGameVersion != GameVersion.RS2012;
            chkShowlights.Enabled = CurrentGameVersion != GameVersion.RS2012;
        }

        public void GeneratePackage(object sender, DoWorkEventArgs e)
        {
            var currentGameVersion = (CurrentGameVersion == GameVersion.RS2012) ? GameVersion.RS2012 : GameVersion.RS2014;
            var packageData = e.Argument as DLCPackageData;
            errorsFound = new StringBuilder();

            var numPlatforms = 0;
            if (PlatformPC)
                numPlatforms++;
            if (PlatformMAC)
                numPlatforms++;
            if (PlatformXBox360)
                numPlatforms++;
            if (PlatformPS3)
                numPlatforms++;

            var step = (int)Math.Round(1.0 / numPlatforms * 100, 0);
            int progress = 0;

            if (PlatformPC)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating PC Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(DestPath, packageData, new Platform(GamePlatform.Pc, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generating PC package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
                }

            if (PlatformMAC)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating Mac Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(DestPath, packageData, new Platform(GamePlatform.Mac, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generating Mac package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
                }

            if (PlatformXBox360)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating XBox 360 Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(DestPath, packageData, new Platform(GamePlatform.XBox360, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generating XBox 360 package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
                }

            if (PlatformPS3)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating PS3 Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(DestPath, packageData, new Platform(GamePlatform.PS3, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generating PS3 package: {0}{1}{0}{2}. {0}PS3 package require 'JAVA x86' (32 bits) installed on your machine to generate properly.{0}", Environment.NewLine, ex.Message, ex.StackTrace));
                }

            // Cache cleanup so we don't serialize or reuse data that could be changed
            packageData.CleanCache();
            e.Result = (numPlatforms == 1 && errorsFound.Length > 0) ? "error" : "generate";

        }

        // capture listbox special keys
        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            var control = (ListBox)sender;

            object item = control.SelectedItem;
            int index = control.SelectedIndex;
            int newIndex = index;

            switch ((Keys)e.KeyValue)
            {
                case Keys.Up:
                case Keys.PageUp:
                    newIndex--;
                    break;
                case Keys.Down:
                case Keys.PageDown:
                    newIndex++;
                    break;
                case Keys.D:
                    DuplicateTone();
                    break;
                case Keys.Delete:
                    RemoveTone();
                    return;
                default:
                    return;
            }

            control.Items.RemoveAt(index);

            if (newIndex >= 0 && newIndex <= control.Items.Count)
            {
                control.Items.Insert(newIndex, item);
                control.SelectedIndex = index;
            }
            else
            {
                control.Items.Insert(index, item);
                control.SelectedIndex = index;
            }

            control.Refresh();
        }

        private void ProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            switch (Convert.ToString(e.Result))
            {
                case "generate":
                    var message = "Package was generated.";
                    if (errorsFound.Length > 0)
                        message = String.Format("Package {2} was generated with errors! See below: {0}{1}", Environment.NewLine, errorsFound, ToolkitVersion.RSTKGuiVersion);
                    else if (ConfigRepository.Instance().GetBoolean("creator_autosavetemplate"))
                        SaveTemplateFile(UnpackedDir);

                    message += String.Format("{0}Would you like to open the folder where the package was generated?{0}", Environment.NewLine);
                    if (MessageBox.Show(message, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start(Path.GetDirectoryName(DestPath));
                    }
                    break;

                case "error":
                    var message2 = String.Format("Package generation {2} failed.  See below: {0}{1}{0}", Environment.NewLine, errorsFound, ToolkitVersion.RSTKGuiVersion);
                    MessageBox.Show(message2, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Parent.Focus();
                    break;
            }

            btnPackageGenerate.Enabled = true;
            pbUpdateProgress.Visible = false;
            lblCurrentOperation.Visible = false;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= pbUpdateProgress.Maximum)
                pbUpdateProgress.Value = e.ProgressPercentage;
            else
                pbUpdateProgress.Value = pbUpdateProgress.Maximum;

            ShowCurrentOperation(e.UserState as string);
        }

        private void ValidateDlcKey(object sender, CancelEventArgs e)
        {
            TextBox dlcKey = sender as TextBox;
            dlcKey.Text = dlcKey.Text.Trim().GetValidKey(SongTitle);
            IsDirty = true;
        }

        private void ValidateName(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            tb.Text = tb.Text.Trim().GetValidAtaSpaceName();
            IsDirty = true;
        }

        private void ValidateSortName(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            tb.Text = tb.Text.Trim().GetValidSortableName();
            IsDirty = true;
        }

        private void ValidateTempo(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            tb.Text = tb.Text.Trim().GetValidTempo();
            IsDirty = true;
        }

        private void ValidateYear(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            tb.Text = tb.Text.Trim().GetValidYear();
            IsDirty = true;
        }

        private void Volume_MouseEnter(object sender, EventArgs e)
        {
            tt.IsBalloon = true;
            tt.InitialDelay = 0;
            tt.ShowAlways = true;
            Control control = (Control)sender;
            string name = control.Name;
            if (name == "numVolSong")
                tt.SetToolTip(numVolSong, "Softer 0, -1, -2 ... Default -7 ... -18, -19, -20 Louder" + Environment.NewLine + "LF (Loudness Factor)");
            else // preview audio volume is normally softer than the main audio
                tt.SetToolTip(numVolPreview, "Softer 0, -1, -2 ... Default -5 ... -18, -19, -20 Louder" + Environment.NewLine + "LF (Loudness Factor)");
        }

        private void btnAlbumArt_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtDlcKey.Text))
            {
                MessageBox.Show("Fill the 'CDLC Name' field first.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDlcKey.Focus();
                return;
            }
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Album Art File (*.dds,*.gif,*.jpg,*.png)|*.dds;*.gif;*.jpg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.FileName.IsValidImage())
                        AlbumArtPath = ofd.FileName;
                    else
                        MessageBox.Show("The selected image is not valid or not supported." + Environment.NewLine +
                            "MimeType doesn't match with file extension!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnArrangementEdit_Click(object sender, EventArgs e)
        {
            if (lstArrangements.SelectedItem != null)
            {
                var arrangement = (Arrangement)lstArrangements.SelectedItem;
                using (var form = new ArrangementForm(arrangement, this, CurrentGameVersion) { Text = "Edit Arrangement" })
                {
                    form.EditMode = true;

                    if (DialogResult.OK != form.ShowDialog())
                        return;

                    arrangement = form.Arrangement;
                }

                lstArrangements.Items[lstArrangements.SelectedIndex] = arrangement;
                IsDirty = true;
            }
        }

        private void btnArrangementQuick_Click(object sender, EventArgs e)
        {
            // use new Custom TreeViewOfd to keep arrangements in correct selected order
            using (var ofd = new TreeViewOfd())
            {
                ofd.InitialDirectory = ConfigGlobals.DefaultProjectFolder;
                ofd.Title = "Multiselect XML Arrangements and Arrange Order ...";
                ofd.Filter = "Rocksmith Arrangement XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                // save last visited project folder (InitialDirectory) to configuration
                ConfigGlobals.DefaultProjectFolder = ofd.InitialDirectory;
                ConfigRepository.Instance()["creator_defaultproject"] = ofd.InitialDirectory;

                List<string> xmlFilePaths = ofd.FileNames;
                AddArrangementsQuick(xmlFilePaths.ToArray());
            }
        }

        private void btnArrangementQuick_MouseEnter(object sender, EventArgs e)
        {
            tt.ToolTipTitle = "USAGE TIP:";
            tt.ToolTipIcon = ToolTipIcon.Info;
            tt.IsBalloon = true;
            tt.InitialDelay = 0;
            tt.ShowAlways = true;

            tt.SetToolTip(btnArrangementQuick,
                          "Add multiple arrangements quickly. Use the " + Environment.NewLine +
                          "file dialog to multiselect arrangements." + Environment.NewLine +
                          "The first arrangement selected will be used to" + Environment.NewLine +
                          "populated the song information on this form." + Environment.NewLine +
                          "Select the bass arrangement first if there is one. " + Environment.NewLine +
                          "Don't forget to update individual tuning, tone" + Environment.NewLine +
                          "and arrangement information using Edit later.");
            tt.Show("", this, 20000); // show for 20 seconds
        }

        private void btnArrangementRemove_Click(object sender, EventArgs e)
        {
            if (lstArrangements.SelectedItem == null)
                return;

            if (MessageBox.Show("Are you sure to remove the selected arrangement?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            lstArrangements.Items.Remove(lstArrangements.SelectedItem);
            IsDirty = true;
        }

        //TODO: allow to choose audio for each arrangement separately. #Lessons, #Multitracks
        //TODO: loudness normalization apply here
        //TODO: detect quality, but suggest keeping it low at 4
        private void btnAudio_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = CurrentOFDAudioFileFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    AudioPath = ofd.FileName;
            }
        }

        /// <summary>
        /// DEVELOPER DEBUG/TEST
        /// </summary>
        private void btnDevUse_Click(object sender, EventArgs e)
        {
            PackageRating = "4";
            return;

            IOExtension.DeleteDirectory(null);

            string srcPath;
            string destPath;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a Source Path ...";
                ofd.Filter = CurrentOFDPackageFilter;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                srcPath = ofd.FileName;
            }

            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.Description = "Select Destination Folder ...";
                fbd.UseDescriptionForTitle = true;
                fbd.SelectedPath = Path.GetDirectoryName(srcPath) + Path.DirectorySeparatorChar;
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                destPath = fbd.SelectedPath;
            }

            var targetPlatform = Packer.GetPlatform(srcPath);
            var unpackedDir = Packer.GetUnpackedDir(srcPath, destPath, targetPlatform);
            var unpackedFolder = Packer.GetUnpackedDir(srcPath, null, targetPlatform);

            var recycleDir = Packer.RecycleUnpackedDir(unpackedDir);
            var recycleFolder = Packer.RecycleUnpackedDir(unpackedFolder);


        }

        private void btnTemplateSave_Click(object sender, EventArgs e)
        {
            SaveTemplateFile("", false);
        }

        private void btnToneDuplicate_Click(object sender, EventArgs e)
        {
            DuplicateTone();
        }

        private void btnToneEdit_Click(object sender, EventArgs e)
        {
            if (lstTones.SelectedItem != null)
            {
                dynamic tone = lstTones.SelectedItem;
                string toneName = tone.Name;

                using (var form = new ToneForm() { Text = "Edit Tone" })
                {
                    form.EditMode = true;
                    var currentGameVersion = CurrentGameVersion != GameVersion.RS2012 ? GameVersion.RS2014 : GameVersion.RS2012;
                    form.CurrentGameVersion = currentGameVersion;
                    form.toneControl.CurrentGameVersion = currentGameVersion;
                    form.toneControl.Init();
                    form.toneControl.Tone = GeneralExtension.Copy(tone);
                    form.ShowDialog();

                    if (form.Saved)
                    {
                        tone = form.toneControl.Tone;
                        lstTones.Items[lstTones.SelectedIndex] = tone;
                        IsDirty = true;
                    }
                }

                if (toneName != tone.Name)
                {
                    // Update tone slots if name are changed
                    for (int i = 0; i < lstArrangements.Items.Count; i++)
                    {
                        var arrangement = (Arrangement)lstArrangements.Items[i];
                        if (arrangement.ArrangementType == ArrangementType.Vocal)
                            continue;
                        if (arrangement.ArrangementType == ArrangementType.ShowLight)
                            continue;

                        // TODO: optimize using common Arrangement.cs method
                        if (CurrentGameVersion == GameVersion.RS2012)
                        {
                            if (toneName.ToLower() == arrangement.ToneBase.ToLower())
                                arrangement.ToneBase = tone.Name;
                        }
                        else
                        {
                            var songXml = Song2014.LoadFromFile(arrangement.SongXml.File);

                            // tested ... this reduces in game hangs
                            // determine correct Tone.Id and update XML
                            Int32 toneId = 0;
                            // recognize that ToneBase alpha case mismatches do exist and process it
                            if (toneName.ToLower() == arrangement.ToneBase.ToLower())
                                songXml.ToneBase = arrangement.ToneBase = tone.Name;

                            if (!String.IsNullOrEmpty(arrangement.ToneA) && toneName.ToLower() == arrangement.ToneA.ToLower())
                                songXml.ToneA = arrangement.ToneA = tone.Name;
                            if (!String.IsNullOrEmpty(arrangement.ToneB) && toneName.ToLower() == arrangement.ToneB.ToLower())
                            {
                                songXml.ToneB = arrangement.ToneB = tone.Name;
                                toneId = 1;
                            }
                            if (!String.IsNullOrEmpty(arrangement.ToneC) && toneName.ToLower() == arrangement.ToneC.ToLower())
                            {
                                songXml.ToneC = arrangement.ToneC = tone.Name;
                                toneId = 2;
                            }
                            if (!String.IsNullOrEmpty(arrangement.ToneD) && toneName.ToLower() == arrangement.ToneD.ToLower())
                            {
                                songXml.ToneD = arrangement.ToneD = tone.Name;
                                toneId = 3;
                            }

                            // update tone name and tone id and accommodate EOF custom tone differences
                            if (songXml.Tones != null)
                            {
                                foreach (var xmlTone in songXml.Tones)
                                {
                                    if (xmlTone.Name.ToLower() == toneName.ToLower() || toneName.ToLower().EndsWith(xmlTone.Name.ToLower())) //todo: SAMENAME tone fix?
                                    {
                                        xmlTone.Name = tone.Name;
                                        xmlTone.Id = toneId;
                                    }
                                }

                                // save changes to xml
                                using (var stream = File.Open(arrangement.SongXml.File, FileMode.Create))
                                    songXml.Serialize(stream, true);
                            }
                        }

                        // force update to tone in arrangement
                        lstArrangements.Items[i] = arrangement;
                    }
                }
            }
        }

        private void btnToneImport_Click(object sender, EventArgs e)
        {
            string[] toneImportFiles;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select CDLC Song Package or Tone File or your Profile";
                ofd.Filter = CurrentOFDToneImportFilter;
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneImportFiles = ofd.FileNames;
            }

            var preToneCount = lstTones.Items.Count;

            foreach (var toneImportFile in toneImportFiles)
            {
                ImportTone(toneImportFile);
            }

            var numTones = lstTones.Items.Count - preToneCount;
            MessageBox.Show("Imported Tones: (" + numTones + ")  ", "CDLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (numTones > 0)
                IsDirty = true;
        }

        private void btnToneRemove_Click(object sender, EventArgs e)
        {
            RemoveTone();
        }

        private void chkPlatform_CheckedChanged(object sender, EventArgs e)
        {
            var cbx = sender as CheckBox;
            if (cbx.Text.Contains("PS3") && !JavaBool)
            {
                JavaBool = true;
                if (!RijndaelEncryptor.IsJavaInstalled())
                {
                    MessageBox.Show("Unable to generate PS3 package, since Java isn't present on this machine.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbx.Checked = cbx.Enabled = false;
                }
            }
            if (PlatformPC || PlatformMAC)
            {
                txtAppId.Enabled = true;
                cmbAppIds.Enabled = true;
            }
            else if (!PlatformPC && !PlatformMAC)
            {
                txtAppId.Enabled = false;
                cmbAppIds.Enabled = false;
            }
        }

        private void cmbAppIds_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbAppIds.SelectedItem != null)
            {
                AppId = ((SongAppId)cmbAppIds.SelectedItem).AppId;
            }
        }

        private void lstArrangement_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnArrangementEdit_Click(sender, e);
        }

        private void lstTone_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnToneEdit_Click(sender, e);
        }

        private void numVolSong_ValueChanged(object sender, EventArgs e)
        {
            //TODO: warn about loudness normalization and that's better to adjust tones volume instead!
            if (numVolPreview.Value == decimal.Parse(numVolSong.Text))
                numVolPreview.Value = numVolSong.Value;
        }

        private void rbConvert_MouseEnter(object sender, EventArgs e)
        {
            tt.IsBalloon = true;
            tt.InitialDelay = 0;
            tt.ShowAlways = true;
            tt.AutoPopDelay = 20000;
            tt.SetToolTip(rbConvert,
                "HINT: To Convert RS1 to RS2014 ..." + Environment.NewLine + Environment.NewLine +
                "1) Activate the 'Convert' radio button" + Environment.NewLine +
                "2) Press the 'Import Package' button" + Environment.NewLine +
                "    and select the RS1 CDLC to convert." + Environment.NewLine +
                "3) Edit 'Song Information' (optional)." + Environment.NewLine +
                "4) Next press the 'Generate' button" + Environment.NewLine +
                "    and create the RS2014 CDLC ... Enjoy");
        }

        private void rbRs2014_MouseEnter(object sender, EventArgs e)
        {
            tt.IsBalloon = true;
            tt.InitialDelay = 0;
            tt.ShowAlways = true;
            tt.AutoPopDelay = 20000;
            tt.SetToolTip(rbRs2014,
                "HINT: To Convert RS2014 to RS1 ..." + Environment.NewLine + Environment.NewLine +
                "1) Activate the 'RS2014' radio button." + Environment.NewLine +
                "2) Press the 'Import Package' button" + Environment.NewLine +
                "    and select the RS2014 CDLC to convert." + Environment.NewLine +
                "3) Change the GameVersion to 'RS2012'." + Environment.NewLine +
                "4) Press the 'Generate' button" + Environment.NewLine +
                "    and create the RS1 CDLC ... Enjoy");
        }

        private void txtAppId_Validating(object sender, CancelEventArgs e)
        {
            var appId = ((TextBox)sender).Text.Trim();
            SelectComboAppId(appId);
        }

        private void txtVersion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != Char.Parse(".") && e.KeyChar != (int)Keys.Back)
                e.Handled = true;
        }

        private class EqSEvent : IEqualityComparer<RocksmithToolkitLib.XML.SongEvent>
        {
            public bool Equals(RocksmithToolkitLib.XML.SongEvent x, RocksmithToolkitLib.XML.SongEvent y)
            {
                if (x == null)
                    return y == null;

                return x.Code == y.Code && x.Time.Equals(y.Time);
            }

            public int GetHashCode(RocksmithToolkitLib.XML.SongEvent obj)
            {
                if (ReferenceEquals(obj, null))
                    return 0;
                return obj.Code.GetHashCode() | obj.Time.GetHashCode();
            }
        }

        private void cbJapaneseTitle_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                txtJapaneseSongTitle.BringToFront();
                txtJapaneseArtistName.BringToFront();
            }
            else
            {
                txtJapaneseSongTitle.SendToBack();
                txtJapaneseArtistName.SendToBack();
            }
        }

        private void txtJapaneseSongTitle_Validating(object sender, CancelEventArgs e)
        {
            ((CueTextBox)sender).Text = ((CueTextBox)sender).Text.TrimEnd();
        }

        private void cbJapaneseTitle_Click(object sender, EventArgs e)
        {
            chkJapaneseTitle.Checked = !((CheckBox)sender).Checked;
        }

        private void ResetPackageCreatorForm(GameVersion oldGameVersion)
        {
            if (CurrentGameVersion == oldGameVersion)
                return;

            // do a quick clear of data
            foreach (Control ctrl in this.gbSongInformation.Controls)
                if (ctrl is TextBox)
                    (ctrl as TextBox).Clear();

            foreach (Control ctrl in this.gbFiles.Controls)
                if (ctrl is TextBox)
                    (ctrl as TextBox).Clear();

            txtDlcKey.Clear();
        }


 
    }
}