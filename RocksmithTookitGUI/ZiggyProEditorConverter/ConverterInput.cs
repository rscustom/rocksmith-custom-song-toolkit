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
    public partial class ConvertInput : UserControl
    {
        public ConvertInput()
        {
            InitializeComponent();
        }

        private string InputXmlFile
        {
            get { return inputXmlTextBox.Text; }
            set { inputXmlTextBox.Text = value; }
        }

        private string OutputXmlFile
        {
            get { return outputFileTextBox.Text; }
            set { outputFileTextBox.Text = value; }
        }

        private void xmlBrowseButton_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                if (DialogResult.OK != fd.ShowDialog())
                {
                    return;
                }

                InputXmlFile = fd.FileName;
            }
        }

        private void outputXmlButton_Click(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                if (DialogResult.OK != fd.ShowDialog())
                {
                    return;
                }

                OutputXmlFile = fd.FileName;
            }
        }

        private void sngConvertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputXmlFile)) return;
            if (string.IsNullOrEmpty(OutputXmlFile)) return;

            try
            {
                new Converter().Convert(InputXmlFile, OutputXmlFile);
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
