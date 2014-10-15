using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Tone;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ArrangementForm : Form
    {
        private Arrangement _arrangement;
        private DLCPackageCreator parentControl = null;
        private Song2014 xmlSong = null;
        private GameVersion currentGameVersion;
        public bool EditMode = false;

        public Arrangement Arrangement {
            get
            {
                return _arrangement;
            }
            private set
            {
                _arrangement = value;

                //Song XML File
                XmlFilePath.Text = _arrangement.SongXml.File;

                //Arrangment Information
                arrangementTypeCombo.SelectedItem = _arrangement.ArrangementType;
                arrangementNameCombo.SelectedItem = _arrangement.Name;
                if (!ReferenceEquals(_arrangement.TuningStrings, null))
                    SetTuningCombo(_arrangement.TuningStrings, _arrangement.ArrangementType == ArrangementType.Bass);
                frequencyTB.Text = (_arrangement.TuningPitch > 0) ? _arrangement.TuningPitch.ToString() : "440";
                
                //Update it only here
                var scrollSpeed = _arrangement.ScrollSpeed;
                if (scrollSpeed == 0)
                    scrollSpeed = Convert.ToInt32(ConfigRepository.Instance().GetDecimal("creator_scrollspeed") * 10);
                scrollSpeedTrackBar.Value = Math.Min(scrollSpeed, scrollSpeedTrackBar.Maximum);
                UpdateScrollSpeedDisplay();
                
                Picked.Checked = _arrangement.PluckedType == PluckedType.Picked;
                BonusCheckBox.Checked = _arrangement.BonusArr;
                RouteMask = _arrangement.RouteMask;

                //DLC ID
                PersistentId.Text = _arrangement.Id.ToString().Replace("-", "").ToUpper();
                MasterId.Text = _arrangement.MasterId.ToString();
            }
        }

        private RouteMask RouteMask {
            get {
                if (routeMaskLeadRadio.Checked)
                    return RouteMask.Lead;
                else if (routeMaskRhythmRadio.Checked)
                    return RouteMask.Rhythm;
                else if (routeMaskBassRadio.Checked)
                    return RouteMask.Bass;
                else
                    return RouteMask.None;
            }
            set {
                switch (value)
                {
                    case RouteMask.Lead:
                        routeMaskLeadRadio.Checked = true;
                        break;
                    case RouteMask.Rhythm:
                        routeMaskRhythmRadio.Checked = true;
                        break;
                    case RouteMask.Bass:
                        routeMaskBassRadio.Checked = true;
                        break;
                    default:
                        routeMaskNoneRadio.Checked = true;
                        break;
                }
            }
        }

        public ArrangementForm(DLCPackageCreator control, GameVersion gameVersion)
            : this(new Arrangement
            {
                SongFile = new SongFile { File = "" },
                SongXml = new SongXML { File = "" },
                ArrangementType = ArrangementType.Guitar
            }, control, gameVersion)
        {
        }

        public ArrangementForm(Arrangement arrangement, DLCPackageCreator control, GameVersion gameVersion)
        {
            InitializeComponent();
            currentGameVersion = gameVersion;
            FillTuningCombo();

            foreach (var val in Enum.GetValues(typeof(ArrangementType)))
                arrangementTypeCombo.Items.Add(val);

            arrangementTypeCombo.SelectedValueChanged += (sender, e) => {
                // Selecting defaults
                var selectedType = ((ArrangementType)((ComboBox)sender).SelectedItem);
                
                switch (selectedType) {
                    case ArrangementType.Bass:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Bass);
                        arrangementNameCombo.SelectedItem = ArrangementName.Bass;
                        break;
                    case ArrangementType.Vocal:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Vocals);
                        arrangementNameCombo.SelectedItem = ArrangementName.Vocals;
                        break;
                    default:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Combo);
                        arrangementNameCombo.Items.Add(ArrangementName.Lead);
                        arrangementNameCombo.Items.Add(ArrangementName.Rhythm);
                        arrangementNameCombo.SelectedItem = arrangement.Name;
                        break;
                }

                var selectedArrangementName = ((ArrangementName)((ComboBox)arrangementNameCombo).SelectedItem);
                
                // Disabling options that are not meant for Arrangement Types
                // Arrangement Information
                arrangementNameCombo.Enabled = selectedType == ArrangementType.Guitar;
                tuningComboBox.Enabled = selectedType != ArrangementType.Vocal;
                gbTuningPitch.Enabled = selectedType != ArrangementType.Vocal && currentGameVersion == GameVersion.RS2014;
                gbScrollSpeed.Enabled = selectedType != ArrangementType.Vocal;
                Picked.Enabled = selectedType == ArrangementType.Bass;
                Picked.Checked = selectedType == ArrangementType.Bass ? false : true;
                BonusCheckBox.Enabled = selectedType != ArrangementType.Vocal && currentGameVersion == GameVersion.RS2014;
                UpdateCentOffset();

                // Gameplay Path
                UpdateRouteMaskPath(selectedType, selectedArrangementName);

                // Tone Selector
                gbTone.Enabled = selectedType != ArrangementType.Vocal;

                // Arrangement ID
                MasterId.Enabled = selectedType != ArrangementType.Vocal;
                PersistentId.Enabled = selectedType != ArrangementType.Vocal;
                
                // Tuning Edit
                tuningEditButton.Enabled = selectedType != ArrangementType.Vocal;

                // Vocal Edit
                vocalEdit.Enabled = selectedType == ArrangementType.Vocal;
            };

            arrangementNameCombo.SelectedValueChanged += (sender, e) =>
            {
                var selectedType = ((ArrangementType)((ComboBox)arrangementTypeCombo).SelectedItem);
                var selectedArrangementName = ((ArrangementName)((ComboBox)sender).SelectedItem);

                UpdateRouteMaskPath(selectedType, selectedArrangementName);
            };

            tuningComboBox.SelectedValueChanged += (sender, e) => {
                // Selecting defaults
                var selectedType = ((ArrangementType)arrangementTypeCombo.SelectedItem);
                var selectedTuning = ((TuningDefinition)((ComboBox)sender).SelectedItem);
                tuningEditButton.Enabled = selectedType != ArrangementType.Vocal && selectedTuning != null;
            };

            parentControl = control;

            //Tones setup
            SetupTones(arrangement);
            
            Arrangement = arrangement;
            EditMode = routeMaskNoneRadio.Checked;
        }

        private void UpdateRouteMaskPath(ArrangementType arrangementType, ArrangementName arrangementName)
        {
            gbGameplayPath.Enabled = arrangementType != ArrangementType.Vocal && currentGameVersion == GameVersion.RS2014;

            //Enabling
            routeMaskLeadRadio.Enabled = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Lead);
            routeMaskRhythmRadio.Enabled = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Rhythm);
            routeMaskBassRadio.Enabled = arrangementType == ArrangementType.Bass;
            
            //Auto-checking
            routeMaskLeadRadio.Checked = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Lead);
            routeMaskRhythmRadio.Checked = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Rhythm);
            routeMaskBassRadio.Checked = arrangementType == ArrangementType.Bass;
        }

        private void FillTuningCombo()
        {
            TuningDefinition firstTuning = null;
            tuningComboBox.Items.Clear();
            foreach (var tuning in TuningDefinitionRepository.Instance().Select(currentGameVersion))
            {
                tuningComboBox.Items.Add(tuning);
                if (firstTuning == null)
                    firstTuning = tuning;
            }
            tuningComboBox.SelectedItem = firstTuning;
            tuningComboBox.Refresh();
        }

        private void SetTuningCombo(TuningStrings tuningStrings, bool isBass = false) 
        {
            //Detect tuning
            TuningDefinition tuning = TuningDefinitionRepository.Instance().SelectAny(tuningStrings, currentGameVersion);

            //Create tuning
            if (tuning == null) {
                using (var form = new TuningForm()) {
                    tuning = new TuningDefinition();
                    tuning.Tuning = tuningStrings;
                    tuning.Custom = true;
                    tuning.GameVersion = currentGameVersion;
                    tuning.Name = tuning.UIName = Arrangement.Tuning;
                    if (String.IsNullOrEmpty(tuning.Name))
                    {
                        tuning.Name = tuning.UIName = tuning.NameFromStrings(tuningStrings, false);
                    }

                    form.Tuning = tuning;
                    form.IsBass = false;
                    if (DialogResult.OK != form.ShowDialog()) {
                        return;
                    }
                    //Update from xml definition
                    tuning = TuningDefinitionRepository.Instance().SelectAny(form.Tuning.Tuning, tuning.GameVersion);

                    FillTuningCombo();
                }
            }
            //Set tuning
            tuningComboBox.SelectedItem = tuning;
        }

        private void UpdateCentOffset() 
        {
            var value = frequencyTB.Text;
            if (!String.IsNullOrEmpty(value)) {
                double freq = 440;
                var isValid = Double.TryParse(value, out freq);
                if (isValid && freq > 0) {
                    string noteName;
                    TuningFrequency.Frequency2Note(freq, out noteName);
                    centOffsetDisplay.Text = String.Format("{0:0.00}", TuningFrequency.Frequency2Cents(freq));
                    noteDisplay.Text = noteName;
                }
            }
        }

        private void FillToneCombo(ComboBox combo, IEnumerable<string> toneNames, bool isBase)
        {
            var lastTone = combo.SelectedItem;
            combo.Items.Clear();
            if (!isBase)
                combo.Items.Add("");

            foreach (var tone in toneNames)
                combo.Items.Add(tone);

            combo.SelectedIndex = 0;
            if (isBase && !ReferenceEquals(lastTone, null))
            {
                combo.SelectedItem = lastTone;
            }
        }
        /// <summary>
        /// Fill toneCombo with autotone values or BaseOnly.
        /// Get tones, fill combo, select tones.
        /// </summary>
        /// <param name="arr"></param>
        private void SetupTones(Arrangement arr)
        {
            disableTonesCheckbox.Checked = false;
            bool isRS2014 = parentControl.CurrentGameVersion == GameVersion.RS2014;

            if (!String.IsNullOrEmpty(arr.ToneBase))
                if (parentControl.TonesLB.Items.Count == 1 && parentControl.TonesLB.Items[0].ToString() == "Default")
                    parentControl.TonesLB.Items.Clear();

            var toneItems = parentControl.TonesLB.Items;
            var toneNames = new List<string>();
            if (isRS2014)
                toneNames.AddRange(parentControl.TonesLB.Items.OfType<Tone2014>().Select(t => t.Name));
            else
                toneNames.AddRange(parentControl.TonesLB.Items.OfType<Tone>().Select(t => t.Name));

            //Check if autotone tones are present and add it's if not.
            if (!toneNames.Contains(arr.ToneBase) && !String.IsNullOrEmpty(arr.ToneBase))
            {
                toneItems.Add(parentControl.CreateNewTone(arr.ToneBase));
                toneNames.Add(arr.ToneBase);
            }
            if (!toneNames.Contains(arr.ToneA) && !String.IsNullOrEmpty(arr.ToneA))
            {
                toneItems.Add(parentControl.CreateNewTone(arr.ToneA));
                toneNames.Add(arr.ToneA);
            }
            if (!toneNames.Contains(arr.ToneB) && !String.IsNullOrEmpty(arr.ToneB))
            {
                toneItems.Add(parentControl.CreateNewTone(arr.ToneB));
                toneNames.Add(arr.ToneB);
            }
            if (!toneNames.Contains(arr.ToneC) && !String.IsNullOrEmpty(arr.ToneC))
            {
                toneItems.Add(parentControl.CreateNewTone(arr.ToneC));
                toneNames.Add(arr.ToneC);
            }
            if (!toneNames.Contains(arr.ToneD) && !String.IsNullOrEmpty(arr.ToneD))
            {
                toneItems.Add(parentControl.CreateNewTone(arr.ToneD));
                toneNames.Add(arr.ToneD);
            }

            // FILL TONE COMBO
            FillToneCombo(toneBaseCombo, toneNames, true);
            FillToneCombo(toneBCombo, toneNames, false);
            FillToneCombo(toneCCombo, toneNames, false);
            FillToneCombo(toneDCombo, toneNames, false);

            // SELECTING TONES
            toneBaseCombo.Enabled = true;
            if (!String.IsNullOrEmpty(arr.ToneBase))
                toneBaseCombo.SelectedItem = arr.ToneBase;
            if (!String.IsNullOrEmpty(arr.ToneB))
                toneBCombo.SelectedItem = arr.ToneB;
            if (!String.IsNullOrEmpty(arr.ToneC))
                toneCCombo.SelectedItem = arr.ToneC;
            if (!String.IsNullOrEmpty(arr.ToneD))
                toneDCombo.SelectedItem = arr.ToneD;

            // If have ToneBase and ToneB is setup it's because auto tone are setup in EoF, so, disable edit to prevent errors.
            disableTonesCheckbox.Checked = (!String.IsNullOrEmpty(arr.ToneBase) && !String.IsNullOrEmpty(arr.ToneB));
        }

        private void SequencialToneComboEnabling() {
            toneCCombo.Enabled = !String.IsNullOrEmpty((string)toneBCombo.SelectedItem) && toneBCombo.SelectedIndex > 0;
            toneDCombo.Enabled = !String.IsNullOrEmpty((string)toneCCombo.SelectedItem) && toneCCombo.SelectedIndex > 0;
        }

        private bool isAlreadyAdded(string xmlPath)
        {
            for (int i = 0; i < parentControl.ArrangementLB.Items.Count; i++)
            {
                var selectedArrangement = (Arrangement)parentControl.ArrangementLB.Items[i];

                if (xmlPath.Equals(selectedArrangement.SongXml.File))
                {
                    if (xmlPath.Equals(Arrangement.SongXml.File)) 
                        continue;
                    else
                        return true;
                }
            }
            return false;
        }

        #region UI events with helpers
        private void songXmlBrowseButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Rocksmith Song Xml Files (*.xml)|*.xml";
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (isAlreadyAdded(ofd.FileName))
                {
                    MessageBox.Show("This arrangement already added, please choose another one. ", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                XmlFilePath.Text = ofd.FileName;
            }
            try {
                bool isVocal = false;
                try {
                    xmlSong = Song2014.LoadFromFile(XmlFilePath.Text);
                } catch (Exception ex) {
                    if (ex.InnerException.Message.ToLower().Contains("<vocals"))
                        isVocal = true;
                    else {
                        MessageBox.Show("Unable to get information from the arrangement XML. \nYour version of the EoF is up to date? \n" + ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // SETUP FIELDS
                if (isVocal) {
                    arrangementTypeCombo.SelectedItem = ArrangementType.Vocal;
                }
                else
                {
                    var version = GameVersion.None;
                    //Detect arrangement GameVersion
                    if (xmlSong != null && xmlSong.Version != null)
                    {
                        var verAttrib = Convert.ToInt32(xmlSong.Version);
                        if (verAttrib <= 6) version = GameVersion.RS2012;
                        else if (verAttrib >= 7) version = GameVersion.RS2014;
                    }
                    else
                        MessageBox.Show("You are using a old version of EoF application, please update first.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (currentGameVersion != version)
                    {
                        MessageBox.Show(String.Format("Please choose valid Rocksmith {0} arrangement file!", currentGameVersion));
                        XmlFilePath.Text = "";
                        return;
                    } 

                    //Setup tuning
                    SetTuningCombo(xmlSong.Tuning);

                    // SONG AND ARRANGEMENT INFO / ROUTE MASK
                    BonusCheckBox.Checked = Equals(xmlSong.ArrangementProperties.BonusArr, 1);
                    if (EditMode)
                    {
                        string arr = xmlSong.Arrangement.ToLowerInvariant();
                        if (arr.Contains("guitar") || arr.Contains("lead") || arr.Contains("rhythm") || arr.Contains("combo"))
                        {
                            arrangementTypeCombo.SelectedItem = ArrangementType.Guitar;

                            if (arr.Contains("guitar 22") || arr.Contains("lead") || arr.Contains("combo") || Equals(xmlSong.ArrangementProperties.PathLead, 1))
                            {
                                arrangementNameCombo.SelectedItem = ArrangementName.Lead;
                                if (currentGameVersion == GameVersion.RS2014) RouteMask = RocksmithToolkitLib.DLCPackage.RouteMask.Lead;
                            }
                            if (arr.Contains("guitar") || arr.Contains("rhythm") || Equals(xmlSong.ArrangementProperties.PathRhythm, 1))
                            {
                                arrangementNameCombo.SelectedItem = ArrangementName.Rhythm;
                                if (currentGameVersion == GameVersion.RS2014) RouteMask = RocksmithToolkitLib.DLCPackage.RouteMask.Rhythm;
                            }

                        }
                        if (arr.Contains("bass"))
                        {
                            arrangementTypeCombo.SelectedItem = ArrangementType.Bass;

                            //SetTuningCombo(xmlSong.Tuning, true);
                            Picked.Checked = Equals(xmlSong.ArrangementProperties.BassPick, 1);
                            if (currentGameVersion == GameVersion.RS2014)
                            {
                                RouteMask = RocksmithToolkitLib.DLCPackage.RouteMask.Bass;
                                //Low tuning fix for bass, If lowstring is B
                                if(xmlSong.Tuning.String0 < -4 && frequencyTB.Text != "440")
                                if(MessageBox.Show("Your tuning is too low, would you like to apply \"Low Tuning Fix?\"\n"+
                                   "Note, that this won't work if you re-save arangement in EOF.\n", "",
                                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {// Apply fix here 
                                    MessageBox.Show("Sorry, not implemented yet, you'd better do it yourself ;)\n");
                                }
                            }
                        }
                    }
                    //Tones setup
                    if (currentGameVersion == GameVersion.RS2014)
                    {
                        Arrangement.ToneBase = xmlSong.ToneBase;
                        Arrangement.ToneA = xmlSong.ToneA;
                        Arrangement.ToneB = xmlSong.ToneB;
                        Arrangement.ToneC = xmlSong.ToneC;
                        Arrangement.ToneD = xmlSong.ToneD;
                        Arrangement.ToneMultiplayer = null;

                        SetupTones(Arrangement);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Unable to get information from the arrangement XML. \nYour version of the EoF is up to date? \n" + ex.Message, 
                                DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void addArrangementButton_Click(object sender, EventArgs e)
        {
            //Validations
            var xmlfilepath = XmlFilePath.Text;
            if (!File.Exists(xmlfilepath))
            {
                XmlFilePath.Focus();
                return;
            }

            if (currentGameVersion == GameVersion.RS2014)
            {
                if (!routeMaskLeadRadio.Checked && !routeMaskRhythmRadio.Checked && !routeMaskBassRadio.Checked && (ArrangementType)arrangementTypeCombo.SelectedItem != ArrangementType.Vocal)
                {
                    if (MessageBox.Show("You not selected a Gameplay Path, this arrangement you show only in song list.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        gbGameplayPath.Focus();
                        return;
                    }
                }
            }
            
            //Song XML File
            Arrangement.SongXml.File = xmlfilepath;

            // SONG INFO
            if (!ReferenceEquals(xmlSong, null))
            {
                if (String.IsNullOrEmpty(parentControl.SongTitle)) parentControl.SongTitle = xmlSong.Title ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.SongTitleSort)) parentControl.SongTitleSort = xmlSong.SongNameSort.GetValidSortName() ?? parentControl.SongTitle.GetValidSortName();
                if (String.IsNullOrEmpty(parentControl.AverageTempo)) parentControl.AverageTempo = Math.Round(xmlSong.AverageTempo).ToString() ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.Artist)) parentControl.Artist = xmlSong.ArtistName ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.ArtistSort)) parentControl.ArtistSort = xmlSong.ArtistNameSort.GetValidSortName() ?? parentControl.Artist.GetValidSortName();
                if (String.IsNullOrEmpty(parentControl.Album)) parentControl.Album = xmlSong.AlbumName ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.AlbumYear)) parentControl.AlbumYear = xmlSong.AlbumYear ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.DLCName)) parentControl.DLCName = parentControl.Artist.Acronym() + parentControl.SongTitleSort;
            }

            //Arrangment Information
            Arrangement.Name = (ArrangementName)arrangementNameCombo.SelectedItem;
            Arrangement.ArrangementType = (ArrangementType)arrangementTypeCombo.SelectedItem;
            Arrangement.ScrollSpeed = scrollSpeedTrackBar.Value;
            Arrangement.PluckedType = Picked.Checked ? PluckedType.Picked : PluckedType.NotPicked;
            Arrangement.BonusArr = BonusCheckBox.Checked;
            
            // Tuning
            Arrangement.Tuning = tuningComboBox.SelectedItem.ToString();
            if (!ReferenceEquals(xmlSong, null))
                Arrangement.TuningStrings = xmlSong.Tuning;
            if (!ReferenceEquals(xmlSong, null))
                Arrangement.CapoFret = xmlSong.Capo;
            Arrangement.TuningPitch = 440;
            var value = frequencyTB.Text;
            if (!String.IsNullOrEmpty(value)) {
                double freq = 440;
                Double.TryParse(value, out freq);
                Arrangement.TuningPitch = freq;
            }
            
            //ToneSelector
            Arrangement.ToneBase = toneBaseCombo.SelectedItem.ToString();
            Arrangement.ToneA = (toneBCombo.SelectedItem != null) ? toneBaseCombo.SelectedItem.ToString() : ""; //Only need if have more than one tone
            Arrangement.ToneB = (toneBCombo.SelectedItem != null) ? toneBCombo.SelectedItem.ToString() : "";
            Arrangement.ToneC = (toneCCombo.SelectedItem != null) ? toneCCombo.SelectedItem.ToString() : "";
            Arrangement.ToneD = (toneDCombo.SelectedItem != null) ? toneDCombo.SelectedItem.ToString() : "";
            
            //Gameplay Path
            Arrangement.RouteMask = RouteMask;

            //Xml data cleanup
            xmlSong = null;
            
            // DLC IDs
            Guid guid;
            if (Guid.TryParse(PersistentId.Text, out guid) == false)
                PersistentId.Focus();
            else
                Arrangement.Id = guid;

            int masterId;
            if (int.TryParse(MasterId.Text, out masterId) == false)
                MasterId.Focus();
            else
                Arrangement.MasterId = masterId;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void toneCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SequencialToneComboEnabling();
        }

        private void frequencyTB_TextChanged(object sender, EventArgs e) {
            UpdateCentOffset();
        }

        private void ToneComboEnabled(bool enabled) {
            // Not disabling in gbTone to not disable labels
            toneBaseCombo.Enabled = enabled;
            toneBCombo.Enabled = enabled;
            toneCCombo.Enabled = enabled;
            toneDCombo.Enabled = enabled;
        }

        private void disableTonesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = !disableTonesCheckbox.Checked;
            ToneComboEnabled(enabled);
            if (enabled)
                SequencialToneComboEnabling();
        }

        private void vocalEdit_Click(object sender, EventArgs e)
        {
        }
        
        private void tuningEditButton_Click(object sender, EventArgs e) 
        {
            var selectedType = ((ArrangementType)((ComboBox)arrangementTypeCombo).SelectedItem);
            TuningDefinition tuning = (TuningDefinition)tuningComboBox.SelectedItem;

            if (tuning == null) {
                MessageBox.Show("At least one tuning definition is needed to edit.\r\n (Current tuning is Null)", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            using (var form = new TuningForm()) 
            {
                form.Tuning = tuning;
                form.IsBass = selectedType == ArrangementType.Bass;
                form.EditMode = true;

                var oldUIName = tuning.UIName.Clone().ToString(); //Important to set it here
                if (DialogResult.OK != form.ShowDialog()) {
                    return;
                }
                                
                if (oldUIName != form.Tuning.UIName && !form.AddNew)
                {
                    // Update LB slots if tuning name are changed
                    for (int i = 0; i < parentControl.ArrangementLB.Items.Count; i++)
                    {
                        var selectedArrangement = (Arrangement)parentControl.ArrangementLB.Items[i];

                        if (oldUIName.Equals(selectedArrangement.Tuning))
                        {
                            selectedArrangement.Tuning = form.Tuning.UIName;
                            parentControl.ArrangementLB.Items[i] = selectedArrangement;
                        }   
                    }
                }
                //Update from xml definition
                tuning = form.Tuning;
                tuning = TuningDefinitionRepository.Instance().SelectAny(tuning.Tuning, tuning.GameVersion);
                
                FillTuningCombo();
                tuningComboBox.SelectedItem = tuning;
                Arrangement.TuningStrings = tuning.Tuning;
            }
        }

        private void scrollSpeedTrackBar_ValueChanged(object sender, EventArgs e) {
            UpdateScrollSpeedDisplay();
        }

        private void UpdateScrollSpeedDisplay()
        {
            scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeedTrackBar.Value) / 10);
        }
        #endregion
    }
}
