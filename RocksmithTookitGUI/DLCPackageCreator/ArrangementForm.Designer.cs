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
            this.songXmlBrowseButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.arrangementTypeCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toneBaseCombo = new System.Windows.Forms.ComboBox();
            this.scrollSpeedTrackBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.scrollSpeedDisplay = new System.Windows.Forms.Label();
            this.Picked = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.arrangementNameCombo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tuningComboBox = new System.Windows.Forms.ComboBox();
            this.gbTone = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.toneACombo = new System.Windows.Forms.ComboBox();
            this.disableTonesCheckbox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.toneDCombo = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.toneCCombo = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.toneBCombo = new System.Windows.Forms.ComboBox();
            this.lblToneA = new System.Windows.Forms.Label();
            this.gbDLCId = new System.Windows.Forms.GroupBox();
            this.PersistentId = new RocksmithToolkitGUI.CueTextBox();
            this.MasterId = new RocksmithToolkitGUI.CueTextBox();
            this.gbXmlDefinition = new System.Windows.Forms.GroupBox();
            this.XmlFilePath = new RocksmithToolkitGUI.CueTextBox();
            this.gbArrInfo = new System.Windows.Forms.GroupBox();
            this.gbScrollSpeed = new System.Windows.Forms.GroupBox();
            this.typeEdit = new System.Windows.Forms.Button();
            this.tuningEditButton = new System.Windows.Forms.Button();
            this.BonusCheckBox = new System.Windows.Forms.CheckBox();
            this.gbTuningPitch = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.noteDisplay = new System.Windows.Forms.Label();
            this.frequencyTB = new RocksmithToolkitGUI.CueTextBox();
            this.centOffsetDisplay = new RocksmithToolkitGUI.CueTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.gbGameplayPath = new System.Windows.Forms.GroupBox();
            this.routeMaskNoneRadio = new System.Windows.Forms.RadioButton();
            this.routeMaskBassRadio = new System.Windows.Forms.RadioButton();
            this.routeMaskRhythmRadio = new System.Windows.Forms.RadioButton();
            this.routeMaskLeadRadio = new System.Windows.Forms.RadioButton();
            this.MetronomeCb = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.scrollSpeedTrackBar)).BeginInit();
            this.gbTone.SuspendLayout();
            this.gbDLCId.SuspendLayout();
            this.gbXmlDefinition.SuspendLayout();
            this.gbArrInfo.SuspendLayout();
            this.gbScrollSpeed.SuspendLayout();
            this.gbTuningPitch.SuspendLayout();
            this.gbGameplayPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // songXmlBrowseButton
            // 
            this.songXmlBrowseButton.Location = new System.Drawing.Point(363, 16);
            this.songXmlBrowseButton.Name = "songXmlBrowseButton";
            this.songXmlBrowseButton.Size = new System.Drawing.Size(62, 23);
            this.songXmlBrowseButton.TabIndex = 1;
            this.songXmlBrowseButton.Text = "Browse";
            this.songXmlBrowseButton.UseVisualStyleBackColor = true;
            this.songXmlBrowseButton.Click += new System.EventHandler(this.songXmlBrowseButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.OkButton.Location = new System.Drawing.Point(291, 440);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(72, 29);
            this.OkButton.TabIndex = 22;
            this.OkButton.Text = "Ok";
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // arrangementTypeCombo
            // 
            this.arrangementTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arrangementTypeCombo.FormattingEnabled = true;
            this.arrangementTypeCombo.Location = new System.Drawing.Point(54, 18);
            this.arrangementTypeCombo.Margin = new System.Windows.Forms.Padding(2);
            this.arrangementTypeCombo.Name = "arrangementTypeCombo";
            this.arrangementTypeCombo.Size = new System.Drawing.Size(119, 21);
            this.arrangementTypeCombo.TabIndex = 2;
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
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.cancelButton.Location = new System.Drawing.Point(369, 440);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(72, 29);
            this.cancelButton.TabIndex = 23;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // toneBaseCombo
            // 
            this.toneBaseCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toneBaseCombo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toneBaseCombo.FormattingEnabled = true;
            this.toneBaseCombo.Location = new System.Drawing.Point(54, 20);
            this.toneBaseCombo.Margin = new System.Windows.Forms.Padding(2);
            this.toneBaseCombo.Name = "toneBaseCombo";
            this.toneBaseCombo.Size = new System.Drawing.Size(152, 21);
            this.toneBaseCombo.TabIndex = 15;
            // 
            // scrollSpeedTrackBar
            // 
            this.scrollSpeedTrackBar.AutoSize = false;
            this.scrollSpeedTrackBar.Location = new System.Drawing.Point(43, 14);
            this.scrollSpeedTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.scrollSpeedTrackBar.Maximum = 45;
            this.scrollSpeedTrackBar.Minimum = 5;
            this.scrollSpeedTrackBar.Name = "scrollSpeedTrackBar";
            this.scrollSpeedTrackBar.Size = new System.Drawing.Size(121, 23);
            this.scrollSpeedTrackBar.TabIndex = 9;
            this.scrollSpeedTrackBar.TabStop = false;
            this.scrollSpeedTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.scrollSpeedTrackBar.Value = 20;
            this.scrollSpeedTrackBar.ValueChanged += new System.EventHandler(this.scrollSpeedTrackBar_ValueChanged);
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
            // scrollSpeedDisplay
            // 
            this.scrollSpeedDisplay.AutoSize = true;
            this.scrollSpeedDisplay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.scrollSpeedDisplay.Location = new System.Drawing.Point(71, 43);
            this.scrollSpeedDisplay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.scrollSpeedDisplay.Name = "scrollSpeedDisplay";
            this.scrollSpeedDisplay.Size = new System.Drawing.Size(71, 13);
            this.scrollSpeedDisplay.TabIndex = 36;
            this.scrollSpeedDisplay.Text = "{Scroll Value}";
            // 
            // Picked
            // 
            this.Picked.AutoSize = true;
            this.Picked.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Picked.Location = new System.Drawing.Point(54, 122);
            this.Picked.Name = "Picked";
            this.Picked.Size = new System.Drawing.Size(85, 17);
            this.Picked.TabIndex = 8;
            this.Picked.Text = "Bass Picked";
            this.Picked.UseVisualStyleBackColor = true;
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
            // arrangementNameCombo
            // 
            this.arrangementNameCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arrangementNameCombo.FormattingEnabled = true;
            this.arrangementNameCombo.Location = new System.Drawing.Point(54, 43);
            this.arrangementNameCombo.Margin = new System.Windows.Forms.Padding(2);
            this.arrangementNameCombo.Name = "arrangementNameCombo";
            this.arrangementNameCombo.Size = new System.Drawing.Size(152, 21);
            this.arrangementNameCombo.TabIndex = 4;
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
            // tuningComboBox
            // 
            this.tuningComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tuningComboBox.FormattingEnabled = true;
            this.tuningComboBox.Location = new System.Drawing.Point(54, 68);
            this.tuningComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.tuningComboBox.Name = "tuningComboBox";
            this.tuningComboBox.Size = new System.Drawing.Size(119, 21);
            this.tuningComboBox.TabIndex = 5;
            // 
            // gbTone
            // 
            this.gbTone.Controls.Add(this.label8);
            this.gbTone.Controls.Add(this.toneACombo);
            this.gbTone.Controls.Add(this.disableTonesCheckbox);
            this.gbTone.Controls.Add(this.label10);
            this.gbTone.Controls.Add(this.toneDCombo);
            this.gbTone.Controls.Add(this.label11);
            this.gbTone.Controls.Add(this.toneCCombo);
            this.gbTone.Controls.Add(this.label9);
            this.gbTone.Controls.Add(this.toneBCombo);
            this.gbTone.Controls.Add(this.lblToneA);
            this.gbTone.Controls.Add(this.toneBaseCombo);
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
            // toneACombo
            // 
            this.toneACombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toneACombo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toneACombo.FormattingEnabled = true;
            this.toneACombo.Location = new System.Drawing.Point(54, 45);
            this.toneACombo.Margin = new System.Windows.Forms.Padding(2);
            this.toneACombo.Name = "toneACombo";
            this.toneACombo.Size = new System.Drawing.Size(152, 21);
            this.toneACombo.TabIndex = 56;
            this.toneACombo.SelectedIndexChanged += new System.EventHandler(this.toneCombo_SelectedIndexChanged);
            // 
            // disableTonesCheckbox
            // 
            this.disableTonesCheckbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.disableTonesCheckbox.Location = new System.Drawing.Point(10, 71);
            this.disableTonesCheckbox.Name = "disableTonesCheckbox";
            this.disableTonesCheckbox.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.disableTonesCheckbox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.disableTonesCheckbox.Size = new System.Drawing.Size(196, 39);
            this.disableTonesCheckbox.TabIndex = 19;
            this.disableTonesCheckbox.Text = "If checked, tone slots are disabled to prevent multitone failure.";
            this.disableTonesCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.disableTonesCheckbox.UseVisualStyleBackColor = true;
            this.disableTonesCheckbox.CheckedChanged += new System.EventHandler(this.disableTonesCheckbox_CheckedChanged);
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
            // toneDCombo
            // 
            this.toneDCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toneDCombo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toneDCombo.FormattingEnabled = true;
            this.toneDCombo.Location = new System.Drawing.Point(270, 69);
            this.toneDCombo.Margin = new System.Windows.Forms.Padding(2);
            this.toneDCombo.Name = "toneDCombo";
            this.toneDCombo.Size = new System.Drawing.Size(152, 21);
            this.toneDCombo.TabIndex = 18;
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
            // toneCCombo
            // 
            this.toneCCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toneCCombo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toneCCombo.FormattingEnabled = true;
            this.toneCCombo.Location = new System.Drawing.Point(270, 44);
            this.toneCCombo.Margin = new System.Windows.Forms.Padding(2);
            this.toneCCombo.Name = "toneCCombo";
            this.toneCCombo.Size = new System.Drawing.Size(152, 21);
            this.toneCCombo.TabIndex = 17;
            this.toneCCombo.SelectedIndexChanged += new System.EventHandler(this.toneCombo_SelectedIndexChanged);
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
            // toneBCombo
            // 
            this.toneBCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toneBCombo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toneBCombo.FormattingEnabled = true;
            this.toneBCombo.Location = new System.Drawing.Point(270, 19);
            this.toneBCombo.Margin = new System.Windows.Forms.Padding(2);
            this.toneBCombo.Name = "toneBCombo";
            this.toneBCombo.Size = new System.Drawing.Size(152, 21);
            this.toneBCombo.TabIndex = 16;
            this.toneBCombo.SelectedIndexChanged += new System.EventHandler(this.toneCombo_SelectedIndexChanged);
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
            this.gbDLCId.Controls.Add(this.PersistentId);
            this.gbDLCId.Controls.Add(this.MasterId);
            this.gbDLCId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbDLCId.Location = new System.Drawing.Point(6, 386);
            this.gbDLCId.Name = "gbDLCId";
            this.gbDLCId.Size = new System.Drawing.Size(435, 47);
            this.gbDLCId.TabIndex = 33;
            this.gbDLCId.TabStop = false;
            this.gbDLCId.Text = "Arrangement Identification";
            // 
            // PersistentId
            // 
            this.PersistentId.Cue = "PersistentID";
            this.PersistentId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PersistentId.ForeColor = System.Drawing.Color.Gray;
            this.PersistentId.Location = new System.Drawing.Point(156, 19);
            this.PersistentId.Name = "PersistentId";
            this.PersistentId.Size = new System.Drawing.Size(269, 20);
            this.PersistentId.TabIndex = 21;
            // 
            // MasterId
            // 
            this.MasterId.Cue = "MasterID";
            this.MasterId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MasterId.ForeColor = System.Drawing.Color.Gray;
            this.MasterId.Location = new System.Drawing.Point(6, 19);
            this.MasterId.Name = "MasterId";
            this.MasterId.Size = new System.Drawing.Size(144, 20);
            this.MasterId.TabIndex = 20;
            // 
            // gbXmlDefinition
            // 
            this.gbXmlDefinition.Controls.Add(this.XmlFilePath);
            this.gbXmlDefinition.Controls.Add(this.songXmlBrowseButton);
            this.gbXmlDefinition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbXmlDefinition.Location = new System.Drawing.Point(6, 11);
            this.gbXmlDefinition.Name = "gbXmlDefinition";
            this.gbXmlDefinition.Size = new System.Drawing.Size(435, 46);
            this.gbXmlDefinition.TabIndex = 44;
            this.gbXmlDefinition.TabStop = false;
            this.gbXmlDefinition.Text = "Song XML File";
            // 
            // XmlFilePath
            // 
            this.XmlFilePath.Cue = "Song Xml File (*.xml)";
            this.XmlFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.XmlFilePath.ForeColor = System.Drawing.Color.Gray;
            this.XmlFilePath.Location = new System.Drawing.Point(10, 17);
            this.XmlFilePath.Multiline = true;
            this.XmlFilePath.Name = "XmlFilePath";
            this.XmlFilePath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.XmlFilePath.Size = new System.Drawing.Size(347, 20);
            this.XmlFilePath.TabIndex = 0;
            // 
            // gbArrInfo
            // 
            this.gbArrInfo.Controls.Add(this.gbScrollSpeed);
            this.gbArrInfo.Controls.Add(this.typeEdit);
            this.gbArrInfo.Controls.Add(this.tuningEditButton);
            this.gbArrInfo.Controls.Add(this.BonusCheckBox);
            this.gbArrInfo.Controls.Add(this.gbTuningPitch);
            this.gbArrInfo.Controls.Add(this.Picked);
            this.gbArrInfo.Controls.Add(this.label4);
            this.gbArrInfo.Controls.Add(this.arrangementTypeCombo);
            this.gbArrInfo.Controls.Add(this.label1);
            this.gbArrInfo.Controls.Add(this.arrangementNameCombo);
            this.gbArrInfo.Controls.Add(this.label6);
            this.gbArrInfo.Controls.Add(this.tuningComboBox);
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
            this.gbScrollSpeed.Controls.Add(this.scrollSpeedDisplay);
            this.gbScrollSpeed.Controls.Add(this.scrollSpeedTrackBar);
            this.gbScrollSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbScrollSpeed.Location = new System.Drawing.Point(214, 12);
            this.gbScrollSpeed.Name = "gbScrollSpeed";
            this.gbScrollSpeed.Size = new System.Drawing.Size(211, 60);
            this.gbScrollSpeed.TabIndex = 42;
            this.gbScrollSpeed.TabStop = false;
            this.gbScrollSpeed.Text = "Scroll Speed";
            // 
            // typeEdit
            // 
            this.typeEdit.Enabled = false;
            this.typeEdit.Location = new System.Drawing.Point(178, 17);
            this.typeEdit.Name = "typeEdit";
            this.typeEdit.Size = new System.Drawing.Size(28, 23);
            this.typeEdit.TabIndex = 3;
            this.typeEdit.Text = "...";
            this.typeEdit.UseVisualStyleBackColor = true;
            this.typeEdit.Click += new System.EventHandler(this.typeEdit_Click);
            // 
            // tuningEditButton
            // 
            this.tuningEditButton.Location = new System.Drawing.Point(178, 67);
            this.tuningEditButton.Name = "tuningEditButton";
            this.tuningEditButton.Size = new System.Drawing.Size(28, 23);
            this.tuningEditButton.TabIndex = 6;
            this.tuningEditButton.Text = "...";
            this.tuningEditButton.UseVisualStyleBackColor = true;
            this.tuningEditButton.Click += new System.EventHandler(this.tuningEditButton_Click);
            // 
            // BonusCheckBox
            // 
            this.BonusCheckBox.AutoSize = true;
            this.BonusCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BonusCheckBox.Location = new System.Drawing.Point(54, 99);
            this.BonusCheckBox.Name = "BonusCheckBox";
            this.BonusCheckBox.Size = new System.Drawing.Size(119, 17);
            this.BonusCheckBox.TabIndex = 7;
            this.BonusCheckBox.Text = "Bonus Arrangement";
            this.BonusCheckBox.UseVisualStyleBackColor = true;
            // 
            // gbTuningPitch
            // 
            this.gbTuningPitch.Controls.Add(this.label12);
            this.gbTuningPitch.Controls.Add(this.noteDisplay);
            this.gbTuningPitch.Controls.Add(this.frequencyTB);
            this.gbTuningPitch.Controls.Add(this.centOffsetDisplay);
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
            // noteDisplay
            // 
            this.noteDisplay.AutoSize = true;
            this.noteDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.noteDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.noteDisplay.Location = new System.Drawing.Point(155, 36);
            this.noteDisplay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.noteDisplay.Name = "noteDisplay";
            this.noteDisplay.Size = new System.Drawing.Size(50, 16);
            this.noteDisplay.TabIndex = 48;
            this.noteDisplay.Text = "{note}";
            this.noteDisplay.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frequencyTB
            // 
            this.frequencyTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.frequencyTB.Cue = "";
            this.frequencyTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.frequencyTB.ForeColor = System.Drawing.Color.Black;
            this.frequencyTB.Location = new System.Drawing.Point(66, 20);
            this.frequencyTB.MaxLength = 9;
            this.frequencyTB.Name = "frequencyTB";
            this.frequencyTB.Size = new System.Drawing.Size(50, 13);
            this.frequencyTB.TabIndex = 10;
            this.frequencyTB.Text = "{A440}";
            this.frequencyTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.frequencyTB.TextChanged += new System.EventHandler(this.frequencyTB_TextChanged);
            // 
            // centOffsetDisplay
            // 
            this.centOffsetDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.centOffsetDisplay.Cue = "";
            this.centOffsetDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.centOffsetDisplay.ForeColor = System.Drawing.Color.Black;
            this.centOffsetDisplay.Location = new System.Drawing.Point(66, 41);
            this.centOffsetDisplay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.centOffsetDisplay.MaxLength = 6;
            this.centOffsetDisplay.Name = "centOffsetDisplay";
            this.centOffsetDisplay.Size = new System.Drawing.Size(50, 13);
            this.centOffsetDisplay.TabIndex = 11;
            this.centOffsetDisplay.Text = "{cOffset}";
            this.centOffsetDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.centOffsetDisplay.TextChanged += new System.EventHandler(this.frequencyTB_TextChanged);
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
            this.gbGameplayPath.Controls.Add(this.routeMaskNoneRadio);
            this.gbGameplayPath.Controls.Add(this.routeMaskBassRadio);
            this.gbGameplayPath.Controls.Add(this.routeMaskRhythmRadio);
            this.gbGameplayPath.Controls.Add(this.routeMaskLeadRadio);
            this.gbGameplayPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbGameplayPath.Location = new System.Drawing.Point(6, 217);
            this.gbGameplayPath.Name = "gbGameplayPath";
            this.gbGameplayPath.Size = new System.Drawing.Size(435, 44);
            this.gbGameplayPath.TabIndex = 34;
            this.gbGameplayPath.TabStop = false;
            this.gbGameplayPath.Text = "Gameplay Path";
            // 
            // routeMaskNoneRadio
            // 
            this.routeMaskNoneRadio.AutoSize = true;
            this.routeMaskNoneRadio.Checked = true;
            this.routeMaskNoneRadio.ForeColor = System.Drawing.SystemColors.ControlText;
            this.routeMaskNoneRadio.Location = new System.Drawing.Point(322, 19);
            this.routeMaskNoneRadio.Name = "routeMaskNoneRadio";
            this.routeMaskNoneRadio.Size = new System.Drawing.Size(51, 17);
            this.routeMaskNoneRadio.TabIndex = 14;
            this.routeMaskNoneRadio.TabStop = true;
            this.routeMaskNoneRadio.Text = "None";
            this.routeMaskNoneRadio.UseVisualStyleBackColor = true;
            // 
            // routeMaskBassRadio
            // 
            this.routeMaskBassRadio.AutoSize = true;
            this.routeMaskBassRadio.ForeColor = System.Drawing.SystemColors.ControlText;
            this.routeMaskBassRadio.Location = new System.Drawing.Point(243, 19);
            this.routeMaskBassRadio.Name = "routeMaskBassRadio";
            this.routeMaskBassRadio.Size = new System.Drawing.Size(48, 17);
            this.routeMaskBassRadio.TabIndex = 13;
            this.routeMaskBassRadio.Text = "Bass";
            this.routeMaskBassRadio.UseVisualStyleBackColor = true;
            // 
            // routeMaskRhythmRadio
            // 
            this.routeMaskRhythmRadio.AutoSize = true;
            this.routeMaskRhythmRadio.ForeColor = System.Drawing.SystemColors.ControlText;
            this.routeMaskRhythmRadio.Location = new System.Drawing.Point(156, 19);
            this.routeMaskRhythmRadio.Name = "routeMaskRhythmRadio";
            this.routeMaskRhythmRadio.Size = new System.Drawing.Size(61, 17);
            this.routeMaskRhythmRadio.TabIndex = 12;
            this.routeMaskRhythmRadio.Text = "Rhythm";
            this.routeMaskRhythmRadio.UseVisualStyleBackColor = true;
            // 
            // routeMaskLeadRadio
            // 
            this.routeMaskLeadRadio.AutoSize = true;
            this.routeMaskLeadRadio.ForeColor = System.Drawing.SystemColors.ControlText;
            this.routeMaskLeadRadio.Location = new System.Drawing.Point(72, 19);
            this.routeMaskLeadRadio.Name = "routeMaskLeadRadio";
            this.routeMaskLeadRadio.Size = new System.Drawing.Size(49, 17);
            this.routeMaskLeadRadio.TabIndex = 46;
            this.routeMaskLeadRadio.Text = "Lead";
            this.routeMaskLeadRadio.UseVisualStyleBackColor = true;
            // 
            // MetronomeCb
            // 
            this.MetronomeCb.AutoSize = true;
            this.MetronomeCb.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MetronomeCb.Location = new System.Drawing.Point(16, 447);
            this.MetronomeCb.Name = "MetronomeCb";
            this.MetronomeCb.Size = new System.Drawing.Size(209, 17);
            this.MetronomeCb.TabIndex = 7;
            this.MetronomeCb.Text = "Create Metronome Bonus Arrangement";
            this.MetronomeCb.UseVisualStyleBackColor = true;
            // 
            // ArrangementForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(450, 481);
            this.Controls.Add(this.gbGameplayPath);
            this.Controls.Add(this.gbArrInfo);
            this.Controls.Add(this.MetronomeCb);
            this.Controls.Add(this.gbXmlDefinition);
            this.Controls.Add(this.gbDLCId);
            this.Controls.Add(this.gbTone);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.OkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArrangementForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Arrangement";
            this.Load += new System.EventHandler(this.ArrangementForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.scrollSpeedTrackBar)).EndInit();
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
        private System.Windows.Forms.CheckBox MetronomeCb;
        private System.Windows.Forms.GroupBox gbDLCId;
        private CueTextBox MasterId;
        private CueTextBox XmlFilePath;
        private CueTextBox PersistentId;
        private CueTextBox centOffsetDisplay;

        #endregion

        private System.Windows.Forms.Button songXmlBrowseButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.ComboBox arrangementTypeCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox toneBaseCombo;
        private System.Windows.Forms.TrackBar scrollSpeedTrackBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label scrollSpeedDisplay;
        private System.Windows.Forms.CheckBox Picked;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox arrangementNameCombo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox tuningComboBox;
        private System.Windows.Forms.GroupBox gbTone;
        private System.Windows.Forms.Label lblToneA;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox toneDCombo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox toneCCombo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox toneBCombo;
        private System.Windows.Forms.GroupBox gbXmlDefinition;
        private System.Windows.Forms.GroupBox gbArrInfo;
        private System.Windows.Forms.GroupBox gbScrollSpeed;
        private System.Windows.Forms.GroupBox gbGameplayPath;
        private System.Windows.Forms.RadioButton routeMaskLeadRadio;
        private System.Windows.Forms.RadioButton routeMaskBassRadio;
        private System.Windows.Forms.RadioButton routeMaskRhythmRadio;
        private System.Windows.Forms.RadioButton routeMaskNoneRadio;
        private System.Windows.Forms.Label label7;
        private CueTextBox frequencyTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox gbTuningPitch;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label noteDisplay;
        private System.Windows.Forms.CheckBox BonusCheckBox;
        private System.Windows.Forms.CheckBox disableTonesCheckbox;
        private System.Windows.Forms.Button tuningEditButton;
        private System.Windows.Forms.Button typeEdit;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox toneACombo;
        private Label label12;
    }
}