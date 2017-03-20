using Ookii.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ShowLightsForm : Form
    {
        public ShowLightsForm(string showLightsXmlPath)
        {
            InitializeComponent();
            ShowLightsPath = showLightsXmlPath;
            PopulateKey();
            PopulateRichText();
        }

        public string ShowLightsPath
        {
            get { return txtShowLights.Text; }
            set { txtShowLights.Text = value; }
        }

        public void PopulateRichText()
        {
            if (!String.IsNullOrEmpty(ShowLightsPath))
                rtbShowlights.LoadFile(ShowLightsPath, RichTextBoxStreamType.PlainText);
        }

        private void PopulateKey()
        {
            lstKey.Items.Add("24 to 35 - Fog Color");
            lstKey.Items.Add("24 - (C) Green");
            lstKey.Items.Add("25 - (C#)Dark Red");
            lstKey.Items.Add("26 - (D) Medium Turquoise");
            lstKey.Items.Add("27 - (D#) Brown");
            lstKey.Items.Add("28 - (E) Blue");
            lstKey.Items.Add("29 - (F) LtGreen");
            lstKey.Items.Add("30 - (F#) Purple");
            lstKey.Items.Add("31 - (G) Dark LtGreen");
            lstKey.Items.Add("32 - (G#) Dark Orange");
            lstKey.Items.Add("33 - (A) Yellow");
            lstKey.Items.Add("34 - (A#) LtBlue");
            lstKey.Items.Add("35 - (B) Dark Violet");
            lstKey.Items.Add("36 to 41 - Unknown");
            lstKey.Items.Add("42 to 59 - Spotlights/colors/effects");
            lstKey.Items.Add("60 to 62 - Causes in-game hangs");
            lstKey.Items.Add("63 to 65 - Unknown");
            lstKey.Items.Add("66 to 67 - Laser Lights");
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ShowLightsPath))
                rtbShowlights.SaveFile(ShowLightsPath, RichTextBoxStreamType.PlainText);
            
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
                    ShowLightsPath = f.FileName;
                    PopulateRichText();
                }
            }
        }

    }
}
