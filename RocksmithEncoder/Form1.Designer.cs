namespace RocksmithEncoder
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.browseButton = new System.Windows.Forms.Button();
            this.inputXmlTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.outputFileTextBox = new System.Windows.Forms.TextBox();
            this.convertBtn = new System.Windows.Forms.Button();
            this.littleEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.bigEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(311, 43);
            this.browseButton.Margin = new System.Windows.Forms.Padding(2);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(56, 19);
            this.browseButton.TabIndex = 0;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // inputXmlTextBox
            // 
            this.inputXmlTextBox.Location = new System.Drawing.Point(20, 44);
            this.inputXmlTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputXmlTextBox.Name = "inputXmlTextBox";
            this.inputXmlTextBox.ReadOnly = true;
            this.inputXmlTextBox.Size = new System.Drawing.Size(288, 20);
            this.inputXmlTextBox.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input OGG File: (Wwise 2010.3.3)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 78);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output OGG File:";
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Location = new System.Drawing.Point(22, 97);
            this.outputFileTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.ReadOnly = true;
            this.outputFileTextBox.Size = new System.Drawing.Size(288, 20);
            this.outputFileTextBox.TabIndex = 4;
            // 
            // convertBtn
            // 
            this.convertBtn.Enabled = false;
            this.convertBtn.Location = new System.Drawing.Point(167, 220);
            this.convertBtn.Margin = new System.Windows.Forms.Padding(2);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(56, 19);
            this.convertBtn.TabIndex = 5;
            this.convertBtn.Text = "Convert";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // littleEndianRadioBtn
            // 
            this.littleEndianRadioBtn.AutoSize = true;
            this.littleEndianRadioBtn.Checked = true;
            this.littleEndianRadioBtn.Location = new System.Drawing.Point(21, 28);
            this.littleEndianRadioBtn.Margin = new System.Windows.Forms.Padding(2);
            this.littleEndianRadioBtn.Name = "littleEndianRadioBtn";
            this.littleEndianRadioBtn.Size = new System.Drawing.Size(106, 17);
            this.littleEndianRadioBtn.TabIndex = 6;
            this.littleEndianRadioBtn.TabStop = true;
            this.littleEndianRadioBtn.Text = "PC (Little Endian)";
            this.littleEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // bigEndianRadioBtn
            // 
            this.bigEndianRadioBtn.AutoSize = true;
            this.bigEndianRadioBtn.Location = new System.Drawing.Point(162, 28);
            this.bigEndianRadioBtn.Margin = new System.Windows.Forms.Padding(2);
            this.bigEndianRadioBtn.Name = "bigEndianRadioBtn";
            this.bigEndianRadioBtn.Size = new System.Drawing.Size(154, 17);
            this.bigEndianRadioBtn.TabIndex = 7;
            this.bigEndianRadioBtn.Text = "Game Console (Big Endian)";
            this.bigEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.littleEndianRadioBtn);
            this.groupBox2.Controls.Add(this.bigEndianRadioBtn);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(22, 139);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(348, 64);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Platform";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 251);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.outputFileTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputXmlTextBox);
            this.Controls.Add(this.browseButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Rocksmith Encoder";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox inputXmlTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.RadioButton littleEndianRadioBtn;
        private System.Windows.Forms.RadioButton bigEndianRadioBtn;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

