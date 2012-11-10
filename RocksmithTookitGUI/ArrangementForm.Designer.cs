namespace RocksmithToolkitGUI
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
            this.ArrangementName = new System.Windows.Forms.TextBox();
            this.SngFilePath = new System.Windows.Forms.TextBox();
            this.XmlFilePath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.PowerChords = new System.Windows.Forms.CheckBox();
            this.BarChords = new System.Windows.Forms.CheckBox();
            this.DoubleStops = new System.Windows.Forms.CheckBox();
            this.DropDPowerChords = new System.Windows.Forms.CheckBox();
            this.FifithsAndOctaves = new System.Windows.Forms.CheckBox();
            this.FretHandMutes = new System.Windows.Forms.CheckBox();
            this.OpenChords = new System.Windows.Forms.CheckBox();
            this.IsVocal = new System.Windows.Forms.CheckBox();
            this.RelativeDifficulty = new System.Windows.Forms.TextBox();
            this.SlapAndPop = new System.Windows.Forms.CheckBox();
            this.PreBends = new System.Windows.Forms.CheckBox();
            this.Vibrato = new System.Windows.Forms.CheckBox();
            this.SongDifficulty = new System.Windows.Forms.TextBox();
            this.AverageTempo = new System.Windows.Forms.TextBox();
            this.Tuning = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ArrangementName
            // 
            this.ArrangementName.Location = new System.Drawing.Point(17, 16);
            this.ArrangementName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ArrangementName.Name = "ArrangementName";
            this.ArrangementName.Size = new System.Drawing.Size(132, 22);
            this.ArrangementName.TabIndex = 0;
            this.ArrangementName.Text = "Name";
            // 
            // SngFilePath
            // 
            this.SngFilePath.Location = new System.Drawing.Point(17, 82);
            this.SngFilePath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SngFilePath.Name = "SngFilePath";
            this.SngFilePath.Size = new System.Drawing.Size(400, 22);
            this.SngFilePath.TabIndex = 1;
            this.SngFilePath.Text = "SongFile";
            // 
            // XmlFilePath
            // 
            this.XmlFilePath.Location = new System.Drawing.Point(17, 116);
            this.XmlFilePath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.XmlFilePath.Name = "XmlFilePath";
            this.XmlFilePath.Size = new System.Drawing.Size(400, 22);
            this.XmlFilePath.TabIndex = 2;
            this.XmlFilePath.Text = "SongXml";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(427, 82);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 28);
            this.button1.TabIndex = 3;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(428, 111);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 28);
            this.button2.TabIndex = 4;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PowerChords
            // 
            this.PowerChords.AutoSize = true;
            this.PowerChords.Location = new System.Drawing.Point(17, 149);
            this.PowerChords.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PowerChords.Name = "PowerChords";
            this.PowerChords.Size = new System.Drawing.Size(114, 21);
            this.PowerChords.TabIndex = 5;
            this.PowerChords.Text = "PowerChords";
            this.PowerChords.UseVisualStyleBackColor = true;
            // 
            // BarChords
            // 
            this.BarChords.AutoSize = true;
            this.BarChords.Location = new System.Drawing.Point(145, 149);
            this.BarChords.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BarChords.Name = "BarChords";
            this.BarChords.Size = new System.Drawing.Size(97, 21);
            this.BarChords.TabIndex = 6;
            this.BarChords.Text = "BarChords";
            this.BarChords.UseVisualStyleBackColor = true;
            // 
            // DoubleStops
            // 
            this.DoubleStops.AutoSize = true;
            this.DoubleStops.Location = new System.Drawing.Point(255, 149);
            this.DoubleStops.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DoubleStops.Name = "DoubleStops";
            this.DoubleStops.Size = new System.Drawing.Size(111, 21);
            this.DoubleStops.TabIndex = 7;
            this.DoubleStops.Text = "DoubleStops";
            this.DoubleStops.UseVisualStyleBackColor = true;
            // 
            // DropDPowerChords
            // 
            this.DropDPowerChords.AutoSize = true;
            this.DropDPowerChords.Location = new System.Drawing.Point(16, 177);
            this.DropDPowerChords.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DropDPowerChords.Name = "DropDPowerChords";
            this.DropDPowerChords.Size = new System.Drawing.Size(155, 21);
            this.DropDPowerChords.TabIndex = 8;
            this.DropDPowerChords.Text = "DropDPowerChords";
            this.DropDPowerChords.UseVisualStyleBackColor = true;
            // 
            // FifithsAndOctaves
            // 
            this.FifithsAndOctaves.AutoSize = true;
            this.FifithsAndOctaves.Location = new System.Drawing.Point(380, 149);
            this.FifithsAndOctaves.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FifithsAndOctaves.Name = "FifithsAndOctaves";
            this.FifithsAndOctaves.Size = new System.Drawing.Size(144, 21);
            this.FifithsAndOctaves.TabIndex = 9;
            this.FifithsAndOctaves.Text = "FifithsAndOctaves";
            this.FifithsAndOctaves.UseVisualStyleBackColor = true;
            // 
            // FretHandMutes
            // 
            this.FretHandMutes.AutoSize = true;
            this.FretHandMutes.Location = new System.Drawing.Point(185, 177);
            this.FretHandMutes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FretHandMutes.Name = "FretHandMutes";
            this.FretHandMutes.Size = new System.Drawing.Size(127, 21);
            this.FretHandMutes.TabIndex = 10;
            this.FretHandMutes.Text = "FretHandMutes";
            this.FretHandMutes.UseVisualStyleBackColor = true;
            // 
            // OpenChords
            // 
            this.OpenChords.AutoSize = true;
            this.OpenChords.Location = new System.Drawing.Point(327, 178);
            this.OpenChords.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OpenChords.Name = "OpenChords";
            this.OpenChords.Size = new System.Drawing.Size(110, 21);
            this.OpenChords.TabIndex = 11;
            this.OpenChords.Text = "OpenChords";
            this.OpenChords.UseVisualStyleBackColor = true;
            // 
            // IsVocal
            // 
            this.IsVocal.AutoSize = true;
            this.IsVocal.Location = new System.Drawing.Point(159, 18);
            this.IsVocal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.IsVocal.Name = "IsVocal";
            this.IsVocal.Size = new System.Drawing.Size(75, 21);
            this.IsVocal.TabIndex = 12;
            this.IsVocal.Text = "IsVocal";
            this.IsVocal.UseVisualStyleBackColor = true;
            // 
            // RelativeDifficulty
            // 
            this.RelativeDifficulty.Location = new System.Drawing.Point(392, 49);
            this.RelativeDifficulty.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RelativeDifficulty.Name = "RelativeDifficulty";
            this.RelativeDifficulty.Size = new System.Drawing.Size(116, 22);
            this.RelativeDifficulty.TabIndex = 13;
            this.RelativeDifficulty.Text = "RelativeDifficulty";
            // 
            // SlapAndPop
            // 
            this.SlapAndPop.AutoSize = true;
            this.SlapAndPop.Location = new System.Drawing.Point(16, 207);
            this.SlapAndPop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SlapAndPop.Name = "SlapAndPop";
            this.SlapAndPop.Size = new System.Drawing.Size(108, 21);
            this.SlapAndPop.TabIndex = 14;
            this.SlapAndPop.Text = "SlapAndPop";
            this.SlapAndPop.UseVisualStyleBackColor = true;
            // 
            // PreBends
            // 
            this.PreBends.AutoSize = true;
            this.PreBends.Location = new System.Drawing.Point(139, 207);
            this.PreBends.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PreBends.Name = "PreBends";
            this.PreBends.Size = new System.Drawing.Size(92, 21);
            this.PreBends.TabIndex = 15;
            this.PreBends.Text = "PreBends";
            this.PreBends.UseVisualStyleBackColor = true;
            // 
            // Vibrato
            // 
            this.Vibrato.AutoSize = true;
            this.Vibrato.Location = new System.Drawing.Point(237, 207);
            this.Vibrato.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Vibrato.Name = "Vibrato";
            this.Vibrato.Size = new System.Drawing.Size(75, 21);
            this.Vibrato.TabIndex = 16;
            this.Vibrato.Text = "Vibrato";
            this.Vibrato.UseVisualStyleBackColor = true;
            // 
            // SongDifficulty
            // 
            this.SongDifficulty.Location = new System.Drawing.Point(287, 49);
            this.SongDifficulty.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SongDifficulty.Name = "SongDifficulty";
            this.SongDifficulty.Size = new System.Drawing.Size(96, 22);
            this.SongDifficulty.TabIndex = 17;
            this.SongDifficulty.Text = "SongDifficulty";
            // 
            // AverageTempo
            // 
            this.AverageTempo.Location = new System.Drawing.Point(167, 49);
            this.AverageTempo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AverageTempo.Name = "AverageTempo";
            this.AverageTempo.Size = new System.Drawing.Size(111, 22);
            this.AverageTempo.TabIndex = 18;
            this.AverageTempo.Text = "AverageTempo";
            // 
            // Tuning
            // 
            this.Tuning.Location = new System.Drawing.Point(17, 49);
            this.Tuning.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Tuning.Name = "Tuning";
            this.Tuning.Size = new System.Drawing.Size(132, 22);
            this.Tuning.TabIndex = 19;
            this.Tuning.Text = "E Standard";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(217, 235);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 28);
            this.button3.TabIndex = 20;
            this.button3.Text = "Add";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ArrangementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 271);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.Tuning);
            this.Controls.Add(this.AverageTempo);
            this.Controls.Add(this.SongDifficulty);
            this.Controls.Add(this.Vibrato);
            this.Controls.Add(this.PreBends);
            this.Controls.Add(this.SlapAndPop);
            this.Controls.Add(this.RelativeDifficulty);
            this.Controls.Add(this.IsVocal);
            this.Controls.Add(this.OpenChords);
            this.Controls.Add(this.FretHandMutes);
            this.Controls.Add(this.FifithsAndOctaves);
            this.Controls.Add(this.DropDPowerChords);
            this.Controls.Add(this.DoubleStops);
            this.Controls.Add(this.BarChords);
            this.Controls.Add(this.PowerChords);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.XmlFilePath);
            this.Controls.Add(this.SngFilePath);
            this.Controls.Add(this.ArrangementName);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ArrangementForm";
            this.Text = "ArrangementForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ArrangementName;
        private System.Windows.Forms.TextBox SngFilePath;
        private System.Windows.Forms.TextBox XmlFilePath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox PowerChords;
        private System.Windows.Forms.CheckBox BarChords;
        private System.Windows.Forms.CheckBox DoubleStops;
        private System.Windows.Forms.CheckBox DropDPowerChords;
        private System.Windows.Forms.CheckBox FifithsAndOctaves;
        private System.Windows.Forms.CheckBox FretHandMutes;
        private System.Windows.Forms.CheckBox OpenChords;
        private System.Windows.Forms.CheckBox IsVocal;
        private System.Windows.Forms.TextBox RelativeDifficulty;
        private System.Windows.Forms.CheckBox SlapAndPop;
        private System.Windows.Forms.CheckBox PreBends;
        private System.Windows.Forms.CheckBox Vibrato;
        private System.Windows.Forms.TextBox SongDifficulty;
        private System.Windows.Forms.TextBox AverageTempo;
        private System.Windows.Forms.TextBox Tuning;
        private System.Windows.Forms.Button button3;
    }
}