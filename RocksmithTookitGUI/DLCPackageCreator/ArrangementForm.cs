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
        private Platform.GameVersion selectedGameVersion;

        public ArrangementForm(IEnumerable<string> toneNames, DLCPackageCreator control, Platform.GameVersion gameVersion)
            : this(new Arrangement
            {
                SongFile = new SongFile { File = "" },
                SongXml = new SongXML { File = "" },
                ArrangementType = ArrangementType.Guitar,
                RelativeDifficulty = 1,
                ScrollSpeed = 20
            }, toneNames, control, gameVersion)
        {
        }

        public ArrangementForm(Arrangement arrangement, IEnumerable<string> toneNames, DLCPackageCreator control, Platform.GameVersion gameVersion)
        {
            InitializeComponent();
            selectedGameVersion = gameVersion;
            
            foreach (var val in Enum.GetValues(typeof(InstrumentTuning))) {
                tuningComboBox.Items.Add(val);
            }

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
                gbScrollSpeed.Enabled = selectedType != ArrangementType.Vocal;
                RelativeDifficulty.Enabled = selectedType != ArrangementType.Vocal;
                Picked.Visible = selectedType == ArrangementType.Bass;
                Picked.Checked = selectedType == ArrangementType.Bass ? false : true;

                // Gameplay Path
                gbGameplayPath.Enabled = selectedType != ArrangementType.Vocal && selectedGameVersion == Platform.GameVersion.RS2014;
                pathLeadCheckbox.Enabled = selectedType == ArrangementType.Guitar;
                pathLeadCheckbox.Checked = selectedType == ArrangementType.Guitar;
                pathRhythmCheckbox.Enabled = selectedType == ArrangementType.Guitar;
                pathRhythmCheckbox.Checked = selectedType == ArrangementType.Guitar && selectedArrangementName == ArrangementName.Rhythm;
                pathBassCheckbox.Enabled = selectedType == ArrangementType.Bass;
                pathBassCheckbox.Checked = selectedType == ArrangementType.Bass;

                // Tone Selector
                gbTone.Enabled = selectedType != ArrangementType.Vocal;
                toneMultiplayerCombo.Enabled = selectedGameVersion == Platform.GameVersion.RS2014;
                toneACombo.Enabled = selectedGameVersion == Platform.GameVersion.RS2014;
                toneBCombo.Enabled = selectedGameVersion == Platform.GameVersion.RS2014;
                toneCCombo.Enabled = selectedGameVersion == Platform.GameVersion.RS2014;
                toneDCombo.Enabled = selectedGameVersion == Platform.GameVersion.RS2014;

                // DLC ID
                MasterId.Enabled = selectedType != ArrangementType.Vocal;
                PersistentId.Enabled = selectedType != ArrangementType.Vocal;
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

        private void FillToneCombo(ComboBox combo, IEnumerable<string> toneNames, bool isBase)
        {
            combo.Items.Clear();
            if (!isBase) combo.Items.Add("");
            foreach (var tone in toneNames)
            {
                combo.Items.Add(tone);
            }
            combo.SelectedIndex = 0;
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
                InstrumentTuning tuning = InstrumentTuning.Standard;
                Enum.TryParse<InstrumentTuning>(arrangement.Tuning, true, out tuning);
                tuningComboBox.SelectedItem = tuning;
                int scrollSpeed = Math.Min(scrollSpeedTrackBar.Maximum, Math.Max(scrollSpeedTrackBar.Minimum, arrangement.ScrollSpeed));
                scrollSpeedTrackBar.Value = scrollSpeed;
                scrollSpeedDisplay.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)scrollSpeed) / 10);
                RelativeDifficulty.Text = arrangement.RelativeDifficulty.ToString();
                Picked.Checked = arrangement.PluckedType == PluckedType.Picked;
                //Gameplay Path
                pathLeadCheckbox.Checked = arrangement.PathLead;
                pathRhythmCheckbox.Checked = arrangement.PathRhythm;
                pathBassCheckbox.Checked = arrangement.PathBass;
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
                            tuningComboBox.SelectedItem = InstrumentTuningExtensions.GetTuningByOffsets(strings);
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Unable to get information from the arrangement XML. \r\nYour version of the EoF is up to date? \r\n" + ex.Message, "DLC Package Creator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            var relDificulty = RelativeDifficulty.Text.ToInt32();
            if (relDificulty == -1)
            {
                RelativeDifficulty.Focus();
                return;
            }
            if (!pathLeadCheckbox.Checked && !pathRhythmCheckbox.Checked && !pathBassCheckbox.Checked && (ArrangementType)arrangementTypeCombo.SelectedItem != ArrangementType.Vocal)
            {

            }
            
            //Song XML File
            arrangement.SongXml.File = xmlfilepath;
            
            //Arrangment Information
            arrangement.Name = (ArrangementName)arrangementNameCombo.SelectedItem;
            arrangement.ArrangementType = (ArrangementType)arrangementTypeCombo.SelectedItem;
            arrangement.Tuning = tuningComboBox.SelectedItem.ToString();
            arrangement.ScrollSpeed = scrollSpeedTrackBar.Value;
            arrangement.RelativeDifficulty = relDificulty;
            arrangement.PluckedType = Picked.Checked ? PluckedType.Picked : PluckedType.NotPicked;

            //ToneSelector
            arrangement.ToneBase = toneBaseCombo.SelectedItem.ToString();
            arrangement.ToneMultiplayer = (toneMultiplayerCombo.SelectedItem != null) ? toneMultiplayerCombo.SelectedItem.ToString() : "";
            arrangement.ToneA = (toneACombo.SelectedItem != null) ? toneACombo.SelectedItem.ToString() : "";
            arrangement.ToneB = (toneBCombo.SelectedItem != null) ? toneBCombo.SelectedItem.ToString() : "";
            arrangement.ToneC = (toneCCombo.SelectedItem != null) ? toneCCombo.SelectedItem.ToString() : "";
            arrangement.ToneD = (toneDCombo.SelectedItem != null) ? toneDCombo.SelectedItem.ToString() : "";
            
            //Gameplay Path
            arrangement.PathLead = pathLeadCheckbox.Checked;
            arrangement.PathRhythm = pathRhythmCheckbox.Checked;
            arrangement.PathBass = pathBassCheckbox.Checked;
            
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
    }
}
