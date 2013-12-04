using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Ogg;
using System.Diagnostics;
using RocksmithToolkitLib.Sng;

namespace RocksmithToolkitGUI.OggConverter
{
    public partial class OggConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "OGG Conversion Process";

        public OggConverter()
        {
            InitializeComponent();
        }

        private enum ConverterType {
            HeaderFix,
            Revorb
        }

        private string[] InputAudioFiles;

        private void oggBrowseButton_Click(object sender, EventArgs e)
        {
            Converter(inputOggTextBox, ConverterType.HeaderFix);
        }

        private void oggRocksmithBrowseButton_Click(object sender, EventArgs e) {
            Converter(inputOggTextBox, ConverterType.Revorb);
        }

        private void Converter(TextBox control, ConverterType converterType) {
            InputAudioFiles = null;

            using (var fd = new OpenFileDialog()) {
                fd.Filter = "Wwise 2010.3.3 OGG files (*.ogg)|*.ogg";
                if (converterType == ConverterType.Revorb)
                    fd.Filter += "|Wwise 2013 WEM files (*.wem)|*.wem";

                fd.Multiselect = true;
                fd.ShowDialog();
                if (fd.FileNames.Count() <= 0) {
                    MessageBox.Show("The selected directory has no valid file inside!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string path = Path.GetDirectoryName(fd.FileName);

                switch (converterType) {
                    case ConverterType.HeaderFix:
                        inputOggTextBox.Text = path;
                        break;
                    case ConverterType.Revorb:
                        inputAudioRocksmithTextBox.Text = path;
                        break;
                }

                InputAudioFiles = fd.FileNames;
                Dictionary<string, string> errorFiles = new Dictionary<string, string>();
                List<string> successFiles = new List<string>();

                foreach (var file in InputAudioFiles) {
                    try {
                        var extension = Path.GetExtension(file);
                        var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), ".ogg"));
                        switch (converterType) {
                            case ConverterType.HeaderFix:
                                OggFile.ConvertOgg(file, outputFileName);
                                break;
                            case ConverterType.Revorb:
                                OggFile.Revorb(file, outputFileName, Path.GetDirectoryName(Application.ExecutablePath), (extension == ".ogg") ? OggFile.WwiseVersion.Wwise2010 : OggFile.WwiseVersion.Wwise2013);
                                break;
                        }
                        successFiles.Add(file);
                    } catch (Exception ex) {
                        errorFiles.Add(file, ex.Message);
                    }
                }

                if (errorFiles.Count <= 0 && successFiles.Count > 0)
                    MessageBox.Show("Conversion complete!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (errorFiles.Count > 0 && successFiles.Count > 0) {
                    StringBuilder alertMessage = new StringBuilder(
                        "Conversion complete with errors." + Environment.NewLine + Environment.NewLine);
                    alertMessage.AppendLine(
                        "Files converted with success:" + Environment.NewLine);

                    foreach (var sFile in successFiles)
                        alertMessage.AppendLine(String.Format("File: {0}", sFile));
                    alertMessage.AppendLine("Files converted with error:" + Environment.NewLine);
                    foreach (var eFile in errorFiles)
                        alertMessage.AppendLine(String.Format("File: {0}; error: {1}", eFile.Key, eFile.Value));

                    MessageBox.Show(alertMessage.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } else {
                    StringBuilder alertMessage = new StringBuilder(
                        "Conversion complete with errors." + Environment.NewLine);
                    alertMessage.AppendLine(
                        "Files converted with error: " + Environment.NewLine);
                    foreach (var eFile in errorFiles)
                        alertMessage.AppendLine(String.Format("File: {0}, error: {1}", eFile.Key, eFile.Value));

                    MessageBox.Show(alertMessage.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
