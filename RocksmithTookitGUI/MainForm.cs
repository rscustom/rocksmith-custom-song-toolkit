using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using RocksmithSngCreator;
using RocksmithEncoder;
using RocksmithDLCCreator;
using RocksmithDLCPackager;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography;


namespace RocksmithTookitGUI
{
    public partial class MainForm : Form
    {        
        OggFile oggFile;
        private readonly Packer packer;
        private RijndaelManaged rij;

        public MainForm()
        {
            InitializeComponent();
            InitializeDLCComponent();

            packer = new Packer();

            this.Text = String.Format("Custom Song Creator Toolkit (v{0}.{1} alpha)",
                Assembly.GetExecutingAssembly().GetName().Version.Major,
                Assembly.GetExecutingAssembly().GetName().Version.Minor);
        }

        #region Common Toolkit

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm a = new AboutForm();
            a.ShowDialog();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm h = new HelpForm();
            h.ShowDialog();
        }

        #endregion

        #region SNG File Creator Tab

        private void xmlBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
            fd.FilterIndex = 1;
            fd.ShowDialog();
            if (!string.IsNullOrEmpty(fd.FileName))
            {
                inputXmlTextBox.Text = fd.FileName;
                outputFileTextBox.Text = inputXmlTextBox.Text.Substring(0, inputXmlTextBox.Text.Length - 4) + ".sng";
            }
        }

        private void sngConvertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputXmlTextBox.Text))
                return;

            SngFileWriter sngFileWriter;

            // platform selection
            if (littleEndianRadioBtn.Checked)
            {
                sngFileWriter = new SngFileWriter(GamePlatform.PC);
            }
            else
            {
                sngFileWriter = new SngFileWriter(GamePlatform.XBOX);
            }

            try
            {
                // song or vocal chart output
                if (vocalsRadioButton.Checked)
                {
                    sngFileWriter.WriteRocksmithVocalChart(inputXmlTextBox.Text, outputFileTextBox.Text);
                }
                else
                {
                    sngFileWriter.WriteRocksmithSongChart(inputXmlTextBox.Text, outputFileTextBox.Text);
                }

                // done
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

        #endregion

        #region OGG Converter Tab        

        private void oggBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "OGG Files|*.ogg|All Files (*.*)|*.*";
            fd.FilterIndex = 1;
            fd.ShowDialog();
            if (!string.IsNullOrEmpty(fd.FileName))
            {
                inputOggTextBox.Text = fd.FileName;
                outputOggTextBox.Text = inputOggTextBox.Text.Substring(0, inputOggTextBox.Text.LastIndexOf(".")) + "_fixed.ogg";

                // Make sure we have a valid file
                oggFile = new OggFile();
                if (oggFile.LoadOgg(inputXmlTextBox.Text))
                    oggConvertButton.Enabled = true;
                else
                    oggConvertButton.Enabled = false;

                // Determine endianness right away, this doesn't matter and is purely visual
                if (oggFile.bigEndian)
                    oggBigEndianRadioBtn.Checked = true;
                else
                    oggLittleEndianRadioBtn.Checked = true;
            }
        }

        private void oggConvertButton_Click(object sender, EventArgs e)
        {
            oggFile.WriteOgg(outputOggTextBox.Text);

            // done
            MessageBox.Show("Process Complete", "File Creation Process");
        }

        #endregion

        #region DLC Package Creator

        private void InitializeDLCComponent()
        {
            rij = new RijndaelManaged
            {
                Padding = PaddingMode.None,
                Mode = CipherMode.ECB,
                BlockSize = 128,
                IV = new byte[16],
                Key = new byte[32]
                {
                    0xFA, 0x6F, 0x4F, 0x42, 0x3E, 0x66, 0x9F, 0x9E,
                    0x6A, 0xD2, 0x3A, 0x2F, 0x8F, 0xE5, 0x81, 0x88,
                    0x63, 0xD9, 0xB8, 0xFD, 0xED, 0xDF, 0xFE, 0xBD,
                    0x12, 0xB2, 0x7F, 0x76, 0x80, 0xD1, 0x51, 0x41
                }
            };
        }

        private static void Crypto(Stream input, Stream output, ICryptoTransform transform)
        {
            var buffer = new byte[1024];
            var cs = new CryptoStream(output, transform, CryptoStreamMode.Write);
            for (long i = 0; i < input.Length; i += buffer.Length)
            {
                int sz = (int)Math.Min(input.Length - input.Position, buffer.Length);
                input.Read(buffer, 0, sz);
                cs.Write(buffer, 0, sz);
            }
            int pad = buffer.Length - (int)(input.Length % buffer.Length);
            if (pad > 0)
                cs.Write(new byte[pad], 0, pad);
            cs.Flush();
            output.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }

        private void arrangementAddButton_Click(object sender, EventArgs e)
        {
            var form = new ArrangementForm();
            form.ShowDialog();
            var arrangement = form.Arragement;
            if (arrangement == null)
                return;
            arrangement.Artist = ArtistTB.Text;
            arrangement.SongDisplayName = SongDisplayNameTB.Text;
            arrangement.SongYear = Convert.ToInt32(YearTB.Text);
            ArrangementLB.Items.Add(arrangement);
        }

        private void arrangementRemoveButton_Click(object sender, EventArgs e)
        {
            if (ArrangementLB.SelectedItem != null)
                ArrangementLB.Items.Remove(ArrangementLB.SelectedItem);
        }

        private string OggPath { get { return OggPathTB.Text; } set { OggPathTB.Text = value; } }

        private void openOggButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Fixed WWise Files|*.ogg";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OggPath = ofd.FileName;
        }

        private void dlcGenerateButton_Click(object sender, EventArgs e)
        {
            var arrangements = new List<Arrangement>();
            foreach (var x in ArrangementLB.Items)
                arrangements.Add(x as Arrangement);
            int vocalCount = 0;
            foreach (var x in arrangements)
                if (x.IsVocal)
                    vocalCount++;
            if (vocalCount == 0)
            {
                MessageBox.Show("Error: Needs 1(ONE) Vocal");
                return;
            }
            if (vocalCount > 1)
            {
                MessageBox.Show("Error: Multiple Vocals Found");
                return;
            }
            var ofd = new SaveFileDialog();
            ofd.Filter = "Rocksmith Dlc|*.dat";
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            var dlcSavePath = ofd.FileName;
            var dlcName = DlcNameTB.Text.Replace(" ", "_");
            var soundBankName = String.Format("Song_{0}", dlcName);
            var songPsarc = new PSARC.PSARC();
            var packPsarc = new PSARC.PSARC();
            var appID = new MemoryStream();
            var writer = new StreamWriter(appID);
            writer.Write("206113");
            writer.Flush();
            appID.Seek(0, SeekOrigin.Begin);
            var packageList = new MemoryStream();
            writer = new StreamWriter(packageList);
            writer.WriteLine(dlcName);
            writer.WriteLine("DLC_Tone_{0}", dlcName);
            //writer.WriteLine("DLC_Tone_DammitClean_Bass");
            writer.Flush();
            packageList.Seek(0, SeekOrigin.Begin);
            packPsarc.AddEntry("APP_ID", appID);
            packPsarc.AddEntry("PackageList.txt", packageList);

            ManifestBuilder builder = new ManifestBuilder();
            builder.AlbumName = AlbumTB.Text;
            builder.AggregateGraph = new AggregateGraph();
            builder.AggregateGraph.SoundBank = new SoundBank() { File = soundBankName + ".bnk" };
            builder.AggregateGraph.AlbumArt = new AlbumArt() { File = AlbumArtPath };

            foreach (var x in arrangements)
            {
                builder.AggregateGraph.SongFiles.Add(x.SongFile);
                builder.AggregateGraph.SongXMLs.Add(x.SongXml);
            }
            builder.AggregateGraph.XBlock = new XBlockFile() { File = dlcName + ".xblock" };

            var aggregateGraph = new MemoryStream();
            builder.AggregateGraph.Write(dlcName, aggregateGraph);
            aggregateGraph.Flush();
            aggregateGraph.Seek(0, SeekOrigin.Begin);

            var manifest = new MemoryStream();
            var manifestData = builder.GenerateManifest(dlcName, arrangements);
            writer = new StreamWriter(manifest);
            writer.Write(manifestData);
            writer.Flush();
            manifest.Seek(0, SeekOrigin.Begin);

            var xblock = new MemoryStream();
            XBlockGenerator.Generate(dlcName, builder.Manifest, builder.AggregateGraph, xblock);
            xblock.Flush();
            xblock.Seek(0, SeekOrigin.Begin);

            string soundFileName;
            var soundbank = new MemoryStream();
            soundFileName = SoundBankGenerator.GenerateSoundBank("Sound_" + dlcName, File.OpenRead(OggPath), soundbank);
            soundbank.Flush();
            soundbank.Seek(0, SeekOrigin.Begin);

            var packageId = new MemoryStream();
            writer = new StreamWriter(packageId);
            writer.Write(dlcName);
            writer.Flush();
            packageId.Seek(0, SeekOrigin.Begin);

            songPsarc.AddEntry("PACKAGE_ID", packageId);
            songPsarc.AddEntry("AggregateGraph.nt", aggregateGraph);
            songPsarc.AddEntry("Manifests/songs.manifest.json", manifest);
            songPsarc.AddEntry(String.Format("Exports/Songs/{0}.xblock", dlcName), xblock);
            songPsarc.AddEntry(String.Format("Audio/Windows/{0}.bnk", soundBankName), soundbank);
            songPsarc.AddEntry(String.Format("Audio/Windows/{0}.ogg", soundFileName), File.OpenRead(OggPath));
            songPsarc.AddEntry(String.Format("GRAssets/AlbumArt/{0}.dds", builder.AggregateGraph.AlbumArt.Name), File.OpenRead(AlbumArtPath));
            foreach (var x in arrangements)
            {
                songPsarc.AddEntry(String.Format("GR/Behaviors/Songs/{0}.xml", Path.GetFileNameWithoutExtension(x.SongXml.File)), File.OpenRead(x.SongXml.File));
                songPsarc.AddEntry(String.Format("GRExports/Generic/{0}.sng", Path.GetFileNameWithoutExtension(x.SongFile.File)), File.OpenRead(x.SongFile.File));
            }

            var pack = new MemoryStream();
            songPsarc.Write(pack);
            pack.Flush();
            pack.Seek(0, SeekOrigin.Begin);
            packPsarc.AddEntry(String.Format("{0}.psarc", dlcName), pack);

            var toneManifestStream = new MemoryStream();
            var toneXblockStream = new MemoryStream();
            var toneAggregateGraph = new MemoryStream();
            var packIdStr = new MemoryStream();
            writer = new StreamWriter(packIdStr);
            writer.WriteLine("DLC_Tone_{0}", dlcName);
            writer.Flush();
            packIdStr.Seek(0, SeekOrigin.Begin);
            ToneGenerator.Generate(dlcName, toneManifestStream, toneXblockStream, toneAggregateGraph);
            var tonePsarc = new PSARC.PSARC();
            tonePsarc.AddEntry(String.Format("Exports/Pedals/DLC_Tone_{0}", dlcName), toneXblockStream);
            tonePsarc.AddEntry("Manifests/tone.manifest.json", toneManifestStream);
            tonePsarc.AddEntry("AggregateGraph.nt", toneAggregateGraph);
            tonePsarc.AddEntry("PACKAGE_ID", packIdStr);
            var toneStr = new MemoryStream();
            tonePsarc.Write(toneStr);
            packPsarc.AddEntry(String.Format("DLC_Tone_{0}.psarc", dlcName), toneStr);

            //packPsarc.AddEntry("DLC_Tone_DammitClean_Bass.psarc", File.OpenRead("DLC_Tone_DammitClean_Bass.psarc"));
            /*using (var fl = File.Create("test.psarc"))
                packPsarc.Write(fl);*/

            var finishedPack = new MemoryStream();
            packPsarc.Write(finishedPack);
            finishedPack.Flush();
            finishedPack.Seek(0, SeekOrigin.Begin);
            using (var fl = File.Create(dlcSavePath))
                Crypto(finishedPack, fl, rij.CreateEncryptor());

        }

        private string AlbumArtPath { get { return AlbumArtPathTB.Text; } set { AlbumArtPathTB.Text = value; } }

        private void albumArtButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "dds Files|*.dds";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                AlbumArtPath = ofd.FileName;
        }

        #endregion

        #region Pack/Unpack Tool

        private void PackButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            var sourcePath = fbd.SelectedPath;
            var saveFileName = sfd.FileName;
            var useCryptography = UseCryptographyCheckbox.Checked;

            packer.Pack(sourcePath, saveFileName, useCryptography);
        }

        private void UnpackButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            var sourceFileName = ofd.FileName;
            var savePath = fbd.SelectedPath;
            var useCryptography = UseCryptographyCheckbox.Checked;

            packer.Unpack(sourceFileName, savePath, useCryptography);
        }

        #endregion
    }
}
