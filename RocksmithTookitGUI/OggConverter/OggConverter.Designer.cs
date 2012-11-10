namespace RocksmithTookitGUI.OggConverter
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
            this.oggConvertButton = new System.Windows.Forms.Button();
            this.outputOggTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.inputOggTextBox = new System.Windows.Forms.TextBox();
            this.oggBrowseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // oggConvertButton
            // 
            this.oggConvertButton.Location = new System.Drawing.Point(179, 76);
            this.oggConvertButton.Margin = new System.Windows.Forms.Padding(2);
            this.oggConvertButton.Name = "oggConvertButton";
            this.oggConvertButton.Size = new System.Drawing.Size(74, 29);
            this.oggConvertButton.TabIndex = 6;
            this.oggConvertButton.Text = "Convert";
            this.oggConvertButton.UseVisualStyleBackColor = true;
            this.oggConvertButton.Click += new System.EventHandler(this.oggConvertButton_Click);
            // 
            // outputOggTextBox
            // 
            this.outputOggTextBox.Location = new System.Drawing.Point(2, 52);
            this.outputOggTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.outputOggTextBox.Name = "outputOggTextBox";
            this.outputOggTextBox.ReadOnly = true;
            this.outputOggTextBox.Size = new System.Drawing.Size(420, 20);
            this.outputOggTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Output OGG File:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Input OGG File: (Wwise 2010.3.3)";
            // 
            // inputOggTextBox
            // 
            this.inputOggTextBox.Location = new System.Drawing.Point(2, 15);
            this.inputOggTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputOggTextBox.Name = "inputOggTextBox";
            this.inputOggTextBox.ReadOnly = true;
            this.inputOggTextBox.Size = new System.Drawing.Size(420, 20);
            this.inputOggTextBox.TabIndex = 1;
            // 
            // oggBrowseButton
            // 
            this.oggBrowseButton.Location = new System.Drawing.Point(426, 14);
            this.oggBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.oggBrowseButton.Name = "oggBrowseButton";
            this.oggBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.oggBrowseButton.TabIndex = 2;
            this.oggBrowseButton.Text = "Browse";
            this.oggBrowseButton.UseVisualStyleBackColor = true;
            this.oggBrowseButton.Click += new System.EventHandler(this.oggBrowseButton_Click);
            // 
            // OggConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.oggConvertButton);
            this.Controls.Add(this.outputOggTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.inputOggTextBox);
            this.Controls.Add(this.oggBrowseButton);
            this.Name = "OggConverter";
            this.Size = new System.Drawing.Size(487, 112);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button oggConvertButton;
        private System.Windows.Forms.TextBox outputOggTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox inputOggTextBox;
        private System.Windows.Forms.Button oggBrowseButton;
    }
}
