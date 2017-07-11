using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using RocksmithToTabLib;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Conversion;
using RocksmithToolkitLib.SngToTab;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitGUI.CDLC2Tab
{
    public partial class CDLC2Tab : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "CDLC 2 Tab Converter";
        private string outputDir;
        private bool allDif;
        private IList<SongInfo> songList;

        public CDLC2Tab()
        {
            // needs to come first
            InitializeComponent();
            try
            {
                if (!MainForm.IsInDesignMode)
                    InitOutputDir();
            }
            catch { /*For mono compatibility*/ }
        }

        private void InitOutputDir()
        {
            // set initial outputDir location
            if (Directory.Exists(ConfigRepository.Instance()["general_rs2014path"]))
                outputDir = Path.Combine(ConfigRepository.Instance()["general_rs2014path"], "dlc");
            else
                outputDir = Path.GetDirectoryName(Application.ExecutablePath);
        }

        private void OpenFileDialog_FileLimit(object sender, CancelEventArgs e)
        {
            OpenFileDialog dlg = sender as OpenFileDialog;
            if (dlg.FileNames.Length > 10)
            {
                MessageBox.Show("Exceeded conversion limit of 10 files per run.",
                     MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            IList<string> sourceFilePaths;
            allDif = difficultyAll.Checked;

            // Input file(s)
            using (var ofd = new OpenFileDialog())
            {
                if (rbAsciiTab.Checked)
                    ofd.Filter = "RS1 (*.dat, *.sng, *.xml) or RS2014 (*.psarc) files|*.dat;*.sng;*.xml;*.psarc";
                else
                    ofd.Filter = "RS2014 (*.psarc, *.xml) files|*.psarc;*.xml";

                ofd.Title = "Select RS1 and/or RS2014 CDLC files to convert";
                ofd.Multiselect = true;
                ofd.FileOk += OpenFileDialog_FileLimit; // Event handler

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFilePaths = ofd.FileNames;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a folder for storing converted files.";
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = outputDir;
                // fbd.SelectedPath = "D:\\Temp"; // for testing
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                outputDir = fbd.SelectedPath;
            }

            Cursor.Current = Cursors.WaitCursor;
            foreach (var inputFilePath in sourceFilePaths)
            {
                string fileExtension = Path.GetExtension(inputFilePath).ToLower();
                switch (fileExtension)
                {
                    case ".xml":
                        if (rbGp5.Checked)
                        {
                            using (var obj = new CDLC2Gp5())
                                obj.XmlToGp5(inputFilePath, outputDir);
                        }
                        else
                        {
                            var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                            var splitPoint = fileName.LastIndexOf('_');
                            var arrangement = fileName.Substring(splitPoint + 1);
                            // skip any files for vocals and/or showlights
                            if (arrangement.ToLower() == "vocals" || arrangement.ToLower() == "showlights")
                                break;
                            Song rs1Song;
                            using (var obj = new Rs1Converter())
                                rs1Song = obj.XmlToSong(inputFilePath);
                            string sngFilePath;
                            using (var obj = new Rs1Converter())
                                sngFilePath = obj.SongToSngFilePath(rs1Song, Path.Combine(outputDir, Path.GetFileName(inputFilePath)));
                            using (var obj = new Sng2Tab())
                                obj.Convert(sngFilePath, outputDir, allDif);
                            if (File.Exists(sngFilePath))
                                File.Delete(sngFilePath);
                        }
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
                            using (var obj = new CDLC2Gp5())
                                obj.PsarcSongList(inputFilePath, outputDir);
                            break;
                        }
                        var fileInfo = new FileInfo(inputFilePath);
                        // give user chance to abort big files
                        if (fileInfo.Length / 1000 > 15000)
                        {
                            if (MessageBox.Show(string.Format("{0} file size is {1:N00} KB{2}It may take a long time to extract and convert that much data.{2}{2}Do you want to continue?", Path.GetFileName(inputFilePath), (fileInfo.Length / 1000), Environment.NewLine), MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                                return;
                        }
                        using (var obj = new CDLC2Gp5())
                            songList = obj.PsarcSongList(inputFilePath);
                        if (rbAsciiTab.Checked)
                        {
                            using (var form = new SongInfoForm())
                            {
                                form.PopSongInfo(songList);
                                do
                                    // waiting for user selection(s)                               
                                    form.ShowDialog();
                                while (form.SongListShort.Count == 0);
                                this.Refresh();
                                if (form.SongListShort[0].Identifier == "User Aborted")
                                    break;
                                Cursor.Current = Cursors.WaitCursor;
                                foreach (var song in form.SongListShort)
                                {
                                    Song2014 rs2014Song;
                                    using (var obj = new CDLC2Gp5())
                                    {
                                        rs2014Song = obj.PsarcToSong2014(inputFilePath, song.Identifier, song.Arrangement);
                                        using (var obj2 = new Rs2014Converter())
                                            obj2.Song2014ToAsciiTab(rs2014Song, outputDir, allDif);
                                    }
                                }
                            }
                            break;
                        }
                        // convert to *.gp5 file(s) optimized code for dll usage
                        if (!allDif && songList.Count == 1)
                            using (var obj = new CDLC2Gp5())
                                obj.PsarcToGp5(inputFilePath, outputDir);
                        else
                            if (!allDif && songList.Count > 1)
                            {
                                using (var form = new SongInfoForm())
                                {
                                    form.PopSongOnly(songList);
                                    //  songs only (merge all arrangements into single GP file)
                                    //  form.PopSongInfo(songList); // choose songs and arrangements
                                    do
                                        // waiting for user selection(s)
                                        form.ShowDialog();
                                    while (form.SongListShort.Count == 0);
                                    this.Refresh();
                                    if (form.SongListShort[0].Identifier == "User Aborted")
                                        break;
                                    Cursor.Current = Cursors.WaitCursor;
                                    using (var obj = new CDLC2Gp5())
                                        obj.PsarcToGp5(inputFilePath, outputDir, form.SongListShort);
                                }
                            }
                            // give user the option to select specific songs and arrangements
                            else
                                if (allDif)
                                {
                                    using (var form = new SongInfoForm())
                                    {
                                        form.PopSongInfo(songList);
                                        // choose songs and arrangements
                                        do
                                            // waiting for user selection(s)
                                            form.ShowDialog();
                                        while (form.SongListShort.Count == 0);
                                        this.Refresh();
                                        if (form.SongListShort[0].Identifier == "User Aborted")
                                            break;
                                        Cursor.Current = Cursors.WaitCursor;
                                        using (var obj = new CDLC2Gp5())
                                            obj.PsarcToGp5(inputFilePath, outputDir, form.SongListShort, "gp5", true);
                                    }
                                }
                        break;
                }
                Refresh();
            }

            Cursor.Current = Cursors.Default;

            if (MessageBox.Show("The conversion is complete.." + Environment.NewLine +
                "Would you like to open the folder?", MESSAGEBOX_CAPTION,
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                Process.Start(outputDir);
        }
    }
}
