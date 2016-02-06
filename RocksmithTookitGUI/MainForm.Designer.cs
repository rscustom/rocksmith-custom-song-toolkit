namespace RocksmithToolkitGUI
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

        protected void LoadTemplate(string path)
        {
            this.dlcPackageCreator1.loadTemplate(path);
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
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.dlcPackageCreatorTab = new System.Windows.Forms.TabPage();
            this.dlcPackerUnpackerTab = new System.Windows.Forms.TabPage();
            this.dlcConverterTab = new System.Windows.Forms.TabPage();
            this.DDCTab = new System.Windows.Forms.TabPage();
            this.dlcInlayCreatorTab = new System.Windows.Forms.TabPage();
            this.sngConverterTab = new System.Windows.Forms.TabPage();
            this.oggConverterTab = new System.Windows.Forms.TabPage();
            this.cdlcConverterTab = new System.Windows.Forms.TabPage();
            this.zigProConverterTab = new System.Windows.Forms.TabPage();
            this.GeneralConfigTab = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.dlcPackageCreator1 = new RocksmithToolkitGUI.DLCPackageCreator.DLCPackageCreator();
            this.dlcPackerUnpacker1 = new RocksmithToolkitGUI.DLCPackerUnpacker.DLCPackerUnpacker();
            this.dlcConverter1 = new RocksmithToolkitGUI.DLCConverter.DLCConverter();
            this.ddc1 = new RocksmithToolkitGUI.DDC.DDC();
            this.dlcInlayCreator1 = new RocksmithToolkitGUI.DLCInlayCreator.DLCInlayCreator();
            this.sngConverter1 = new RocksmithToolkitGUI.SngConverter.SngConverter();
            this.oggConverter1 = new RocksmithToolkitGUI.OggConverter.OggConverter();
            this.cdlC2Tab1 = new RocksmithToolkitGUI.CDLC2Tab.CDLC2Tab();
            this.zpeConverter1 = new RocksmithToolkitGUI.ZiggyProEditorConverter.ZpeConverter();
            this.generalConfig1 = new RocksmithToolkitGUI.Config.GeneralConfig();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.dlcPackageCreatorTab.SuspendLayout();
            this.dlcPackerUnpackerTab.SuspendLayout();
            this.dlcConverterTab.SuspendLayout();
            this.DDCTab.SuspendLayout();
            this.dlcInlayCreatorTab.SuspendLayout();
            this.sngConverterTab.SuspendLayout();
            this.oggConverterTab.SuspendLayout();
            this.cdlcConverterTab.SuspendLayout();
            this.zigProConverterTab.SuspendLayout();
            this.GeneralConfigTab.SuspendLayout();
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
            this.aboutToolStripMenuItem,
            this.configurationToolStripMenuItem});
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
            this.restartToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.configurationToolStripMenuItem.Text = "Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.dlcPackageCreatorTab);
            this.tabControl1.Controls.Add(this.dlcPackerUnpackerTab);
            this.tabControl1.Controls.Add(this.dlcConverterTab);
            this.tabControl1.Controls.Add(this.DDCTab);
            this.tabControl1.Controls.Add(this.dlcInlayCreatorTab);
            this.tabControl1.Controls.Add(this.sngConverterTab);
            this.tabControl1.Controls.Add(this.oggConverterTab);
            this.tabControl1.Controls.Add(this.cdlcConverterTab);
            this.tabControl1.Controls.Add(this.zigProConverterTab);
            this.tabControl1.Controls.Add(this.GeneralConfigTab);
            this.tabControl1.Location = new System.Drawing.Point(17, 100);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(8);
            this.tabControl1.MinimumSize = new System.Drawing.Size(550, 590);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(550, 590);
            this.tabControl1.TabIndex = 16;
            // 
            // dlcPackageCreatorTab
            // 
            this.dlcPackageCreatorTab.Controls.Add(this.dlcPackageCreator1);
            this.dlcPackageCreatorTab.Location = new System.Drawing.Point(4, 22);
            this.dlcPackageCreatorTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcPackageCreatorTab.Name = "dlcPackageCreatorTab";
            this.dlcPackageCreatorTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcPackageCreatorTab.Size = new System.Drawing.Size(542, 564);
            this.dlcPackageCreatorTab.TabIndex = 0;
            this.dlcPackageCreatorTab.Text = "CDLC Creator";
            this.dlcPackageCreatorTab.UseVisualStyleBackColor = true;
            // 
            // dlcPackerUnpackerTab
            // 
            this.dlcPackerUnpackerTab.Controls.Add(this.dlcPackerUnpacker1);
            this.dlcPackerUnpackerTab.Location = new System.Drawing.Point(4, 22);
            this.dlcPackerUnpackerTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcPackerUnpackerTab.Name = "dlcPackerUnpackerTab";
            this.dlcPackerUnpackerTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcPackerUnpackerTab.Size = new System.Drawing.Size(542, 564);
            this.dlcPackerUnpackerTab.TabIndex = 1;
            this.dlcPackerUnpackerTab.Text = "Packer/Unpacker";
            this.dlcPackerUnpackerTab.UseVisualStyleBackColor = true;
            // 
            // dlcConverterTab
            // 
            this.dlcConverterTab.Controls.Add(this.dlcConverter1);
            this.dlcConverterTab.Location = new System.Drawing.Point(4, 22);
            this.dlcConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcConverterTab.Name = "dlcConverterTab";
            this.dlcConverterTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcConverterTab.Size = new System.Drawing.Size(542, 564);
            this.dlcConverterTab.TabIndex = 2;
            this.dlcConverterTab.Text = "Converter";
            this.dlcConverterTab.UseVisualStyleBackColor = true;
            // 
            // DDCTab
            // 
            this.DDCTab.Controls.Add(this.ddc1);
            this.DDCTab.Location = new System.Drawing.Point(4, 22);
            this.DDCTab.Name = "DDCTab";
            this.DDCTab.Padding = new System.Windows.Forms.Padding(3);
            this.DDCTab.Size = new System.Drawing.Size(542, 564);
            this.DDCTab.TabIndex = 3;
            this.DDCTab.Text = "DDC";
            this.DDCTab.ToolTipText = "Generator of low levels for arrangement.";
            this.DDCTab.UseVisualStyleBackColor = true;
            // 
            // dlcInlayCreatorTab
            // 
            this.dlcInlayCreatorTab.Controls.Add(this.dlcInlayCreator1);
            this.dlcInlayCreatorTab.Location = new System.Drawing.Point(4, 22);
            this.dlcInlayCreatorTab.Name = "dlcInlayCreatorTab";
            this.dlcInlayCreatorTab.Padding = new System.Windows.Forms.Padding(3);
            this.dlcInlayCreatorTab.Size = new System.Drawing.Size(542, 564);
            this.dlcInlayCreatorTab.TabIndex = 4;
            this.dlcInlayCreatorTab.Text = "Inlay Creator";
            this.dlcInlayCreatorTab.UseVisualStyleBackColor = true;
            // 
            // sngConverterTab
            // 
            this.sngConverterTab.Controls.Add(this.sngConverter1);
            this.sngConverterTab.Location = new System.Drawing.Point(4, 22);
            this.sngConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.sngConverterTab.Name = "sngConverterTab";
            this.sngConverterTab.Padding = new System.Windows.Forms.Padding(2);
            this.sngConverterTab.Size = new System.Drawing.Size(542, 564);
            this.sngConverterTab.TabIndex = 5;
            this.sngConverterTab.Text = "SNG";
            this.sngConverterTab.UseVisualStyleBackColor = true;
            // 
            // oggConverterTab
            // 
            this.oggConverterTab.Controls.Add(this.oggConverter1);
            this.oggConverterTab.Location = new System.Drawing.Point(4, 22);
            this.oggConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.oggConverterTab.Name = "oggConverterTab";
            this.oggConverterTab.Padding = new System.Windows.Forms.Padding(2);
            this.oggConverterTab.Size = new System.Drawing.Size(542, 564);
            this.oggConverterTab.TabIndex = 6;
            this.oggConverterTab.Text = "OGG";
            this.oggConverterTab.UseVisualStyleBackColor = true;
            // 
            // cdlcConverterTab
            // 
            this.cdlcConverterTab.Controls.Add(this.cdlC2Tab1);
            this.cdlcConverterTab.Location = new System.Drawing.Point(4, 22);
            this.cdlcConverterTab.Name = "cdlcConverterTab";
            this.cdlcConverterTab.Size = new System.Drawing.Size(542, 564);
            this.cdlcConverterTab.TabIndex = 10;
            this.cdlcConverterTab.Text = "CDLC 2 Tab";
            this.cdlcConverterTab.UseVisualStyleBackColor = true;
            // 
            // zigProConverterTab
            // 
            this.zigProConverterTab.Controls.Add(this.zpeConverter1);
            this.zigProConverterTab.Location = new System.Drawing.Point(4, 22);
            this.zigProConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.zigProConverterTab.Name = "zigProConverterTab";
            this.zigProConverterTab.Size = new System.Drawing.Size(542, 564);
            this.zigProConverterTab.TabIndex = 8;
            this.zigProConverterTab.Text = "Ziggy Pro";
            this.zigProConverterTab.UseVisualStyleBackColor = true;
            // 
            // GeneralConfigTab
            // 
            this.GeneralConfigTab.Controls.Add(this.generalConfig1);
            this.GeneralConfigTab.Location = new System.Drawing.Point(4, 22);
            this.GeneralConfigTab.Name = "GeneralConfigTab";
            this.GeneralConfigTab.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralConfigTab.Size = new System.Drawing.Size(542, 564);
            this.GeneralConfigTab.TabIndex = 9;
            this.GeneralConfigTab.Text = "General Config";
            this.GeneralConfigTab.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Image = global::RocksmithToolkitGUI.Properties.Resources.toolkit_logo;
            this.pictureBox1.Location = new System.Drawing.Point(75, 24);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(398, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // updateButton
            // 
            this.updateButton.FlatAppearance.BorderSize = 0;
            this.updateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateButton.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.updateButton.Location = new System.Drawing.Point(467, 0);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(117, 24);
            this.updateButton.TabIndex = 0;
            this.updateButton.Text = "Click here to update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Visible = false;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // dlcPackageCreator1
            // 
            this.dlcPackageCreator1.Album = "";
            this.dlcPackageCreator1.AlbumSort = "";
            this.dlcPackageCreator1.AlbumYear = "";
            this.dlcPackageCreator1.AppId = "";
            this.dlcPackageCreator1.Artist = "";
            this.dlcPackageCreator1.ArtistSort = "";
            this.dlcPackageCreator1.AverageTempo = "";
            this.dlcPackageCreator1.DLCKey = "";
            this.dlcPackageCreator1.JavaBool = false;
            this.dlcPackageCreator1.Location = new System.Drawing.Point(17, 1);
            this.dlcPackageCreator1.LyricArtPath = null;
            this.dlcPackageCreator1.Name = "dlcPackageCreator1";
            this.dlcPackageCreator1.PackageVersion = "";
            this.dlcPackageCreator1.Size = new System.Drawing.Size(507, 560);
            this.dlcPackageCreator1.SongTitle = "";
            this.dlcPackageCreator1.SongTitleSort = "";
            this.dlcPackageCreator1.TabIndex = 0;
            // 
            // dlcPackerUnpacker1
            // 
            this.dlcPackerUnpacker1.Location = new System.Drawing.Point(43, 14);
            this.dlcPackerUnpacker1.MinimumSize = new System.Drawing.Size(400, 308);
            this.dlcPackerUnpacker1.Name = "dlcPackerUnpacker1";
            this.dlcPackerUnpacker1.Size = new System.Drawing.Size(450, 403);
            this.dlcPackerUnpacker1.TabIndex = 0;
            // 
            // dlcConverter1
            // 
            this.dlcConverter1.AppId = "";
            this.dlcConverter1.Location = new System.Drawing.Point(72, 20);
            this.dlcConverter1.MinimumSize = new System.Drawing.Size(400, 279);
            this.dlcConverter1.Name = "dlcConverter1";
            this.dlcConverter1.Size = new System.Drawing.Size(400, 280);
            this.dlcConverter1.SourcePlatform = null;
            this.dlcConverter1.TabIndex = 0;
            this.dlcConverter1.TargetPlatform = null;
            // 
            // ddc1
            // 
            this.ddc1.KeepLog = false;
            this.ddc1.Location = new System.Drawing.Point(6, 6);
            this.ddc1.MinimumSize = new System.Drawing.Size(530, 380);
            this.ddc1.Name = "ddc1";
            this.ddc1.Size = new System.Drawing.Size(530, 419);
            this.ddc1.TabIndex = 0;
            // 
            // dlcInlayCreator1
            // 
            this.dlcInlayCreator1.Location = new System.Drawing.Point(15, 15);
            this.dlcInlayCreator1.Name = "dlcInlayCreator1";
            this.dlcInlayCreator1.Size = new System.Drawing.Size(507, 520);
            this.dlcInlayCreator1.TabIndex = 0;
            // 
            // sngConverter1
            // 
            this.sngConverter1.Location = new System.Drawing.Point(20, 21);
            this.sngConverter1.Name = "sngConverter1";
            this.sngConverter1.Size = new System.Drawing.Size(496, 259);
            this.sngConverter1.TabIndex = 0;
            // 
            // oggConverter1
            // 
            this.oggConverter1.Location = new System.Drawing.Point(19, 23);
            this.oggConverter1.Name = "oggConverter1";
            this.oggConverter1.Size = new System.Drawing.Size(496, 431);
            this.oggConverter1.TabIndex = 0;
            // 
            // cdlC2Tab1
            // 
            this.cdlC2Tab1.Location = new System.Drawing.Point(54, 24);
            this.cdlC2Tab1.Name = "cdlC2Tab1";
            this.cdlC2Tab1.Size = new System.Drawing.Size(420, 209);
            this.cdlC2Tab1.TabIndex = 0;
            // 
            // zpeConverter1
            // 
            this.zpeConverter1.Location = new System.Drawing.Point(18, 19);
            this.zpeConverter1.Name = "zpeConverter1";
            this.zpeConverter1.Size = new System.Drawing.Size(500, 386);
            this.zpeConverter1.TabIndex = 0;
            // 
            // generalConfig1
            // 
            this.generalConfig1.Location = new System.Drawing.Point(9, 1);
            this.generalConfig1.Name = "generalConfig1";
            this.generalConfig1.Size = new System.Drawing.Size(522, 560);
            this.generalConfig1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(592, 694);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 688);
            this.Name = "MainForm";
            this.Text = "Custom Song Creator Toolkit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.dlcPackageCreatorTab.ResumeLayout(false);
            this.dlcPackerUnpackerTab.ResumeLayout(false);
            this.dlcConverterTab.ResumeLayout(false);
            this.DDCTab.ResumeLayout(false);
            this.dlcInlayCreatorTab.ResumeLayout(false);
            this.sngConverterTab.ResumeLayout(false);
            this.oggConverterTab.ResumeLayout(false);
            this.cdlcConverterTab.ResumeLayout(false);
            this.zigProConverterTab.ResumeLayout(false);
            this.GeneralConfigTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage dlcPackageCreatorTab;
        private System.Windows.Forms.TabPage dlcPackerUnpackerTab;
        private System.Windows.Forms.TabPage dlcConverterTab;
        private System.Windows.Forms.TabPage DDCTab;
        private System.Windows.Forms.TabPage dlcInlayCreatorTab;
        private System.Windows.Forms.TabPage sngConverterTab;
        private System.Windows.Forms.TabPage oggConverterTab;
        private System.Windows.Forms.TabPage zigProConverterTab;
        private System.Windows.Forms.TabPage GeneralConfigTab;
        private System.Windows.Forms.TabPage cdlcConverterTab;
        private CDLC2Tab.CDLC2Tab cdlC2Tab1;
        private OggConverter.OggConverter oggConverter1;
        private DLCInlayCreator.DLCInlayCreator dlcInlayCreator1;
        private DLCPackageCreator.DLCPackageCreator dlcPackageCreator1;
        private Config.GeneralConfig generalConfig1;
        private DLCPackerUnpacker.DLCPackerUnpacker dlcPackerUnpacker1;
        private DLCConverter.DLCConverter dlcConverter1;
        private ZiggyProEditorConverter.ZpeConverter zpeConverter1;
        private DDC.DDC ddc1;
        private SngConverter.SngConverter sngConverter1;
     }
}

