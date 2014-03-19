using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using RocksmithToolkitLib.DLCPackage;
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
        private Arrangement arrangement;
        private DLCPackageCreator parentControl = null;
        private GameVersion currentGameVersion;

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

        public ArrangementForm(IEnumerable<string> toneNames, DLCPackageCreator control, GameVersion gameVersion)
            : this(new Arrangement
            {
                SongFile = new SongFile { File = "" },
                SongXml = new SongXML { File = "" },
                ArrangementType = ArrangementType.Guitar
            }, toneNames, control, gameVersion)
        {
        }

        public ArrangementForm(Arrangement arrangement, IEnumerable<string> toneNames, DLCPackageCreator control, GameVersion gameVersion)
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
                toneBCombo.Enabled = currentGameVersion == GameVersion.RS2014;
                toneCCombo.Enabled = currentGameVersion == GameVersion.RS2014;
                toneDCombo.Enabled = currentGameVersion == GameVersion.RS2014;

                SequencialToneComboEnabling();

                // Arrangement ID
                MasterId.Enabled = selectedType != ArrangementType.Vocal;
                PersistentId.Enabled = selectedType != ArrangementType.Vocal;
                
                // Tuning Edit
                tuningEditButton.Enabled = selectedType != ArrangementType.Vocal;
            };

            arrangementNameCombo.SelectedValueChanged += (sender, e) =>
            {
                var selectedType = ((ArrangementType)((ComboBox)arrangementTypeCombo).SelectedItem);
                var selectedArrangementName = ((ArrangementName)((ComboBox)sender).SelectedItem);

                UpdateRouteMaskPath(selectedType, selectedArrangementName);
            };

            tuningComboBox.SelectedValueChanged += (sender, e) => {
                // Selecting defaults
                var selectedType = ((ArrangementType)((ComboBox)arrangementTypeCombo).SelectedItem);
                var selectedTuning = ((TuningDefinition)((ComboBox)sender).SelectedItem);
                tuningEditButton.Enabled = selectedType != ArrangementType.Vocal && selectedTuning != null;
            };

            FillToneCombo(toneBaseCombo, toneNames, true);
            FillToneCombo(toneBCombo, toneNames, false);
            FillToneCombo(toneCCombo, toneNames, false);
            FillToneCombo(toneDCombo, toneNames, false);

            var scrollSpeed = arrangement.ScrollSpeed;
            if (scrollSpeed == 0)
                scrollSpeed = Convert.ToInt32(ConfigRepository.Instance().GetDecimal("creator_scrollspeed") * 10);
            scrollSpeedTrackBar.Value = scrollSpeed;            
            
            Arrangement = arrangement;
            parentControl = control;
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

        private void FillToneCombo(ComboBox combo, IEnumerable<string> toneNames, bool isBase)
        {
            combo.Items.Clear();
            if (!isBase)
                combo.Items.Add("");

            foreach (var tone in toneNames)
                combo.Items.Add(tone);

            combo.SelectedIndex = 0;
        }

        private void SequencialToneComboEnabling() {
            toneCCombo.Enabled = toneBCombo.SelectedIndex > 0;
            toneDCombo.Enabled = toneCCombo.SelectedIndex > 0;
        }

        public Arrangement Arrangement
        {
            get
            {
                return arrangement;
            }
            private set
            {
                arrangement = value;

                //Song XML File
                XmlFilePath.Text = arrangement.SongXml.File;
                //Arrangment Information
                arrangementTypeCombo.SelectedItem = arrangement.ArrangementType;
                arrangementNameCombo.SelectedItem = arrangement.Name;
                TuningDefinition tuning = TuningDefinitionRepository.Instance().Select(arrangement.Tuning, currentGameVersion);
                if (tuning != null)
                    tuningComboBox.SelectedItem = tuning;
                frequencyTB.Text = (arrangement.TuningPitch > 0) ? arrangement.TuningPitch.ToString() : "440";

                var scrollSpeed = arrangement.ScrollSpeed;
                if (scrollSpeed == 0)
                    scrollSpeed = Convert.ToInt32(ConfigRepository.Instance().GetDecimal("creator_scrollspeed") * 10);
                scrollSpeedTrackBar.Value = scrollSpeed;
                
                Picked.Checked = arrangement.PluckedType == PluckedType.Picked;
                BonusCheckBox.Checked = arrangement.BonusArr;
                RouteMask = arrangement.RouteMask;
                //Tone Selector
                toneBaseCombo.SelectedItem = arrangement.ToneBase;
                if (toneBaseCombo.SelectedItem == null && toneBaseCombo.Items.Count > 0)
                    toneBaseCombo.SelectedItem = toneBaseCombo.Items[0];
                toneBCombo.SelectedItem = arrangement.ToneB;
                toneCCombo.SelectedItem = arrangement.ToneC;
                toneDCombo.SelectedItem = arrangement.ToneD;
                
                // If have ToneBase and ToneB is setup it's because auto tone are setup in EoF, so, disable edit to prevent errors.
                disableTonesCheckbox.Checked = ((toneBaseCombo.SelectedItem != null && !String.IsNullOrEmpty(toneBaseCombo.SelectedItem.ToString())) && (toneBCombo.SelectedItem != null && !String.IsNullOrEmpty(toneBCombo.SelectedItem.ToString()))); ;

                //DLC ID
                PersistentId.Text = arrangement.Id.ToString().Replace("-", "").ToUpper();
                MasterId.Text = arrangement.MasterId.ToString();
            }
        }

        private void songXmlBrowseButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Rocksmith Song Xml Files (*.xml)|*.xml";
                if (ofd.ShowDialog() == DialogResult.OK)
                    XmlFilePath.Text = ofd.FileName;
                else
                    return;
            }
            try {
                Song2014 xmlSong = null;
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

                    if (xmlSong != null && xmlSong.Version != null)
                    {
                        var verAttrib = Convert.ToInt32(xmlSong.Version);
                        if (verAttrib <= 6) version = GameVersion.RS2012;
                        else if (verAttrib >= 7) version = GameVersion.RS2014;
                    }
                    else
                        MessageBox.Show("You are using a old version of EoF application, please update first.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (!(currentGameVersion == version))
                    {
                        MessageBox.Show(String.Format("Please choose valid Rocksmith {0} arrangement file!", currentGameVersion));
                        XmlFilePath.Text = "";
                        return;
                    } 

                    // SONG INFO
                    if (String.IsNullOrEmpty(parentControl.SongTitle)) parentControl.SongTitle = xmlSong.Title ?? String.Empty;
                    if (String.IsNullOrEmpty(parentControl.SongTitleSort)) parentControl.SongTitleSort = xmlSong.SongNameSort ?? parentControl.SongTitle;
                    if (String.IsNullOrEmpty(parentControl.DLCName)) parentControl.DLCName = parentControl.SongTitleSort;
                    if (String.IsNullOrEmpty(parentControl.AverageTempo)) parentControl.AverageTempo = Math.Round(xmlSong.AverageTempo).ToString() ?? String.Empty;
                    if (String.IsNullOrEmpty(parentControl.Artist)) parentControl.Artist = xmlSong.ArtistName ?? String.Empty;
                    if (String.IsNullOrEmpty(parentControl.ArtistSort)) parentControl.ArtistSort = xmlSong.ArtistNameSort ?? parentControl.Artist;
                    if (String.IsNullOrEmpty(parentControl.Album)) parentControl.Album = xmlSong.AlbumName ?? String.Empty;
                    if (String.IsNullOrEmpty(parentControl.AlbumYear)) parentControl.AlbumYear = xmlSong.AlbumYear ?? String.Empty;

                    //Setup tuning
                    SetTuningCombo(xmlSong.Tuning);

                    // SONG AND ARRANGEMENT INFO / ROUTE MASK
                    string arr = xmlSong.Arrangement;
                    bool Edit = routeMaskNoneRadio.Checked;
                    if (arr.ToLower().Contains("guitar") || arr.ToLower().Contains("lead") || arr.ToLower().Contains("rhythm") || arr.ToLower().Contains("combo"))
                    {
                        arrangementTypeCombo.SelectedItem = ArrangementType.Guitar;
                        if (Edit & arr.ToLower().Contains("guitar 22") || arr.ToLower().Contains("lead") || arr.ToLower().Contains("combo"))
                        {
                            arrangementNameCombo.SelectedItem = ArrangementName.Lead;
                            if (currentGameVersion == GameVersion.RS2014) RouteMask = RocksmithToolkitLib.DLCPackage.RouteMask.Lead;
                        }
                        if (Edit & arr.ToLower().Contains("guitar") || arr.ToLower().Contains("rhythm"))
                        {
                            arrangementNameCombo.SelectedItem = ArrangementName.Rhythm;
                            if (currentGameVersion == GameVersion.RS2014) RouteMask = RocksmithToolkitLib.DLCPackage.RouteMask.Rhythm;
                        }
                    }
                    if (arr.ToLower().Contains("bass"))
                    {
                        SetTuningCombo(xmlSong.Tuning, true);
                        arrangementTypeCombo.SelectedItem = ArrangementType.Bass;
                        Picked.Checked = xmlSong.ArrangementProperties.BassPick == 1;
                        if (currentGameVersion == GameVersion.RS2014) RouteMask = RocksmithToolkitLib.DLCPackage.RouteMask.Bass;
                    }
                        
                    if (currentGameVersion == GameVersion.RS2014)
                    {
                        // TONES
                        if (xmlSong.Tones != null && xmlSong.Tones.Count() > 0 && xmlSong.ToneBase != null)
                        {
                            // FILL TONE COMBO
                            List<string> toneNames = new List<string>();

                            if (parentControl.TonesLB.Items.Count == 1 && parentControl.TonesLB.Items[0].ToString() == "Default")
                                parentControl.TonesLB.Items.Clear();
                            else
                                toneNames.AddRange(parentControl.TonesLB.Items.OfType<Tone2014>().Select(t => t.Name));

                            if (!toneNames.Contains(xmlSong.ToneBase))
                                toneNames.Add(xmlSong.ToneBase);
                            if (!toneNames.Contains(xmlSong.ToneB))
                                toneNames.Add(xmlSong.ToneB);
                            if (!toneNames.Contains(xmlSong.ToneC))
                                if (!String.IsNullOrEmpty(xmlSong.ToneC))
                                    toneNames.Add(xmlSong.ToneC);
                            if (!toneNames.Contains(xmlSong.ToneD))
                                if (!String.IsNullOrEmpty(xmlSong.ToneD))
                                    toneNames.Add(xmlSong.ToneD);

                            FillToneCombo(toneBaseCombo, toneNames, true);
                            FillToneCombo(toneBCombo, toneNames, false);
                            FillToneCombo(toneCCombo, toneNames, false);
                            FillToneCombo(toneDCombo, toneNames, false);

                            // SELECTING TONES
                            toneBaseCombo.SelectedItem = xmlSong.ToneBase;
                            if (!parentControl.TonesLB.Items.OfType<Tone2014>().Any(t => t.Name == xmlSong.ToneBase))
                                parentControl.TonesLB.Items.Add(parentControl.CreateNewTone(xmlSong.ToneBase));

                            toneBCombo.SelectedItem = xmlSong.ToneB;
                            if (!parentControl.TonesLB.Items.OfType<Tone2014>().Any(t => t.Name == xmlSong.ToneB))
                                parentControl.TonesLB.Items.Add(parentControl.CreateNewTone(xmlSong.ToneB));

                            if (!String.IsNullOrEmpty(xmlSong.ToneC))
                            {
                                toneCCombo.Enabled = true;
                                toneCCombo.SelectedItem = xmlSong.ToneC;
                                if (!parentControl.TonesLB.Items.OfType<Tone2014>().Any(t => t.Name == xmlSong.ToneC))
                                    parentControl.TonesLB.Items.Add(parentControl.CreateNewTone(xmlSong.ToneC));
                            }

                            if (!String.IsNullOrEmpty(xmlSong.ToneD))
                            {
                                toneDCombo.Enabled = true;
                                toneDCombo.SelectedItem = xmlSong.ToneD;
                                if (!parentControl.TonesLB.Items.OfType<Tone2014>().Any(t => t.Name == xmlSong.ToneD))
                                    parentControl.TonesLB.Items.Add(parentControl.CreateNewTone(xmlSong.ToneD));
                            }

                            // If have ToneBase and ToneB is setup it's because auto tone are setup in EoF, so, disable edit to prevent errors.
                            disableTonesCheckbox.Checked = (!String.IsNullOrEmpty(xmlSong.ToneBase) && !String.IsNullOrEmpty(xmlSong.ToneB));
                        }
                        else
                        {
                            disableTonesCheckbox.Checked = false;

                            toneBaseCombo.Enabled = true;
                            if (xmlSong.ToneBase != null)
                                toneBaseCombo.SelectedItem = xmlSong.ToneBase;
                            else
                                toneBaseCombo.SelectedIndex = 0;

                            if (xmlSong.ToneB != null)
                                toneBCombo.SelectedItem = xmlSong.ToneB;
                            else
                                toneBCombo.SelectedIndex = 0;

                            if (xmlSong.ToneC != null)
                                toneCCombo.SelectedItem = xmlSong.ToneC;
                            else
                                toneBCombo.SelectedIndex = 0;

                            if (xmlSong.ToneD != null)
                                toneDCombo.SelectedItem = xmlSong.ToneD;
                            else
                                toneBCombo.SelectedIndex = 0;

                            SequencialToneComboEnabling();
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Unable to get information from the arrangement XML. \nYour version of the EoF is up to date? \n" + ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetTuningCombo(TuningStrings tuningStrings, bool isBass = false) {
            //Detect tuning
            TuningDefinition tuning = TuningDefinitionRepository.Instance().SelectAny(tuningStrings, currentGameVersion);
            //Create tuning
            if (tuning == null) {
                using (var form = new TuningForm()) {
                    tuning = new TuningDefinition();
                    tuning.Tuning = tuningStrings;
                    tuning.Custom = true;
                    tuning.GameVersion = currentGameVersion;
                    tuning.Name = tuning.UIName = tuning.NameFromStrings(tuningStrings, isBass);

                    form.Tuning = tuning;
                    form.IsBass = isBass;
                    if (DialogResult.OK != form.ShowDialog()) {
                        return;
                    }

                    FillTuningCombo();
                }
            }
            //Set tuning
            tuningComboBox.SelectedItem = tuning;
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
            arrangement.SongXml.File = xmlfilepath;
            
            //Arrangment Information
            arrangement.Name = (ArrangementName)arrangementNameCombo.SelectedItem;
            arrangement.ArrangementType = (ArrangementType)arrangementTypeCombo.SelectedItem;

            // Tuning
            arrangement.Tuning = tuningComboBox.SelectedItem.ToString();
            arrangement.TuningPitch = 440;
            var value = frequencyTB.Text;
            if (!String.IsNullOrEmpty(value)) {
                double freq = 440;
                Double.TryParse(value, out freq);
                arrangement.TuningPitch = freq;
            }

            arrangement.ScrollSpeed = scrollSpeedTrackBar.Value;
            arrangement.PluckedType = Picked.Checked ? PluckedType.Picked : PluckedType.NotPicked;
            arrangement.BonusArr = BonusCheckBox.Checked;

            //ToneSelector
            //TODO if tone not exist - create empty Tone instance and add it to tonesLB, used for autotone.
            arrangement.ToneBase = toneBaseCombo.SelectedItem.ToString();
            arrangement.ToneA = (toneBCombo.SelectedItem != null) ? toneBaseCombo.SelectedItem.ToString() : ""; //Only need if have more than one tone
            arrangement.ToneB = (toneBCombo.SelectedItem != null) ? toneBCombo.SelectedItem.ToString() : "";
            arrangement.ToneC = (toneCCombo.SelectedItem != null) ? toneCCombo.SelectedItem.ToString() : "";
            arrangement.ToneD = (toneDCombo.SelectedItem != null) ? toneDCombo.SelectedItem.ToString() : "";
            
            //Gameplay Path
            arrangement.RouteMask = RouteMask;
            
            // DLC ID
            Guid guid;
            if (Guid.TryParse(PersistentId.Text, out guid) == false)
                PersistentId.Focus();
            else
                arrangement.Id = guid;

            int masterId;
            if (int.TryParse(MasterId.Text, out masterId) == false)
                MasterId.Focus();
            else
                arrangement.MasterId = masterId;

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

        private void UpdateCentOffset() {
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

        private void tuningEditButton_Click(object sender, EventArgs e) {
            var selectedType = ((ArrangementType)((ComboBox)arrangementTypeCombo).SelectedItem);
            TuningDefinition selectedTuning = (TuningDefinition)tuningComboBox.SelectedItem;

            if (selectedTuning == null) {
                MessageBox.Show("At least one tuning definition is needed to edit.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var form = new TuningForm()) {
                form.Tuning = selectedTuning;
                form.IsBass = selectedType == ArrangementType.Bass;
                form.EditMode = true;

                var oldUIName = selectedTuning.UIName.Clone().ToString();
                if (DialogResult.OK != form.ShowDialog()) {
                    return;
                }

                if (oldUIName != form.Tuning.UIName)
                {
                    // Update tone slots if name are changed
                    for (int i = 0; i < parentControl.ArrangementLB.Items.Count; i++)
                    {
                        var arrangement = (Arrangement)parentControl.ArrangementLB.Items[i];
                        
                        if (oldUIName.Equals(arrangement.Tuning))
                        {
                            arrangement.Tuning = form.Tuning.UIName;
                            parentControl.ArrangementLB.Items[i] = arrangement;
                        }   
                    }
                }

                FillTuningCombo();
                tuningComboBox.SelectedItem = (form.IsBass) ? TuningDefinitionRepository.Instance().SelectForBass(form.Tuning.Tuning, currentGameVersion) : TuningDefinitionRepository.Instance().Select(form.Tuning.Tuning, currentGameVersion);
            }
        }

        private void scrollSpeedTrackBar_ValueChanged(object sender, EventArgs e) {
            scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeedTrackBar.Value) / 10);
        }
    }
}
