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
using SngToTab;

namespace RocksmithToolkitGUI.SngToTabConverter
{
    public partial class SngToTabConverter : UserControl
    {
        private enum PathType
        {
            INVALID,
            FILE,
            FOLDER
        }

        private string InputPath
        {
            get { return inputTextBox.Text; }
            set { inputTextBox.Text = value; }
        }

        private string OutputPath
        {
            get { return outputTextBox.Text; }
            set { outputTextBox.Text = value; }
        }

        public SngToTabConverter()
        {
            InitializeComponent();
        }

        private PathType getPathType(string path)
        {
            if (Directory.Exists(path))
                return PathType.FOLDER;
            else if (File.Exists(path))
                return PathType.FILE;
            else
                return PathType.INVALID;
        }

        private void inputFileBrowseButton_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "SNG File|*.sng|All Files (*.*)|*.*";
                fd.Multiselect = false;
                fd.FilterIndex = 1;
                fd.ShowDialog();

                if (string.IsNullOrEmpty(fd.FileName))
                    return;

                InputPath = fd.FileName;
                
                string outputExtension = Path.GetExtension(OutputPath);
                if (string.IsNullOrEmpty(outputExtension))
                    outputExtension = ".txt";
                OutputPath = Path.ChangeExtension(fd.FileName, outputExtension);
            }
        }

        private void inputFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (var fd = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(InputPath) && getPathType(InputPath) != PathType.FOLDER)
                    fd.SelectedPath = Path.GetDirectoryName(InputPath);
                else
                    fd.SelectedPath = InputPath;

                fd.ShowDialog();

                if (string.IsNullOrEmpty(fd.SelectedPath))
                    return;

                InputPath = fd.SelectedPath;

                if (string.IsNullOrEmpty(OutputPath))
                    OutputPath = InputPath;
                else if (getPathType(OutputPath) != PathType.FOLDER)
                    OutputPath = Path.GetDirectoryName(OutputPath);
            }
        }

        private void outputFileBrowseButton_Click(object sender, EventArgs e)
        {
            PathType inputPathType = getPathType(InputPath);

            if (inputPathType == PathType.INVALID)
                MessageBox.Show("A valid input file or folder must be specified.", "Input Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (inputPathType == PathType.FILE)
            {
                using (var fd = new SaveFileDialog())
                {
                    fd.Filter = "Text File|*.txt|Tab File|*.tab|All Files (*.*)|*.*";
                    fd.FilterIndex = 1;
                    fd.ShowDialog();

                    if (string.IsNullOrEmpty(fd.FileName))
                        return;

                    OutputPath = fd.FileName;
                }
            }
            else if (inputPathType == PathType.FOLDER)
            {
                using (var fd = new FolderBrowserDialog())
                {
                    if (!string.IsNullOrEmpty(OutputPath) && (getPathType(OutputPath) != PathType.FOLDER))
                        fd.SelectedPath = Path.GetDirectoryName(OutputPath);
                    else
                        fd.SelectedPath = InputPath;

                    fd.ShowDialog();

                    if (string.IsNullOrEmpty(fd.SelectedPath))
                        return;

                    OutputPath = fd.SelectedPath;
                }
            }
        }

        private void convertFileButton_Click(object sender, EventArgs e)
        {
            PathType inputPathType = getPathType(InputPath);
            PathType outputPathType = getPathType(OutputPath);
            
            if (inputPathType == PathType.INVALID)
            {
                MessageBox.Show("A valid input file or folder must be specified.", "Input Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (inputPathType == PathType.FOLDER && inputPathType != outputPathType)
            {
                MessageBox.Show("If the input path is a folder, the output path must be a folder too.",
                    "Input / Output Path Type Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] inputFiles;
            if (inputPathType == PathType.FILE)
                inputFiles = new string[] { InputPath };
            else
            {
                inputFiles = Directory.GetFiles(InputPath, "*.sng");
                if (inputFiles.Length == 0)
                {
                    MessageBox.Show("Input folder does not contain any *.sng files.",
                        "Input Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            ReplaceDialog.ResultType replaceAction = ReplaceDialog.ResultType.CANCEL;

            foreach (string inputFile in inputFiles)
            {
                SngFile sngFile = new SngFile(inputFile);
                int maxDifficulty = Common.getMaxDifficulty(sngFile);

                int[] difficulties;
                if (difficultyAll.Checked)
                    difficulties = Enumerable.Range(0, maxDifficulty + 1).ToArray();
                else // if (difficultyMax.Checked)
                    difficulties = new int[] { maxDifficulty };
 
                foreach (int d in difficulties)
                {
                    TabFile tabFile = new TabFile(sngFile, d);
                    
                    string outputFilePath;
                    if (outputPathType == PathType.FOLDER)
                        outputFilePath = Path.ChangeExtension(OutputPath + '\\' + Path.GetFileName(inputFile), ".txt");
                    else
                        outputFilePath = OutputPath;
                    
                    if (difficulties.Length != 1)
                        outputFilePath = Path.ChangeExtension(outputFilePath, "Level " + d + Path.GetExtension(outputFilePath));
                    
                    if (File.Exists(outputFilePath))
                    {
                        if (replaceAction == ReplaceDialog.ResultType.NO_TO_ALL)
                            continue;

                        if (replaceAction != ReplaceDialog.ResultType.YES_TO_ALL)
                        {
                            ReplaceDialog rd = new ReplaceDialog(outputFilePath);
                            rd.ShowDialog();

                            replaceAction = rd.Result;

                            if (replaceAction == ReplaceDialog.ResultType.CANCEL)
                                return;
                            if (replaceAction == ReplaceDialog.ResultType.NO || replaceAction == ReplaceDialog.ResultType.NO_TO_ALL)
                                continue;
                        }
                    }

                   
                    TextWriter tw = new StreamWriter(outputFilePath);
                    tw.Write(tabFile.ToString());
                    tw.Close();
                }
            }
        }
    }
}
