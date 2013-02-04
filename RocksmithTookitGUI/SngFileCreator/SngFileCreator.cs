using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Sng;

namespace RocksmithTookitGUI.SngFileCreator
{
    public partial class SngFileCreator : UserControl
    {
        public SngFileCreator()
        {
            InitializeComponent();
            tuningComboBox.SelectedIndex = 0;
        }

        private string InputXmlFile
        {
            get { return inputXmlTextBox.Text; }
            set { inputXmlTextBox.Text = value; }
        }

        private string OutputSngFile
        {
            get { return outputFileTextBox.Text; }
            set { outputFileTextBox.Text = value; }
        }

        private ArrangementType ArrangementType
        {
            get
            {
                if (guitarRadioButton.Checked)
                    return ArrangementType.Guitar;
                if (bassRadioButton.Checked)
                    return ArrangementType.Bass;
                if (vocalsRadioButton.Checked)
                    return ArrangementType.Vocal;
                throw new InvalidOperationException("No arrangement type selected");
            }
        }

        private GamePlatform Platform
        {
            get
            {
                if (littleEndianRadioBtn.Checked)
                    return GamePlatform.Pc;
                if (bigEndianRadioBtn.Checked)
                    return GamePlatform.XBox360; /*Same as PS3*/
                throw new InvalidOperationException("No game platform selected");
            }
        }

        private InstrumentTuning Tuning
        {
            get
            {
                return (InstrumentTuning)tuningComboBox.SelectedIndex;
            }
        }

        private void xmlBrowseButton_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.ShowDialog();
                if (string.IsNullOrEmpty(fd.FileName)) return;

                InputXmlFile = fd.FileName;
                OutputSngFile = Path.ChangeExtension(fd.FileName, "sng");
            }
        }

        private void sngConvertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputXmlFile)) return;
            if (string.IsNullOrEmpty(OutputSngFile)) return;

            try
            {
                SngFileWriter.Write(InputXmlFile, OutputSngFile, ArrangementType, Platform, Tuning);
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
