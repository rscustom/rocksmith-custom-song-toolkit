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
            this.toneRemoveButton = new System.Windows.Forms.Button();
            this.toneAddButton = new System.Windows.Forms.Button();
            this.TonesLB = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.arrangementEditButton = new System.Windows.Forms.Button();
            this.toneEditButton = new System.Windows.Forms.Button();
            this.toneImportButton = new System.Windows.Forms.Button();
            this.openOggXBox360Button = new System.Windows.Forms.Button();
            this.platformPC = new System.Windows.Forms.CheckBox();
            this.platformXBox360 = new System.Windows.Forms.CheckBox();
            this.platformPS3 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ArtistSortTB = new RocksmithTookitGUI.CueTextBox();
            this.SongDisplayNameSortTB = new RocksmithTookitGUI.CueTextBox();
            this.oggXBox360PathTB = new RocksmithTookitGUI.CueTextBox();
            this.AppIdTB = new RocksmithTookitGUI.CueTextBox();
            this.AverageTempo = new RocksmithTookitGUI.CueTextBox();
            this.AlbumArtPathTB = new RocksmithTookitGUI.CueTextBox();
            this.oggPathTB = new RocksmithTookitGUI.CueTextBox();
            this.YearTB = new RocksmithTookitGUI.CueTextBox();
            this.AlbumTB = new RocksmithTookitGUI.CueTextBox();
            this.ArtistTB = new RocksmithTookitGUI.CueTextBox();
            this.SongDisplayNameTB = new RocksmithTookitGUI.CueTextBox();
            this.DlcNameTB = new RocksmithTookitGUI.CueTextBox();
            this.SuspendLayout();
            // 
            // albumArtButton
            // 
            this.albumArtButton.Location = new System.Drawing.Point(427, 189);
            this.albumArtButton.Name = "albumArtButton";
            this.albumArtButton.Size = new System.Drawing.Size(34, 23);
            this.albumArtButton.TabIndex = 14;
            this.albumArtButton.Text = "...";
            this.albumArtButton.UseVisualStyleBackColor = true;
            this.albumArtButton.Click += new System.EventHandler(this.albumArtButton_Click);
            // 
            // dlcGenerateButton
            // 
            this.dlcGenerateButton.Location = new System.Drawing.Point(427, 395);
            this.dlcGenerateButton.Name = "dlcGenerateButton";
            this.dlcGenerateButton.Size = new System.Drawing.Size(75, 29);
            this.dlcGenerateButton.TabIndex = 27;
            this.dlcGenerateButton.Text = "Generate";
            this.dlcGenerateButton.UseVisualStyleBackColor = true;
            this.dlcGenerateButton.Click += new System.EventHandler(this.dlcGenerateButton_Click);
            // 
            // openOggButton
            // 
            this.openOggButton.Location = new System.Drawing.Point(427, 215);
            this.openOggButton.Name = "openOggButton";
            this.openOggButton.Size = new System.Drawing.Size(34, 23);
            this.openOggButton.TabIndex = 16;
            this.openOggButton.Text = "...";
            this.openOggButton.UseVisualStyleBackColor = true;
            this.openOggButton.Click += new System.EventHandler(this.openOggButton_Click);
            // 
            // arrangementRemoveButton
            // 
            this.arrangementRemoveButton.Location = new System.Drawing.Point(427, 150);
            this.arrangementRemoveButton.Name = "arrangementRemoveButton";
            this.arrangementRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementRemoveButton.TabIndex = 12;
            this.arrangementRemoveButton.Text = "Remove";
            this.arrangementRemoveButton.UseVisualStyleBackColor = true;
            this.arrangementRemoveButton.Click += new System.EventHandler(this.arrangementRemoveButton_Click);
            // 
            // arrangementAddButton
            // 
            this.arrangementAddButton.Location = new System.Drawing.Point(427, 91);
            this.arrangementAddButton.Name = "arrangementAddButton";
            this.arrangementAddButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementAddButton.TabIndex = 10;
            this.arrangementAddButton.Text = "Add";
            this.arrangementAddButton.UseVisualStyleBackColor = true;
            this.arrangementAddButton.Click += new System.EventHandler(this.arrangementAddButton_Click);
            // 
            // ArrangementLB
            // 
            this.ArrangementLB.FormattingEnabled = true;
            this.ArrangementLB.Location = new System.Drawing.Point(4, 91);
            this.ArrangementLB.Name = "ArrangementLB";
            this.ArrangementLB.Size = new System.Drawing.Size(417, 82);
            this.ArrangementLB.TabIndex = 34;
            this.ArrangementLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ArrangementLB_MouseDoubleClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Arrangements:";
            // 
            // dlcSaveButton
            // 
            this.dlcSaveButton.Location = new System.Drawing.Point(85, 395);
            this.dlcSaveButton.Name = "dlcSaveButton";
            this.dlcSaveButton.Size = new System.Drawing.Size(75, 29);
            this.dlcSaveButton.TabIndex = 24;
            this.dlcSaveButton.Text = "Save";
            this.dlcSaveButton.UseVisualStyleBackColor = true;
            this.dlcSaveButton.Click += new System.EventHandler(this.dlcSaveButton_Click);
            // 
            // dlcLoadButton
            // 
            this.dlcLoadButton.Location = new System.Drawing.Point(4, 395);
            this.dlcLoadButton.Name = "dlcLoadButton";
            this.dlcLoadButton.Size = new System.Drawing.Size(75, 29);
            this.dlcLoadButton.TabIndex = 23;
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
            this.cmbAppIds.Size = new System.Drawing.Size(309, 21);
            this.cmbAppIds.TabIndex = 9;
            this.cmbAppIds.SelectedIndexChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // toneRemoveButton
            // 
            this.toneRemoveButton.Location = new System.Drawing.Point(427, 337);
            this.toneRemoveButton.Name = "toneRemoveButton";
            this.toneRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.toneRemoveButton.TabIndex = 21;
            this.toneRemoveButton.Text = "Remove";
            this.toneRemoveButton.UseVisualStyleBackColor = true;
            this.toneRemoveButton.Click += new System.EventHandler(this.toneRemoveButton_Click);
            // 
            // toneAddButton
            // 
            this.toneAddButton.Location = new System.Drawing.Point(427, 279);
            this.toneAddButton.Name = "toneAddButton";
            this.toneAddButton.Size = new System.Drawing.Size(75, 23);
            this.toneAddButton.TabIndex = 19;
            this.toneAddButton.Text = "Add";
            this.toneAddButton.UseVisualStyleBackColor = true;
            this.toneAddButton.Click += new System.EventHandler(this.toneAddButton_Click);
            // 
            // TonesLB
            // 
            this.TonesLB.FormattingEnabled = true;
            this.TonesLB.Location = new System.Drawing.Point(4, 281);
            this.TonesLB.Name = "TonesLB";
            this.TonesLB.Size = new System.Drawing.Size(417, 108);
            this.TonesLB.TabIndex = 50;
            this.TonesLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ToneLB_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Tones:";
            // 
            // arrangementEditButton
            // 
            this.arrangementEditButton.Location = new System.Drawing.Point(427, 120);
            this.arrangementEditButton.Name = "arrangementEditButton";
            this.arrangementEditButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementEditButton.TabIndex = 11;
            this.arrangementEditButton.Text = "Edit";
            this.arrangementEditButton.UseVisualStyleBackColor = true;
            this.arrangementEditButton.Click += new System.EventHandler(this.arrangementEditButton_Click);
            // 
            // toneEditButton
            // 
            this.toneEditButton.Location = new System.Drawing.Point(427, 308);
            this.toneEditButton.Name = "toneEditButton";
            this.toneEditButton.Size = new System.Drawing.Size(75, 23);
            this.toneEditButton.TabIndex = 20;
            this.toneEditButton.Text = "Edit";
            this.toneEditButton.UseVisualStyleBackColor = true;
            this.toneEditButton.Click += new System.EventHandler(this.toneEditButton_Click);
            // 
            // toneImportButton
            // 
            this.toneImportButton.Location = new System.Drawing.Point(427, 366);
            this.toneImportButton.Name = "toneImportButton";
            this.toneImportButton.Size = new System.Drawing.Size(75, 23);
            this.toneImportButton.TabIndex = 22;
            this.toneImportButton.Text = "Import";
            this.toneImportButton.UseVisualStyleBackColor = true;
            this.toneImportButton.Click += new System.EventHandler(this.toneImportButton_Click);
            // 
            // openOggXBox360Button
            // 
            this.openOggXBox360Button.Location = new System.Drawing.Point(427, 242);
            this.openOggXBox360Button.Name = "openOggXBox360Button";
            this.openOggXBox360Button.Size = new System.Drawing.Size(34, 23);
            this.openOggXBox360Button.TabIndex = 18;
            this.openOggXBox360Button.Text = "...";
            this.openOggXBox360Button.UseVisualStyleBackColor = true;
            this.openOggXBox360Button.Visible = false;
            this.openOggXBox360Button.Click += new System.EventHandler(this.openOggXBox360Button_Click);
            // 
            // platformPC
            // 
            this.platformPC.AutoSize = true;
            this.platformPC.Checked = true;
            this.platformPC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformPC.Location = new System.Drawing.Point(205, 402);
            this.platformPC.Name = "platformPC";
            this.platformPC.Size = new System.Drawing.Size(40, 17);
            this.platformPC.TabIndex = 25;
            this.platformPC.Text = "PC";
            this.platformPC.UseVisualStyleBackColor = true;
            this.platformPC.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformXBox360
            // 
            this.platformXBox360.AutoSize = true;
            this.platformXBox360.Location = new System.Drawing.Point(251, 402);
            this.platformXBox360.Name = "platformXBox360";
            this.platformXBox360.Size = new System.Drawing.Size(69, 17);
            this.platformXBox360.TabIndex = 26;
            this.platformXBox360.Text = "XBox360";
            this.platformXBox360.UseVisualStyleBackColor = true;
            this.platformXBox360.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformPS3
            // 
            this.platformPS3.AutoSize = true;
            this.platformPS3.Enabled = false;
            this.platformPS3.Location = new System.Drawing.Point(326, 402);
            this.platformPS3.Name = "platformPS3";
            this.platformPS3.Size = new System.Drawing.Size(46, 17);
            this.platformPS3.TabIndex = 60;
            this.platformPS3.Text = "PS3";
            this.platformPS3.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 176);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 61;
            this.label2.Text = "Files:";
            // 
            // ArtistSortTB
            // 
            this.ArtistSortTB.Cue = "Artist Sort";
            this.ArtistSortTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArtistSortTB.ForeColor = System.Drawing.Color.Gray;
            this.ArtistSortTB.Location = new System.Drawing.Point(291, 27);
            this.ArtistSortTB.Name = "ArtistSortTB";
            this.ArtistSortTB.Size = new System.Drawing.Size(130, 20);
            this.ArtistSortTB.TabIndex = 5;
            // 
            // SongDisplayNameSortTB
            // 
            this.SongDisplayNameSortTB.Cue = "Song Title Sort";
            this.SongDisplayNameSortTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SongDisplayNameSortTB.ForeColor = System.Drawing.Color.Gray;
            this.SongDisplayNameSortTB.Location = new System.Drawing.Point(326, 3);
            this.SongDisplayNameSortTB.Name = "SongDisplayNameSortTB";
            this.SongDisplayNameSortTB.Size = new System.Drawing.Size(176, 20);
            this.SongDisplayNameSortTB.TabIndex = 2;
            // 
            // oggXBox360PathTB
            // 
            this.oggXBox360PathTB.Cue = "Converted audio for XBox360 on WWise 2010 (.ogg)";
            this.oggXBox360PathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.oggXBox360PathTB.ForeColor = System.Drawing.Color.Gray;
            this.oggXBox360PathTB.Location = new System.Drawing.Point(4, 243);
            this.oggXBox360PathTB.Name = "oggXBox360PathTB";
            this.oggXBox360PathTB.Size = new System.Drawing.Size(417, 20);
            this.oggXBox360PathTB.TabIndex = 17;
            this.oggXBox360PathTB.Visible = false;
            // 
            // AppIdTB
            // 
            this.AppIdTB.Cue = "DLC App ID";
            this.AppIdTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AppIdTB.ForeColor = System.Drawing.Color.Gray;
            this.AppIdTB.Location = new System.Drawing.Point(125, 53);
            this.AppIdTB.Name = "AppIdTB";
            this.AppIdTB.Size = new System.Drawing.Size(63, 20);
            this.AppIdTB.TabIndex = 8;
            this.AppIdTB.TextChanged += new System.EventHandler(this.AppIdTB_TextChanged);
            // 
            // AverageTempo
            // 
            this.AverageTempo.Cue = "Avg Tempo";
            this.AverageTempo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AverageTempo.ForeColor = System.Drawing.Color.Gray;
            this.AverageTempo.Location = new System.Drawing.Point(3, 53);
            this.AverageTempo.Name = "AverageTempo";
            this.AverageTempo.Size = new System.Drawing.Size(117, 20);
            this.AverageTempo.TabIndex = 7;
            // 
            // AlbumArtPathTB
            // 
            this.AlbumArtPathTB.Cue = "Album Art (.dds)";
            this.AlbumArtPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AlbumArtPathTB.ForeColor = System.Drawing.Color.Gray;
            this.AlbumArtPathTB.Location = new System.Drawing.Point(4, 191);
            this.AlbumArtPathTB.Name = "AlbumArtPathTB";
            this.AlbumArtPathTB.Size = new System.Drawing.Size(417, 20);
            this.AlbumArtPathTB.TabIndex = 13;
            // 
            // oggPathTB
            // 
            this.oggPathTB.Cue = "Converted audio for Windows on WWise 2010 (.ogg)";
            this.oggPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.oggPathTB.ForeColor = System.Drawing.Color.Gray;
            this.oggPathTB.Location = new System.Drawing.Point(4, 217);
            this.oggPathTB.Name = "oggPathTB";
            this.oggPathTB.Size = new System.Drawing.Size(417, 20);
            this.oggPathTB.TabIndex = 15;
            // 
            // YearTB
            // 
            this.YearTB.Cue = "Release Year";
            this.YearTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.YearTB.ForeColor = System.Drawing.Color.Gray;
            this.YearTB.Location = new System.Drawing.Point(427, 27);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(75, 20);
            this.YearTB.TabIndex = 6;
            // 
            // AlbumTB
            // 
            this.AlbumTB.Cue = "Album";
            this.AlbumTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AlbumTB.ForeColor = System.Drawing.Color.Gray;
            this.AlbumTB.Location = new System.Drawing.Point(3, 27);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(117, 20);
            this.AlbumTB.TabIndex = 3;
            // 
            // ArtistTB
            // 
            this.ArtistTB.Cue = "Artist";
            this.ArtistTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArtistTB.ForeColor = System.Drawing.Color.Gray;
            this.ArtistTB.Location = new System.Drawing.Point(125, 27);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(160, 20);
            this.ArtistTB.TabIndex = 4;
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.Cue = "Song Title";
            this.SongDisplayNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SongDisplayNameTB.ForeColor = System.Drawing.Color.Gray;
            this.SongDisplayNameTB.Location = new System.Drawing.Point(125, 3);
            this.SongDisplayNameTB.Name = "SongDisplayNameTB";
            this.SongDisplayNameTB.Size = new System.Drawing.Size(195, 20);
            this.SongDisplayNameTB.TabIndex = 1;
            // 
            // DlcNameTB
            // 
            this.DlcNameTB.Cue = "DLC Name";
            this.DlcNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DlcNameTB.ForeColor = System.Drawing.Color.Gray;
            this.DlcNameTB.Location = new System.Drawing.Point(3, 3);
            this.DlcNameTB.Name = "DlcNameTB";
            this.DlcNameTB.Size = new System.Drawing.Size(117, 20);
            this.DlcNameTB.TabIndex = 0;
            this.DlcNameTB.Leave += new System.EventHandler(this.DlcNameTB_Leave);
            // 
            // DLCPackageCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ArtistSortTB);
            this.Controls.Add(this.SongDisplayNameSortTB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.platformPS3);
            this.Controls.Add(this.platformXBox360);
            this.Controls.Add(this.platformPC);
            this.Controls.Add(this.openOggXBox360Button);
            this.Controls.Add(this.oggXBox360PathTB);
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
            this.Controls.Add(this.oggPathTB);
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
        private RocksmithTookitGUI.CueTextBox oggPathTB;
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
        private CueTextBox oggXBox360PathTB;
        private System.Windows.Forms.Button openOggXBox360Button;
        private System.Windows.Forms.CheckBox platformPC;
        private System.Windows.Forms.CheckBox platformXBox360;
        private System.Windows.Forms.CheckBox platformPS3;
        private System.Windows.Forms.Label label2;
        private CueTextBox SongDisplayNameSortTB;
        private CueTextBox ArtistSortTB;
    }
}
