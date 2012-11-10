namespace RocksmithTookitGUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.sngFileCreatorTab = new System.Windows.Forms.TabPage();
            this.oggConverterTab = new System.Windows.Forms.TabPage();
            this.dlcPackageCreatorTab = new System.Windows.Forms.TabPage();
            this.albumArtButton = new System.Windows.Forms.Button();
            this.AlbumArtPathTB = new System.Windows.Forms.TextBox();
            this.dlcGenerateButton = new System.Windows.Forms.Button();
            this.openOggButton = new System.Windows.Forms.Button();
            this.OggPathTB = new System.Windows.Forms.TextBox();
            this.arrangementRemoveButton = new System.Windows.Forms.Button();
            this.arrangementAddButton = new System.Windows.Forms.Button();
            this.ArrangementLB = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.YearTB = new System.Windows.Forms.TextBox();
            this.AlbumTB = new System.Windows.Forms.TextBox();
            this.ArtistTB = new System.Windows.Forms.TextBox();
            this.SongDisplayNameTB = new System.Windows.Forms.TextBox();
            this.DlcNameTB = new System.Windows.Forms.TextBox();
            this.dlcPackerUnpackerTab = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.sngFileCreatorControl = new RocksmithTookitGUI.SngFileCreator.SngFileCreator();
            this.oggConverterControl = new RocksmithTookitGUI.OggConverter.OggConverter();
            this.dlcPackerUnpackerControl = new RocksmithTookitGUI.DLCPackerUnpacker.DLCPackerUnpacker();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.sngFileCreatorTab.SuspendLayout();
            this.oggConverterTab.SuspendLayout();
            this.dlcPackageCreatorTab.SuspendLayout();
            this.dlcPackerUnpackerTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(592, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.sngFileCreatorTab);
            this.tabControl1.Controls.Add(this.oggConverterTab);
            this.tabControl1.Controls.Add(this.dlcPackageCreatorTab);
            this.tabControl1.Controls.Add(this.dlcPackerUnpackerTab);
            this.tabControl1.Location = new System.Drawing.Point(12, 128);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(569, 418);
            this.tabControl1.TabIndex = 16;
            // 
            // sngFileCreatorTab
            // 
            this.sngFileCreatorTab.Controls.Add(this.sngFileCreatorControl);
            this.sngFileCreatorTab.Location = new System.Drawing.Point(4, 22);
            this.sngFileCreatorTab.Margin = new System.Windows.Forms.Padding(2);
            this.sngFileCreatorTab.Name = "sngFileCreatorTab";
            this.sngFileCreatorTab.Padding = new System.Windows.Forms.Padding(2);
            this.sngFileCreatorTab.Size = new System.Drawing.Size(561, 392);
            this.sngFileCreatorTab.TabIndex = 1;
            this.sngFileCreatorTab.Text = "SNG File Creator";
            this.sngFileCreatorTab.UseVisualStyleBackColor = true;
            // 
            // oggConverterTab
            // 
            this.oggConverterTab.Controls.Add(this.oggConverterControl);
            this.oggConverterTab.Location = new System.Drawing.Point(4, 22);
            this.oggConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.oggConverterTab.Name = "oggConverterTab";
            this.oggConverterTab.Padding = new System.Windows.Forms.Padding(2);
            this.oggConverterTab.Size = new System.Drawing.Size(561, 392);
            this.oggConverterTab.TabIndex = 2;
            this.oggConverterTab.Text = "OGG Converter";
            this.oggConverterTab.UseVisualStyleBackColor = true;
            // 
            // dlcPackageCreatorTab
            // 
            this.dlcPackageCreatorTab.Controls.Add(this.albumArtButton);
            this.dlcPackageCreatorTab.Controls.Add(this.AlbumArtPathTB);
            this.dlcPackageCreatorTab.Controls.Add(this.dlcGenerateButton);
            this.dlcPackageCreatorTab.Controls.Add(this.openOggButton);
            this.dlcPackageCreatorTab.Controls.Add(this.OggPathTB);
            this.dlcPackageCreatorTab.Controls.Add(this.arrangementRemoveButton);
            this.dlcPackageCreatorTab.Controls.Add(this.arrangementAddButton);
            this.dlcPackageCreatorTab.Controls.Add(this.ArrangementLB);
            this.dlcPackageCreatorTab.Controls.Add(this.label5);
            this.dlcPackageCreatorTab.Controls.Add(this.YearTB);
            this.dlcPackageCreatorTab.Controls.Add(this.AlbumTB);
            this.dlcPackageCreatorTab.Controls.Add(this.ArtistTB);
            this.dlcPackageCreatorTab.Controls.Add(this.SongDisplayNameTB);
            this.dlcPackageCreatorTab.Controls.Add(this.DlcNameTB);
            this.dlcPackageCreatorTab.Location = new System.Drawing.Point(4, 22);
            this.dlcPackageCreatorTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcPackageCreatorTab.Name = "dlcPackageCreatorTab";
            this.dlcPackageCreatorTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcPackageCreatorTab.Size = new System.Drawing.Size(561, 392);
            this.dlcPackageCreatorTab.TabIndex = 0;
            this.dlcPackageCreatorTab.Text = "DLC Package Creator";
            this.dlcPackageCreatorTab.UseVisualStyleBackColor = true;
            // 
            // albumArtButton
            // 
            this.albumArtButton.Location = new System.Drawing.Point(472, 65);
            this.albumArtButton.Name = "albumArtButton";
            this.albumArtButton.Size = new System.Drawing.Size(75, 23);
            this.albumArtButton.TabIndex = 27;
            this.albumArtButton.Text = "...";
            this.albumArtButton.UseVisualStyleBackColor = true;
            this.albumArtButton.Click += new System.EventHandler(this.albumArtButton_Click);
            // 
            // AlbumArtPathTB
            // 
            this.AlbumArtPathTB.Location = new System.Drawing.Point(11, 65);
            this.AlbumArtPathTB.Name = "AlbumArtPathTB";
            this.AlbumArtPathTB.Size = new System.Drawing.Size(454, 20);
            this.AlbumArtPathTB.TabIndex = 26;
            this.AlbumArtPathTB.Text = "Album Art";
            // 
            // dlcGenerateButton
            // 
            this.dlcGenerateButton.Location = new System.Drawing.Point(237, 365);
            this.dlcGenerateButton.Name = "dlcGenerateButton";
            this.dlcGenerateButton.Size = new System.Drawing.Size(75, 23);
            this.dlcGenerateButton.TabIndex = 25;
            this.dlcGenerateButton.Text = "Generate";
            this.dlcGenerateButton.UseVisualStyleBackColor = true;
            this.dlcGenerateButton.Click += new System.EventHandler(this.dlcGenerateButton_Click);
            // 
            // openOggButton
            // 
            this.openOggButton.Location = new System.Drawing.Point(471, 337);
            this.openOggButton.Name = "openOggButton";
            this.openOggButton.Size = new System.Drawing.Size(75, 23);
            this.openOggButton.TabIndex = 24;
            this.openOggButton.Text = "Open";
            this.openOggButton.UseVisualStyleBackColor = true;
            this.openOggButton.Click += new System.EventHandler(this.openOggButton_Click);
            // 
            // OggPathTB
            // 
            this.OggPathTB.Location = new System.Drawing.Point(14, 339);
            this.OggPathTB.Name = "OggPathTB";
            this.OggPathTB.Size = new System.Drawing.Size(451, 20);
            this.OggPathTB.TabIndex = 23;
            this.OggPathTB.Text = "Ogg File";
            // 
            // arrangementRemoveButton
            // 
            this.arrangementRemoveButton.Location = new System.Drawing.Point(472, 137);
            this.arrangementRemoveButton.Name = "arrangementRemoveButton";
            this.arrangementRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementRemoveButton.TabIndex = 22;
            this.arrangementRemoveButton.Text = "Remove";
            this.arrangementRemoveButton.UseVisualStyleBackColor = true;
            this.arrangementRemoveButton.Click += new System.EventHandler(this.arrangementRemoveButton_Click);
            // 
            // arrangementAddButton
            // 
            this.arrangementAddButton.Location = new System.Drawing.Point(472, 107);
            this.arrangementAddButton.Name = "arrangementAddButton";
            this.arrangementAddButton.Size = new System.Drawing.Size(75, 23);
            this.arrangementAddButton.TabIndex = 21;
            this.arrangementAddButton.Text = "Add";
            this.arrangementAddButton.UseVisualStyleBackColor = true;
            this.arrangementAddButton.Click += new System.EventHandler(this.arrangementAddButton_Click);
            // 
            // ArrangementLB
            // 
            this.ArrangementLB.FormattingEnabled = true;
            this.ArrangementLB.Location = new System.Drawing.Point(14, 107);
            this.ArrangementLB.Name = "ArrangementLB";
            this.ArrangementLB.Size = new System.Drawing.Size(451, 225);
            this.ArrangementLB.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Arrangements";
            // 
            // YearTB
            // 
            this.YearTB.Location = new System.Drawing.Point(225, 38);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(100, 20);
            this.YearTB.TabIndex = 18;
            this.YearTB.Text = "1997";
            // 
            // AlbumTB
            // 
            this.AlbumTB.Location = new System.Drawing.Point(118, 38);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(101, 20);
            this.AlbumTB.TabIndex = 17;
            this.AlbumTB.Text = "Album";
            // 
            // ArtistTB
            // 
            this.ArtistTB.Location = new System.Drawing.Point(12, 39);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(100, 20);
            this.ArtistTB.TabIndex = 16;
            this.ArtistTB.Text = "Artist";
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.Location = new System.Drawing.Point(118, 13);
            this.SongDisplayNameTB.Name = "SongDisplayNameTB";
            this.SongDisplayNameTB.Size = new System.Drawing.Size(428, 20);
            this.SongDisplayNameTB.TabIndex = 15;
            this.SongDisplayNameTB.Text = "SongDisplayName";
            // 
            // DlcNameTB
            // 
            this.DlcNameTB.Location = new System.Drawing.Point(12, 13);
            this.DlcNameTB.Name = "DlcNameTB";
            this.DlcNameTB.Size = new System.Drawing.Size(100, 20);
            this.DlcNameTB.TabIndex = 14;
            this.DlcNameTB.Text = "DLCName";
            // 
            // dlcPackerUnpackerTab
            // 
            this.dlcPackerUnpackerTab.Controls.Add(this.dlcPackerUnpackerControl);
            this.dlcPackerUnpackerTab.Location = new System.Drawing.Point(4, 22);
            this.dlcPackerUnpackerTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcPackerUnpackerTab.Name = "dlcPackerUnpackerTab";
            this.dlcPackerUnpackerTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcPackerUnpackerTab.Size = new System.Drawing.Size(561, 392);
            this.dlcPackerUnpackerTab.TabIndex = 3;
            this.dlcPackerUnpackerTab.Text = "DLC Packer/Unpacker";
            this.dlcPackerUnpackerTab.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RocksmithTookitGUI.Properties.Resources.toolkit_logo;
            this.pictureBox1.Location = new System.Drawing.Point(92, 37);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(398, 77);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // sngFileCreatorControl
            // 
            this.sngFileCreatorControl.Location = new System.Drawing.Point(56, 66);
            this.sngFileCreatorControl.Name = "sngFileCreatorControl";
            this.sngFileCreatorControl.Size = new System.Drawing.Size(491, 251);
            this.sngFileCreatorControl.TabIndex = 0;
            // 
            // oggConverterControl
            // 
            this.oggConverterControl.Location = new System.Drawing.Point(56, 66);
            this.oggConverterControl.Name = "oggConverterControl";
            this.oggConverterControl.Size = new System.Drawing.Size(487, 112);
            this.oggConverterControl.TabIndex = 0;
            // 
            // dlcPackerUnpackerControl
            // 
            this.dlcPackerUnpackerControl.Location = new System.Drawing.Point(169, 66);
            this.dlcPackerUnpackerControl.Name = "dlcPackerUnpackerControl";
            this.dlcPackerUnpackerControl.Size = new System.Drawing.Size(217, 110);
            this.dlcPackerUnpackerControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 565);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Custom Song Creator Toolkit";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.sngFileCreatorTab.ResumeLayout(false);
            this.oggConverterTab.ResumeLayout(false);
            this.dlcPackageCreatorTab.ResumeLayout(false);
            this.dlcPackageCreatorTab.PerformLayout();
            this.dlcPackerUnpackerTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage dlcPackageCreatorTab;
        private System.Windows.Forms.TabPage sngFileCreatorTab;
        private System.Windows.Forms.TabPage oggConverterTab;
        private System.Windows.Forms.Button albumArtButton;
        private System.Windows.Forms.TextBox AlbumArtPathTB;
        private System.Windows.Forms.Button dlcGenerateButton;
        private System.Windows.Forms.Button openOggButton;
        private System.Windows.Forms.TextBox OggPathTB;
        private System.Windows.Forms.Button arrangementRemoveButton;
        private System.Windows.Forms.Button arrangementAddButton;
        private System.Windows.Forms.ListBox ArrangementLB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox YearTB;
        private System.Windows.Forms.TextBox AlbumTB;
        private System.Windows.Forms.TextBox ArtistTB;
        private System.Windows.Forms.TextBox SongDisplayNameTB;
        private System.Windows.Forms.TextBox DlcNameTB;
        private SngFileCreator.SngFileCreator sngFileCreatorControl;
        private OggConverter.OggConverter oggConverterControl;
        private System.Windows.Forms.TabPage dlcPackerUnpackerTab;
        private DLCPackerUnpacker.DLCPackerUnpacker dlcPackerUnpackerControl;
    }
}

