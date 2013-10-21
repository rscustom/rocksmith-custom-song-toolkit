using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.SngToTab;
using Ookii.Dialogs;
using RocksmithToolkitLib.DLCPackage;
using System.Reflection;

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
            IList<string> sourceFileNames;
            string savePath;
            bool max = difficultyMax.Checked;
            bool all = difficultyAll.Checked;

            // Input file(s)
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourceFileNames = ofd.FileNames;
            }

            // Output path
            using (var fbd = new VistaFolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                savePath = fbd.SelectedPath;
            }

            foreach (string inputFile in sourceFileNames)
            {
                if (Path.GetExtension(inputFile) == ".sng")
                    Convert(inputFile, savePath, all);
                else
                    ExtractBeforeConvert(inputFile, savePath, all);
            }

            MessageBox.Show("The conversion is complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Convert(string inputFile, string savePath, bool all)
        {
            SngFile sngFile = new SngFile(inputFile);

            if (String.IsNullOrEmpty(sngFile.Metadata.Arrangement))
                return; // Vocal

            int maxDifficulty = Common.getMaxDifficulty(sngFile);

            int[] difficulties;
            if (all)
                difficulties = Enumerable.Range(0, maxDifficulty + 1).ToArray();
            else // if (max)
                difficulties = new int[] { maxDifficulty };

            foreach (int d in difficulties)
            {
                TabFile tabFile = new TabFile(sngFile, d);

                var outputFileName = (sngFile != null && sngFile.Metadata != null) ? String.Format("{0} - ", sngFile.Metadata.SongTitle) : "";
                outputFileName += Path.GetFileNameWithoutExtension(inputFile);
                outputFileName += (difficulties.Length != 1) ? String.Format(" (level {0:00}).txt", d) : ".txt";
                var outputFilePath = Path.Combine(savePath, outputFileName);

                using (TextWriter tw = new StreamWriter(outputFilePath))
                {
                    tw.Write(tabFile.ToString());
                }
            }
        }

        private void ExtractBeforeConvert(string inputFile, string savePath, bool all) {
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Packer.Unpack(inputFile, appDir, true);
            string unpackedDir = Path.Combine(appDir, Path.GetFileNameWithoutExtension(inputFile) + String.Format("_{0}", Packer.GetPlatform(inputFile).ToString()));
            string[] sngFiles = Directory.GetFiles(unpackedDir, "*.sng", SearchOption.AllDirectories);

            foreach (var sng in sngFiles) {
                Convert(sng, savePath, all);
            }

            try
            {
                if (Directory.Exists(unpackedDir))
                    Directory.Delete(unpackedDir, true);
            }
            catch { /*Have no problem if don't delete*/ }
        }
    }
}
