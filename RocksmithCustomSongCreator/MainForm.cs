using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace RocksmithSngCreator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.Text = String.Format("Rocksmith .SNG File Creator (v{0}.{1} alpha)",
                Assembly.GetExecutingAssembly().GetName().Version.Major,
                Assembly.GetExecutingAssembly().GetName().Version.Minor);
        }

        private void browseButton_Click(object sender, EventArgs e)
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

        private void convertBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputXmlTextBox.Text))
                return;

            SngFileWriter sngFileWriter;

            // platform selection
            if (littleEndianRadioBtn.Checked)
            {
                sngFileWriter = new SngFileWriter(GamePlatform.Pc);
            }
            else
            {
                sngFileWriter = new SngFileWriter(GamePlatform.Console);
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
    }
}
