using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using RocksmithToolkitLib.Xml;
using Control = System.Windows.Forms.Control;
using ProgressBarStyle = System.Windows.Forms.ProgressBarStyle;
using RocksmithToolkitGUI.Config;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class DLCPackageCreator : UserControl
    {
        #region Constants

        private const string TKI_ARRID = "(Arrangement ID by CDLC Creator)";
        private const string TKI_DDC = "(DDC by CDLC Creator)";
        private const string TKI_REMASTER = "(Remastered by CDLC Creator)";

        #endregion

        public static readonly string MESSAGEBOX_CAPTION = "CDLC Package Creator";
        private BackgroundWorker bwGenerate = new BackgroundWorker();
        private string dlcDestPath;
        private StringBuilder errorsFound;
        private bool fixLowBass;
        private bool fixMultiTone;
        private string packageComment;
        // prevents multiple tool tip appearance and gives better action
        private ToolTip tt = new ToolTip();


        public DLCPackageCreator()
        {
            InitializeComponent();

#if (!DEBUG)
            btnDevUse.Visible = false;
#endif

            lstArrangements.AllowDrop = true;
            numAudioQuality.MouseEnter += AudioQuality_MouseEnter;
            rbConvert.MouseEnter += rbConvert_MouseEnter;
            numVolSong.MouseEnter += Volume_MouseEnter;
            numVolPreview.MouseEnter += Volume_MouseEnter;
            rbRs2012.MouseUp += GameVersion_MouseUp;
            rbRs2014.MouseUp += GameVersion_MouseUp;
            rbConvert.MouseUp += GameVersion_MouseUp;

            // Generate package worker
            bwGenerate.DoWork += GeneratePackage;
            bwGenerate.ProgressChanged += ProgressChanged;
            bwGenerate.RunWorkerCompleted += ProcessCompleted;
            bwGenerate.WorkerReportsProgress = true;
            AddValidationEventHandlers();

            try
            {
                // this sequence gets done everytime config changes
                SetDefaultFromConfig();
                PopulateAppIdCombo();
                PopulateTonesLB();
            }
            catch
            {
                /*For mono compatibility*/
            }
        }

        public bool IsDirty { get; set; }
        public string UnpackedDir { get; set; }

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
                        filter += CurrentRocksmithTitle + " XBox360 Package (*.*)|*.*|";
                        filter += CurrentRocksmithTitle + " PS3 Package (*.edat)|*.edat";
                        break;
                    default:
                        filter = CurrentRocksmithTitle + " PC/Mac Package (*.dat)|*.dat|";
                        filter += CurrentRocksmithTitle + " XBox360 Package (*.*)|*.*";
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

        public bool JavaBool { get; set; }
        public string LyricArtPath { get; set; }
        public string ToolkitVers { get; set; }
        public string PackageAuthor { get; set; }

        public string PackageComment
        {
            get
            {
                // add any ToolkitInfo comment here
                if (String.IsNullOrEmpty(packageComment))
                    return TKI_REMASTER;

                if (!packageComment.Contains("Remastered"))
                    return packageComment + " " + TKI_REMASTER;

                return packageComment;
            }
            set { packageComment = value; }
        }

        public string PackageVersion
        {
            get { return txtVersion.Text; }
            set { txtVersion.Text = String.IsNullOrEmpty(value) ? "" : value.GetValidVersion(); }
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

        private string AlbumArtPath // 512 (RS1)
        {
            get { return txtAlbumArtPath.Text; }
            set { txtAlbumArtPath.Text = value; }
        }

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
                        // TODO: Test WEM generation with non-PC Platforms
                        if (chkPlatformMAC.Checked == chkPlatformPS3.Checked == chkPlatformXBox360.Checked == false)
                            return "All Supported Files|*.wem;*.ogg;*.wav|Wwise 2013 audio files (*.wem)|*.wem|Ogg Vorbis audio files (*.ogg)|*.ogg|Wave audio files (*.wav)|*.wav";

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

            if (CurrentGameVersion != GameVersion.RS2012)
                return new Tone2014() { Name = name, Key = name };
            return new Tone { Name = name, Key = name };
        }

        public Arrangement GenMetronomeArr(Arrangement arr)
        {
            var mArr = GeneralExtensions.Copy(arr);
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
            var songEvents = new RocksmithToolkitLib.Xml.SongEvent[ebeats.Length];
            for (var i = 0; i < ebeats.Length; i++)
            {
                songEvents[i] = new RocksmithToolkitLib.Xml.SongEvent
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
            DLCPackageData info = null;
            try
            {
                if (rbRs2014.Checked)
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
                rbRs2012.Checked = (Convert.ToInt32(info.AppId) < 230000);
                rbRs2014.Checked = (Convert.ToInt32(info.AppId) > 240000);
            }
            catch (Exception se)
            {
                MessageBox.Show("Can not load CDLC Template. \n" + se.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var arr in info.Arrangements)
            {
                // load xml comments
                arr.XmlComments = Song2014.ReadXmlComments(arr.SongXml.File);

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
                        arr.TuningStrings = Song2014.LoadFromFile(arr.SongXml.File).Tuning;
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

            // Application.DoEvents();
            MessageBox.Show(CurrentRocksmithTitle + " CDLC template was loaded.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Parent.Focus();
        }

        public void SaveTemplateFile(string templateDir = "", bool validate = true)
        {
            var templatePath = String.Empty;
            var fileName = String.Empty;
            DLCPackageData packageData = GetPackageData(validate);

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
                        return;

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

            if (CurrentGameVersion != GameVersion.RS2012)
            {
                var songXml = Song2014.LoadFromFile(arr.SongXml.File);
                songXml.AlbumYear = info.SongInfo.SongYear.ToString();
                songXml.ArtistName = info.SongInfo.Artist;
                songXml.Title = info.SongInfo.SongDisplayName;
                songXml.AlbumName = info.SongInfo.Album;
                songXml.ArtistNameSort = info.SongInfo.ArtistSort;
                songXml.SongNameSort = info.SongInfo.SongDisplayNameSort;
                songXml.AlbumNameSort = info.SongInfo.AlbumSort;
                songXml.AverageTempo = info.SongInfo.AverageTempo;
                songXml.Tuning = arr.TuningStrings;
                songXml.Capo = (byte)arr.CapoFret;
                // all other ArrangementProperties in the xml are set by EOF and not changed by Toolkit (currently)
                songXml.ArrangementProperties = arr.ArrangementPropeties;
                songXml.ArrangementProperties.BonusArr = arr.BonusArr ? 1 : 0;
                songXml.ArrangementProperties.PathLead = Convert.ToInt32(arr.RouteMask == RouteMask.Lead);
                songXml.ArrangementProperties.PathRhythm = Convert.ToInt32(arr.RouteMask == RouteMask.Rhythm);
                songXml.ArrangementProperties.PathBass = Convert.ToInt32(arr.RouteMask == RouteMask.Bass);
                songXml.ArrangementProperties.RouteMask = (int)arr.RouteMask;
                songXml.ArrangementProperties.StandardTuning = arr.Tuning == "E Standard" ? 1 : 0;

                if (arr.ArrangementType == ArrangementType.Bass)
                    songXml.ArrangementProperties.BassPick = (int)arr.PluckedType;
                else
                    songXml.ArrangementProperties.BassPick = 0;

                // TODO: monitor this new code for bugs
                // represent is set to "1" by default, if there is a bonus then set represent to "0"
                songXml.ArrangementProperties.Represent = arr.BonusArr ? 0 : 1;

                // for alternate arrangement then both represent and bonus are set to "0"
                if (songXml.Part > 1 && !arr.BonusArr)
                    songXml.ArrangementProperties.Represent = 0;

                //TODO: before this, check somewhere if autotone present, like update arrangement info in GetPackageData section.
                bool updTones = songXml.Tones != null;
                if (!String.IsNullOrEmpty(arr.ToneBase)) songXml.ToneBase = arr.ToneBase;
                if (!String.IsNullOrEmpty(arr.ToneA))
                {
                    if (updTones)
                        foreach (var t in songXml.Tones)
                            if (t.Name == songXml.ToneA)
                            {
                                t.Name = arr.ToneA;
                                t.Id = 0;
                            }
                    songXml.ToneA = arr.ToneA;
                }
                if (!String.IsNullOrEmpty(arr.ToneB))
                {
                    if (updTones)
                        foreach (var t in songXml.Tones)
                            if (t.Name == songXml.ToneB)
                            {
                                t.Name = arr.ToneB;
                                t.Id = 1;
                            }
                    songXml.ToneB = arr.ToneB;
                }
                if (!String.IsNullOrEmpty(arr.ToneC))
                {
                    if (updTones)
                        foreach (var t in songXml.Tones)
                            if (t.Name == songXml.ToneC)
                            {
                                t.Name = arr.ToneC;
                                t.Id = 2;
                            }
                    songXml.ToneC = arr.ToneC;
                }
                if (!String.IsNullOrEmpty(arr.ToneD))
                {
                    if (updTones)
                        foreach (var t in songXml.Tones)
                            if (t.Name == songXml.ToneD)
                            {
                                t.Name = arr.ToneD;
                                t.Id = 3;
                            }
                    songXml.ToneD = arr.ToneD;
                }

                // write updated xml arrangement
                using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                    songXml.Serialize(stream, true);
            }
            else
            {
                var songXml = Song.LoadFromFile(arr.SongXml.File);
                songXml.Title = info.SongInfo.SongDisplayName;
                songXml.AlbumName = info.SongInfo.Album;
                songXml.AlbumYear = info.SongInfo.SongYear.ToString();
                songXml.ArtistName = info.SongInfo.Artist;
                songXml.AverageTempo = info.SongInfo.AverageTempo;
                songXml.Title = info.SongInfo.SongDisplayName;
                songXml.Tuning = arr.TuningStrings;

                // write updated xml arrangement
                using (var stream = File.Open(arr.SongXml.File, FileMode.Create))
                    songXml.Serialize(stream, true);
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
                    if (!form.LoadXmlArrangement(xmlFilePath))
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
                        tone = GeneralExtensions.Copy<Tone>((Tone)lstTones.SelectedItem);
                        break;
                    case GameVersion.None:
                    case GameVersion.RS2014:
                        tone = GeneralExtensions.Copy<Tone2014>((Tone2014)lstTones.SelectedItem);
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

        private void FillPackageCreatorForm(DLCPackageData info, string filesBaseDir)
        {
            chkPlatformPC.Checked = info.Pc;
            chkPlatformMAC.Checked = info.Mac;
            chkPlatformXBox360.Checked = info.XBox360;
            chkPlatformPS3.Checked = info.PS3;

            if (info.ToolkitInfo == null)
                info.ToolkitInfo = new ToolkitInfo { PackageVersion = "1" };

            ToolkitVers = info.ToolkitInfo.ToolkitVersion;
            PackageVersion = info.ToolkitInfo.PackageVersion;
            PackageComment = info.ToolkitInfo.PackageComment;
            PackageAuthor = info.ToolkitInfo.PackageAuthor;

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
                        if (String.IsNullOrEmpty(tone.Key))
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

            PopulateAppIdCombo();
            Application.DoEvents();

            txtAppId.Text = info.AppId;
            if (String.IsNullOrEmpty(txtAppId.Text))
                txtAppId.Text = info.AppId = "248750"; // hardcoded for now

            SelectComboAppId(info.AppId);
            txtAlbum.Text = info.SongInfo.Album;
            txtAlbumSort.Text = info.SongInfo.AlbumSort;
            txtSongTitle.Text = info.SongInfo.SongDisplayName;
            txtSongTitleSort.Text = info.SongInfo.SongDisplayNameSort;
            txtYear.Text = info.SongInfo.SongYear.ToString();
            txtArtist.Text = info.SongInfo.Artist;
            txtArtistSort.Text = info.SongInfo.ArtistSort;
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
            AlbumArtPath = info.AlbumArtPath.AbsoluteTo(BasePath);
            // forces the ArtFiles array to be generated from the AlbumArtPath
            if (!String.IsNullOrEmpty(AlbumArtPath))
                info.ArtFiles = null;

            // Lyric art
            if (!String.IsNullOrEmpty(info.LyricArtPath))
                LyricArtPath = info.LyricArtPath.AbsoluteTo(BasePath);

            // Audio file
            if (!String.IsNullOrEmpty(info.OggPath))
                AudioPath = info.OggPath.AbsoluteTo(BasePath);

            numVolSong.Value = Decimal.Round((decimal)info.Volume, 2);
            numVolPreview.Value = (info.PreviewVolume != null) ? Decimal.Round((decimal)info.PreviewVolume, 2) : numVolSong.Value;

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

        private DLCPackageData GetPackageData(bool validate = true)
        {
            if (validate)
            {
                if (CurrentGameVersion != GameVersion.RS2012)
                {
                    if (!chkPlatformPC.Checked && !chkPlatformMAC.Checked && !chkPlatformXBox360.Checked && !chkPlatformPS3.Checked)
                    {
                        MessageBox.Show("Error: No game platform selected", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                else
                {
                    if (!chkPlatformPC.Checked && !chkPlatformXBox360.Checked && !chkPlatformPS3.Checked)
                    {
                        MessageBox.Show("Error: No game platform selected", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }

                if (String.IsNullOrEmpty(DLCKey))
                {
                    txtDlcKey.Focus();
                    return null;
                }

                // actually some ODLC do have same DLCKey as SongTitle so commented this conditional check out 
                //if (DLCKey == SongTitle)
                //{
                //    MessageBox.Show("Error: DLC Key can't be the same of song name", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    DlcKeyTB.Focus();
                //    return null;
                //}

                if (String.IsNullOrEmpty(SongTitle))
                {
                    txtSongTitle.Focus();
                    return null;
                }
                if (string.IsNullOrEmpty(Album))
                {
                    txtAlbum.Focus();
                    return null;
                }

                if (String.IsNullOrEmpty(Artist))
                {
                    txtArtist.Focus();
                    return null;
                }

                if (string.IsNullOrEmpty(ArtistSort))
                {
                    txtArtistSort.Focus();
                    return null;
                }

                if (String.IsNullOrEmpty(SongTitleSort))
                {
                    txtSongTitleSort.Focus();
                    return null;
                }

                if (String.IsNullOrEmpty(AlbumSort))
                {
                    txtAlbumSort.Focus();
                    return null;
                }

                if (String.IsNullOrEmpty(AppId))
                {
                    txtAppId.Focus();
                    return null;
                }

                if (String.IsNullOrEmpty(PackageVersion))
                {
                    // force user to make entry rather than defaulting
                    // PackageVersion = "1";
                    txtVersion.Focus();
                    return null;
                }

                if (!PackageVersion.Equals(PackageVersion.GetValidVersion()))
                {
                    MessageBox.Show(String.Format("Package version field contain invalid characters!\n" + "Please replace this: {0}\n" + "with something like this: 1 or 2.1 or 2.2.1", PackageVersion), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtVersion.Focus();
                    return null;
                }

                //Album Art validation (alert only)
                if (String.IsNullOrEmpty(AlbumArtPath) || !File.Exists(AlbumArtPath))
                {
                    var diagResult = MessageBox.Show("Album Artwork not found." + Environment.NewLine + "Default album art will be used." + Environment.NewLine + "Click 'Yes' to continue or 'No' to select Album Artwork.", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (diagResult == DialogResult.No)
                    {
                        txtAlbumArtPath.Focus();
                        return null;
                    }
                }

                // NOTE: CDLC produced with older versions of toolkit may not
                // have an audio preview file so it is auto regenerate here
                if (!File.Exists(AudioPath))
                {
                    txtAudioPath.Focus();
                    return null;
                }

            } // end of validation

            int year;
            if (!Int32.TryParse(AlbumYear, out year))
            {
                txtYear.Focus();

                if (validate)
                    return null;
            }

            int tempo;
            if (!Int32.TryParse(AverageTempo, out tempo))
            {
                txtTempo.Focus();

                if (validate)
                    return null;
            }

            var arrangements = lstArrangements.Items.OfType<Arrangement>().ToList();

            if (arrangements.Count(x => x.ArrangementType == ArrangementType.Vocal && x.Name == ArrangementName.Vocals) > 1)
            {
                MessageBox.Show("Error: Multiple Vocals arrangement found.  " + Environment.NewLine +
                                "Please remove any multiples and retry.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (arrangements.Count(x => x.ArrangementType == ArrangementType.Vocal && x.Name == ArrangementName.JVocals) > 1)
            {
                MessageBox.Show("Error: Multiple JVocals arrangement found.  " + Environment.NewLine +
                                "Please remove any multiples and retry.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (arrangements.Count(x => x.ArrangementType == ArrangementType.ShowLight && x.Name == ArrangementName.ShowLights) > 1)
            {
                MessageBox.Show("Error: Multiple Showlights arrangements found.  " + Environment.NewLine +
                                "Please remove any multiples and retry.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // theoretically the code below should not be called if imported CDLC is properly formed/valid
            // TODO: CDLC that end up here may be responsible for cross platform conversion failures

            int chorusTime = 4000;
            int previewLength = 30000;

            foreach (Arrangement arr in arrangements)
            {
                if (!File.Exists(arr.SongXml.File))
                {
                    MessageBox.Show("Error: Song Arrangement Xml file doesn't exist: " + arr.SongXml.File, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    AudioPath.VerifyHeaders();
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(ex.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var audioPreviewPath = String.Empty;
            if (CurrentGameVersion != GameVersion.RS2012 && validate)
            {
                // implement reusable audio to WEM conversion code
                AudioPath = OggFile.Convert2Wem(AudioPath, (int)numAudioQuality.Value, previewLength, chorusTime);
                var audioPathNoExt = Path.Combine(Path.GetDirectoryName(AudioPath), Path.GetFileNameWithoutExtension(AudioPath));
                audioPreviewPath = String.Format(audioPathNoExt + "_preview.wem");
            }

            var tones = new List<Tone>();
            if (CurrentGameVersion == GameVersion.RS2012)
                tones = lstTones.Items.OfType<Tone>().ToList();

            var tonesRS2014 = new List<Tone2014>();
            if (CurrentGameVersion != GameVersion.RS2012)
                tonesRS2014 = lstTones.Items.OfType<Tone2014>().ToList();

            //TODO FIXME:
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

            var songVol = (float)numVolSong.Value;
            var previewVol = (float)numVolPreview.Value;
            var audioQualiy = numAudioQuality.Value;
            var data = new DLCPackageData
                {
                    GameVersion = CurrentGameVersion,
                    Pc = chkPlatformPC.Checked,
                    Mac = chkPlatformMAC.Checked,
                    XBox360 = chkPlatformXBox360.Checked,
                    PS3 = chkPlatformPS3.Checked,
                    Name = txtDlcKey.Text,
                    AppId = txtAppId.Text,

                    SongInfo = new SongInfo
                        {
                            SongDisplayName = txtSongTitle.Text,
                            SongDisplayNameSort = String.IsNullOrEmpty(txtSongTitleSort.Text.Trim()) ? txtSongTitle.Text : txtSongTitleSort.Text,
                            Album = txtAlbum.Text,
                            AlbumSort = txtAlbumSort.Text,
                            SongYear = year,
                            Artist = txtArtist.Text,
                            ArtistSort = String.IsNullOrEmpty(txtArtistSort.Text.Trim()) ? txtArtist.Text : txtArtistSort.Text,
                            AverageTempo = tempo
                        },

                    ToolkitInfo = new ToolkitInfo
                        {
                            ToolkitVersion = ToolkitVers,
                            PackageAuthor = PackageAuthor,
                            PackageVersion = PackageVersion.GetValidVersion(),
                            PackageComment = PackageComment
                        },

                    AlbumArtPath = AlbumArtPath,
                    LyricArtPath = LyricArtPath,
                    OggPath = AudioPath,
                    OggPreviewPath = audioPreviewPath,
                    OggQuality = audioQualiy,
                    Arrangements = arrangements,
                    Tones = tones,
                    TonesRS2014 = tonesRS2014,
                    Volume = songVol,
                    PreviewVolume = previewVol,
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
            // takes into account possible conversion
            var currentGameVersion = GameVersion.RS2014;
            if (CurrentGameVersion == GameVersion.RS2012)
                currentGameVersion = GameVersion.RS2012;

            cmbAppIds.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(currentGameVersion))
                cmbAppIds.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select((currentGameVersion == GameVersion.RS2014) ? ConfigRepository.Instance()["general_defaultappid_RS2014"] : ConfigRepository.Instance()["general_defaultappid_RS2012"], currentGameVersion);
            cmbAppIds.SelectedItem = songAppId;
            txtAppId.Text = songAppId.AppId;
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
                    numAudioQuality.Enabled = true;
                    if (oldGameVersion == GameVersion.RS2012)
                        txtAudioPath.Text = "";
                    if (!chkPlatformMAC.Checked && !chkPlatformPS3.Checked && !chkPlatformXBox360.Checked)
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
                default:
                    if (oldGameVersion != GameVersion.RS2012)
                        txtAudioPath.Text = "";
                    chkPlatformMAC.Checked = false;
                    numAudioQuality.Enabled = false;
                    txtAudioPath.Cue = "Converted audio on Wwise 2010 for Windows, XBox360 or PS3 (*.ogg)";
                    label2.Text = "";
                    break;
            }
        }

        private void PopulateTonesLB(GameVersion oldGameVersion)
        {
            switch (CurrentGameVersion)
            {
                case GameVersion.None:
                case GameVersion.RS2014:
                    if (oldGameVersion == GameVersion.RS2012)
                    {
                        PopulateTonesLB();
                    }
                    break;
                default:
                    if (oldGameVersion != GameVersion.RS2012)
                    {
                        PopulateTonesLB();
                    }
                    break;
            }
        }

        private void PopulateTonesLB()
        {
            lstTones.Items.Clear();

            // check if user has assigned default tone and it exists
            if (!String.IsNullOrEmpty(Globals.DefaultToneFile) && File.Exists(Globals.DefaultToneFile))
            {
                var tone = CreateNewTone();
                using (var form = new ToneForm())
                {
                    form.EditMode = false;
                    form.CurrentGameVersion = CurrentGameVersion;
                    form.toneControl.CurrentGameVersion = CurrentGameVersion;
                    form.toneControl.Init();
                    form.toneControl.Tone = GeneralExtensions.Copy(tone);
                    form.LoadToneFile(Globals.DefaultToneFile, false);
                    lstTones.Items.Add(form.toneControl.Tone);
                }
            }
            else
                lstTones.Items.Add(CreateNewTone());
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
        }

        private void SelectComboAppId(string appId)
        {
            var songAppId = SongAppIdRepository.Instance().Select(appId, CurrentGameVersion);
            if (SongAppIdRepository.Instance().List.Any<SongAppId>(a => a.AppId == appId))
                cmbAppIds.SelectedItem = songAppId;
            else//TODO: combobox
            {
                if (!appId.IsAppIdSixDigits())
                    MessageBox.Show("Please enter a valid six digit  " + Environment.NewLine + "App ID before continuing.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    MessageBox.Show("User entered an unknown AppID." + Environment.NewLine + Environment.NewLine + "Toolkit will use the AppID that  " + Environment.NewLine + "was entered manually but it can  " + Environment.NewLine + "not assess its validity.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetDefaultFromConfig()
        {
            // read from RocksmithToolkitLib.Config.xml
            try
            {

                fixLowBass = ConfigRepository.Instance().GetBoolean("creator_fixlowbass");
                fixMultiTone = ConfigRepository.Instance().GetBoolean("creator_fixmultitone");
                Globals.DefaultProjectDir = ConfigRepository.Instance()["creator_defaultproject"];
                Globals.DefaultToneFile = ConfigRepository.Instance()["creator_defaulttone"];
                CurrentGameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), ConfigRepository.Instance()["general_defaultgameversion"]);
                var defaultPlatform = (GamePlatform)Enum.Parse(typeof(GamePlatform), ConfigRepository.Instance()["general_defaultplatform"]);

                switch (defaultPlatform)
                {
                    case GamePlatform.Pc:
                        chkPlatformPC.Checked = true;
                        break;
                    case GamePlatform.Mac:
                        chkPlatformMAC.Checked = true;
                        break;
                    case GamePlatform.XBox360:
                        chkPlatformXBox360.Checked = true;
                        break;
                    case GamePlatform.PS3:
                        chkPlatformPS3.Checked = true;
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

        public void btnArrangementAdd_Click(object sender = null, EventArgs e = null)
        {
            AddArrangement();
        }

        public void btnPackageGenerate_Click(object sender = null, EventArgs e = null)
        {
            var packageData = GetPackageData();
            if (packageData == null || String.IsNullOrEmpty(txtDlcKey.Text))
            {
                MessageBox.Show("One or more fields are missing information.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var playableArrCount = packageData.Arrangements.Count(arr => arr.ArrangementType == ArrangementType.Guitar || arr.ArrangementType == ArrangementType.Bass);
            if (playableArrCount > 5) // may crash RS14R
            {
                var errMsg = "This CDLC will likely crash if it is played in Rocksmith 2014 Remastered." + Environment.NewLine + "The combined number of guitar and bass arrangements" + Environment.NewLine + "(including bonus arrangements) is " + playableArrCount + ", which exceeds the limit of 5." + Environment.NewLine + Environment.NewLine + "Do you still want to package this CDLC?";
                if (MessageBox.Show(errMsg, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                    return;
            }

            // check if ANY arrangment has pre existing DD
            var isDD = false;
            foreach (var arr in packageData.Arrangements)
            {
                // skip vocal and showlight arrangements
                if (arr.ArrangementType == ArrangementType.Vocal || arr.ArrangementType == ArrangementType.ShowLight)
                    continue;

                var songXml = Song2014.LoadFromFile(arr.SongXml.File);
                var mf = new ManifestFunctions(GameVersion.RS2014);
                if (mf.GetMaxDifficulty(songXml) != 0)
                {
                    isDD = true;
                    break;
                }
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

                var packageVersion = String.Format("{0}{1}", versionPrefix, PackageVersion.Replace(".", "_"));
                var ddAcronym = ConfigRepository.Instance().GetBoolean("ddc_autogen") || isDD ? "DD" : "NDD";
                var fileName = String.Format("{0}_{1}", StringExtensions.GetValidShortFileName(ArtistSort, SongTitleSort, packageVersion, ConfigRepository.Instance().GetBoolean("creator_useacronyms")), ddAcronym);
                sfd.FileName = fileName.GetValidFileName();
                sfd.Filter = CurrentRocksmithTitle + " CDLC (*.*)|*.*";

                if (sfd.ShowDialog(this) != DialogResult.OK) // 'this' ensures sfd is topmost
                    return;

                dlcDestPath = sfd.FileName;
            }

            // showlights cause in game hanging for some RS1-RS2 conversions
            // and/or can be defaulted to a minimum set by devs if required
            packageData.DefaultShowlights = chkShowlights.Checked;

            //Generate metronome arrangements here
            var mArr = new List<Arrangement>();
            foreach (var arr in packageData.Arrangements)
            {
                if (arr.Metronome == Metronome.Generate)
                    mArr.Add(GenMetronomeArr(arr));
            }

            packageData.Arrangements.AddRange(mArr);

            // Update XML arrangements song info
            bool updateArrangmentID = false;
            if (IsDirty)
            {
                if (MessageBox.Show(@"The song information has been changed." + Environment.NewLine + @"Do you want to update the 'Arrangement Identification'?  " + Environment.NewLine + @"Answering 'Yes' will reduce the risk of CDLC" + Environment.NewLine + @"in game hanging and song stats will be reset.", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
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

            if (ConfigRepository.Instance().GetBoolean("ddc_autogen"))
            {
                // add TKI_DDC comment
                var ddcComment = packageData.ToolkitInfo.PackageComment;
                if (String.IsNullOrEmpty(ddcComment))
                    ddcComment = TKI_DDC;
                else if (!ddcComment.Contains(TKI_DDC))
                    ddcComment = ddcComment + " " + TKI_DDC;

                packageData.ToolkitInfo.PackageComment = ddcComment;
            }

            // add TKI_REMASTER comment
            var remasterComment = packageData.ToolkitInfo.PackageComment;
            if (String.IsNullOrEmpty(remasterComment))
                remasterComment = TKI_REMASTER;
            else if (!remasterComment.Contains(TKI_REMASTER))
                remasterComment = remasterComment + " " + TKI_REMASTER;

            packageData.ToolkitInfo.PackageComment = remasterComment;

            // declare one time for DDC generation   
            var consoleOutput = String.Empty;
            SettingsDDC.Instance.LoadConfigXml();
            var phraseLen = SettingsDDC.Instance.PhraseLen;
            // removeSus may be depricated in latest DDC but left here for comptiblity
            var removeSus = SettingsDDC.Instance.RemoveSus;
            var rampPath = SettingsDDC.Instance.RampPath;
            var cfgPath = SettingsDDC.Instance.CfgPath;

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
                    // only validate lyrics that do not use a custom font
                    if (!arr.CustomFont)
                    {
                        var oldXml = GeneralExtensions.CopyToTempFile(arr.SongXml.File);
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

                // restore arrangement comments
                Song2014.WriteXmlComments(arr.SongXml.File, arr.XmlComments);

                // add DDC to arrangement
                if (ConfigRepository.Instance().GetBoolean("ddc_autogen"))
                {
                    // Important ... don't overwrite author's DD if it is already present in any arragement
                    if (!isDD)
                    {
                        using (var ddc = new DDC.DDC())
                        {
                            // apply DD to xml arrangments
                            var singleResult = ddc.ApplyDD(arr.SongXml.File, phraseLen, removeSus, rampPath, cfgPath, out consoleOutput, true);
                            if (singleResult == 1)
                            {
                                var errMsg = "DDC generated an error while processing arrangement:" + Environment.NewLine + arr.SongXml.File + Environment.NewLine;
                                BetterDialog2.ShowDialog(errMsg, "DDC Generated Error", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                            }

                            if (singleResult == 2)
                            {
                                consoleOutput = String.Format("Arrangement file '{0}' => {1}", Path.GetFileNameWithoutExtension(arr.SongXml.File), consoleOutput);
                                BetterDialog2.ShowDialog(consoleOutput, "DDC Generation Info", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                            }
                        }
                    }
                    else
                    {
                        // commented out ... don't nag user with this message
                        // MessageBox.Show("Existing DD content in arrangement: " + arr.Name + " was not changed", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Debug.WriteLine("Existing DD content in arrangement: " + arr.Name + " was not changed");
                    }
                }

                // put arrangement comments in correct order
                Song2014.WriteXmlComments(arr.SongXml.File);
            }

            if (Path.GetFileName(dlcDestPath).Contains(" ") && chkPlatformPS3.Checked)
            {
                if (!ConfigRepository.Instance().GetBoolean("creator_ps3pkgnamewarn"))
                    MessageBox.Show(String.Format("PS3 package name can't support space character due to encryption limitation. {0} Spaces will be automatic removed for your PS3 package name.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    ConfigRepository.Instance()["creator_ps3pkgnamewarn"] = true.ToString();
            }

            if (!bwGenerate.IsBusy && packageData != null)
            {
                pbUpdateProgress.Style = ProgressBarStyle.Marquee;
                pbUpdateProgress.Visible = true;
                lblCurrentOperation.Visible = true;
                btnPackageGenerate.Enabled = false;
                bwGenerate.RunWorkerAsync(packageData);
            }
        }

        public void btnPackageImport_Click(object sender = null, EventArgs e = null)
        {
            string srcPackage;
            string savePath;
            string tmpPath = Path.GetTempPath();

            // GET PATH
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a CDLC package (archive) to import";
                ofd.Filter = CurrentOFDPackageFilter;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                srcPackage = ofd.FileName;
            }

            if (CurrentGameVersion == GameVersion.RS2014)
                if (!srcPackage.IsValidPSARC())
                {
                    MessageBox.Show(String.Format("Invalid File Exception:  File '{0}' can not be used by current process.", Path.GetFileName(srcPackage)), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.Description = "Select folder to save project artifacts";
                fbd.UseDescriptionForTitle = true;
                fbd.SelectedPath = Path.GetDirectoryName(srcPackage) + Path.DirectorySeparatorChar;
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                savePath = fbd.SelectedPath;
            }

            GlobalExtension.UpdateProgress = pbUpdateProgress;
            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Continuous;
            GlobalExtension.CurrentOperationLabel = lblCurrentOperation;
            Thread.Sleep(100); // give Globals a chance to initialize
            Application.DoEvents();

            // UNPACK
            var packagePlatform = srcPackage.GetPlatform();
            UnpackedDir = Packer.Unpack(srcPackage, tmpPath, true, predefinedPlatform: packagePlatform);
            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(srcPackage));

            // Same name xbox issue fix
            if (packagePlatform.platform == GamePlatform.XBox360)
                savePath = String.Format("{0}_{1}", savePath, GamePlatform.XBox360.ToString());

            DirectoryExtension.Move(UnpackedDir, savePath, true);
            UnpackedDir = savePath;

            // REORGANIZE
            GlobalExtension.ShowProgress("Reorganizing Package Data ...", 35);
            var structured = ConfigRepository.Instance().GetBoolean("creator_structured");
            if (structured && CurrentGameVersion == GameVersion.RS2014)
                UnpackedDir = DLCPackageData.DoLikeProject(savePath);

            // LOAD DATA
            GlobalExtension.ShowProgress("Loading Package Data ...", 70);
            DLCPackageData info = null; // DLCPackageData specific to RS2
            if (CurrentGameVersion == GameVersion.RS2014)
                info = DLCPackageData.LoadFromFolder(UnpackedDir, packagePlatform, packagePlatform, fixMultiTone, fixLowBass);
            else
                info = DLCPackageData.RS1LoadFromFolder(UnpackedDir, packagePlatform, rbConvert.Checked);


            switch (packagePlatform.platform)
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
            MessageBox.Show(CurrentRocksmithTitle + " CDLC package was imported.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);

            // prevents possible cross threading
            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Marquee;
            GlobalExtension.Dispose();

            // for whatever freak'n reason users like to have template autosave happen here
            // even though this is not the right freak'n place to do so cause data can change
            // AUTO SAVE CDLC TEMPLATE ... so be it!
            if (!rbConvert.Checked)
                IsDirty = true;

            Parent.Focus();
        }

        public void btnTemplateLoad_Click(object sender = null, EventArgs e = null)
        {
            //TODO: issue with gameversion
            string dlcTemplatePath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Globals.DefaultProjectDir;
                ofd.SupportMultiDottedExtensions = true;
                ofd.Filter = CurrentRocksmithTitle + " CDLC Template (*.dlc.xml)|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                dlcTemplatePath = Globals.DefaultProjectDir = ofd.FileName;
            }

            UnpackedDir = Path.GetDirectoryName(dlcTemplatePath);
            LoadTemplateFile(dlcTemplatePath);
        }

        public void btnToneAdd_Click(object sender = null, EventArgs e = null)
        {
            var tone = CreateNewTone();
            using (var form = new ToneForm() { Text = "Add Tone" })
            {
                form.EditMode = false;
                form.CurrentGameVersion = CurrentGameVersion;
                form.toneControl.CurrentGameVersion = CurrentGameVersion;
                form.toneControl.Init();
                form.toneControl.Tone = GeneralExtensions.Copy(tone);
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
            IsDirty = true;
        }

        private void GameVersion_MouseUp(object sender, MouseEventArgs e)
        {
            // GameVersion_CheckedChanged usage comes with problems
            // everytime the value of checked is changed the event handler fires
            // this is not what we want ... use MouseUp instead to detect changes

            GameVersion oldGameVersion;
            var btn = sender as RadioButton;
            switch (btn.Text.ToLowerInvariant())
            {
                case "rocksmith 2014":
                    oldGameVersion = GameVersion.RS2014;
                    break;
                case "rocksmith":
                    oldGameVersion = GameVersion.RS2012;
                    break;
                default:
                    oldGameVersion = GameVersion.None;
                    break;
            }
            // MAC RS2014 only
            chkPlatformMAC.Enabled = CurrentGameVersion != GameVersion.RS2012;

            // Song preview volume RS2014 only
            numVolPreview.Enabled = CurrentGameVersion != GameVersion.RS2012;

            PopulateAppIdCombo();
            PopulateTonesLB(oldGameVersion);
            PopulateArrangements(oldGameVersion);
            PopulateAudioTB(oldGameVersion);
        }

        private void GeneratePackage(object sender, DoWorkEventArgs e)
        {
            var currentGameVersion = (CurrentGameVersion == GameVersion.RS2012) ? GameVersion.RS2012 : GameVersion.RS2014;
            var packageData = e.Argument as DLCPackageData;
            errorsFound = new StringBuilder();

            var numPlatforms = 0;
            if (chkPlatformPC.Checked)
                numPlatforms++;
            if (chkPlatformMAC.Checked)
                numPlatforms++;
            if (chkPlatformXBox360.Checked)
                numPlatforms++;
            if (chkPlatformPS3.Checked)
                numPlatforms++;

            var step = (int)Math.Round(1.0 / numPlatforms * 100, 0);
            int progress = 0;

            if (chkPlatformPC.Checked)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating PC Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcDestPath, packageData, new Platform(GamePlatform.Pc, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate PC package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
                }

            if (chkPlatformMAC.Checked)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating Mac Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcDestPath, packageData, new Platform(GamePlatform.Mac, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate Mac package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
                }

            if (chkPlatformXBox360.Checked)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating XBox 360 Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcDestPath, packageData, new Platform(GamePlatform.XBox360, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate XBox 360 package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
                }

            if (chkPlatformPS3.Checked)
                try
                {
                    bwGenerate.ReportProgress(progress, "Generating PS3 Package ...");
                    RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcDestPath, packageData, new Platform(GamePlatform.PS3, currentGameVersion), pnum: numPlatforms);
                    progress += step;
                    numPlatforms--;
                    bwGenerate.ReportProgress(progress);
                }
                catch (Exception ex)
                {
                    errorsFound.AppendLine(String.Format("Error generate PS3 package: {0}{1}. {0}PS3 package require 'JAVA x86' (32 bits) installed on your machine to generate properly.{0}", Environment.NewLine, ex.StackTrace));
                }

            // Cache cleanup so we don't serialize or reuse data that could be changed
            packageData.CleanCache();
            e.Result = (numPlatforms == 1 && errorsFound.Length > 0) ? "error" : "generate";

        }

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
                        message = String.Format("Package was generated with errors! See below: {0}{1}", Environment.NewLine, errorsFound);
                    else if (ConfigRepository.Instance().GetBoolean("creator_autosavetemplate"))
                        SaveTemplateFile(UnpackedDir);

                    message += String.Format("{0}Would you like to open the folder where the package was generated?{0}", Environment.NewLine);
                    if (MessageBox.Show(message, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start(Path.GetDirectoryName(dlcDestPath));
                    }
                    break;

                case "error":
                    var message2 = String.Format("Package generation failed. See below: {0}{1}{0}", Environment.NewLine, errorsFound);
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
                tt.SetToolTip(numVolSong, "Higher 0,-1,-2,-3,..., Average -12 ,...,-16,-17 Lower");
            else
                tt.SetToolTip(numVolPreview, "Higher 0,-1,-2,-3,..., Average -12 ,...,-16,-17 Lower");
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
                ofd.Filter = "Album Art File (*.bmp,*.dds,*.gif,*.jpg,*.jpeg,*.png)|*.bmp;*.dds;*.gif;*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.FileName.IsValidImage())
                        AlbumArtPath = ofd.FileName;
                    else
                        MessageBox.Show("The selected image is not valid or not supported." + Environment.NewLine + "MimeType doesn't match with file extension!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Globals.DefaultProjectDir;
                ofd.Title = "Quick Add ... Multiselect Arrangements";
                ofd.Filter = "Rocksmith EOF XML Files (*.xml)|*.xml";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string[] xmlFilePaths = ofd.FileNames;
                Globals.DefaultProjectDir = xmlFilePaths[0];
                AddArrangementsQuick(xmlFilePaths);
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
        /// used for debugging ... There is tone name error in XML Arrangement: 
        /// xxxx_xxxx is not properly defined.
        /// Use EOF to re-author custom tones or Notepad to attempt manual repair.
        /// </summary>
        private void btnDevUse_Click(object sender, EventArgs e)
        {
            // load artifacts
            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.Description = "Select artifacts folder (a previously unpacked CDLC)";
                fbd.UseDescriptionForTitle = true;
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                UnpackedDir = fbd.SelectedPath;
            }

            // hook up the progress bar
            GlobalExtension.UpdateProgress = pbUpdateProgress;
            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Continuous;
            GlobalExtension.CurrentOperationLabel = lblCurrentOperation;
            Thread.Sleep(100); // give Globals a chance to initialize
            Application.DoEvents();

            Platform packagePlatform;
            try
            {
                packagePlatform = UnpackedDir.GetPlatform();
            }
            catch // set default packagePlatform if folder is not readable
            {
                packagePlatform = new Platform(GamePlatform.Pc, CurrentGameVersion);
            }

            // Same name xbox issue fix
            if (packagePlatform.platform == GamePlatform.XBox360)
                UnpackedDir = String.Format("{0}_{1}", UnpackedDir, GamePlatform.XBox360.ToString());

            // REORGANIZE
            GlobalExtension.ShowProgress("Reorganizing Package Data ...", 35);
            var structured = ConfigRepository.Instance().GetBoolean("creator_structured");
            if (structured && CurrentGameVersion == GameVersion.RS2014)
                UnpackedDir = DLCPackageData.DoLikeProject(UnpackedDir);


            // LOAD DATA
            GlobalExtension.ShowProgress("Loading Package Data ...", 70);
            DLCPackageData info = null; // DLCPackageData specific to RS2
            if (CurrentGameVersion == GameVersion.RS2014)
                info = DLCPackageData.LoadFromFolder(UnpackedDir, packagePlatform, packagePlatform, fixMultiTone, fixLowBass);
            else
                info = DLCPackageData.RS1LoadFromFolder(UnpackedDir, packagePlatform, rbConvert.Checked);


            switch (packagePlatform.platform)
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

            Parent.Focus();

            // prevents possible cross threading
            GlobalExtension.UpdateProgress.Style = ProgressBarStyle.Marquee;
            GlobalExtension.Dispose();
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
                    form.toneControl.Tone = GeneralExtensions.Copy(tone);
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
                var tif = toneImportFile;
                // game may convert profiles to uppercase
                if (tif.ToLower().Contains("prfldb"))
                    tif = tif.ToLower();

                ImportTone(tif);
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
            if (chkPlatformPC.Checked || chkPlatformMAC.Checked)
            {
                txtAppId.Enabled = true;
                cmbAppIds.Enabled = true;
            }
            else if (!chkPlatformPC.Checked && !chkPlatformMAC.Checked)
            {
                txtAppId.Enabled = false;
                cmbAppIds.Enabled = false;
            }
        }

        private void cmbAppIds_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbAppIds.SelectedItem != null)
            {
                txtAppId.Text = ((SongAppId)cmbAppIds.SelectedItem).AppId;
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
            if (numVolPreview.Value == decimal.Parse(numVolSong.Text)) //let's confuve user a bit more here :D
                numVolPreview.Value = numVolSong.Value;
        }

        private void rbConvert_MouseEnter(object sender, EventArgs e)
        {
            tt.IsBalloon = true;
            tt.InitialDelay = 0;
            tt.ShowAlways = true;
            tt.SetToolTip(rbConvert, "Convert RS1 Arrangements" +
                                     Environment.NewLine + "to RS2014 Arrangements");
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

        private class EqSEvent : IEqualityComparer<RocksmithToolkitLib.Xml.SongEvent>
        {
            public bool Equals(RocksmithToolkitLib.Xml.SongEvent x, RocksmithToolkitLib.Xml.SongEvent y)
            {
                if (x == null)
                    return y == null;

                return x.Code == y.Code && x.Time.Equals(y.Time);
            }

            public int GetHashCode(RocksmithToolkitLib.Xml.SongEvent obj)
            {
                if (ReferenceEquals(obj, null))
                    return 0;
                return obj.Code.GetHashCode() | obj.Time.GetHashCode();
            }
        }



    }
}