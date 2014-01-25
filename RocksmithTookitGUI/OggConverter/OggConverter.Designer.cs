namespace RocksmithToolkitGUI.OggConverter
{
    partial class OggConverter
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputOggTextBox = new System.Windows.Forms.TextBox();
            this.oggBrowseButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.oggRocksmithBrowseButton = new System.Windows.Forms.Button();
            this.inputAudioRocksmithTextBox = new System.Windows.Forms.TextBox();
            this.InputWemConversionTextbox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.WEMConvertBrowseButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputOggTextBox
            // 
            this.inputOggTextBox.Location = new System.Drawing.Point(5, 18);
            this.inputOggTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputOggTextBox.Name = "inputOggTextBox";
            this.inputOggTextBox.ReadOnly = true;
            this.inputOggTextBox.Size = new System.Drawing.Size(407, 20);
            this.inputOggTextBox.TabIndex = 0;
            // 
            // oggBrowseButton
            // 
            this.oggBrowseButton.Location = new System.Drawing.Point(416, 18);
            this.oggBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.oggBrowseButton.Name = "oggBrowseButton";
            this.oggBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.oggBrowseButton.TabIndex = 1;
            this.oggBrowseButton.Text = "Browse";
            this.oggBrowseButton.UseVisualStyleBackColor = true;
            this.oggBrowseButton.Click += new System.EventHandler(this.oggBrowseButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.oggBrowseButton);
            this.groupBox1.Controls.Add(this.inputOggTextBox);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(477, 45);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input OGG file or directory to fix header (Wwise 2010.3.3):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.oggRocksmithBrowseButton);
            this.groupBox2.Controls.Add(this.inputAudioRocksmithTextBox);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(5, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(477, 47);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input OGG (Wwise 2010) or WEM (Wwise 2013) file or directory to convert to Vorbis" +
    " default:";
            // 
            // oggRocksmithBrowseButton
            // 
            this.oggRocksmithBrowseButton.Location = new System.Drawing.Point(416, 18);
            this.oggRocksmithBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.oggRocksmithBrowseButton.Name = "oggRocksmithBrowseButton";
            this.oggRocksmithBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.oggRocksmithBrowseButton.TabIndex = 3;
            this.oggRocksmithBrowseButton.Text = "Browse";
            this.oggRocksmithBrowseButton.UseVisualStyleBackColor = true;
            this.oggRocksmithBrowseButton.Click += new System.EventHandler(this.oggRocksmithBrowseButton_Click);
            // 
            // inputAudioRocksmithTextBox
            // 
            this.inputAudioRocksmithTextBox.Location = new System.Drawing.Point(5, 18);
            this.inputAudioRocksmithTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputAudioRocksmithTextBox.Name = "inputAudioRocksmithTextBox";
            this.inputAudioRocksmithTextBox.ReadOnly = true;
            this.inputAudioRocksmithTextBox.Size = new System.Drawing.Size(407, 20);
            this.inputAudioRocksmithTextBox.TabIndex = 2;
            // 
            // InputWemConversionTextbox
            // 
            this.InputWemConversionTextbox.Location = new System.Drawing.Point(5, 19);
            this.InputWemConversionTextbox.Name = "InputWemConversionTextbox";
            this.InputWemConversionTextbox.ReadOnly = true;
            this.InputWemConversionTextbox.Size = new System.Drawing.Size(406, 20);
            this.InputWemConversionTextbox.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.WEMConvertBrowseButton);
            this.groupBox3.Controls.Add(this.InputWemConversionTextbox);
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox3.Location = new System.Drawing.Point(5, 146);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(477, 48);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input OGG (Wwise 2010) or WEM (Wwise 2013) file to convert from PC <-> Console:";
            // 
            // WEMConvertBrowseButton
            // 
            this.WEMConvertBrowseButton.Location = new System.Drawing.Point(416, 17);
            this.WEMConvertBrowseButton.Name = "WEMConvertBrowseButton";
            this.WEMConvertBrowseButton.Size = new System.Drawing.Size(55, 23);
            this.WEMConvertBrowseButton.TabIndex = 5;
            this.WEMConvertBrowseButton.Text = "Browse";
            this.WEMConvertBrowseButton.UseVisualStyleBackColor = true;
            this.WEMConvertBrowseButton.Click += new System.EventHandler(this.WEMConvertBrowseButton_Click_1);
            // 
            // OggConverter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "OggConverter";
            this.Size = new System.Drawing.Size(496, 210);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox inputOggTextBox;
        private System.Windows.Forms.Button oggBrowseButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button oggRocksmithBrowseButton;
        private System.Windows.Forms.TextBox inputAudioRocksmithTextBox;
        private System.Windows.Forms.TextBox InputWemConversionTextbox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button WEMConvertBrowseButton;
    }
}
