using System;
using System.Windows.Forms;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class TuningForm : Form
    {
        private TuningDefinition _tuning;

        public bool IsBass = false;
        public bool AddNew = false;

        public TuningDefinition Tuning
        {
            get
            {
                if (String.IsNullOrWhiteSpace(uiNameTB.Text))
                {
                    uiNameTB.Focus();
                    return null;
                }
                _tuning.UIName = uiNameTB.Text.Trim();

                if (String.IsNullOrWhiteSpace(nameTB.Text))
                {
                    nameTB.Focus();
                    return null;
                }
                _tuning.Name = nameTB.Text.Trim();

                _tuning.Tuning = new TuningStrings();

                #region Parse fields

                Int16 s0 = 0;
                bool b0 = Int16.TryParse(string0TB.Text.Trim(), out s0);
                if (!b0)
                {
                    string0TB.Focus();
                    return null;
                }
                _tuning.Tuning.String0 = s0;

                Int16 s1 = 0;
                bool b1 = Int16.TryParse(string1TB.Text.Trim(), out s1);
                if (!b1)
                {
                    string1TB.Focus();
                    return null;
                }
                _tuning.Tuning.String1 = s1;

                Int16 s2 = 0;
                bool b2 = Int16.TryParse(string2TB.Text.Trim(), out s2);
                if (!b2)
                {
                    string2TB.Focus();
                    return null;
                }
                _tuning.Tuning.String2 = s2;

                Int16 s3 = 0;
                bool b3 = Int16.TryParse(string3TB.Text.Trim(), out s3);
                if (!b3)
                {
                    string3TB.Focus();
                    return null;
                }
                _tuning.Tuning.String3 = s3;

                Int16 s4 = 0;
                bool b4 = Int16.TryParse(string4TB.Text.Trim(), out s4);
                if (!b4)
                {
                    string4TB.Focus();
                    return null;
                }
                _tuning.Tuning.String4 = s4;

                Int16 s5 = 0;
                bool b5 = Int16.TryParse(string5TB.Text.Trim(), out s5);
                if (!b5)
                {
                    string5TB.Focus();
                    return null;
                }
                _tuning.Tuning.String5 = s5;

                #endregion

                return _tuning;
            }
            set
            {
                _tuning = value;
                uiNameTB.Text = _tuning.UIName;
                nameTB.Text = _tuning.Name;
                string0TB.Text = _tuning.Tuning.String0.ToString();
                string1TB.Text = _tuning.Tuning.String1.ToString();
                string2TB.Text = _tuning.Tuning.String2.ToString();
                string3TB.Text = _tuning.Tuning.String3.ToString();
                string4TB.Text = _tuning.Tuning.String4.ToString();
                string5TB.Text = _tuning.Tuning.String5.ToString();
            }
        }

        public TuningForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (Tuning == null)
            {
                MessageBox.Show("All fields are required!", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //in case it's bass and EOF won't wrote a tuning for last 2 strings...
            //TODO: guess guitar tuning.
            if (IsBass && Tuning.Tuning.String4 == 0 && Tuning.Tuning.String5 == 0)
                if (MessageBox.Show(String.Format("Are strings 4 and 5 really 0?{0}We recommend filling in all strings fields (to cover guitar tuning).{0}To avoid two of the same tuning for different instruments.{0}{0}Would you like to cancel save and correct it now?{0}",
                    Environment.NewLine), DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    return;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void nameTB_TextChanged(object sender, EventArgs e)
        {
            TextBox name = (TextBox)sender;
            name.TextChanged -= nameTB_TextChanged;
            name.Text = name.Text.GetValidName(false);
            name.TextChanged += nameTB_TextChanged;
        }

        private void uiNameTB_TextChanged(object sender, EventArgs e)
        {
            TextBox name = (TextBox)sender;
            name.TextChanged -= uiNameTB_TextChanged;
            nameTB.Text = name.Text.GetValidName(true);
            name.TextChanged += uiNameTB_TextChanged;
        }

        private void StateAdd_CheckedChanged(object sender, EventArgs e)
        {
            AddNew = StateAdd.Checked;
        }
    }
}
