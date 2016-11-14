using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Song2014ToTab;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ArrangementForm : Form
    {
        #region Fields
        private Arrangement _arrangement;
        private DLCPackageCreator parentControl = null;
        private Song2014 xmlSong = null;
        private GameVersion currentGameVersion;
        public bool EditMode = false;
        private bool bassFix = false;
        private ToolTip tt = new ToolTip();

        public Arrangement Arrangement
        {
            get
            {
                return _arrangement;
            }
            private set
            {
                _arrangement = value;

                //Song XML File
                XmlFilePath.Text = value.SongXml.File;

                //Arrangement Information
                arrangementTypeCombo.SelectedItem = value.ArrangementType;
                arrangementNameCombo.SelectedItem = value.Name;
                if (!String.IsNullOrEmpty(value.Tuning))
                    tuningComboBox.SelectedIndex = tuningComboBox.FindStringExact(value.Tuning);
                frequencyTB.Text = (value.TuningPitch > 0) ? value.TuningPitch.ToString() : "440.00";

                //Update it only here
                var scrollSpeed = value.ScrollSpeed;
                if (scrollSpeed == 0)
                    scrollSpeed = Convert.ToInt32(ConfigRepository.Instance().GetDecimal("creator_scrollspeed") * 10);
                scrollSpeedTrackBar.Value = Math.Min(scrollSpeed, scrollSpeedTrackBar.Maximum);
                UpdateScrollSpeedDisplay();

                Picked.Checked = value.PluckedType == PluckedType.Picked;
                BonusCheckBox.Checked = value.BonusArr;
                MetronomeCb.Checked = value.Metronome == Metronome.Generate;
                RouteMask = value.RouteMask;

                //DLC ID
                PersistentId.Text = value.Id.ToString().Replace("-", "").ToUpper();
                MasterId.Text = value.MasterId.ToString();
            }
        }

        private RouteMask RouteMask
        {
            get
            {
                if (routeMaskLeadRadio.Checked)
                    return RouteMask.Lead;
                if (routeMaskRhythmRadio.Checked)
                    return RouteMask.Rhythm;
                if (routeMaskBassRadio.Checked)
                    return RouteMask.Bass;
                return RouteMask.None;
            }
            set
            {
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
        #endregion

        public ArrangementForm(DLCPackageCreator control, GameVersion gameVersion)
            : this(new Arrangement
            {
                SongFile = new SongFile { File = "" },
                SongXml = new SongXML { File = "" },
                ArrangementType = ArrangementType.Guitar
            }, control, gameVersion)
        {
            Console.WriteLine("Debug");
        }

        public ArrangementForm(Arrangement arrangement, DLCPackageCreator control, GameVersion gameVersion) // , string projectDir = "")
        {
            InitializeComponent();

            currentGameVersion = gameVersion == GameVersion.RS2012 ? GameVersion.RS2012 : GameVersion.RS2014;
            FillTuningCombo(arrangement.ArrangementType, currentGameVersion);

            foreach (var val in Enum.GetValues(typeof(ArrangementType)))
                arrangementTypeCombo.Items.Add(val);

            #region embedded Events
            // this is a giant EH - careful
            arrangementTypeCombo.SelectedValueChanged += (sender, e) =>
            {
                // Selecting defaults
                var selectedType = ((ArrangementType)((ComboBox)sender).SelectedItem);

                switch (selectedType)
                {
                    case ArrangementType.Bass:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Bass);
                        arrangementNameCombo.SelectedItem = ArrangementName.Bass;
                        break;
                    case ArrangementType.Vocal:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Vocals);
                        arrangementNameCombo.Items.Add(ArrangementName.JVocals);
                        arrangementNameCombo.SelectedItem = ArrangementName.Vocals;
                        break;
                    case ArrangementType.ShowLight:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.ShowLights);
                        arrangementNameCombo.SelectedItem = ArrangementName.ShowLights;
                        break;
                    default:
                        arrangementNameCombo.Items.Clear();
                        arrangementNameCombo.Items.Add(ArrangementName.Combo);
                        arrangementNameCombo.Items.Add(ArrangementName.Lead);
                        arrangementNameCombo.Items.Add(ArrangementName.Rhythm);
                        arrangementNameCombo.SelectedItem = ArrangementName.Lead;
                        break;
                }

                var selectedArrangementName = (ArrangementName)arrangementNameCombo.SelectedItem;
                var guitarebass = selectedType != ArrangementType.Vocal && selectedType != ArrangementType.ShowLight;
                // Disabling options that are not meant for Arrangement Types
                // Arrangement Information
                Picked.Enabled = selectedType == ArrangementType.Bass;
                arrangementNameCombo.Enabled = selectedType != ArrangementType.Bass;
                tuningComboBox.Enabled = guitarebass;
                gbScrollSpeed.Enabled = guitarebass;
                gbTuningPitch.Enabled = guitarebass && currentGameVersion != GameVersion.RS2012;
                BonusCheckBox.Enabled = gbTuningPitch.Enabled;
                MetronomeCb.Enabled = gbTuningPitch.Enabled;
                //ltFixCb.Enabled = gbTuningPitch.Enabled;

                // Gameplay Path
                UpdateRouteMaskPath(selectedType, selectedArrangementName);

                // Tone Selector
                gbTone.Enabled = guitarebass;

                // Arrangement ID
                MasterId.Enabled = true;
                PersistentId.Enabled = true;

                // Tuning Edit
                tuningEditButton.Enabled = guitarebass;

                // Vocal/ShowLights Edit
                typeEdit.Enabled = !guitarebass;

                // Update tuningComboBox
                if (guitarebass)
                    FillTuningCombo(selectedType, currentGameVersion);

            }; //END arrangementTypeCombo.SelectedValueChanged

            // this EH may cause serious brain damage
            arrangementNameCombo.SelectedValueChanged += (sender, e) =>
            {
                var selectedType = ((ArrangementType)arrangementTypeCombo.SelectedItem);
                var selectedArrangementName = ((ArrangementName)((ComboBox)sender).SelectedItem);
                UpdateRouteMaskPath(selectedType, selectedArrangementName);
            };

            // this EH may cause serious brain damage
            tuningComboBox.SelectedValueChanged += (sender, e) =>
            {
                // Selecting defaults
                var selectedType = (ArrangementType)arrangementTypeCombo.SelectedItem;
                var selectedTuning = (TuningDefinition)((ComboBox)sender).SelectedItem;
                tuningEditButton.Enabled = (selectedType != ArrangementType.Vocal && selectedType != ArrangementType.ShowLight) && selectedTuning != null;
            };
            #endregion

            parentControl = control;
            SetupTones(arrangement);
            Arrangement = arrangement; // total update by SET action
            EditMode = routeMaskNoneRadio.Checked;
        }


        private void UpdateRouteMaskPath(ArrangementType arrangementType, ArrangementName arrangementName)
        {
            gbGameplayPath.Enabled = (arrangementType != ArrangementType.Vocal && arrangementType != ArrangementType.ShowLight) && currentGameVersion != GameVersion.RS2012;

            //Enabling
            routeMaskLeadRadio.Enabled = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Lead);
            routeMaskRhythmRadio.Enabled = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Rhythm);
            routeMaskBassRadio.Enabled = arrangementType == ArrangementType.Bass;

            //Auto-checking
            routeMaskLeadRadio.Checked = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Lead);
            routeMaskRhythmRadio.Checked = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Rhythm);
            routeMaskBassRadio.Checked = arrangementType == ArrangementType.Bass;
        }

        private void FillTuningCombo(ArrangementType arrangementType, GameVersion gameVersion)
        {
            tuningComboBox.Items.Clear();

            // TODO: figure out logic behind unexpected LINQ behaviors
            // tuningComboBox list is being updated with custom tuning info
            // so refilling the combo-box is required to produce the expected results
            // for now using old fashioned non-LINQ method
            var tuningDefinitions = TuningDefinitionRepository.Instance.LoadTuningDefinitions(gameVersion);
            foreach (var tuning in tuningDefinitions) //also add tuning from songXml used
            {
                // need to populate for Vocals too even though not used
                //if (arrangementType != ArrangementType.Bass)
                tuningComboBox.Items.Add(tuning);
                //if (arrangementType == ArrangementType.Bass)
                // tuningComboBox.Items.Add(TuningDefinition.Convert2Bass(tuning));
            }

            tuningComboBox.SelectedIndex = 0;
            tuningComboBox.Refresh();
        }

        private void ShowTuningForm(ArrangementType selectedType, TuningDefinition tuning)
        {
            if (tuning == null)
            {
                MessageBox.Show("Pick a tuning definition to start editing.\r\n (DEBUG: Current tuning is Null)", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool addNew;
            TuningDefinition formTuning;
            using (var form = new TuningForm())
            {
                form.Tuning = tuning;
                form.IsBass = selectedType == ArrangementType.Bass;

                if (DialogResult.OK != form.ShowDialog())
                    return;

                // prevent any further SET calls to form.Tuning
                formTuning = form.Tuning;
                addNew = form.AddNew;
            }

            if (tuning.UIName != formTuning.UIName)
            {
                // Update LB slots if tuning name is changed
                for (int i = 0; i < parentControl.arrangementLB.Items.Count; i++)
                {
                    var selectedArrangement = (Arrangement)parentControl.arrangementLB.Items[i];

                    if (tuning.UIName.Equals(selectedArrangement.Tuning))
                    {
                        selectedArrangement.Tuning = formTuning.UIName;
                        parentControl.arrangementLB.Items[i] = selectedArrangement;
                    }
                }
            }

            FillTuningCombo(selectedType, currentGameVersion);

            int foundTuning = -1;
            tuningComboBox.SelectedIndex = -1;
            for (int tcbIndex = 0; tcbIndex < tuningComboBox.Items.Count; tcbIndex++)
            {
                tuningComboBox.SelectedIndex = tcbIndex;
                tuning = (TuningDefinition)tuningComboBox.Items[tcbIndex];
                if (tuning.Tuning == formTuning.Tuning)
                {
                    foundTuning = tcbIndex;
                    break;
                }
            }

            // add the custom tuning to tuningComboBox
            if (foundTuning == -1)
            {
                formTuning.Custom = true;
                tuningComboBox.Items.Add(formTuning);
                tuningComboBox.SelectedIndex = tuningComboBox.Items.Count - 1;

                if (addNew)
                    SaveTuningDefinition(formTuning);
            }
            else
                tuningComboBox.SelectedIndex = foundTuning;

            tuningComboBox.Refresh();
            Arrangement.TuningStrings = formTuning.Tuning; // forces SET update
        }

        private bool dirty = false;
        /// <summary>
        /// Update TuningPitch related fields.
        /// </summary>
        /// <param name="type">Set type update, by Hz or cents</param>
        private void UpdateCentOffset(string type = "HZ")
        {
            string value;
            double freq;

            if (type.StartsWith("c") && !dirty)
            {
                value = centOffsetDisplay.Text;
                if (String.IsNullOrEmpty(value)) //im not sure if this useful at all
                    value = "0";

                dirty = true;
                double cent;
                if (Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out cent))
                {
                    freq = cent.Cents2Frequency();
                    //upd self + main
                    centOffsetDisplay.Text = String.Format("{0:0}", cent);
                    frequencyTB.Text = String.Format("{0:0.00}", freq);
                    //upd rest
                    noteDisplay.Text = freq.Frequency2Note();
                    Arrangement.TuningPitch = freq;
                    centOffsetDisplay.BackColor = frequencyTB.BackColor = System.Drawing.SystemColors.Window;
                }
                else
                {
                    centOffsetDisplay.BackColor = System.Drawing.Color.HotPink;
                }
                dirty = false;
            }
            else if (type.StartsWith("HZ") && !dirty)
            {
                value = frequencyTB.Text;
                if (String.IsNullOrEmpty(value))
                    value="440.00";

                dirty = true;
                double hz;
                if (Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out hz) && hz > 0.00)
                {
                    freq = hz;
                    //upd sub
                    centOffsetDisplay.Text = String.Format("{0:0}", hz.Frequency2Cents());
                    //upd rest
                    noteDisplay.Text = freq.Frequency2Note();
                    Arrangement.TuningPitch = freq;
                    centOffsetDisplay.BackColor = frequencyTB.BackColor = System.Drawing.SystemColors.Window;
                }
                else
                {
                    frequencyTB.BackColor = System.Drawing.Color.HotPink;
                }
                dirty = false;
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
            bool isRS2014 = parentControl.CurrentGameVersion != GameVersion.RS2012;

            if (!String.IsNullOrEmpty(arr.ToneBase))
                if (parentControl.tonesLB.Items.Count == 1 && parentControl.tonesLB.Items[0].ToString() == "Default")
                    parentControl.tonesLB.Items.Clear();

            var toneItems = parentControl.tonesLB.Items;
            var toneNames = new List<string>();
            if (isRS2014)
                toneNames.AddRange(parentControl.tonesLB.Items.OfType<Tone2014>().Select(t => t.Name));
            else
                toneNames.AddRange(parentControl.tonesLB.Items.OfType<Tone>().Select(t => t.Name));

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
            FillToneCombo(toneACombo, toneNames, false);
            FillToneCombo(toneBCombo, toneNames, false);
            FillToneCombo(toneCCombo, toneNames, false);
            FillToneCombo(toneDCombo, toneNames, false);

            // SELECTING TONES
            toneBaseCombo.Enabled = true;
            if (!String.IsNullOrEmpty(arr.ToneBase))
                toneBaseCombo.SelectedItem = arr.ToneBase;
            if (!String.IsNullOrEmpty(arr.ToneA))
                toneACombo.SelectedItem = arr.ToneA;
            if (!String.IsNullOrEmpty(arr.ToneB))
                toneBCombo.SelectedItem = arr.ToneB;
            if (!String.IsNullOrEmpty(arr.ToneC))
                toneCCombo.SelectedItem = arr.ToneC;
            if (!String.IsNullOrEmpty(arr.ToneD))
                toneDCombo.SelectedItem = arr.ToneD;

            // If have ToneBase and ToneB is setup it's because auto tone are setup in EoF, so, disable edit to prevent errors.
            disableTonesCheckbox.Checked = (!String.IsNullOrEmpty(arr.ToneBase) && !String.IsNullOrEmpty(arr.ToneB));
            if (disableTonesCheckbox.Checked && !EditMode)
            {
                // disableTonesCheckbox.Enabled = false;
                tt.IsBalloon = true;
                tt.AutoPopDelay = 9000; // tt remains visible if the point is stationary
                tt.InitialDelay = 100; // time that passes before tt appears
                tt.ReshowDelay = 100; // time before next tt window appears
                tt.ShowAlways = true; // force tt display even if parent is not active
                tt.SetToolTip(disableTonesCheckbox, "For Toolkit Expert Use Only");
            }
        }

        private void SequencialToneComboEnabling()
        {//TODO: handle not one-by-one enabling disabling tone slots and use data from enabled one, confused about this one.
            if (currentGameVersion != GameVersion.RS2012)
            {
                toneBCombo.Enabled = !String.IsNullOrEmpty((string)toneACombo.SelectedItem) && toneACombo.SelectedIndex > 0;
                toneCCombo.Enabled = !String.IsNullOrEmpty((string)toneBCombo.SelectedItem) && toneBCombo.SelectedIndex > 0;
                toneDCombo.Enabled = !String.IsNullOrEmpty((string)toneCCombo.SelectedItem) && toneCCombo.SelectedIndex > 0;
            }
            else
            {
                toneACombo.Enabled = toneBCombo.Enabled = toneCCombo.Enabled = toneDCombo.Enabled = false;
            }
        }

        private bool IsAlreadyAdded(string xmlPath)
        {
            for (int i = 0; i < parentControl.arrangementLB.Items.Count; i++)
            {
                var selectedArrangement = (Arrangement)parentControl.arrangementLB.Items[i];

                if (!xmlPath.Equals(selectedArrangement.SongXml.File)) continue;
                if (xmlPath.Equals(Arrangement.SongXml.File)) continue;
                return true;
            }
            return false;
        }

        #region UI events with helpers
        private void songXmlBrowseButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Globals.DefaultProjectDir;
                ofd.Filter = "Rocksmith Song Xml Files (*.xml)|*.xml";
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string xmlFilePath = XmlFilePath.Text = ofd.FileName;
                Globals.DefaultProjectDir = Path.GetDirectoryName(xmlFilePath);
                LoadXmlArrangement(xmlFilePath);
            }
        }

        public bool LoadXmlArrangement(string xmlFilePath)
        {
            if (IsAlreadyAdded(xmlFilePath))
            {
                MessageBox.Show(@"XML Arrangement: " + Path.GetFileName(xmlFilePath) + @"   " + Environment.NewLine +
                    @"has already been added.  Please choose a new file. ",
                    DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                bool isVocal = false;
                bool isShowlight = false;
                try
                {
                    xmlSong = Song2014.LoadFromFile(xmlFilePath);
                    Arrangement.XmlComments = Song2014.ReadXmlComments(xmlFilePath);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.ToLower().Contains("<vocals"))
                        isVocal = true;
                    else if (ex.InnerException.Message.ToLower().Contains("<showlights"))
                        isShowlight = true;
                    else
                    {
                        MessageBox.Show(@"Unable to get information from XML arrangement:  " + Environment.NewLine +
                            Path.GetFileName(xmlFilePath) + Environment.NewLine +
                            @"It may not be a valid arrangement or " + Environment.NewLine +
                            @"your version of the EOF may be out of date." + Environment.NewLine +
                            ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // SETUP FIELDS
                if (isVocal)
                {
                    arrangementTypeCombo.SelectedItem = ArrangementType.Vocal;
                    _arrangement.ArrangementType = ArrangementType.Vocal;
                }
                else if (isShowlight)
                {
                    arrangementTypeCombo.SelectedItem = ArrangementType.ShowLight;
                    _arrangement.ArrangementType = ArrangementType.ShowLight;
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
                    else switch (currentGameVersion)
                    {
                        case GameVersion.RS2012:
                            // add missing XML elements
                            xmlSong.Version = "4";
                            xmlSong.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                            xmlSong.ArrangementProperties = new SongArrangementProperties2014 { StandardTuning = 1 };
                            version = GameVersion.RS2012;
                            break;
                        case GameVersion.None:
                            using (var obj = new Rs1Converter())
                            {
                                xmlSong = null;
                                xmlSong = obj.SongToSong2014(Song.LoadFromFile(XmlFilePath.Text));
                            }
                            currentGameVersion = GameVersion.RS2014;
                            break;
                        default:
                            MessageBox.Show("Your version of EOF may be out of date, please update.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }

                    // TODO: fix error checking logic for new types of conversion
                    if (currentGameVersion != version && version != GameVersion.None)
                    {
                        Debug.WriteLine("Please choose valid Rocksmith {0} arrangement file!", currentGameVersion);
                        //XmlFilePath.Text = "";
                        //return;
                    }

                    // SONG AND ARRANGEMENT INFO / ROUTE MASK
                    BonusCheckBox.Checked = Equals(xmlSong.ArrangementProperties.BonusArr, 1);
                    MetronomeCb.Checked = Equals(xmlSong.ArrangementProperties.Metronome, 2);
                    Arrangement.ArrangementPropeties = xmlSong.ArrangementProperties;
                    Arrangement.CapoFret = xmlSong.Capo;

                    if (!EditMode)
                    {
                        string arr = xmlSong.Arrangement.ToLowerInvariant();
                        if (arr.Contains("guitar") || arr.Contains("lead") || arr.Contains("rhythm") || arr.Contains("combo"))
                        {
                            arrangementTypeCombo.SelectedItem = ArrangementType.Guitar;

                            if (arr.Contains("combo"))
                            {
                                arrangementNameCombo.SelectedItem = ArrangementName.Combo;
                                if (currentGameVersion != GameVersion.RS2012) RouteMask = RouteMask.Lead;
                            }
                            else if (arr.Contains("guitar_22") || arr.Contains("lead") || Equals(xmlSong.ArrangementProperties.PathLead, 1))
                            {
                                arrangementNameCombo.SelectedItem = ArrangementName.Lead;
                                if (currentGameVersion != GameVersion.RS2012) RouteMask = RouteMask.Lead;
                            }
                            else if (arr.Contains("guitar") || arr.Contains("rhythm") || Equals(xmlSong.ArrangementProperties.PathRhythm, 1))
                            {
                                arrangementNameCombo.SelectedItem = ArrangementName.Rhythm;
                                if (currentGameVersion != GameVersion.RS2012) RouteMask = RouteMask.Rhythm;
                            }
                        }
                        else if (arr.Contains("bass"))
                        {
                            arrangementTypeCombo.SelectedItem = ArrangementType.Bass;
                            Picked.Checked = Equals(xmlSong.ArrangementProperties.BassPick, 1);

                            if (currentGameVersion != GameVersion.RS2012)
                            {
                                RouteMask = RouteMask.Bass;
                                //Low tuning fix for bass, If lowest string is B and bass fix not applied TODO:applyonly when 'generate'
                                if (xmlSong.Tuning.String0 < -4 && Arrangement.TuningPitch != 220)
                                    bassFix |= MessageBox.Show(@"The bass tuning may be too low.  Apply Low Bass Tuning Fix?" + Environment.NewLine +
                                                               @"Note: The fix will revert if bass arrangement is re-saved in EOF.  ",
                                                               @"Warning ... Low Bass Tuning", MessageBoxButtons.YesNo) == DialogResult.Yes;
                            }
                        }
                    }

                    if (currentGameVersion != GameVersion.RS2012)
                    {
                        //Tones setup //TODO: add parsing tones events
                        Arrangement.ToneBase = xmlSong.ToneBase;
                        Arrangement.ToneA = xmlSong.ToneA;
                        Arrangement.ToneB = xmlSong.ToneB;
                        Arrangement.ToneC = xmlSong.ToneC;
                        Arrangement.ToneD = xmlSong.ToneD;
                        Arrangement.ToneMultiplayer = null;
                        SetupTones(Arrangement);

                        // Fix Low Bass Tuning
                        if (bassFix)
                        {
                            bassFix = false;
                            Arrangement.SongXml.File = XmlFilePath.Text;

                            if (Arrangement.TuningStrings == null)
                            {
                                // need to load tuning here from the xml arrangement
                                Arrangement.TuningStrings = new TuningStrings();
                                Arrangement.TuningStrings = xmlSong.Tuning;
                            }

                            if (!TuningFrequency.ApplyBassFix(Arrangement))
                                MessageBox.Show("This bass arrangement is already at 220Hz pitch.  ", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                            {
                                var commentsList = Arrangement.XmlComments.ToList();
                                commentsList.Add(new XComment("Low Bass Tuning Fixed"));
                                Arrangement.XmlComments = commentsList;
                            }

                            xmlSong.Tuning = Arrangement.TuningStrings;
                        }
                    }

                    // Setup tuning
                    ArrangementType selectedType;
                    if (xmlSong.Arrangement.ToLower() == "bass")
                        selectedType = ArrangementType.Bass;
                    else
                        selectedType = ArrangementType.Guitar;

                    FillTuningCombo(selectedType, currentGameVersion);

                    // find tuning in tuningComboBox list and make selection
                    int foundTuning = -1;
                    for (int tcbIndex = 0; tcbIndex < tuningComboBox.Items.Count; tcbIndex++)
                    {
                        tuningComboBox.SelectedIndex = tcbIndex;
                        TuningDefinition tuning = (TuningDefinition)tuningComboBox.Items[tcbIndex];
                        if (tuning.Tuning == xmlSong.Tuning)
                        {
                            foundTuning = tcbIndex;
                            break;
                        }
                    }

                    if (foundTuning == -1 && selectedType != ArrangementType.Bass)
                    {
                        tuningComboBox.SelectedIndex = 0;
                        ShowTuningForm(selectedType, new TuningDefinition(xmlSong.Tuning, currentGameVersion)); //FIXME: Don't use this for QuickAdd call
                    }

                    // E Standard, Drop D, and Open E tuning are now the same for both guitar and bass
                    if (foundTuning == -1 && selectedType == ArrangementType.Bass)
                    {
                        tuningComboBox.SelectedIndex = 0;
                        MessageBox.Show("Toolkit was not able to automatically set tuning for" + Environment.NewLine +
                                        "Bass Arrangement: " + Path.GetFileName(xmlFilePath) + Environment.NewLine +
                                        "Use the tuning selector dropdown or Tuning Editor" + Environment.NewLine +
                                        "to customize bass tuning (as defined for six strings).  ", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    tuningComboBox.Refresh();
                    Arrangement.Tuning = tuningComboBox.SelectedItem.ToString();
                    Arrangement.TuningStrings = xmlSong.Tuning;
                    Arrangement.CapoFret = xmlSong.Capo;
                    frequencyTB.Text = Arrangement.TuningPitch.ToString(CultureInfo.InvariantCulture);

                    // bastard bass hack
                    if (Arrangement.Tuning.ToLower().Contains("fixed"))
                        frequencyTB.Text = "220";

                    //UpdateCentOffset();//bad way to update tuning info, IMO

                    // save converted RS1 to RS2014 Song2014 XML
                    if (version == GameVersion.None)
                    {
                        var srcDir = Path.GetDirectoryName(XmlFilePath.Text);
                        var srcName = Path.GetFileNameWithoutExtension(XmlFilePath.Text);
                        var backupSrcPath = String.Format("{0}_{1}.xml", Path.Combine(srcDir, srcName), "RS1");

                        // backup original RS1 file
                        File.Copy(XmlFilePath.Text, backupSrcPath);

                        // write converted RS1 file
                        using (var stream = new FileStream(XmlFilePath.Text, FileMode.Create))
                            xmlSong.Serialize(stream, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Unable to get information from XML arrangement:  " + Environment.NewLine +
                    Path.GetFileName(xmlFilePath) + Environment.NewLine +
                    @"It may not be a valid arrangement or " + Environment.NewLine +
                    @"your version of the EOF may be out of date." + Environment.NewLine +
                    ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            //Validations
            var xmlfilepath = XmlFilePath.Text;
            if (!File.Exists(xmlfilepath))
                if (MessageBox.Show("Xml Arrangement file path is not valid.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                {
                    XmlFilePath.Focus();
                    return;
                }

            if (currentGameVersion != GameVersion.RS2012)
            {
                if (!routeMaskLeadRadio.Checked && !routeMaskRhythmRadio.Checked && !routeMaskBassRadio.Checked && (ArrangementType)arrangementTypeCombo.SelectedItem != ArrangementType.Vocal && (ArrangementType)arrangementTypeCombo.SelectedItem != ArrangementType.ShowLight)
                {
                    if (MessageBox.Show("You did not select a Gameplay Path for this arrangement.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        gbGameplayPath.Focus();
                        return;
                    }
                }
            }

            LoadArrangementData(xmlfilepath);
            Globals.DefaultProjectDir = Path.GetDirectoryName(xmlfilepath);
            DialogResult = DialogResult.OK;
            Close();
        }

        public bool LoadArrangementData(string xmlfilepath)
        {
            //Song XML File
            Arrangement.SongXml.File = xmlfilepath;

            // SONG INFO
            // TODO: get song info from json or hsan file (would be better than from xml)
            if (!ReferenceEquals(xmlSong, null))
            {
                var defaultAuthor = ConfigRepository.Instance()["general_defaultauthor"].Trim();

                if (String.IsNullOrEmpty(parentControl.SongTitle)) parentControl.SongTitle = xmlSong.Title ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.SongTitleSort)) parentControl.SongTitleSort = xmlSong.SongNameSort.GetValidSortableName() ?? parentControl.SongTitle.GetValidSortableName();
                if (String.IsNullOrEmpty(parentControl.AverageTempo)) parentControl.AverageTempo = Math.Round(xmlSong.AverageTempo).ToString() ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.Artist)) parentControl.Artist = xmlSong.ArtistName ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.ArtistSort)) parentControl.ArtistSort = xmlSong.ArtistNameSort.GetValidSortableName() ?? parentControl.Artist.GetValidSortableName();
                if (String.IsNullOrEmpty(parentControl.Album)) parentControl.Album = xmlSong.AlbumName ?? String.Empty;
                if (String.IsNullOrEmpty(parentControl.AlbumYear)) parentControl.AlbumYear = xmlSong.AlbumYear ?? String.Empty;
                // using first three letters of defaultAuthor to make DLCKey unique
                if (String.IsNullOrEmpty(parentControl.DLCKey)) parentControl.DLCKey = String.Format("{0}{1}{2}", 
                   defaultAuthor.Substring(0, Math.Min(3, defaultAuthor.Length)), parentControl.Artist.GetValidAcronym(), parentControl.SongTitle).GetValidKey(parentControl.SongTitle); 

                if (String.IsNullOrEmpty(parentControl.AlbumSort))
                {
                    // use default author for AlbumSort or generate
                    var useDefaultAuthor = ConfigRepository.Instance().GetBoolean("creator_usedefaultauthor");
                    if (useDefaultAuthor) // && currentGameVersion == GameVersion.RS2014)
                        parentControl.AlbumSort = defaultAuthor.GetValidSortableName();
                    else
                        parentControl.AlbumSort = xmlSong.AlbumNameSort.GetValidSortableName() ?? parentControl.Album.GetValidSortableName();
                }
            }

            //Arrangment Information
            Arrangement.Name = (ArrangementName)arrangementNameCombo.SelectedItem;
            Arrangement.ArrangementType = (ArrangementType)arrangementTypeCombo.SelectedItem;
            Arrangement.ScrollSpeed = scrollSpeedTrackBar.Value;
            Arrangement.PluckedType = Picked.Checked ? PluckedType.Picked : PluckedType.NotPicked;
            Arrangement.BonusArr = BonusCheckBox.Checked;
            Arrangement.Metronome = MetronomeCb.Checked ? Metronome.Generate : Metronome.None;

            // Tuning
            TuningDefinition tuning = (TuningDefinition)tuningComboBox.SelectedItem;
            Arrangement.Tuning = tuning.UIName;
            Arrangement.TuningStrings = tuning.Tuning;

            // TODO: Add capo selection to arrangement form
            if (!ReferenceEquals(xmlSong, null))
                Arrangement.CapoFret = xmlSong.Capo;
            UpdateCentOffset();

            //ToneSelector //TODO: add parsing tones events
            Arrangement.ToneBase = toneBaseCombo.SelectedItem.ToString();
            Arrangement.ToneA = (toneACombo.SelectedItem != null) ? toneACombo.SelectedItem.ToString() : ""; //Only need if have more than one tone
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

            return true;
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

        private void frequencyTB_TextChanged(object sender, EventArgs e)
        {
            var bFreq = ((CueTextBox) sender).Name.StartsWith("frequency");
            UpdateCentOffset(bFreq ? "HZ" : "c");
        }

        private void ToneComboEnabled(bool enabled)
        {
            // Not disabling in gbTone to not disable labels
            toneBaseCombo.Enabled = enabled;
            toneACombo.Enabled = enabled;
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

        private void typeEdit_Click(object sender, EventArgs e)
        {
            if (_arrangement.ArrangementType == ArrangementType.Vocal) // (ArrangementType)arrangementTypeCombo.SelectedItem)
                vocalEdit_Click(sender, e);

            else if (_arrangement.ArrangementType == ArrangementType.ShowLight)
                showlightEdit_Click(sender, e);

            else
            {
                //Extra options like personal Audio file. #multitracks
                //guitarEdit_Click(sender, e);
            }
        }

        private void vocalEdit_Click(object sender, EventArgs e)
        {//TODO: wrong behaviour with this warning message
            if (!String.IsNullOrEmpty(parentControl.LyricArtPath) && String.IsNullOrEmpty(Arrangement.FontSng))
                MessageBox.Show("FYI, there is already defined one custom font.\r\n", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            using (var form = new VocalsForm(Arrangement.FontSng, parentControl.LyricArtPath, Arrangement.CustomFont))
            {
                if (DialogResult.OK != form.ShowDialog())
                {
                    return;
                }
                Arrangement.FontSng = form.SngPath;
                parentControl.LyricArtPath = form.ArtPath;
                Arrangement.CustomFont = form.IsCustom;
            }
        }

        private void showlightEdit_Click(object sender, EventArgs e)
        {
            using (var form = new ShowLightsForm(Arrangement.SongFile.File))
            {
                if (DialogResult.OK != form.ShowDialog())
                    return;
                if (!String.IsNullOrEmpty(form.ShowLightsPath))
                    Arrangement.SongXml.File = XmlFilePath.Text = form.ShowLightsPath;
            }
        }

        private void tuningEditButton_Click(object sender, EventArgs e)
        {
            var selectedType = (ArrangementType)arrangementTypeCombo.SelectedItem;
            var tuning = (TuningDefinition)tuningComboBox.SelectedItem;

            ShowTuningForm(selectedType, tuning);
        }

        private void scrollSpeedTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateScrollSpeedDisplay();
        }

        private void UpdateScrollSpeedDisplay()
        {
            scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeedTrackBar.Value) / 10);
        }

        private void SaveTuningDefinition(TuningDefinition formTuning)
        {
            // can mess up the TuningDefinition.xml file on multiple adds
            TuningDefinitionRepository.Instance.Add(formTuning, true);
            TuningDefinitionRepository.Instance.Save(true);
        }

        #endregion

        private void ArrangementForm_Load(object sender, EventArgs e)
        {
            // disallow changing XML file name when in edit mode
            if (EditMode)
            {
                songXmlBrowseButton.Enabled = false;
                XmlFilePath.ReadOnly = true;
            }
        }

    }
}

