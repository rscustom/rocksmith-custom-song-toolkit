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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SngToTabConverter));
            this.difficultyAll = new System.Windows.Forms.RadioButton();
            this.difficultyMax = new System.Windows.Forms.RadioButton();
            this.convertButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbOutput = new System.Windows.Forms.GroupBox();
            this.rbSongList = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rbAsciiTab = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
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
            this.difficultyAll.Location = new System.Drawing.Point(10, 56);
            this.difficultyAll.Name = "difficultyAll";
            this.difficultyAll.Size = new System.Drawing.Size(128, 17);
            this.difficultyAll.TabIndex = 20;
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
            this.difficultyMax.Location = new System.Drawing.Point(10, 19);
            this.difficultyMax.Name = "difficultyMax";
            this.difficultyMax.Size = new System.Drawing.Size(186, 17);
            this.difficultyMax.TabIndex = 21;
            this.difficultyMax.TabStop = true;
            this.difficultyMax.Text = "Maximum difficulty level only";
            this.difficultyMax.UseVisualStyleBackColor = true;
            // 
            // convertButton
            // 
            this.convertButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.convertButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.convertButton.Location = new System.Drawing.Point(80, 112);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(105, 23);
            this.convertButton.TabIndex = 24;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = false;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gbOutput);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.difficultyMax);
            this.groupBox1.Controls.Add(this.convertButton);
            this.groupBox1.Controls.Add(this.difficultyAll);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(383, 174);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Convert CDLC Archive:";
            // 
            // gbOutput
            // 
            this.gbOutput.Controls.Add(this.rbSongList);
            this.gbOutput.Controls.Add(this.pictureBox1);
            this.gbOutput.Controls.Add(this.rbAsciiTab);
            this.gbOutput.Location = new System.Drawing.Point(264, 19);
            this.gbOutput.Name = "gbOutput";
            this.gbOutput.Size = new System.Drawing.Size(99, 135);
            this.gbOutput.TabIndex = 39;
            this.gbOutput.TabStop = false;
            this.gbOutput.Text = "Output To:";
            // 
            // rbSongList
            // 
            this.rbSongList.AutoSize = true;
            this.rbSongList.ForeColor = System.Drawing.Color.Black;
            this.rbSongList.Location = new System.Drawing.Point(12, 93);
            this.rbSongList.Name = "rbSongList";
            this.rbSongList.Size = new System.Drawing.Size(66, 17);
            this.rbSongList.TabIndex = 41;
            this.rbSongList.Text = "SongList";
            this.toolTip1.SetToolTip(this.rbSongList, "Create a text file that contains archive song info.");
            this.rbSongList.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(34, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 40;
            this.pictureBox1.TabStop = false;
            // 
            // rbAsciiTab
            // 
            this.rbAsciiTab.AutoSize = true;
            this.rbAsciiTab.Checked = true;
            this.rbAsciiTab.ForeColor = System.Drawing.Color.Black;
            this.rbAsciiTab.Location = new System.Drawing.Point(12, 65);
            this.rbAsciiTab.Name = "rbAsciiTab";
            this.rbAsciiTab.Size = new System.Drawing.Size(74, 17);
            this.rbAsciiTab.TabIndex = 38;
            this.rbAsciiTab.TabStop = true;
            this.rbAsciiTab.Text = "ASCII Tab";
            this.rbAsciiTab.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Location = new System.Drawing.Point(17, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(227, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "Now Compatible with RS1 and RS2014 CDLC.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(29, 76);
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
            this.label1.Location = new System.Drawing.Point(29, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "One file with last level including all notes";
            // 
            // SngToTabConverter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox1);
            this.Name = "SngToTabConverter";
            this.Size = new System.Drawing.Size(410, 205);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox gbOutput;
        private System.Windows.Forms.RadioButton rbAsciiTab;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton rbSongList;
        private System.Windows.Forms.ToolTip toolTip1;


    }
}
