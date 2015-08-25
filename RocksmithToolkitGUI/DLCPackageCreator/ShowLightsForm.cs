using Ookii.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ShowLightsForm : Form
    {
        public string ShowLightsPath { get; set; }

        public ShowLightsForm(string showLightsPath)
        {
            InitializeComponent();
            txtShowLights.Text = ShowLightsPath = showLightsPath;
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ShowLightsPath))
                ShowLightsPath = "";

            this.DialogResult = DialogResult.OK;
            Close();
        }

        void btnShowLights_Click(object sender, EventArgs e)
        {
            using (var f = new VistaOpenFileDialog())
            {
                f.FileName = ShowLightsPath;
                f.Filter = "Rocksmith 2014 ShowLight XML Files (*_showlights.xml)|*_showlights.xml";
                if (f.ShowDialog() == DialogResult.OK)
                {
                    txtShowLights.Text = ShowLightsPath = f.FileName;
                }
            }
        }

        void txtShowLights_TextChanged(object sender, EventArgs e)
        {
            var name = (TextBox)sender;
            ShowLightsPath = name.Text;
        }

    }
}
