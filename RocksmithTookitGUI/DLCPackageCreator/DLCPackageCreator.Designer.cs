namespace RocksmithTookitGUI.DLCPackageCreator
{
    partial class DLCPackageCreator
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
            this.albumArtButton = new System.Windows.Forms.Button();
            this.dlcGenerateButton = new System.Windows.Forms.Button();
            this.openOggButton = new System.Windows.Forms.Button();
            this.arrangementRemoveButton = new System.Windows.Forms.Button();
            this.arrangementAddButton = new System.Windows.Forms.Button();
            this.ArrangementLB = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dlcSaveButton = new System.Windows.Forms.Button();
            this.dlcLoadButton = new System.Windows.Forms.Button();
            this.cmbAppIds = new System.Windows.Forms.ComboBox();
            this.AppIdTB = new RocksmithTookitGUI.CueTextBox();
            this.AverageTempo = new RocksmithTookitGUI.CueTextBox();
            this.AlbumArtPathTB = new RocksmithTookitGUI.CueTextBox();
            this.OggPathTB = new RocksmithTookitGUI.CueTextBox();
            this.YearTB = new RocksmithTookitGUI.CueTextBox();
            this.AlbumTB = new RocksmithTookitGUI.CueTextBox();
            this.ArtistTB = new RocksmithTookitGUI.CueTextBox();
            this.SongDisplayNameTB = new RocksmithTookitGUI.CueTextBox();
            this.DlcNameTB = new RocksmithTookitGUI.CueTextBox();
            this.toneRemoveButton = new System.Windows.Forms.Button();
            this.toneAddButton = new System.Windows.Forms.Button();
            this.TonesLB = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.arrangementEditButton = new System.Windows.Forms.Button();
            this.toneEditButton = new System.Windows.Forms.Button();
            this.toneImportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // albumArtButton
            // 
            this.albumArtButton.Location = new System.Drawing.Point(427, 203);
            this.albumArtButton.Name = "albumArtButton";
            this.albumArtButton.Size = new System.Drawing.Size(75, 23);
            this.albumArtButton.TabIndex = 41;
            this.albumArtButton.Text = "Browse";
            this.albumArtButton.UseVisualStyleBackColor = true;
            this.albumArtButton.Click += new System.EventHandler(this.albumArtButton_Click);
            // 
            // dlcGenerateButton
            // 
            this.dlcGenerateButton.Location = new System.Drawing.Point(427, 395);
            this.dlcGenerateButton.Name = "dlcGenerateButton";
            this.dlcGenerateButton.Size = new System.Drawing.Size(75, 29);
            this.dlcGenerateButton.TabIndex = 39;
            this.dlcGenerateButton.Text = "Generate";
            this.dlcGenerateButton.UseVisualStyleBackColor = true;
            this.dlcGenerateButton.Click += new System.EventHandler(this.dlcGenerateButton_Click);
            // 
            // openOggButton
            // 
            this.openOggButton.Location = new System.Drawing.Point(427, 229);
            this.openOggButton.Name = "openOggButton";
            this.openOggButton.Size = new System.Drawing.Size(75, 23);
            this.openOggButton.TabIndex = 38;
            this.openOggButton.Text = "Browse";
            this.openOggButton.UseVisualStyleBackColor = true;
            this.openOggButton.Click += new System.EventHandler(this.openOggButton_Click);
            // 
            // arrangementRemoveButton
            // 
            this.arrangementRemoveButton.Location = new System.Drawing.Point(427, 150);
            this.arrangementRemoveButton.Name = "arrangementRemoveButton";
            this.arrangementRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementRemoveButton.TabIndex = 36;
            this.arrangementRemoveButton.Text = "Remove";
            this.arrangementRemoveButton.UseVisualStyleBackColor = true;
            this.arrangementRemoveButton.Click += new System.EventHandler(this.arrangementRemoveButton_Click);
            // 
            // arrangementAddButton
            // 
            this.arrangementAddButton.Location = new System.Drawing.Point(427, 91);
            this.arrangementAddButton.Name = "arrangementAddButton";
            this.arrangementAddButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementAddButton.TabIndex = 35;
            this.arrangementAddButton.Text = "Add";
            this.arrangementAddButton.UseVisualStyleBackColor = true;
            this.arrangementAddButton.Click += new System.EventHandler(this.arrangementAddButton_Click);
            // 
            // ArrangementLB
            // 
            this.ArrangementLB.FormattingEnabled = true;
            this.ArrangementLB.Location = new System.Drawing.Point(4, 91);
            this.ArrangementLB.Name = "ArrangementLB";
            this.ArrangementLB.Size = new System.Drawing.Size(417, 108);
            this.ArrangementLB.TabIndex = 34;
            this.ArrangementLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ArrangementLB_MouseDoubleClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Arrangements";
            // 
            // dlcSaveButton
            // 
            this.dlcSaveButton.Location = new System.Drawing.Point(85, 395);
            this.dlcSaveButton.Name = "dlcSaveButton";
            this.dlcSaveButton.Size = new System.Drawing.Size(75, 29);
            this.dlcSaveButton.TabIndex = 43;
            this.dlcSaveButton.Text = "Save";
            this.dlcSaveButton.UseVisualStyleBackColor = true;
            this.dlcSaveButton.Click += new System.EventHandler(this.dlcSaveButton_Click);
            // 
            // dlcLoadButton
            // 
            this.dlcLoadButton.Location = new System.Drawing.Point(4, 395);
            this.dlcLoadButton.Name = "dlcLoadButton";
            this.dlcLoadButton.Size = new System.Drawing.Size(75, 29);
            this.dlcLoadButton.TabIndex = 44;
            this.dlcLoadButton.Text = "Load";
            this.dlcLoadButton.UseVisualStyleBackColor = true;
            this.dlcLoadButton.Click += new System.EventHandler(this.dlcLoadButton_Click);
            // 
            // cmbAppIds
            // 
            this.cmbAppIds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppIds.Location = new System.Drawing.Point(193, 53);
            this.cmbAppIds.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAppIds.Name = "cmbAppIds";
            this.cmbAppIds.Size = new System.Drawing.Size(227, 21);
            this.cmbAppIds.TabIndex = 48;
            this.cmbAppIds.SelectedIndexChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // AppIdTB
            // 
            this.AppIdTB.Cue = "DLC App ID";
            this.AppIdTB.Location = new System.Drawing.Point(125, 53);
            this.AppIdTB.Name = "AppIdTB";
            this.AppIdTB.Size = new System.Drawing.Size(63, 20);
            this.AppIdTB.TabIndex = 47;
            // 
            // AverageTempo
            // 
            this.AverageTempo.Cue = "Avg Tempo";
            this.AverageTempo.Location = new System.Drawing.Point(3, 53);
            this.AverageTempo.Name = "AverageTempo";
            this.AverageTempo.Size = new System.Drawing.Size(117, 20);
            this.AverageTempo.TabIndex = 46;
            // 
            // AlbumArtPathTB
            // 
            this.AlbumArtPathTB.Cue = "Album Art File";
            this.AlbumArtPathTB.Location = new System.Drawing.Point(4, 205);
            this.AlbumArtPathTB.Name = "AlbumArtPathTB";
            this.AlbumArtPathTB.Size = new System.Drawing.Size(417, 20);
            this.AlbumArtPathTB.TabIndex = 40;
            // 
            // OggPathTB
            // 
            this.OggPathTB.Cue = "Converted WWise 2010 .ogg File";
            this.OggPathTB.Location = new System.Drawing.Point(4, 231);
            this.OggPathTB.Name = "OggPathTB";
            this.OggPathTB.Size = new System.Drawing.Size(417, 20);
            this.OggPathTB.TabIndex = 37;
            // 
            // YearTB
            // 
            this.YearTB.Cue = "Release Year";
            this.YearTB.Location = new System.Drawing.Point(310, 28);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(110, 20);
            this.YearTB.TabIndex = 32;
            // 
            // AlbumTB
            // 
            this.AlbumTB.Cue = "Album";
            this.AlbumTB.Location = new System.Drawing.Point(125, 28);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(180, 20);
            this.AlbumTB.TabIndex = 31;
            // 
            // ArtistTB
            // 
            this.ArtistTB.Cue = "Artist";
            this.ArtistTB.Location = new System.Drawing.Point(3, 28);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(117, 20);
            this.ArtistTB.TabIndex = 30;
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.Cue = "Song Title";
            this.SongDisplayNameTB.Location = new System.Drawing.Point(125, 3);
            this.SongDisplayNameTB.Name = "SongDisplayNameTB";
            this.SongDisplayNameTB.Size = new System.Drawing.Size(296, 20);
            this.SongDisplayNameTB.TabIndex = 29;
            // 
            // DlcNameTB
            // 
            this.DlcNameTB.Cue = "DLC Name";
            this.DlcNameTB.Location = new System.Drawing.Point(3, 3);
            this.DlcNameTB.Name = "DlcNameTB";
            this.DlcNameTB.Size = new System.Drawing.Size(117, 20);
            this.DlcNameTB.TabIndex = 28;
            // 
            // toneRemoveButton
            // 
            this.toneRemoveButton.Location = new System.Drawing.Point(427, 327);
            this.toneRemoveButton.Name = "toneRemoveButton";
            this.toneRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.toneRemoveButton.TabIndex = 52;
            this.toneRemoveButton.Text = "Remove";
            this.toneRemoveButton.UseVisualStyleBackColor = true;
            this.toneRemoveButton.Click += new System.EventHandler(this.toneRemoveButton_Click);
            // 
            // toneAddButton
            // 
            this.toneAddButton.Location = new System.Drawing.Point(427, 268);
            this.toneAddButton.Name = "toneAddButton";
            this.toneAddButton.Size = new System.Drawing.Size(75, 23);
            this.toneAddButton.TabIndex = 51;
            this.toneAddButton.Text = "Add";
            this.toneAddButton.UseVisualStyleBackColor = true;
            this.toneAddButton.Click += new System.EventHandler(this.toneAddButton_Click);
            // 
            // TonesLB
            // 
            this.TonesLB.FormattingEnabled = true;
            this.TonesLB.Location = new System.Drawing.Point(4, 268);
            this.TonesLB.Name = "TonesLB";
            this.TonesLB.Size = new System.Drawing.Size(417, 121);
            this.TonesLB.TabIndex = 50;
            this.TonesLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ToneLB_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 252);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Tones";
            // 
            // arrangementEditButton
            // 
            this.arrangementEditButton.Location = new System.Drawing.Point(427, 120);
            this.arrangementEditButton.Name = "arrangementEditButton";
            this.arrangementEditButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementEditButton.TabIndex = 53;
            this.arrangementEditButton.Text = "Edit";
            this.arrangementEditButton.UseVisualStyleBackColor = true;
            this.arrangementEditButton.Click += new System.EventHandler(this.arrangementEditButton_Click);
            // 
            // toneEditButton
            // 
            this.toneEditButton.Location = new System.Drawing.Point(427, 297);
            this.toneEditButton.Name = "toneEditButton";
            this.toneEditButton.Size = new System.Drawing.Size(75, 23);
            this.toneEditButton.TabIndex = 54;
            this.toneEditButton.Text = "Edit";
            this.toneEditButton.UseVisualStyleBackColor = true;
            this.toneEditButton.Click += new System.EventHandler(this.toneEditButton_Click);
            // 
            // toneImportButton
            // 
            this.toneImportButton.Location = new System.Drawing.Point(427, 356);
            this.toneImportButton.Name = "toneImportButton";
            this.toneImportButton.Size = new System.Drawing.Size(75, 23);
            this.toneImportButton.TabIndex = 55;
            this.toneImportButton.Text = "Import";
            this.toneImportButton.UseVisualStyleBackColor = true;
            this.toneImportButton.Click += new System.EventHandler(this.toneImportButton_Click);
            // 
            // DLCPackageCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toneImportButton);
            this.Controls.Add(this.toneEditButton);
            this.Controls.Add(this.arrangementEditButton);
            this.Controls.Add(this.toneRemoveButton);
            this.Controls.Add(this.toneAddButton);
            this.Controls.Add(this.TonesLB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbAppIds);
            this.Controls.Add(this.AppIdTB);
            this.Controls.Add(this.AverageTempo);
            this.Controls.Add(this.dlcLoadButton);
            this.Controls.Add(this.dlcSaveButton);
            this.Controls.Add(this.albumArtButton);
            this.Controls.Add(this.AlbumArtPathTB);
            this.Controls.Add(this.dlcGenerateButton);
            this.Controls.Add(this.openOggButton);
            this.Controls.Add(this.OggPathTB);
            this.Controls.Add(this.arrangementRemoveButton);
            this.Controls.Add(this.arrangementAddButton);
            this.Controls.Add(this.ArrangementLB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.YearTB);
            this.Controls.Add(this.AlbumTB);
            this.Controls.Add(this.ArtistTB);
            this.Controls.Add(this.SongDisplayNameTB);
            this.Controls.Add(this.DlcNameTB);
            this.Name = "DLCPackageCreator";
            this.Size = new System.Drawing.Size(509, 453);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button albumArtButton;
        private RocksmithTookitGUI.CueTextBox AlbumArtPathTB;
        private System.Windows.Forms.Button dlcGenerateButton;
        private System.Windows.Forms.Button openOggButton;
        private RocksmithTookitGUI.CueTextBox OggPathTB;
        private System.Windows.Forms.Button arrangementRemoveButton;
        private System.Windows.Forms.Button arrangementAddButton;
        private System.Windows.Forms.ListBox ArrangementLB;
        private System.Windows.Forms.Label label5;
        private RocksmithTookitGUI.CueTextBox YearTB;
        private RocksmithTookitGUI.CueTextBox AlbumTB;
        private RocksmithTookitGUI.CueTextBox ArtistTB;
        private RocksmithTookitGUI.CueTextBox SongDisplayNameTB;
        private RocksmithTookitGUI.CueTextBox DlcNameTB;
        private System.Windows.Forms.Button dlcSaveButton;
        private System.Windows.Forms.Button dlcLoadButton;
        private CueTextBox AverageTempo;
        private CueTextBox AppIdTB;
        private System.Windows.Forms.ComboBox cmbAppIds;
        private System.Windows.Forms.Button toneRemoveButton;
        private System.Windows.Forms.Button toneAddButton;
        private System.Windows.Forms.ListBox TonesLB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button arrangementEditButton;
        private System.Windows.Forms.Button toneEditButton;
        private System.Windows.Forms.Button toneImportButton;
    }
}
