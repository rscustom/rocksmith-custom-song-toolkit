using System.Drawing;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.DLCInlayCreator
{
    partial class DLCInlayCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DLCInlayCreator));
            this.appIdCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkFlipX = new System.Windows.Forms.CheckBox();
            this.chkFlipY = new System.Windows.Forms.CheckBox();
            this.ColoredCheckbox = new System.Windows.Forms.CheckBox();
            this.Frets24Checkbox = new System.Windows.Forms.CheckBox();
            this.gbInfo = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.authorTextbox = new RocksmithToolkitGUI.CueTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.inlayNameTextbox = new RocksmithToolkitGUI.CueTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.inlayTemplateCombo = new System.Windows.Forms.ComboBox();
            this.picColored = new System.Windows.Forms.PictureBox();
            this.picInlay = new System.Windows.Forms.PictureBox();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.picFlipY = new System.Windows.Forms.PictureBox();
            this.picFlipX = new System.Windows.Forms.PictureBox();
            this.updateProgress = new System.Windows.Forms.ProgressBar();
            this.currentOperationLabel = new System.Windows.Forms.Label();
            this.gbPlatform = new System.Windows.Forms.GroupBox();
            this.platformMAC = new System.Windows.Forms.CheckBox();
            this.platformPS3 = new System.Windows.Forms.CheckBox();
            this.platformXBox360 = new System.Windows.Forms.CheckBox();
            this.platformPC = new System.Windows.Forms.CheckBox();
            this.saveCGMButton = new System.Windows.Forms.Button();
            this.inlayGenerateButton = new System.Windows.Forms.Button();
            this.loadCGMButton = new System.Windows.Forms.Button();
            this.gbInlayType = new System.Windows.Forms.GroupBox();
            this.inlayTypeCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.helpLink = new System.Windows.Forms.LinkLabel();
            this.expansionMod1 = new RocksmithToolkitGUI.DLCInlayCreator.ExpansionMod();
            this.label4 = new System.Windows.Forms.Label();
            this.gbInfo.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picColored)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipX)).BeginInit();
            this.gbPlatform.SuspendLayout();
            this.gbInlayType.SuspendLayout();
            this.SuspendLayout();
            // 
            // appIdCombo
            // 
            this.appIdCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appIdCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appIdCombo.FormattingEnabled = true;
            this.appIdCombo.Location = new System.Drawing.Point(195, 30);
            this.appIdCombo.Margin = new System.Windows.Forms.Padding(2);
            this.appIdCombo.Name = "appIdCombo";
            this.appIdCombo.Size = new System.Drawing.Size(280, 21);
            this.appIdCombo.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(195, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 71;
            this.label5.Text = "App ID:";
            // 
            // chkFlipX
            // 
            this.chkFlipX.AutoSize = true;
            this.chkFlipX.Location = new System.Drawing.Point(361, 20);
            this.chkFlipX.Name = "chkFlipX";
            this.chkFlipX.Size = new System.Drawing.Size(15, 14);
            this.chkFlipX.TabIndex = 10;
            this.chkFlipX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkFlipX.UseVisualStyleBackColor = true;
            this.chkFlipX.CheckedChanged += new System.EventHandler(this.FlipX_Changed);
            // 
            // chkFlipY
            // 
            this.chkFlipY.AutoSize = true;
            this.chkFlipY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chkFlipY.Location = new System.Drawing.Point(426, 20);
            this.chkFlipY.Name = "chkFlipY";
            this.chkFlipY.Size = new System.Drawing.Size(15, 14);
            this.chkFlipY.TabIndex = 11;
            this.chkFlipY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkFlipY.UseVisualStyleBackColor = true;
            this.chkFlipY.CheckedChanged += new System.EventHandler(this.FlipY_Changed);
            // 
            // ColoredCheckbox
            // 
            this.ColoredCheckbox.AutoSize = true;
            this.ColoredCheckbox.Location = new System.Drawing.Point(296, 20);
            this.ColoredCheckbox.Name = "ColoredCheckbox";
            this.ColoredCheckbox.Size = new System.Drawing.Size(15, 14);
            this.ColoredCheckbox.TabIndex = 9;
            this.ColoredCheckbox.UseVisualStyleBackColor = true;
            // 
            // Frets24Checkbox
            // 
            this.Frets24Checkbox.AutoSize = true;
            this.Frets24Checkbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frets24Checkbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frets24Checkbox.Location = new System.Drawing.Point(218, 19);
            this.Frets24Checkbox.Name = "Frets24Checkbox";
            this.Frets24Checkbox.Size = new System.Drawing.Size(61, 17);
            this.Frets24Checkbox.TabIndex = 8;
            this.Frets24Checkbox.Text = "24 frets";
            this.Frets24Checkbox.UseVisualStyleBackColor = true;
            // 
            // gbInfo
            // 
            this.gbInfo.Controls.Add(this.appIdCombo);
            this.gbInfo.Controls.Add(this.label5);
            this.gbInfo.Controls.Add(this.authorTextbox);
            this.gbInfo.Controls.Add(this.label1);
            this.gbInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbInfo.Location = new System.Drawing.Point(11, 47);
            this.gbInfo.Name = "gbInfo";
            this.gbInfo.Size = new System.Drawing.Size(494, 58);
            this.gbInfo.TabIndex = 72;
            this.gbInfo.TabStop = false;
            this.gbInfo.Text = "Info: (will be written inside the package)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "Author:";
            // 
            // authorTextbox
            // 
            this.authorTextbox.Cue = "Author";
            this.authorTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.authorTextbox.ForeColor = System.Drawing.Color.Gray;
            this.authorTextbox.Location = new System.Drawing.Point(11, 31);
            this.authorTextbox.Name = "authorTextbox";
            this.authorTextbox.Size = new System.Drawing.Size(150, 20);
            this.authorTextbox.TabIndex = 72;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.chkFlipX);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.chkFlipY);
            this.groupBox2.Controls.Add(this.inlayTemplateCombo);
            this.groupBox2.Controls.Add(this.picInlay);
            this.groupBox2.Controls.Add(this.ColoredCheckbox);
            this.groupBox2.Controls.Add(this.picIcon);
            this.groupBox2.Controls.Add(this.inlayNameTextbox);
            this.groupBox2.Controls.Add(this.Frets24Checkbox);
            this.groupBox2.Controls.Add(this.picFlipX);
            this.groupBox2.Controls.Add(this.picFlipY);
            this.groupBox2.Controls.Add(this.picColored);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(11, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(494, 188);
            this.groupBox2.TabIndex = 83;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Custom Guitar Inlays";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(15, 121);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 41);
            this.label9.TabIndex = 88;
            this.label9.Text = "Icon and Inlay Artwork\r\n";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
             // 
            // inlayNameTextbox
            // 
            this.inlayNameTextbox.Cue = "Inlay Name";
            this.inlayNameTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.inlayNameTextbox.ForeColor = System.Drawing.Color.Gray;
            this.inlayNameTextbox.Location = new System.Drawing.Point(11, 73);
            this.inlayNameTextbox.Name = "inlayNameTextbox";
            this.inlayNameTextbox.Size = new System.Drawing.Size(150, 20);
            this.inlayNameTextbox.TabIndex = 6;
            this.inlayNameTextbox.Leave += new System.EventHandler(this.inlayNameTextbox_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(8, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 82;
            this.label2.Text = "Local Templates:";
            // 
            // inlayTemplateCombo
            // 
            this.inlayTemplateCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.inlayTemplateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inlayTemplateCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inlayTemplateCombo.FormattingEnabled = true;
            this.inlayTemplateCombo.Location = new System.Drawing.Point(11, 32);
            this.inlayTemplateCombo.Margin = new System.Windows.Forms.Padding(2);
            this.inlayTemplateCombo.Name = "inlayTemplateCombo";
            this.inlayTemplateCombo.Size = new System.Drawing.Size(166, 21);
            this.inlayTemplateCombo.TabIndex = 2;
            this.inlayTemplateCombo.SelectedIndexChanged += new System.EventHandler(this.inlayTemplateCombo_SelectedIndexChanged);
            // 
            // picColored
            // 
            this.picColored.Image = global::RocksmithToolkitGUI.Properties.Resources.colored;
            this.picColored.Location = new System.Drawing.Point(317, 19);
            this.picColored.Name = "picColored";
            this.picColored.Size = new System.Drawing.Size(16, 16);
            this.picColored.TabIndex = 81;
            this.picColored.TabStop = false;
            // 
            // picInlay
            // 
            this.picInlay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picInlay.Location = new System.Drawing.Point(198, 48);
            this.picInlay.Name = "picInlay";
            this.picInlay.Size = new System.Drawing.Size(284, 128);
            this.picInlay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInlay.TabIndex = 66;
            this.picInlay.TabStop = false;
            this.picInlay.Click += new System.EventHandler(this.picInlay_Click);
            // 
            // picIcon
            // 
            this.picIcon.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picIcon.Location = new System.Drawing.Point(109, 104);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(72, 72);
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picIcon.TabIndex = 67;
            this.picIcon.TabStop = false;
            this.picIcon.Click += new System.EventHandler(this.picIcon_Click);
            // 
            // picFlipY
            // 
            this.picFlipY.Image = ((System.Drawing.Image)(resources.GetObject("picFlipY.Image")));
            this.picFlipY.Location = new System.Drawing.Point(447, 19);
            this.picFlipY.Name = "picFlipY";
            this.picFlipY.Size = new System.Drawing.Size(16, 16);
            this.picFlipY.TabIndex = 80;
            this.picFlipY.TabStop = false;
            // 
            // picFlipX
            // 
            this.picFlipX.Image = global::RocksmithToolkitGUI.Properties.Resources.flipXc;
            this.picFlipX.Location = new System.Drawing.Point(382, 19);
            this.picFlipX.Name = "picFlipX";
            this.picFlipX.Size = new System.Drawing.Size(16, 16);
            this.picFlipX.TabIndex = 78;
            this.picFlipX.TabStop = false;
            // 
            // updateProgress
            // 
            this.updateProgress.Location = new System.Drawing.Point(96, 466);
            this.updateProgress.Name = "updateProgress";
            this.updateProgress.Size = new System.Drawing.Size(328, 25);
            this.updateProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.updateProgress.TabIndex = 84;
            this.updateProgress.Visible = false;
            // 
            // currentOperationLabel
            // 
            this.currentOperationLabel.AutoSize = true;
            this.currentOperationLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.currentOperationLabel.Location = new System.Drawing.Point(93, 494);
            this.currentOperationLabel.Name = "currentOperationLabel";
            this.currentOperationLabel.Size = new System.Drawing.Size(16, 13);
            this.currentOperationLabel.TabIndex = 85;
            this.currentOperationLabel.Text = "...";
            this.currentOperationLabel.Visible = false;
            // 
            // gbPlatform
            // 
            this.gbPlatform.Controls.Add(this.platformXBox360);
            this.gbPlatform.Controls.Add(this.platformPC);
            this.gbPlatform.Controls.Add(this.platformMAC);
            this.gbPlatform.Controls.Add(this.platformPS3);
            this.gbPlatform.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPlatform.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbPlatform.Location = new System.Drawing.Point(209, 0);
            this.gbPlatform.Name = "gbPlatform";
            this.gbPlatform.Size = new System.Drawing.Size(296, 41);
            this.gbPlatform.TabIndex = 87;
            this.gbPlatform.TabStop = false;
            this.gbPlatform.Text = "Platform:";
            // 
            // platformMAC
            // 
            this.platformMAC.AutoSize = true;
            this.platformMAC.Checked = true;
            this.platformMAC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformMAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformMAC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformMAC.Location = new System.Drawing.Point(75, 17);
            this.platformMAC.Name = "platformMAC";
            this.platformMAC.Size = new System.Drawing.Size(49, 17);
            this.platformMAC.TabIndex = 3;
            this.platformMAC.Text = "MAC";
            this.platformMAC.UseVisualStyleBackColor = true;
            this.platformMAC.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformPS3
            // 
            this.platformPS3.AutoSize = true;
            this.platformPS3.Checked = true;
            this.platformPS3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformPS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformPS3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformPS3.Location = new System.Drawing.Point(235, 18);
            this.platformPS3.Name = "platformPS3";
            this.platformPS3.Size = new System.Drawing.Size(46, 17);
            this.platformPS3.TabIndex = 5;
            this.platformPS3.Text = "PS3";
            this.platformPS3.UseVisualStyleBackColor = true;
            this.platformPS3.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformXBox360
            // 
            this.platformXBox360.AutoSize = true;
            this.platformXBox360.Checked = true;
            this.platformXBox360.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformXBox360.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformXBox360.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformXBox360.Location = new System.Drawing.Point(145, 17);
            this.platformXBox360.Name = "platformXBox360";
            this.platformXBox360.Size = new System.Drawing.Size(69, 17);
            this.platformXBox360.TabIndex = 4;
            this.platformXBox360.Text = "XBox360";
            this.platformXBox360.UseVisualStyleBackColor = true;
            this.platformXBox360.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformPC
            // 
            this.platformPC.AutoSize = true;
            this.platformPC.Checked = true;
            this.platformPC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformPC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformPC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformPC.Location = new System.Drawing.Point(14, 17);
            this.platformPC.Name = "platformPC";
            this.platformPC.Size = new System.Drawing.Size(40, 17);
            this.platformPC.TabIndex = 2;
            this.platformPC.Text = "PC";
            this.platformPC.UseVisualStyleBackColor = true;
            this.platformPC.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // saveCGMButton
            // 
            this.saveCGMButton.BackColor = System.Drawing.SystemColors.Control;
            this.saveCGMButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.saveCGMButton.Location = new System.Drawing.Point(140, 339);
            this.saveCGMButton.Name = "saveCGMButton";
            this.saveCGMButton.Size = new System.Drawing.Size(97, 29);
            this.saveCGMButton.TabIndex = 12;
            this.saveCGMButton.Text = "Save Template";
            this.saveCGMButton.UseVisualStyleBackColor = false;
            this.saveCGMButton.Click += new System.EventHandler(this.saveCGMButton_Click);
            // 
            // inlayGenerateButton
            // 
            this.inlayGenerateButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.inlayGenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inlayGenerateButton.Location = new System.Drawing.Point(317, 339);
            this.inlayGenerateButton.Name = "inlayGenerateButton";
            this.inlayGenerateButton.Size = new System.Drawing.Size(176, 29);
            this.inlayGenerateButton.TabIndex = 13;
            this.inlayGenerateButton.Text = "Generate Package";
            this.inlayGenerateButton.UseVisualStyleBackColor = false;
            this.inlayGenerateButton.Click += new System.EventHandler(this.inlayGenerateButton_Click);
            // 
            // loadCGMButton
            // 
            this.loadCGMButton.BackColor = System.Drawing.SystemColors.Control;
            this.loadCGMButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.loadCGMButton.Location = new System.Drawing.Point(22, 339);
            this.loadCGMButton.Name = "loadCGMButton";
            this.loadCGMButton.Size = new System.Drawing.Size(97, 29);
            this.loadCGMButton.TabIndex = 0;
            this.loadCGMButton.Text = "Load Template";
            this.loadCGMButton.UseVisualStyleBackColor = false;
            this.loadCGMButton.Click += new System.EventHandler(this.loadCGMButton_Click);
            // 
            // gbInlayType
            // 
            this.gbInlayType.Controls.Add(this.inlayTypeCombo);
            this.gbInlayType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbInlayType.Location = new System.Drawing.Point(11, 0);
            this.gbInlayType.Name = "gbInlayType";
            this.gbInlayType.Size = new System.Drawing.Size(192, 41);
            this.gbInlayType.TabIndex = 73;
            this.gbInlayType.TabStop = false;
            this.gbInlayType.Text = "Mod Type:";
            // 
            // inlayTypeCombo
            // 
            this.inlayTypeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.inlayTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inlayTypeCombo.FormattingEnabled = true;
            this.inlayTypeCombo.Location = new System.Drawing.Point(11, 13);
            this.inlayTypeCombo.Margin = new System.Windows.Forms.Padding(2);
            this.inlayTypeCombo.Name = "inlayTypeCombo";
            this.inlayTypeCombo.Size = new System.Drawing.Size(170, 21);
            this.inlayTypeCombo.TabIndex = 1;
            this.inlayTypeCombo.SelectedIndexChanged += new System.EventHandler(this.inlayTypeCombo_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 13);
            this.label3.TabIndex = 89;
            this.label3.Text = "Help and additional content can be found at:";
            // 
            // helpLink
            // 
            this.helpLink.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.helpLink.ActiveLinkColor = System.Drawing.Color.RosyBrown;
            this.helpLink.AutoEllipsis = true;
            this.helpLink.AutoSize = true;
            this.helpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helpLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.helpLink.Location = new System.Drawing.Point(243, 113);
            this.helpLink.Name = "helpLink";
            this.helpLink.Size = new System.Drawing.Size(116, 15);
            this.helpLink.TabIndex = 88;
            this.helpLink.TabStop = true;
            this.helpLink.Text = "http://goo.gl/pJxMuz";
            this.helpLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.helpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DescriptionDDC_LinkClicked);
            // 
            // expansionMod1
            // 
            this.expansionMod1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.expansionMod1.Location = new System.Drawing.Point(192, 394);
            this.expansionMod1.Name = "expansionMod1";
            this.expansionMod1.Size = new System.Drawing.Size(137, 41);
            this.expansionMod1.TabIndex = 90;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(8, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 89;
            this.label4.Text = "Inlay Name:";
            // 
            // DLCInlayCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gbInfo);
            this.Controls.Add(this.updateProgress);
            this.Controls.Add(this.currentOperationLabel);
            this.Controls.Add(this.saveCGMButton);
            this.Controls.Add(this.inlayGenerateButton);
            this.Controls.Add(this.loadCGMButton);
            this.Controls.Add(this.helpLink);
            this.Controls.Add(this.expansionMod1);
            this.Controls.Add(this.gbPlatform);
            this.Controls.Add(this.gbInlayType);
            this.Controls.Add(this.groupBox2);
            this.Name = "DLCInlayCreator";
            this.Size = new System.Drawing.Size(520, 520);
            this.Load += new System.EventHandler(this.DLCInlayCreator_Load);
            this.Disposed += new System.EventHandler(this.DLCInlayCreator_Dispose);
            this.gbInfo.ResumeLayout(false);
            this.gbInfo.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picColored)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipX)).EndInit();
            this.gbPlatform.ResumeLayout(false);
            this.gbPlatform.PerformLayout();
            this.gbInlayType.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.ComboBox appIdCombo;
        private System.Windows.Forms.PictureBox picInlay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkFlipX;
        private System.Windows.Forms.PictureBox picFlipX;
        private System.Windows.Forms.PictureBox picFlipY;
        private System.Windows.Forms.CheckBox chkFlipY;
        private System.Windows.Forms.CheckBox ColoredCheckbox;
        private System.Windows.Forms.CheckBox Frets24Checkbox;
        private GroupBox gbInfo;
        private GroupBox groupBox2;
        private Label currentOperationLabel;
        private GroupBox gbPlatform;
        private CheckBox platformMAC;
        private CheckBox platformPS3;
        private CheckBox platformXBox360;
        private CheckBox platformPC;
        private Button saveCGMButton;
        private Button inlayGenerateButton;
        private Button loadCGMButton;
        private CueTextBox inlayNameTextbox;
        private GroupBox gbInlayType;
        private ComboBox inlayTypeCombo;
        private PictureBox picColored;
        private ComboBox inlayTemplateCombo;
        private Label label2;
        private Label label1;
        private CueTextBox authorTextbox;
        private Label label3;
        private LinkLabel helpLink;
        private Label label9;
        private ExpansionMod expansionMod1;
        private ProgressBar updateProgress;
        private Label label4;

    }
}
