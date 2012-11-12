namespace RocksmithTookitGUI.SngFileCreator
{
    partial class SngFileCreator
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
            this.components = new System.ComponentModel.Container();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.littleEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.bigEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.vocalsRadioButton = new System.Windows.Forms.RadioButton();
            this.instrumentRadioButton = new System.Windows.Forms.RadioButton();
            this.sngConvertButton = new System.Windows.Forms.Button();
            this.outputFileTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.inputXmlTextBox = new System.Windows.Forms.TextBox();
            this.sngFileCreatorModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.xmlBrowseButton = new System.Windows.Forms.Button();
            this.tuningComboBox = new System.Windows.Forms.ComboBox();
            this.tuningLabel = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.littleEndianRadioBtn);
            this.groupBox2.Controls.Add(this.bigEndianRadioBtn);
            this.groupBox2.Location = new System.Drawing.Point(3, 242);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(560, 79);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Platform";
            // 
            // littleEndianRadioBtn
            // 
            this.littleEndianRadioBtn.AutoSize = true;
            this.littleEndianRadioBtn.Checked = true;
            this.littleEndianRadioBtn.Location = new System.Drawing.Point(45, 34);
            this.littleEndianRadioBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.littleEndianRadioBtn.Name = "littleEndianRadioBtn";
            this.littleEndianRadioBtn.Size = new System.Drawing.Size(139, 21);
            this.littleEndianRadioBtn.TabIndex = 6;
            this.littleEndianRadioBtn.TabStop = true;
            this.littleEndianRadioBtn.Text = "PC (Little Endian)";
            this.littleEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // bigEndianRadioBtn
            // 
            this.bigEndianRadioBtn.AutoSize = true;
            this.bigEndianRadioBtn.Location = new System.Drawing.Point(236, 34);
            this.bigEndianRadioBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bigEndianRadioBtn.Name = "bigEndianRadioBtn";
            this.bigEndianRadioBtn.Size = new System.Drawing.Size(204, 21);
            this.bigEndianRadioBtn.TabIndex = 7;
            this.bigEndianRadioBtn.Text = "Game Console (Big Endian)";
            this.bigEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tuningLabel);
            this.groupBox1.Controls.Add(this.tuningComboBox);
            this.groupBox1.Controls.Add(this.vocalsRadioButton);
            this.groupBox1.Controls.Add(this.instrumentRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(3, 94);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(560, 128);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input File Type";
            // 
            // vocalsRadioButton
            // 
            this.vocalsRadioButton.AutoSize = true;
            this.vocalsRadioButton.Location = new System.Drawing.Point(236, 34);
            this.vocalsRadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.vocalsRadioButton.Name = "vocalsRadioButton";
            this.vocalsRadioButton.Size = new System.Drawing.Size(120, 21);
            this.vocalsRadioButton.TabIndex = 5;
            this.vocalsRadioButton.Text = "Vocals / Lyrics";
            this.vocalsRadioButton.UseVisualStyleBackColor = true;
            // 
            // instrumentRadioButton
            // 
            this.instrumentRadioButton.AutoSize = true;
            this.instrumentRadioButton.Checked = true;
            this.instrumentRadioButton.Location = new System.Drawing.Point(45, 34);
            this.instrumentRadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.instrumentRadioButton.Name = "instrumentRadioButton";
            this.instrumentRadioButton.Size = new System.Drawing.Size(95, 21);
            this.instrumentRadioButton.TabIndex = 4;
            this.instrumentRadioButton.TabStop = true;
            this.instrumentRadioButton.Text = "Instrument";
            this.instrumentRadioButton.UseVisualStyleBackColor = true;
            // 
            // sngConvertButton
            // 
            this.sngConvertButton.Location = new System.Drawing.Point(239, 337);
            this.sngConvertButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sngConvertButton.Name = "sngConvertButton";
            this.sngConvertButton.Size = new System.Drawing.Size(99, 36);
            this.sngConvertButton.TabIndex = 8;
            this.sngConvertButton.Text = "Convert";
            this.sngConvertButton.UseVisualStyleBackColor = true;
            this.sngConvertButton.Click += new System.EventHandler(this.sngConvertButton_Click);
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Location = new System.Drawing.Point(3, 64);
            this.outputFileTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.ReadOnly = true;
            this.outputFileTextBox.Size = new System.Drawing.Size(559, 22);
            this.outputFileTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output SNG File:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input XML File:";
            // 
            // inputXmlTextBox
            // 
            this.inputXmlTextBox.Location = new System.Drawing.Point(3, 18);
            this.inputXmlTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.inputXmlTextBox.Name = "inputXmlTextBox";
            this.inputXmlTextBox.ReadOnly = true;
            this.inputXmlTextBox.Size = new System.Drawing.Size(559, 22);
            this.inputXmlTextBox.TabIndex = 1;
            // 
            // xmlBrowseButton
            // 
            this.xmlBrowseButton.Location = new System.Drawing.Point(568, 17);
            this.xmlBrowseButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.xmlBrowseButton.Name = "xmlBrowseButton";
            this.xmlBrowseButton.Size = new System.Drawing.Size(75, 25);
            this.xmlBrowseButton.TabIndex = 2;
            this.xmlBrowseButton.Text = "Browse";
            this.xmlBrowseButton.UseVisualStyleBackColor = true;
            this.xmlBrowseButton.Click += new System.EventHandler(this.xmlBrowseButton_Click);
            // 
            // tuningComboBox
            // 
            this.tuningComboBox.FormattingEnabled = true;
            this.tuningComboBox.Items.AddRange(new object[] {
            "Standard (default)",
            "Drop D",
            "E Flat",
            "Open G"});
            this.tuningComboBox.Location = new System.Drawing.Point(182, 78);
            this.tuningComboBox.Name = "tuningComboBox";
            this.tuningComboBox.Size = new System.Drawing.Size(176, 24);
            this.tuningComboBox.TabIndex = 0;
            // 
            // tuningLabel
            // 
            this.tuningLabel.AutoSize = true;
            this.tuningLabel.Location = new System.Drawing.Point(45, 81);
            this.tuningLabel.Name = "tuningLabel";
            this.tuningLabel.Size = new System.Drawing.Size(126, 17);
            this.tuningLabel.TabIndex = 6;
            this.tuningLabel.Text = "Instrument Tuning:";
            // 
            // SngFileCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.sngConvertButton);
            this.Controls.Add(this.outputFileTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputXmlTextBox);
            this.Controls.Add(this.xmlBrowseButton);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SngFileCreator";
            this.Size = new System.Drawing.Size(655, 383);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton littleEndianRadioBtn;
        private System.Windows.Forms.RadioButton bigEndianRadioBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton vocalsRadioButton;
        private System.Windows.Forms.RadioButton instrumentRadioButton;
        private System.Windows.Forms.Button sngConvertButton;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputXmlTextBox;
        private System.Windows.Forms.Button xmlBrowseButton;
        private System.Windows.Forms.BindingSource sngFileCreatorModelBindingSource;
        private System.Windows.Forms.Label tuningLabel;
        private System.Windows.Forms.ComboBox tuningComboBox;
    }
}
