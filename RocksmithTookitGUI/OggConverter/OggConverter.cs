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
        private const string MESSAGEBOX_CAPTION = "OGG Conversion Process";

        public OggConverter()
        {
            InitializeComponent();
        }

        private string[] InputOggFiles;

        private string InputOggFolder
        {
            get { return inputOggTextBox.Text; }
            set { inputOggTextBox.Text = value; }
        }

        private void oggBrowseButton_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "Wwise 2010.3.3 OGG Files|*.ogg";
                fd.Multiselect = true;
                fd.ShowDialog();
                if (fd.FileNames.Count() <= 0) {
                    MessageBox.Show("The selected directory has no .ogg file inside!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                InputOggFolder = Path.GetDirectoryName(fd.FileName);
                InputOggFiles = fd.FileNames;
                Dictionary<string, string> errorFiles = new Dictionary<string, string>();
                List<string> successFiles = new List<string>();

                foreach (var file in InputOggFiles) {
                    try
                    {
                        var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                        OggFile.ConvertOgg(file, outputFileName);
                        successFiles.Add(file);
                    }
                    catch (Exception ex)
                    {
                        errorFiles.Add(file, ex.Message);
                    }
                }

                if (errorFiles.Count <= 0 && successFiles.Count > 0)
                    MessageBox.Show("Conversion complete!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (errorFiles.Count > 0 && successFiles.Count > 0)
                {
                    StringBuilder alertMessage = new StringBuilder("Conversion complete with errors!");
                    alertMessage.AppendLine("Files converted with success:");
                    foreach (var sFile in successFiles)
                        alertMessage.AppendLine(String.Format("File: {0}", sFile));
                    foreach (var eFile in errorFiles)
                        alertMessage.AppendLine(String.Format("File: {0}; error: {1}", eFile.Key, eFile.Value));

                    MessageBox.Show(alertMessage.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("Problem ocurred! Check if file(s) is valid!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
