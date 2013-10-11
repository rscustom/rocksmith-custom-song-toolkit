namespace RocksmithToolkitGUI.SngToTabConverter
{
    partial class SngToTabConverter
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
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.inputFileBrowseButton = new System.Windows.Forms.Button();
            this.outputBrowseButton = new System.Windows.Forms.Button();
            this.convertFileButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.difficultyAll = new System.Windows.Forms.RadioButton();
            this.difficultyMax = new System.Windows.Forms.RadioButton();
            this.inputFolderBrowseButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Location = new System.Drawing.Point(5, 73);
            this.outputTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.Size = new System.Drawing.Size(415, 20);
            this.outputTextBox.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output TAB File / Folder:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Input SNG File / Folder:";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextBox.Location = new System.Drawing.Point(5, 36);
            this.inputTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.ReadOnly = true;
            this.inputTextBox.Size = new System.Drawing.Size(291, 20);
            this.inputTextBox.TabIndex = 6;
            // 
            // inputFileBrowseButton
            // 
            this.inputFileBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFileBrowseButton.Location = new System.Drawing.Point(300, 36);
            this.inputFileBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.inputFileBrowseButton.Name = "inputFileBrowseButton";
            this.inputFileBrowseButton.Size = new System.Drawing.Size(88, 20);
            this.inputFileBrowseButton.TabIndex = 7;
            this.inputFileBrowseButton.Text = "Select File...";
            this.inputFileBrowseButton.UseVisualStyleBackColor = true;
            this.inputFileBrowseButton.Click += new System.EventHandler(this.inputFileBrowseButton_Click);
            // 
            // outputBrowseButton
            // 
            this.outputBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputBrowseButton.Location = new System.Drawing.Point(424, 73);
            this.outputBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.outputBrowseButton.Name = "outputBrowseButton";
            this.outputBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.outputBrowseButton.TabIndex = 7;
            this.outputBrowseButton.Text = "Browse";
            this.outputBrowseButton.UseVisualStyleBackColor = true;
            this.outputBrowseButton.Click += new System.EventHandler(this.outputFileBrowseButton_Click);
            // 
            // convertFileButton
            // 
            this.convertFileButton.Location = new System.Drawing.Point(8, 157);
            this.convertFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.convertFileButton.Name = "convertFileButton";
            this.convertFileButton.Size = new System.Drawing.Size(74, 29);
            this.convertFileButton.TabIndex = 9;
            this.convertFileButton.Text = "Convert";
            this.convertFileButton.UseVisualStyleBackColor = true;
            this.convertFileButton.Click += new System.EventHandler(this.convertFileButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.difficultyAll);
            this.groupBox1.Controls.Add(this.difficultyMax);
            this.groupBox1.Controls.Add(this.convertFileButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.outputTextBox);
            this.groupBox1.Controls.Add(this.inputFolderBrowseButton);
            this.groupBox1.Controls.Add(this.inputFileBrowseButton);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.outputBrowseButton);
            this.groupBox1.Controls.Add(this.inputTextBox);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(485, 195);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single File Conversion";
            // 
            // difficultyAll
            // 
            this.difficultyAll.AutoSize = true;
            this.difficultyAll.Location = new System.Drawing.Point(8, 135);
            this.difficultyAll.Name = "difficultyAll";
            this.difficultyAll.Size = new System.Drawing.Size(299, 17);
            this.difficultyAll.TabIndex = 10;
            this.difficultyAll.TabStop = true;
            this.difficultyAll.Text = "All difficulty levels (will create multiple output files per song)";
            this.difficultyAll.UseVisualStyleBackColor = true;
            // 
            // difficultyMax
            // 
            this.difficultyMax.AutoSize = true;
            this.difficultyMax.Checked = true;
            this.difficultyMax.Location = new System.Drawing.Point(8, 112);
            this.difficultyMax.Name = "difficultyMax";
            this.difficultyMax.Size = new System.Drawing.Size(256, 17);
            this.difficultyMax.TabIndex = 10;
            this.difficultyMax.TabStop = true;
            this.difficultyMax.Text = "Maximum difficulty level only (i.e. the actual song)";
            this.difficultyMax.UseVisualStyleBackColor = true;
            // 
            // inputFolderBrowseButton
            // 
            this.inputFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFolderBrowseButton.Location = new System.Drawing.Point(392, 36);
            this.inputFolderBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.inputFolderBrowseButton.Name = "inputFolderBrowseButton";
            this.inputFolderBrowseButton.Size = new System.Drawing.Size(88, 20);
            this.inputFolderBrowseButton.TabIndex = 7;
            this.inputFolderBrowseButton.Text = "Select Folder...";
            this.inputFolderBrowseButton.UseVisualStyleBackColor = true;
            this.inputFolderBrowseButton.Click += new System.EventHandler(this.inputFolderBrowseButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 95);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Difficulty levels to convert:";
            // 
            // SngToTabConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SngToTabConverter";
            this.Size = new System.Drawing.Size(491, 206);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button inputFileBrowseButton;
        private System.Windows.Forms.Button outputBrowseButton;
        private System.Windows.Forms.Button convertFileButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton difficultyAll;
        private System.Windows.Forms.RadioButton difficultyMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button inputFolderBrowseButton;
    }
}
