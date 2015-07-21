using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage.Manifest2014;
using RocksmithToolkitLib.Ogg;
using System.Diagnostics;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;
using System.Xml.Linq;
using System.Collections;
using System.Threading;

namespace RocksmithToolkitGUI.SngConverter
{
    public partial class SngConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "SNG Converter";

        private Platform PackerPlatform {
            get {
                if (platformCombo.Items.Count > 0)
                    return new Platform(platformCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                else
                    return new Platform(GamePlatform.None, GameVersion.None);
            }
        }

        private Platform ConverterPlatform {
            get {
                if (platform2Combo.Items.Count > 0)
                    return new Platform(platform2Combo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                else
                    return new Platform(GamePlatform.None, GameVersion.None);
            }
        }

        private string ConverterManifestFile {
            get { return manifestTB.Text; }
            set { manifestTB.Text = value; }
        }

        private string ConverterSngXmlFile {
            get { return sngXmlTB.Text; }
            set { sngXmlTB.Text = value; }
        }

        private ArrangementType ConverterArrangementType {
            get {
                var arrType = ArrangementType.Guitar;

                if (vocalRadio.Checked)
                    arrType = ArrangementType.Vocal;

                return arrType;
            }
        }

        private string CurrentConverterFilter {
            get {
                var filter = "SNG or XML song file (*.sng,*.xml)|*.sng,*.xml";

                if (sng2xmlRadio.Checked)
                    filter = "SNG song file (*.sng)|*.sng";
                else if (xml2sngRadio.Checked)
                    filter = "XML song file (*.xml)|*.xml";

                return filter;
            }
        }

        public SngConverter()
        {
            InitializeComponent();
            try {
                populatePlatformCombo(platformCombo);
                populatePlatformCombo(platform2Combo);
            } catch { /*For mono compatibility*/ }
        }

        private void populatePlatformCombo(ComboBox combo) {
            var platformList = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
            platformList.Remove("None");
            combo.DataSource = platformList;
            combo.SelectedIndex = 0; //Pc
        }

        private void packUnpackButton_Click(object sender, EventArgs e) {
            List<string> badFiles = new List<string>();
            IList<string> sourceFileNames;
            using (var ofd = new OpenFileDialog()) {
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileNames = ofd.FileNames;
            }

            var errorsFound = new StringBuilder();
            var message = (unpackRadio.Checked) ? "decrypted" : "encrypted";
            foreach (string sourceFileName in sourceFileNames) {
                Application.DoEvents();

                var outputFile = Path.Combine(Path.GetDirectoryName(sourceFileName), String.Format("{0}_{1}.sng", Path.GetFileNameWithoutExtension(sourceFileName), message));
                try {
                    // Pack/Unpack SNG
                    using (FileStream inputStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
                    using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite)) {
                        if (packRadio.Checked)
                            Sng2014File.PackSng(inputStream, outputStream, PackerPlatform);
                        else if (unpackRadio.Checked)
                            Sng2014File.UnpackSng(inputStream, outputStream, PackerPlatform);
                    }
                }/* catch (FileNotFoundException ex) {
                    errorsFound.AppendLine(ex.Message);
                } catch (DirectoryNotFoundException ex) {
                    errorsFound.AppendLine(ex.Message);
                }*/catch (Exception ex) {
                    errorsFound.AppendLine(ex.Message);
                    badFiles.Add(outputFile);
                }
            }
            if (badFiles.Count > 0)
                foreach (var trash in badFiles) {
                    try {
                        File.Delete(trash);
                    } catch { //Do nothing
                    }
                }

            if (errorsFound.Length <= 0)
                MessageBox.Show(String.Format("File(s) was {0}.", message), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(String.Format("File(s) was {0} with errors. See below: {1}{2}", message, errorsFound.ToString(), Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void browseManifestButton_Click(object sender, EventArgs e) {
            using (var ofd = new OpenFileDialog()) {
                ofd.Filter = "Manifest file (*.json)|*.json";
                if (ofd.ShowDialog() == DialogResult.OK) {
                    ConverterManifestFile = ofd.FileName;
                }
            }
        }

        private void browseSngXmlButton_Click(object sender, EventArgs e) {
            using (var ofd = new OpenFileDialog()) {
                ofd.Filter = CurrentConverterFilter;
                if (ofd.ShowDialog() == DialogResult.OK) {
                    ConverterSngXmlFile = ofd.FileName;
                }
            }
        }

        private void convertSngXmlButton_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(ConverterSngXmlFile)) {
                MessageBox.Show(String.Format("File not found: {0}: ", ConverterSngXmlFile), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                sngXmlTB.Focus();
                return;
            }

            if (sng2xmlRadio.Checked) {
                if (String.IsNullOrEmpty(ConverterManifestFile))
                    MessageBox.Show("No manifest file was entered. The song xml file will be generated without song informations like song title, album, artist, tone names, etc.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Attributes2014 att = null;
                if (ConverterArrangementType != ArrangementType.Vocal && !String.IsNullOrEmpty(ConverterManifestFile))
                    att = Manifest2014<Attributes2014>.LoadFromFile(ConverterManifestFile).Entries.ToArray()[0].Value.ToArray()[0].Value;

                var sng = Sng2014File.LoadFromFile(ConverterSngXmlFile, ConverterPlatform);

                var outputFile = Path.Combine(Path.GetDirectoryName(ConverterSngXmlFile), String.Format("{0}.xml", Path.GetFileNameWithoutExtension(ConverterSngXmlFile)));
                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    dynamic xml = null;

                    if (ConverterArrangementType == ArrangementType.Vocal)
                        xml = new Vocals(sng);
                    else
                        xml = new Song2014(sng, att ?? null);

                    xml.Serialize(outputStream);

                    MessageBox.Show(String.Format("XML file was generated! {0}It was saved on same location of sng file specified.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } else if (xml2sngRadio.Checked) {
                var outputFile = Path.Combine(Path.GetDirectoryName(ConverterSngXmlFile), String.Format("{0}.sng", Path.GetFileNameWithoutExtension(ConverterSngXmlFile)));

                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite)) {
                    Sng2014File sng = Sng2014File.ConvertXML(ConverterSngXmlFile, ConverterArrangementType);
                    sng.WriteSng(outputStream, ConverterPlatform);
                }

                MessageBox.Show(String.Format("SNG file was generated! {0}It was saved on same location of xml file specified.", Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void sng2xmlRadio_CheckedChanged(object sender, EventArgs e) {
            ConverterManifestFile = "";
            ConverterSngXmlFile = "";

            var radio = ((RadioButton)sender);

            if (radio.Text.ToLower().StartsWith("sng")) {
                sngXmlTB.Cue = "Sng file to convert";
                manifestTB.Enabled = true;
                browseManifestButton.Enabled = true;
            } else if (radio.Text.ToLower().StartsWith("xml")) {
                sngXmlTB.Cue = "Xml file to convert";
                manifestTB.Enabled = false;
                browseManifestButton.Enabled = false;
            }
        }
    }
}
