using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.Tone;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public partial class ToneForm : Form
    {
        public ToneForm(Tone tone)
        {
            InitializeComponent();
            toneControl1.Tone = tone;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
