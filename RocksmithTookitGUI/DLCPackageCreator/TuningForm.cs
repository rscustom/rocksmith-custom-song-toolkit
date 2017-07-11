using System;
using System.Windows.Forms;
using RocksmithToolkitLib;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class TuningForm : Form
    {
        public bool AddNew = false;
        public bool IsBass = false;

        private TuningDefinition _tuning;

        public TuningForm()
        {
            InitializeComponent();
        }

        public TuningDefinition Tuning
        {
            get
            {
                if (String.IsNullOrWhiteSpace(txtUIName.Text))
                {
                    txtUIName.Focus();
                    return null;
                }
                _tuning.UIName = txtUIName.Text.Trim();

                if (String.IsNullOrWhiteSpace(txtName.Text))
                {
                    txtName.Focus();
                    return null;
                }
                _tuning.Name = txtName.Text.Trim();

                _tuning.Tuning = new TuningStrings { String0 = 0, String1 = 0, String2 = 0, String3 = 0, String4 = 0, String5 = 0 };

                #region Parse fields

                Int16 s0 = 0;
                bool b0 = Int16.TryParse(txtString0.Text.Trim(), out s0);
                if (!b0)
                {
                    txtString0.Focus();
                    return null;
                }
                _tuning.Tuning.String0 = s0;

                Int16 s1 = 0;
                bool b1 = Int16.TryParse(txtString1.Text.Trim(), out s1);
                if (!b1)
                {
                    txtString1.Focus();
                    return null;
                }
                _tuning.Tuning.String1 = s1;

                Int16 s2 = 0;
                bool b2 = Int16.TryParse(txtString2.Text.Trim(), out s2);
                if (!b2)
                {
                    txtString2.Focus();
                    return null;
                }
                _tuning.Tuning.String2 = s2;

                Int16 s3 = 0;
                bool b3 = Int16.TryParse(txtString3.Text.Trim(), out s3);
                if (!b3)
                {
                    txtString3.Focus();
                    return null;
                }
                _tuning.Tuning.String3 = s3;

                Int16 s4 = 0;
                bool b4 = Int16.TryParse(txtString4.Text.Trim(), out s4);
                if (!b4)
                {
                    txtString4.Focus();
                    return null;
                }
                _tuning.Tuning.String4 = s4;

                Int16 s5 = 0;
                bool b5 = Int16.TryParse(txtString5.Text.Trim(), out s5);
                if (!b5)
                {
                    txtString5.Focus();
                    return null;
                }
                _tuning.Tuning.String5 = s5;

                #endregion

                return _tuning;
            }
            set
            {
                _tuning = value;
                txtUIName.Text = _tuning.UIName;
                txtName.Text = _tuning.Name;
                txtString0.Text = _tuning.Tuning.String0.ToString();
                txtString1.Text = _tuning.Tuning.String1.ToString();
                txtString2.Text = _tuning.Tuning.String2.ToString();
                txtString3.Text = _tuning.Tuning.String3.ToString();
                txtString4.Text = _tuning.Tuning.String4.ToString();
                txtString5.Text = _tuning.Tuning.String5.ToString();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Tuning == null)
            {
                MessageBox.Show("All fields are required!", DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //in case it's bass and EOF won't wrote a tuning for last 2 strings...
            //TODO: guess guitar tuning.
            if (IsBass && Tuning.Tuning.String4 == 0 && Tuning.Tuning.String5 == 0)
                if (MessageBox.Show(String.Format("Are strings 4 and 5 really 0?{0}It is recommended that all strings fields be filled in to cover guitar tuning.{0}This avoids having two of the same tuning for different instruments.{0}{0}Would you like to correct it now?{0}",
                    Environment.NewLine), DLCPackageCreator.MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    return;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void chkAddTuning_CheckedChanged(object sender, EventArgs e)
        {
            AddNew = chkAddTuning.Checked;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            TextBox name = (TextBox)sender;
            name.TextChanged -= txtName_TextChanged;
            name.Text = name.Text.GetValidAtaSpaceName().ReplaceSpaceWith("");
            name.TextChanged += txtName_TextChanged;
        }

        private void txtUIName_TextChanged(object sender, EventArgs e)
        {
            TextBox name = (TextBox)sender;
            name.TextChanged -= txtUIName_TextChanged;
            txtName.Text = name.Text.GetValidAtaSpaceName();
            name.TextChanged += txtUIName_TextChanged;
        }


    }
}
