using System.Windows.Forms;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    partial class ArrangementForm
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
            this.components = new System.ComponentModel.Container();
            this.btnBrowseXml = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.cmbArrangementType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbToneBase = new System.Windows.Forms.ComboBox();
            this.tbarScrollSpeed = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtScrollSpeed = new System.Windows.Forms.Label();
            this.chkBassPicked = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbArrangementName = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbTuningName = new System.Windows.Forms.ComboBox();
            this.gbTone = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbToneA = new System.Windows.Forms.ComboBox();
            this.chkTonesDisabled = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbToneD = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbToneC = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbToneB = new System.Windows.Forms.ComboBox();
            this.lblToneA = new System.Windows.Forms.Label();
            this.gbDLCId = new System.Windows.Forms.GroupBox();
            this.txtPersistentId = new RocksmithToolkitGUI.CueTextBox();
            this.txtMasterId = new RocksmithToolkitGUI.CueTextBox();
            this.gbXmlDefinition = new System.Windows.Forms.GroupBox();
            this.txtXmlPath = new RocksmithToolkitGUI.CueTextBox();
            this.gbArrInfo = new System.Windows.Forms.GroupBox();
            this.gbScrollSpeed = new System.Windows.Forms.GroupBox();
            this.btnEditType = new System.Windows.Forms.Button();
            this.btnEditTuning = new System.Windows.Forms.Button();
            this.chkBonusArrangement = new System.Windows.Forms.CheckBox();
            this.gbTuningPitch = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblRootNote = new System.Windows.Forms.Label();
            this.txtFrequency = new RocksmithToolkitGUI.CueTextBox();
            this.txtCentOffset = new RocksmithToolkitGUI.CueTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.gbGameplayPath = new System.Windows.Forms.GroupBox();
            this.rbRouteMaskNone = new System.Windows.Forms.RadioButton();
            this.rbRouteMaskBass = new System.Windows.Forms.RadioButton();
            this.rbRouteMaskRhythm = new System.Windows.Forms.RadioButton();
            this.rbRouteMaskLead = new System.Windows.Forms.RadioButton();
            this.chkMetronome = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tbarScrollSpeed)).BeginInit();
            this.gbTone.SuspendLayout();
            this.gbDLCId.SuspendLayout();
            this.gbXmlDefinition.SuspendLayout();
            this.gbArrInfo.SuspendLayout();
            this.gbScrollSpeed.SuspendLayout();
            this.gbTuningPitch.SuspendLayout();
            this.gbGameplayPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowseXml
            // 
            this.btnBrowseXml.Location = new System.Drawing.Point(363, 16);
            this.btnBrowseXml.Name = "btnBrowseXml";
            this.btnBrowseXml.Size = new System.Drawing.Size(62, 23);
            this.btnBrowseXml.TabIndex = 1;
            this.btnBrowseXml.Text = "Browse";
            this.btnBrowseXml.UseVisualStyleBackColor = true;
            this.btnBrowseXml.Click += new System.EventHandler(this.btnBrowseXml_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnOk.Location = new System.Drawing.Point(291, 440);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(72, 29);
            this.btnOk.TabIndex = 22;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbArrangementType
            // 
            this.cmbArrangementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArrangementType.FormattingEnabled = true;
            this.cmbArrangementType.Location = new System.Drawing.Point(54, 18);
            this.cmbArrangementType.Margin = new System.Windows.Forms.Padding(2);
            this.cmbArrangementType.Name = "cmbArrangementType";
            this.cmbArrangementType.Size = new System.Drawing.Size(119, 21);
            this.cmbArrangementType.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnCancel.Location = new System.Drawing.Point(369, 440);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 29);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbToneBase
            // 
            this.cmbToneBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToneBase.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbToneBase.FormattingEnabled = true;
            this.cmbToneBase.Location = new System.Drawing.Point(54, 20);
            this.cmbToneBase.Margin = new System.Windows.Forms.Padding(2);
            this.cmbToneBase.Name = "cmbToneBase";
            this.cmbToneBase.Size = new System.Drawing.Size(152, 21);
            this.cmbToneBase.TabIndex = 15;
            this.cmbToneBase.SelectedIndexChanged += new System.EventHandler(this.cmbTone_SelectedIndexChanged);
            // 
            // tbarScrollSpeed
            // 
            this.tbarScrollSpeed.AutoSize = false;
            this.tbarScrollSpeed.Location = new System.Drawing.Point(43, 14);
            this.tbarScrollSpeed.Margin = new System.Windows.Forms.Padding(2);
            this.tbarScrollSpeed.Maximum = 45;
            this.tbarScrollSpeed.Minimum = 5;
            this.tbarScrollSpeed.Name = "tbarScrollSpeed";
            this.tbarScrollSpeed.Size = new System.Drawing.Size(121, 23);
            this.tbarScrollSpeed.TabIndex = 9;
            this.tbarScrollSpeed.TabStop = false;
            this.tbarScrollSpeed.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.tbarScrollSpeed.Value = 20;
            this.tbarScrollSpeed.ValueChanged += new System.EventHandler(this.tbarScrollSpeed_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(5, 21);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Fastest";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(162, 21);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Slowest";
            // 
            // txtScrollSpeed
            // 
            this.txtScrollSpeed.AutoSize = true;
            this.txtScrollSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtScrollSpeed.Location = new System.Drawing.Point(71, 43);
            this.txtScrollSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtScrollSpeed.Name = "txtScrollSpeed";
            this.txtScrollSpeed.Size = new System.Drawing.Size(71, 13);
            this.txtScrollSpeed.TabIndex = 36;
            this.txtScrollSpeed.Text = "{Scroll Value}";
            // 
            // chkBassPicked
            // 
            this.chkBassPicked.AutoSize = true;
            this.chkBassPicked.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkBassPicked.Location = new System.Drawing.Point(54, 122);
            this.chkBassPicked.Name = "chkBassPicked";
            this.chkBassPicked.Size = new System.Drawing.Size(85, 17);
            this.chkBassPicked.TabIndex = 8;
            this.chkBassPicked.Text = "Bass Picked";
            this.chkBassPicked.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(7, 45);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "Name:";
            // 
            // cmbArrangementName
            // 
            this.cmbArrangementName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArrangementName.FormattingEnabled = true;
            this.cmbArrangementName.Location = new System.Drawing.Point(54, 43);
            this.cmbArrangementName.Margin = new System.Windows.Forms.Padding(2);
            this.cmbArrangementName.Name = "cmbArrangementName";
            this.cmbArrangementName.Size = new System.Drawing.Size(152, 21);
            this.cmbArrangementName.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(7, 71);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Tuning:";
            // 
            // cmbTuningName
            // 
            this.cmbTuningName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTuningName.FormattingEnabled = true;
            this.cmbTuningName.Location = new System.Drawing.Point(54, 68);
            this.cmbTuningName.Margin = new System.Windows.Forms.Padding(2);
            this.cmbTuningName.Name = "cmbTuningName";
            this.cmbTuningName.Size = new System.Drawing.Size(119, 21);
            this.cmbTuningName.TabIndex = 5;
            // 
            // gbTone
            // 
            this.gbTone.Controls.Add(this.label8);
            this.gbTone.Controls.Add(this.cmbToneA);
            this.gbTone.Controls.Add(this.chkTonesDisabled);
            this.gbTone.Controls.Add(this.label10);
            this.gbTone.Controls.Add(this.cmbToneD);
            this.gbTone.Controls.Add(this.label11);
            this.gbTone.Controls.Add(this.cmbToneC);
            this.gbTone.Controls.Add(this.label9);
            this.gbTone.Controls.Add(this.cmbToneB);
            this.gbTone.Controls.Add(this.lblToneA);
            this.gbTone.Controls.Add(this.cmbToneBase);
            this.gbTone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbTone.Location = new System.Drawing.Point(6, 267);
            this.gbTone.Name = "gbTone";
            this.gbTone.Padding = new System.Windows.Forms.Padding(0);
            this.gbTone.Size = new System.Drawing.Size(435, 113);
            this.gbTone.TabIndex = 42;
            this.gbTone.TabStop = false;
            this.gbTone.Text = "Tone Selector";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(7, 48);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 57;
            this.label8.Text = "Tone A:";
            // 
            // cmbToneA
            // 
            this.cmbToneA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToneA.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbToneA.FormattingEnabled = true;
            this.cmbToneA.Location = new System.Drawing.Point(54, 45);
            this.cmbToneA.Margin = new System.Windows.Forms.Padding(2);
            this.cmbToneA.Name = "cmbToneA";
            this.cmbToneA.Size = new System.Drawing.Size(152, 21);
            this.cmbToneA.TabIndex = 56;
            this.cmbToneA.SelectedIndexChanged += new System.EventHandler(this.cmbTone_SelectedIndexChanged);
            // 
            // chkTonesDisabled
            // 
            this.chkTonesDisabled.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkTonesDisabled.Location = new System.Drawing.Point(10, 71);
            this.chkTonesDisabled.Name = "chkTonesDisabled";
            this.chkTonesDisabled.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkTonesDisabled.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkTonesDisabled.Size = new System.Drawing.Size(196, 39);
            this.chkTonesDisabled.TabIndex = 19;
            this.chkTonesDisabled.Text = "If checked, tone slots are disabled to prevent multitone failure.";
            this.chkTonesDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkTonesDisabled.UseVisualStyleBackColor = true;
            this.chkTonesDisabled.CheckedChanged += new System.EventHandler(this.chkTonesDisabled_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(221, 72);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 55;
            this.label10.Text = "Tone D:";
            // 
            // cmbToneD
            // 
            this.cmbToneD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToneD.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbToneD.FormattingEnabled = true;
            this.cmbToneD.Location = new System.Drawing.Point(270, 69);
            this.cmbToneD.Margin = new System.Windows.Forms.Padding(2);
            this.cmbToneD.Name = "cmbToneD";
            this.cmbToneD.Size = new System.Drawing.Size(152, 21);
            this.cmbToneD.TabIndex = 18;
            this.cmbToneD.SelectedIndexChanged += new System.EventHandler(this.cmbTone_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(221, 47);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 53;
            this.label11.Text = "Tone C:";
            // 
            // cmbToneC
            // 
            this.cmbToneC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToneC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbToneC.FormattingEnabled = true;
            this.cmbToneC.Location = new System.Drawing.Point(270, 44);
            this.cmbToneC.Margin = new System.Windows.Forms.Padding(2);
            this.cmbToneC.Name = "cmbToneC";
            this.cmbToneC.Size = new System.Drawing.Size(152, 21);
            this.cmbToneC.TabIndex = 17;
            this.cmbToneC.SelectedIndexChanged += new System.EventHandler(this.cmbTone_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(221, 23);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 49;
            this.label9.Text = "Tone B:";
            // 
            // cmbToneB
            // 
            this.cmbToneB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToneB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbToneB.FormattingEnabled = true;
            this.cmbToneB.Location = new System.Drawing.Point(270, 19);
            this.cmbToneB.Margin = new System.Windows.Forms.Padding(2);
            this.cmbToneB.Name = "cmbToneB";
            this.cmbToneB.Size = new System.Drawing.Size(152, 21);
            this.cmbToneB.TabIndex = 16;
            this.cmbToneB.SelectedIndexChanged += new System.EventHandler(this.cmbTone_SelectedIndexChanged);
            // 
            // lblToneA
            // 
            this.lblToneA.AutoSize = true;
            this.lblToneA.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblToneA.Location = new System.Drawing.Point(7, 23);
            this.lblToneA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblToneA.Name = "lblToneA";
            this.lblToneA.Size = new System.Drawing.Size(34, 13);
            this.lblToneA.TabIndex = 45;
            this.lblToneA.Text = "Base:";
            // 
            // gbDLCId
            // 
            this.gbDLCId.Controls.Add(this.txtPersistentId);
            this.gbDLCId.Controls.Add(this.txtMasterId);
            this.gbDLCId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbDLCId.Location = new System.Drawing.Point(6, 386);
            this.gbDLCId.Name = "gbDLCId";
            this.gbDLCId.Size = new System.Drawing.Size(435, 47);
            this.gbDLCId.TabIndex = 33;
            this.gbDLCId.TabStop = false;
            this.gbDLCId.Text = "Arrangement Identification";
            // 
            // txtPersistentId
            // 
            this.txtPersistentId.Cue = "PersistentID";
            this.txtPersistentId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtPersistentId.ForeColor = System.Drawing.Color.Gray;
            this.txtPersistentId.Location = new System.Drawing.Point(156, 19);
            this.txtPersistentId.Name = "txtPersistentId";
            this.txtPersistentId.Size = new System.Drawing.Size(269, 20);
            this.txtPersistentId.TabIndex = 21;
            // 
            // txtMasterId
            // 
            this.txtMasterId.Cue = "MasterID";
            this.txtMasterId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtMasterId.ForeColor = System.Drawing.Color.Gray;
            this.txtMasterId.Location = new System.Drawing.Point(6, 19);
            this.txtMasterId.Name = "txtMasterId";
            this.txtMasterId.Size = new System.Drawing.Size(144, 20);
            this.txtMasterId.TabIndex = 20;
            // 
            // gbXmlDefinition
            // 
            this.gbXmlDefinition.Controls.Add(this.txtXmlPath);
            this.gbXmlDefinition.Controls.Add(this.btnBrowseXml);
            this.gbXmlDefinition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbXmlDefinition.Location = new System.Drawing.Point(6, 11);
            this.gbXmlDefinition.Name = "gbXmlDefinition";
            this.gbXmlDefinition.Size = new System.Drawing.Size(435, 46);
            this.gbXmlDefinition.TabIndex = 44;
            this.gbXmlDefinition.TabStop = false;
            this.gbXmlDefinition.Text = "Song XML File";
            // 
            // txtXmlPath
            // 
            this.txtXmlPath.Cue = "Song Xml File (*.xml)";
            this.txtXmlPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtXmlPath.ForeColor = System.Drawing.Color.Gray;
            this.txtXmlPath.Location = new System.Drawing.Point(10, 17);
            this.txtXmlPath.Multiline = true;
            this.txtXmlPath.Name = "txtXmlPath";
            this.txtXmlPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtXmlPath.Size = new System.Drawing.Size(347, 20);
            this.txtXmlPath.TabIndex = 0;
            // 
            // gbArrInfo
            // 
            this.gbArrInfo.Controls.Add(this.gbScrollSpeed);
            this.gbArrInfo.Controls.Add(this.btnEditType);
            this.gbArrInfo.Controls.Add(this.btnEditTuning);
            this.gbArrInfo.Controls.Add(this.chkBonusArrangement);
            this.gbArrInfo.Controls.Add(this.gbTuningPitch);
            this.gbArrInfo.Controls.Add(this.chkBassPicked);
            this.gbArrInfo.Controls.Add(this.label4);
            this.gbArrInfo.Controls.Add(this.cmbArrangementType);
            this.gbArrInfo.Controls.Add(this.label1);
            this.gbArrInfo.Controls.Add(this.cmbArrangementName);
            this.gbArrInfo.Controls.Add(this.label6);
            this.gbArrInfo.Controls.Add(this.cmbTuningName);
            this.gbArrInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbArrInfo.Location = new System.Drawing.Point(6, 64);
            this.gbArrInfo.Name = "gbArrInfo";
            this.gbArrInfo.Size = new System.Drawing.Size(435, 147);
            this.gbArrInfo.TabIndex = 45;
            this.gbArrInfo.TabStop = false;
            this.gbArrInfo.Text = "Arrangement Information";
            // 
            // gbScrollSpeed
            // 
            this.gbScrollSpeed.Controls.Add(this.label5);
            this.gbScrollSpeed.Controls.Add(this.label3);
            this.gbScrollSpeed.Controls.Add(this.txtScrollSpeed);
            this.gbScrollSpeed.Controls.Add(this.tbarScrollSpeed);
            this.gbScrollSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbScrollSpeed.Location = new System.Drawing.Point(214, 12);
            this.gbScrollSpeed.Name = "gbScrollSpeed";
            this.gbScrollSpeed.Size = new System.Drawing.Size(211, 60);
            this.gbScrollSpeed.TabIndex = 42;
            this.gbScrollSpeed.TabStop = false;
            this.gbScrollSpeed.Text = "Scroll Speed";
            // 
            // btnEditType
            // 
            this.btnEditType.Enabled = false;
            this.btnEditType.Location = new System.Drawing.Point(178, 17);
            this.btnEditType.Name = "btnEditType";
            this.btnEditType.Size = new System.Drawing.Size(28, 23);
            this.btnEditType.TabIndex = 3;
            this.btnEditType.Text = "...";
            this.toolTip.SetToolTip(this.btnEditType, "Click to open type editor.");
            this.btnEditType.UseVisualStyleBackColor = true;
            this.btnEditType.Click += new System.EventHandler(this.btnEditType_Click);
            // 
            // btnEditTuning
            // 
            this.btnEditTuning.Location = new System.Drawing.Point(178, 67);
            this.btnEditTuning.Name = "btnEditTuning";
            this.btnEditTuning.Size = new System.Drawing.Size(28, 23);
            this.btnEditTuning.TabIndex = 6;
            this.btnEditTuning.Text = "...";
            this.toolTip.SetToolTip(this.btnEditTuning, "Click to open tuning editor.");
            this.btnEditTuning.UseVisualStyleBackColor = true;
            this.btnEditTuning.Click += new System.EventHandler(this.btnEditTuning_Click);
            // 
            // chkBonusArrangement
            // 
            this.chkBonusArrangement.AutoSize = true;
            this.chkBonusArrangement.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkBonusArrangement.Location = new System.Drawing.Point(54, 99);
            this.chkBonusArrangement.Name = "chkBonusArrangement";
            this.chkBonusArrangement.Size = new System.Drawing.Size(119, 17);
            this.chkBonusArrangement.TabIndex = 7;
            this.chkBonusArrangement.Text = "Bonus Arrangement";
            this.chkBonusArrangement.UseVisualStyleBackColor = true;
            // 
            // gbTuningPitch
            // 
            this.gbTuningPitch.Controls.Add(this.label12);
            this.gbTuningPitch.Controls.Add(this.lblRootNote);
            this.gbTuningPitch.Controls.Add(this.txtFrequency);
            this.gbTuningPitch.Controls.Add(this.txtCentOffset);
            this.gbTuningPitch.Controls.Add(this.label2);
            this.gbTuningPitch.Controls.Add(this.label14);
            this.gbTuningPitch.Controls.Add(this.label7);
            this.gbTuningPitch.Controls.Add(this.label13);
            this.gbTuningPitch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbTuningPitch.Location = new System.Drawing.Point(214, 79);
            this.gbTuningPitch.Name = "gbTuningPitch";
            this.gbTuningPitch.Size = new System.Drawing.Size(211, 60);
            this.gbTuningPitch.TabIndex = 43;
            this.gbTuningPitch.TabStop = false;
            this.gbTuningPitch.Text = "Tuning Pitch";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(115, 38);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 15);
            this.label12.TabIndex = 49;
            this.label12.Text = "¢";
            // 
            // lblRootNote
            // 
            this.lblRootNote.AutoSize = true;
            this.lblRootNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRootNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblRootNote.Location = new System.Drawing.Point(155, 36);
            this.lblRootNote.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRootNote.Name = "lblRootNote";
            this.lblRootNote.Size = new System.Drawing.Size(50, 16);
            this.lblRootNote.TabIndex = 48;
            this.lblRootNote.Text = "{note}";
            this.lblRootNote.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtFrequency
            // 
            this.txtFrequency.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFrequency.Cue = "";
            this.txtFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtFrequency.ForeColor = System.Drawing.Color.Black;
            this.txtFrequency.Location = new System.Drawing.Point(66, 20);
            this.txtFrequency.MaxLength = 9;
            this.txtFrequency.Name = "txtFrequency";
            this.txtFrequency.Size = new System.Drawing.Size(50, 13);
            this.txtFrequency.TabIndex = 10;
            this.txtFrequency.Text = "{A440}";
            this.txtFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtFrequency.TextChanged += new System.EventHandler(this.txtFrequency_TextChanged);
            // 
            // txtCentOffset
            // 
            this.txtCentOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCentOffset.Cue = "";
            this.txtCentOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtCentOffset.ForeColor = System.Drawing.Color.Black;
            this.txtCentOffset.Location = new System.Drawing.Point(66, 41);
            this.txtCentOffset.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtCentOffset.MaxLength = 6;
            this.txtCentOffset.Name = "txtCentOffset";
            this.txtCentOffset.Size = new System.Drawing.Size(50, 13);
            this.txtCentOffset.TabIndex = 11;
            this.txtCentOffset.Text = "{cOffset}";
            this.txtCentOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCentOffset.TextChanged += new System.EventHandler(this.txtFrequency_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(7, 19);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Frequency:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(146, 19);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 13);
            this.label14.TabIndex = 47;
            this.label14.Text = "Root Note:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(8, 41);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 44;
            this.label7.Text = "Offset:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(115, 20);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(20, 13);
            this.label13.TabIndex = 45;
            this.label13.Text = "Hz";
            // 
            // gbGameplayPath
            // 
            this.gbGameplayPath.Controls.Add(this.rbRouteMaskNone);
            this.gbGameplayPath.Controls.Add(this.rbRouteMaskBass);
            this.gbGameplayPath.Controls.Add(this.rbRouteMaskRhythm);
            this.gbGameplayPath.Controls.Add(this.rbRouteMaskLead);
            this.gbGameplayPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbGameplayPath.Location = new System.Drawing.Point(6, 217);
            this.gbGameplayPath.Name = "gbGameplayPath";
            this.gbGameplayPath.Size = new System.Drawing.Size(435, 44);
            this.gbGameplayPath.TabIndex = 34;
            this.gbGameplayPath.TabStop = false;
            this.gbGameplayPath.Text = "Gameplay Path";
            // 
            // rbRouteMaskNone
            // 
            this.rbRouteMaskNone.AutoSize = true;
            this.rbRouteMaskNone.Checked = true;
            this.rbRouteMaskNone.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbRouteMaskNone.Location = new System.Drawing.Point(322, 19);
            this.rbRouteMaskNone.Name = "rbRouteMaskNone";
            this.rbRouteMaskNone.Size = new System.Drawing.Size(51, 17);
            this.rbRouteMaskNone.TabIndex = 14;
            this.rbRouteMaskNone.TabStop = true;
            this.rbRouteMaskNone.Text = "None";
            this.rbRouteMaskNone.UseVisualStyleBackColor = true;
            // 
            // rbRouteMaskBass
            // 
            this.rbRouteMaskBass.AutoSize = true;
            this.rbRouteMaskBass.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbRouteMaskBass.Location = new System.Drawing.Point(243, 19);
            this.rbRouteMaskBass.Name = "rbRouteMaskBass";
            this.rbRouteMaskBass.Size = new System.Drawing.Size(48, 17);
            this.rbRouteMaskBass.TabIndex = 13;
            this.rbRouteMaskBass.Text = "Bass";
            this.rbRouteMaskBass.UseVisualStyleBackColor = true;
            // 
            // rbRouteMaskRhythm
            // 
            this.rbRouteMaskRhythm.AutoSize = true;
            this.rbRouteMaskRhythm.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbRouteMaskRhythm.Location = new System.Drawing.Point(156, 19);
            this.rbRouteMaskRhythm.Name = "rbRouteMaskRhythm";
            this.rbRouteMaskRhythm.Size = new System.Drawing.Size(61, 17);
            this.rbRouteMaskRhythm.TabIndex = 12;
            this.rbRouteMaskRhythm.Text = "Rhythm";
            this.rbRouteMaskRhythm.UseVisualStyleBackColor = true;
            // 
            // rbRouteMaskLead
            // 
            this.rbRouteMaskLead.AutoSize = true;
            this.rbRouteMaskLead.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbRouteMaskLead.Location = new System.Drawing.Point(72, 19);
            this.rbRouteMaskLead.Name = "rbRouteMaskLead";
            this.rbRouteMaskLead.Size = new System.Drawing.Size(49, 17);
            this.rbRouteMaskLead.TabIndex = 46;
            this.rbRouteMaskLead.Text = "Lead";
            this.rbRouteMaskLead.UseVisualStyleBackColor = true;
            // 
            // chkMetronome
            // 
            this.chkMetronome.AutoSize = true;
            this.chkMetronome.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkMetronome.Location = new System.Drawing.Point(16, 447);
            this.chkMetronome.Name = "chkMetronome";
            this.chkMetronome.Size = new System.Drawing.Size(209, 17);
            this.chkMetronome.TabIndex = 7;
            this.chkMetronome.Text = "Create Metronome Bonus Arrangement";
            this.chkMetronome.UseVisualStyleBackColor = true;
            // 
            // ArrangementForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(450, 481);
            this.Controls.Add(this.gbGameplayPath);
            this.Controls.Add(this.gbArrInfo);
            this.Controls.Add(this.chkMetronome);
            this.Controls.Add(this.gbXmlDefinition);
            this.Controls.Add(this.gbDLCId);
            this.Controls.Add(this.gbTone);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArrangementForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Arrangement";
            this.Load += new System.EventHandler(this.ArrangementForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbarScrollSpeed)).EndInit();
            this.gbTone.ResumeLayout(false);
            this.gbTone.PerformLayout();
            this.gbDLCId.ResumeLayout(false);
            this.gbDLCId.PerformLayout();
            this.gbXmlDefinition.ResumeLayout(false);
            this.gbXmlDefinition.PerformLayout();
            this.gbArrInfo.ResumeLayout(false);
            this.gbArrInfo.PerformLayout();
            this.gbScrollSpeed.ResumeLayout(false);
            this.gbScrollSpeed.PerformLayout();
            this.gbTuningPitch.ResumeLayout(false);
            this.gbTuningPitch.PerformLayout();
            this.gbGameplayPath.ResumeLayout(false);
            this.gbGameplayPath.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.CheckBox chkMetronome;
        private System.Windows.Forms.GroupBox gbDLCId;
        private CueTextBox txtMasterId;
        private CueTextBox txtXmlPath;
        private CueTextBox txtPersistentId;
        private CueTextBox txtCentOffset;

        #endregion

        private System.Windows.Forms.Button btnBrowseXml;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ComboBox cmbArrangementType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbToneBase;
        private System.Windows.Forms.TrackBar tbarScrollSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label txtScrollSpeed;
        private System.Windows.Forms.CheckBox chkBassPicked;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbArrangementName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbTuningName;
        private System.Windows.Forms.GroupBox gbTone;
        private System.Windows.Forms.Label lblToneA;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbToneD;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbToneC;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbToneB;
        private System.Windows.Forms.GroupBox gbXmlDefinition;
        private System.Windows.Forms.GroupBox gbArrInfo;
        private System.Windows.Forms.GroupBox gbScrollSpeed;
        private System.Windows.Forms.GroupBox gbGameplayPath;
        private System.Windows.Forms.RadioButton rbRouteMaskLead;
        private System.Windows.Forms.RadioButton rbRouteMaskBass;
        private System.Windows.Forms.RadioButton rbRouteMaskRhythm;
        private System.Windows.Forms.RadioButton rbRouteMaskNone;
        private System.Windows.Forms.Label label7;
        private CueTextBox txtFrequency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox gbTuningPitch;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblRootNote;
        private System.Windows.Forms.CheckBox chkBonusArrangement;
        private System.Windows.Forms.CheckBox chkTonesDisabled;
        private System.Windows.Forms.Button btnEditTuning;
        private System.Windows.Forms.Button btnEditType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbToneA;
        private Label label12;
        private ToolTip toolTip;
    }
}