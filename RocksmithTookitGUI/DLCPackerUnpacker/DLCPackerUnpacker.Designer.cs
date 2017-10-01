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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DLCPackerUnpacker));
            this.btnUnpack = new System.Windows.Forms.Button();
            this.btnPack = new System.Windows.Forms.Button();
            this.btnRepackAppId = new System.Windows.Forms.Button();
            this.gbAppIdUpdater = new System.Windows.Forms.GroupBox();
            this.lblAppId = new System.Windows.Forms.Label();
            this.cmbGameVersion = new System.Windows.Forms.ComboBox();
            this.txtAppId = new System.Windows.Forms.TextBox();
            this.cmbAppId = new System.Windows.Forms.ComboBox();
            this.chkDecodeAudio = new System.Windows.Forms.CheckBox();
            this.chkUpdateSng = new System.Windows.Forms.CheckBox();
            this.gbUnpacker = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkOverwriteSongXml = new System.Windows.Forms.CheckBox();
            this.gbPacker = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkUpdateManifest = new System.Windows.Forms.CheckBox();
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.pbUpdateProgress = new System.Windows.Forms.ProgressBar();
            this.gbCustomFixes = new System.Windows.Forms.GroupBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.chkDeleteSourceFile = new System.Windows.Forms.CheckBox();
            this.chkQuickBassFix = new System.Windows.Forms.CheckBox();
            this.btnFixLowBassTuning = new System.Windows.Forms.Button();
            this.gpSongPacks = new System.Windows.Forms.GroupBox();
            this.btnSelectSongs = new System.Windows.Forms.Button();
            this.btnPackSongPack = new System.Windows.Forms.Button();
            this.lblHelp = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.gbAppIdUpdater.SuspendLayout();
            this.gbUnpacker.SuspendLayout();
            this.gbPacker.SuspendLayout();
            this.gbCustomFixes.SuspendLayout();
            this.gpSongPacks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUnpack
            // 
            this.btnUnpack.ForeColor = System.Drawing.Color.Black;
            this.btnUnpack.Location = new System.Drawing.Point(107, 54);
            this.btnUnpack.Name = "btnUnpack";
            this.btnUnpack.Size = new System.Drawing.Size(75, 23);
            this.btnUnpack.TabIndex = 2;
            this.btnUnpack.Text = "Unpack";
            this.btnUnpack.UseVisualStyleBackColor = true;
            this.btnUnpack.Click += new System.EventHandler(this.btnUnpack_Click);
            // 
            // btnPack
            // 
            this.btnPack.ForeColor = System.Drawing.Color.Black;
            this.btnPack.Location = new System.Drawing.Point(63, 54);
            this.btnPack.Name = "btnPack";
            this.btnPack.Size = new System.Drawing.Size(75, 23);
            this.btnPack.TabIndex = 4;
            this.btnPack.Text = "Pack";
            this.toolTip.SetToolTip(this.btnPack, resources.GetString("btnPack.ToolTip"));
            this.btnPack.UseVisualStyleBackColor = true;
            this.btnPack.Click += new System.EventHandler(this.btnPack_Click);
            // 
            // btnRepackAppId
            // 
            this.btnRepackAppId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRepackAppId.ForeColor = System.Drawing.Color.Black;
            this.btnRepackAppId.Location = new System.Drawing.Point(297, 42);
            this.btnRepackAppId.Name = "btnRepackAppId";
            this.btnRepackAppId.Size = new System.Drawing.Size(128, 23);
            this.btnRepackAppId.TabIndex = 7;
            this.btnRepackAppId.Text = "Repack AppId";
            this.toolTip.SetToolTip(this.btnRepackAppId, "You can also try checking the SNG to XML box if you \r\nreceive an error message wh" +
                    "en using Repack AppId.");
            this.btnRepackAppId.UseVisualStyleBackColor = true;
            this.btnRepackAppId.Click += new System.EventHandler(this.btnRepackAppId_Click);
            // 
            // gbAppIdUpdater
            // 
            this.gbAppIdUpdater.Controls.Add(this.lblAppId);
            this.gbAppIdUpdater.Controls.Add(this.cmbGameVersion);
            this.gbAppIdUpdater.Controls.Add(this.txtAppId);
            this.gbAppIdUpdater.Controls.Add(this.cmbAppId);
            this.gbAppIdUpdater.Controls.Add(this.btnRepackAppId);
            this.gbAppIdUpdater.ForeColor = System.Drawing.Color.Firebrick;
            this.gbAppIdUpdater.Location = new System.Drawing.Point(8, 102);
            this.gbAppIdUpdater.Margin = new System.Windows.Forms.Padding(2);
            this.gbAppIdUpdater.Name = "gbAppIdUpdater";
            this.gbAppIdUpdater.Padding = new System.Windows.Forms.Padding(2);
            this.gbAppIdUpdater.Size = new System.Drawing.Size(434, 71);
            this.gbAppIdUpdater.TabIndex = 13;
            this.gbAppIdUpdater.TabStop = false;
            this.gbAppIdUpdater.Text = "App ID Updater";
            // 
            // lblAppId
            // 
            this.lblAppId.AutoSize = true;
            this.lblAppId.ForeColor = System.Drawing.Color.Black;
            this.lblAppId.Location = new System.Drawing.Point(7, 47);
            this.lblAppId.Name = "lblAppId";
            this.lblAppId.Size = new System.Drawing.Size(142, 13);
            this.lblAppId.TabIndex = 25;
            this.lblAppId.Text = "Enter a Custom App ID here:";
            // 
            // cmbGameVersion
            // 
            this.cmbGameVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGameVersion.FormattingEnabled = true;
            this.cmbGameVersion.Location = new System.Drawing.Point(10, 16);
            this.cmbGameVersion.Margin = new System.Windows.Forms.Padding(2);
            this.cmbGameVersion.Name = "cmbGameVersion";
            this.cmbGameVersion.Size = new System.Drawing.Size(81, 21);
            this.cmbGameVersion.TabIndex = 5;
            this.cmbGameVersion.SelectedIndexChanged += new System.EventHandler(this.cmbGameVersion_SelectedIndexChanged);
            // 
            // txtAppId
            // 
            this.txtAppId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAppId.ForeColor = System.Drawing.Color.Gray;
            this.txtAppId.Location = new System.Drawing.Point(156, 44);
            this.txtAppId.Name = "txtAppId";
            this.txtAppId.Size = new System.Drawing.Size(82, 20);
            this.txtAppId.TabIndex = 41;
            this.txtAppId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.txtAppId, "Specify any valid App ID for a song\r\nthat you own by typing it into this box");
            this.txtAppId.Validating += new System.ComponentModel.CancelEventHandler(this.txtAppId_Validating);
            // 
            // cmbAppId
            // 
            this.cmbAppId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAppId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppId.FormattingEnabled = true;
            this.cmbAppId.Location = new System.Drawing.Point(99, 16);
            this.cmbAppId.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAppId.Name = "cmbAppId";
            this.cmbAppId.Size = new System.Drawing.Size(326, 21);
            this.cmbAppId.TabIndex = 6;
            this.cmbAppId.SelectedValueChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // chkDecodeAudio
            // 
            this.chkDecodeAudio.AutoSize = true;
            this.chkDecodeAudio.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkDecodeAudio.Location = new System.Drawing.Point(7, 58);
            this.chkDecodeAudio.Name = "chkDecodeAudio";
            this.chkDecodeAudio.Size = new System.Drawing.Size(94, 17);
            this.chkDecodeAudio.TabIndex = 0;
            this.chkDecodeAudio.Text = "Decode Audio";
            this.toolTip.SetToolTip(this.chkDecodeAudio, "If checked, decodes audio\r\nto a playable ogg format.");
            this.chkDecodeAudio.UseVisualStyleBackColor = true;
            // 
            // chkUpdateSng
            // 
            this.chkUpdateSng.AutoSize = true;
            this.chkUpdateSng.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkUpdateSng.Location = new System.Drawing.Point(89, 19);
            this.chkUpdateSng.Name = "chkUpdateSng";
            this.chkUpdateSng.Size = new System.Drawing.Size(49, 17);
            this.chkUpdateSng.TabIndex = 3;
            this.chkUpdateSng.Text = "SNG";
            this.toolTip.SetToolTip(this.chkUpdateSng, "If checked generates fresh\r\nSNG files from XML info");
            this.chkUpdateSng.UseVisualStyleBackColor = true;
            // 
            // gbUnpacker
            // 
            this.gbUnpacker.Controls.Add(this.label2);
            this.gbUnpacker.Controls.Add(this.chkOverwriteSongXml);
            this.gbUnpacker.Controls.Add(this.chkDecodeAudio);
            this.gbUnpacker.Controls.Add(this.btnUnpack);
            this.gbUnpacker.ForeColor = System.Drawing.Color.Firebrick;
            this.gbUnpacker.Location = new System.Drawing.Point(100, 4);
            this.gbUnpacker.Name = "gbUnpacker";
            this.gbUnpacker.Size = new System.Drawing.Size(189, 83);
            this.gbUnpacker.TabIndex = 16;
            this.gbUnpacker.TabStop = false;
            this.gbUnpacker.Text = "Unpacker";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Location = new System.Drawing.Point(8, 36);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "Generate fresh XML from SNG files";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkOverwriteSongXml
            // 
            this.chkOverwriteSongXml.AutoSize = true;
            this.chkOverwriteSongXml.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkOverwriteSongXml.Location = new System.Drawing.Point(7, 19);
            this.chkOverwriteSongXml.Name = "chkOverwriteSongXml";
            this.chkOverwriteSongXml.Size = new System.Drawing.Size(86, 17);
            this.chkOverwriteSongXml.TabIndex = 1;
            this.chkOverwriteSongXml.Text = "SNG to XML";
            this.toolTip.SetToolTip(this.chkOverwriteSongXml, "If checked generates fresh XML files from SNG info\r\n\r\nYou can also check this box" +
                    " if you are trying to \r\nRepack AppId and receive an error message.");
            this.chkOverwriteSongXml.UseVisualStyleBackColor = true;
            // 
            // gbPacker
            // 
            this.gbPacker.Controls.Add(this.label1);
            this.gbPacker.Controls.Add(this.chkUpdateManifest);
            this.gbPacker.Controls.Add(this.chkUpdateSng);
            this.gbPacker.Controls.Add(this.btnPack);
            this.gbPacker.ForeColor = System.Drawing.Color.Firebrick;
            this.gbPacker.Location = new System.Drawing.Point(295, 4);
            this.gbPacker.Name = "gbPacker";
            this.gbPacker.Size = new System.Drawing.Size(147, 83);
            this.gbPacker.TabIndex = 17;
            this.gbPacker.TabStop = false;
            this.gbPacker.Text = "Packer";
            this.toolTip.SetToolTip(this.gbPacker, "HINT: Use the CDLC Creator tab\r\nfor making repairs or changes");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(6, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "Update Manifest / SNG files";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkUpdateManifest
            // 
            this.chkUpdateManifest.AutoSize = true;
            this.chkUpdateManifest.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkUpdateManifest.Location = new System.Drawing.Point(6, 19);
            this.chkUpdateManifest.Name = "chkUpdateManifest";
            this.chkUpdateManifest.Size = new System.Drawing.Size(66, 17);
            this.chkUpdateManifest.TabIndex = 5;
            this.chkUpdateManifest.Text = "Manifest";
            this.toolTip.SetToolTip(this.chkUpdateManifest, "If checked regenerates showlights\r\nand updates existing Manifest files\r\nfrom XML " +
                    "info\r\n");
            this.chkUpdateManifest.UseVisualStyleBackColor = true;
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.AutoSize = true;
            this.lblCurrentOperation.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblCurrentOperation.Location = new System.Drawing.Point(16, 391);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.Size = new System.Drawing.Size(16, 13);
            this.lblCurrentOperation.TabIndex = 19;
            this.lblCurrentOperation.Text = "...";
            this.lblCurrentOperation.Visible = false;
            // 
            // pbUpdateProgress
            // 
            this.pbUpdateProgress.Location = new System.Drawing.Point(18, 411);
            this.pbUpdateProgress.Name = "pbUpdateProgress";
            this.pbUpdateProgress.Size = new System.Drawing.Size(416, 20);
            this.pbUpdateProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbUpdateProgress.TabIndex = 0;
            this.pbUpdateProgress.Visible = false;
            // 
            // gbCustomFixes
            // 
            this.gbCustomFixes.Controls.Add(this.chkVerbose);
            this.gbCustomFixes.Controls.Add(this.chkDeleteSourceFile);
            this.gbCustomFixes.Controls.Add(this.chkQuickBassFix);
            this.gbCustomFixes.Controls.Add(this.btnFixLowBassTuning);
            this.gbCustomFixes.ForeColor = System.Drawing.Color.Firebrick;
            this.gbCustomFixes.Location = new System.Drawing.Point(9, 187);
            this.gbCustomFixes.Name = "gbCustomFixes";
            this.gbCustomFixes.Size = new System.Drawing.Size(434, 43);
            this.gbCustomFixes.TabIndex = 20;
            this.gbCustomFixes.TabStop = false;
            this.gbCustomFixes.Text = "Low Bass Tuning Fix";
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Checked = true;
            this.chkVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVerbose.ForeColor = System.Drawing.Color.Black;
            this.chkVerbose.Location = new System.Drawing.Point(226, 19);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(65, 17);
            this.chkVerbose.TabIndex = 11;
            this.chkVerbose.Text = "Verbose";
            this.toolTip.SetToolTip(this.chkVerbose, "If checked shows any error messages.\r\nIncluding if the file doesn\'t need fixing.");
            this.chkVerbose.UseVisualStyleBackColor = true;
            // 
            // chkDeleteSourceFile
            // 
            this.chkDeleteSourceFile.AutoSize = true;
            this.chkDeleteSourceFile.ForeColor = System.Drawing.Color.Black;
            this.chkDeleteSourceFile.Location = new System.Drawing.Point(91, 19);
            this.chkDeleteSourceFile.Name = "chkDeleteSourceFile";
            this.chkDeleteSourceFile.Size = new System.Drawing.Size(124, 17);
            this.chkDeleteSourceFile.TabIndex = 9;
            this.chkDeleteSourceFile.Text = "Delete Source File(s)";
            this.toolTip.SetToolTip(this.chkDeleteSourceFile, "If checked deletes the original CDLC file.");
            this.chkDeleteSourceFile.UseVisualStyleBackColor = true;
            // 
            // chkQuickBassFix
            // 
            this.chkQuickBassFix.AutoSize = true;
            this.chkQuickBassFix.Checked = true;
            this.chkQuickBassFix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQuickBassFix.ForeColor = System.Drawing.Color.Black;
            this.chkQuickBassFix.Location = new System.Drawing.Point(10, 19);
            this.chkQuickBassFix.Name = "chkQuickBassFix";
            this.chkQuickBassFix.Size = new System.Drawing.Size(70, 17);
            this.chkQuickBassFix.TabIndex = 8;
            this.chkQuickBassFix.Text = "Quick Fix";
            this.toolTip.SetToolTip(this.chkQuickBassFix, "If checked automatically apply Low Bass Tuning fix.\r\nNote: Creates new file and d" +
                    "oes not overwrite\r\nthe original CDLC file.  Speeds up the process\r\nif appling Lo" +
                    "w Bass Tuning fix to multiple CDLC.");
            this.chkQuickBassFix.UseVisualStyleBackColor = true;
            // 
            // btnFixLowBassTuning
            // 
            this.btnFixLowBassTuning.ForeColor = System.Drawing.Color.Black;
            this.btnFixLowBassTuning.Location = new System.Drawing.Point(297, 15);
            this.btnFixLowBassTuning.Name = "btnFixLowBassTuning";
            this.btnFixLowBassTuning.Size = new System.Drawing.Size(128, 23);
            this.btnFixLowBassTuning.TabIndex = 10;
            this.btnFixLowBassTuning.Text = "Fix Low Bass Tuning";
            this.btnFixLowBassTuning.UseVisualStyleBackColor = true;
            this.btnFixLowBassTuning.Click += new System.EventHandler(this.btnFixLowBassTuning_Click);
            // 
            // gpSongPacks
            // 
            this.gpSongPacks.Controls.Add(this.btnSelectSongs);
            this.gpSongPacks.Controls.Add(this.btnPackSongPack);
            this.gpSongPacks.Controls.Add(this.lblHelp);
            this.gpSongPacks.ForeColor = System.Drawing.Color.Firebrick;
            this.gpSongPacks.Location = new System.Drawing.Point(8, 247);
            this.gpSongPacks.Name = "gpSongPacks";
            this.gpSongPacks.Size = new System.Drawing.Size(434, 128);
            this.gpSongPacks.TabIndex = 21;
            this.gpSongPacks.TabStop = false;
            this.gpSongPacks.Text = "Song Packs (Rocksmith 2014 PC Only)";
            this.toolTip.SetToolTip(this.gpSongPacks, "Use the \'Converter\' tab to convert\r\nPC Song Packs to other platforms.");
            // 
            // btnSelectSongs
            // 
            this.btnSelectSongs.ForeColor = System.Drawing.Color.Black;
            this.btnSelectSongs.Location = new System.Drawing.Point(297, 22);
            this.btnSelectSongs.Name = "btnSelectSongs";
            this.btnSelectSongs.Size = new System.Drawing.Size(128, 23);
            this.btnSelectSongs.TabIndex = 23;
            this.btnSelectSongs.Text = "Select Songs";
            this.btnSelectSongs.UseVisualStyleBackColor = true;
            this.btnSelectSongs.Click += new System.EventHandler(this.btnSelectSongs_Click);
            // 
            // btnPackSongPack
            // 
            this.btnPackSongPack.ForeColor = System.Drawing.Color.Black;
            this.btnPackSongPack.Location = new System.Drawing.Point(297, 90);
            this.btnPackSongPack.Name = "btnPackSongPack";
            this.btnPackSongPack.Size = new System.Drawing.Size(128, 23);
            this.btnPackSongPack.TabIndex = 5;
            this.btnPackSongPack.Text = "Repack as Song Pack";
            this.btnPackSongPack.UseVisualStyleBackColor = true;
            this.btnPackSongPack.Click += new System.EventHandler(this.btnPackSongPack_Click);
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblHelp.Location = new System.Drawing.Point(7, 22);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(268, 91);
            this.lblHelp.TabIndex = 24;
            this.lblHelp.Text = resources.GetString("lblHelp.Text");
            // 
            // picLogo
            // 
            this.picLogo.Image = global::RocksmithToolkitGUI.Properties.Resources.brasil_logo;
            this.picLogo.Location = new System.Drawing.Point(8, 12);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(75, 75);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 11;
            this.picLogo.TabStop = false;
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 15000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 10;
            // 
            // DLCPackerUnpacker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.gpSongPacks);
            this.Controls.Add(this.gbCustomFixes);
            this.Controls.Add(this.lblCurrentOperation);
            this.Controls.Add(this.pbUpdateProgress);
            this.Controls.Add(this.gbPacker);
            this.Controls.Add(this.gbUnpacker);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.gbAppIdUpdater);
            this.MinimumSize = new System.Drawing.Size(400, 308);
            this.Name = "DLCPackerUnpacker";
            this.Size = new System.Drawing.Size(448, 462);
            this.gbAppIdUpdater.ResumeLayout(false);
            this.gbAppIdUpdater.PerformLayout();
            this.gbUnpacker.ResumeLayout(false);
            this.gbUnpacker.PerformLayout();
            this.gbPacker.ResumeLayout(false);
            this.gbPacker.PerformLayout();
            this.gbCustomFixes.ResumeLayout(false);
            this.gbCustomFixes.PerformLayout();
            this.gpSongPacks.ResumeLayout(false);
            this.gpSongPacks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUnpack;
        private System.Windows.Forms.Button btnPack;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnRepackAppId;
        private System.Windows.Forms.GroupBox gbAppIdUpdater;
        private System.Windows.Forms.ComboBox cmbAppId;
        private System.Windows.Forms.TextBox txtAppId;
        private System.Windows.Forms.CheckBox chkDecodeAudio;
        private System.Windows.Forms.CheckBox chkUpdateSng;
        private System.Windows.Forms.ComboBox cmbGameVersion;
        private System.Windows.Forms.GroupBox gbUnpacker;
        private System.Windows.Forms.CheckBox chkOverwriteSongXml;
        private System.Windows.Forms.GroupBox gbPacker;
        private System.Windows.Forms.Label lblCurrentOperation;
        private System.Windows.Forms.GroupBox gbCustomFixes;
        protected System.Windows.Forms.Button btnFixLowBassTuning;
        private System.Windows.Forms.CheckBox chkQuickBassFix;
        private System.Windows.Forms.CheckBox chkDeleteSourceFile;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ProgressBar pbUpdateProgress;
        private System.Windows.Forms.GroupBox gpSongPacks;
        private System.Windows.Forms.Button btnPackSongPack;
        private System.Windows.Forms.Button btnSelectSongs;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.Label lblAppId;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkVerbose;
        private System.Windows.Forms.CheckBox chkUpdateManifest;
        private System.Windows.Forms.Label label1;
    }
}
