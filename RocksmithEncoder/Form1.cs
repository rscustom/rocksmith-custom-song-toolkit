using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RocksmithEncoder
{
    public partial class Form1 : Form
    {
        OggFile oggFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "OGG Files|*.ogg|All Files (*.*)|*.*";
            fd.FilterIndex = 1;
            fd.ShowDialog();
            inputXmlTextBox.Text = fd.FileName;
            outputFileTextBox.Text = inputXmlTextBox.Text.Substring(0, inputXmlTextBox.Text.LastIndexOf(".")) + "_fixed.ogg";

            // Make sure we have a valid file
            oggFile = new OggFile();
            if (oggFile.LoadOgg(inputXmlTextBox.Text))
                convertBtn.Enabled = true;
            else
                convertBtn.Enabled = false;

            // Determine endianness right away, this doesn't matter and is purely visual
            if (oggFile.bigEndian)
                bigEndianRadioBtn.Checked = true;
            else
                littleEndianRadioBtn.Checked = true;
        }

        private void convertBtn_Click(object sender, EventArgs e)
        {
            oggFile.WriteOgg(outputFileTextBox.Text);
            
            // done
            MessageBox.Show("Process Complete","File Creation Process");
        }
    }
}
