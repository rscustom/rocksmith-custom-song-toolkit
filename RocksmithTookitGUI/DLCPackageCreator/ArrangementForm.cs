﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using RocksmithToolkitGUI.Config;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Conversion;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.XmlRepository;
using System.Drawing;
using System.Xml;

// do most work with the arrangment as memory variable

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ArrangementForm : Form
    {
        public bool EditMode;
        private Arrangement _arrangement;
        private bool _dirty;
        private bool _fixLowBass;
        private GameVersion _gameVersion;
        private DLCPackageCreator _parentControl;
        private ToolTip _toolTip = new ToolTip();
        private Song2014 _song2014;

        public ArrangementForm(DLCPackageCreator control, GameVersion gameVersion)
            : this(new Arrangement
            {
                SongFile = new SongFile { File = "" },
                SongXml = new SongXML { File = "" },
                ArrangementType = ArrangementType.Guitar
            }, control, gameVersion)
        {
            EditMode = false;
            Console.WriteLine("Debug");
        }

        public ArrangementForm(Arrangement arrangement, DLCPackageCreator control, GameVersion gameVersion) // , string projectDir = "")
        {
            InitializeComponent();

            _gameVersion = gameVersion == GameVersion.RS2012 ? GameVersion.RS2012 : GameVersion.RS2014;
            _parentControl = control;
            _fixLowBass = ConfigRepository.Instance().GetBoolean("creator_fixlowbass");

            foreach (var val in Enum.GetValues(typeof(ArrangementType)))
                cmbArrangementType.Items.Add(val);

            #region Event Handlers

            // this is a giant freak'n EH - CAREFUL
            cmbArrangementType.SelectedValueChanged += (sender, e) =>
                {
                    // Selecting defaults
                    var selectedType = ((ArrangementType)((ComboBox)sender).SelectedItem);

                    switch (selectedType)
                    {
                        case ArrangementType.Bass:
                            cmbArrangementName.Items.Clear();
                            cmbArrangementName.Items.Add(ArrangementName.Bass);
                            cmbArrangementName.SelectedItem = ArrangementName.Bass;
                            break;
                        case ArrangementType.Vocal:
                            cmbArrangementName.Items.Clear();
                            cmbArrangementName.Items.Add(ArrangementName.Vocals);
                            cmbArrangementName.Items.Add(ArrangementName.JVocals);
                            cmbArrangementName.SelectedItem = ArrangementName.Vocals;
                            break;
                        case ArrangementType.ShowLight:
                            cmbArrangementName.Items.Clear();
                            cmbArrangementName.Items.Add(ArrangementName.ShowLights);
                            cmbArrangementName.SelectedItem = ArrangementName.ShowLights;
                            break;
                        default:
                            cmbArrangementName.Items.Clear();
                            cmbArrangementName.Items.Add(ArrangementName.Combo);
                            cmbArrangementName.Items.Add(ArrangementName.Lead);
                            cmbArrangementName.Items.Add(ArrangementName.Rhythm);
                            cmbArrangementName.SelectedItem = ArrangementName.Lead;
                            break;
                    }

                    var selectedArrangementName = (ArrangementName)cmbArrangementName.SelectedItem;
                    var guitarebass = selectedType != ArrangementType.Vocal && selectedType != ArrangementType.ShowLight;
                    // Disabling options that are not meant for Arrangement Types
                    // Arrangement Information
                    chkBassPicked.Enabled = selectedType == ArrangementType.Bass;
                    cmbArrangementName.Enabled = selectedType != ArrangementType.Bass;
                    cmbTuningName.Enabled = guitarebass;
                    gbScrollSpeed.Enabled = guitarebass;
                    gbTuningPitch.Enabled = guitarebass && _gameVersion != GameVersion.RS2012;
                    pnlArrangementRepresentative.Enabled = guitarebass;
                    rbArrangementBonus.Enabled = _gameVersion != GameVersion.RS2012;
                    chkMetronome.Enabled = gbTuningPitch.Enabled;

                    // Gameplay Path
                    UpdateRouteMaskPath(selectedType, selectedArrangementName);

                    // Tone Selector
                    gbTone.Enabled = guitarebass;

                    // Arrangement ID
                    txtMasterId.Enabled = true;
                    txtPersistentId.Enabled = true;

                    // Tuning Edit
                    btnEditTuning.Enabled = guitarebass;

                    // Vocal/ShowLights Edit
                    btnEditType.Enabled = !guitarebass;

                    // Update tuningComboBox
                    FillTuningCombo(_gameVersion);

                }; //END EH arrangementTypeCombo.SelectedValueChanged

            // this EH may cause serious brain damage
            cmbArrangementName.SelectedValueChanged += (sender, e) =>
                {
                    var selectedType = ((ArrangementType)cmbArrangementType.SelectedItem);
                    var selectedArrangementName = ((ArrangementName)((ComboBox)sender).SelectedItem);
                    UpdateRouteMaskPath(selectedType, selectedArrangementName);
                };

            // this EH may cause serious brain damage
            cmbTuningName.SelectedValueChanged += cmbTuningName_SelectedValueChanged;

            #endregion

            SetupTones(arrangement);
            Arrangement = arrangement; // update form using SET action
        }

        public string XmlPath
        {
            get { return txtXmlPath.Text; }
            set { txtXmlPath.Text = value; }
        }

        // get/set data for JSON arrangement
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
                XmlPath = value.SongXml.File;

                //Arrangement Information
                cmbArrangementType.SelectedItem = value.ArrangementType;
                cmbArrangementName.SelectedItem = value.ArrangementName;
                if (!String.IsNullOrEmpty(value.Tuning))
                    cmbTuningName.SelectedIndex = cmbTuningName.FindStringExact(value.Tuning);

                txtFrequency.Text = (value.TuningPitch > 0) ? value.TuningPitch.ToString() : "440.00";

                //Update it only here
                var scrollSpeed = value.ScrollSpeed;
                if (scrollSpeed == 0)
                    scrollSpeed = Convert.ToInt32(ConfigRepository.Instance().GetDecimal("creator_scrollspeed") * 10);
                
                tbarScrollSpeed.Value = Math.Min(scrollSpeed, tbarScrollSpeed.Maximum);
                UpdateScrollSpeedDisplay();

                if (Arrangement.RouteMask == RouteMask.Bass)
                    chkBassPicked.Checked = value.PluckedType == PluckedType.Picked;

                // TODO: monitor this change
                // Default => represent is true and bonus is false 
                // Bonus => represent is false and bonus is true     
                // Alternate => both represent and bonus are false
                // Unknown => both represent and bonus are true
                if (value.Represent && value.BonusArr)
                {
                    var diaMsg = "Invalid Arrangement Default/Bonus/Alternate Conditon ...  " + Environment.NewLine + Environment.NewLine +
                                 "Toolkit will reset the arrangement to" + Environment.NewLine +
                                 "the default represent condition." + Environment.NewLine;
                    BetterDialog2.ShowDialog(diaMsg, "<WARNING> Arrangement Represent", null, null, "OK", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning", 150, 150);
                    value.Represent = true;
                    value.BonusArr = false;
                }

                rbArrangementDefault.Checked = value.Represent;
                rbArrangementBonus.Checked = value.BonusArr;
                rbArrangementAlternate.Checked = !value.BonusArr && !value.Represent ? true : false;

                var debugMe = "StopHere";

                chkMetronome.Checked = value.Metronome == Metronome.Generate;
                RouteMask = value.RouteMask;

                //DLC ID
                txtPersistentId.Text = value.Id.ToString().Replace("-", "").ToUpper();
                txtMasterId.Text = value.MasterId.ToString();

                // TODO: confirm XML gets updated after this somewhere
            }
        }

        private RouteMask RouteMask
        {
            get
            {
                if (rbRouteMaskLead.Checked)
                    return RouteMask.Lead;
                if (rbRouteMaskRhythm.Checked)
                    return RouteMask.Rhythm;
                if (rbRouteMaskBass.Checked)
                    return RouteMask.Bass;
                return RouteMask.None;
            }
            set
            {
                switch (value)
                {
                    case RouteMask.Lead:
                        rbRouteMaskLead.Checked = true;
                        break;
                    case RouteMask.Rhythm:
                        rbRouteMaskRhythm.Checked = true;
                        break;
                    case RouteMask.Bass:
                        rbRouteMaskBass.Checked = true;
                        break;
                    default:
                        rbRouteMaskNone.Checked = true;
                        break;
                }
            }
        }

        public void LoadArrangementData(string xmlFilePath)
        {
            //Song XML File
            Arrangement.SongXml.File = xmlFilePath;
            Arrangement.XmlComments = Song2014.ReadXmlComments(xmlFilePath);

            // Song Info
            if (!ReferenceEquals(_song2014, null))
            {
                var defaultAuthor = ConfigRepository.Instance()["general_defaultauthor"].Trim();

                if (String.IsNullOrEmpty(_parentControl.SongTitle)) _parentControl.SongTitle = _song2014.Title.GetValidAtaSpaceName();
                if (String.IsNullOrEmpty(_parentControl.SongTitleSort)) _parentControl.SongTitleSort = _song2014.SongNameSort.GetValidSortableName();
                if (String.IsNullOrEmpty(_parentControl.AverageTempo)) _parentControl.AverageTempo = _song2014.AverageTempo.ToString().GetValidTempo();
                if (String.IsNullOrEmpty(_parentControl.Artist)) _parentControl.Artist = _song2014.ArtistName.GetValidAtaSpaceName();
                if (String.IsNullOrEmpty(_parentControl.ArtistSort)) _parentControl.ArtistSort = _song2014.ArtistNameSort.GetValidSortableName();
                if (String.IsNullOrEmpty(_parentControl.Album)) _parentControl.Album = _song2014.AlbumName.GetValidAtaSpaceName();
                if (String.IsNullOrEmpty(_parentControl.AlbumYear)) _parentControl.AlbumYear = _song2014.AlbumYear.GetValidYear();
                // using first three letters of defaultAuthor to make DLCKey unique
                if (String.IsNullOrEmpty(_parentControl.DLCKey)) _parentControl.DLCKey = String.Format("{0}{1}{2}",
                    defaultAuthor.Substring(0, Math.Min(3, defaultAuthor.Length)), _parentControl.Artist.GetValidAcronym(), _parentControl.SongTitle).GetValidKey(_parentControl.SongTitle);

                if (String.IsNullOrEmpty(_parentControl.AlbumSort))
                {
                    // use default author for AlbumSort or generate
                    var useDefaultAuthor = ConfigRepository.Instance().GetBoolean("creator_usedefaultauthor");
                    if (useDefaultAuthor)
                        _parentControl.AlbumSort = defaultAuthor.GetValidSortableName();
                    else
                        _parentControl.AlbumSort = _song2014.AlbumNameSort.GetValidSortableName();
                }
            }

            // Arrangment Information
            Arrangement.ArrangementName = (ArrangementName)cmbArrangementName.SelectedItem;
            Arrangement.ArrangementType = (ArrangementType)cmbArrangementType.SelectedItem;
            Arrangement.ScrollSpeed = tbarScrollSpeed.Value;
            Arrangement.PluckedType = chkBassPicked.Checked ? PluckedType.Picked : PluckedType.NotPicked;
            Arrangement.Represent = rbArrangementDefault.Checked;
            Arrangement.BonusArr = rbArrangementBonus.Checked;
            if (rbArrangementAlternate.Checked)
            {
                Arrangement.Represent = false;
                Arrangement.BonusArr = false;
            }

            Arrangement.Metronome = chkMetronome.Checked ? Metronome.Generate : Metronome.None;

            // Tuning
            TuningDefinition tuning = (TuningDefinition)cmbTuningName.SelectedItem;
            Arrangement.Tuning = tuning.UIName;
            Arrangement.TuningStrings = tuning.Tuning;

            // TODO: Add capo selection to arrangement form
            if (!ReferenceEquals(_song2014, null))
                Arrangement.CapoFret = _song2014.Capo;

            UpdateCentOffset();

            try
            {
                // ToneSelector //TODO: add parsing tones events
                Arrangement.ToneBase = cmbToneBase.SelectedItem.ToString();
                Arrangement.ToneA = (cmbToneA.SelectedItem != null) ? cmbToneA.SelectedItem.ToString() : ""; //Only need if have more than one tone
                Arrangement.ToneB = (cmbToneB.SelectedItem != null) ? cmbToneB.SelectedItem.ToString() : "";
                Arrangement.ToneC = (cmbToneC.SelectedItem != null) ? cmbToneC.SelectedItem.ToString() : "";
                Arrangement.ToneD = (cmbToneD.SelectedItem != null) ? cmbToneD.SelectedItem.ToString() : "";
            }
            catch (Exception ex)
            {
                // catch RS1 and do nothing 
                Debug.WriteLine("LoadArrangementData: " + ex.Message);
            }

            // Gameplay Path
            Arrangement.RouteMask = RouteMask;

            // Xml data cleanup
            _song2014 = null;

            // DLC IDs
            Guid guid;
            if (Guid.TryParse(txtPersistentId.Text, out guid) == false)
                txtPersistentId.Focus();
            else
                Arrangement.Id = guid;

            int masterId;
            if (int.TryParse(txtMasterId.Text, out masterId) == false)
                txtMasterId.Focus();
            else
                Arrangement.MasterId = masterId;
        }

        private static ArrangementType DetectArrangementType(string filename)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                reader.MoveToContent();
                if (reader.LocalName == "vocals")
                    return ArrangementType.Vocal;
                if (reader.LocalName == "showlights")
                    return ArrangementType.ShowLight;

                // Return guitar for everything else
                return ArrangementType.Guitar;
            }
        }

        public bool AddXmlArrangement(string xmlFilePath)
        {
            // only use this method when adding new arrangements
            if (EditMode)
                return false;

            Arrangement = new Arrangement
            {
                SongFile = new SongFile { File = "" },
                SongXml = new SongXML { File = xmlFilePath }
            };

            var arrType = DetectArrangementType(xmlFilePath);

            // SETUP FIELDS
            if (arrType == ArrangementType.Vocal)
            {
                cmbArrangementType.SelectedItem = ArrangementType.Vocal;
                Arrangement.ArrangementType = ArrangementType.Vocal;

                // Attempt to auto-detect Japanese vocals from filename
                if (xmlFilePath.IndexOf("jvocal", StringComparison.OrdinalIgnoreCase) >= 0
                   || xmlFilePath.IndexOf("jlyric", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    cmbArrangementName.SelectedItem = ArrangementName.JVocals;

                    // Try to find a custom font texture with the filename lyrics.dds
                    string fontTexture = Path.Combine(Path.GetDirectoryName(xmlFilePath), "lyrics.dds");
                    if (File.Exists(fontTexture))
                    {
                        Arrangement.LyricArt = fontTexture;
                    }
                }
                else
                {
                    cmbArrangementName.SelectedItem = ArrangementName.Vocals;
                }
            }
            else if (arrType == ArrangementType.ShowLight)
            {
                cmbArrangementType.SelectedItem = ArrangementType.ShowLight;
                Arrangement.ArrangementType = ArrangementType.ShowLight;
            }
            else // add Instrument Arrangement
            {
                _song2014 = Song2014.LoadFromFile(xmlFilePath);
                if (_song2014 == null)
                    return false;

                // Detect Arrangement XML Version
                var xmlVersion = GameVersion.None;
                var verAttrib = Convert.ToInt32(_song2014.Version);
                if (verAttrib < 7) // RS1 format
                {
                    xmlVersion = GameVersion.RS2012;
                }
                else if (verAttrib > 6) // RS2014 format 
                {
                    // CAUTION newer RS1 use RS2014 XML Arrangement format
                    xmlVersion = GameVersion.RS2014;
                }
                else
                    throw new DataException("<ERROR> Unknown Arrangement XML Version: " + verAttrib + Environment.NewLine);

                // Check tuning before converting
                var hasTuning = _song2014.Tuning == null ? false : true;

                // Convert RS1 to RS2014 XML Arrangement
                if (_gameVersion == GameVersion.None && xmlVersion == GameVersion.RS2012)
                {
                    using (var obj = new Rs1Converter())
                    {
                        _song2014 = null;
                        _song2014 = obj.SongToSong2014(Song.LoadFromFile(xmlFilePath));
                    }

                    if (!hasTuning)
                    {
                        MessageBox.Show("<Warning> Default E Standard tuning was selected for" + Environment.NewLine +
                                        "this RS1 arrangement.  Please confirm the arrangement  " + Environment.NewLine +
                                        "tuning and all other data before leaving the editor.",
                                        "Default Tuning Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // SONG AND ARRANGEMENT INFO / ROUTE MASK
                var arr = _song2014.Arrangement.ToLowerInvariant();

                // TODO: monitor this code
                if (arr.Contains("guitar") || arr.Contains("lead") || arr.Contains("rhythm") || arr.Contains("combo"))
                {
                    cmbArrangementType.SelectedItem = ArrangementType.Guitar;
                    Arrangement.ArrangementType = ArrangementType.Guitar;

                    if (arr.Contains("combo"))
                    {
                        cmbArrangementName.SelectedItem = ArrangementName.Combo;
                        if (_gameVersion != GameVersion.RS2012)
                            RouteMask = RouteMask.Lead;
                    }
                    else if (arr.Contains("guitar_22") || arr.Contains("lead") || Equals(_song2014.ArrangementProperties.PathLead, 1))
                    {
                        cmbArrangementName.SelectedItem = ArrangementName.Lead;
                        if (_gameVersion != GameVersion.RS2012)
                            RouteMask = RouteMask.Lead;
                    }
                    else if (arr.Contains("guitar") || arr.Contains("rhythm") || Equals(_song2014.ArrangementProperties.PathRhythm, 1))
                    {
                        cmbArrangementName.SelectedItem = ArrangementName.Rhythm;
                        if (_gameVersion != GameVersion.RS2012)
                            RouteMask = RouteMask.Rhythm;
                    }
                }
                else if (arr.Contains("bass"))
                {
                    cmbArrangementType.SelectedItem = ArrangementType.Bass;
                    Arrangement.ArrangementType = ArrangementType.Bass;
                    chkBassPicked.Checked = Equals(_song2014.ArrangementProperties.BassPick, 1);
                    if (_gameVersion != GameVersion.RS2012)
                        RouteMask = RouteMask.Bass;
                }

                Arrangement.ArrangementPropeties = _song2014.ArrangementProperties != null ? _song2014.ArrangementProperties : new SongArrangementProperties2014();
                rbArrangementDefault.Checked = true; // set initial default

                // old RS1 and/or EOF Arrangement XML may fail here on loading
                try
                {
                    rbArrangementDefault.Checked = Equals(_song2014.ArrangementProperties.Represent, 1);
                    rbArrangementBonus.Checked = Equals(_song2014.ArrangementProperties.BonusArr, 1);
                    if (!rbArrangementDefault.Checked && !rbArrangementBonus.Checked)
                        rbArrangementAlternate.Checked = true;

                    chkMetronome.Checked = Equals(_song2014.ArrangementProperties.Metronome, 2);
                    Arrangement.CapoFret = _song2014.Capo;
                    Arrangement.TuningStrings = _song2014.Tuning;

                    if (!String.IsNullOrEmpty(_song2014.CentOffset))
                        Arrangement.TuningPitch = Convert.ToDouble(_song2014.CentOffset).Cents2Frequency();

                    txtFrequency.Text = (Arrangement.TuningPitch > 0) ? Arrangement.TuningPitch.ToString() : "440.00";

                    if (_gameVersion != GameVersion.RS2012)
                    {
                        //Tones setup //TODO: add parsing tones events
                        Arrangement.ToneBase = _song2014.ToneBase;
                        Arrangement.ToneA = _song2014.ToneA;
                        Arrangement.ToneB = _song2014.ToneB;
                        Arrangement.ToneC = _song2014.ToneC;
                        Arrangement.ToneD = _song2014.ToneD;
                        Arrangement.ToneMultiplayer = null;
                        SetupTones(Arrangement);
                    }

                    if (Arrangement.ArrangementType == ArrangementType.Bass)
                        FixBassTuning();

                    SelectTuningName();
                    CheckBassTuning();

                    // save converted RS1 to RS2014 Song2014 XML
                    if (_gameVersion == GameVersion.None && xmlVersion == GameVersion.RS2012)
                    {
                        var srcDir = Path.GetDirectoryName(xmlFilePath);
                        var srcName = Path.GetFileNameWithoutExtension(xmlFilePath);
                        var backupSrcPath = String.Format("{0}_{1}.xml", Path.Combine(srcDir, srcName), "RS1");

                        // backup original RS1 file
                        File.Copy(xmlFilePath, backupSrcPath);

                        // write converted RS1 file
                        using (var stream = new FileStream(xmlFilePath, FileMode.Create))
                            _song2014.Serialize(stream, true);
                    }
                }
                catch (Exception ex)
                {
                    var diaMsg = "This " + xmlVersion + " arrangement has some invalid or" + Environment.NewLine +
                                 "missing data: " + Path.GetFileName(xmlFilePath) + Environment.NewLine + Environment.NewLine +
                                 ex.Message + Environment.NewLine +
                                 "Make sure you are using the latest version of EOF." + Environment.NewLine + Environment.NewLine +
                                 "Please select the correct tuning and confirm all" + Environment.NewLine +
                                 "other data before leaving the arrangement editor.";


                    BetterDialog2.ShowDialog(diaMsg, DLCPackageCreator.MESSAGEBOX_CAPTION, null, null, "OK", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning", 150, 150);
                    return false;
                }
            }
            return true;
        }

        private bool CheckBassTuning()
        {
            // for now just check bass arrangements
            if (Arrangement.ArrangementType != ArrangementType.Bass)
                return true;

            // Error check 220.00 and Fixed
            TuningDefinition tuning = (TuningDefinition)cmbTuningName.SelectedItem;
            Arrangement.Tuning = tuning.UIName;
            Arrangement.TuningStrings = tuning.Tuning;
            UpdateCentOffset();
            // RE: annoying stuff. ony indicate that arrangement isn't autofixed since you won't name tuning that way.
            if ((Arrangement.TuningPitch == 220.00 && !Arrangement.Tuning.Contains("Fixed")) ||
                (Arrangement.TuningPitch != 220.00 && Arrangement.Tuning.Contains("Fixed")))
            {
                MessageBox.Show(@"The bass tuning name and frequency are not set correctly." + Environment.NewLine + Environment.NewLine +
                                @"Tuning name must contain 'Fixed' when frequency is 220Hz" + Environment.NewLine +
                                @"and name may not contain 'Fixed' if frequency is not 220Hz.", @"Error ... Low Bass Tuning", MessageBoxButtons.OK);
                return false;
            }

            return true;
        }

        private void EditShowlights()
        {
            // TODO: future
            using (var form = new ShowLightsForm(Arrangement.SongXml.File))
            {
                if (DialogResult.OK != form.ShowDialog())
                    return;

                if (!String.IsNullOrEmpty(Arrangement.SongXml.File))

                    if (!String.IsNullOrEmpty(form.ShowLightsPath))
                        XmlPath = form.ShowLightsPath;
            }
        }

        private void EditVocals()
        {
            using (var form = new VocalsForm(Arrangement.LyricArt, Arrangement.SongXml.File))
            {
                if (DialogResult.OK != form.ShowDialog())
                    return;

                Arrangement.LyricArt = File.Exists(form.ArtPath) ? form.ArtPath : null;

                if (!String.IsNullOrEmpty(form.VocalsPath))
                {
                    XmlPath = form.VocalsPath;
                    Arrangement.SongXml.File = form.VocalsPath;
                }
            }
        }

        private void FillToneCombo(ComboBox combo, IEnumerable<string> toneNames, bool isBase)
        {
            var lastTone = combo.SelectedItem;
            combo.Items.Clear();
            if (!isBase)
                combo.Items.Add("");
            //TODO: filter out bass tones from guitar tones so you won't mess things out here and reduce tones list a bit.
            foreach (var tone in toneNames)
                combo.Items.Add(tone);

            combo.SelectedIndex = 0;
            if (isBase && !ReferenceEquals(lastTone, null))
            {
                combo.SelectedItem = lastTone;
            }
        }

        private void FillTuningCombo(GameVersion gameVersion)
        {
            cmbTuningName.Items.Clear();

            // TODO: figure out logic behind unexpected LINQ behaviors
            // tuningComboBox list is being updated with custom tuning info
            // so refilling the combo-box is required to produce the expected results
            // for now using old fashioned non-LINQ method
            // also add tuning from songXml used (just populate tuning definitions while loading *.dlc.xml)
            var tuningDefinitions = TuningDefinitionRepository.Instance.LoadTuningDefinitions(gameVersion);
            foreach (var tuning in tuningDefinitions)
            {
                cmbTuningName.Items.Add(tuning);
            }

            cmbTuningName.SelectedIndex = 0;
            cmbTuningName.Refresh();
        }

        private bool FixBassTuning()
        {
            // fix old toolkit behavior
            if (Arrangement.TuningStrings == null)
            {
                Arrangement.TuningStrings = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };
                return false;
            }

            if (_gameVersion == GameVersion.RS2012)
                return false;

            var bassFix = false;
            //Low tuning fix for bass, If lowest string is B and bass fix not applied 
            if (Arrangement.TuningStrings.String0 < -4 && Arrangement.TuningPitch != 220.00)
                if (_fixLowBass)
                    bassFix = true;
                else
                    bassFix |= MessageBox.Show(@"The bass tuning may be too low.  Apply Low Bass Tuning Fix?" + Environment.NewLine + @"Note: The fix may revert if bass Arrangement is re-saved in EOF.  ", @"Warning ... Low Bass Tuning", MessageBoxButtons.YesNo) == DialogResult.Yes;

            // Fix Low Bass Tuning
            if (bassFix && TuningFrequency.ApplyBassFix(Arrangement, _fixLowBass))
            {
                Arrangement.TuningStrings = Song2014.LoadFromFile(Arrangement.SongXml.File).Tuning;
                Arrangement.TuningPitch = 220.00;
                txtFrequency.Text = "220.00";

                return true;
            }

            return false;
        }

        public bool IsAlreadyAdded(string xmlFilePath)
        {
            for (int i = 0; i < _parentControl.lstArrangements.Items.Count; i++)
            {
                var selectedArrangement = (Arrangement)_parentControl.lstArrangements.Items[i];

                if (!xmlFilePath.Equals(selectedArrangement.SongXml.File)) continue;
                if (xmlFilePath.Equals(Arrangement.SongXml.File)) continue;
                return true;
            }
            return false;
        }

        private void SelectTuningName()
        {
            if (Arrangement.ArrangementType == ArrangementType.Vocal || Arrangement.ArrangementType == ArrangementType.ShowLight)
                return;

            // find tuning in tuningComboBox list and make selection
            int foundTuning = -1;
            for (int tcbIndex = 0; tcbIndex < cmbTuningName.Items.Count; tcbIndex++)
            {
                cmbTuningName.SelectedIndex = tcbIndex;
                TuningDefinition tuning = (TuningDefinition)cmbTuningName.Items[tcbIndex];

                // check both tuning strings and name match
                if (tuning.Tuning == Arrangement.TuningStrings) // && tuning.UIName == Arrangement.Tuning)
                {
                    foundTuning = tcbIndex;
                    break;
                }
            }

            // TODO: testing toolkit's AI
            // toolkit is pretty smart now so let it automatically set tuning if found
            // E Standard, Drop D, and Open E tuning are now the same for both guitar and bass
            if (foundTuning == -1)
            {
                cmbTuningName.SelectedIndex = 0;
                var selectedType = ((ArrangementType)cmbArrangementType.SelectedItem);
                ShowTuningForm(selectedType, new TuningDefinition(Arrangement.TuningStrings, _gameVersion));
            }
        }

        private void SequencialToneComboEnabling()
        {//TODO: handle not one-by-one enabling disabling tone slots and use data from enabled one, confused about this one.
            if (_gameVersion != GameVersion.RS2012)
            {
                cmbToneB.Enabled = !String.IsNullOrEmpty((string)cmbToneA.SelectedItem) && cmbToneA.SelectedIndex > 0;
                cmbToneC.Enabled = !String.IsNullOrEmpty((string)cmbToneB.SelectedItem) && cmbToneB.SelectedIndex > 0;
                cmbToneD.Enabled = !String.IsNullOrEmpty((string)cmbToneC.SelectedItem) && cmbToneC.SelectedIndex > 0;
            }
            else
            {
                cmbToneA.Enabled = cmbToneB.Enabled = cmbToneC.Enabled = cmbToneD.Enabled = false;
            }
        }

        /// <summary>
        /// Fill toneCombo with autotone values or BaseOnly.
        /// Get tones, fill combo, select tones.
        /// </summary>
        /// <param name="arr"></param>
        private void SetupTones(Arrangement arr)
        {
            chkTonesDisabled.Checked = false;
            bool isRS2014 = _parentControl.CurrentGameVersion != GameVersion.RS2012;

            if (!String.IsNullOrEmpty(arr.ToneBase))
                if (_parentControl.lstTones.Items.Count == 1 && _parentControl.lstTones.Items[0].ToString() == "Default")
                    _parentControl.lstTones.Items.Clear();

            var toneItems = _parentControl.lstTones.Items;
            var toneNames = new List<string>();
            if (isRS2014)
                toneNames.AddRange(_parentControl.lstTones.Items.OfType<Tone2014>().Select(t => t.Name));
            else
                toneNames.AddRange(_parentControl.lstTones.Items.OfType<Tone>().Select(t => t.Name));

            //Check if autotone tones are present and add it's if not.
            if (!toneNames.Contains(arr.ToneBase) && !String.IsNullOrEmpty(arr.ToneBase))
            {
                toneItems.Add(_parentControl.CreateNewTone(arr.ToneBase));
                toneNames.Add(arr.ToneBase);
            }
            if (!toneNames.Contains(arr.ToneA) && !String.IsNullOrEmpty(arr.ToneA))
            {
                toneItems.Add(_parentControl.CreateNewTone(arr.ToneA));
                toneNames.Add(arr.ToneA);
            }
            if (!toneNames.Contains(arr.ToneB) && !String.IsNullOrEmpty(arr.ToneB))
            {
                toneItems.Add(_parentControl.CreateNewTone(arr.ToneB));
                toneNames.Add(arr.ToneB);
            }
            if (!toneNames.Contains(arr.ToneC) && !String.IsNullOrEmpty(arr.ToneC))
            {
                toneItems.Add(_parentControl.CreateNewTone(arr.ToneC));
                toneNames.Add(arr.ToneC);
            }
            if (!toneNames.Contains(arr.ToneD) && !String.IsNullOrEmpty(arr.ToneD))
            {
                toneItems.Add(_parentControl.CreateNewTone(arr.ToneD));
                toneNames.Add(arr.ToneD);
            }

            try
            {
                // FILL TONE COMBO
                FillToneCombo(cmbToneBase, toneNames, true);
                FillToneCombo(cmbToneA, toneNames, false);
                FillToneCombo(cmbToneB, toneNames, false);
                FillToneCombo(cmbToneC, toneNames, false);
                FillToneCombo(cmbToneD, toneNames, false);
            }
            catch (Exception ex)
            {
                // catch RS1 and do nothing 
                Debug.WriteLine("SetupTones: " + ex.Message);
            }

            // SELECTING TONES
            cmbToneBase.Enabled = true;
            if (!String.IsNullOrEmpty(arr.ToneBase))
                cmbToneBase.SelectedItem = arr.ToneBase;
            if (!String.IsNullOrEmpty(arr.ToneA))
                cmbToneA.SelectedItem = arr.ToneA;
            if (!String.IsNullOrEmpty(arr.ToneB))
                cmbToneB.SelectedItem = arr.ToneB;
            if (!String.IsNullOrEmpty(arr.ToneC))
                cmbToneC.SelectedItem = arr.ToneC;
            if (!String.IsNullOrEmpty(arr.ToneD))
                cmbToneD.SelectedItem = arr.ToneD;

            // If have ToneBase and ToneB is setup it's because auto tone are setup in EoF, so, disable edit to prevent errors.
            chkTonesDisabled.Checked = (!String.IsNullOrEmpty(arr.ToneBase) && !String.IsNullOrEmpty(arr.ToneB));
            if (chkTonesDisabled.Checked) // && !EditMode)
            {
                // disableTonesCheckbox.Enabled = false;
                _toolTip.IsBalloon = true;
                _toolTip.AutoPopDelay = 9000; // tt remains visible if the point is stationary
                _toolTip.InitialDelay = 100; // time that passes before tt appears
                _toolTip.ReshowDelay = 100; // time before next tt window appears
                _toolTip.ShowAlways = true; // force tt display even if parent is not active
                _toolTip.SetToolTip(chkTonesDisabled, "Expert Toolkit Users Only");
            }
        }

        private void ShowTuningForm(ArrangementType selectedType, TuningDefinition tuning)
        {
            if (tuning == null)
            {
                MessageBox.Show("Pick a tuning definition to start editing.\r\n (DEBUG: Current tuning is Null)", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // get the latest comments from the XML to check if previous bass fixed is valid
            if (!String.IsNullOrEmpty(Arrangement.SongXml.File) && selectedType == ArrangementType.Bass)
            {
                var xmlComments = Song2014.ReadXmlComments(Arrangement.SongXml.File);
                var isBassFixed = xmlComments.Any(xComment => xComment.ToString().Contains("Low Bass Tuning Fixed")) || Convert.ToDouble(txtFrequency.Text) == 220.00;

                // commented out ... Arrangement.XmlComments may not be populated
                // var isBassFixed = Arrangement.XmlComments.Any(xComment => xComment.ToString().Contains("Low Bass Tuning Fixed")) || Convert.ToDouble(txtFrequency.Text) == 220.00;

                if (isBassFixed && !tuning.UIName.Contains("Fixed"))
                {
                    // UIName may contain spaces, where as Name contains no spaces
                    tuning.UIName = String.Format("{0} Fixed", tuning.UIName);
                    tuning.Name = tuning.UIName.ReplaceSpaceWith("");
                    tuning.Custom = true;
                    // fixed bass tuning definition is auto added to repository
                    TuningDefinitionRepository.SaveUnique(tuning);
                }
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
                // Update lstArrangements slots if tuning name is changed
                for (int i = 0; i < _parentControl.lstArrangements.Items.Count; i++)
                {
                    var selectedArrangement = (Arrangement)_parentControl.lstArrangements.Items[i];

                    if (tuning.UIName.Equals(selectedArrangement.Tuning))
                    {
                        selectedArrangement.Tuning = formTuning.UIName;
                        _parentControl.lstArrangements.Items[i] = selectedArrangement;
                    }
                }
            }

            FillTuningCombo(_gameVersion);

            int foundTuning = -1;
            cmbTuningName.SelectedIndex = -1;
            for (int tcbIndex = 0; tcbIndex < cmbTuningName.Items.Count; tcbIndex++)
            {
                cmbTuningName.SelectedIndex = tcbIndex;
                tuning = (TuningDefinition)cmbTuningName.Items[tcbIndex];

                // check at least tuning strings and name match
                if (tuning.Tuning == formTuning.Tuning && tuning.Name == formTuning.Name)
                {
                    foundTuning = tcbIndex;
                    break;
                }
            }

            // add the custom tuning to tuningComboBox
            if (foundTuning == -1)
            {
                if (addNew)
                    TuningDefinitionRepository.SaveUnique(formTuning);

                formTuning.Custom = true;
                cmbTuningName.Items.Add(formTuning);
                cmbTuningName.SelectedIndex = cmbTuningName.Items.Count - 1;
            }
            else
                cmbTuningName.SelectedIndex = foundTuning;

            cmbTuningName.Refresh();
            Arrangement.TuningStrings = formTuning.Tuning;
            Arrangement.Tuning = formTuning.UIName;
        }

        private void ToneComboEnabled(bool enabled)
        {
            // Not disabling in gbTone to not disable labels
            cmbToneBase.Enabled = enabled; //TODO: check if this required to be disabled if we've got autotone changes.
            cmbToneA.Enabled = enabled;
            cmbToneB.Enabled = enabled;
            cmbToneC.Enabled = enabled;
            cmbToneD.Enabled = enabled;
        }

        /// <summary>
        /// Update TuningPitch related fields.
        /// </summary>
        /// <param name="type">Set type update, by Hz or cents</param>
        private void UpdateCentOffset(string type = "HZ")
        {
            string value;
            double freq;

            if (type.StartsWith("c") && !_dirty)
            {
                value = txtCentOffset.Text;
                if (String.IsNullOrEmpty(value)) //im not sure if this useful at all
                    value = "0";

                _dirty = true;
                double cent;
                if (Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out cent))
                {
                    freq = cent.Cents2Frequency();
                    //upd self + main
                    txtCentOffset.Text = String.Format("{0:0}", cent);
                    txtFrequency.Text = String.Format("{0:0.00}", freq);
                    //upd rest
                    lblRootNote.Text = freq.Frequency2Note();
                    Arrangement.TuningPitch = freq;
                    txtCentOffset.BackColor = txtFrequency.BackColor = System.Drawing.SystemColors.Window;
                }
                else
                {
                    txtCentOffset.BackColor = System.Drawing.Color.HotPink;
                }
                _dirty = false;
            }
            else if (type.StartsWith("HZ") && !_dirty)
            {
                value = txtFrequency.Text;
                if (String.IsNullOrEmpty(value))
                    value = "440.00";

                _dirty = true;
                double hz;
                if (Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out hz) && hz > 0.00)
                {
                    freq = hz;
                    //upd sub
                    txtCentOffset.Text = String.Format("{0:0}", hz.Frequency2Cents());
                    //upd rest
                    lblRootNote.Text = freq.Frequency2Note();
                    Arrangement.TuningPitch = freq;
                    txtCentOffset.BackColor = txtFrequency.BackColor = System.Drawing.SystemColors.Window;
                }
                else
                {
                    txtFrequency.BackColor = System.Drawing.Color.HotPink;
                }
                _dirty = false;
            }
        }

        private void UpdateRouteMaskPath(ArrangementType arrangementType, ArrangementName arrangementName)
        {
            gbGameplayPath.Enabled = (arrangementType != ArrangementType.Vocal && arrangementType != ArrangementType.ShowLight) && _gameVersion != GameVersion.RS2012;

            //Enabling
            rbRouteMaskLead.Enabled = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Lead);
            rbRouteMaskRhythm.Enabled = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Rhythm);
            rbRouteMaskBass.Enabled = arrangementType == ArrangementType.Bass;

            //Auto-checking
            rbRouteMaskLead.Checked = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Lead);
            rbRouteMaskRhythm.Checked = arrangementType == ArrangementType.Guitar && (arrangementName == ArrangementName.Combo || arrangementName == ArrangementName.Rhythm);
            rbRouteMaskBass.Checked = arrangementType == ArrangementType.Bass;
        }

        private void UpdateScrollSpeedDisplay()
        {
            txtScrollSpeed.Text = String.Format("Scroll speed: {0:#.0}", Math.Truncate((decimal)tbarScrollSpeed.Value) / 10);
        }

        private void ArrangementForm_Load(object sender, EventArgs e)
        {
            // disallow changing XML file name when in edit mode
            if (EditMode)
            {
                btnBrowseXml.Enabled = false;
                txtXmlPath.ReadOnly = true;

                if (Arrangement.ArrangementType == ArrangementType.Bass)
                    FixBassTuning();

                SelectTuningName();
                CheckBassTuning();
            }
        }

        private void btnBrowseXml_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = ConfigGlobals.DefaultProjectFolder;
                ofd.Filter = "Rocksmith EOF XML Files (*.xml)|*.xml";
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                XmlPath = ofd.FileName;
                ConfigGlobals.DefaultProjectFolder = Path.GetDirectoryName(XmlPath);

                if (IsAlreadyAdded(XmlPath))
                {
                    MessageBox.Show(@"XML Arrangement: " + Path.GetFileName(XmlPath) + @"   " + Environment.NewLine +
                                    @"has already been added.  Please choose a new file. ",
                                    DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AddXmlArrangement(XmlPath);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnEditTuning_Click(object sender, EventArgs e)
        {
            var selectedType = (ArrangementType)cmbArrangementType.SelectedItem;
            var tuning = (TuningDefinition)cmbTuningName.SelectedItem;

            ShowTuningForm(selectedType, tuning);
        }

        private void btnEditType_Click(object sender, EventArgs e)
        {
            if ((ArrangementType)cmbArrangementType.SelectedItem == ArrangementType.Vocal)
                EditVocals();

            else if ((ArrangementType)cmbArrangementType.SelectedItem == ArrangementType.ShowLight)
                EditShowlights();

            else
            {
                // TODO: future options like personal Audio file, multitracks, etc.
                // GuitarEdit();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //Validations
            if (!File.Exists(XmlPath) || String.IsNullOrEmpty(XmlPath))
            {
                MessageBox.Show("Xml Arrangement file path is not valid.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtXmlPath.Focus();
                return;
            }

            if (_gameVersion != GameVersion.RS2012)
            {
                if (!rbRouteMaskLead.Checked && !rbRouteMaskRhythm.Checked && !rbRouteMaskBass.Checked && (ArrangementType)cmbArrangementType.SelectedItem != ArrangementType.Vocal && (ArrangementType)cmbArrangementType.SelectedItem != ArrangementType.ShowLight)
                {
                    MessageBox.Show("You did not select a Gameplay Path for this arrangement.", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    gbGameplayPath.Focus();
                    return;
                }
            }

            if (!CheckBassTuning())
                return;

            LoadArrangementData(XmlPath);
            ConfigGlobals.DefaultProjectFolder = Path.GetDirectoryName(XmlPath);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void chkTonesDisabled_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = !((CheckBox)sender).Checked;
            ToneComboEnabled(enabled);
            if (enabled)
                SequencialToneComboEnabling();
        }

        private void cmbTone_SelectedIndexChanged(object sender, EventArgs e)
        {
            SequencialToneComboEnabling();
        }

        private void tbarScrollSpeed_ValueChanged(object sender, EventArgs e)
        {
            UpdateScrollSpeedDisplay();
        }

        private void txtFrequency_TextChanged(object sender, EventArgs e)
        {
            var bFreq = ((CueTextBox)sender).Name.StartsWith("txtFrequency");
            UpdateCentOffset(bFreq ? "HZ" : "c");
        }

        private void cmbTuningName_SelectedValueChanged(object sender, EventArgs e)
        {
            // Selecting defaults
            var selectedType = (ArrangementType)cmbArrangementType.SelectedItem;
            var selectedTuning = (TuningDefinition)((ComboBox)sender).SelectedItem;
            btnEditTuning.Enabled = (selectedType != ArrangementType.Vocal && selectedType != ArrangementType.ShowLight) && selectedTuning != null;
        }
    }
}

