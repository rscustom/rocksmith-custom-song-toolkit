namespace RocksmithToolkitGUI.CDLC2Tab
{
    partial class CDLC2Tab
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
            this.difficultyAll = new System.Windows.Forms.RadioButton();
            this.difficultyMax = new System.Windows.Forms.RadioButton();
            this.convertButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbOutput = new System.Windows.Forms.GroupBox();
            this.rbGp5 = new System.Windows.Forms.RadioButton();
            this.rbSongList = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rbAsciiTab = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.gbOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // difficultyAll
            // 
            this.difficultyAll.AutoSize = true;
            this.difficultyAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.difficultyAll.ForeColor = System.Drawing.SystemColors.ControlText;
            this.difficultyAll.Location = new System.Drawing.Point(13, 61);
            this.difficultyAll.Name = "difficultyAll";
            this.difficultyAll.Size = new System.Drawing.Size(128, 17);
            this.difficultyAll.TabIndex = 1;
            this.difficultyAll.TabStop = true;
            this.difficultyAll.Text = "All difficulty levels";
            this.difficultyAll.UseVisualStyleBackColor = true;
            // 
            // difficultyMax
            // 
            this.difficultyMax.AutoSize = true;
            this.difficultyMax.Checked = true;
            this.difficultyMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.difficultyMax.ForeColor = System.Drawing.SystemColors.ControlText;
            this.difficultyMax.Location = new System.Drawing.Point(13, 24);
            this.difficultyMax.Name = "difficultyMax";
            this.difficultyMax.Size = new System.Drawing.Size(186, 17);
            this.difficultyMax.TabIndex = 0;
            this.difficultyMax.TabStop = true;
            this.difficultyMax.Text = "Maximum difficulty level only";
            this.difficultyMax.UseVisualStyleBackColor = true;
            // 
            // convertButton
            // 
            this.convertButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.convertButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.convertButton.Location = new System.Drawing.Point(295, 158);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(106, 27);
            this.convertButton.TabIndex = 5;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = false;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gbOutput);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.difficultyMax);
            this.groupBox1.Controls.Add(this.difficultyAll);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(387, 141);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Conversion Options:";
            // 
            // gbOutput
            // 
            this.gbOutput.Controls.Add(this.rbGp5);
            this.gbOutput.Controls.Add(this.rbSongList);
            this.gbOutput.Controls.Add(this.pictureBox1);
            this.gbOutput.Controls.Add(this.rbAsciiTab);
            this.gbOutput.Location = new System.Drawing.Point(282, 9);
            this.gbOutput.Name = "gbOutput";
            this.gbOutput.Size = new System.Drawing.Size(99, 126);
            this.gbOutput.TabIndex = 39;
            this.gbOutput.TabStop = false;
            this.gbOutput.Text = "Output To:";
            // 
            // rbGp5
            // 
            this.rbGp5.AutoSize = true;
            this.rbGp5.Checked = true;
            this.rbGp5.ForeColor = System.Drawing.Color.Black;
            this.rbGp5.Location = new System.Drawing.Point(13, 82);
            this.rbGp5.Name = "rbGp5";
            this.rbGp5.Size = new System.Drawing.Size(69, 17);
            this.rbGp5.TabIndex = 3;
            this.rbGp5.TabStop = true;
            this.rbGp5.Text = "GuitarPro";
            this.toolTip1.SetToolTip(this.rbGp5, "Create a GuitarPro *.gp5 file from RS2014 CDLC");
            this.rbGp5.UseVisualStyleBackColor = true;
            // 
            // rbSongList
            // 
            this.rbSongList.AutoSize = true;
            this.rbSongList.ForeColor = System.Drawing.Color.Black;
            this.rbSongList.Location = new System.Drawing.Point(13, 103);
            this.rbSongList.Name = "rbSongList";
            this.rbSongList.Size = new System.Drawing.Size(66, 17);
            this.rbSongList.TabIndex = 4;
            this.rbSongList.Text = "SongList";
            this.toolTip1.SetToolTip(this.rbSongList, "Create a text file that contains RS2014 CDLC song info");
            this.rbSongList.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RocksmithToolkitGUI.Properties.Resources.music_edit;
            this.pictureBox1.Location = new System.Drawing.Point(34, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 40;
            this.pictureBox1.TabStop = false;
            // 
            // rbAsciiTab
            // 
            this.rbAsciiTab.AutoSize = true;
            this.rbAsciiTab.ForeColor = System.Drawing.Color.Black;
            this.rbAsciiTab.Location = new System.Drawing.Point(13, 61);
            this.rbAsciiTab.Name = "rbAsciiTab";
            this.rbAsciiTab.Size = new System.Drawing.Size(74, 17);
            this.rbAsciiTab.TabIndex = 2;
            this.rbAsciiTab.Text = "ASCII Tab";
            this.toolTip1.SetToolTip(this.rbAsciiTab, "Create a ASCII Tablature *.txt file from RS2014 CDLC");
            this.rbAsciiTab.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(32, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Will create multiple output files per arrangement";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(32, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "One file with last level including all notes";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 100;
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 20;
            // 
            // CDLC2Tab
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.convertButton);
            this.Name = "CDLC2Tab";
            this.Size = new System.Drawing.Size(421, 340);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbOutput.ResumeLayout(false);
            this.gbOutput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton difficultyAll;
        private System.Windows.Forms.RadioButton difficultyMax;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbOutput;
        private System.Windows.Forms.RadioButton rbAsciiTab;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton rbSongList;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton rbGp5;


    }
}
