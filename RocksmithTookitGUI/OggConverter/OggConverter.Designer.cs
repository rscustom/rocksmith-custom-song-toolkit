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
            this.inputOggRocksmithTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputOggTextBox
            // 
            this.inputOggTextBox.Location = new System.Drawing.Point(5, 18);
            this.inputOggTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputOggTextBox.Name = "inputOggTextBox";
            this.inputOggTextBox.ReadOnly = true;
            this.inputOggTextBox.Size = new System.Drawing.Size(407, 20);
            this.inputOggTextBox.TabIndex = 1;
            // 
            // oggBrowseButton
            // 
            this.oggBrowseButton.Location = new System.Drawing.Point(416, 18);
            this.oggBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.oggBrowseButton.Name = "oggBrowseButton";
            this.oggBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.oggBrowseButton.TabIndex = 2;
            this.oggBrowseButton.Text = "Browse";
            this.oggBrowseButton.UseVisualStyleBackColor = true;
            this.oggBrowseButton.Click += new System.EventHandler(this.oggBrowseButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.oggBrowseButton);
            this.groupBox1.Controls.Add(this.inputOggTextBox);
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
            this.groupBox2.Controls.Add(this.inputOggRocksmithTextBox);
            this.groupBox2.Location = new System.Drawing.Point(5, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(477, 47);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input OGG Wwise (2010.3.3)/Rocksmith file or directory to convert to Vorbis defau" +
    "lt:";
            // 
            // oggRocksmithBrowseButton
            // 
            this.oggRocksmithBrowseButton.Location = new System.Drawing.Point(416, 18);
            this.oggRocksmithBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.oggRocksmithBrowseButton.Name = "oggRocksmithBrowseButton";
            this.oggRocksmithBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.oggRocksmithBrowseButton.TabIndex = 4;
            this.oggRocksmithBrowseButton.Text = "Browse";
            this.oggRocksmithBrowseButton.UseVisualStyleBackColor = true;
            this.oggRocksmithBrowseButton.Click += new System.EventHandler(this.oggRocksmithBrowseButton_Click);
            // 
            // inputOggRocksmithTextBox
            // 
            this.inputOggRocksmithTextBox.Location = new System.Drawing.Point(5, 18);
            this.inputOggRocksmithTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputOggRocksmithTextBox.Name = "inputOggRocksmithTextBox";
            this.inputOggRocksmithTextBox.ReadOnly = true;
            this.inputOggRocksmithTextBox.Size = new System.Drawing.Size(407, 20);
            this.inputOggRocksmithTextBox.TabIndex = 3;
            // 
            // OggConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "OggConverter";
            this.Size = new System.Drawing.Size(487, 148);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox inputOggTextBox;
        private System.Windows.Forms.Button oggBrowseButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button oggRocksmithBrowseButton;
        private System.Windows.Forms.TextBox inputOggRocksmithTextBox;
    }
}
