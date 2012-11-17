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
            this.toneGroupBox = new System.Windows.Forms.GroupBox();
            this.toneControl = new RocksmithTookitGUI.DLCPackageCreator.ToneControl();
            this.AlbumArtPathTB = new RocksmithTookitGUI.CueTextBox();
            this.OggPathTB = new RocksmithTookitGUI.CueTextBox();
            this.YearTB = new RocksmithTookitGUI.CueTextBox();
            this.AlbumTB = new RocksmithTookitGUI.CueTextBox();
            this.ArtistTB = new RocksmithTookitGUI.CueTextBox();
            this.SongDisplayNameTB = new RocksmithTookitGUI.CueTextBox();
            this.DlcNameTB = new RocksmithTookitGUI.CueTextBox();
            this.toneGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // albumArtButton
            // 
            this.albumArtButton.Location = new System.Drawing.Point(428, 183);
            this.albumArtButton.Name = "albumArtButton";
            this.albumArtButton.Size = new System.Drawing.Size(75, 23);
            this.albumArtButton.TabIndex = 41;
            this.albumArtButton.Text = "Browse";
            this.albumArtButton.UseVisualStyleBackColor = true;
            this.albumArtButton.Click += new System.EventHandler(this.albumArtButton_Click);
            // 
            // dlcGenerateButton
            // 
            this.dlcGenerateButton.Location = new System.Drawing.Point(219, 481);
            this.dlcGenerateButton.Name = "dlcGenerateButton";
            this.dlcGenerateButton.Size = new System.Drawing.Size(75, 29);
            this.dlcGenerateButton.TabIndex = 39;
            this.dlcGenerateButton.Text = "Generate";
            this.dlcGenerateButton.UseVisualStyleBackColor = true;
            this.dlcGenerateButton.Click += new System.EventHandler(this.dlcGenerateButton_Click);
            // 
            // openOggButton
            // 
            this.openOggButton.Location = new System.Drawing.Point(428, 209);
            this.openOggButton.Name = "openOggButton";
            this.openOggButton.Size = new System.Drawing.Size(75, 23);
            this.openOggButton.TabIndex = 38;
            this.openOggButton.Text = "Browse";
            this.openOggButton.UseVisualStyleBackColor = true;
            this.openOggButton.Click += new System.EventHandler(this.openOggButton_Click);
            // 
            // arrangementRemoveButton
            // 
            this.arrangementRemoveButton.Location = new System.Drawing.Point(428, 100);
            this.arrangementRemoveButton.Name = "arrangementRemoveButton";
            this.arrangementRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementRemoveButton.TabIndex = 36;
            this.arrangementRemoveButton.Text = "Remove";
            this.arrangementRemoveButton.UseVisualStyleBackColor = true;
            this.arrangementRemoveButton.Click += new System.EventHandler(this.arrangementRemoveButton_Click);
            // 
            // arrangementAddButton
            // 
            this.arrangementAddButton.Location = new System.Drawing.Point(428, 71);
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
            this.ArrangementLB.Location = new System.Drawing.Point(5, 71);
            this.ArrangementLB.Name = "ArrangementLB";
            this.ArrangementLB.Size = new System.Drawing.Size(417, 108);
            this.ArrangementLB.TabIndex = 34;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Arrangements";
            // 
            // toneGroupBox
            // 
            this.toneGroupBox.Controls.Add(this.toneControl);
            this.toneGroupBox.Location = new System.Drawing.Point(5, 237);
            this.toneGroupBox.Name = "toneGroupBox";
            this.toneGroupBox.Size = new System.Drawing.Size(498, 238);
            this.toneGroupBox.TabIndex = 42;
            this.toneGroupBox.TabStop = false;
            this.toneGroupBox.Text = "Tone Settings";
            // 
            // toneControl
            // 
            this.toneControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toneControl.Location = new System.Drawing.Point(7, 20);
            this.toneControl.Name = "toneControl";
            this.toneControl.Size = new System.Drawing.Size(485, 212);
            this.toneControl.TabIndex = 0;
            // 
            // AlbumArtPathTB
            // 
            this.AlbumArtPathTB.Cue = "Album Art File";
            this.AlbumArtPathTB.Location = new System.Drawing.Point(5, 185);
            this.AlbumArtPathTB.Name = "AlbumArtPathTB";
            this.AlbumArtPathTB.Size = new System.Drawing.Size(417, 20);
            this.AlbumArtPathTB.TabIndex = 40;
            // 
            // OggPathTB
            // 
            this.OggPathTB.Cue = "Converted WWise 2010 .ogg File";
            this.OggPathTB.Location = new System.Drawing.Point(5, 211);
            this.OggPathTB.Name = "OggPathTB";
            this.OggPathTB.Size = new System.Drawing.Size(417, 20);
            this.OggPathTB.TabIndex = 37;
            // 
            // YearTB
            // 
            this.YearTB.Cue = "Release Year";
            this.YearTB.Location = new System.Drawing.Point(312, 28);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(110, 20);
            this.YearTB.TabIndex = 32;
            // 
            // AlbumTB
            // 
            this.AlbumTB.Cue = "Album";
            this.AlbumTB.Location = new System.Drawing.Point(126, 28);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(180, 20);
            this.AlbumTB.TabIndex = 31;
            // 
            // ArtistTB
            // 
            this.ArtistTB.Cue = "Artist";
            this.ArtistTB.Location = new System.Drawing.Point(3, 29);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(117, 20);
            this.ArtistTB.TabIndex = 30;
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.Cue = "Song Title";
            this.SongDisplayNameTB.Location = new System.Drawing.Point(126, 3);
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
            // DLCPackageCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toneGroupBox);
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
            this.Size = new System.Drawing.Size(509, 517);
            this.toneGroupBox.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox toneGroupBox;
        private ToneControl toneControl;
    }
}
