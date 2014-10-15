﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.ToolkitTone;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using X360.STFS;
using Ookii.Dialogs;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class DLCPackageCreator : UserControl
    {
        public static readonly string MESSAGEBOX_CAPTION =  "DLC Package Creator";
        private BackgroundWorker bwGenerate = new BackgroundWorker();
        private StringBuilder errorsFound;
        private string dlcSavePath;
        
        #region Properties

        public GameVersion CurrentGameVersion {
            get {
                if (RS2014.Checked)
                    return GameVersion.RS2014;
                else
                    return GameVersion.RS2012; //Default
            }
            set {
                switch (value) {
                    case GameVersion.RS2014:
                        RS2014.Checked = true;
                        break;
                    default:
                        RS2012.Checked = true;
                        break;
                }
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
                var filter = CurrentOFDPackageFilter + "|";
                filter += CurrentRocksmithTitle + " Song Manifest (*.json)|*.json";

                return filter;
            }
        }

        public string CurrentOFDPackageFilter
        {
            get
            {
                var filter = "All Files (*.*)|*.*";
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2014:
                        filter  = CurrentRocksmithTitle + " PC/Mac Package (*.psarc)|*.psarc|";
                        filter += CurrentRocksmithTitle + " XBox360 Package (*.*)|*.*|";
                        filter += CurrentRocksmithTitle + " PS3 Package (*.edat)|*.edat";
                        break;
                    default:
                        filter  = CurrentRocksmithTitle + " PC/Mac Package (*.dat)|*.dat|";
                        filter += CurrentRocksmithTitle + " XBox360 Package (*.*)|*.*";
                        break;
                }
                return filter;
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
            set { DlcNameTB.Text = value.GetValidSongName(SongTitle); }
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
            set { AverageTempoTB.Text = value; }
        }

        public string PackageVersion
        {
            get { return packageVersionTB.Text; }
            set { packageVersionTB.Text = value; }
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

        private string AudioPath
        {
            get { return audioPathTB.Text; }
            set { audioPathTB.Text = value; }
        }

        #endregion

        public DLCPackageCreator()
        {
            InitializeComponent();
            try
            {
                PopulateAppIdCombo();
                PopulateTonesLB();
                SetDefaultFromConfig();
            }
            catch { /*For mono compatibility*/ }

            // Generate package worker
            bwGenerate.DoWork += new DoWorkEventHandler(GeneratePackage);
            bwGenerate.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            bwGenerate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            bwGenerate.WorkerReportsProgress = true;
        }

        private void SetDefaultFromConfig() {
            CurrentGameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), ConfigRepository.Instance()["general_defaultgameversion"]);
        }

        private void PopulateTonesLB()
        {
            TonesLB.Items.Clear();
            TonesLB.Items.Add(CreateNewTone());
        }

        public dynamic CreateNewTone(string toneName = "Default")
        {
            var name = GetUniqueToneName(toneName);

            if (CurrentGameVersion == GameVersion.RS2014)
                return new Tone2014() { Name = name, Key = name };
            else
                return new Tone() { Name = name, Key = name };
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
                        isUnique = !TonesLB.Items.OfType<Tone>().Any(n => n.Name == uniqueName);
                        break;
                    case GameVersion.RS2014:
                        isUnique = !TonesLB.Items.OfType<Tone2014>().Any(n => n.Name == uniqueName);
                        break;
                }

                if (!isUnique)
                {
                    uniqueName = toneName + (++ind);
                }
            } while (!isUnique);

            return uniqueName;
        }

        public void arrangementAddButton_Click(object sender = null, EventArgs e = null)
        {
            Arrangement arrangement;
            using (var form = new ArrangementForm((DLCPackageCreator)this, CurrentGameVersion))
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
            if (MessageBox.Show("Are you sure to delete the selected arrangement?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (ArrangementLB.SelectedItem != null)
                ArrangementLB.Items.Remove(ArrangementLB.SelectedItem);
        }
        //TODO: allow to choose audio for each arrangement separately. #Lessons, #Multitracks
        private void openAudioButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = CurrentOFDAudioFileFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    AudioPath = ofd.FileName;
            }
        }

        public void dlcGenerateButton_Click(object sender = null, EventArgs e = null)
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
                    OggFile.VerifyHeaders(AudioPath);
                }
                catch (InvalidDataException ex)
                {
                    MessageBox.Show(ex.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            using (var ofd = new SaveFileDialog())
            {
                ofd.FileName = GeneralExtensions.GetShortName("{0}_{1}_v{2}", ArtistSort, SongTitleSort, PackageVersion.Replace(".","_"), ConfigRepository.Instance().GetBoolean("creator_useacronyms"));
                ofd.Filter = CurrentRocksmithTitle + " DLC (*.*)|*.*";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }

            if (Path.GetFileName(dlcSavePath).Contains(" ") && platformPS3.Checked)
                if (!ConfigRepository.Instance().GetBoolean("creator_ps3pkgnamewarn")) {
                    MessageBox.Show(String.Format("PS3 package name can't support space character due to encryption limitation. {0} Spaces will be automatic removed for your PS3 package name.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } else {
                    ConfigRepository.Instance()["creator_ps3pkgnamewarn"] = true.ToString();
                }

            if (!bwGenerate.IsBusy && packageData != null) {
                updateProgress.Visible = true;
                currentOperationLabel.Visible = true;
                dlcGenerateButton.Enabled = false;
                bwGenerate.RunWorkerAsync(packageData);
            }
        }

        private void GeneratePackage(object sender, DoWorkEventArgs e) {
            var packageData = e.Argument as DLCPackageData;
            errorsFound = new StringBuilder();
            
            var numPlatforms = 0;
            if (platformPC.Checked)
                numPlatforms++;
            if (platformMAC.Checked)
                numPlatforms++;
            if (platformXBox360.Checked)
                numPlatforms++;
            if (platformPS3.Checked)
                numPlatforms++;

            var step = (int)Math.Round(1.0 / numPlatforms * 100, 0);
            int progress = 0;

            if (platformPC.Checked)
                try {
                bwGenerate.ReportProgress (progress, "Generating PC package");
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate (dlcSavePath, packageData, new Platform (GamePlatform.Pc, CurrentGameVersion));
                progress += step;
                bwGenerate.ReportProgress (progress);
            } catch (Exception ex) {
                errorsFound.AppendLine (String.Format ("Error generate PC package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
            }

            if (platformMAC.Checked)
                try {
                bwGenerate.ReportProgress (progress, "Generating Mac package");                
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate (dlcSavePath, packageData, new Platform (GamePlatform.Mac, CurrentGameVersion));
                progress += step;
                bwGenerate.ReportProgress (progress);
            } catch (Exception ex) {
                errorsFound.AppendLine (String.Format ("Error generate Mac package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
            }

            if (platformXBox360.Checked)
                try {
                bwGenerate.ReportProgress (progress, "Generating XBox 360 package");
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate (dlcSavePath, packageData, new Platform (GamePlatform.XBox360, CurrentGameVersion));
                progress += step;
                bwGenerate.ReportProgress (progress);
            } catch (Exception ex) {
                errorsFound.AppendLine (String.Format ("Error generate XBox 360 package: {0}{1}{0}{2}{0}", Environment.NewLine, ex.Message, ex.StackTrace));
            }

            if (platformPS3.Checked)
                try {
                bwGenerate.ReportProgress (progress, "Generating PS3 package");
                RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate (dlcSavePath, packageData, new Platform (GamePlatform.PS3, CurrentGameVersion));
                progress += step;
                bwGenerate.ReportProgress (progress);
            } catch (Exception ex) {
                errorsFound.AppendLine (String.Format ("Error generate PS3 package: {0}{1}. {0}PS3 package require 'JAVA x86' (32 bits) installed on your machine to generate properly.{0}", Environment.NewLine, ex.StackTrace));
            }

            // Cache cleanup so we don't serialize or reuse data that could be changed
            packageData.CleanCache ();
            e.Result = (numPlatforms == 1 && errorsFound.Length > 0) ? "error" : "generate";
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
                ofd.Filter = "Album Art File (*.bmp,*.dds,*.gif,*.jpg,*.jpeg,*.png)|*.bmp;*.dds;*.gif;*.jpg;*.jpeg;*.png";
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

        private void dlcSaveButton_Click(object sender, EventArgs e)
        {
            SaveTemplateFile();
        }

        public void SaveTemplateFile(string defaultSavePath = null)
        {
            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            var packageData = GetPackageData();
            if (packageData == null) {
                MessageBox.Show("One or more fields are missing information.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileName = GeneralExtensions.GetShortName("{0}_{1}_{2}", ArtistSort, SongTitleSort, CurrentGameVersion.ToString(), ConfigRepository.Instance().GetBoolean("creator_useacronyms"));
            string dlcSavePath;
            
            if (!String.IsNullOrEmpty(defaultSavePath))
            {
                dlcSavePath = Path.Combine(defaultSavePath, fileName + ".dlc.xml");
            }
            else
            {
                using (var ofd = new SaveFileDialog())
                {
                    ofd.SupportMultiDottedExtensions = true;
                    ofd.Filter = CurrentRocksmithTitle + " DLC Template (*.dlc.xml)|*.dlc.xml";
                    ofd.FileName = fileName;
                    if (DialogResult.OK != ofd.ShowDialog()) return;
                    dlcSavePath = ofd.FileName;
                }
            }

            var BasePath = Path.GetDirectoryName(dlcSavePath);

            //Make the paths relative
            if (!string.IsNullOrEmpty(packageData.AlbumArtPath))
                packageData.AlbumArtPath = BasePath.RelativeTo(packageData.AlbumArtPath);
            
            string audioPath = packageData.OggPath;
            string audioPreviewPath = packageData.OggPreviewPath;
            if (!String.IsNullOrEmpty(audioPath))
                packageData.OggPath = BasePath.RelativeTo(audioPath);
            if (!String.IsNullOrEmpty(audioPreviewPath))
                packageData.OggPreviewPath = BasePath.RelativeTo(audioPreviewPath);

            foreach (var arr in packageData.Arrangements)
            {
            	if (!String.IsNullOrEmpty(arr.SongXml.File))
                    arr.SongXml.File = BasePath.RelativeTo(arr.SongXml.File);
            	if (!String.IsNullOrEmpty(arr.SongFile.File))
                    arr.SongFile.File = "";
            }
            var serializer = new DataContractSerializer(typeof(DLCPackageData));
            using (var stm = XmlWriter.Create(dlcSavePath, new XmlWriterSettings() { CheckCharacters = true, Indent = true }))
            {
                serializer.WriteObject(stm, packageData);
            }

            //Re-absolutize the paths
            foreach (var arr in packageData.Arrangements)
            {
                if (!String.IsNullOrEmpty(arr.SongXml.File))
                	arr.SongXml.File = BasePath.AbsoluteTo(arr.SongXml.File);
                if(!String.IsNullOrEmpty(arr.SongFile.File))
                    arr.SongFile.File = BasePath.AbsoluteTo(arr.SongFile.File);
            }

            if (String.IsNullOrEmpty(defaultSavePath))
                MessageBox.Show(CurrentRocksmithTitle + " DLC Package template was saved.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void dlcLoadButton_Click(object sender = null, EventArgs e = null)
        {
            string dlcLoadPath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.SupportMultiDottedExtensions = true;
                ofd.Filter = CurrentRocksmithTitle + " DLC Template (*.dlc.xml)|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcLoadPath = ofd.FileName;
            }

            loadTemplate(dlcLoadPath);
        }

        public void loadTemplate(string dlcLoadPath)
        {
            DLCPackageData info = null;
            try
            {
                var deserializer = new DataContractSerializer(typeof(DLCPackageData));
                using (var stm = new XmlTextReader(dlcLoadPath))
                {
                    info = (DLCPackageData)deserializer.ReadObject(stm);
                }
                if (info == null) throw new InvalidDataException("DLC Template Is Null");
            }
            catch (Exception se)
            {
                MessageBox.Show("Can't load saved DLC because is not compatible with new DLC template format. \n" + se.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FillPackageCreatorForm(info, dlcLoadPath);

            MessageBox.Show(CurrentRocksmithTitle + " DLC Template was loaded.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Parent.Focus();
        }

        public void dlcImportButton_Click(object sender = null, EventArgs e = null) {
            string sourcePackage;
            string savePath;
            string tmp = Path.GetTempPath();

            // GET PATH
            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select one DLC to import";
                ofd.Filter = CurrentOFDPackageFilter;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourcePackage = ofd.FileName;
            }

            if (!sourcePackage.IsValidPSARC()){
                MessageBox.Show(String.Format("File '{0}' isn't valid. File extension was changed to '.invalid'", Path.GetFileName(sourcePackage)), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var fbd = new VistaFolderBrowserDialog()) {
                fbd.Description = "Select folder to save project artifacts";
                fbd.UseDescriptionForTitle = true;
                fbd.SelectedPath = Path.GetDirectoryName (sourcePackage);
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                savePath = fbd.SelectedPath;
            }

            // UNPACK
            var packagePlatform = sourcePackage.GetPlatform();
            var unpackedDir = Packer.Unpack(sourcePackage, tmp, true, true, false);
            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(sourcePackage));
            DirectoryExtension.Move(unpackedDir, savePath, true);
            unpackedDir = savePath;

            // REORGANIZE
            var structured = ConfigRepository.Instance().GetBoolean("creator_structured");
            if (structured)
                unpackedDir = DLCPackageData.DoLikeProject(savePath);

            
            // LOAD DATA
            var info = DLCPackageData.LoadFromFolder(unpackedDir, packagePlatform);

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
            FillPackageCreatorForm(info, unpackedDir);

            // AUTO SAVE DLC TEMPLATE
            SaveTemplateFile(unpackedDir);

            MessageBox.Show(CurrentRocksmithTitle + " DLC Template was imported.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Parent.Focus();
        }



        /**
            * Fixes B Standard and below tuning issues for bass. This is done by adjusting the pitch for the arrangement to 220hz,
            * while raising the tuning 12 steps to offset the ptich change.
            * This function is meant for fast and easy, one "click" fixes to PSARC's/packed CDLC.
            * The CDLC will be unpacked, edited, and then repacked with '_bassfixed' appended to the file name.
         * Parameters: 
         * quick - chooses directory of source file, uses source file name with "_bassfix" appended.
         * deleteSourceFile - deletes source file after extracting needed information and generating the file.
         **/   
        public void dlcLowTuningBassFix(object sender, EventArgs e, Button lowTuningBassFixButton,  Boolean quick = false, Boolean deleteSourceFile = false)
        {
            string sourcePackage;
            string saveWorkingDirectoryPath;

            lowTuningBassFixButton.Enabled = false;

            // GET PATH
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select one DLC to import";
                ofd.Filter = CurrentOFDPackageFilter;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    lowTuningBassFixButton.Enabled = true;
                    return;
                }
                sourcePackage = ofd.FileName;
            }


            if (!sourcePackage.IsValidPSARC())
            {
                MessageBox.Show(String.Format("File '{0}' isn't valid. File extension was changed to '.invalid'", Path.GetFileName(sourcePackage)), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (!quick)
            {
                using (var fbd = new VistaFolderBrowserDialog())
                {
                    fbd.Description = "Select folder to save project artifacts";
                    fbd.UseDescriptionForTitle = true;

                    if (fbd.ShowDialog() != DialogResult.OK)
                    {
                        lowTuningBassFixButton.Enabled = true;
                        return;
                    }
                    saveWorkingDirectoryPath = fbd.SelectedPath;
                }
            }
            else
            {
                saveWorkingDirectoryPath = new FileInfo(sourcePackage).Directory.FullName;
            }

            // UNPACK
            var unpackedDir = Packer.Unpack(sourcePackage, saveWorkingDirectoryPath, true, true, false);
            var packagePlatform = sourcePackage.GetPlatform();

            // REORGANIZE
            var structured = ConfigRepository.Instance().GetBoolean("creator_structured");
            if (structured && !quick)
                unpackedDir = DLCPackageData.DoLikeProject(unpackedDir);

            // LOAD DATA
            var info = DLCPackageData.LoadFromFolder(unpackedDir, packagePlatform);
            info.PackageVersion = "1";
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

            //apply bass fix.
            for (int i = 0; i < info.Arrangements.Count; i++)
            {
                Arrangement arr = info.Arrangements[i];
                if (arr.ArrangementType == ArrangementType.Bass)
                {
         
                    if (arr.TuningPitch == 220.0)
                    {
                        MessageBox.Show("This song is already at 220hz pitch (bass fixed applied already?)",
                            MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        arr.TuningPitch = 220.0;
                    }

                    Song2014 songXml = null;
                    using (var reader = new StreamReader(arr.SongXml.File))
                    {
                        songXml = (Song2014)new RocksmithToolkitLib.Extensions.XmlStreamingDeserializer<Song2014>(reader).Deserialize();
                    }
                    songXml.CentOffset = "-1200.0"; // Force 220Hz

                    // Octave up for each string
                    short[] strings = arr.TuningStrings.ToShortArray();
                    for (int s = 0; s < strings.Length; s++)
                    {
                        strings[s] += 12;
                    }
                    //Detect tuning
                    TuningDefinition tuning = TuningDefinitionRepository.Instance().SelectAny(new TuningStrings(strings), CurrentGameVersion);
                    if (tuning == null)
                    {
                        tuning = new TuningDefinition();
                        tuning.Tuning = new TuningStrings(strings);
                        tuning.UIName = tuning.Name = tuning.NameFromStrings(tuning.Tuning, true, false) + "BassFix";
                        tuning.Custom = true;
                        tuning.GameVersion = GameVersion.RS2014;
                        TuningDefinitionRepository.Instance().Add(tuning, true);
                    }
                    arr.TuningStrings = tuning.Tuning;
                    arr.Tuning = tuning.Name;
                    songXml.Tuning = tuning.Tuning;

                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    XmlSerializer serializer = new XmlSerializer(typeof(Song2014));
                    TextWriter textWriter = new StreamWriter(arr.SongXml.File);
                    serializer.Serialize(textWriter, songXml, ns);
                    textWriter.Close();

                }
            }
            
            if (!quick)
            {
                using (var ofd = new SaveFileDialog())
                {
                    ofd.FileName = GeneralExtensions.GetShortName("{0}_{1}_v{2}", ArtistSort, SongTitleSort, PackageVersion.Replace(".","_"), ConfigRepository.Instance().GetBoolean("creator_useacronyms"));
                    ofd.Filter = CurrentRocksmithTitle + " DLC (*.*)|*.*";
                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        lowTuningBassFixButton.Enabled = true;
                        return;
                    } 
                    dlcSavePath = ofd.FileName;
                }
            }
            else
            {
                
                dlcSavePath = Path.Combine(saveWorkingDirectoryPath, Path.GetFileNameWithoutExtension(sourcePackage) + "_bassfix");
            }

            if (Path.GetFileName(dlcSavePath).Contains(" ") && platformPS3.Checked)
                if (!ConfigRepository.Instance().GetBoolean("creator_ps3pkgnamewarn"))
                {
                    MessageBox.Show(String.Format("PS3 package name can't support space character due to encryption limitation. {0} Spaces will be automatic removed for your PS3 package name.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ConfigRepository.Instance()["creator_ps3pkgnamewarn"] = true.ToString();
                }

            if(deleteSourceFile)
            {
                try
                {
                    File.Delete(sourcePackage);
                }
                catch (Exception ex) 
                {
                    Console.Write(ex.Message);
                    MessageBox.Show("Denied rights needed to delete source package, or an error occured. Package still may exist. Try running as Administrator.");
                }
            }
           

        

            if (!bwGenerate.IsBusy && info != null)
            {
                
                //Generate CDLC
                bwGenerate.RunWorkerAsync(info);
            }
        }
    
        private void FillPackageCreatorForm(DLCPackageData info, string filesBaseDir) {
            RS2012.Checked = info.GameVersion == GameVersion.RS2012;
            RS2014.Checked = info.GameVersion == GameVersion.RS2014;

            platformPC.Checked = info.Pc;
            platformMAC.Checked = info.Mac;
            platformXBox360.Checked = info.XBox360;
            platformPS3.Checked = info.PS3;

            PackageVersion = info.PackageVersion;

            TonesLB.Items.Clear();
            switch (CurrentGameVersion) {
                case GameVersion.RS2012:
                    if (info.Tones == null)
                        info.Tones = new List<Tone>();
                    if (info.Tones.Count == 0)
                        info.Tones.Add(CreateNewTone());

                    foreach (var tone in info.Tones) {
                        if (String.IsNullOrEmpty(tone.Key))
                            tone.Key = tone.Name.GetValidName();

                        TonesLB.Items.Add(tone);
                    }
                    break;
                case GameVersion.RS2014:
                    if (info.TonesRS2014 == null)
                        info.TonesRS2014 = new List<Tone2014>();
                    if (info.TonesRS2014.Count == 0)
                        info.TonesRS2014.Add(CreateNewTone());

                    foreach (var toneRS2014 in info.TonesRS2014) {
                        if (String.IsNullOrEmpty(toneRS2014.Key))
                            toneRS2014.Key = toneRS2014.Name.GetValidName();

                        TonesLB.Items.Add(toneRS2014);
                    }
                    break;
            }

            var BasePath = Path.GetDirectoryName(filesBaseDir);

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
            AlbumArtPath = BasePath.AbsoluteTo(info.AlbumArtPath);

            // Audio file
            if (!String.IsNullOrEmpty(info.OggPath))
                AudioPath = BasePath.AbsoluteTo(info.OggPath);
            platformPC.Checked = !String.IsNullOrEmpty(info.OggPath);

            songVolumeBox.Value = Decimal.Round((decimal)info.Volume, 2);
            previewVolumeBox.Value = (info.PreviewVolume != null) ? Decimal.Round((decimal)info.PreviewVolume, 2) : songVolumeBox.Value;

            //if (platformXBox360.Checked)
            //    rbuttonSignatureLIVE.Checked = info.SignatureType == PackageMagic.LIVE;

            ArrangementLB.Items.Clear();
            foreach (var arrangement in info.Arrangements) {
                arrangement.SongXml.File = BasePath.AbsoluteTo(arrangement.SongXml.File);
                arrangement.CleanCache();
                if (arrangement.ToneBase == null)
                {
                    switch (CurrentGameVersion) {
                        case GameVersion.RS2012:
                            arrangement.ToneBase = info.Tones[0].Name;
                            break;
                        case GameVersion.RS2014:
                            arrangement.ToneBase = info.TonesRS2014[0].Name;
                            break;
                    }
                }
                if (arrangement.ArrangementType != ArrangementType.Vocal)
                {
                    try
                    {
                        var songXml = Song2014.LoadFromFile(arrangement.SongXml.File);
                        arrangement.CapoFret = songXml.Capo;
                        //Load tuning from Arrangement
                        var tuning = TuningDefinitionRepository.Instance().SelectAny(arrangement.TuningStrings, CurrentGameVersion);
                        if (tuning == null)
                        {
                            tuning = new TuningDefinition();
                            tuning.Tuning = arrangement.TuningStrings;
                            tuning.Custom = true;
                            tuning.GameVersion = CurrentGameVersion;
                            tuning.Name = tuning.UIName = arrangement.Tuning;
                            if (String.IsNullOrEmpty(tuning.Name))
                            {
                                tuning.Name = tuning.UIName = tuning.NameFromStrings(arrangement.TuningStrings, arrangement.ArrangementType == ArrangementType.Bass);
                            }

                            TuningDefinitionRepository.Instance().Add(tuning, true);
                        }
                        // Populate Arrangement tuning info
                        arrangement.Tuning = tuning.UIName;
                        arrangement.TuningStrings = tuning.Tuning;
                        //Cleanup
                        tuning = null;
                        songXml = null;
                    }
                    catch { /* Handle old types of *.dlc.xml */ }
                }
                ArrangementLB.Items.Add(arrangement);
            }
        }

        private void songVolumeBox_ValueChanged(object sender, EventArgs e)
        {
            previewVolumeBox.Value = songVolumeBox.Value;
            ShowVolumeTip();
        }

        private void previewVolumeBox_ValueChanged(object sender, EventArgs e)
        {
            ShowVolumeTip();
        }

        private void ShowVolumeTip() {
            ToolTip tt = new ToolTip();
            tt.IsBalloon = true;
            tt.InitialDelay = 0;
            tt.ShowAlways = true;
            tt.SetToolTip(songVolumeBox, "HIGHER 0,-1,-2,-3,..., AVERAGE -12 ,...,-16,-17 LOWER");
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
            if (String.IsNullOrEmpty(PackageVersion))
            {
                PackageVersion = "1";
            }
            if (!PackageVersion.Equals(PackageVersion.GetValidVersion()))
            {
                MessageBox.Show(String.Format("Package verion field contain invalid characters!\n" +
                                              "Please replace this: {0}\n" +
                                              "By something like this: 1 or 2.1 or 2.2.1",
                                              PackageVersion), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                packageVersionTB.Focus();
                return null;
            }

            //Album Art validation (alert only)
            if (String.IsNullOrEmpty(AlbumArtPath) || !File.Exists(AlbumArtPath))
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

            if (!File.Exists(AudioPath))
            {
                audioPathTB.Focus();
                return null;
            }

            string audioPreviewPath = null;
            if (CurrentGameVersion == GameVersion.RS2014)
            {
                audioPreviewPath = Path.Combine(Path.GetDirectoryName(AudioPath), String.Format(Path.GetFileNameWithoutExtension(AudioPath) + "_preview" + Path.GetExtension(AudioPath)));
                if (!File.Exists(audioPreviewPath))
                {
                    if (MessageBox.Show("Warning: Song Preview not found!" + Environment.NewLine +
                                        "File: " + audioPreviewPath + Environment.NewLine +
                                        "If you click 'Yes' the song file will be used for the song preview." + Environment.NewLine +
                                        "Else you click 'No' you could fix the problem before package generation.", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        audioPathTB.Focus();
                        return null;
                    }
                }
            }
            //TODO: jVocals support
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
                    MessageBox.Show("Error: Song Xml File doesn't exist: " +  arr.SongXml.File, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                arr.SongFile.File = "";
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

            var songVol = (float)songVolumeBox.Value;
            var previewVol = (!String.IsNullOrEmpty(audioPreviewPath)) ? (float)songVolumeBox.Value : songVol;

            var data = new DLCPackageData
            {
                GameVersion = CurrentGameVersion,
                Pc = platformPC.Checked,
                Mac = platformMAC.Checked,
                XBox360 = platformXBox360.Checked,
                PS3 = platformPS3.Checked,
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
                OggPath = AudioPath,
                OggPreviewPath = audioPreviewPath,
                Arrangements = arrangements,
                Tones = tones,
                TonesRS2014 = tonesRS2014,
                Volume = songVol,
                PreviewVolume = previewVol,
                SignatureType = PackageMagic.CON,
                PackageVersion = PackageVersion.GetValidVersion()
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
                using (var form = new ArrangementForm(arrangement, (DLCPackageCreator)this, CurrentGameVersion) { Text = "Edit Arrangement" })
                {
                    if (DialogResult.OK != form.ShowDialog())
                    {
                        return;
                    }
                }
                ArrangementLB.Items[ArrangementLB.SelectedIndex] = arrangement;
            }
        }

        public void toneAddButton_Click(object sender = null, EventArgs e = null)
        {
            var tone = CreateNewTone();
            using (var form = new ToneForm())
            {
                form.CurrentGameVersion = CurrentGameVersion;
                form.toneControl1.CurrentGameVersion = CurrentGameVersion;
                form.toneControl1.Init();
                form.toneControl1.Tone = GeneralExtensions.Copy(tone);
                form.ShowDialog();

                if (form.Saved)
                    TonesLB.Items.Add(form.toneControl1.Tone);
            }
        }

        private void toneRemoveButton_Click(object sender, EventArgs e)
        {
            RemoveTone();
        }

        private void RemoveTone()
        {
            if (MessageBox.Show("Are you sure to delete the selected tone?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (TonesLB.SelectedItem != null && TonesLB.Items.Count > 1)
            {
                dynamic tone = TonesLB.SelectedItem;
                TonesLB.Items.Remove(TonesLB.SelectedItem);

                dynamic firstTone = TonesLB.Items[0];
                foreach (var item in ArrangementLB.Items.OfType<Arrangement>())
                    if (tone.Name.Equals(item.ToneBase) && String.IsNullOrEmpty(item.ToneB))
                        item.ToneBase = firstTone.Name;
                ArrangementLB.Refresh();
            }
        }

        private void toneDuplicateButton_Click(object sender, EventArgs e)
        {
            DuplicateTone();
        }

        private void DuplicateTone()
        {
            if (TonesLB.SelectedItem != null && TonesLB.Items.Count > 0)
            {
                dynamic tone = TonesLB.SelectedItem;
                switch (CurrentGameVersion)
                {
                    case GameVersion.RS2012:
                        tone = GeneralExtensions.Copy<Tone>((Tone)TonesLB.SelectedItem);
                        break;
                    case GameVersion.RS2014:
                        tone = GeneralExtensions.Copy<Tone2014>((Tone2014)TonesLB.SelectedItem);
                        break;
                 }
                var name = GetUniqueToneName(TonesLB.Text);
                tone.Name = name;
                tone.Key = name;
                TonesLB.Items.Add(tone);

                dynamic firstTone = TonesLB.Items[0];
                foreach (var item in ArrangementLB.Items.OfType<Arrangement>())
                    if (tone.Name.Equals(item.ToneBase))
                        item.ToneBase = firstTone.Name;
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
                var toneName = tone.Name;
                using (var form = new ToneForm())
                {
                    form.CurrentGameVersion = CurrentGameVersion;
                    form.toneControl1.CurrentGameVersion = CurrentGameVersion;
                    form.toneControl1.Init();
                    form.toneControl1.Tone = GeneralExtensions.Copy(tone);
                    form.ShowDialog();
                    
                    if (form.Saved)
                        TonesLB.Items[TonesLB.SelectedIndex] = form.toneControl1.Tone;
                }
                if (toneName != tone.Name)
                {
                    // Update tone slots if name are changed
                    for(int i = 0; i <ArrangementLB.Items.Count; i++) {
                        var arrangement = (Arrangement)ArrangementLB.Items[i];
                        var toneSlotsAffected = false;

                        if (toneName.Equals(arrangement.ToneBase))
                        {
                            arrangement.ToneBase = tone.Name;
                            if (CurrentGameVersion == GameVersion.RS2014)
                                arrangement.ToneA = tone.Name;
                        }
                        if (CurrentGameVersion == GameVersion.RS2014) {
                            if (toneName.Equals(arrangement.ToneB))
                                arrangement.ToneB = tone.Name;
                            if (toneName.Equals(arrangement.ToneC))
                                arrangement.ToneC = tone.Name;
                            if (toneName.Equals(arrangement.ToneD))
                                arrangement.ToneD = tone.Name;
                        }

                        if (toneSlotsAffected)
                            ArrangementLB.Items[i] = arrangement;
                    }
                }
            }
        }

        private void toneImportButton_Click(object sender, EventArgs e)
        {
            ImportTone();
        }

        private void ImportTone() {
            string toneImportFile;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select DLC Song Package or Tone File or your Profile";
                ofd.Filter = CurrentOFDToneImportFilter;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                toneImportFile = ofd.FileName;
            }

            try
            {
                Application.DoEvents();
                if (CurrentGameVersion == GameVersion.RS2014)
                {
                    List<Tone2014> tones2014 = Tone2014.Import(toneImportFile);
                    //Popup ToneImportForm if tones > 1
                    if( tones2014.Count > 1 )
                        using( var importForm = new ToneImportForm())
                        {
                            importForm.Tone2014 = tones2014;
                            importForm.PopList();
                            if( importForm.ShowDialog() != DialogResult.OK )
                                return;
                            foreach(var tone in importForm.Tone2014.Where(t => !t.GearList.IsNull()) )
                                TonesLB.Items.Add(tone);
                        }
                    else if( tones2014.Count > 0 ) TonesLB.Items.Add( tones2014.FirstOrDefault(t => !t.GearList.IsNull()) );
                }
                else
                {
                    List<Tone> tones = Tone.Import(toneImportFile);
                    //Popup ToneImportForm if tones > 1
                    if( tones.Count > 1 )
                        using( var importForm = new ToneImportForm())
                        {
                            importForm.Tone = tones;
                            importForm.PopList();
                            if( importForm.ShowDialog() != DialogResult.OK )
                                return;
                            foreach(var tone in importForm.Tone.Where(t => t.PedalList.Count != 0) )
                                TonesLB.Items.Add(tone);
                        }
                    else if( tones.Count > 0 ) TonesLB.Items.Add( tones.FirstOrDefault(t => t.PedalList.Count != 0) );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't import tone(s). \n" + ex.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Tone(s) was imported.", "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void plataform_CheckedChanged(object sender, EventArgs e)
        {
            if (platformPC.Checked || platformMAC.Checked)
            {
                AppIdTB.Enabled = true;
                cmbAppIds.Enabled = true;
            }
            else if (!platformPC.Checked && !platformMAC.Checked)
            {
                AppIdTB.Enabled = false;
                cmbAppIds.Enabled = false;
            }
        }

        private void AppIdTB_TextChanged(object sender, EventArgs e) {
            var appId = ((TextBox)sender).Text.Trim();
            AppIdTB.TextChanged -= AppIdTB_TextChanged; 
            SelectComboAppId (appId);
            AppIdTB.TextChanged += AppIdTB_TextChanged;
        }

        private void SelectComboAppId(string appId) {
            SongAppId songAppId = SongAppIdRepository.Instance().Select(appId, CurrentGameVersion);
            if (SongAppIdRepository.Instance().List.Any<SongAppId>(a => a.AppId == appId))
                cmbAppIds.SelectedItem = songAppId;
        }

        private void DlcNameTB_Leave(object sender, EventArgs e)
        {
            TextBox dlcName = (TextBox)sender;
            dlcName.Text = dlcName.Text.Trim().GetValidSongName(SongTitle);
        }

        private void AverageTempoTB_Leave(object sender, EventArgs e)
        {
            TextBox averageTB = (TextBox)sender;
            float tempo = 0;
            float.TryParse(averageTB.Text.Trim(), out tempo);
            averageTB.Text = Math.Round(tempo).ToString();
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

            switch ((Keys)e.KeyValue) {
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
            platformMAC.Checked = CurrentGameVersion == GameVersion.RS2014;

            // Song preview volume RS2014 only
            previewVolumeBox.Enabled = CurrentGameVersion == GameVersion.RS2014;
            
            // AudioTB
            switch (CurrentGameVersion)
            {
                case GameVersion.RS2012:
                    audioPathTB.Cue = "Converted audio on Wwise 2010 for Windows, XBox360 or PS3 (*.ogg)";
                    break;
                case GameVersion.RS2014:
                    audioPathTB.Cue = "Converted audio on Wwise 2013 for Windows, Mac, XBox360 or PS3 (*.wem)";
                    break;
             }

            // Import Package RS2014 only
            dlcImportButton.Enabled = CurrentGameVersion == GameVersion.RS2014;
        }

        private void PopulateAppIdCombo()
        {
            cmbAppIds.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(CurrentGameVersion))
                cmbAppIds.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select((CurrentGameVersion == GameVersion.RS2014) ? ConfigRepository.Instance()["general_defaultappid_RS2014"] : ConfigRepository.Instance()["general_defaultappid_RS2012"], CurrentGameVersion);
            cmbAppIds.SelectedItem = songAppId;
            AppId = songAppId.AppId;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.ProgressPercentage <= updateProgress.Maximum)
                updateProgress.Value = e.ProgressPercentage;
            else
                updateProgress.Value = updateProgress.Maximum;

            ShowCurrentOperation(e.UserState as string);
        }

        private void ProcessCompleted(object sender, RunWorkerCompletedEventArgs e) {
            switch (Convert.ToString(e.Result)) {
                case "generate":
                    var message = "Package was generated.";
                    if (errorsFound.Length > 0)
                        message = String.Format ("Package was generated with errors! See below: {0}(1}", Environment.NewLine, errorsFound);
                    message += String.Format ("{0}You want to open the folder in which the package was generated?{0}", Environment.NewLine);
                    if (MessageBox.Show (message, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                        Process.Start (Path.GetDirectoryName (dlcSavePath));
                    }
                    break;
                case "error":
                    var message2 = String.Format ("Package generation failed. See below: {0}{1}{0}", Environment.NewLine, errorsFound);
                    MessageBox.Show (message2, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Parent.Focus();
                    break;
            }

            dlcGenerateButton.Enabled = true;
            updateProgress.Visible = false;
            currentOperationLabel.Visible = false;
        }

        private void ShowCurrentOperation(string message) {
            currentOperationLabel.Text = message;
            currentOperationLabel.Refresh();
        }

        private void ArtistSortTB_TextChanged(object sender, EventArgs e)
        {
            ArtistSortTB.TextChanged -= ArtistSortTB_TextChanged;
            var artist = ArtistSortTB.Text.ToUpperInvariant();
            if(artist.StartsWith("THE ")){
                ArtistSortTB.Text = artist.GetValidSortName();
            }
            ArtistSortTB.TextChanged += ArtistSortTB_TextChanged;
        }
        // Crunches here :)
        private void packageVersionTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            { }
            else if (e.KeyChar == Char.Parse("."))
            { }
            else if (e.KeyChar == (int)Keys.Back)
            { }
            else e.Handled = true;
        }
    }
}