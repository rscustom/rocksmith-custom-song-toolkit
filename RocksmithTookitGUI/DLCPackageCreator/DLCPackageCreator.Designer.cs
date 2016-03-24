namespace RocksmithToolkitGUI.DLCPackageCreator
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DLCPackageCreator));
            this.albumArtButton = new System.Windows.Forms.Button();
            this.dlcGenerateButton = new System.Windows.Forms.Button();
            this.openAudioButton = new System.Windows.Forms.Button();
            this.arrangementRemoveButton = new System.Windows.Forms.Button();
            this.arrangementAddButton = new System.Windows.Forms.Button();
            this.arrangementLB = new System.Windows.Forms.ListBox();
            this.dlcSaveButton = new System.Windows.Forms.Button();
            this.dlcLoadButton = new System.Windows.Forms.Button();
            this.cmbAppIds = new System.Windows.Forms.ComboBox();
            this.toneRemoveButton = new System.Windows.Forms.Button();
            this.toneAddButton = new System.Windows.Forms.Button();
            this.tonesLB = new System.Windows.Forms.ListBox();
            this.arrangementEditButton = new System.Windows.Forms.Button();
            this.toneEditButton = new System.Windows.Forms.Button();
            this.toneImportButton = new System.Windows.Forms.Button();
            this.platformPC = new System.Windows.Forms.CheckBox();
            this.platformXBox360 = new System.Windows.Forms.CheckBox();
            this.platformPS3 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rbRs2012 = new System.Windows.Forms.RadioButton();
            this.rbRs2014 = new System.Windows.Forms.RadioButton();
            this.gbPlatofmr = new System.Windows.Forms.GroupBox();
            this.platformMAC = new System.Windows.Forms.CheckBox();
            this.gbFiles = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.albumArtPathTB = new RocksmithToolkitGUI.CueTextBox();
            this.audioQualityBox = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.audioPathTB = new RocksmithToolkitGUI.CueTextBox();
            this.gbTones = new System.Windows.Forms.GroupBox();
            this.toneDuplicateButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkShowlights = new System.Windows.Forms.CheckBox();
            this.btnQuickAdd = new System.Windows.Forms.Button();
            this.keyboardDescArrLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DlcKeyTB = new RocksmithToolkitGUI.CueTextBox();
            this.AlbumSortTB = new RocksmithToolkitGUI.CueTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.previewVolumeBox = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.packageVersionTB = new RocksmithToolkitGUI.CueTextBox();
            this.SongDisplayNameTB = new RocksmithToolkitGUI.CueTextBox();
            this.ArtistTB = new RocksmithToolkitGUI.CueTextBox();
            this.AlbumTB = new RocksmithToolkitGUI.CueTextBox();
            this.YearTB = new RocksmithToolkitGUI.CueTextBox();
            this.AverageTempoTB = new RocksmithToolkitGUI.CueTextBox();
            this.songVolumeBox = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.AppIdTB = new System.Windows.Forms.TextBox();
            this.SongDisplayNameSortTB = new RocksmithToolkitGUI.CueTextBox();
            this.ArtistSortTB = new RocksmithToolkitGUI.CueTextBox();
            this.gbGameVersion = new System.Windows.Forms.GroupBox();
            this.rbConvert = new System.Windows.Forms.RadioButton();
            this.dlcImportButton = new System.Windows.Forms.Button();
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.pbUpdateProgress = new System.Windows.Forms.ProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.gbPlatofmr.SuspendLayout();
            this.gbFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.audioQualityBox)).BeginInit();
            this.gbTones.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewVolumeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.songVolumeBox)).BeginInit();
            this.gbGameVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // albumArtButton
            // 
            this.albumArtButton.Location = new System.Drawing.Point(404, 17);
            this.albumArtButton.Name = "albumArtButton";
            this.albumArtButton.Size = new System.Drawing.Size(34, 23);
            this.albumArtButton.TabIndex = 25;
            this.albumArtButton.Text = "...";
            this.albumArtButton.UseVisualStyleBackColor = true;
            this.albumArtButton.Click += new System.EventHandler(this.albumArtButton_Click);
            // 
            // dlcGenerateButton
            // 
            this.dlcGenerateButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.dlcGenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dlcGenerateButton.Location = new System.Drawing.Point(396, 503);
            this.dlcGenerateButton.Name = "dlcGenerateButton";
            this.dlcGenerateButton.Size = new System.Drawing.Size(97, 29);
            this.dlcGenerateButton.TabIndex = 36;
            this.dlcGenerateButton.Text = "Generate";
            this.dlcGenerateButton.UseVisualStyleBackColor = false;
            this.dlcGenerateButton.Click += new System.EventHandler(this.dlcGenerateButton_Click);
            // 
            // openAudioButton
            // 
            this.openAudioButton.Location = new System.Drawing.Point(404, 43);
            this.openAudioButton.Name = "openAudioButton";
            this.openAudioButton.Size = new System.Drawing.Size(34, 23);
            this.openAudioButton.TabIndex = 27;
            this.openAudioButton.Text = "...";
            this.openAudioButton.UseVisualStyleBackColor = true;
            this.openAudioButton.Click += new System.EventHandler(this.openAudioButton_Click);
            // 
            // arrangementRemoveButton
            // 
            this.arrangementRemoveButton.ForeColor = System.Drawing.Color.IndianRed;
            this.arrangementRemoveButton.Location = new System.Drawing.Point(400, 64);
            this.arrangementRemoveButton.Name = "arrangementRemoveButton";
            this.arrangementRemoveButton.Size = new System.Drawing.Size(92, 23);
            this.arrangementRemoveButton.TabIndex = 23;
            this.arrangementRemoveButton.Text = "Remove";
            this.arrangementRemoveButton.UseVisualStyleBackColor = true;
            this.arrangementRemoveButton.Click += new System.EventHandler(this.arrangementRemoveButton_Click);
            // 
            // arrangementAddButton
            // 
            this.arrangementAddButton.ForeColor = System.Drawing.Color.IndianRed;
            this.arrangementAddButton.Location = new System.Drawing.Point(401, 39);
            this.arrangementAddButton.Name = "arrangementAddButton";
            this.arrangementAddButton.Size = new System.Drawing.Size(45, 23);
            this.arrangementAddButton.TabIndex = 21;
            this.arrangementAddButton.Text = "Add";
            this.arrangementAddButton.UseVisualStyleBackColor = true;
            this.arrangementAddButton.Click += new System.EventHandler(this.arrangementAddButton_Click);
            // 
            // arrangementLB
            // 
            this.arrangementLB.FormattingEnabled = true;
            this.arrangementLB.Location = new System.Drawing.Point(7, 17);
            this.arrangementLB.Name = "arrangementLB";
            this.arrangementLB.ScrollAlwaysVisible = true;
            this.arrangementLB.Size = new System.Drawing.Size(389, 69);
            this.arrangementLB.TabIndex = 20;
            this.arrangementLB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.arrangementLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ArrangementLB_MouseDoubleClick);
            // 
            // dlcSaveButton
            // 
            this.dlcSaveButton.BackColor = System.Drawing.SystemColors.Control;
            this.dlcSaveButton.ForeColor = System.Drawing.Color.IndianRed;
            this.dlcSaveButton.Location = new System.Drawing.Point(116, 503);
            this.dlcSaveButton.Name = "dlcSaveButton";
            this.dlcSaveButton.Size = new System.Drawing.Size(97, 29);
            this.dlcSaveButton.TabIndex = 34;
            this.dlcSaveButton.Text = "Save Package";
            this.dlcSaveButton.UseVisualStyleBackColor = false;
            this.dlcSaveButton.Click += new System.EventHandler(this.dlcSaveButton_Click);
            // 
            // dlcLoadButton
            // 
            this.dlcLoadButton.BackColor = System.Drawing.SystemColors.Control;
            this.dlcLoadButton.ForeColor = System.Drawing.Color.IndianRed;
            this.dlcLoadButton.Location = new System.Drawing.Point(10, 503);
            this.dlcLoadButton.Name = "dlcLoadButton";
            this.dlcLoadButton.Size = new System.Drawing.Size(97, 29);
            this.dlcLoadButton.TabIndex = 0;
            this.dlcLoadButton.Text = "Load Package";
            this.dlcLoadButton.UseVisualStyleBackColor = false;
            this.dlcLoadButton.Click += new System.EventHandler(this.dlcLoadButton_Click);
            // 
            // cmbAppIds
            // 
            this.cmbAppIds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppIds.DropDownWidth = 320;
            this.cmbAppIds.Location = new System.Drawing.Point(193, 69);
            this.cmbAppIds.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAppIds.Name = "cmbAppIds";
            this.cmbAppIds.Size = new System.Drawing.Size(196, 21);
            this.cmbAppIds.TabIndex = 17;
            this.cmbAppIds.SelectedIndexChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // toneRemoveButton
            // 
            this.toneRemoveButton.ForeColor = System.Drawing.Color.IndianRed;
            this.toneRemoveButton.Location = new System.Drawing.Point(447, 43);
            this.toneRemoveButton.Name = "toneRemoveButton";
            this.toneRemoveButton.Size = new System.Drawing.Size(44, 23);
            this.toneRemoveButton.TabIndex = 31;
            this.toneRemoveButton.Text = "Del";
            this.toneRemoveButton.UseVisualStyleBackColor = true;
            this.toneRemoveButton.Click += new System.EventHandler(this.toneRemoveButton_Click);
            // 
            // toneAddButton
            // 
            this.toneAddButton.ForeColor = System.Drawing.Color.IndianRed;
            this.toneAddButton.Location = new System.Drawing.Point(400, 18);
            this.toneAddButton.Name = "toneAddButton";
            this.toneAddButton.Size = new System.Drawing.Size(91, 23);
            this.toneAddButton.TabIndex = 29;
            this.toneAddButton.Text = "Add";
            this.toneAddButton.UseVisualStyleBackColor = true;
            this.toneAddButton.Click += new System.EventHandler(this.toneAddButton_Click);
            // 
            // tonesLB
            // 
            this.tonesLB.FormattingEnabled = true;
            this.tonesLB.Location = new System.Drawing.Point(7, 19);
            this.tonesLB.Name = "tonesLB";
            this.tonesLB.ScrollAlwaysVisible = true;
            this.tonesLB.Size = new System.Drawing.Size(389, 95);
            this.tonesLB.TabIndex = 28;
            this.tonesLB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.tonesLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ToneLB_MouseDoubleClick);
            // 
            // arrangementEditButton
            // 
            this.arrangementEditButton.ForeColor = System.Drawing.Color.IndianRed;
            this.arrangementEditButton.Location = new System.Drawing.Point(447, 39);
            this.arrangementEditButton.Name = "arrangementEditButton";
            this.arrangementEditButton.Size = new System.Drawing.Size(45, 23);
            this.arrangementEditButton.TabIndex = 22;
            this.arrangementEditButton.Text = "Edit";
            this.arrangementEditButton.UseVisualStyleBackColor = true;
            this.arrangementEditButton.Click += new System.EventHandler(this.arrangementEditButton_Click);
            // 
            // toneEditButton
            // 
            this.toneEditButton.ForeColor = System.Drawing.Color.IndianRed;
            this.toneEditButton.Location = new System.Drawing.Point(400, 43);
            this.toneEditButton.Name = "toneEditButton";
            this.toneEditButton.Size = new System.Drawing.Size(45, 23);
            this.toneEditButton.TabIndex = 30;
            this.toneEditButton.Text = "Edit";
            this.toneEditButton.UseVisualStyleBackColor = true;
            this.toneEditButton.Click += new System.EventHandler(this.toneEditButton_Click);
            // 
            // toneImportButton
            // 
            this.toneImportButton.ForeColor = System.Drawing.Color.IndianRed;
            this.toneImportButton.Location = new System.Drawing.Point(400, 92);
            this.toneImportButton.Name = "toneImportButton";
            this.toneImportButton.Size = new System.Drawing.Size(91, 23);
            this.toneImportButton.TabIndex = 33;
            this.toneImportButton.Text = "Import";
            this.toneImportButton.UseVisualStyleBackColor = true;
            this.toneImportButton.Click += new System.EventHandler(this.toneImportButton_Click);
            // 
            // platformPC
            // 
            this.platformPC.AutoSize = true;
            this.platformPC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformPC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformPC.Location = new System.Drawing.Point(8, 17);
            this.platformPC.Name = "platformPC";
            this.platformPC.Size = new System.Drawing.Size(40, 17);
            this.platformPC.TabIndex = 3;
            this.platformPC.Text = "PC";
            this.platformPC.UseVisualStyleBackColor = true;
            this.platformPC.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformXBox360
            // 
            this.platformXBox360.AutoSize = true;
            this.platformXBox360.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformXBox360.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformXBox360.Location = new System.Drawing.Point(103, 17);
            this.platformXBox360.Name = "platformXBox360";
            this.platformXBox360.Size = new System.Drawing.Size(69, 17);
            this.platformXBox360.TabIndex = 5;
            this.platformXBox360.Text = "XBox360";
            this.platformXBox360.UseVisualStyleBackColor = true;
            this.platformXBox360.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformPS3
            // 
            this.platformPS3.AutoSize = true;
            this.platformPS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformPS3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformPS3.Location = new System.Drawing.Point(175, 17);
            this.platformPS3.Name = "platformPS3";
            this.platformPS3.Size = new System.Drawing.Size(46, 17);
            this.platformPS3.TabIndex = 6;
            this.platformPS3.Text = "PS3";
            this.platformPS3.UseVisualStyleBackColor = true;
            this.platformPS3.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(52, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 63;
            this.label3.Text = "dB";
            // 
            // rbRs2012
            // 
            this.rbRs2012.AutoSize = true;
            this.rbRs2012.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbRs2012.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbRs2012.Location = new System.Drawing.Point(8, 17);
            this.rbRs2012.Name = "rbRs2012";
            this.rbRs2012.Size = new System.Drawing.Size(75, 17);
            this.rbRs2012.TabIndex = 1;
            this.rbRs2012.Text = "Rocksmith";
            this.rbRs2012.UseVisualStyleBackColor = true;
            // 
            // rbRs2014
            // 
            this.rbRs2014.AutoSize = true;
            this.rbRs2014.Checked = true;
            this.rbRs2014.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbRs2014.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbRs2014.Location = new System.Drawing.Point(88, 17);
            this.rbRs2014.Name = "rbRs2014";
            this.rbRs2014.Size = new System.Drawing.Size(102, 17);
            this.rbRs2014.TabIndex = 2;
            this.rbRs2014.TabStop = true;
            this.rbRs2014.Text = "Rocksmith 2014";
            this.rbRs2014.UseVisualStyleBackColor = true;
            // 
            // gbPlatofmr
            // 
            this.gbPlatofmr.Controls.Add(this.platformMAC);
            this.gbPlatofmr.Controls.Add(this.platformPS3);
            this.gbPlatofmr.Controls.Add(this.platformXBox360);
            this.gbPlatofmr.Controls.Add(this.platformPC);
            this.gbPlatofmr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPlatofmr.ForeColor = System.Drawing.Color.IndianRed;
            this.gbPlatofmr.Location = new System.Drawing.Point(272, 0);
            this.gbPlatofmr.Name = "gbPlatofmr";
            this.gbPlatofmr.Size = new System.Drawing.Size(229, 41);
            this.gbPlatofmr.TabIndex = 75;
            this.gbPlatofmr.TabStop = false;
            this.gbPlatofmr.Text = "Platform:";
            // 
            // platformMAC
            // 
            this.platformMAC.AutoSize = true;
            this.platformMAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformMAC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformMAC.Location = new System.Drawing.Point(51, 17);
            this.platformMAC.Name = "platformMAC";
            this.platformMAC.Size = new System.Drawing.Size(49, 17);
            this.platformMAC.TabIndex = 4;
            this.platformMAC.Text = "MAC";
            this.platformMAC.UseVisualStyleBackColor = true;
            this.platformMAC.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // gbFiles
            // 
            this.gbFiles.Controls.Add(this.label7);
            this.gbFiles.Controls.Add(this.label2);
            this.gbFiles.Controls.Add(this.albumArtPathTB);
            this.gbFiles.Controls.Add(this.audioQualityBox);
            this.gbFiles.Controls.Add(this.audioPathTB);
            this.gbFiles.Controls.Add(this.openAudioButton);
            this.gbFiles.Controls.Add(this.albumArtButton);
            this.gbFiles.ForeColor = System.Drawing.Color.IndianRed;
            this.gbFiles.Location = new System.Drawing.Point(3, 273);
            this.gbFiles.Name = "gbFiles";
            this.gbFiles.Size = new System.Drawing.Size(498, 88);
            this.gbFiles.TabIndex = 78;
            this.gbFiles.TabStop = false;
            this.gbFiles.Text = "Files";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label7.Location = new System.Drawing.Point(445, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 26);
            this.label7.TabIndex = 38;
            this.label7.Text = "Audio\r\nQuality";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label2.Location = new System.Drawing.Point(4, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(424, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Song preview is generated automatically if not provided in format \'filename_previ" +
                "ew.wem\'\r\n";
            // 
            // albumArtPathTB
            // 
            this.albumArtPathTB.BackColor = System.Drawing.SystemColors.Window;
            this.albumArtPathTB.Cue = "Album Art [use 512x512 size only] (*.dds,*.gif,*.jpg,*.jpeg,*.bmp,*.png)";
            this.albumArtPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.albumArtPathTB.ForeColor = System.Drawing.Color.Gray;
            this.albumArtPathTB.Location = new System.Drawing.Point(7, 19);
            this.albumArtPathTB.Multiline = true;
            this.albumArtPathTB.Name = "albumArtPathTB";
            this.albumArtPathTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.albumArtPathTB.Size = new System.Drawing.Size(389, 20);
            this.albumArtPathTB.TabIndex = 24;
            // 
            // audioQualityBox
            // 
            this.audioQualityBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.audioQualityBox.Location = new System.Drawing.Point(447, 46);
            this.audioQualityBox.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.audioQualityBox.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.audioQualityBox.Name = "audioQualityBox";
            this.audioQualityBox.Size = new System.Drawing.Size(37, 20);
            this.audioQualityBox.TabIndex = 37;
            this.audioQualityBox.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // audioPathTB
            // 
            this.audioPathTB.BackColor = System.Drawing.SystemColors.Window;
            this.audioPathTB.Cue = "Convert compatible audio to Wwise 2013 (*.wem, *.ogg, *.wav)";
            this.audioPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.audioPathTB.ForeColor = System.Drawing.Color.Gray;
            this.audioPathTB.Location = new System.Drawing.Point(7, 45);
            this.audioPathTB.Multiline = true;
            this.audioPathTB.Name = "audioPathTB";
            this.audioPathTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.audioPathTB.Size = new System.Drawing.Size(389, 20);
            this.audioPathTB.TabIndex = 26;
            // 
            // gbTones
            // 
            this.gbTones.Controls.Add(this.toneDuplicateButton);
            this.gbTones.Controls.Add(this.label1);
            this.gbTones.Controls.Add(this.tonesLB);
            this.gbTones.Controls.Add(this.toneAddButton);
            this.gbTones.Controls.Add(this.toneRemoveButton);
            this.gbTones.Controls.Add(this.toneEditButton);
            this.gbTones.Controls.Add(this.toneImportButton);
            this.gbTones.ForeColor = System.Drawing.Color.IndianRed;
            this.gbTones.Location = new System.Drawing.Point(3, 367);
            this.gbTones.Name = "gbTones";
            this.gbTones.Size = new System.Drawing.Size(498, 133);
            this.gbTones.TabIndex = 79;
            this.gbTones.TabStop = false;
            this.gbTones.Text = "Tones";
            // 
            // toneDuplicateButton
            // 
            this.toneDuplicateButton.ForeColor = System.Drawing.Color.IndianRed;
            this.toneDuplicateButton.Location = new System.Drawing.Point(400, 67);
            this.toneDuplicateButton.Name = "toneDuplicateButton";
            this.toneDuplicateButton.Size = new System.Drawing.Size(91, 23);
            this.toneDuplicateButton.TabIndex = 32;
            this.toneDuplicateButton.Text = "Duplicate";
            this.toneDuplicateButton.UseVisualStyleBackColor = true;
            this.toneDuplicateButton.Click += new System.EventHandler(this.toneDuplicateButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label1.Location = new System.Drawing.Point(2, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(495, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Use \"Up/Down\" keys to change order of the tones, use \"Delete\" to delete and \"D\" t" +
                "o duplicate a tone.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkShowlights);
            this.groupBox1.Controls.Add(this.btnQuickAdd);
            this.groupBox1.Controls.Add(this.keyboardDescArrLabel);
            this.groupBox1.Controls.Add(this.arrangementLB);
            this.groupBox1.Controls.Add(this.arrangementAddButton);
            this.groupBox1.Controls.Add(this.arrangementRemoveButton);
            this.groupBox1.Controls.Add(this.arrangementEditButton);
            this.groupBox1.ForeColor = System.Drawing.Color.IndianRed;
            this.groupBox1.Location = new System.Drawing.Point(3, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 107);
            this.groupBox1.TabIndex = 80;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Arrangements";
            // 
            // chkShowlights
            // 
            this.chkShowlights.AutoSize = true;
            this.chkShowlights.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkShowlights.Checked = true;
            this.chkShowlights.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowlights.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowlights.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.chkShowlights.Location = new System.Drawing.Point(376, 88);
            this.chkShowlights.Name = "chkShowlights";
            this.chkShowlights.Size = new System.Drawing.Size(115, 17);
            this.chkShowlights.TabIndex = 38;
            this.chkShowlights.Text = "Include Showlights";
            this.chkShowlights.UseVisualStyleBackColor = true;
            // 
            // btnQuickAdd
            // 
            this.btnQuickAdd.ForeColor = System.Drawing.Color.IndianRed;
            this.btnQuickAdd.Location = new System.Drawing.Point(401, 14);
            this.btnQuickAdd.Name = "btnQuickAdd";
            this.btnQuickAdd.Size = new System.Drawing.Size(91, 23);
            this.btnQuickAdd.TabIndex = 37;
            this.btnQuickAdd.Text = "Quick Add";
            this.btnQuickAdd.UseVisualStyleBackColor = true;
            this.btnQuickAdd.Click += new System.EventHandler(this.btnQuickAdd_Click);
            this.btnQuickAdd.MouseEnter += new System.EventHandler(this.btnQuickAdd_MouseEnter);
            // 
            // keyboardDescArrLabel
            // 
            this.keyboardDescArrLabel.AutoSize = true;
            this.keyboardDescArrLabel.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.keyboardDescArrLabel.Location = new System.Drawing.Point(2, 89);
            this.keyboardDescArrLabel.Name = "keyboardDescArrLabel";
            this.keyboardDescArrLabel.Size = new System.Drawing.Size(336, 13);
            this.keyboardDescArrLabel.TabIndex = 35;
            this.keyboardDescArrLabel.Text = "Use keyboard \"Up/Down\" keys to change order of the arrangements.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbAppIds);
            this.groupBox2.Controls.Add(this.DlcKeyTB);
            this.groupBox2.Controls.Add(this.AlbumSortTB);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.previewVolumeBox);
            this.groupBox2.Controls.Add(this.packageVersionTB);
            this.groupBox2.Controls.Add(this.SongDisplayNameTB);
            this.groupBox2.Controls.Add(this.ArtistTB);
            this.groupBox2.Controls.Add(this.AlbumTB);
            this.groupBox2.Controls.Add(this.YearTB);
            this.groupBox2.Controls.Add(this.AverageTempoTB);
            this.groupBox2.Controls.Add(this.songVolumeBox);
            this.groupBox2.Controls.Add(this.AppIdTB);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.SongDisplayNameSortTB);
            this.groupBox2.Controls.Add(this.ArtistSortTB);
            this.groupBox2.ForeColor = System.Drawing.Color.IndianRed;
            this.groupBox2.Location = new System.Drawing.Point(3, 47);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(498, 107);
            this.groupBox2.TabIndex = 81;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Song Information";
            // 
            // DlcKeyTB
            // 
            this.DlcKeyTB.BackColor = System.Drawing.Color.Snow;
            this.DlcKeyTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DlcKeyTB.Cue = "DLC Key";
            this.DlcKeyTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DlcKeyTB.ForeColor = System.Drawing.Color.Gray;
            this.DlcKeyTB.Location = new System.Drawing.Point(303, -2);
            this.DlcKeyTB.Name = "DlcKeyTB";
            this.DlcKeyTB.Size = new System.Drawing.Size(187, 13);
            this.DlcKeyTB.TabIndex = 7;
            this.DlcKeyTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.DlcKeyTB, resources.GetString("DlcKeyTB.ToolTip"));
            // 
            // AlbumSortTB
            // 
            this.AlbumSortTB.BackColor = System.Drawing.SystemColors.Window;
            this.AlbumSortTB.Cue = "Album Sort";
            this.AlbumSortTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AlbumSortTB.ForeColor = System.Drawing.Color.Gray;
            this.AlbumSortTB.Location = new System.Drawing.Point(303, 43);
            this.AlbumSortTB.Name = "AlbumSortTB";
            this.AlbumSortTB.Size = new System.Drawing.Size(122, 20);
            this.AlbumSortTB.TabIndex = 67;
            this.toolTip.SetToolTip(this.AlbumSortTB, "Album Sort");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label6.Location = new System.Drawing.Point(68, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 66;
            this.label6.Text = "Preview vol.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label5.Location = new System.Drawing.Point(4, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Song vol.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(118, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 65;
            this.label4.Text = "dB";
            // 
            // previewVolumeBox
            // 
            this.previewVolumeBox.DecimalPlaces = 1;
            this.previewVolumeBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.previewVolumeBox.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.previewVolumeBox.Location = new System.Drawing.Point(72, 69);
            this.previewVolumeBox.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.previewVolumeBox.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.previewVolumeBox.Name = "previewVolumeBox";
            this.previewVolumeBox.Size = new System.Drawing.Size(45, 20);
            this.previewVolumeBox.TabIndex = 15;
            this.previewVolumeBox.Value = new decimal(new int[] {
            7,
            0,
            0,
            -2147483648});
            // 
            // packageVersionTB
            // 
            this.packageVersionTB.BackColor = System.Drawing.SystemColors.Window;
            this.packageVersionTB.Cue = "Version";
            this.packageVersionTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.packageVersionTB.ForeColor = System.Drawing.Color.Gray;
            this.packageVersionTB.Location = new System.Drawing.Point(446, 69);
            this.packageVersionTB.MaxLength = 5;
            this.packageVersionTB.Name = "packageVersionTB";
            this.packageVersionTB.Size = new System.Drawing.Size(45, 20);
            this.packageVersionTB.TabIndex = 19;
            this.toolTip.SetToolTip(this.packageVersionTB, "Song Version");
            this.packageVersionTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.packageVersionTB_KeyPress);
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.BackColor = System.Drawing.SystemColors.Window;
            this.SongDisplayNameTB.Cue = "Song Title";
            this.SongDisplayNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SongDisplayNameTB.ForeColor = System.Drawing.Color.Gray;
            this.SongDisplayNameTB.Location = new System.Drawing.Point(165, 17);
            this.SongDisplayNameTB.Name = "SongDisplayNameTB";
            this.SongDisplayNameTB.Size = new System.Drawing.Size(173, 20);
            this.SongDisplayNameTB.TabIndex = 8;
            this.toolTip.SetToolTip(this.SongDisplayNameTB, "Song Title");
            // 
            // ArtistTB
            // 
            this.ArtistTB.BackColor = System.Drawing.SystemColors.Window;
            this.ArtistTB.Cue = "Artist";
            this.ArtistTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArtistTB.ForeColor = System.Drawing.Color.Gray;
            this.ArtistTB.Location = new System.Drawing.Point(8, 17);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(151, 20);
            this.ArtistTB.TabIndex = 11;
            this.toolTip.SetToolTip(this.ArtistTB, "Artist");
            // 
            // AlbumTB
            // 
            this.AlbumTB.BackColor = System.Drawing.SystemColors.Window;
            this.AlbumTB.Cue = "Album";
            this.AlbumTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AlbumTB.ForeColor = System.Drawing.Color.Gray;
            this.AlbumTB.Location = new System.Drawing.Point(344, 17);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(147, 20);
            this.AlbumTB.TabIndex = 10;
            this.toolTip.SetToolTip(this.AlbumTB, "Album");
            // 
            // YearTB
            // 
            this.YearTB.BackColor = System.Drawing.SystemColors.Window;
            this.YearTB.Cue = "Year";
            this.YearTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.YearTB.ForeColor = System.Drawing.Color.Gray;
            this.YearTB.Location = new System.Drawing.Point(431, 43);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(60, 20);
            this.YearTB.TabIndex = 13;
            this.toolTip.SetToolTip(this.YearTB, "Year");
            // 
            // AverageTempoTB
            // 
            this.AverageTempoTB.BackColor = System.Drawing.SystemColors.Window;
            this.AverageTempoTB.Cue = "Tempo";
            this.AverageTempoTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AverageTempoTB.ForeColor = System.Drawing.Color.Gray;
            this.AverageTempoTB.Location = new System.Drawing.Point(394, 69);
            this.AverageTempoTB.Name = "AverageTempoTB";
            this.AverageTempoTB.Size = new System.Drawing.Size(46, 20);
            this.AverageTempoTB.TabIndex = 18;
            this.toolTip.SetToolTip(this.AverageTempoTB, "Average Tempo");
            // 
            // songVolumeBox
            // 
            this.songVolumeBox.DecimalPlaces = 1;
            this.songVolumeBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.songVolumeBox.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.songVolumeBox.Location = new System.Drawing.Point(8, 69);
            this.songVolumeBox.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.songVolumeBox.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.songVolumeBox.Name = "songVolumeBox";
            this.songVolumeBox.Size = new System.Drawing.Size(45, 20);
            this.songVolumeBox.TabIndex = 14;
            this.songVolumeBox.Value = new decimal(new int[] {
            7,
            0,
            0,
            -2147483648});
            this.songVolumeBox.ValueChanged += new System.EventHandler(this.songVolumeBox_ValueChanged);
            // 
            // AppIdTB
            // 
            this.AppIdTB.BackColor = System.Drawing.SystemColors.Window;
            this.AppIdTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AppIdTB.ForeColor = System.Drawing.Color.Gray;
            this.AppIdTB.Location = new System.Drawing.Point(139, 69);
            this.AppIdTB.Name = "AppIdTB";
            this.AppIdTB.Size = new System.Drawing.Size(49, 20);
            this.AppIdTB.TabIndex = 16;
            this.toolTip.SetToolTip(this.AppIdTB, "Specify any valid App ID\r\nby typing it into this box");
            this.AppIdTB.Validating += new System.ComponentModel.CancelEventHandler(this.AppIdTB_Validating);
            // 
            // SongDisplayNameSortTB
            // 
            this.SongDisplayNameSortTB.BackColor = System.Drawing.SystemColors.Window;
            this.SongDisplayNameSortTB.Cue = "Song Title Sort";
            this.SongDisplayNameSortTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SongDisplayNameSortTB.ForeColor = System.Drawing.Color.Gray;
            this.SongDisplayNameSortTB.Location = new System.Drawing.Point(139, 43);
            this.SongDisplayNameSortTB.Name = "SongDisplayNameSortTB";
            this.SongDisplayNameSortTB.Size = new System.Drawing.Size(158, 20);
            this.SongDisplayNameSortTB.TabIndex = 9;
            this.toolTip.SetToolTip(this.SongDisplayNameSortTB, "Song Title Sort");
            // 
            // ArtistSortTB
            // 
            this.ArtistSortTB.BackColor = System.Drawing.SystemColors.Window;
            this.ArtistSortTB.Cue = "Artist Sort";
            this.ArtistSortTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArtistSortTB.ForeColor = System.Drawing.Color.Gray;
            this.ArtistSortTB.Location = new System.Drawing.Point(8, 43);
            this.ArtistSortTB.Name = "ArtistSortTB";
            this.ArtistSortTB.Size = new System.Drawing.Size(125, 20);
            this.ArtistSortTB.TabIndex = 12;
            this.toolTip.SetToolTip(this.ArtistSortTB, "Artist Sort");
            this.ArtistSortTB.TextChanged += new System.EventHandler(this.ArtistSortTB_TextChanged);
            // 
            // gbGameVersion
            // 
            this.gbGameVersion.Controls.Add(this.rbConvert);
            this.gbGameVersion.Controls.Add(this.rbRs2014);
            this.gbGameVersion.Controls.Add(this.rbRs2012);
            this.gbGameVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbGameVersion.ForeColor = System.Drawing.Color.IndianRed;
            this.gbGameVersion.Location = new System.Drawing.Point(3, 0);
            this.gbGameVersion.Name = "gbGameVersion";
            this.gbGameVersion.Size = new System.Drawing.Size(261, 41);
            this.gbGameVersion.TabIndex = 82;
            this.gbGameVersion.TabStop = false;
            this.gbGameVersion.Text = "Game Version";
            // 
            // rbConvert
            // 
            this.rbConvert.AutoSize = true;
            this.rbConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbConvert.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbConvert.Location = new System.Drawing.Point(195, 17);
            this.rbConvert.Name = "rbConvert";
            this.rbConvert.Size = new System.Drawing.Size(62, 17);
            this.rbConvert.TabIndex = 3;
            this.rbConvert.Text = "Convert";
            this.rbConvert.UseVisualStyleBackColor = true;
            // 
            // dlcImportButton
            // 
            this.dlcImportButton.BackColor = System.Drawing.SystemColors.Control;
            this.dlcImportButton.ForeColor = System.Drawing.Color.IndianRed;
            this.dlcImportButton.Location = new System.Drawing.Point(222, 503);
            this.dlcImportButton.Name = "dlcImportButton";
            this.dlcImportButton.Size = new System.Drawing.Size(97, 29);
            this.dlcImportButton.TabIndex = 35;
            this.dlcImportButton.Text = "Import Package";
            this.dlcImportButton.UseVisualStyleBackColor = false;
            this.dlcImportButton.Click += new System.EventHandler(this.dlcImportButton_Click);
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblCurrentOperation.Location = new System.Drawing.Point(8, 538);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCurrentOperation.Size = new System.Drawing.Size(205, 17);
            this.lblCurrentOperation.TabIndex = 0;
            this.lblCurrentOperation.Text = "...";
            this.lblCurrentOperation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCurrentOperation.Visible = false;
            // 
            // pbUpdateProgress
            // 
            this.pbUpdateProgress.Location = new System.Drawing.Point(222, 538);
            this.pbUpdateProgress.Name = "pbUpdateProgress";
            this.pbUpdateProgress.Size = new System.Drawing.Size(271, 17);
            this.pbUpdateProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbUpdateProgress.TabIndex = 999;
            this.pbUpdateProgress.Visible = false;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 9000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // DLCPackageCreator
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lblCurrentOperation);
            this.Controls.Add(this.pbUpdateProgress);
            this.Controls.Add(this.dlcImportButton);
            this.Controls.Add(this.gbGameVersion);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbTones);
            this.Controls.Add(this.gbFiles);
            this.Controls.Add(this.gbPlatofmr);
            this.Controls.Add(this.dlcLoadButton);
            this.Controls.Add(this.dlcSaveButton);
            this.Controls.Add(this.dlcGenerateButton);
            this.Name = "DLCPackageCreator";
            this.Size = new System.Drawing.Size(507, 560);
            this.gbPlatofmr.ResumeLayout(false);
            this.gbPlatofmr.PerformLayout();
            this.gbFiles.ResumeLayout(false);
            this.gbFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.audioQualityBox)).EndInit();
            this.gbTones.ResumeLayout(false);
            this.gbTones.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewVolumeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.songVolumeBox)).EndInit();
            this.gbGameVersion.ResumeLayout(false);
            this.gbGameVersion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button albumArtButton;
        private RocksmithToolkitGUI.CueTextBox albumArtPathTB;
        private System.Windows.Forms.Button dlcGenerateButton;
        private System.Windows.Forms.Button openAudioButton;
        private RocksmithToolkitGUI.CueTextBox audioPathTB;
        private System.Windows.Forms.Button arrangementRemoveButton;
        private System.Windows.Forms.Button arrangementAddButton;
        private RocksmithToolkitGUI.CueTextBox YearTB;
        private RocksmithToolkitGUI.CueTextBox AlbumTB;
        private RocksmithToolkitGUI.CueTextBox ArtistTB;
        private RocksmithToolkitGUI.CueTextBox SongDisplayNameTB;
        private RocksmithToolkitGUI.CueTextBox DlcKeyTB;
        private System.Windows.Forms.Button dlcSaveButton;
        private System.Windows.Forms.Button dlcLoadButton;
        private CueTextBox AverageTempoTB;
        private System.Windows.Forms.TextBox AppIdTB;
        private System.Windows.Forms.ComboBox cmbAppIds;
        private System.Windows.Forms.Button toneRemoveButton;
        private System.Windows.Forms.Button toneAddButton;
        public System.Windows.Forms.ListBox tonesLB;
        private System.Windows.Forms.Button arrangementEditButton;
        private System.Windows.Forms.Button toneEditButton;
        private System.Windows.Forms.Button toneImportButton;
        private System.Windows.Forms.CheckBox platformPC;
        private System.Windows.Forms.CheckBox platformXBox360;
        private System.Windows.Forms.CheckBox platformPS3;
        private CueTextBox SongDisplayNameSortTB;
        private CueTextBox ArtistSortTB;
        private NumericUpDownFixed songVolumeBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbRs2012;
        private System.Windows.Forms.RadioButton rbRs2014;
        private System.Windows.Forms.GroupBox gbPlatofmr;
        private System.Windows.Forms.CheckBox platformMAC;
        private System.Windows.Forms.GroupBox gbFiles;
        private System.Windows.Forms.GroupBox gbTones;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label keyboardDescArrLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gbGameVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button toneDuplicateButton;
        private CueTextBox packageVersionTB;
        private System.Windows.Forms.Button dlcImportButton;
        public System.Windows.Forms.ListBox arrangementLB;
        private System.Windows.Forms.Label lblCurrentOperation;
        private System.Windows.Forms.ProgressBar pbUpdateProgress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private NumericUpDownFixed previewVolumeBox;
        private System.Windows.Forms.Label label7;
        private NumericUpDownFixed audioQualityBox;
        private System.Windows.Forms.RadioButton rbConvert;
        private System.Windows.Forms.Button btnQuickAdd;
        private System.Windows.Forms.CheckBox chkShowlights;
        private CueTextBox AlbumSortTB;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
