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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.littleEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.bigEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.vocalsRadioButton = new System.Windows.Forms.RadioButton();
            this.instrumentRadioButton = new System.Windows.Forms.RadioButton();
            this.sngConvertButton = new System.Windows.Forms.Button();
            this.outputFileTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.inputXmlTextBox = new System.Windows.Forms.TextBox();
            this.xmlBrowseButton = new System.Windows.Forms.Button();
            this.oggConverterTab = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.oggLittleEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.oggBigEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.oggConvertButton = new System.Windows.Forms.Button();
            this.outputOggTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.inputOggTextBox = new System.Windows.Forms.TextBox();
            this.oggBrowseButton = new System.Windows.Forms.Button();
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
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.UseCryptographyCheckbox = new System.Windows.Forms.CheckBox();
            this.UnpackButton = new System.Windows.Forms.Button();
            this.PackButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.sngFileCreatorTab.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.oggConverterTab.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.dlcPackageCreatorTab.SuspendLayout();
            this.dlcPackerUnpackerTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
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
            this.menuStrip1.Size = new System.Drawing.Size(790, 28);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(102, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(119, 24);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(119, 24);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.sngFileCreatorTab);
            this.tabControl1.Controls.Add(this.oggConverterTab);
            this.tabControl1.Controls.Add(this.dlcPackageCreatorTab);
            this.tabControl1.Controls.Add(this.dlcPackerUnpackerTab);
            this.tabControl1.Location = new System.Drawing.Point(16, 157);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(759, 514);
            this.tabControl1.TabIndex = 16;
            // 
            // sngFileCreatorTab
            // 
            this.sngFileCreatorTab.Controls.Add(this.groupBox2);
            this.sngFileCreatorTab.Controls.Add(this.groupBox1);
            this.sngFileCreatorTab.Controls.Add(this.sngConvertButton);
            this.sngFileCreatorTab.Controls.Add(this.outputFileTextBox);
            this.sngFileCreatorTab.Controls.Add(this.label2);
            this.sngFileCreatorTab.Controls.Add(this.label1);
            this.sngFileCreatorTab.Controls.Add(this.inputXmlTextBox);
            this.sngFileCreatorTab.Controls.Add(this.xmlBrowseButton);
            this.sngFileCreatorTab.Location = new System.Drawing.Point(4, 25);
            this.sngFileCreatorTab.Name = "sngFileCreatorTab";
            this.sngFileCreatorTab.Padding = new System.Windows.Forms.Padding(3);
            this.sngFileCreatorTab.Size = new System.Drawing.Size(751, 485);
            this.sngFileCreatorTab.TabIndex = 1;
            this.sngFileCreatorTab.Text = "SNG File Creator";
            this.sngFileCreatorTab.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.littleEndianRadioBtn);
            this.groupBox2.Controls.Add(this.bigEndianRadioBtn);
            this.groupBox2.Location = new System.Drawing.Point(91, 323);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(558, 79);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Platform";
            // 
            // littleEndianRadioBtn
            // 
            this.littleEndianRadioBtn.AutoSize = true;
            this.littleEndianRadioBtn.Checked = true;
            this.littleEndianRadioBtn.Location = new System.Drawing.Point(88, 35);
            this.littleEndianRadioBtn.Name = "littleEndianRadioBtn";
            this.littleEndianRadioBtn.Size = new System.Drawing.Size(139, 21);
            this.littleEndianRadioBtn.TabIndex = 6;
            this.littleEndianRadioBtn.TabStop = true;
            this.littleEndianRadioBtn.Text = "PC (Little Endian)";
            this.littleEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // bigEndianRadioBtn
            // 
            this.bigEndianRadioBtn.AutoSize = true;
            this.bigEndianRadioBtn.Location = new System.Drawing.Point(276, 35);
            this.bigEndianRadioBtn.Name = "bigEndianRadioBtn";
            this.bigEndianRadioBtn.Size = new System.Drawing.Size(204, 21);
            this.bigEndianRadioBtn.TabIndex = 7;
            this.bigEndianRadioBtn.Text = "Game Console (Big Endian)";
            this.bigEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.vocalsRadioButton);
            this.groupBox1.Controls.Add(this.instrumentRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(91, 209);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(558, 79);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input File Type";
            // 
            // vocalsRadioButton
            // 
            this.vocalsRadioButton.AutoSize = true;
            this.vocalsRadioButton.Location = new System.Drawing.Point(276, 34);
            this.vocalsRadioButton.Name = "vocalsRadioButton";
            this.vocalsRadioButton.Size = new System.Drawing.Size(120, 21);
            this.vocalsRadioButton.TabIndex = 9;
            this.vocalsRadioButton.Text = "Vocals / Lyrics";
            this.vocalsRadioButton.UseVisualStyleBackColor = true;
            // 
            // instrumentRadioButton
            // 
            this.instrumentRadioButton.AutoSize = true;
            this.instrumentRadioButton.Checked = true;
            this.instrumentRadioButton.Location = new System.Drawing.Point(88, 34);
            this.instrumentRadioButton.Name = "instrumentRadioButton";
            this.instrumentRadioButton.Size = new System.Drawing.Size(95, 21);
            this.instrumentRadioButton.TabIndex = 10;
            this.instrumentRadioButton.TabStop = true;
            this.instrumentRadioButton.Text = "Instrument";
            this.instrumentRadioButton.UseVisualStyleBackColor = true;
            // 
            // sngConvertButton
            // 
            this.sngConvertButton.Location = new System.Drawing.Point(338, 430);
            this.sngConvertButton.Name = "sngConvertButton";
            this.sngConvertButton.Size = new System.Drawing.Size(75, 23);
            this.sngConvertButton.TabIndex = 27;
            this.sngConvertButton.Text = "Convert";
            this.sngConvertButton.UseVisualStyleBackColor = true;
            this.sngConvertButton.Click += new System.EventHandler(this.sngConvertButton_Click);
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Location = new System.Drawing.Point(91, 144);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.ReadOnly = true;
            this.outputFileTextBox.Size = new System.Drawing.Size(558, 22);
            this.outputFileTextBox.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 25;
            this.label2.Text = "Output SNG File:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(88, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "Input XML File:";
            // 
            // inputXmlTextBox
            // 
            this.inputXmlTextBox.Location = new System.Drawing.Point(91, 68);
            this.inputXmlTextBox.Name = "inputXmlTextBox";
            this.inputXmlTextBox.ReadOnly = true;
            this.inputXmlTextBox.Size = new System.Drawing.Size(558, 22);
            this.inputXmlTextBox.TabIndex = 23;
            // 
            // xmlBrowseButton
            // 
            this.xmlBrowseButton.Location = new System.Drawing.Point(655, 67);
            this.xmlBrowseButton.Name = "xmlBrowseButton";
            this.xmlBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.xmlBrowseButton.TabIndex = 22;
            this.xmlBrowseButton.Text = "Browse";
            this.xmlBrowseButton.UseVisualStyleBackColor = true;
            this.xmlBrowseButton.Click += new System.EventHandler(this.xmlBrowseButton_Click);
            // 
            // oggConverterTab
            // 
            this.oggConverterTab.Controls.Add(this.groupBox3);
            this.oggConverterTab.Controls.Add(this.oggConvertButton);
            this.oggConverterTab.Controls.Add(this.outputOggTextBox);
            this.oggConverterTab.Controls.Add(this.label3);
            this.oggConverterTab.Controls.Add(this.label4);
            this.oggConverterTab.Controls.Add(this.inputOggTextBox);
            this.oggConverterTab.Controls.Add(this.oggBrowseButton);
            this.oggConverterTab.Location = new System.Drawing.Point(4, 25);
            this.oggConverterTab.Name = "oggConverterTab";
            this.oggConverterTab.Padding = new System.Windows.Forms.Padding(3);
            this.oggConverterTab.Size = new System.Drawing.Size(751, 485);
            this.oggConverterTab.TabIndex = 2;
            this.oggConverterTab.Text = "OGG Converter";
            this.oggConverterTab.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.oggLittleEndianRadioBtn);
            this.groupBox3.Controls.Add(this.oggBigEndianRadioBtn);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(91, 257);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(558, 79);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Target Platform";
            // 
            // oggLittleEndianRadioBtn
            // 
            this.oggLittleEndianRadioBtn.AutoSize = true;
            this.oggLittleEndianRadioBtn.Checked = true;
            this.oggLittleEndianRadioBtn.Location = new System.Drawing.Point(89, 34);
            this.oggLittleEndianRadioBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.oggLittleEndianRadioBtn.Name = "oggLittleEndianRadioBtn";
            this.oggLittleEndianRadioBtn.Size = new System.Drawing.Size(139, 21);
            this.oggLittleEndianRadioBtn.TabIndex = 6;
            this.oggLittleEndianRadioBtn.TabStop = true;
            this.oggLittleEndianRadioBtn.Text = "PC (Little Endian)";
            this.oggLittleEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // oggBigEndianRadioBtn
            // 
            this.oggBigEndianRadioBtn.AutoSize = true;
            this.oggBigEndianRadioBtn.Location = new System.Drawing.Point(277, 34);
            this.oggBigEndianRadioBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.oggBigEndianRadioBtn.Name = "oggBigEndianRadioBtn";
            this.oggBigEndianRadioBtn.Size = new System.Drawing.Size(204, 21);
            this.oggBigEndianRadioBtn.TabIndex = 7;
            this.oggBigEndianRadioBtn.Text = "Game Console (Big Endian)";
            this.oggBigEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // oggConvertButton
            // 
            this.oggConvertButton.Enabled = false;
            this.oggConvertButton.Location = new System.Drawing.Point(337, 399);
            this.oggConvertButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.oggConvertButton.Name = "oggConvertButton";
            this.oggConvertButton.Size = new System.Drawing.Size(75, 23);
            this.oggConvertButton.TabIndex = 26;
            this.oggConvertButton.Text = "Convert";
            this.oggConvertButton.UseVisualStyleBackColor = true;
            this.oggConvertButton.Click += new System.EventHandler(this.oggConvertButton_Click);
            // 
            // outputOggTextBox
            // 
            this.outputOggTextBox.Location = new System.Drawing.Point(91, 175);
            this.outputOggTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.outputOggTextBox.Name = "outputOggTextBox";
            this.outputOggTextBox.ReadOnly = true;
            this.outputOggTextBox.Size = new System.Drawing.Size(558, 22);
            this.outputOggTextBox.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(88, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 17);
            this.label3.TabIndex = 24;
            this.label3.Text = "Output OGG File:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 17);
            this.label4.TabIndex = 23;
            this.label4.Text = "Input OGG File: (Wwise 2010.3.3)";
            // 
            // inputOggTextBox
            // 
            this.inputOggTextBox.Location = new System.Drawing.Point(91, 99);
            this.inputOggTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.inputOggTextBox.Name = "inputOggTextBox";
            this.inputOggTextBox.ReadOnly = true;
            this.inputOggTextBox.Size = new System.Drawing.Size(558, 22);
            this.inputOggTextBox.TabIndex = 22;
            // 
            // oggBrowseButton
            // 
            this.oggBrowseButton.Location = new System.Drawing.Point(655, 98);
            this.oggBrowseButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.oggBrowseButton.Name = "oggBrowseButton";
            this.oggBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.oggBrowseButton.TabIndex = 21;
            this.oggBrowseButton.Text = "Browse";
            this.oggBrowseButton.UseVisualStyleBackColor = true;
            this.oggBrowseButton.Click += new System.EventHandler(this.oggBrowseButton_Click);
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
            this.dlcPackageCreatorTab.Location = new System.Drawing.Point(4, 25);
            this.dlcPackageCreatorTab.Name = "dlcPackageCreatorTab";
            this.dlcPackageCreatorTab.Padding = new System.Windows.Forms.Padding(3);
            this.dlcPackageCreatorTab.Size = new System.Drawing.Size(751, 485);
            this.dlcPackageCreatorTab.TabIndex = 0;
            this.dlcPackageCreatorTab.Text = "DLC Package Creator";
            this.dlcPackageCreatorTab.UseVisualStyleBackColor = true;
            // 
            // albumArtButton
            // 
            this.albumArtButton.Location = new System.Drawing.Point(630, 80);
            this.albumArtButton.Margin = new System.Windows.Forms.Padding(4);
            this.albumArtButton.Name = "albumArtButton";
            this.albumArtButton.Size = new System.Drawing.Size(100, 28);
            this.albumArtButton.TabIndex = 27;
            this.albumArtButton.Text = "...";
            this.albumArtButton.UseVisualStyleBackColor = true;
            this.albumArtButton.Click += new System.EventHandler(this.albumArtButton_Click);
            // 
            // AlbumArtPathTB
            // 
            this.AlbumArtPathTB.Location = new System.Drawing.Point(15, 80);
            this.AlbumArtPathTB.Margin = new System.Windows.Forms.Padding(4);
            this.AlbumArtPathTB.Name = "AlbumArtPathTB";
            this.AlbumArtPathTB.Size = new System.Drawing.Size(604, 22);
            this.AlbumArtPathTB.TabIndex = 26;
            this.AlbumArtPathTB.Text = "Album Art";
            // 
            // dlcGenerateButton
            // 
            this.dlcGenerateButton.Location = new System.Drawing.Point(316, 449);
            this.dlcGenerateButton.Margin = new System.Windows.Forms.Padding(4);
            this.dlcGenerateButton.Name = "dlcGenerateButton";
            this.dlcGenerateButton.Size = new System.Drawing.Size(100, 28);
            this.dlcGenerateButton.TabIndex = 25;
            this.dlcGenerateButton.Text = "Generate";
            this.dlcGenerateButton.UseVisualStyleBackColor = true;
            this.dlcGenerateButton.Click += new System.EventHandler(this.dlcGenerateButton_Click);
            // 
            // openOggButton
            // 
            this.openOggButton.Location = new System.Drawing.Point(628, 415);
            this.openOggButton.Margin = new System.Windows.Forms.Padding(4);
            this.openOggButton.Name = "openOggButton";
            this.openOggButton.Size = new System.Drawing.Size(100, 28);
            this.openOggButton.TabIndex = 24;
            this.openOggButton.Text = "Open";
            this.openOggButton.UseVisualStyleBackColor = true;
            this.openOggButton.Click += new System.EventHandler(this.openOggButton_Click);
            // 
            // OggPathTB
            // 
            this.OggPathTB.Location = new System.Drawing.Point(19, 417);
            this.OggPathTB.Margin = new System.Windows.Forms.Padding(4);
            this.OggPathTB.Name = "OggPathTB";
            this.OggPathTB.Size = new System.Drawing.Size(600, 22);
            this.OggPathTB.TabIndex = 23;
            this.OggPathTB.Text = "Ogg File";
            // 
            // arrangementRemoveButton
            // 
            this.arrangementRemoveButton.Location = new System.Drawing.Point(630, 169);
            this.arrangementRemoveButton.Margin = new System.Windows.Forms.Padding(4);
            this.arrangementRemoveButton.Name = "arrangementRemoveButton";
            this.arrangementRemoveButton.Size = new System.Drawing.Size(100, 28);
            this.arrangementRemoveButton.TabIndex = 22;
            this.arrangementRemoveButton.Text = "Remove";
            this.arrangementRemoveButton.UseVisualStyleBackColor = true;
            this.arrangementRemoveButton.Click += new System.EventHandler(this.arrangementRemoveButton_Click);
            // 
            // arrangementAddButton
            // 
            this.arrangementAddButton.Location = new System.Drawing.Point(630, 132);
            this.arrangementAddButton.Margin = new System.Windows.Forms.Padding(4);
            this.arrangementAddButton.Name = "arrangementAddButton";
            this.arrangementAddButton.Size = new System.Drawing.Size(100, 28);
            this.arrangementAddButton.TabIndex = 21;
            this.arrangementAddButton.Text = "Add";
            this.arrangementAddButton.UseVisualStyleBackColor = true;
            this.arrangementAddButton.Click += new System.EventHandler(this.arrangementAddButton_Click);
            // 
            // ArrangementLB
            // 
            this.ArrangementLB.FormattingEnabled = true;
            this.ArrangementLB.ItemHeight = 16;
            this.ArrangementLB.Location = new System.Drawing.Point(19, 132);
            this.ArrangementLB.Margin = new System.Windows.Forms.Padding(4);
            this.ArrangementLB.Name = "ArrangementLB";
            this.ArrangementLB.Size = new System.Drawing.Size(600, 276);
            this.ArrangementLB.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 112);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "Arrangements";
            // 
            // YearTB
            // 
            this.YearTB.Location = new System.Drawing.Point(300, 47);
            this.YearTB.Margin = new System.Windows.Forms.Padding(4);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(132, 22);
            this.YearTB.TabIndex = 18;
            this.YearTB.Text = "1997";
            // 
            // AlbumTB
            // 
            this.AlbumTB.Location = new System.Drawing.Point(158, 47);
            this.AlbumTB.Margin = new System.Windows.Forms.Padding(4);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(133, 22);
            this.AlbumTB.TabIndex = 17;
            this.AlbumTB.Text = "Album";
            // 
            // ArtistTB
            // 
            this.ArtistTB.Location = new System.Drawing.Point(16, 48);
            this.ArtistTB.Margin = new System.Windows.Forms.Padding(4);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(132, 22);
            this.ArtistTB.TabIndex = 16;
            this.ArtistTB.Text = "Artist";
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.Location = new System.Drawing.Point(158, 16);
            this.SongDisplayNameTB.Margin = new System.Windows.Forms.Padding(4);
            this.SongDisplayNameTB.Name = "SongDisplayNameTB";
            this.SongDisplayNameTB.Size = new System.Drawing.Size(569, 22);
            this.SongDisplayNameTB.TabIndex = 15;
            this.SongDisplayNameTB.Text = "SongDisplayName";
            // 
            // DlcNameTB
            // 
            this.DlcNameTB.Location = new System.Drawing.Point(16, 16);
            this.DlcNameTB.Margin = new System.Windows.Forms.Padding(4);
            this.DlcNameTB.Name = "DlcNameTB";
            this.DlcNameTB.Size = new System.Drawing.Size(132, 22);
            this.DlcNameTB.TabIndex = 14;
            this.DlcNameTB.Text = "DLCName";
            // 
            // dlcPackerUnpackerTab
            // 
            this.dlcPackerUnpackerTab.Controls.Add(this.pictureBox2);
            this.dlcPackerUnpackerTab.Controls.Add(this.UseCryptographyCheckbox);
            this.dlcPackerUnpackerTab.Controls.Add(this.UnpackButton);
            this.dlcPackerUnpackerTab.Controls.Add(this.PackButton);
            this.dlcPackerUnpackerTab.Location = new System.Drawing.Point(4, 25);
            this.dlcPackerUnpackerTab.Name = "dlcPackerUnpackerTab";
            this.dlcPackerUnpackerTab.Padding = new System.Windows.Forms.Padding(3);
            this.dlcPackerUnpackerTab.Size = new System.Drawing.Size(751, 485);
            this.dlcPackerUnpackerTab.TabIndex = 3;
            this.dlcPackerUnpackerTab.Text = "DLC Packer/Unpacker";
            this.dlcPackerUnpackerTab.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(254, 193);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(133, 127);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // UseCryptographyCheckbox
            // 
            this.UseCryptographyCheckbox.AutoSize = true;
            this.UseCryptographyCheckbox.Checked = true;
            this.UseCryptographyCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseCryptographyCheckbox.Location = new System.Drawing.Point(254, 165);
            this.UseCryptographyCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.UseCryptographyCheckbox.Name = "UseCryptographyCheckbox";
            this.UseCryptographyCheckbox.Size = new System.Drawing.Size(142, 21);
            this.UseCryptographyCheckbox.TabIndex = 6;
            this.UseCryptographyCheckbox.Text = "Use cryptography";
            this.UseCryptographyCheckbox.UseVisualStyleBackColor = true;
            // 
            // UnpackButton
            // 
            this.UnpackButton.Location = new System.Drawing.Point(396, 254);
            this.UnpackButton.Margin = new System.Windows.Forms.Padding(4);
            this.UnpackButton.Name = "UnpackButton";
            this.UnpackButton.Size = new System.Drawing.Size(100, 28);
            this.UnpackButton.TabIndex = 5;
            this.UnpackButton.Text = "Unpack";
            this.UnpackButton.UseVisualStyleBackColor = true;
            this.UnpackButton.Click += new System.EventHandler(this.UnpackButton_Click);
            // 
            // PackButton
            // 
            this.PackButton.Location = new System.Drawing.Point(396, 218);
            this.PackButton.Margin = new System.Windows.Forms.Padding(4);
            this.PackButton.Name = "PackButton";
            this.PackButton.Size = new System.Drawing.Size(100, 28);
            this.PackButton.TabIndex = 4;
            this.PackButton.Text = "Pack";
            this.PackButton.UseVisualStyleBackColor = true;
            this.PackButton.Click += new System.EventHandler(this.PackButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RocksmithTookitGUI.Properties.Resources.toolkit_logo;
            this.pictureBox1.Location = new System.Drawing.Point(122, 46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(530, 95);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 695);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Custom Song Creator Toolkit";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.sngFileCreatorTab.ResumeLayout(false);
            this.sngFileCreatorTab.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.oggConverterTab.ResumeLayout(false);
            this.oggConverterTab.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.dlcPackageCreatorTab.ResumeLayout(false);
            this.dlcPackageCreatorTab.PerformLayout();
            this.dlcPackerUnpackerTab.ResumeLayout(false);
            this.dlcPackerUnpackerTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton littleEndianRadioBtn;
        private System.Windows.Forms.RadioButton bigEndianRadioBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton vocalsRadioButton;
        private System.Windows.Forms.RadioButton instrumentRadioButton;
        private System.Windows.Forms.Button sngConvertButton;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputXmlTextBox;
        private System.Windows.Forms.Button xmlBrowseButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton oggLittleEndianRadioBtn;
        private System.Windows.Forms.RadioButton oggBigEndianRadioBtn;
        private System.Windows.Forms.Button oggConvertButton;
        private System.Windows.Forms.TextBox outputOggTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox inputOggTextBox;
        private System.Windows.Forms.Button oggBrowseButton;
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
        private System.Windows.Forms.TabPage dlcPackerUnpackerTab;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.CheckBox UseCryptographyCheckbox;
        private System.Windows.Forms.Button UnpackButton;
        private System.Windows.Forms.Button PackButton;
    }
}

