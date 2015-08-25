using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.ZiggyProEditor;

namespace RocksmithToolkitGUI.ZiggyProEditorConverter
{
    public partial class ZpeConverter : UserControl
    {
        public ZpeConverter()
        {
            InitializeComponent();
        }

        private string InputZpeXmlFile
        {
            get { return txtZpeInput.Text; }
            set { txtZpeInput.Text = value; }
        }

        private string OutputRs1XmlFile
        {
            get { return txtRs1Output.Text; }
            set { txtRs1Output.Text = value; }
        }

        private string OutputRs2014XmlFile
        {
            get { return txtRs2014Output.Text; }
            set { txtRs2014Output.Text = value; }
        }


        private void btnZpeInput_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                if (DialogResult.OK != fd.ShowDialog())
                {
                    return;
                }

                InputZpeXmlFile = fd.FileName;
            }
        }

        private void btnRs1Output_Click(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.FileName = Path.GetFileNameWithoutExtension(InputZpeXmlFile) + "_Song_Combo.xml";

                if (DialogResult.OK != fd.ShowDialog())
                {
                    return;
                }

                OutputRs1XmlFile = fd.FileName;
            }
        }

        private void btnRs2014Output_Click_1(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.FileName = Path.GetFileNameWithoutExtension(InputZpeXmlFile) + "_Song2014.xml";

                if (DialogResult.OK != fd.ShowDialog())
                {
                    return;
                }

                OutputRs2014XmlFile = fd.FileName;
            }
        }
        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputZpeXmlFile)) return;

            if (string.IsNullOrEmpty(OutputRs1XmlFile) &&
                string.IsNullOrEmpty(OutputRs2014XmlFile)) return;

            try
            {
                if (!string.IsNullOrEmpty(OutputRs1XmlFile))
                    new Converter().Convert(InputZpeXmlFile, OutputRs1XmlFile);

                if (!string.IsNullOrEmpty(OutputRs2014XmlFile))
                    new Converter2014().Convert(InputZpeXmlFile, OutputRs2014XmlFile);

                MessageBox.Show("Process Complete", "File Creation Process");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    errorMessage += "\n" + ex.InnerException.Message;

                PSTaskDialog.cTaskDialog.MessageBox("Error", "Conversion has failed.", errorMessage, ex.ToString(),
                    "Click 'show details' for complete exception information.", "",
                    PSTaskDialog.eTaskDialogButtons.OK, PSTaskDialog.eSysIcons.Error, PSTaskDialog.eSysIcons.Information);
            }
        }






    }
}
