namespace RocksmithTookitGUI.DLCPackageCreator
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
            this.songFileBrowseButton = new System.Windows.Forms.Button();
            this.songXmlBrowseButton = new System.Windows.Forms.Button();
            this.PowerChords = new System.Windows.Forms.CheckBox();
            this.BarChords = new System.Windows.Forms.CheckBox();
            this.DoubleStops = new System.Windows.Forms.CheckBox();
            this.DropDPowerChords = new System.Windows.Forms.CheckBox();
            this.FifthsAndOctaves = new System.Windows.Forms.CheckBox();
            this.FretHandMutes = new System.Windows.Forms.CheckBox();
            this.OpenChords = new System.Windows.Forms.CheckBox();
            this.SlapAndPop = new System.Windows.Forms.CheckBox();
            this.PreBends = new System.Windows.Forms.CheckBox();
            this.Vibrato = new System.Windows.Forms.CheckBox();
            this.addArrangementButton = new System.Windows.Forms.Button();
            this.RelativeDifficulty = new RocksmithTookitGUI.CueTextBox();
            this.Tuning = new RocksmithTookitGUI.CueTextBox();
            this.XmlFilePath = new RocksmithTookitGUI.CueTextBox();
            this.SngFilePath = new RocksmithTookitGUI.CueTextBox();
            this.ArrangementName = new RocksmithTookitGUI.CueTextBox();
            this.arrangementTypeCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tonesCombo = new System.Windows.Forms.ComboBox();
            this.scrollSpeedTrackBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.scrollSpeedTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // songFileBrowseButton
            // 
            this.songFileBrowseButton.Location = new System.Drawing.Point(427, 82);
            this.songFileBrowseButton.Margin = new System.Windows.Forms.Padding(4);
            this.songFileBrowseButton.Name = "songFileBrowseButton";
            this.songFileBrowseButton.Size = new System.Drawing.Size(83, 28);
            this.songFileBrowseButton.TabIndex = 8;
            this.songFileBrowseButton.Text = "Browse";
            this.songFileBrowseButton.UseVisualStyleBackColor = true;
            this.songFileBrowseButton.Click += new System.EventHandler(this.songFileBrowseButton_Click);
            // 
            // songXmlBrowseButton
            // 
            this.songXmlBrowseButton.Location = new System.Drawing.Point(428, 111);
            this.songXmlBrowseButton.Margin = new System.Windows.Forms.Padding(4);
            this.songXmlBrowseButton.Name = "songXmlBrowseButton";
            this.songXmlBrowseButton.Size = new System.Drawing.Size(81, 28);
            this.songXmlBrowseButton.TabIndex = 12;
            this.songXmlBrowseButton.Text = "Browse";
            this.songXmlBrowseButton.UseVisualStyleBackColor = true;
            this.songXmlBrowseButton.Click += new System.EventHandler(this.songXmlBrowseButton_Click);
            // 
            // PowerChords
            // 
            this.PowerChords.AutoSize = true;
            this.PowerChords.Location = new System.Drawing.Point(17, 149);
            this.PowerChords.Margin = new System.Windows.Forms.Padding(4);
            this.PowerChords.Name = "PowerChords";
            this.PowerChords.Size = new System.Drawing.Size(114, 21);
            this.PowerChords.TabIndex = 13;
            this.PowerChords.Text = "PowerChords";
            this.PowerChords.UseVisualStyleBackColor = true;
            // 
            // BarChords
            // 
            this.BarChords.AutoSize = true;
            this.BarChords.Location = new System.Drawing.Point(144, 149);
            this.BarChords.Margin = new System.Windows.Forms.Padding(4);
            this.BarChords.Name = "BarChords";
            this.BarChords.Size = new System.Drawing.Size(97, 21);
            this.BarChords.TabIndex = 14;
            this.BarChords.Text = "BarChords";
            this.BarChords.UseVisualStyleBackColor = true;
            // 
            // DoubleStops
            // 
            this.DoubleStops.AutoSize = true;
            this.DoubleStops.Location = new System.Drawing.Point(252, 149);
            this.DoubleStops.Margin = new System.Windows.Forms.Padding(4);
            this.DoubleStops.Name = "DoubleStops";
            this.DoubleStops.Size = new System.Drawing.Size(111, 21);
            this.DoubleStops.TabIndex = 15;
            this.DoubleStops.Text = "DoubleStops";
            this.DoubleStops.UseVisualStyleBackColor = true;
            // 
            // DropDPowerChords
            // 
            this.DropDPowerChords.AutoSize = true;
            this.DropDPowerChords.Location = new System.Drawing.Point(16, 177);
            this.DropDPowerChords.Margin = new System.Windows.Forms.Padding(4);
            this.DropDPowerChords.Name = "DropDPowerChords";
            this.DropDPowerChords.Size = new System.Drawing.Size(155, 21);
            this.DropDPowerChords.TabIndex = 17;
            this.DropDPowerChords.Text = "DropDPowerChords";
            this.DropDPowerChords.UseVisualStyleBackColor = true;
            // 
            // FifthsAndOctaves
            // 
            this.FifthsAndOctaves.AutoSize = true;
            this.FifthsAndOctaves.Location = new System.Drawing.Point(376, 149);
            this.FifthsAndOctaves.Margin = new System.Windows.Forms.Padding(4);
            this.FifthsAndOctaves.Name = "FifthsAndOctaves";
            this.FifthsAndOctaves.Size = new System.Drawing.Size(144, 21);
            this.FifthsAndOctaves.TabIndex = 16;
            this.FifthsAndOctaves.Text = "FifithsAndOctaves";
            this.FifthsAndOctaves.UseVisualStyleBackColor = true;
            // 
            // FretHandMutes
            // 
            this.FretHandMutes.AutoSize = true;
            this.FretHandMutes.Location = new System.Drawing.Point(209, 177);
            this.FretHandMutes.Margin = new System.Windows.Forms.Padding(4);
            this.FretHandMutes.Name = "FretHandMutes";
            this.FretHandMutes.Size = new System.Drawing.Size(127, 21);
            this.FretHandMutes.TabIndex = 18;
            this.FretHandMutes.Text = "FretHandMutes";
            this.FretHandMutes.UseVisualStyleBackColor = true;
            // 
            // OpenChords
            // 
            this.OpenChords.AutoSize = true;
            this.OpenChords.Location = new System.Drawing.Point(376, 177);
            this.OpenChords.Margin = new System.Windows.Forms.Padding(4);
            this.OpenChords.Name = "OpenChords";
            this.OpenChords.Size = new System.Drawing.Size(110, 21);
            this.OpenChords.TabIndex = 19;
            this.OpenChords.Text = "OpenChords";
            this.OpenChords.UseVisualStyleBackColor = true;
            // 
            // SlapAndPop
            // 
            this.SlapAndPop.AutoSize = true;
            this.SlapAndPop.Location = new System.Drawing.Point(16, 206);
            this.SlapAndPop.Margin = new System.Windows.Forms.Padding(4);
            this.SlapAndPop.Name = "SlapAndPop";
            this.SlapAndPop.Size = new System.Drawing.Size(108, 21);
            this.SlapAndPop.TabIndex = 20;
            this.SlapAndPop.Text = "SlapAndPop";
            this.SlapAndPop.UseVisualStyleBackColor = true;
            // 
            // PreBends
            // 
            this.PreBends.AutoSize = true;
            this.PreBends.Location = new System.Drawing.Point(209, 206);
            this.PreBends.Margin = new System.Windows.Forms.Padding(4);
            this.PreBends.Name = "PreBends";
            this.PreBends.Size = new System.Drawing.Size(92, 21);
            this.PreBends.TabIndex = 21;
            this.PreBends.Text = "PreBends";
            this.PreBends.UseVisualStyleBackColor = true;
            // 
            // Vibrato
            // 
            this.Vibrato.AutoSize = true;
            this.Vibrato.Location = new System.Drawing.Point(376, 206);
            this.Vibrato.Margin = new System.Windows.Forms.Padding(4);
            this.Vibrato.Name = "Vibrato";
            this.Vibrato.Size = new System.Drawing.Size(75, 21);
            this.Vibrato.TabIndex = 22;
            this.Vibrato.Text = "Vibrato";
            this.Vibrato.UseVisualStyleBackColor = true;
            // 
            // addArrangementButton
            // 
            this.addArrangementButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addArrangementButton.Location = new System.Drawing.Point(312, 274);
            this.addArrangementButton.Margin = new System.Windows.Forms.Padding(4);
            this.addArrangementButton.Name = "addArrangementButton";
            this.addArrangementButton.Size = new System.Drawing.Size(96, 36);
            this.addArrangementButton.TabIndex = 23;
            this.addArrangementButton.Text = "Ok";
            this.addArrangementButton.UseVisualStyleBackColor = true;
            this.addArrangementButton.Click += new System.EventHandler(this.addArrangementButton_Click);
            // 
            // RelativeDifficulty
            // 
            this.RelativeDifficulty.Cue = "Relative Difficulty";
            this.RelativeDifficulty.Location = new System.Drawing.Point(157, 49);
            this.RelativeDifficulty.Margin = new System.Windows.Forms.Padding(4);
            this.RelativeDifficulty.Name = "RelativeDifficulty";
            this.RelativeDifficulty.Size = new System.Drawing.Size(116, 22);
            this.RelativeDifficulty.TabIndex = 4;
            // 
            // Tuning
            // 
            this.Tuning.Cue = "Tuning";
            this.Tuning.Location = new System.Drawing.Point(17, 49);
            this.Tuning.Margin = new System.Windows.Forms.Padding(4);
            this.Tuning.Name = "Tuning";
            this.Tuning.Size = new System.Drawing.Size(132, 22);
            this.Tuning.TabIndex = 3;
            this.Tuning.Text = "E Standard";
            // 
            // XmlFilePath
            // 
            this.XmlFilePath.Cue = ".xml File";
            this.XmlFilePath.Location = new System.Drawing.Point(17, 116);
            this.XmlFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.XmlFilePath.Name = "XmlFilePath";
            this.XmlFilePath.Size = new System.Drawing.Size(400, 22);
            this.XmlFilePath.TabIndex = 11;
            // 
            // SngFilePath
            // 
            this.SngFilePath.Cue = ".sng File";
            this.SngFilePath.Location = new System.Drawing.Point(17, 82);
            this.SngFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.SngFilePath.Name = "SngFilePath";
            this.SngFilePath.Size = new System.Drawing.Size(400, 22);
            this.SngFilePath.TabIndex = 5;
            // 
            // ArrangementName
            // 
            this.ArrangementName.Cue = "Name";
            this.ArrangementName.Location = new System.Drawing.Point(17, 16);
            this.ArrangementName.Margin = new System.Windows.Forms.Padding(4);
            this.ArrangementName.Name = "ArrangementName";
            this.ArrangementName.Size = new System.Drawing.Size(132, 22);
            this.ArrangementName.TabIndex = 1;
            // 
            // arrangementTypeCombo
            // 
            this.arrangementTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arrangementTypeCombo.FormattingEnabled = true;
            this.arrangementTypeCombo.Location = new System.Drawing.Point(293, 14);
            this.arrangementTypeCombo.Name = "arrangementTypeCombo";
            this.arrangementTypeCombo.Size = new System.Drawing.Size(216, 24);
            this.arrangementTypeCombo.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(157, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 17);
            this.label1.TabIndex = 25;
            this.label1.Text = "Arrangement Type:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(416, 274);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(96, 36);
            this.cancelButton.TabIndex = 26;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(280, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 28;
            this.label2.Text = "Tone:";
            // 
            // tonesCombo
            // 
            this.tonesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tonesCombo.FormattingEnabled = true;
            this.tonesCombo.Location = new System.Drawing.Point(331, 46);
            this.tonesCombo.Name = "tonesCombo";
            this.tonesCombo.Size = new System.Drawing.Size(178, 24);
            this.tonesCombo.TabIndex = 27;
            // 
            // scrollSpeedTrackBar
            // 
            this.scrollSpeedTrackBar.AutoSize = false;
            this.scrollSpeedTrackBar.Location = new System.Drawing.Point(76, 234);
            this.scrollSpeedTrackBar.Maximum = 45;
            this.scrollSpeedTrackBar.Minimum = 10;
            this.scrollSpeedTrackBar.Name = "scrollSpeedTrackBar";
            this.scrollSpeedTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.scrollSpeedTrackBar.Size = new System.Drawing.Size(174, 28);
            this.scrollSpeedTrackBar.TabIndex = 29;
            this.scrollSpeedTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.scrollSpeedTrackBar.Value = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 234);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 30;
            this.label3.Text = "Slowest";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(256, 234);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 17);
            this.label5.TabIndex = 32;
            this.label5.Text = "Fastest";
            // 
            // ArrangementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 323);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.scrollSpeedTrackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tonesCombo);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.arrangementTypeCombo);
            this.Controls.Add(this.RelativeDifficulty);
            this.Controls.Add(this.addArrangementButton);
            this.Controls.Add(this.Tuning);
            this.Controls.Add(this.Vibrato);
            this.Controls.Add(this.PreBends);
            this.Controls.Add(this.SlapAndPop);
            this.Controls.Add(this.OpenChords);
            this.Controls.Add(this.FretHandMutes);
            this.Controls.Add(this.FifthsAndOctaves);
            this.Controls.Add(this.DropDPowerChords);
            this.Controls.Add(this.DoubleStops);
            this.Controls.Add(this.BarChords);
            this.Controls.Add(this.PowerChords);
            this.Controls.Add(this.songXmlBrowseButton);
            this.Controls.Add(this.songFileBrowseButton);
            this.Controls.Add(this.XmlFilePath);
            this.Controls.Add(this.SngFilePath);
            this.Controls.Add(this.ArrangementName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArrangementForm";
            this.Text = "Add Arrangement";
            ((System.ComponentModel.ISupportInitialize)(this.scrollSpeedTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CueTextBox ArrangementName;
        private CueTextBox SngFilePath;
        private CueTextBox XmlFilePath;
        private System.Windows.Forms.Button songFileBrowseButton;
        private System.Windows.Forms.Button songXmlBrowseButton;
        private System.Windows.Forms.CheckBox PowerChords;
        private System.Windows.Forms.CheckBox BarChords;
        private System.Windows.Forms.CheckBox DoubleStops;
        private System.Windows.Forms.CheckBox DropDPowerChords;
        private System.Windows.Forms.CheckBox FifthsAndOctaves;
        private System.Windows.Forms.CheckBox FretHandMutes;
        private System.Windows.Forms.CheckBox OpenChords;
        private System.Windows.Forms.CheckBox SlapAndPop;
        private System.Windows.Forms.CheckBox PreBends;
        private System.Windows.Forms.CheckBox Vibrato;
        private CueTextBox Tuning;
        private System.Windows.Forms.Button addArrangementButton;
        private CueTextBox RelativeDifficulty;
        private System.Windows.Forms.ComboBox arrangementTypeCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tonesCombo;
        private System.Windows.Forms.TrackBar scrollSpeedTrackBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
    }
}