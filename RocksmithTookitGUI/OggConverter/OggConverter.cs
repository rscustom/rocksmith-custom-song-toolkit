using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Ogg;

namespace RocksmithTookitGUI.OggConverter
{
    public partial class OggConverter : UserControl
    {
        public OggConverter()
        {
            InitializeComponent();
        }

        private string InputOggFile
        {
            get { return inputOggTextBox.Text; }
            set { inputOggTextBox.Text = value; }
        }

        private string OutputOggFile
        {
            get { return outputOggTextBox.Text; }
            set { outputOggTextBox.Text = value; }
        }

        private void oggBrowseButton_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "Wwise 2010.3.3 OGG Files|*.ogg|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.ShowDialog();
                if (string.IsNullOrEmpty(fd.FileName)) return;

                InputOggFile = fd.FileName;
                var fileName = Path.ChangeExtension(fd.FileName, null);
                fileName += "_fixed";
                fileName = Path.ChangeExtension(fileName, "ogg");
                OutputOggFile = fileName;
            }
        }

        private void oggConvertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputOggFile)) return;
            if (string.IsNullOrEmpty(OutputOggFile)) return;

            try
            {
                OggFile.ConvertOgg(InputOggFile, OutputOggFile);
                MessageBox.Show("Conversion complete!", "OGG Conversion Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(ex.Message, "OGG Conversion Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show("Conversion has failed.", ex);
            }
        }
    }
}
