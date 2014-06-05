using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.Tone;
using System.Runtime.Serialization;
using System.Xml;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class TuningForm : Form
    {
        public bool IsBass = false;
        public bool EditMode = false;
        public bool AddNew = false;

        private TuningDefinition tuning;
        public TuningDefinition Tuning {
            get {
                if (String.IsNullOrEmpty(uiNameTB.Text.Trim()))
                {
                    uiNameTB.Focus();
                    return null;
                }
                tuning.UIName = uiNameTB.Text.Trim();

                if (String.IsNullOrEmpty(nameTB.Text.Trim()))
                {
                    nameTB.Focus();
                    return null;
                }
                tuning.Name = nameTB.Text.Trim();

                tuning.Tuning = new TuningStrings();

                Int32 s0 = 0;
                bool b0 = Int32.TryParse(string0TB.Text.Trim(), out s0);
                if (!b0) {
                    string0TB.Focus();
                    return null;
                }
                tuning.Tuning.String0 = s0;

                Int32 s1 = 0;
                bool b1 = Int32.TryParse(string1TB.Text.Trim(), out s1);
                if (!b1) {
                    string1TB.Focus();
                    return null;
                }
                tuning.Tuning.String1 = s1;

                Int32 s2 = 0;
                bool b2 = Int32.TryParse(string2TB.Text.Trim(), out s2);
                if (!b2) {
                    string2TB.Focus();
                    return null;
                }
                tuning.Tuning.String2 = s2;

                Int32 s3 = 0;
                bool b3 = Int32.TryParse(string3TB.Text.Trim(), out s3);
                if (!b3) {
                    string3TB.Focus();
                    return null;
                }
                tuning.Tuning.String3 = s3;

                Int32 s4 = 0;
                bool b4 = Int32.TryParse(string4TB.Text.Trim(), out s4);
                if (!b4) {
                    string4TB.Focus();
                    return null;
                }
                tuning.Tuning.String4 = s4;

                Int32 s5 = 0;
                bool b5 = Int32.TryParse(string5TB.Text.Trim(), out s5);
                if (!b5) {
                    string5TB.Focus();
                    return null;
                }
                tuning.Tuning.String5 = s5;

                return tuning;
            }
            set {
                tuning = value;
                uiNameTB.Text = tuning.UIName;
                nameTB.Text = tuning.Name;
                string0TB.Text = tuning.Tuning.String0.ToString();
                string1TB.Text = tuning.Tuning.String1.ToString();
                string2TB.Text = tuning.Tuning.String2.ToString();
                string3TB.Text = tuning.Tuning.String3.ToString();
                string4TB.Text = tuning.Tuning.String4.ToString();
                string5TB.Text = tuning.Tuning.String5.ToString();
            }
        }

        public TuningForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (Tuning == null) {
                MessageBox.Show("All fields are required!", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (IsBass && Tuning.Tuning.String4 == 0 && Tuning.Tuning.String5 == 0)
                if (MessageBox.Show("Strings 4 and 5 are really 0 (B and E)?" + Environment.NewLine +
                                    "We recommend add all strings to cover also guitar tuning to avoid two same tuning for different instruments." + Environment.NewLine +
                                    "Cancel save and fix it now?", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    return;

            if (EditMode) {
                // Update tuning by strings
                TuningDefinition t = (IsBass) ? TuningDefinitionRepository.Instance().SelectForBass(tuning.Tuning, tuning.GameVersion) : TuningDefinitionRepository.Instance().Select(tuning.Tuning, tuning.GameVersion);
                if (t != null) {
                    t.UIName = Tuning.UIName;
                    t.Name = Tuning.Name;
                    TuningDefinitionRepository.Instance().Save(true);
                } else
                    TuningDefinitionRepository.Instance().Add(Tuning, true);
            } else
                TuningDefinitionRepository.Instance().Add(Tuning, true);

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void nameTB_TextChanged(object sender, EventArgs e)
        {
            TextBox name = (TextBox)sender;
            name.Text = name.Text.GetValidName(false);
        }

        private void uiNameTB_TextChanged(object sender, EventArgs e) {
            TextBox name = (TextBox)sender;
            nameTB.Text = name.Text.GetValidName(false);
        }

        private void StateAdd_CheckedChanged(object sender, EventArgs e)
        {
            AddNew = StateAdd.Checked;
        }
    }
}
