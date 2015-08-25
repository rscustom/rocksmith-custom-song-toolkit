using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Ogg;


namespace RocksmithToolkitGUI.OggConverter
{
    public partial class OggConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "OGG Conversion Process";

        public OggConverter()
        {
            InitializeComponent();
        }

        private enum ConverterType
        {
            HeaderFix,
            Revorb,
            WEM,
            Ogg2Wem
        }

        private string[] InputAudioFiles;

        private void btnOgg2FixHdr_Click(object sender, EventArgs e)
        {
            Converter(txtOgg2FixHdr, ConverterType.HeaderFix);
        }

        private void btnWwise2Ogg_Click(object sender, EventArgs e)
        {
            Converter(txtWwise2Ogg, ConverterType.Revorb);
        }

        private void btnWwiseConvert_Click(object sender, EventArgs e)
        {
            Converter(txtWwiseConvert, ConverterType.WEM);
        }

        private void btnOgg2Wem_Click(object sender, EventArgs e)
        {
            Converter(txtAudio2Wem, ConverterType.Ogg2Wem);
        }

        private void Converter(TextBox control, ConverterType converterType)
        {
            InputAudioFiles = null;
            txtOgg2FixHdr.Text = String.Empty;
            txtWwiseConvert.Text = String.Empty;
            txtWwise2Ogg.Text = String.Empty;
            txtAudio2Wem.Text = String.Empty;

            using (var fd = new OpenFileDialog())
            {
                fd.Multiselect = true;
                fd.Filter = "Wwise 2010.3.3 OGG files (*.ogg)|*.ogg";
                if (converterType == ConverterType.Revorb || converterType == ConverterType.WEM)
                    fd.Filter += "|Wwise 2013 WEM files (*.wem)|*.wem";
                else if (converterType == ConverterType.Ogg2Wem)
                    fd.Filter = "Vobis Ogg or Wave files (*.ogg, *.wav)|*.ogg; *.wav";

                fd.ShowDialog();
                if (!fd.FileNames.Any())
                    return;

                InputAudioFiles = fd.FileNames;
                Dictionary<string, string> errorFiles = new Dictionary<string, string>();
                List<string> successFiles = new List<string>();

                foreach (var file in InputAudioFiles)
                {
                    try
                    {
                        var extension = Path.GetExtension(file);
                        var outputFileName = Path.Combine(Path.GetDirectoryName(file), String.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(file), ".ogg"));
                        switch (converterType)
                        {
                            case ConverterType.HeaderFix:
                                txtOgg2FixHdr.Text = file;
                                using (FileStream fl = File.Create(outputFileName))
                                    OggFile.ConvertOgg(file).CopyTo(fl);
                                break;
                            case ConverterType.Revorb:
                                txtWwise2Ogg.Text = file;
                                OggFile.Revorb(file, outputFileName, Path.GetDirectoryName(Application.ExecutablePath), (extension == ".ogg") ? OggFile.WwiseVersion.Wwise2010 : OggFile.WwiseVersion.Wwise2013);
                                break;
                            case ConverterType.WEM:
                                txtWwiseConvert.Text = file;
                                outputFileName = Path.ChangeExtension(outputFileName, Path.GetExtension(file));
                                OggFile.ConvertAudioPlatform(file, outputFileName);
                                break;
                            case ConverterType.Ogg2Wem:
                                txtAudio2Wem.Text = file;
                                OggFile.Convert2Wem(file, (int)audioQualityBox.Value, (long)Convert.ToDouble(lblChorusTime.Text) * 1000);
                                break;
                        }

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
                }
                else
                {
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

        private void tbarChorusTime_ValueChanged(object sender, EventArgs e)
        {
            lblChorusTime.Text = String.Format("{0:0.0}", Math.Truncate((decimal)tbarChorusTime.Value) / 10);
            // removes focus ring from tbarChorusTime
            groupBox4.Focus();
        }

    }
}
