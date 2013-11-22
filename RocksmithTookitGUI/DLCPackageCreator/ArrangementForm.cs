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
                ArrangementType = ArrangementType.Guitar,
                ScrollSpeed = 20
            }, toneNames, control, gameVersion)
        {
        }

        public ArrangementForm(Arrangement arrangement, IEnumerable<string> toneNames, DLCPackageCreator control, GameVersion gameVersion)
        {
            InitializeComponent();
            currentGameVersion = gameVersion;
            FillTuningCombo();

            foreach (var val in Enum.GetValues(typeof(ArrangementType)))
            {
                arrangementTypeCombo.Items.Add(val);
            }
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
                gbTuningPitch.Visible = currentGameVersion == GameVersion.RS2014;
                gbScrollSpeed.Enabled = selectedType != ArrangementType.Vocal;
                Picked.Visible = selectedType == ArrangementType.Bass;
                Picked.Checked = selectedType == ArrangementType.Bass ? false : true;
                UpdateCentOffset();

                // Gameplay Path
                UpdateRouteMaskPath(selectedType, selectedArrangementName);

                // Tone Selector
                gbTone.Enabled = selectedType != ArrangementType.Vocal;
                toneMultiplayerCombo.Enabled = currentGameVersion == GameVersion.RS2014;
                toneACombo.Enabled = currentGameVersion == GameVersion.RS2014;
                toneBCombo.Enabled = currentGameVersion == GameVersion.RS2014;
                toneCCombo.Enabled = currentGameVersion == GameVersion.RS2014;
                toneDCombo.Enabled = currentGameVersion == GameVersion.RS2014;

                SequencialToneComboEnabling();

                // Arrangement ID
                MasterId.Enabled = selectedType != ArrangementType.Vocal;
                PersistentId.Enabled = selectedType != ArrangementType.Vocal;
            };

            arrangementNameCombo.SelectedValueChanged += (sender, e) =>
            {
                var selectedType = ((ArrangementType)((ComboBox)arrangementTypeCombo).SelectedItem);
                var selectedArrangementName = ((ArrangementName)((ComboBox)sender).SelectedItem);

                UpdateRouteMaskPath(selectedType, selectedArrangementName);
            };

            FillToneCombo(toneBaseCombo, toneNames, true);
            FillToneCombo(toneMultiplayerCombo, toneNames, false);
            FillToneCombo(toneACombo, toneNames, false);
            FillToneCombo(toneBCombo, toneNames, false);
            FillToneCombo(toneCCombo, toneNames, false);
            FillToneCombo(toneDCombo, toneNames, false);

            scrollSpeedTrackBar.Scroll += (sender, e) =>
            {
                scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeedTrackBar.Value) / 10);
            };

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
            toneBCombo.Enabled = toneACombo.SelectedIndex > 0;
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
                int scrollSpeed = Math.Min(scrollSpeedTrackBar.Maximum, Math.Max(scrollSpeedTrackBar.Minimum, arrangement.ScrollSpeed));
                scrollSpeedTrackBar.Value = scrollSpeed;
                scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeed) / 10);
                Picked.Checked = arrangement.PluckedType == PluckedType.Picked;
                RouteMask = arrangement.RouteMask;
                //Tone Selector
                toneBaseCombo.SelectedItem = arrangement.ToneBase;
                if (toneBaseCombo.SelectedItem == null && toneBaseCombo.Items.Count > 0)
                    toneBaseCombo.SelectedItem = toneBaseCombo.Items[0];
                toneMultiplayerCombo.SelectedItem = arrangement.ToneMultiplayer;
                toneACombo.SelectedItem = arrangement.ToneA;
                toneBCombo.SelectedItem = arrangement.ToneB;
                toneCCombo.SelectedItem = arrangement.ToneC;
                toneDCombo.SelectedItem = arrangement.ToneD;
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
                //Read XML chart info (generated by EoF) and try to pre-input some fields
                var doc = XDocument.Load(XmlFilePath.Text);

                bool isVocal = doc.XPathSelectElement("/vocals") != null;

                if (isVocal) {
                    arrangementTypeCombo.SelectedItem = ArrangementType.Vocal;
                } else {
                    Form parentForm = (this.Parent as Form);

                    if (String.IsNullOrEmpty(parentControl.SongTitle)) parentControl.SongTitle = doc.XPathSelectElement("/song/title") != null ? doc.XPathSelectElement("/song/title").Value : String.Empty;
                    if (String.IsNullOrEmpty(parentControl.SongTitleSort)) parentControl.SongTitleSort = parentControl.SongTitle;
                    if (String.IsNullOrEmpty(parentControl.DLCName)) parentControl.DLCName = parentControl.SongTitleSort;
                    if (String.IsNullOrEmpty(parentControl.AverageTempo)) parentControl.AverageTempo = doc.XPathSelectElement("/song/averageTempo") != null ? doc.XPathSelectElement("/song/averageTempo").Value : String.Empty;
                    if (String.IsNullOrEmpty(parentControl.Artist)) parentControl.Artist = doc.XPathSelectElement("/song/artistName") != null ? doc.XPathSelectElement("/song/artistName").Value : String.Empty;
                    if (String.IsNullOrEmpty(parentControl.ArtistSort)) parentControl.ArtistSort = parentControl.Artist;
                    if (String.IsNullOrEmpty(parentControl.Album)) parentControl.Album = doc.XPathSelectElement("/song/albumName") != null ? doc.XPathSelectElement("/song/albumName").Value : String.Empty;
                    if (String.IsNullOrEmpty(parentControl.AlbumYear)) parentControl.AlbumYear = doc.XPathSelectElement("/song/albumYear") != null ? doc.XPathSelectElement("/song/albumYear").Value : String.Empty;

                    string arr = doc.XPathSelectElement("/song/arrangement").Value;
                    if (arr.ToLower().IndexOf("guitar") > -1 || arr.ToLower().IndexOf("lead") > -1 || arr.ToLower().IndexOf("rhythm") > -1 || arr.ToLower().IndexOf("combo") > -1)
                    {
                        arrangementTypeCombo.SelectedItem = ArrangementType.Guitar;
                        if (arr.ToLower().IndexOf("guitar 22") > -1 || arr.ToLower().IndexOf("rhythm") > -1)
                            arrangementNameCombo.SelectedItem = ArrangementName.Rhythm;
                    }
                    if (arr.ToLower().IndexOf("bass") > -1)
                        arrangementTypeCombo.SelectedItem = ArrangementType.Bass;

                    XElement arrangementProperties = doc.XPathSelectElement("/song/arrangementProperties");
                    if (arrangementProperties != null && arrangementProperties.Attributes() != null && arrangementProperties.Attributes().Count() > 25) {
                        Picked.Checked = Convert.ToBoolean(Convert.ToInt16(arrangementProperties.Attribute("bassPick").Value));

                        bool standardTuning = Convert.ToBoolean(Convert.ToInt16(arrangementProperties.Attribute("standardTuning").Value));
                        if (!standardTuning) {
                            XElement tuning = doc.XPathSelectElement("/song/tuning");
                            int[] strings = {
                                            Convert.ToInt32(tuning.Attribute("string0").Value),
                                            Convert.ToInt32(tuning.Attribute("string1").Value),
                                            Convert.ToInt32(tuning.Attribute("string2").Value),
                                            Convert.ToInt32(tuning.Attribute("string3").Value),
                                            Convert.ToInt32(tuning.Attribute("string4").Value),
                                            Convert.ToInt32(tuning.Attribute("string5").Value)
                                        };
                            //tuningComboBox.SelectedItem = InstrumentTuningExtensions.GetTuningByOffsets(strings);
                            tuningComboBox.SelectedItem = TuningDefinitionRepository.Instance().Select(strings, currentGameVersion);
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Unable to get information from the arrangement XML. \r\nYour version of the EoF is up to date? \r\n" + ex.Message, DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (!routeMaskLeadRadio.Checked && !routeMaskRhythmRadio.Checked && !routeMaskBassRadio.Checked && (ArrangementType)arrangementTypeCombo.SelectedItem != ArrangementType.Vocal)
            {
                if (MessageBox.Show("You not selected a Gameplay Path, this arrangement you show only in song list.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
                {
                    gbGameplayPath.Focus();
                    return;
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

            //ToneSelector
            arrangement.ToneBase = toneBaseCombo.SelectedItem.ToString();
            arrangement.ToneMultiplayer = (toneMultiplayerCombo.SelectedItem != null) ? toneMultiplayerCombo.SelectedItem.ToString() : "";
            arrangement.ToneA = (toneACombo.SelectedItem != null) ? toneACombo.SelectedItem.ToString() : "";
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
                    centOffsetDisplay.Text = TuningFrequency.Frequency2Note(freq, out noteName).ToString();
                    noteDisplay.Text = noteName;
                }
            }
        }
    }
}
