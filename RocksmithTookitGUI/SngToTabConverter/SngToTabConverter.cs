using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using RocksmithToolkitLib.SngToTab;
using RocksmithToolkitLib.Song2014ToTab;
using RocksmithToolkitLib.Xml;


namespace RocksmithToolkitGUI.SngToTabConverter
{
    public partial class SngToTabConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "SNG 2 Tab Converter";

        public SngToTabConverter()
        {
            InitializeComponent();
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            IList<string> sourceFilePaths;
            string outputDir;
            bool allDif = difficultyAll.Checked;

            // Input file(s)
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Title = "Select a CDLC file to convert";
                ofd.Filter = "RS1 (*.dat, *.sng, *.xml) or RS2014 (*.psarc) files|*.dat;*.sng;*.xml;*.psarc";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFilePaths = ofd.FileNames;
            }

            // Output Dir
            using (var fbd = new FolderBrowserDialog())
            {
                // fbd.SelectedPath = "d:\\temp"; // for debugging
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
                        {
                            using (var obj = new Rs2014Converter())
                                obj.PsarcSongList(inputFilePath, outputDir, true);
                        }
                        else if (rbAsciiTab.Checked)
                        {
                            int songCount = 0;
                            using (var obj = new Rs2014Converter())
                                songCount = obj.PsarcSongList(inputFilePath, outputDir, false);

                            if (songCount > 10)  // use unpack method
                            {
                                if (MessageBox.Show("This archive contains " + songCount + " songs." + Environment.NewLine +
    "It may take a long time to extract and convert this data." + Environment.NewLine +
    "Do you want to continue?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) return;

                                using (var obj = new Rs2014Converter())
                                    obj.ExtractBeforeConvert(inputFilePath, outputDir, allDif);
                            }
                            else  // use memory method
                            {
                                using (var obj = new Rs2014Converter())
                                    obj.PsarcAllToSong2014(inputFilePath, outputDir, allDif);
                            }
                        }
                        break;
                }

                if (!rbAsciiTab.Checked && fileExtension != ".psarc")
                {
                    MessageBox.Show("Only ASCII Tab is supported for RS1 CDLC.", MESSAGEBOX_CAPTION,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    rbAsciiTab.Checked = true;
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
