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
            this.btnAlbumArt = new System.Windows.Forms.Button();
            this.btnPackageGenerate = new System.Windows.Forms.Button();
            this.btnAudio = new System.Windows.Forms.Button();
            this.btnArrangementRemove = new System.Windows.Forms.Button();
            this.btnArrangementAdd = new System.Windows.Forms.Button();
            this.lstArrangements = new System.Windows.Forms.ListBox();
            this.btnTemplateSave = new System.Windows.Forms.Button();
            this.btnTemplateLoad = new System.Windows.Forms.Button();
            this.cmbAppIds = new System.Windows.Forms.ComboBox();
            this.btnToneRemove = new System.Windows.Forms.Button();
            this.btnToneAdd = new System.Windows.Forms.Button();
            this.lstTones = new System.Windows.Forms.ListBox();
            this.btnArrangementEdit = new System.Windows.Forms.Button();
            this.btnToneEdit = new System.Windows.Forms.Button();
            this.btnToneImport = new System.Windows.Forms.Button();
            this.chkPlatformPC = new System.Windows.Forms.CheckBox();
            this.chkPlatformXBox360 = new System.Windows.Forms.CheckBox();
            this.chkPlatformPS3 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rbRs2012 = new System.Windows.Forms.RadioButton();
            this.rbRs2014 = new System.Windows.Forms.RadioButton();
            this.gbPlatform = new System.Windows.Forms.GroupBox();
            this.chkPlatformMAC = new System.Windows.Forms.CheckBox();
            this.gbFiles = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbTones = new System.Windows.Forms.GroupBox();
            this.btnToneDuplicate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbArrangements = new System.Windows.Forms.GroupBox();
            this.chkShowlights = new System.Windows.Forms.CheckBox();
            this.btnArrangementQuick = new System.Windows.Forms.Button();
            this.keyboardDescArrLabel = new System.Windows.Forms.Label();
            this.gbSongInformation = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAppId = new System.Windows.Forms.TextBox();
            this.gbGameVersion = new System.Windows.Forms.GroupBox();
            this.rbConvert = new System.Windows.Forms.RadioButton();
            this.btnPackageImport = new System.Windows.Forms.Button();
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.pbUpdateProgress = new System.Windows.Forms.ProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnDevUse = new System.Windows.Forms.Button();
            this.txtDlcKey = new RocksmithToolkitGUI.CueTextBox();
            this.txtAlbumSort = new RocksmithToolkitGUI.CueTextBox();
            this.numVolPreview = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.txtVersion = new RocksmithToolkitGUI.CueTextBox();
            this.txtSongTitle = new RocksmithToolkitGUI.CueTextBox();
            this.txtArtist = new RocksmithToolkitGUI.CueTextBox();
            this.txtAlbum = new RocksmithToolkitGUI.CueTextBox();
            this.txtYear = new RocksmithToolkitGUI.CueTextBox();
            this.txtTempo = new RocksmithToolkitGUI.CueTextBox();
            this.numVolSong = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.txtSongTitleSort = new RocksmithToolkitGUI.CueTextBox();
            this.txtArtistSort = new RocksmithToolkitGUI.CueTextBox();
            this.txtAlbumArtPath = new RocksmithToolkitGUI.CueTextBox();
            this.numAudioQuality = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.txtAudioPath = new RocksmithToolkitGUI.CueTextBox();
            this.gbPlatform.SuspendLayout();
            this.gbFiles.SuspendLayout();
            this.gbTones.SuspendLayout();
            this.gbArrangements.SuspendLayout();
            this.gbSongInformation.SuspendLayout();
            this.gbGameVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVolPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVolSong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAudioQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAlbumArt
            // 
            this.btnAlbumArt.Location = new System.Drawing.Point(404, 17);
            this.btnAlbumArt.Name = "btnAlbumArt";
            this.btnAlbumArt.Size = new System.Drawing.Size(34, 23);
            this.btnAlbumArt.TabIndex = 0;
            this.btnAlbumArt.Text = "...";
            this.btnAlbumArt.UseVisualStyleBackColor = true;
            this.btnAlbumArt.Click += new System.EventHandler(this.btnAlbumArt_Click);
            // 
            // btnPackageGenerate
            // 
            this.btnPackageGenerate.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnPackageGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPackageGenerate.Location = new System.Drawing.Point(396, 503);
            this.btnPackageGenerate.Name = "btnPackageGenerate";
            this.btnPackageGenerate.Size = new System.Drawing.Size(97, 29);
            this.btnPackageGenerate.TabIndex = 9;
            this.btnPackageGenerate.Text = "Generate";
            this.btnPackageGenerate.UseVisualStyleBackColor = false;
            this.btnPackageGenerate.Click += new System.EventHandler(this.btnPackageGenerate_Click);
            // 
            // btnAudio
            // 
            this.btnAudio.Location = new System.Drawing.Point(404, 43);
            this.btnAudio.Name = "btnAudio";
            this.btnAudio.Size = new System.Drawing.Size(34, 23);
            this.btnAudio.TabIndex = 1;
            this.btnAudio.Text = "...";
            this.btnAudio.UseVisualStyleBackColor = true;
            this.btnAudio.Click += new System.EventHandler(this.btnAudio_Click);
            // 
            // btnArrangementRemove
            // 
            this.btnArrangementRemove.ForeColor = System.Drawing.Color.IndianRed;
            this.btnArrangementRemove.Location = new System.Drawing.Point(400, 64);
            this.btnArrangementRemove.Name = "btnArrangementRemove";
            this.btnArrangementRemove.Size = new System.Drawing.Size(92, 23);
            this.btnArrangementRemove.TabIndex = 4;
            this.btnArrangementRemove.Text = "Remove";
            this.btnArrangementRemove.UseVisualStyleBackColor = true;
            this.btnArrangementRemove.Click += new System.EventHandler(this.btnArrangementRemove_Click);
            // 
            // btnArrangementAdd
            // 
            this.btnArrangementAdd.ForeColor = System.Drawing.Color.IndianRed;
            this.btnArrangementAdd.Location = new System.Drawing.Point(401, 39);
            this.btnArrangementAdd.Name = "btnArrangementAdd";
            this.btnArrangementAdd.Size = new System.Drawing.Size(40, 23);
            this.btnArrangementAdd.TabIndex = 2;
            this.btnArrangementAdd.Text = "Add";
            this.btnArrangementAdd.UseVisualStyleBackColor = true;
            this.btnArrangementAdd.Click += new System.EventHandler(this.btnArrangementAdd_Click);
            // 
            // lstArrangements
            // 
            this.lstArrangements.FormattingEnabled = true;
            this.lstArrangements.Location = new System.Drawing.Point(7, 17);
            this.lstArrangements.Name = "lstArrangements";
            this.lstArrangements.ScrollAlwaysVisible = true;
            this.lstArrangements.Size = new System.Drawing.Size(389, 69);
            this.lstArrangements.TabIndex = 0;
            this.lstArrangements.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.lstArrangements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstArrangement_MouseDoubleClick);
            // 
            // btnTemplateSave
            // 
            this.btnTemplateSave.BackColor = System.Drawing.SystemColors.Control;
            this.btnTemplateSave.ForeColor = System.Drawing.Color.IndianRed;
            this.btnTemplateSave.Location = new System.Drawing.Point(116, 503);
            this.btnTemplateSave.Name = "btnTemplateSave";
            this.btnTemplateSave.Size = new System.Drawing.Size(97, 29);
            this.btnTemplateSave.TabIndex = 7;
            this.btnTemplateSave.Text = "Save Template";
            this.btnTemplateSave.UseVisualStyleBackColor = false;
            this.btnTemplateSave.Click += new System.EventHandler(this.btnTemplateSave_Click);
            // 
            // btnTemplateLoad
            // 
            this.btnTemplateLoad.BackColor = System.Drawing.SystemColors.Control;
            this.btnTemplateLoad.ForeColor = System.Drawing.Color.IndianRed;
            this.btnTemplateLoad.Location = new System.Drawing.Point(10, 503);
            this.btnTemplateLoad.Name = "btnTemplateLoad";
            this.btnTemplateLoad.Size = new System.Drawing.Size(97, 29);
            this.btnTemplateLoad.TabIndex = 6;
            this.btnTemplateLoad.Text = "Load Template";
            this.btnTemplateLoad.UseVisualStyleBackColor = false;
            this.btnTemplateLoad.Click += new System.EventHandler(this.btnTemplateLoad_Click);
            // 
            // cmbAppIds
            // 
            this.cmbAppIds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppIds.DropDownWidth = 320;
            this.cmbAppIds.Location = new System.Drawing.Point(193, 69);
            this.cmbAppIds.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAppIds.Name = "cmbAppIds";
            this.cmbAppIds.Size = new System.Drawing.Size(196, 21);
            this.cmbAppIds.TabIndex = 12;
            this.cmbAppIds.SelectedIndexChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // btnToneRemove
            // 
            this.btnToneRemove.ForeColor = System.Drawing.Color.IndianRed;
            this.btnToneRemove.Location = new System.Drawing.Point(449, 43);
            this.btnToneRemove.Name = "btnToneRemove";
            this.btnToneRemove.Size = new System.Drawing.Size(42, 23);
            this.btnToneRemove.TabIndex = 3;
            this.btnToneRemove.Text = "Rmv";
            this.toolTip.SetToolTip(this.btnToneRemove, "Removes the selected tone.");
            this.btnToneRemove.UseVisualStyleBackColor = true;
            this.btnToneRemove.Click += new System.EventHandler(this.btnToneRemove_Click);
            // 
            // btnToneAdd
            // 
            this.btnToneAdd.ForeColor = System.Drawing.Color.IndianRed;
            this.btnToneAdd.Location = new System.Drawing.Point(400, 18);
            this.btnToneAdd.Name = "btnToneAdd";
            this.btnToneAdd.Size = new System.Drawing.Size(91, 23);
            this.btnToneAdd.TabIndex = 1;
            this.btnToneAdd.Text = "Add";
            this.btnToneAdd.UseVisualStyleBackColor = true;
            this.btnToneAdd.Click += new System.EventHandler(this.btnToneAdd_Click);
            // 
            // lstTones
            // 
            this.lstTones.FormattingEnabled = true;
            this.lstTones.Location = new System.Drawing.Point(7, 19);
            this.lstTones.Name = "lstTones";
            this.lstTones.ScrollAlwaysVisible = true;
            this.lstTones.Size = new System.Drawing.Size(389, 95);
            this.lstTones.TabIndex = 0;
            this.lstTones.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.lstTones.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstTone_MouseDoubleClick);
            // 
            // btnArrangementEdit
            // 
            this.btnArrangementEdit.ForeColor = System.Drawing.Color.IndianRed;
            this.btnArrangementEdit.Location = new System.Drawing.Point(447, 39);
            this.btnArrangementEdit.Name = "btnArrangementEdit";
            this.btnArrangementEdit.Size = new System.Drawing.Size(45, 23);
            this.btnArrangementEdit.TabIndex = 3;
            this.btnArrangementEdit.Text = "Edit";
            this.btnArrangementEdit.UseVisualStyleBackColor = true;
            this.btnArrangementEdit.Click += new System.EventHandler(this.btnArrangementEdit_Click);
            // 
            // btnToneEdit
            // 
            this.btnToneEdit.ForeColor = System.Drawing.Color.IndianRed;
            this.btnToneEdit.Location = new System.Drawing.Point(401, 43);
            this.btnToneEdit.Name = "btnToneEdit";
            this.btnToneEdit.Size = new System.Drawing.Size(42, 23);
            this.btnToneEdit.TabIndex = 2;
            this.btnToneEdit.Text = "Edit";
            this.btnToneEdit.UseVisualStyleBackColor = true;
            this.btnToneEdit.Click += new System.EventHandler(this.btnToneEdit_Click);
            // 
            // btnToneImport
            // 
            this.btnToneImport.ForeColor = System.Drawing.Color.IndianRed;
            this.btnToneImport.Location = new System.Drawing.Point(449, 68);
            this.btnToneImport.Name = "btnToneImport";
            this.btnToneImport.Size = new System.Drawing.Size(42, 23);
            this.btnToneImport.TabIndex = 5;
            this.btnToneImport.Text = "Imprt";
            this.toolTip.SetToolTip(this.btnToneImport, "Import tone from file");
            this.btnToneImport.UseVisualStyleBackColor = true;
            this.btnToneImport.Click += new System.EventHandler(this.btnToneImport_Click);
            // 
            // chkPlatformPC
            // 
            this.chkPlatformPC.AutoSize = true;
            this.chkPlatformPC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPlatformPC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkPlatformPC.Location = new System.Drawing.Point(8, 17);
            this.chkPlatformPC.Name = "chkPlatformPC";
            this.chkPlatformPC.Size = new System.Drawing.Size(40, 17);
            this.chkPlatformPC.TabIndex = 0;
            this.chkPlatformPC.Text = "PC";
            this.chkPlatformPC.UseVisualStyleBackColor = true;
            this.chkPlatformPC.CheckedChanged += new System.EventHandler(this.chkPlatform_CheckedChanged);
            // 
            // chkPlatformXBox360
            // 
            this.chkPlatformXBox360.AutoSize = true;
            this.chkPlatformXBox360.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPlatformXBox360.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkPlatformXBox360.Location = new System.Drawing.Point(103, 17);
            this.chkPlatformXBox360.Name = "chkPlatformXBox360";
            this.chkPlatformXBox360.Size = new System.Drawing.Size(69, 17);
            this.chkPlatformXBox360.TabIndex = 2;
            this.chkPlatformXBox360.Text = "XBox360";
            this.chkPlatformXBox360.UseVisualStyleBackColor = true;
            this.chkPlatformXBox360.CheckedChanged += new System.EventHandler(this.chkPlatform_CheckedChanged);
            // 
            // chkPlatformPS3
            // 
            this.chkPlatformPS3.AutoSize = true;
            this.chkPlatformPS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPlatformPS3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkPlatformPS3.Location = new System.Drawing.Point(175, 17);
            this.chkPlatformPS3.Name = "chkPlatformPS3";
            this.chkPlatformPS3.Size = new System.Drawing.Size(46, 17);
            this.chkPlatformPS3.TabIndex = 3;
            this.chkPlatformPS3.Text = "PS3";
            this.chkPlatformPS3.UseVisualStyleBackColor = true;
            this.chkPlatformPS3.CheckedChanged += new System.EventHandler(this.chkPlatform_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(52, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 13;
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
            this.rbRs2012.TabIndex = 2;
            this.rbRs2012.TabStop = true;
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
            this.rbRs2014.TabIndex = 0;
            this.rbRs2014.TabStop = true;
            this.rbRs2014.Text = "Rocksmith 2014";
            this.rbRs2014.UseVisualStyleBackColor = true;
            // 
            // gbPlatform
            // 
            this.gbPlatform.Controls.Add(this.chkPlatformMAC);
            this.gbPlatform.Controls.Add(this.chkPlatformPS3);
            this.gbPlatform.Controls.Add(this.chkPlatformXBox360);
            this.gbPlatform.Controls.Add(this.chkPlatformPC);
            this.gbPlatform.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPlatform.ForeColor = System.Drawing.Color.IndianRed;
            this.gbPlatform.Location = new System.Drawing.Point(272, 0);
            this.gbPlatform.Name = "gbPlatform";
            this.gbPlatform.Size = new System.Drawing.Size(229, 41);
            this.gbPlatform.TabIndex = 1;
            this.gbPlatform.TabStop = false;
            this.gbPlatform.Text = "Platform:";
            // 
            // chkPlatformMAC
            // 
            this.chkPlatformMAC.AutoSize = true;
            this.chkPlatformMAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPlatformMAC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkPlatformMAC.Location = new System.Drawing.Point(51, 17);
            this.chkPlatformMAC.Name = "chkPlatformMAC";
            this.chkPlatformMAC.Size = new System.Drawing.Size(49, 17);
            this.chkPlatformMAC.TabIndex = 1;
            this.chkPlatformMAC.Text = "MAC";
            this.chkPlatformMAC.UseVisualStyleBackColor = true;
            this.chkPlatformMAC.CheckedChanged += new System.EventHandler(this.chkPlatform_CheckedChanged);
            // 
            // gbFiles
            // 
            this.gbFiles.Controls.Add(this.label7);
            this.gbFiles.Controls.Add(this.label2);
            this.gbFiles.Controls.Add(this.txtAlbumArtPath);
            this.gbFiles.Controls.Add(this.numAudioQuality);
            this.gbFiles.Controls.Add(this.txtAudioPath);
            this.gbFiles.Controls.Add(this.btnAudio);
            this.gbFiles.Controls.Add(this.btnAlbumArt);
            this.gbFiles.ForeColor = System.Drawing.Color.IndianRed;
            this.gbFiles.Location = new System.Drawing.Point(3, 273);
            this.gbFiles.Name = "gbFiles";
            this.gbFiles.Size = new System.Drawing.Size(498, 88);
            this.gbFiles.TabIndex = 4;
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
            this.label7.TabIndex = 3;
            this.label7.Text = "Audio\r\nQuality";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.MouseHover += new System.EventHandler(this.AudioQuality_MouseEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label2.Location = new System.Drawing.Point(4, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(424, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Song preview is generated automatically if not provided in format \'filename_previ" +
                "ew.wem\'\r\n";
            // 
            // gbTones
            // 
            this.gbTones.Controls.Add(this.btnToneDuplicate);
            this.gbTones.Controls.Add(this.label1);
            this.gbTones.Controls.Add(this.lstTones);
            this.gbTones.Controls.Add(this.btnToneAdd);
            this.gbTones.Controls.Add(this.btnToneRemove);
            this.gbTones.Controls.Add(this.btnToneEdit);
            this.gbTones.Controls.Add(this.btnToneImport);
            this.gbTones.ForeColor = System.Drawing.Color.IndianRed;
            this.gbTones.Location = new System.Drawing.Point(3, 367);
            this.gbTones.Name = "gbTones";
            this.gbTones.Size = new System.Drawing.Size(498, 133);
            this.gbTones.TabIndex = 5;
            this.gbTones.TabStop = false;
            this.gbTones.Text = "Tones";
            // 
            // btnToneDuplicate
            // 
            this.btnToneDuplicate.ForeColor = System.Drawing.Color.IndianRed;
            this.btnToneDuplicate.Location = new System.Drawing.Point(401, 68);
            this.btnToneDuplicate.Name = "btnToneDuplicate";
            this.btnToneDuplicate.Size = new System.Drawing.Size(42, 23);
            this.btnToneDuplicate.TabIndex = 4;
            this.btnToneDuplicate.Text = "Dupl";
            this.toolTip.SetToolTip(this.btnToneDuplicate, "Duplicate selected tone");
            this.btnToneDuplicate.UseVisualStyleBackColor = true;
            this.btnToneDuplicate.Click += new System.EventHandler(this.btnToneDuplicate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label1.Location = new System.Drawing.Point(2, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(495, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Use \"Up/Down\" keys to change order of the tones, use \"Delete\" to delete and \"D\" t" +
                "o duplicate a tone.";
            // 
            // gbArrangements
            // 
            this.gbArrangements.Controls.Add(this.chkShowlights);
            this.gbArrangements.Controls.Add(this.btnArrangementQuick);
            this.gbArrangements.Controls.Add(this.keyboardDescArrLabel);
            this.gbArrangements.Controls.Add(this.lstArrangements);
            this.gbArrangements.Controls.Add(this.btnArrangementAdd);
            this.gbArrangements.Controls.Add(this.btnArrangementRemove);
            this.gbArrangements.Controls.Add(this.btnArrangementEdit);
            this.gbArrangements.ForeColor = System.Drawing.Color.IndianRed;
            this.gbArrangements.Location = new System.Drawing.Point(3, 160);
            this.gbArrangements.Name = "gbArrangements";
            this.gbArrangements.Size = new System.Drawing.Size(498, 107);
            this.gbArrangements.TabIndex = 3;
            this.gbArrangements.TabStop = false;
            this.gbArrangements.Text = "Arrangements";
            // 
            // chkShowlights
            // 
            this.chkShowlights.AutoSize = true;
            this.chkShowlights.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkShowlights.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowlights.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.chkShowlights.Location = new System.Drawing.Point(378, 88);
            this.chkShowlights.Name = "chkShowlights";
            this.chkShowlights.Size = new System.Drawing.Size(114, 17);
            this.chkShowlights.TabIndex = 5;
            this.chkShowlights.Text = "Default Showlights";
            this.toolTip.SetToolTip(this.chkShowlights, resources.GetString("chkShowlights.ToolTip"));
            this.chkShowlights.UseVisualStyleBackColor = true;
            // 
            // btnArrangementQuick
            // 
            this.btnArrangementQuick.ForeColor = System.Drawing.Color.IndianRed;
            this.btnArrangementQuick.Location = new System.Drawing.Point(401, 14);
            this.btnArrangementQuick.Name = "btnArrangementQuick";
            this.btnArrangementQuick.Size = new System.Drawing.Size(91, 23);
            this.btnArrangementQuick.TabIndex = 1;
            this.btnArrangementQuick.Text = "Quick Add";
            this.btnArrangementQuick.UseVisualStyleBackColor = true;
            this.btnArrangementQuick.Click += new System.EventHandler(this.btnArrangementQuick_Click);
            this.btnArrangementQuick.MouseEnter += new System.EventHandler(this.btnArrangementQuick_MouseEnter);
            // 
            // keyboardDescArrLabel
            // 
            this.keyboardDescArrLabel.AutoSize = true;
            this.keyboardDescArrLabel.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.keyboardDescArrLabel.Location = new System.Drawing.Point(2, 89);
            this.keyboardDescArrLabel.Name = "keyboardDescArrLabel";
            this.keyboardDescArrLabel.Size = new System.Drawing.Size(336, 13);
            this.keyboardDescArrLabel.TabIndex = 6;
            this.keyboardDescArrLabel.Text = "Use keyboard \"Up/Down\" keys to change order of the arrangements.";
            // 
            // gbSongInformation
            // 
            this.gbSongInformation.Controls.Add(this.cmbAppIds);
            this.gbSongInformation.Controls.Add(this.txtDlcKey);
            this.gbSongInformation.Controls.Add(this.txtAlbumSort);
            this.gbSongInformation.Controls.Add(this.label6);
            this.gbSongInformation.Controls.Add(this.label5);
            this.gbSongInformation.Controls.Add(this.label4);
            this.gbSongInformation.Controls.Add(this.numVolPreview);
            this.gbSongInformation.Controls.Add(this.txtVersion);
            this.gbSongInformation.Controls.Add(this.txtSongTitle);
            this.gbSongInformation.Controls.Add(this.txtArtist);
            this.gbSongInformation.Controls.Add(this.txtAlbum);
            this.gbSongInformation.Controls.Add(this.txtYear);
            this.gbSongInformation.Controls.Add(this.txtTempo);
            this.gbSongInformation.Controls.Add(this.numVolSong);
            this.gbSongInformation.Controls.Add(this.txtAppId);
            this.gbSongInformation.Controls.Add(this.label3);
            this.gbSongInformation.Controls.Add(this.txtSongTitleSort);
            this.gbSongInformation.Controls.Add(this.txtArtistSort);
            this.gbSongInformation.ForeColor = System.Drawing.Color.IndianRed;
            this.gbSongInformation.Location = new System.Drawing.Point(3, 47);
            this.gbSongInformation.Name = "gbSongInformation";
            this.gbSongInformation.Size = new System.Drawing.Size(498, 107);
            this.gbSongInformation.TabIndex = 2;
            this.gbSongInformation.TabStop = false;
            this.gbSongInformation.Text = "Song Information";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label6.Location = new System.Drawing.Point(68, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Preview vol.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.label5.Location = new System.Drawing.Point(4, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Song vol.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(118, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "dB";
            // 
            // txtAppId
            // 
            this.txtAppId.BackColor = System.Drawing.SystemColors.Window;
            this.txtAppId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAppId.ForeColor = System.Drawing.Color.Gray;
            this.txtAppId.Location = new System.Drawing.Point(139, 69);
            this.txtAppId.Name = "txtAppId";
            this.txtAppId.Size = new System.Drawing.Size(49, 20);
            this.txtAppId.TabIndex = 11;
            this.txtAppId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.txtAppId, "Specify any valid App ID for a song\r\nthat you own by typing it into this box");
            this.txtAppId.Validating += new System.ComponentModel.CancelEventHandler(this.txtAppId_Validating);
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
            this.gbGameVersion.TabIndex = 0;
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
            this.rbConvert.TabIndex = 1;
            this.rbConvert.TabStop = true;
            this.rbConvert.Text = "Convert";
            this.rbConvert.UseVisualStyleBackColor = true;
            // 
            // btnPackageImport
            // 
            this.btnPackageImport.BackColor = System.Drawing.SystemColors.Control;
            this.btnPackageImport.ForeColor = System.Drawing.Color.IndianRed;
            this.btnPackageImport.Location = new System.Drawing.Point(222, 503);
            this.btnPackageImport.Name = "btnPackageImport";
            this.btnPackageImport.Size = new System.Drawing.Size(97, 29);
            this.btnPackageImport.TabIndex = 0;
            this.btnPackageImport.Text = "Import Package";
            this.btnPackageImport.UseVisualStyleBackColor = false;
            this.btnPackageImport.Click += new System.EventHandler(this.btnPackageImport_Click);
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblCurrentOperation.Location = new System.Drawing.Point(8, 538);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCurrentOperation.Size = new System.Drawing.Size(205, 17);
            this.lblCurrentOperation.TabIndex = 10;
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
            this.pbUpdateProgress.TabIndex = 11;
            this.pbUpdateProgress.Visible = false;
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 300;
            this.toolTip.AutoPopDelay = 15000;
            this.toolTip.InitialDelay = 300;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 300;
            // 
            // btnDevUse
            // 
            this.btnDevUse.BackColor = System.Drawing.SystemColors.Control;
            this.btnDevUse.ForeColor = System.Drawing.Color.IndianRed;
            this.btnDevUse.Location = new System.Drawing.Point(331, 503);
            this.btnDevUse.Name = "btnDevUse";
            this.btnDevUse.Size = new System.Drawing.Size(47, 29);
            this.btnDevUse.TabIndex = 8;
            this.btnDevUse.Text = "DEV";
            this.toolTip.SetToolTip(this.btnDevUse, "Developer User Only - For Debugging\r\nUse to load a folder containing song artifac" +
                    "ts\r\n(a previously unpacked CDLC archive)");
            this.btnDevUse.UseVisualStyleBackColor = false;
            this.btnDevUse.Click += new System.EventHandler(this.btnDevUse_Click);
            // 
            // txtDlcKey
            // 
            this.txtDlcKey.BackColor = System.Drawing.Color.Snow;
            this.txtDlcKey.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDlcKey.Cue = "DLC Key";
            this.txtDlcKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtDlcKey.ForeColor = System.Drawing.Color.Gray;
            this.txtDlcKey.Location = new System.Drawing.Point(303, -2);
            this.txtDlcKey.Name = "txtDlcKey";
            this.txtDlcKey.Size = new System.Drawing.Size(187, 13);
            this.txtDlcKey.TabIndex = 3;
            this.txtDlcKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.txtDlcKey, "DLC Key (aka Song Key):\nA unique humanly readable song key.\nNo spaces or special " +
                    "characters allowed.\n\nUse the Configuration menu to \nsave your Charter Name so th" +
                    "at \nDLC Key is auto formatted properly.");
            // 
            // txtAlbumSort
            // 
            this.txtAlbumSort.BackColor = System.Drawing.SystemColors.Window;
            this.txtAlbumSort.Cue = "Album Sort";
            this.txtAlbumSort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAlbumSort.ForeColor = System.Drawing.Color.Gray;
            this.txtAlbumSort.Location = new System.Drawing.Point(303, 43);
            this.txtAlbumSort.Name = "txtAlbumSort";
            this.txtAlbumSort.Size = new System.Drawing.Size(122, 20);
            this.txtAlbumSort.TabIndex = 6;
            this.toolTip.SetToolTip(this.txtAlbumSort, "Album Sort");
            // 
            // numVolPreview
            // 
            this.numVolPreview.DecimalPlaces = 1;
            this.numVolPreview.ForeColor = System.Drawing.SystemColors.ControlText;
            this.numVolPreview.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numVolPreview.Location = new System.Drawing.Point(72, 69);
            this.numVolPreview.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numVolPreview.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numVolPreview.Name = "numVolPreview";
            this.numVolPreview.Size = new System.Drawing.Size(45, 20);
            this.numVolPreview.TabIndex = 9;
            this.numVolPreview.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numVolPreview.Value = new decimal(new int[] {
            7,
            0,
            0,
            -2147483648});
            this.numVolPreview.Enter += new System.EventHandler(this.Volume_MouseEnter);
            // 
            // txtVersion
            // 
            this.txtVersion.BackColor = System.Drawing.SystemColors.Window;
            this.txtVersion.Cue = "Version";
            this.txtVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVersion.ForeColor = System.Drawing.Color.Gray;
            this.txtVersion.Location = new System.Drawing.Point(446, 69);
            this.txtVersion.MaxLength = 5;
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(45, 20);
            this.txtVersion.TabIndex = 14;
            this.txtVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.txtVersion, "Song Version");
            this.txtVersion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVersion_KeyPress);
            // 
            // txtSongTitle
            // 
            this.txtSongTitle.BackColor = System.Drawing.SystemColors.Window;
            this.txtSongTitle.Cue = "Song Title";
            this.txtSongTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtSongTitle.ForeColor = System.Drawing.Color.Gray;
            this.txtSongTitle.Location = new System.Drawing.Point(165, 17);
            this.txtSongTitle.Name = "txtSongTitle";
            this.txtSongTitle.Size = new System.Drawing.Size(173, 20);
            this.txtSongTitle.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtSongTitle, "Song Title");
            // 
            // txtArtist
            // 
            this.txtArtist.BackColor = System.Drawing.SystemColors.Window;
            this.txtArtist.Cue = "Artist";
            this.txtArtist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtArtist.ForeColor = System.Drawing.Color.Gray;
            this.txtArtist.Location = new System.Drawing.Point(8, 17);
            this.txtArtist.Name = "txtArtist";
            this.txtArtist.Size = new System.Drawing.Size(151, 20);
            this.txtArtist.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtArtist, "Artist");
            // 
            // txtAlbum
            // 
            this.txtAlbum.BackColor = System.Drawing.SystemColors.Window;
            this.txtAlbum.Cue = "Album";
            this.txtAlbum.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAlbum.ForeColor = System.Drawing.Color.Gray;
            this.txtAlbum.Location = new System.Drawing.Point(344, 17);
            this.txtAlbum.Name = "txtAlbum";
            this.txtAlbum.Size = new System.Drawing.Size(147, 20);
            this.txtAlbum.TabIndex = 2;
            this.toolTip.SetToolTip(this.txtAlbum, "Album");
            // 
            // txtYear
            // 
            this.txtYear.BackColor = System.Drawing.SystemColors.Window;
            this.txtYear.Cue = "Year";
            this.txtYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtYear.ForeColor = System.Drawing.Color.Gray;
            this.txtYear.Location = new System.Drawing.Point(431, 43);
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(60, 20);
            this.txtYear.TabIndex = 7;
            this.txtYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.txtYear, "Year");
            // 
            // txtTempo
            // 
            this.txtTempo.BackColor = System.Drawing.SystemColors.Window;
            this.txtTempo.Cue = "Tempo";
            this.txtTempo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtTempo.ForeColor = System.Drawing.Color.Gray;
            this.txtTempo.Location = new System.Drawing.Point(394, 69);
            this.txtTempo.Name = "txtTempo";
            this.txtTempo.Size = new System.Drawing.Size(46, 20);
            this.txtTempo.TabIndex = 13;
            this.txtTempo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.txtTempo, "Average Tempo");
            // 
            // numVolSong
            // 
            this.numVolSong.DecimalPlaces = 1;
            this.numVolSong.ForeColor = System.Drawing.SystemColors.ControlText;
            this.numVolSong.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numVolSong.Location = new System.Drawing.Point(8, 69);
            this.numVolSong.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numVolSong.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numVolSong.Name = "numVolSong";
            this.numVolSong.Size = new System.Drawing.Size(45, 20);
            this.numVolSong.TabIndex = 8;
            this.numVolSong.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numVolSong.Value = new decimal(new int[] {
            7,
            0,
            0,
            -2147483648});
            this.numVolSong.ValueChanged += new System.EventHandler(this.numVolSong_ValueChanged);
            this.numVolSong.Enter += new System.EventHandler(this.Volume_MouseEnter);
            // 
            // txtSongTitleSort
            // 
            this.txtSongTitleSort.BackColor = System.Drawing.SystemColors.Window;
            this.txtSongTitleSort.Cue = "Song Title Sort";
            this.txtSongTitleSort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtSongTitleSort.ForeColor = System.Drawing.Color.Gray;
            this.txtSongTitleSort.Location = new System.Drawing.Point(139, 43);
            this.txtSongTitleSort.Name = "txtSongTitleSort";
            this.txtSongTitleSort.Size = new System.Drawing.Size(158, 20);
            this.txtSongTitleSort.TabIndex = 5;
            this.toolTip.SetToolTip(this.txtSongTitleSort, "Song Title Sort");
            // 
            // txtArtistSort
            // 
            this.txtArtistSort.BackColor = System.Drawing.SystemColors.Window;
            this.txtArtistSort.Cue = "Artist Sort";
            this.txtArtistSort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtArtistSort.ForeColor = System.Drawing.Color.Gray;
            this.txtArtistSort.Location = new System.Drawing.Point(8, 43);
            this.txtArtistSort.Name = "txtArtistSort";
            this.txtArtistSort.Size = new System.Drawing.Size(125, 20);
            this.txtArtistSort.TabIndex = 4;
            this.toolTip.SetToolTip(this.txtArtistSort, "Artist Sort");
            // 
            // txtAlbumArtPath
            // 
            this.txtAlbumArtPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtAlbumArtPath.Cue = "Album Art [use 512x512 size only] (*.dds,*.gif,*.jpg,*.jpeg,*.bmp,*.png)";
            this.txtAlbumArtPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAlbumArtPath.ForeColor = System.Drawing.Color.Gray;
            this.txtAlbumArtPath.Location = new System.Drawing.Point(7, 19);
            this.txtAlbumArtPath.Multiline = true;
            this.txtAlbumArtPath.Name = "txtAlbumArtPath";
            this.txtAlbumArtPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAlbumArtPath.Size = new System.Drawing.Size(389, 20);
            this.txtAlbumArtPath.TabIndex = 4;
            // 
            // numAudioQuality
            // 
            this.numAudioQuality.ForeColor = System.Drawing.SystemColors.ControlText;
            this.numAudioQuality.Location = new System.Drawing.Point(447, 46);
            this.numAudioQuality.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numAudioQuality.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numAudioQuality.Name = "numAudioQuality";
            this.numAudioQuality.Size = new System.Drawing.Size(37, 20);
            this.numAudioQuality.TabIndex = 2;
            this.numAudioQuality.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numAudioQuality.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numAudioQuality.Enter += new System.EventHandler(this.AudioQuality_MouseEnter);
            // 
            // txtAudioPath
            // 
            this.txtAudioPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtAudioPath.Cue = "Convert compatible audio to Wwise 2013 (*.wem, *.ogg, *.wav)";
            this.txtAudioPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAudioPath.ForeColor = System.Drawing.Color.Gray;
            this.txtAudioPath.Location = new System.Drawing.Point(7, 45);
            this.txtAudioPath.Multiline = true;
            this.txtAudioPath.Name = "txtAudioPath";
            this.txtAudioPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAudioPath.Size = new System.Drawing.Size(389, 20);
            this.txtAudioPath.TabIndex = 5;
            // 
            // DLCPackageCreator
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.btnDevUse);
            this.Controls.Add(this.lblCurrentOperation);
            this.Controls.Add(this.pbUpdateProgress);
            this.Controls.Add(this.btnPackageImport);
            this.Controls.Add(this.gbGameVersion);
            this.Controls.Add(this.gbSongInformation);
            this.Controls.Add(this.gbArrangements);
            this.Controls.Add(this.gbTones);
            this.Controls.Add(this.gbFiles);
            this.Controls.Add(this.gbPlatform);
            this.Controls.Add(this.btnTemplateLoad);
            this.Controls.Add(this.btnTemplateSave);
            this.Controls.Add(this.btnPackageGenerate);
            this.Name = "DLCPackageCreator";
            this.Size = new System.Drawing.Size(507, 560);
            this.gbPlatform.ResumeLayout(false);
            this.gbPlatform.PerformLayout();
            this.gbFiles.ResumeLayout(false);
            this.gbFiles.PerformLayout();
            this.gbTones.ResumeLayout(false);
            this.gbTones.PerformLayout();
            this.gbArrangements.ResumeLayout(false);
            this.gbArrangements.PerformLayout();
            this.gbSongInformation.ResumeLayout(false);
            this.gbSongInformation.PerformLayout();
            this.gbGameVersion.ResumeLayout(false);
            this.gbGameVersion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVolPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVolSong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAudioQuality)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAlbumArt;
        private RocksmithToolkitGUI.CueTextBox txtAlbumArtPath;
        private System.Windows.Forms.Button btnPackageGenerate;
        private System.Windows.Forms.Button btnAudio;
        private RocksmithToolkitGUI.CueTextBox txtAudioPath;
        private System.Windows.Forms.Button btnArrangementRemove;
        private System.Windows.Forms.Button btnArrangementAdd;
        private RocksmithToolkitGUI.CueTextBox txtYear;
        private RocksmithToolkitGUI.CueTextBox txtAlbum;
        private RocksmithToolkitGUI.CueTextBox txtArtist;
        private RocksmithToolkitGUI.CueTextBox txtSongTitle;
        private RocksmithToolkitGUI.CueTextBox txtDlcKey;
        private System.Windows.Forms.Button btnTemplateSave;
        private System.Windows.Forms.Button btnTemplateLoad;
        private CueTextBox txtTempo;
        private System.Windows.Forms.TextBox txtAppId;
        private System.Windows.Forms.ComboBox cmbAppIds;
        private System.Windows.Forms.Button btnToneRemove;
        private System.Windows.Forms.Button btnToneAdd;
        public System.Windows.Forms.ListBox lstTones;
        private System.Windows.Forms.Button btnArrangementEdit;
        private System.Windows.Forms.Button btnToneEdit;
        private System.Windows.Forms.Button btnToneImport;
        private System.Windows.Forms.CheckBox chkPlatformPC;
        private System.Windows.Forms.CheckBox chkPlatformXBox360;
        private System.Windows.Forms.CheckBox chkPlatformPS3;
        private CueTextBox txtSongTitleSort;
        private CueTextBox txtArtistSort;
        private NumericUpDownFixed numVolSong;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbRs2012;
        private System.Windows.Forms.RadioButton rbRs2014;
        private System.Windows.Forms.GroupBox gbPlatform;
        private System.Windows.Forms.CheckBox chkPlatformMAC;
        private System.Windows.Forms.GroupBox gbFiles;
        private System.Windows.Forms.GroupBox gbTones;
        private System.Windows.Forms.GroupBox gbArrangements;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label keyboardDescArrLabel;
        private System.Windows.Forms.GroupBox gbSongInformation;
        private System.Windows.Forms.GroupBox gbGameVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnToneDuplicate;
        private CueTextBox txtVersion;
        private System.Windows.Forms.Button btnPackageImport;
        public System.Windows.Forms.ListBox lstArrangements;
        private System.Windows.Forms.Label lblCurrentOperation;
        private System.Windows.Forms.ProgressBar pbUpdateProgress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private NumericUpDownFixed numVolPreview;
        private System.Windows.Forms.Label label7;
        private NumericUpDownFixed numAudioQuality;
        private System.Windows.Forms.RadioButton rbConvert;
        private System.Windows.Forms.Button btnArrangementQuick;
        private System.Windows.Forms.CheckBox chkShowlights;
        private CueTextBox txtAlbumSort;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnDevUse;
    }
}
