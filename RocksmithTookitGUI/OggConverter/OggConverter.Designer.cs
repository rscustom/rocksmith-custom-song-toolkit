namespace RocksmithToolkitGUI.OggConverter
{
    partial class OggConverter
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
            this.txtOgg2FixHdr = new System.Windows.Forms.TextBox();
            this.btnOgg2FixHdr = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnWwise2Ogg = new System.Windows.Forms.Button();
            this.txtWwise2Ogg = new System.Windows.Forms.TextBox();
            this.txtWwiseConvert = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnWwiseConvert = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblChorusTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnAudio2Wem = new System.Windows.Forms.Button();
            this.txtAudio2Wem = new System.Windows.Forms.TextBox();
            this.tbarChorusTime = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.audioQualityBox = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbarChorusTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioQualityBox)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOgg2FixHdr
            // 
            this.txtOgg2FixHdr.BackColor = System.Drawing.SystemColors.Window;
            this.txtOgg2FixHdr.Location = new System.Drawing.Point(9, 18);
            this.txtOgg2FixHdr.Margin = new System.Windows.Forms.Padding(2);
            this.txtOgg2FixHdr.Multiline = true;
            this.txtOgg2FixHdr.Name = "txtOgg2FixHdr";
            this.txtOgg2FixHdr.ReadOnly = true;
            this.txtOgg2FixHdr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOgg2FixHdr.Size = new System.Drawing.Size(399, 20);
            this.txtOgg2FixHdr.TabIndex = 0;
            // 
            // btnOgg2FixHdr
            // 
            this.btnOgg2FixHdr.Location = new System.Drawing.Point(412, 18);
            this.btnOgg2FixHdr.Margin = new System.Windows.Forms.Padding(2);
            this.btnOgg2FixHdr.Name = "btnOgg2FixHdr";
            this.btnOgg2FixHdr.Size = new System.Drawing.Size(56, 20);
            this.btnOgg2FixHdr.TabIndex = 1;
            this.btnOgg2FixHdr.Text = "Browse";
            this.btnOgg2FixHdr.UseVisualStyleBackColor = true;
            this.btnOgg2FixHdr.Click += new System.EventHandler(this.btnOgg2FixHdr_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnOgg2FixHdr);
            this.groupBox1.Controls.Add(this.txtOgg2FixHdr);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(477, 45);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input OGG (Wwise 2010.3.3) file or directory to fix header:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnWwise2Ogg);
            this.groupBox2.Controls.Add(this.txtWwise2Ogg);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(5, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(477, 47);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input OGG (Wwise 2010) or WEM (Wwise 2013) file or directory to convert to Vobis " +
                "OGG:";
            // 
            // btnWwise2Ogg
            // 
            this.btnWwise2Ogg.Location = new System.Drawing.Point(412, 18);
            this.btnWwise2Ogg.Margin = new System.Windows.Forms.Padding(2);
            this.btnWwise2Ogg.Name = "btnWwise2Ogg";
            this.btnWwise2Ogg.Size = new System.Drawing.Size(56, 20);
            this.btnWwise2Ogg.TabIndex = 2;
            this.btnWwise2Ogg.Text = "Browse";
            this.btnWwise2Ogg.UseVisualStyleBackColor = true;
            this.btnWwise2Ogg.Click += new System.EventHandler(this.btnWwise2Ogg_Click);
            // 
            // txtWwise2Ogg
            // 
            this.txtWwise2Ogg.BackColor = System.Drawing.SystemColors.Window;
            this.txtWwise2Ogg.Location = new System.Drawing.Point(9, 18);
            this.txtWwise2Ogg.Margin = new System.Windows.Forms.Padding(2);
            this.txtWwise2Ogg.Multiline = true;
            this.txtWwise2Ogg.Name = "txtWwise2Ogg";
            this.txtWwise2Ogg.ReadOnly = true;
            this.txtWwise2Ogg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWwise2Ogg.Size = new System.Drawing.Size(399, 20);
            this.txtWwise2Ogg.TabIndex = 2;
            // 
            // txtWwiseConvert
            // 
            this.txtWwiseConvert.BackColor = System.Drawing.SystemColors.Window;
            this.txtWwiseConvert.Location = new System.Drawing.Point(9, 19);
            this.txtWwiseConvert.Multiline = true;
            this.txtWwiseConvert.Name = "txtWwiseConvert";
            this.txtWwiseConvert.ReadOnly = true;
            this.txtWwiseConvert.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWwiseConvert.Size = new System.Drawing.Size(399, 20);
            this.txtWwiseConvert.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnWwiseConvert);
            this.groupBox3.Controls.Add(this.txtWwiseConvert);
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox3.Location = new System.Drawing.Point(5, 145);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(477, 48);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input OGG (Wwise 2010) or WEM (Wwise 2013) file to convert from PC <-> Console:";
            // 
            // btnWwiseConvert
            // 
            this.btnWwiseConvert.Location = new System.Drawing.Point(412, 19);
            this.btnWwiseConvert.Name = "btnWwiseConvert";
            this.btnWwiseConvert.Size = new System.Drawing.Size(56, 20);
            this.btnWwiseConvert.TabIndex = 3;
            this.btnWwiseConvert.Text = "Browse";
            this.btnWwiseConvert.UseVisualStyleBackColor = true;
            this.btnWwiseConvert.Click += new System.EventHandler(this.btnWwiseConvert_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.lblChorusTime);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.audioQualityBox);
            this.groupBox4.Controls.Add(this.btnAudio2Wem);
            this.groupBox4.Controls.Add(this.txtAudio2Wem);
            this.groupBox4.Controls.Add(this.tbarChorusTime);
            this.groupBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox4.Location = new System.Drawing.Point(5, 218);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(477, 70);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Input Vobis OGG or WAV file to convert to WEM (Wwise 2013):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(136, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Chorus Start Time (seconds)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label1, "Set start time for preview audio");
            // 
            // lblChorusTime
            // 
            this.lblChorusTime.AutoSize = true;
            this.lblChorusTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChorusTime.Location = new System.Drawing.Point(394, 46);
            this.lblChorusTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblChorusTime.Name = "lblChorusTime";
            this.lblChorusTime.Size = new System.Drawing.Size(22, 13);
            this.lblChorusTime.TabIndex = 36;
            this.lblChorusTime.Text = "4.0";
            this.lblChorusTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Location = new System.Drawing.Point(6, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 40;
            this.label7.Text = "Audio Quality";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAudio2Wem
            // 
            this.btnAudio2Wem.Location = new System.Drawing.Point(412, 19);
            this.btnAudio2Wem.Name = "btnAudio2Wem";
            this.btnAudio2Wem.Size = new System.Drawing.Size(56, 20);
            this.btnAudio2Wem.TabIndex = 3;
            this.btnAudio2Wem.Text = "Browse";
            this.toolTip1.SetToolTip(this.btnAudio2Wem, "Select an Ogg or Wav file to convert to\r\nWwise 2013 Wem audio and _preview.wem fi" +
                    "les");
            this.btnAudio2Wem.UseVisualStyleBackColor = true;
            this.btnAudio2Wem.Click += new System.EventHandler(this.btnOgg2Wem_Click);
            // 
            // txtAudio2Wem
            // 
            this.txtAudio2Wem.BackColor = System.Drawing.SystemColors.Window;
            this.txtAudio2Wem.Location = new System.Drawing.Point(9, 19);
            this.txtAudio2Wem.Multiline = true;
            this.txtAudio2Wem.Name = "txtAudio2Wem";
            this.txtAudio2Wem.ReadOnly = true;
            this.txtAudio2Wem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAudio2Wem.Size = new System.Drawing.Size(399, 20);
            this.txtAudio2Wem.TabIndex = 4;
            // 
            // tbarChorusTime
            // 
            this.tbarChorusTime.AutoSize = false;
            this.tbarChorusTime.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbarChorusTime.Location = new System.Drawing.Point(281, 44);
            this.tbarChorusTime.Margin = new System.Windows.Forms.Padding(2);
            this.tbarChorusTime.Maximum = 600;
            this.tbarChorusTime.Name = "tbarChorusTime";
            this.tbarChorusTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbarChorusTime.Size = new System.Drawing.Size(109, 20);
            this.tbarChorusTime.SmallChange = 5;
            this.tbarChorusTime.TabIndex = 10;
            this.tbarChorusTime.TickStyle = System.Windows.Forms.TickStyle.None;
            this.toolTip1.SetToolTip(this.tbarChorusTime, "Left mouse click on slider line \r\nfor 0.5 second increments");
            this.tbarChorusTime.Value = 40;
            this.tbarChorusTime.ValueChanged += new System.EventHandler(this.tbarChorusTime_ValueChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 7000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 100;
            // 
            // audioQualityBox
            // 
            this.audioQualityBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.audioQualityBox.Location = new System.Drawing.Point(81, 44);
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
            this.audioQualityBox.TabIndex = 39;
            this.toolTip1.SetToolTip(this.audioQualityBox, "High Quality 6 ... Default Quality 4\r\nLeave audio quality set to Default 4\r\nif so" +
                    "urce audio quality is unknown");
            this.audioQualityBox.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // OggConverter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "OggConverter";
            this.Size = new System.Drawing.Size(496, 345);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbarChorusTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioQualityBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtOgg2FixHdr;
        private System.Windows.Forms.Button btnOgg2FixHdr;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnWwise2Ogg;
        private System.Windows.Forms.TextBox txtWwise2Ogg;
        private System.Windows.Forms.TextBox txtWwiseConvert;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnWwiseConvert;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnAudio2Wem;
        private System.Windows.Forms.TextBox txtAudio2Wem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private DLCPackageCreator.NumericUpDownFixed audioQualityBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblChorusTime;
        private System.Windows.Forms.TrackBar tbarChorusTime;
    }
}
