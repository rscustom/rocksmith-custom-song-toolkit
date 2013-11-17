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
            this.albumArtButton = new System.Windows.Forms.Button();
            this.dlcGenerateButton = new System.Windows.Forms.Button();
            this.openOggPcButton = new System.Windows.Forms.Button();
            this.arrangementRemoveButton = new System.Windows.Forms.Button();
            this.arrangementAddButton = new System.Windows.Forms.Button();
            this.ArrangementLB = new System.Windows.Forms.ListBox();
            this.dlcSaveButton = new System.Windows.Forms.Button();
            this.dlcLoadButton = new System.Windows.Forms.Button();
            this.cmbAppIds = new System.Windows.Forms.ComboBox();
            this.toneRemoveButton = new System.Windows.Forms.Button();
            this.toneAddButton = new System.Windows.Forms.Button();
            this.TonesLB = new System.Windows.Forms.ListBox();
            this.arrangementEditButton = new System.Windows.Forms.Button();
            this.toneEditButton = new System.Windows.Forms.Button();
            this.toneImportButton = new System.Windows.Forms.Button();
            this.openOggXBox360Button = new System.Windows.Forms.Button();
            this.platformPC = new System.Windows.Forms.CheckBox();
            this.platformXBox360 = new System.Windows.Forms.CheckBox();
            this.platformPS3 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.openOggPS3Button = new System.Windows.Forms.Button();
            this.RS2012 = new System.Windows.Forms.RadioButton();
            this.RS2014 = new System.Windows.Forms.RadioButton();
            this.gbPlatofmr = new System.Windows.Forms.GroupBox();
            this.platformMAC = new System.Windows.Forms.CheckBox();
            this.openOggMacButton = new System.Windows.Forms.Button();
            this.gbFiles = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AlbumArtPathTB = new RocksmithToolkitGUI.CueTextBox();
            this.oggPcPathTB = new RocksmithToolkitGUI.CueTextBox();
            this.oggMacPathTB = new RocksmithToolkitGUI.CueTextBox();
            this.oggXBox360PathTB = new RocksmithToolkitGUI.CueTextBox();
            this.oggPS3PathTB = new RocksmithToolkitGUI.CueTextBox();
            this.gbTones = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.keyboardDescArrLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DlcNameTB = new RocksmithToolkitGUI.CueTextBox();
            this.SongDisplayNameTB = new RocksmithToolkitGUI.CueTextBox();
            this.ArtistTB = new RocksmithToolkitGUI.CueTextBox();
            this.AlbumTB = new RocksmithToolkitGUI.CueTextBox();
            this.YearTB = new RocksmithToolkitGUI.CueTextBox();
            this.AverageTempoTB = new RocksmithToolkitGUI.CueTextBox();
            this.volumeBox = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.AppIdTB = new RocksmithToolkitGUI.CueTextBox();
            this.SongDisplayNameSortTB = new RocksmithToolkitGUI.CueTextBox();
            this.ArtistSortTB = new RocksmithToolkitGUI.CueTextBox();
            this.gbGameVersion = new System.Windows.Forms.GroupBox();
            this.gbPlatofmr.SuspendLayout();
            this.gbFiles.SuspendLayout();
            this.gbTones.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBox)).BeginInit();
            this.gbGameVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // albumArtButton
            // 
            this.albumArtButton.Location = new System.Drawing.Point(457, 18);
            this.albumArtButton.Name = "albumArtButton";
            this.albumArtButton.Size = new System.Drawing.Size(34, 23);
            this.albumArtButton.TabIndex = 21;
            this.albumArtButton.Text = "...";
            this.albumArtButton.UseVisualStyleBackColor = true;
            this.albumArtButton.Click += new System.EventHandler(this.albumArtButton_Click);
            // 
            // dlcGenerateButton
            // 
            this.dlcGenerateButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.dlcGenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dlcGenerateButton.Location = new System.Drawing.Point(405, 519);
            this.dlcGenerateButton.Name = "dlcGenerateButton";
            this.dlcGenerateButton.Size = new System.Drawing.Size(97, 29);
            this.dlcGenerateButton.TabIndex = 31;
            this.dlcGenerateButton.Text = "Generate";
            this.dlcGenerateButton.UseVisualStyleBackColor = false;
            this.dlcGenerateButton.Click += new System.EventHandler(this.dlcGenerateButton_Click);
            // 
            // openOggPcButton
            // 
            this.openOggPcButton.Location = new System.Drawing.Point(210, 43);
            this.openOggPcButton.Name = "openOggPcButton";
            this.openOggPcButton.Size = new System.Drawing.Size(34, 23);
            this.openOggPcButton.TabIndex = 22;
            this.openOggPcButton.Text = "...";
            this.openOggPcButton.UseVisualStyleBackColor = true;
            this.openOggPcButton.Click += new System.EventHandler(this.openOggButton_Click);
            // 
            // arrangementRemoveButton
            // 
            this.arrangementRemoveButton.Location = new System.Drawing.Point(400, 65);
            this.arrangementRemoveButton.Name = "arrangementRemoveButton";
            this.arrangementRemoveButton.Size = new System.Drawing.Size(91, 23);
            this.arrangementRemoveButton.TabIndex = 20;
            this.arrangementRemoveButton.Text = "Remove";
            this.arrangementRemoveButton.UseVisualStyleBackColor = true;
            this.arrangementRemoveButton.Click += new System.EventHandler(this.arrangementRemoveButton_Click);
            // 
            // arrangementAddButton
            // 
            this.arrangementAddButton.Location = new System.Drawing.Point(400, 16);
            this.arrangementAddButton.Name = "arrangementAddButton";
            this.arrangementAddButton.Size = new System.Drawing.Size(91, 23);
            this.arrangementAddButton.TabIndex = 18;
            this.arrangementAddButton.Text = "Add";
            this.arrangementAddButton.UseVisualStyleBackColor = true;
            this.arrangementAddButton.Click += new System.EventHandler(this.arrangementAddButton_Click);
            // 
            // ArrangementLB
            // 
            this.ArrangementLB.FormattingEnabled = true;
            this.ArrangementLB.Location = new System.Drawing.Point(6, 18);
            this.ArrangementLB.Name = "ArrangementLB";
            this.ArrangementLB.Size = new System.Drawing.Size(389, 69);
            this.ArrangementLB.TabIndex = 34;
            this.ArrangementLB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.ArrangementLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ArrangementLB_MouseDoubleClick);
            // 
            // dlcSaveButton
            // 
            this.dlcSaveButton.BackColor = System.Drawing.SystemColors.Control;
            this.dlcSaveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.dlcSaveButton.Location = new System.Drawing.Point(105, 519);
            this.dlcSaveButton.Name = "dlcSaveButton";
            this.dlcSaveButton.Size = new System.Drawing.Size(97, 29);
            this.dlcSaveButton.TabIndex = 30;
            this.dlcSaveButton.Text = "Save Package";
            this.dlcSaveButton.UseVisualStyleBackColor = false;
            this.dlcSaveButton.Click += new System.EventHandler(this.dlcSaveButton_Click);
            // 
            // dlcLoadButton
            // 
            this.dlcLoadButton.BackColor = System.Drawing.SystemColors.Control;
            this.dlcLoadButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.dlcLoadButton.Location = new System.Drawing.Point(2, 519);
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
            this.cmbAppIds.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbAppIds.Location = new System.Drawing.Point(197, 68);
            this.cmbAppIds.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAppIds.Name = "cmbAppIds";
            this.cmbAppIds.Size = new System.Drawing.Size(198, 21);
            this.cmbAppIds.TabIndex = 16;
            this.cmbAppIds.SelectedIndexChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // toneRemoveButton
            // 
            this.toneRemoveButton.Location = new System.Drawing.Point(400, 67);
            this.toneRemoveButton.Name = "toneRemoveButton";
            this.toneRemoveButton.Size = new System.Drawing.Size(91, 23);
            this.toneRemoveButton.TabIndex = 28;
            this.toneRemoveButton.Text = "Remove";
            this.toneRemoveButton.UseVisualStyleBackColor = true;
            this.toneRemoveButton.Click += new System.EventHandler(this.toneRemoveButton_Click);
            // 
            // toneAddButton
            // 
            this.toneAddButton.Location = new System.Drawing.Point(400, 18);
            this.toneAddButton.Name = "toneAddButton";
            this.toneAddButton.Size = new System.Drawing.Size(91, 23);
            this.toneAddButton.TabIndex = 26;
            this.toneAddButton.Text = "Add";
            this.toneAddButton.UseVisualStyleBackColor = true;
            this.toneAddButton.Click += new System.EventHandler(this.toneAddButton_Click);
            // 
            // TonesLB
            // 
            this.TonesLB.FormattingEnabled = true;
            this.TonesLB.Location = new System.Drawing.Point(6, 19);
            this.TonesLB.Name = "TonesLB";
            this.TonesLB.Size = new System.Drawing.Size(389, 95);
            this.TonesLB.TabIndex = 50;
            this.TonesLB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.TonesLB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ToneLB_MouseDoubleClick);
            // 
            // arrangementEditButton
            // 
            this.arrangementEditButton.Location = new System.Drawing.Point(400, 40);
            this.arrangementEditButton.Name = "arrangementEditButton";
            this.arrangementEditButton.Size = new System.Drawing.Size(91, 23);
            this.arrangementEditButton.TabIndex = 19;
            this.arrangementEditButton.Text = "Edit";
            this.arrangementEditButton.UseVisualStyleBackColor = true;
            this.arrangementEditButton.Click += new System.EventHandler(this.arrangementEditButton_Click);
            // 
            // toneEditButton
            // 
            this.toneEditButton.Location = new System.Drawing.Point(400, 42);
            this.toneEditButton.Name = "toneEditButton";
            this.toneEditButton.Size = new System.Drawing.Size(91, 23);
            this.toneEditButton.TabIndex = 27;
            this.toneEditButton.Text = "Edit";
            this.toneEditButton.UseVisualStyleBackColor = true;
            this.toneEditButton.Click += new System.EventHandler(this.toneEditButton_Click);
            // 
            // toneImportButton
            // 
            this.toneImportButton.Location = new System.Drawing.Point(400, 92);
            this.toneImportButton.Name = "toneImportButton";
            this.toneImportButton.Size = new System.Drawing.Size(91, 23);
            this.toneImportButton.TabIndex = 29;
            this.toneImportButton.Text = "Import";
            this.toneImportButton.UseVisualStyleBackColor = true;
            this.toneImportButton.Click += new System.EventHandler(this.toneImportButton_Click);
            // 
            // openOggXBox360Button
            // 
            this.openOggXBox360Button.Enabled = false;
            this.openOggXBox360Button.Location = new System.Drawing.Point(457, 43);
            this.openOggXBox360Button.Name = "openOggXBox360Button";
            this.openOggXBox360Button.Size = new System.Drawing.Size(34, 23);
            this.openOggXBox360Button.TabIndex = 24;
            this.openOggXBox360Button.Text = "...";
            this.openOggXBox360Button.UseVisualStyleBackColor = true;
            this.openOggXBox360Button.Click += new System.EventHandler(this.openOggXBox360Button_Click);
            // 
            // platformPC
            // 
            this.platformPC.AutoSize = true;
            this.platformPC.Checked = true;
            this.platformPC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformPC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformPC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformPC.Location = new System.Drawing.Point(9, 19);
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
            this.platformXBox360.Location = new System.Drawing.Point(150, 19);
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
            this.platformPS3.Location = new System.Drawing.Point(248, 19);
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
            this.label3.Location = new System.Drawing.Point(97, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 63;
            this.label3.Text = "dB";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(8, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 62;
            this.label4.Text = "Volume";
            // 
            // openOggPS3Button
            // 
            this.openOggPS3Button.Enabled = false;
            this.openOggPS3Button.Location = new System.Drawing.Point(457, 69);
            this.openOggPS3Button.Name = "openOggPS3Button";
            this.openOggPS3Button.Size = new System.Drawing.Size(34, 23);
            this.openOggPS3Button.TabIndex = 25;
            this.openOggPS3Button.Text = "...";
            this.openOggPS3Button.UseVisualStyleBackColor = true;
            this.openOggPS3Button.Click += new System.EventHandler(this.openOggPS3Button_Click);
            // 
            // RS2012
            // 
            this.RS2012.AutoSize = true;
            this.RS2012.Checked = true;
            this.RS2012.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RS2012.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RS2012.Location = new System.Drawing.Point(8, 18);
            this.RS2012.Name = "RS2012";
            this.RS2012.Size = new System.Drawing.Size(75, 17);
            this.RS2012.TabIndex = 1;
            this.RS2012.TabStop = true;
            this.RS2012.Text = "Rocksmith";
            this.RS2012.UseVisualStyleBackColor = true;
            this.RS2012.CheckedChanged += new System.EventHandler(this.GameVersion_CheckedChanged);
            // 
            // RS2014
            // 
            this.RS2014.AutoSize = true;
            this.RS2014.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RS2014.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RS2014.Location = new System.Drawing.Point(88, 18);
            this.RS2014.Name = "RS2014";
            this.RS2014.Size = new System.Drawing.Size(102, 17);
            this.RS2014.TabIndex = 2;
            this.RS2014.Text = "Rocksmith 2014";
            this.RS2014.UseVisualStyleBackColor = true;
            this.RS2014.CheckedChanged += new System.EventHandler(this.GameVersion_CheckedChanged);
            // 
            // gbPlatofmr
            // 
            this.gbPlatofmr.Controls.Add(this.platformMAC);
            this.gbPlatofmr.Controls.Add(this.platformPS3);
            this.gbPlatofmr.Controls.Add(this.platformXBox360);
            this.gbPlatofmr.Controls.Add(this.platformPC);
            this.gbPlatofmr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPlatofmr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbPlatofmr.Location = new System.Drawing.Point(201, 0);
            this.gbPlatofmr.Name = "gbPlatofmr";
            this.gbPlatofmr.Size = new System.Drawing.Size(300, 41);
            this.gbPlatofmr.TabIndex = 75;
            this.gbPlatofmr.TabStop = false;
            this.gbPlatofmr.Text = "Platform:";
            // 
            // platformMAC
            // 
            this.platformMAC.AutoSize = true;
            this.platformMAC.Enabled = false;
            this.platformMAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformMAC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformMAC.Location = new System.Drawing.Point(74, 19);
            this.platformMAC.Name = "platformMAC";
            this.platformMAC.Size = new System.Drawing.Size(49, 17);
            this.platformMAC.TabIndex = 4;
            this.platformMAC.Text = "MAC";
            this.platformMAC.UseVisualStyleBackColor = true;
            this.platformMAC.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // openOggMacButton
            // 
            this.openOggMacButton.Enabled = false;
            this.openOggMacButton.Location = new System.Drawing.Point(210, 70);
            this.openOggMacButton.Name = "openOggMacButton";
            this.openOggMacButton.Size = new System.Drawing.Size(34, 23);
            this.openOggMacButton.TabIndex = 23;
            this.openOggMacButton.Text = "...";
            this.openOggMacButton.UseVisualStyleBackColor = true;
            this.openOggMacButton.Click += new System.EventHandler(this.openOggMacButton_Click);
            // 
            // gbFiles
            // 
            this.gbFiles.Controls.Add(this.label2);
            this.gbFiles.Controls.Add(this.AlbumArtPathTB);
            this.gbFiles.Controls.Add(this.openOggMacButton);
            this.gbFiles.Controls.Add(this.oggPcPathTB);
            this.gbFiles.Controls.Add(this.oggMacPathTB);
            this.gbFiles.Controls.Add(this.openOggPcButton);
            this.gbFiles.Controls.Add(this.albumArtButton);
            this.gbFiles.Controls.Add(this.oggXBox360PathTB);
            this.gbFiles.Controls.Add(this.openOggPS3Button);
            this.gbFiles.Controls.Add(this.openOggXBox360Button);
            this.gbFiles.Controls.Add(this.oggPS3PathTB);
            this.gbFiles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbFiles.Location = new System.Drawing.Point(3, 264);
            this.gbFiles.Name = "gbFiles";
            this.gbFiles.Size = new System.Drawing.Size(498, 111);
            this.gbFiles.TabIndex = 78;
            this.gbFiles.TabStop = false;
            this.gbFiles.Text = "Files";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Location = new System.Drawing.Point(5, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(487, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Song preview must have the same file name with \"_preview\" in the end, eg.: \"filen" +
    "ame_preview.wem\"";
            // 
            // AlbumArtPathTB
            // 
            this.AlbumArtPathTB.BackColor = System.Drawing.SystemColors.Window;
            this.AlbumArtPathTB.Cue = "Album Art (use 512x512 size only) (*.dds,*.gif,*.jpg,*.jpeg,*.bmp,*.png)";
            this.AlbumArtPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AlbumArtPathTB.ForeColor = System.Drawing.Color.Gray;
            this.AlbumArtPathTB.Location = new System.Drawing.Point(6, 19);
            this.AlbumArtPathTB.Name = "AlbumArtPathTB";
            this.AlbumArtPathTB.Size = new System.Drawing.Size(445, 20);
            this.AlbumArtPathTB.TabIndex = 13;
            // 
            // oggPcPathTB
            // 
            this.oggPcPathTB.Cue = "Audio - PC - Wwise (*.ogg; *.wem)";
            this.oggPcPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.oggPcPathTB.ForeColor = System.Drawing.Color.Gray;
            this.oggPcPathTB.Location = new System.Drawing.Point(6, 45);
            this.oggPcPathTB.Name = "oggPcPathTB";
            this.oggPcPathTB.Size = new System.Drawing.Size(198, 20);
            this.oggPcPathTB.TabIndex = 15;
            // 
            // oggMacPathTB
            // 
            this.oggMacPathTB.Cue = "Audio - MAC on Wwise (*.ogg; *.wem)";
            this.oggMacPathTB.Enabled = false;
            this.oggMacPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.oggMacPathTB.ForeColor = System.Drawing.Color.Gray;
            this.oggMacPathTB.Location = new System.Drawing.Point(6, 71);
            this.oggMacPathTB.Name = "oggMacPathTB";
            this.oggMacPathTB.Size = new System.Drawing.Size(198, 20);
            this.oggMacPathTB.TabIndex = 76;
            // 
            // oggXBox360PathTB
            // 
            this.oggXBox360PathTB.Cue = "Audio - XBox360 - Wwise (*.ogg; *.wem)";
            this.oggXBox360PathTB.Enabled = false;
            this.oggXBox360PathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.oggXBox360PathTB.ForeColor = System.Drawing.Color.Gray;
            this.oggXBox360PathTB.Location = new System.Drawing.Point(256, 45);
            this.oggXBox360PathTB.Name = "oggXBox360PathTB";
            this.oggXBox360PathTB.Size = new System.Drawing.Size(195, 20);
            this.oggXBox360PathTB.TabIndex = 17;
            // 
            // oggPS3PathTB
            // 
            this.oggPS3PathTB.Cue = "Audio - PS3 - Wwise (*.ogg; *.wem)";
            this.oggPS3PathTB.Enabled = false;
            this.oggPS3PathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.oggPS3PathTB.ForeColor = System.Drawing.Color.Gray;
            this.oggPS3PathTB.Location = new System.Drawing.Point(256, 71);
            this.oggPS3PathTB.Name = "oggPS3PathTB";
            this.oggPS3PathTB.Size = new System.Drawing.Size(195, 20);
            this.oggPS3PathTB.TabIndex = 70;
            // 
            // gbTones
            // 
            this.gbTones.Controls.Add(this.label1);
            this.gbTones.Controls.Add(this.TonesLB);
            this.gbTones.Controls.Add(this.toneAddButton);
            this.gbTones.Controls.Add(this.toneRemoveButton);
            this.gbTones.Controls.Add(this.toneEditButton);
            this.gbTones.Controls.Add(this.toneImportButton);
            this.gbTones.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbTones.Location = new System.Drawing.Point(3, 381);
            this.gbTones.Name = "gbTones";
            this.gbTones.Size = new System.Drawing.Size(498, 133);
            this.gbTones.TabIndex = 79;
            this.gbTones.TabStop = false;
            this.gbTones.Text = "Tones";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(4, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Use keyboard up/down keys to change order of the tones.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.keyboardDescArrLabel);
            this.groupBox1.Controls.Add(this.ArrangementLB);
            this.groupBox1.Controls.Add(this.arrangementAddButton);
            this.groupBox1.Controls.Add(this.arrangementRemoveButton);
            this.groupBox1.Controls.Add(this.arrangementEditButton);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(3, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 107);
            this.groupBox1.TabIndex = 80;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Arrangements";
            // 
            // keyboardDescArrLabel
            // 
            this.keyboardDescArrLabel.AutoSize = true;
            this.keyboardDescArrLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.keyboardDescArrLabel.Location = new System.Drawing.Point(4, 89);
            this.keyboardDescArrLabel.Name = "keyboardDescArrLabel";
            this.keyboardDescArrLabel.Size = new System.Drawing.Size(322, 13);
            this.keyboardDescArrLabel.TabIndex = 35;
            this.keyboardDescArrLabel.Text = "Use keyboard up/down keys to change order of the arrangements.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.DlcNameTB);
            this.groupBox2.Controls.Add(this.SongDisplayNameTB);
            this.groupBox2.Controls.Add(this.ArtistTB);
            this.groupBox2.Controls.Add(this.AlbumTB);
            this.groupBox2.Controls.Add(this.YearTB);
            this.groupBox2.Controls.Add(this.AverageTempoTB);
            this.groupBox2.Controls.Add(this.volumeBox);
            this.groupBox2.Controls.Add(this.AppIdTB);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cmbAppIds);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.SongDisplayNameSortTB);
            this.groupBox2.Controls.Add(this.ArtistSortTB);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(3, 47);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(498, 98);
            this.groupBox2.TabIndex = 81;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Song Information";
            // 
            // DlcNameTB
            // 
            this.DlcNameTB.Cue = "DLC Name";
            this.DlcNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DlcNameTB.ForeColor = System.Drawing.Color.Gray;
            this.DlcNameTB.Location = new System.Drawing.Point(6, 19);
            this.DlcNameTB.Name = "DlcNameTB";
            this.DlcNameTB.Size = new System.Drawing.Size(117, 20);
            this.DlcNameTB.TabIndex = 7;
            this.DlcNameTB.Leave += new System.EventHandler(this.DlcNameTB_Leave);
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.Cue = "Song Title";
            this.SongDisplayNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SongDisplayNameTB.ForeColor = System.Drawing.Color.Gray;
            this.SongDisplayNameTB.Location = new System.Drawing.Point(129, 19);
            this.SongDisplayNameTB.Name = "SongDisplayNameTB";
            this.SongDisplayNameTB.Size = new System.Drawing.Size(195, 20);
            this.SongDisplayNameTB.TabIndex = 8;
            // 
            // ArtistTB
            // 
            this.ArtistTB.Cue = "Artist";
            this.ArtistTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArtistTB.ForeColor = System.Drawing.Color.Gray;
            this.ArtistTB.Location = new System.Drawing.Point(129, 43);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(160, 20);
            this.ArtistTB.TabIndex = 11;
            // 
            // AlbumTB
            // 
            this.AlbumTB.Cue = "Album";
            this.AlbumTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AlbumTB.ForeColor = System.Drawing.Color.Gray;
            this.AlbumTB.Location = new System.Drawing.Point(6, 43);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(117, 20);
            this.AlbumTB.TabIndex = 10;
            // 
            // YearTB
            // 
            this.YearTB.Cue = "Year";
            this.YearTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.YearTB.ForeColor = System.Drawing.Color.Gray;
            this.YearTB.Location = new System.Drawing.Point(431, 43);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(60, 20);
            this.YearTB.TabIndex = 13;
            // 
            // AverageTempoTB
            // 
            this.AverageTempoTB.Cue = "Average Tempo";
            this.AverageTempoTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AverageTempoTB.ForeColor = System.Drawing.Color.Gray;
            this.AverageTempoTB.Location = new System.Drawing.Point(400, 69);
            this.AverageTempoTB.Name = "AverageTempoTB";
            this.AverageTempoTB.Size = new System.Drawing.Size(91, 20);
            this.AverageTempoTB.TabIndex = 17;
            // 
            // volumeBox
            // 
            this.volumeBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.volumeBox.Location = new System.Drawing.Point(52, 69);
            this.volumeBox.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.volumeBox.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.volumeBox.Name = "volumeBox";
            this.volumeBox.Size = new System.Drawing.Size(43, 20);
            this.volumeBox.TabIndex = 14;
            this.volumeBox.Value = new decimal(new int[] {
            15,
            0,
            0,
            -2147483648});
            // 
            // AppIdTB
            // 
            this.AppIdTB.Cue = "DLC App ID";
            this.AppIdTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AppIdTB.ForeColor = System.Drawing.Color.Gray;
            this.AppIdTB.Location = new System.Drawing.Point(129, 69);
            this.AppIdTB.Name = "AppIdTB";
            this.AppIdTB.Size = new System.Drawing.Size(63, 20);
            this.AppIdTB.TabIndex = 15;
            this.AppIdTB.TextChanged += new System.EventHandler(this.AppIdTB_TextChanged);
            // 
            // SongDisplayNameSortTB
            // 
            this.SongDisplayNameSortTB.Cue = "Song Title Sort";
            this.SongDisplayNameSortTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SongDisplayNameSortTB.ForeColor = System.Drawing.Color.Gray;
            this.SongDisplayNameSortTB.Location = new System.Drawing.Point(330, 19);
            this.SongDisplayNameSortTB.Name = "SongDisplayNameSortTB";
            this.SongDisplayNameSortTB.Size = new System.Drawing.Size(161, 20);
            this.SongDisplayNameSortTB.TabIndex = 9;
            // 
            // ArtistSortTB
            // 
            this.ArtistSortTB.Cue = "Artist Sort";
            this.ArtistSortTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArtistSortTB.ForeColor = System.Drawing.Color.Gray;
            this.ArtistSortTB.Location = new System.Drawing.Point(295, 43);
            this.ArtistSortTB.Name = "ArtistSortTB";
            this.ArtistSortTB.Size = new System.Drawing.Size(130, 20);
            this.ArtistSortTB.TabIndex = 12;
            // 
            // gbGameVersion
            // 
            this.gbGameVersion.Controls.Add(this.RS2014);
            this.gbGameVersion.Controls.Add(this.RS2012);
            this.gbGameVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbGameVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbGameVersion.Location = new System.Drawing.Point(3, 0);
            this.gbGameVersion.Name = "gbGameVersion";
            this.gbGameVersion.Size = new System.Drawing.Size(192, 41);
            this.gbGameVersion.TabIndex = 82;
            this.gbGameVersion.TabStop = false;
            this.gbGameVersion.Text = "Game Version";
            // 
            // DLCPackageCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Size = new System.Drawing.Size(507, 552);
            this.gbPlatofmr.ResumeLayout(false);
            this.gbPlatofmr.PerformLayout();
            this.gbFiles.ResumeLayout(false);
            this.gbFiles.PerformLayout();
            this.gbTones.ResumeLayout(false);
            this.gbTones.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBox)).EndInit();
            this.gbGameVersion.ResumeLayout(false);
            this.gbGameVersion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button albumArtButton;
        private RocksmithToolkitGUI.CueTextBox AlbumArtPathTB;
        private System.Windows.Forms.Button dlcGenerateButton;
        private System.Windows.Forms.Button openOggPcButton;
        private RocksmithToolkitGUI.CueTextBox oggPcPathTB;
        private System.Windows.Forms.Button arrangementRemoveButton;
        private System.Windows.Forms.Button arrangementAddButton;
        private System.Windows.Forms.ListBox ArrangementLB;
        private RocksmithToolkitGUI.CueTextBox YearTB;
        private RocksmithToolkitGUI.CueTextBox AlbumTB;
        private RocksmithToolkitGUI.CueTextBox ArtistTB;
        private RocksmithToolkitGUI.CueTextBox SongDisplayNameTB;
        private RocksmithToolkitGUI.CueTextBox DlcNameTB;
        private System.Windows.Forms.Button dlcSaveButton;
        private System.Windows.Forms.Button dlcLoadButton;
        private CueTextBox AverageTempoTB;
        private CueTextBox AppIdTB;
        private System.Windows.Forms.ComboBox cmbAppIds;
        private System.Windows.Forms.Button toneRemoveButton;
        private System.Windows.Forms.Button toneAddButton;
        private System.Windows.Forms.ListBox TonesLB;
        private System.Windows.Forms.Button arrangementEditButton;
        private System.Windows.Forms.Button toneEditButton;
        private System.Windows.Forms.Button toneImportButton;
        private CueTextBox oggXBox360PathTB;
        private System.Windows.Forms.Button openOggXBox360Button;
        private System.Windows.Forms.CheckBox platformPC;
        private System.Windows.Forms.CheckBox platformXBox360;
        private System.Windows.Forms.CheckBox platformPS3;
        private CueTextBox SongDisplayNameSortTB;
        private CueTextBox ArtistSortTB;
        private NumericUpDownFixed volumeBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button openOggPS3Button;
        private CueTextBox oggPS3PathTB;
        private System.Windows.Forms.RadioButton RS2012;
        private System.Windows.Forms.RadioButton RS2014;
        private System.Windows.Forms.GroupBox gbPlatofmr;
        private System.Windows.Forms.CheckBox platformMAC;
        private System.Windows.Forms.Button openOggMacButton;
        private CueTextBox oggMacPathTB;
        private System.Windows.Forms.GroupBox gbFiles;
        private System.Windows.Forms.GroupBox gbTones;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label keyboardDescArrLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gbGameVersion;
        private System.Windows.Forms.Label label2;
    }
}
