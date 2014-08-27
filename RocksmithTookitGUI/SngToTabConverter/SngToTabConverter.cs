using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using RocksmithToolkitLib;
using RocksmithToolkitLib.SngToTab;
using RocksmithToolkitLib.Song2014ToTab;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitGUI.SngToTabConverter
{
    public partial class SngToTabConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "SNG 2 Tab Converter";
        private string outputDir;
        private bool allDif;
        private IList<SongInfo> songList;

        public SngToTabConverter()
        {
            InitializeComponent();

            if (Directory.Exists(ConfigRepository.Instance()["general_rs2014path"]))
                outputDir = Path.Combine(ConfigRepository.Instance()["general_rs2014path"], "dlc");
            else
                outputDir = Path.GetDirectoryName(Application.ExecutablePath);
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            IList<string> sourceFilePaths;
            allDif = difficultyAll.Checked;

            // Input file(s)
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Title = "Select RS1 and/or RS2014 CDLC files to convert";
                ofd.Filter = "RS1 (*.dat, *.sng, *.xml) or RS2014 (*.psarc) files|*.dat;*.sng;*.xml;*.psarc";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFilePaths = ofd.FileNames;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = outputDir;
                // fbd.SelectedPath = "D:\\Temp"; // for testing
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                outputDir = fbd.SelectedPath;
            }

            Cursor.Current = Cursors.WaitCursor;
            foreach (string inputFilePath in sourceFilePaths)
            {
                string fileExtension = Path.GetExtension(inputFilePath).ToLower();

                switch (fileExtension)
                {
                    case ".xml":
                        var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                        var splitPoint = fileName.LastIndexOf('_');
                        var arrangement = fileName.Substring(splitPoint + 1);
                        // exclude files for vocals and showlights 
                        if (arrangement.ToLower() == "vocals" || arrangement.ToLower() == "showlights")
                        {
                            MessageBox.Show(inputFilePath + Environment.NewLine + Environment.NewLine +
                                            "Conversion not supported at this time!", MESSAGEBOX_CAPTION,
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        }

                        Song rs1Song;
                        using (var obj = new Rs1Converter())
                            rs1Song = obj.XmlToSong(inputFilePath);

                        string sngFilePath;
                        using (var obj = new Rs1Converter())
                            sngFilePath = obj.SongToSngFilePath(rs1Song, outputDir);

                        using (var obj = new Sng2Tab())
                            obj.Convert(sngFilePath, outputDir, allDif);

                        if (File.Exists(sngFilePath)) File.Delete(sngFilePath);
                        break;

                    case ".dat":
                        using (var obj = new Sng2Tab())
                            obj.ExtractBeforeConvert(inputFilePath, outputDir, allDif);
                        break;

                    case ".sng":
                        using (var obj = new Sng2Tab())
                            obj.Convert(inputFilePath, outputDir, allDif);
                        break;

                    case ".psarc":
                        if (rbSongList.Checked)
                            using (var obj = new Rs2014Converter())
                                obj.PsarcSongList(inputFilePath, outputDir);

                        else if (rbAsciiTab.Checked)
                        {
                            var fileInfo = new FileInfo(inputFilePath);
                            if (fileInfo.Length / 1000 > 15000)
                            {
                                if (MessageBox.Show(Path.GetFileName(inputFilePath) + " file size is " +
                                    (fileInfo.Length / 1000).ToString("N00") + " KB" + Environment.NewLine +
                                    "It may take a long time to extract and convert that much data." +
                                     Environment.NewLine + Environment.NewLine + "Do you want to continue?", MESSAGEBOX_CAPTION,
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) return;
                            }

                            using (var obj = new Rs2014Converter())
                                songList = obj.PsarcSongList(inputFilePath);

                            if (songList.Count > 0) // popup selection list
                            {
                                using (var form = new SongListForm())
                                {
                                    form.PopSongListBox(songList);

                                    do // waiting for user selection
                                        form.ShowDialog();
                                    while (form.SongListShort == null);

                                    this.Refresh();
                                    Cursor.Current = Cursors.WaitCursor;

                                    foreach (var song in form.SongListShort)
                                    {
                                        Song2014 rs2014Song;
                                        using (var obj = new Rs2014Converter())
                                            rs2014Song = obj.PsarcToSong2014(inputFilePath, song.Identifier, song.Arrangement);

                                        using (var obj = new Rs2014Converter())
                                            obj.Song2014ToAsciiTab(rs2014Song, outputDir, allDif);
                                    }
                                }
                            }
                            else
                            {
                                // convert all songs and arrangements by memory method
                                using (var obj = new Rs2014Converter())
                                    obj.PsarcToAsciiTab(inputFilePath, outputDir, allDif);

                                // covert all songs and arrangements by unpacking method
                                // using (var obj = new Rs2014Converter())
                                //    obj.ExtractBeforeConvert(inputFilePath, outputDir, allDif);
                            }
                        }

                        break;
                }

                if (!rbAsciiTab.Checked && fileExtension != ".psarc")
                {
                    MessageBox.Show("Only ASCII Tab is supported for RS1 CDLC.", MESSAGEBOX_CAPTION,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }

            Cursor.Current = Cursors.Default;

            if (MessageBox.Show("The conversion is complete.." + Environment.NewLine +
                "Would you like to open the folder?", MESSAGEBOX_CAPTION,
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                Process.Start(outputDir);
        }




    }
}
