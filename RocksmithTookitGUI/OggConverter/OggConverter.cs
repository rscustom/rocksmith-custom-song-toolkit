using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Ogg;
using System.Diagnostics;

namespace RocksmithTookitGUI.OggConverter
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

        private string[] InputOggFiles;

        private void oggBrowseButton_Click(object sender, EventArgs e)
        {
            Converter(inputOggTextBox, ConverterType.HeaderFix);
        }

        private void oggRocksmithBrowseButton_Click(object sender, EventArgs e) {
            Converter(inputOggTextBox, ConverterType.Revorb);
        }

        private void Converter(TextBox control, ConverterType converterType) {
            InputOggFiles = null;

            using (var fd = new OpenFileDialog()) {
                fd.Filter = "Wwise 2010.3.3 OGG files|*.ogg";
                fd.Multiselect = true;
                fd.ShowDialog();
                if (fd.FileNames.Count() <= 0) {
                    MessageBox.Show("The selected directory has no .ogg file inside!", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string path = Path.GetDirectoryName(fd.FileName);

                switch (converterType) {
                    case ConverterType.HeaderFix:
                        inputOggTextBox.Text = path;
                        break;
                    case ConverterType.Revorb:
                        inputOggRocksmithTextBox.Text = path;
                        break;
                }

                InputOggFiles = fd.FileNames;
                Dictionary<string, string> errorFiles = new Dictionary<string, string>();
                List<string> successFiles = new List<string>();

                foreach (var file in InputOggFiles) {
                    try {
                        var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
                        switch (converterType) {
                            case ConverterType.HeaderFix:
                                OggFile.ConvertOgg(file, outputFileName);
                                break;
                            case ConverterType.Revorb:
                                Revorb(file, outputFileName);
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
                    StringBuilder alertMessage = new StringBuilder("Conversion complete with errors." + Environment.NewLine + Environment.NewLine);
                    alertMessage.AppendLine("Files converted with success:" + Environment.NewLine);
                    foreach (var sFile in successFiles)
                        alertMessage.AppendLine(String.Format("File: {0}", sFile));
                    alertMessage.AppendLine("Files converted with error:" + Environment.NewLine);
                    foreach (var eFile in errorFiles)
                        alertMessage.AppendLine(String.Format("File: {0}; error: {1}", eFile.Key, eFile.Value));

                    MessageBox.Show(alertMessage.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } else {
                    StringBuilder alertMessage = new StringBuilder("Conversion complete with errors." + Environment.NewLine);
                    alertMessage.AppendLine("Files converted with error: " + Environment.NewLine);
                    foreach (var eFile in errorFiles)
                        alertMessage.AppendLine(String.Format("File: {0}, error: {1}", eFile.Key, eFile.Value));

                    MessageBox.Show(alertMessage.ToString(), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Revorb(string file, string outputFileName) {
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            string ww2oggPath = Path.Combine(appPath, "ww2ogg.exe");
            string revorbPath = Path.Combine(appPath, "revorb.exe");
            string codebooksPath = Path.Combine(appPath, "packed_codebooks.bin");

            // Verifying if third part apps is in root application directory
            if (!File.Exists(ww2oggPath))
                throw new FileNotFoundException("ww2ogg executable not found!");

            if (!File.Exists(revorbPath))
                throw new FileNotFoundException("revorb executable not found!");

            if (!File.Exists(codebooksPath))
                throw new FileNotFoundException("packed_codebooks.bin not found!");

            // Processing with ww2ogg
            Process ww2oggProcess = new Process();
            ww2oggProcess.StartInfo.FileName = ww2oggPath;
            ww2oggProcess.StartInfo.Arguments = String.Format("{0} -o {1}", file, outputFileName);
            ww2oggProcess.StartInfo.UseShellExecute = false;
            ww2oggProcess.StartInfo.CreateNoWindow = true;
            ww2oggProcess.StartInfo.RedirectStandardOutput = true;

            ww2oggProcess.Start();
            ww2oggProcess.WaitForExit();
            string ww2oggResult = ww2oggProcess.StandardOutput.ReadToEnd();

            if (ww2oggResult.IndexOf("error") > -1)
                throw new Exception("ww2ogg process error." + Environment.NewLine + ww2oggResult);

            // Processing with revorb
            Process revorbProcess = new Process();
            revorbProcess.StartInfo.FileName = revorbPath;
            //revorbProcess.StartInfo.Arguments = String.Format("{0}", outputFileName);
            revorbProcess.StartInfo.UseShellExecute = false;
            revorbProcess.StartInfo.CreateNoWindow = true;
            revorbProcess.StartInfo.RedirectStandardOutput = true;

            revorbProcess.Start();
            revorbProcess.WaitForExit();
            string revorbResult = revorbProcess.StandardOutput.ReadToEnd();

            if (ww2oggResult.IndexOf("error") > -1) {
                if (File.Exists(outputFileName))
                    File.Delete(outputFileName);
                
                throw new Exception("revorb process error." + Environment.NewLine + revorbResult);
            }
        }
    }
}
