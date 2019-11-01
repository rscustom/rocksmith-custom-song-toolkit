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
            lstKey.Items.Add("Fog midi notes: 24-35");
            lstKey.Items.Add("24 - (C) Green");
            lstKey.Items.Add("25 - (C#)Dark Red");
            lstKey.Items.Add("26 - (D) Medium Turquoise");
            lstKey.Items.Add("27 - (D#) Brown");
            lstKey.Items.Add("28 - (E) Blue");
            lstKey.Items.Add("29 - (F) LtGreen");
            lstKey.Items.Add("30 - (F#) Purple");
            lstKey.Items.Add("31 - (G) Dark LtGreen");
            lstKey.Items.Add("32 - (G#) Dark Orange");
            lstKey.Items.Add("33 - (A) LtBlue");
            lstKey.Items.Add("34 - (A#) Yellow");
            lstKey.Items.Add("35 - (B) Dark Violet");
            lstKey.Items.Add("Spotlights midi notes: 48-59, 42 is off");
            lstKey.Items.Add("48 - (C) Green");
            lstKey.Items.Add("49 - (C#)Dark Red");
            lstKey.Items.Add("50 - (D) Medium Turquoise");
            lstKey.Items.Add("51 - (D#) Brown");
            lstKey.Items.Add("52 - (E) Blue");
            lstKey.Items.Add("53 - (F) LtGreen");
            lstKey.Items.Add("54 - (F#) Purple");
            lstKey.Items.Add("55 - (G) Dark LtGreen");
            lstKey.Items.Add("56 - (G#) Dark Orange");
            lstKey.Items.Add("57 - (A) LtBlue");
            lstKey.Items.Add("58 - (A#) Yellow");
            lstKey.Items.Add("59 - (B) Dark Violet");
            lstKey.Items.Add("Effects midi notes: 66-67, and combinations");
            lstKey.Items.Add("Spinning Laser Light: 66");
            lstKey.Items.Add("Search Light Spot: 67");
            lstKey.Items.Add("Back spotlights combine: 66 folled by 67");
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
                f.InitialDirectory = Path.GetDirectoryName(ShowLightsPath);
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
