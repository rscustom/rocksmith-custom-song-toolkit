using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithEncoder;

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
            var fd = new OpenFileDialog
            {
                Filter = "Wwise 2010.3.3 OGG Files|*.ogg|All Files (*.*)|*.*",
                FilterIndex = 1
            };
            fd.ShowDialog();
            if (string.IsNullOrEmpty(fd.FileName)) return;

            InputOggFile = fd.FileName;

            var fileName = Path.ChangeExtension(fd.FileName, null);
            fileName += "_fixed";
            fileName = Path.ChangeExtension(fileName, "ogg");
            OutputOggFile = fileName;
        }

        private void oggConvertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputOggFile)) return;
            if (string.IsNullOrEmpty(OutputOggFile)) return;

            try
            {
                var oggFile = new OggFile();
                var isOggFileValid = oggFile.LoadOgg(InputOggFile);

                if (!isOggFileValid)
                {
                    MessageBox.Show("The selected input OGG file is not a valid WWise 2010.3.3 OGG file.", "OGG Conversion Process");
                    return;
                }

                oggFile.WriteOgg(OutputOggFile);
                MessageBox.Show("Conversion complete!", "OGG Conversion Process");
            }
            catch (Exception ex)
            {
                ErrorDialog.Show("Conversion has failed.", ex);
            }
        }
    }
}
