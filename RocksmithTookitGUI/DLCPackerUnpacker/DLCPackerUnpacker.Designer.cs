namespace RocksmithToolkitGUI.DLCPackerUnpacker
{
    partial class DLCPackerUnpacker
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
            this.unpackButton = new System.Windows.Forms.Button();
            this.packButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.repackButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gameVersionCombo = new System.Windows.Forms.ComboBox();
            this.AppIdTB = new RocksmithToolkitGUI.CueTextBox();
            this.appIdCombo = new System.Windows.Forms.ComboBox();
            this.decodeAudioCheckbox = new System.Windows.Forms.CheckBox();
            this.updateSngCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.extractSongXmlCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.currentOperationLabel = new System.Windows.Forms.Label();
            this.updateProgress = new System.Windows.Forms.ProgressBar();
            this.customFixesGroupBox = new System.Windows.Forms.GroupBox();
            this.deleteSourceFileCheckBox = new System.Windows.Forms.CheckBox();
            this.quickBassFixBox = new System.Windows.Forms.CheckBox();
            this.lowTuningBassFixButton = new System.Windows.Forms.Button();
            this.dlcPackageCreatorControl = new RocksmithToolkitGUI.DLCPackageCreator.DLCPackageCreator();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.customFixesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // unpackButton
            // 
            this.unpackButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.unpackButton.Location = new System.Drawing.Point(6, 76);
            this.unpackButton.Name = "unpackButton";
            this.unpackButton.Size = new System.Drawing.Size(122, 23);
            this.unpackButton.TabIndex = 2;
            this.unpackButton.Text = "Unpack";
            this.unpackButton.UseVisualStyleBackColor = true;
            this.unpackButton.Click += new System.EventHandler(this.unpackButton_Click);
            // 
            // packButton
            // 
            this.packButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.packButton.Location = new System.Drawing.Point(6, 76);
            this.packButton.Name = "packButton";
            this.packButton.Size = new System.Drawing.Size(122, 23);
            this.packButton.TabIndex = 4;
            this.packButton.Text = "Pack";
            this.packButton.UseVisualStyleBackColor = true;
            this.packButton.Click += new System.EventHandler(this.packButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::RocksmithToolkitGUI.Properties.Resources.brasil_logo;
            this.pictureBox2.Location = new System.Drawing.Point(8, 7);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 103);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // repackButton
            // 
            this.repackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.repackButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.repackButton.Location = new System.Drawing.Point(101, 42);
            this.repackButton.Name = "repackButton";
            this.repackButton.Size = new System.Drawing.Size(285, 23);
            this.repackButton.TabIndex = 7;
            this.repackButton.Text = "Choose DLC";
            this.repackButton.UseVisualStyleBackColor = true;
            this.repackButton.Click += new System.EventHandler(this.repackButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gameVersionCombo);
            this.groupBox1.Controls.Add(this.AppIdTB);
            this.groupBox1.Controls.Add(this.appIdCombo);
            this.groupBox1.Controls.Add(this.repackButton);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(8, 115);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(389, 71);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "App ID Updater";
            // 
            // gameVersionCombo
            // 
            this.gameVersionCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameVersionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gameVersionCombo.FormattingEnabled = true;
            this.gameVersionCombo.Location = new System.Drawing.Point(5, 17);
            this.gameVersionCombo.Margin = new System.Windows.Forms.Padding(2);
            this.gameVersionCombo.Name = "gameVersionCombo";
            this.gameVersionCombo.Size = new System.Drawing.Size(93, 21);
            this.gameVersionCombo.TabIndex = 5;
            this.gameVersionCombo.SelectedIndexChanged += new System.EventHandler(this.gameVersionCombo_SelectedIndexChanged);
            // 
            // AppIdTB
            // 
            this.AppIdTB.Cue = "APP ID";
            this.AppIdTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AppIdTB.ForeColor = System.Drawing.Color.Gray;
            this.AppIdTB.Location = new System.Drawing.Point(5, 44);
            this.AppIdTB.Name = "AppIdTB";
            this.AppIdTB.Size = new System.Drawing.Size(93, 20);
            this.AppIdTB.TabIndex = 41;
            // 
            // appIdCombo
            // 
            this.appIdCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appIdCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appIdCombo.FormattingEnabled = true;
            this.appIdCombo.Location = new System.Drawing.Point(102, 17);
            this.appIdCombo.Margin = new System.Windows.Forms.Padding(2);
            this.appIdCombo.Name = "appIdCombo";
            this.appIdCombo.Size = new System.Drawing.Size(283, 21);
            this.appIdCombo.TabIndex = 6;
            this.appIdCombo.SelectedValueChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // decodeAudioCheckbox
            // 
            this.decodeAudioCheckbox.AutoSize = true;
            this.decodeAudioCheckbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.decodeAudioCheckbox.Location = new System.Drawing.Point(18, 19);
            this.decodeAudioCheckbox.Name = "decodeAudioCheckbox";
            this.decodeAudioCheckbox.Size = new System.Drawing.Size(94, 17);
            this.decodeAudioCheckbox.TabIndex = 0;
            this.decodeAudioCheckbox.Text = "Decode Audio";
            this.decodeAudioCheckbox.UseVisualStyleBackColor = true;
            // 
            // updateSngCheckBox
            // 
            this.updateSngCheckBox.AutoSize = true;
            this.updateSngCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.updateSngCheckBox.Location = new System.Drawing.Point(21, 42);
            this.updateSngCheckBox.Name = "updateSngCheckBox";
            this.updateSngCheckBox.Size = new System.Drawing.Size(87, 17);
            this.updateSngCheckBox.TabIndex = 3;
            this.updateSngCheckBox.Text = "Update SNG";
            this.updateSngCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.extractSongXmlCheckBox);
            this.groupBox2.Controls.Add(this.decodeAudioCheckbox);
            this.groupBox2.Controls.Add(this.unpackButton);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(123, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(134, 106);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Unpacker";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(53, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "RS2014 only";
            // 
            // extractSongXmlCheckBox
            // 
            this.extractSongXmlCheckBox.AutoSize = true;
            this.extractSongXmlCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.extractSongXmlCheckBox.Location = new System.Drawing.Point(18, 42);
            this.extractSongXmlCheckBox.Name = "extractSongXmlCheckBox";
            this.extractSongXmlCheckBox.Size = new System.Drawing.Size(86, 17);
            this.extractSongXmlCheckBox.TabIndex = 1;
            this.extractSongXmlCheckBox.Text = "SNG to XML";
            this.extractSongXmlCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.updateSngCheckBox);
            this.groupBox3.Controls.Add(this.packButton);
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox3.Location = new System.Drawing.Point(263, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(134, 106);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Packer";
            // 
            // currentOperationLabel
            // 
            this.currentOperationLabel.AutoSize = true;
            this.currentOperationLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.currentOperationLabel.Location = new System.Drawing.Point(6, 241);
            this.currentOperationLabel.Name = "currentOperationLabel";
            this.currentOperationLabel.Size = new System.Drawing.Size(16, 13);
            this.currentOperationLabel.TabIndex = 19;
            this.currentOperationLabel.Text = "...";
            this.currentOperationLabel.Visible = false;
            // 
            // updateProgress
            // 
            this.updateProgress.Location = new System.Drawing.Point(8, 260);
            this.updateProgress.Name = "updateProgress";
            this.updateProgress.Size = new System.Drawing.Size(389, 26);
            this.updateProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.updateProgress.TabIndex = 0;
            this.updateProgress.Visible = false;
            // 
            // customFixesGroupBox
            // 
            this.customFixesGroupBox.Controls.Add(this.deleteSourceFileCheckBox);
            this.customFixesGroupBox.Controls.Add(this.quickBassFixBox);
            this.customFixesGroupBox.Controls.Add(this.lowTuningBassFixButton);
            this.customFixesGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.customFixesGroupBox.Location = new System.Drawing.Point(8, 191);
            this.customFixesGroupBox.Name = "customFixesGroupBox";
            this.customFixesGroupBox.Size = new System.Drawing.Size(389, 43);
            this.customFixesGroupBox.TabIndex = 20;
            this.customFixesGroupBox.TabStop = false;
            this.customFixesGroupBox.Text = "Custom Fixes (RS 2014)";
            // 
            // deleteSourceFileCheckBox
            // 
            this.deleteSourceFileCheckBox.AutoSize = true;
            this.deleteSourceFileCheckBox.ForeColor = System.Drawing.Color.Black;
            this.deleteSourceFileCheckBox.Location = new System.Drawing.Point(82, 19);
            this.deleteSourceFileCheckBox.Name = "deleteSourceFileCheckBox";
            this.deleteSourceFileCheckBox.Size = new System.Drawing.Size(113, 17);
            this.deleteSourceFileCheckBox.TabIndex = 9;
            this.deleteSourceFileCheckBox.Text = "Delete Source File";
            this.deleteSourceFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // quickBassFixBox
            // 
            this.quickBassFixBox.AutoSize = true;
            this.quickBassFixBox.ForeColor = System.Drawing.Color.Black;
            this.quickBassFixBox.Location = new System.Drawing.Point(6, 19);
            this.quickBassFixBox.Name = "quickBassFixBox";
            this.quickBassFixBox.Size = new System.Drawing.Size(70, 17);
            this.quickBassFixBox.TabIndex = 8;
            this.quickBassFixBox.Text = "Quick Fix";
            this.quickBassFixBox.UseVisualStyleBackColor = true;
            // 
            // lowTuningBassFixButton
            // 
            this.lowTuningBassFixButton.Location = new System.Drawing.Point(201, 15);
            this.lowTuningBassFixButton.Name = "lowTuningBassFixButton";
            this.lowTuningBassFixButton.Size = new System.Drawing.Size(184, 23);
            this.lowTuningBassFixButton.TabIndex = 10;
            this.lowTuningBassFixButton.Text = "Fix Low Bass Tuning";
            this.lowTuningBassFixButton.UseVisualStyleBackColor = true;
            this.lowTuningBassFixButton.Click += new System.EventHandler(this.lowTuningBassFixButton_Click);
            // 
            // dlcPackageCreatorControl
            // 
            this.dlcPackageCreatorControl.Album = "";
            this.dlcPackageCreatorControl.AlbumYear = "";
            this.dlcPackageCreatorControl.AppId = "";
            this.dlcPackageCreatorControl.Artist = "";
            this.dlcPackageCreatorControl.ArtistSort = "";
            this.dlcPackageCreatorControl.AverageTempo = "";
            this.dlcPackageCreatorControl.CurrentGameVersion = RocksmithToolkitLib.GameVersion.RS2014;
            this.dlcPackageCreatorControl.DLCName = "";
            this.dlcPackageCreatorControl.Location = new System.Drawing.Point(0, 0);
            this.dlcPackageCreatorControl.Name = "dlcPackageCreatorControl";
            this.dlcPackageCreatorControl.PackageVersion = "";
            this.dlcPackageCreatorControl.Size = new System.Drawing.Size(507, 571);
            this.dlcPackageCreatorControl.SongTitle = "";
            this.dlcPackageCreatorControl.SongTitleSort = "";
            this.dlcPackageCreatorControl.TabIndex = 0;
            // 
            // DLCPackerUnpacker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.customFixesGroupBox);
            this.Controls.Add(this.currentOperationLabel);
            this.Controls.Add(this.updateProgress);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "DLCPackerUnpacker";
            this.Size = new System.Drawing.Size(400, 308);
            this.MinimumSize = new System.Drawing.Size(400, 308);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.customFixesGroupBox.ResumeLayout(false);
            this.customFixesGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button unpackButton;
        private System.Windows.Forms.Button packButton;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button repackButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox appIdCombo;
        private CueTextBox AppIdTB;
        private System.Windows.Forms.CheckBox decodeAudioCheckbox;
        private System.Windows.Forms.CheckBox updateSngCheckBox;
        private System.Windows.Forms.ComboBox gameVersionCombo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox extractSongXmlCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentOperationLabel;
        private System.Windows.Forms.ProgressBar updateProgress;
        private System.Windows.Forms.GroupBox customFixesGroupBox;
        protected System.Windows.Forms.Button lowTuningBassFixButton;
        private DLCPackageCreator.DLCPackageCreator dlcPackageCreatorControl;
        private System.Windows.Forms.CheckBox quickBassFixBox;
        private System.Windows.Forms.CheckBox deleteSourceFileCheckBox;
    }
}
