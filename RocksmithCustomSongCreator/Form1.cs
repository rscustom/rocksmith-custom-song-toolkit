using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RocksmithSngCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                MessageBox.Show(string.Format("Conversion has failed: {0}", ex.InnerException.Message), "Error");
            }            
            
        }
    }
}
