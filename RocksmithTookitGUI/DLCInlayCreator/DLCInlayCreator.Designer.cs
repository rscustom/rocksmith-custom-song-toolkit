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
            this.components = new System.ComponentModel.Container();
            this.appIdCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkFlipX = new System.Windows.Forms.CheckBox();
            this.chkFlipY = new System.Windows.Forms.CheckBox();
            this.ColoredCheckbox = new System.Windows.Forms.CheckBox();
            this.Frets24Checkbox = new System.Windows.Forms.CheckBox();
            this.gbInfo = new System.Windows.Forms.GroupBox();
            this.keyboardDescArrLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inlayTemplateCombo = new System.Windows.Forms.ComboBox();
            this.picInlay = new System.Windows.Forms.PictureBox();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.picFlipX = new System.Windows.Forms.PictureBox();
            this.picFlipY = new System.Windows.Forms.PictureBox();
            this.picColored = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.helpLink = new System.Windows.Forms.LinkLabel();
            this.updateProgress = new System.Windows.Forms.ProgressBar();
            this.currentOperationLabel = new System.Windows.Forms.Label();
            this.gbPlatform = new System.Windows.Forms.GroupBox();
            this.platformXBox360 = new System.Windows.Forms.CheckBox();
            this.platformPC = new System.Windows.Forms.CheckBox();
            this.platformMAC = new System.Windows.Forms.CheckBox();
            this.platformPS3 = new System.Windows.Forms.CheckBox();
            this.saveCGMButton = new System.Windows.Forms.Button();
            this.inlayGenerateButton = new System.Windows.Forms.Button();
            this.loadCGMButton = new System.Windows.Forms.Button();
            this.gbInlayType = new System.Windows.Forms.GroupBox();
            this.inlayTypeCombo = new System.Windows.Forms.ComboBox();
            this.gbRbPlatform = new System.Windows.Forms.GroupBox();
            this.rbPs3 = new System.Windows.Forms.RadioButton();
            this.rbXbox360 = new System.Windows.Forms.RadioButton();
            this.rbMac = new System.Windows.Forms.RadioButton();
            this.rbPc = new System.Windows.Forms.RadioButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.introScreensCreator1 = new RocksmithToolkitGUI.DLCInlayCreator.IntroScreensCreator();
            this.authorTextbox = new RocksmithToolkitGUI.CueTextBox();
            this.inlayNameTextbox = new RocksmithToolkitGUI.CueTextBox();
            this.gbInfo.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picInlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picColored)).BeginInit();
            this.gbPlatform.SuspendLayout();
            this.gbInlayType.SuspendLayout();
            this.gbRbPlatform.SuspendLayout();
            this.SuspendLayout();
            // 
            // appIdCombo
            // 
            this.appIdCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appIdCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appIdCombo.DropDownWidth = 320;
            this.appIdCombo.FormattingEnabled = true;
            this.appIdCombo.Location = new System.Drawing.Point(204, 30);
            this.appIdCombo.Margin = new System.Windows.Forms.Padding(2);
            this.appIdCombo.Name = "appIdCombo";
            this.appIdCombo.Size = new System.Drawing.Size(288, 21);
            this.appIdCombo.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(201, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 71;
            this.label5.Text = "App ID:";
            // 
            // chkFlipX
            // 
            this.chkFlipX.AutoSize = true;
            this.chkFlipX.Location = new System.Drawing.Point(357, 16);
            this.chkFlipX.Name = "chkFlipX";
            this.chkFlipX.Size = new System.Drawing.Size(15, 14);
            this.chkFlipX.TabIndex = 12;
            this.chkFlipX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkFlipX.UseVisualStyleBackColor = true;
            this.chkFlipX.CheckedChanged += new System.EventHandler(this.FlipX_Changed);
            // 
            // chkFlipY
            // 
            this.chkFlipY.AutoSize = true;
            this.chkFlipY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chkFlipY.Location = new System.Drawing.Point(422, 16);
            this.chkFlipY.Name = "chkFlipY";
            this.chkFlipY.Size = new System.Drawing.Size(15, 14);
            this.chkFlipY.TabIndex = 13;
            this.chkFlipY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkFlipY.UseVisualStyleBackColor = true;
            this.chkFlipY.CheckedChanged += new System.EventHandler(this.FlipY_Changed);
            // 
            // ColoredCheckbox
            // 
            this.ColoredCheckbox.AutoSize = true;
            this.ColoredCheckbox.Location = new System.Drawing.Point(292, 16);
            this.ColoredCheckbox.Name = "ColoredCheckbox";
            this.ColoredCheckbox.Size = new System.Drawing.Size(15, 14);
            this.ColoredCheckbox.TabIndex = 11;
            this.ColoredCheckbox.UseVisualStyleBackColor = true;
            // 
            // Frets24Checkbox
            // 
            this.Frets24Checkbox.AutoSize = true;
            this.Frets24Checkbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frets24Checkbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frets24Checkbox.Location = new System.Drawing.Point(214, 15);
            this.Frets24Checkbox.Name = "Frets24Checkbox";
            this.Frets24Checkbox.Size = new System.Drawing.Size(61, 17);
            this.Frets24Checkbox.TabIndex = 10;
            this.Frets24Checkbox.Text = "24 frets";
            this.Frets24Checkbox.UseVisualStyleBackColor = true;
            // 
            // gbInfo
            // 
            this.gbInfo.Controls.Add(this.keyboardDescArrLabel);
            this.gbInfo.Controls.Add(this.appIdCombo);
            this.gbInfo.Controls.Add(this.label5);
            this.gbInfo.Controls.Add(this.authorTextbox);
            this.gbInfo.Controls.Add(this.label1);
            this.gbInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbInfo.Location = new System.Drawing.Point(3, 47);
            this.gbInfo.Name = "gbInfo";
            this.gbInfo.Size = new System.Drawing.Size(498, 70);
            this.gbInfo.TabIndex = 72;
            this.gbInfo.TabStop = false;
            this.gbInfo.Text = "Package info:";
            // 
            // keyboardDescArrLabel
            // 
            this.keyboardDescArrLabel.AutoSize = true;
            this.keyboardDescArrLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.keyboardDescArrLabel.Location = new System.Drawing.Point(8, 52);
            this.keyboardDescArrLabel.Name = "keyboardDescArrLabel";
            this.keyboardDescArrLabel.Size = new System.Drawing.Size(166, 13);
            this.keyboardDescArrLabel.TabIndex = 74;
            this.keyboardDescArrLabel.Text = "Will be written inside the package";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "Author:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
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
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(3, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(498, 184);
            this.groupBox2.TabIndex = 83;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Guitar Inlay:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Location = new System.Drawing.Point(202, 162);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 15);
            this.label7.TabIndex = 90;
            this.label7.Text = "Inlay";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Location = new System.Drawing.Point(123, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 15);
            this.label6.TabIndex = 75;
            this.label6.Text = "Icon";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(7, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 89;
            this.label4.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(5, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 82;
            this.label2.Text = "Saved Templates:";
            // 
            // inlayTemplateCombo
            // 
            this.inlayTemplateCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.inlayTemplateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inlayTemplateCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inlayTemplateCombo.FormattingEnabled = true;
            this.inlayTemplateCombo.Location = new System.Drawing.Point(8, 34);
            this.inlayTemplateCombo.Margin = new System.Windows.Forms.Padding(2);
            this.inlayTemplateCombo.Name = "inlayTemplateCombo";
            this.inlayTemplateCombo.Size = new System.Drawing.Size(190, 21);
            this.inlayTemplateCombo.TabIndex = 8;
            this.inlayTemplateCombo.SelectedIndexChanged += new System.EventHandler(this.inlayTemplateCombo_SelectedIndexChanged);
            // 
            // picInlay
            // 
            this.picInlay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picInlay.Location = new System.Drawing.Point(204, 34);
            this.picInlay.Name = "picInlay";
            this.picInlay.Size = new System.Drawing.Size(284, 128);
            this.picInlay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInlay.TabIndex = 66;
            this.picInlay.TabStop = false;
            this.picInlay.Click += new System.EventHandler(this.picInlay_Click);
            // 
            // picIcon
            // 
            this.picIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picIcon.Location = new System.Drawing.Point(126, 90);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(72, 72);
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picIcon.TabIndex = 67;
            this.picIcon.TabStop = false;
            this.picIcon.Click += new System.EventHandler(this.picIcon_Click);
            // 
            // picFlipX
            // 
            this.picFlipX.Image = global::RocksmithToolkitGUI.Properties.Resources.flipXc;
            this.picFlipX.Location = new System.Drawing.Point(378, 15);
            this.picFlipX.Name = "picFlipX";
            this.picFlipX.Size = new System.Drawing.Size(16, 16);
            this.picFlipX.TabIndex = 78;
            this.picFlipX.TabStop = false;
            // 
            // picFlipY
            // 
            this.picFlipY.Image = global::RocksmithToolkitGUI.Properties.Resources.flipYc;
            this.picFlipY.Location = new System.Drawing.Point(443, 15);
            this.picFlipY.Name = "picFlipY";
            this.picFlipY.Size = new System.Drawing.Size(16, 16);
            this.picFlipY.TabIndex = 80;
            this.picFlipY.TabStop = false;
            // 
            // picColored
            // 
            this.picColored.Image = global::RocksmithToolkitGUI.Properties.Resources.colored;
            this.picColored.Location = new System.Drawing.Point(313, 15);
            this.picColored.Name = "picColored";
            this.picColored.Size = new System.Drawing.Size(16, 16);
            this.picColored.TabIndex = 81;
            this.picColored.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(272, 368);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 89;
            this.label3.Text = "Help can be found at:";
            // 
            // helpLink
            // 
            this.helpLink.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.helpLink.ActiveLinkColor = System.Drawing.Color.RosyBrown;
            this.helpLink.AutoEllipsis = true;
            this.helpLink.AutoSize = true;
            this.helpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helpLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.helpLink.Location = new System.Drawing.Point(379, 366);
            this.helpLink.Name = "helpLink";
            this.helpLink.Size = new System.Drawing.Size(116, 15);
            this.helpLink.TabIndex = 88;
            this.helpLink.TabStop = true;
            this.helpLink.Text = "http://goo.gl/pJxMuz";
            this.helpLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.helpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpLink_LinkClicked);
            // 
            // updateProgress
            // 
            this.updateProgress.Location = new System.Drawing.Point(3, 347);
            this.updateProgress.Name = "updateProgress";
            this.updateProgress.Size = new System.Drawing.Size(498, 18);
            this.updateProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.updateProgress.TabIndex = 84;
            this.updateProgress.Visible = false;
            // 
            // currentOperationLabel
            // 
            this.currentOperationLabel.AutoSize = true;
            this.currentOperationLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.currentOperationLabel.Location = new System.Drawing.Point(3, 368);
            this.currentOperationLabel.Name = "currentOperationLabel";
            this.currentOperationLabel.Size = new System.Drawing.Size(71, 13);
            this.currentOperationLabel.TabIndex = 85;
            this.currentOperationLabel.Text = "Generating ...";
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
            this.gbPlatform.Location = new System.Drawing.Point(201, 0);
            this.gbPlatform.Name = "gbPlatform";
            this.gbPlatform.Size = new System.Drawing.Size(300, 41);
            this.gbPlatform.TabIndex = 87;
            this.gbPlatform.TabStop = false;
            this.gbPlatform.Text = "Platform:";
            // 
            // platformXBox360
            // 
            this.platformXBox360.AutoSize = true;
            this.platformXBox360.Checked = true;
            this.platformXBox360.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformXBox360.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformXBox360.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformXBox360.Location = new System.Drawing.Point(150, 19);
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
            this.platformPC.Location = new System.Drawing.Point(9, 19);
            this.platformPC.Name = "platformPC";
            this.platformPC.Size = new System.Drawing.Size(40, 17);
            this.platformPC.TabIndex = 2;
            this.platformPC.Text = "PC";
            this.platformPC.UseVisualStyleBackColor = true;
            this.platformPC.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // platformMAC
            // 
            this.platformMAC.AutoSize = true;
            this.platformMAC.Checked = true;
            this.platformMAC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.platformMAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.platformMAC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformMAC.Location = new System.Drawing.Point(74, 19);
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
            this.platformPS3.Location = new System.Drawing.Point(248, 19);
            this.platformPS3.Name = "platformPS3";
            this.platformPS3.Size = new System.Drawing.Size(46, 17);
            this.platformPS3.TabIndex = 5;
            this.platformPS3.Text = "PS3";
            this.platformPS3.UseVisualStyleBackColor = true;
            this.platformPS3.CheckedChanged += new System.EventHandler(this.plataform_CheckedChanged);
            // 
            // saveCGMButton
            // 
            this.saveCGMButton.BackColor = System.Drawing.SystemColors.Control;
            this.saveCGMButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.saveCGMButton.Location = new System.Drawing.Point(112, 313);
            this.saveCGMButton.Name = "saveCGMButton";
            this.saveCGMButton.Size = new System.Drawing.Size(97, 29);
            this.saveCGMButton.TabIndex = 14;
            this.saveCGMButton.Text = "Save Template";
            this.saveCGMButton.UseVisualStyleBackColor = false;
            this.saveCGMButton.Click += new System.EventHandler(this.saveCGMButton_Click);
            // 
            // inlayGenerateButton
            // 
            this.inlayGenerateButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.inlayGenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inlayGenerateButton.Location = new System.Drawing.Point(326, 313);
            this.inlayGenerateButton.Name = "inlayGenerateButton";
            this.inlayGenerateButton.Size = new System.Drawing.Size(176, 29);
            this.inlayGenerateButton.TabIndex = 15;
            this.inlayGenerateButton.Text = "Generate Package";
            this.inlayGenerateButton.UseVisualStyleBackColor = false;
            this.inlayGenerateButton.Click += new System.EventHandler(this.inlayGenerateButton_Click);
            // 
            // loadCGMButton
            // 
            this.loadCGMButton.BackColor = System.Drawing.SystemColors.Control;
            this.loadCGMButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.loadCGMButton.Location = new System.Drawing.Point(2, 313);
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
            this.gbInlayType.Location = new System.Drawing.Point(3, 0);
            this.gbInlayType.Name = "gbInlayType";
            this.gbInlayType.Size = new System.Drawing.Size(192, 41);
            this.gbInlayType.TabIndex = 73;
            this.gbInlayType.TabStop = false;
            this.gbInlayType.Text = "Type:";
            // 
            // inlayTypeCombo
            // 
            this.inlayTypeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.inlayTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inlayTypeCombo.FormattingEnabled = true;
            this.inlayTypeCombo.Location = new System.Drawing.Point(10, 15);
            this.inlayTypeCombo.Margin = new System.Windows.Forms.Padding(2);
            this.inlayTypeCombo.Name = "inlayTypeCombo";
            this.inlayTypeCombo.Size = new System.Drawing.Size(170, 21);
            this.inlayTypeCombo.TabIndex = 1;
            this.inlayTypeCombo.SelectedIndexChanged += new System.EventHandler(this.inlayTypeCombo_SelectedIndexChanged);
            // 
            // gbRbPlatform
            // 
            this.gbRbPlatform.Controls.Add(this.rbPs3);
            this.gbRbPlatform.Controls.Add(this.rbXbox360);
            this.gbRbPlatform.Controls.Add(this.rbMac);
            this.gbRbPlatform.Controls.Add(this.rbPc);
            this.gbRbPlatform.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbRbPlatform.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbRbPlatform.Location = new System.Drawing.Point(75, 457);
            this.gbRbPlatform.Name = "gbRbPlatform";
            this.gbRbPlatform.Size = new System.Drawing.Size(300, 41);
            this.gbRbPlatform.TabIndex = 94;
            this.gbRbPlatform.TabStop = false;
            this.gbRbPlatform.Text = "Platform:";
            this.gbRbPlatform.Visible = false;
            // 
            // rbPs3
            // 
            this.rbPs3.AutoSize = true;
            this.rbPs3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbPs3.Location = new System.Drawing.Point(248, 18);
            this.rbPs3.Name = "rbPs3";
            this.rbPs3.Size = new System.Drawing.Size(45, 17);
            this.rbPs3.TabIndex = 5;
            this.rbPs3.Text = "PS3";
            this.rbPs3.UseVisualStyleBackColor = true;
            this.rbPs3.CheckedChanged += new System.EventHandler(this.radioPlatform_CheckedChanged);
            // 
            // rbXbox360
            // 
            this.rbXbox360.AutoSize = true;
            this.rbXbox360.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbXbox360.Location = new System.Drawing.Point(150, 18);
            this.rbXbox360.Name = "rbXbox360";
            this.rbXbox360.Size = new System.Drawing.Size(68, 17);
            this.rbXbox360.TabIndex = 4;
            this.rbXbox360.Text = "XBox360";
            this.rbXbox360.UseVisualStyleBackColor = true;
            this.rbXbox360.CheckedChanged += new System.EventHandler(this.radioPlatform_CheckedChanged);
            // 
            // rbMac
            // 
            this.rbMac.AutoSize = true;
            this.rbMac.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbMac.Location = new System.Drawing.Point(74, 18);
            this.rbMac.Name = "rbMac";
            this.rbMac.Size = new System.Drawing.Size(48, 17);
            this.rbMac.TabIndex = 3;
            this.rbMac.Text = "MAC";
            this.rbMac.UseVisualStyleBackColor = true;
            this.rbMac.CheckedChanged += new System.EventHandler(this.radioPlatform_CheckedChanged);
            // 
            // rbPc
            // 
            this.rbPc.AutoSize = true;
            this.rbPc.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbPc.Location = new System.Drawing.Point(9, 18);
            this.rbPc.Name = "rbPc";
            this.rbPc.Size = new System.Drawing.Size(39, 17);
            this.rbPc.TabIndex = 2;
            this.rbPc.Text = "PC";
            this.rbPc.UseVisualStyleBackColor = true;
            this.rbPc.CheckedChanged += new System.EventHandler(this.radioPlatform_CheckedChanged);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 25000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // introScreensCreator1
            // 
            this.introScreensCreator1.Location = new System.Drawing.Point(209, 399);
            this.introScreensCreator1.Name = "introScreensCreator1";
            this.introScreensCreator1.Size = new System.Drawing.Size(166, 52);
            this.introScreensCreator1.TabIndex = 91;
            // 
            // authorTextbox
            // 
            this.authorTextbox.Cue = "Author";
            this.authorTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.authorTextbox.ForeColor = System.Drawing.Color.Gray;
            this.authorTextbox.Location = new System.Drawing.Point(10, 31);
            this.authorTextbox.Name = "authorTextbox";
            this.authorTextbox.Size = new System.Drawing.Size(188, 20);
            this.authorTextbox.TabIndex = 6;
            // 
            // inlayNameTextbox
            // 
            this.inlayNameTextbox.Cue = "Inlay Name";
            this.inlayNameTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.inlayNameTextbox.ForeColor = System.Drawing.Color.Gray;
            this.inlayNameTextbox.Location = new System.Drawing.Point(51, 63);
            this.inlayNameTextbox.Name = "inlayNameTextbox";
            this.inlayNameTextbox.Size = new System.Drawing.Size(147, 20);
            this.inlayNameTextbox.TabIndex = 9;
            this.inlayNameTextbox.Leave += new System.EventHandler(this.inlayNameTextbox_Leave);
            // 
            // DLCInlayCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbRbPlatform);
            this.Controls.Add(this.introScreensCreator1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gbInfo);
            this.Controls.Add(this.updateProgress);
            this.Controls.Add(this.currentOperationLabel);
            this.Controls.Add(this.saveCGMButton);
            this.Controls.Add(this.inlayGenerateButton);
            this.Controls.Add(this.helpLink);
            this.Controls.Add(this.loadCGMButton);
            this.Controls.Add(this.gbPlatform);
            this.Controls.Add(this.gbInlayType);
            this.Controls.Add(this.groupBox2);
            this.Name = "DLCInlayCreator";
            this.Size = new System.Drawing.Size(507, 520);
            this.toolTip.SetToolTip(this, "ReadMe: \r\nThis toolkit feature is no longer maintained or updated.\r\nPlease use th" +
                    "e latestest version of Custom Game Toolkit\r\nto create custom guitar inlays and o" +
                    "ther mods.");
            this.Load += new System.EventHandler(this.DLCInlayCreator_Load);
            this.Disposed += new System.EventHandler(this.DLCInlayCreator_Dispose);
            this.gbInfo.ResumeLayout(false);
            this.gbInfo.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picInlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picColored)).EndInit();
            this.gbPlatform.ResumeLayout(false);
            this.gbPlatform.PerformLayout();
            this.gbInlayType.ResumeLayout(false);
            this.gbRbPlatform.ResumeLayout(false);
            this.gbRbPlatform.PerformLayout();
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
        private Label label3;
        private LinkLabel helpLink;
        private ProgressBar updateProgress;
        private Label label4;
        private Label keyboardDescArrLabel;
        private Label label7;
        private Label label6;
        private IntroScreensCreator introScreensCreator1;
        public CueTextBox authorTextbox;
        public GroupBox gbRbPlatform;
        public RadioButton rbPs3;
        public RadioButton rbXbox360;
        public RadioButton rbMac;
        public RadioButton rbPc;
        private ToolTip toolTip;

    }
}
