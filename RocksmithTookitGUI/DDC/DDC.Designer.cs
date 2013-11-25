namespace RocksmithToolkitGUI.DDC
{
    partial class DDC
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.AddArrBT = new System.Windows.Forms.Button();
            this.ProduceDDbt = new System.Windows.Forms.Button();
            this.rampUpBT = new System.Windows.Forms.Button();
            this.delsustainsBT = new System.Windows.Forms.CheckBox();
            this.process1 = new System.Diagnostics.Process();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.DescriptionDDC = new System.Windows.Forms.LinkLabel();
            this.remChordsCB = new System.Windows.Forms.CheckBox();
            this.NDDcbx = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GeneratRampUPcstp = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.GenRMPPitm = new System.Windows.Forms.ToolStripMenuItem();
            this.phaseLenNum = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.RampUPcbbx = new RocksmithToolkitGUI.CueTextBox();
            this.ArrFilePathTB = new RocksmithToolkitGUI.CueTextBox();
            this.basePH = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.GeneratRampUPcstp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.phaseLenNum)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RocksmithToolkitGUI.Properties.Resources.ddc_512;
            this.pictureBox1.Location = new System.Drawing.Point(10, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // AddArrBT
            // 
            this.AddArrBT.Location = new System.Drawing.Point(450, 160);
            this.AddArrBT.Name = "AddArrBT";
            this.AddArrBT.Size = new System.Drawing.Size(64, 24);
            this.AddArrBT.TabIndex = 3;
            this.AddArrBT.Text = "Select...";
            this.AddArrBT.UseVisualStyleBackColor = true;
            this.AddArrBT.Click += new System.EventHandler(this.AddArrBT_Click);
            // 
            // ProduceDDbt
            // 
            this.ProduceDDbt.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ProduceDDbt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.ProduceDDbt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.ProduceDDbt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ProduceDDbt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProduceDDbt.Location = new System.Drawing.Point(380, 336);
            this.ProduceDDbt.Name = "ProduceDDbt";
            this.ProduceDDbt.Size = new System.Drawing.Size(134, 32);
            this.ProduceDDbt.TabIndex = 5;
            this.ProduceDDbt.Text = "Generate DD";
            this.ProduceDDbt.UseVisualStyleBackColor = false;
            this.ProduceDDbt.Click += new System.EventHandler(this.ProduceDDbt_Click);
            // 
            // rampUpBT
            // 
            this.rampUpBT.Location = new System.Drawing.Point(450, 192);
            this.rampUpBT.Name = "rampUpBT";
            this.rampUpBT.Size = new System.Drawing.Size(64, 20);
            this.rampUpBT.TabIndex = 6;
            this.rampUpBT.Text = "Select...";
            this.rampUpBT.UseVisualStyleBackColor = true;
            this.rampUpBT.Click += new System.EventHandler(this.rampUpBT_Click);
            // 
            // delsustainsBT
            // 
            this.delsustainsBT.AutoSize = true;
            this.delsustainsBT.Location = new System.Drawing.Point(10, 227);
            this.delsustainsBT.Name = "delsustainsBT";
            this.delsustainsBT.Size = new System.Drawing.Size(107, 17);
            this.delsustainsBT.TabIndex = 7;
            this.delsustainsBT.Text = "Remove sustains";
            this.delsustainsBT.UseVisualStyleBackColor = true;
            // 
            // process1
            // 
            this.process1.StartInfo.Domain = "";
            this.process1.StartInfo.LoadUserProfile = false;
            this.process1.StartInfo.Password = null;
            this.process1.StartInfo.StandardErrorEncoding = null;
            this.process1.StartInfo.StandardOutputEncoding = null;
            this.process1.StartInfo.UserName = "";
            this.process1.SynchronizingObject = this;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DescriptionDDC
            // 
            this.DescriptionDDC.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.DescriptionDDC.ActiveLinkColor = System.Drawing.Color.RosyBrown;
            this.DescriptionDDC.AllowDrop = true;
            this.DescriptionDDC.AutoEllipsis = true;
            this.DescriptionDDC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DescriptionDDC.LinkArea = new System.Windows.Forms.LinkArea(14, 15);
            this.DescriptionDDC.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DescriptionDDC.Location = new System.Drawing.Point(145, 10);
            this.DescriptionDDC.Name = "DescriptionDDC";
            this.DescriptionDDC.Size = new System.Drawing.Size(369, 128);
            this.DescriptionDDC.TabIndex = 10;
            this.DescriptionDDC.TabStop = true;
            this.DescriptionDDC.Text = "Text, just text";
            this.DescriptionDDC.UseCompatibleTextRendering = true;
            this.DescriptionDDC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DescriptionDDC_LinkClicked);
            // 
            // remChordsCB
            // 
            this.remChordsCB.AutoSize = true;
            this.remChordsCB.Location = new System.Drawing.Point(124, 227);
            this.remChordsCB.Name = "remChordsCB";
            this.remChordsCB.Size = new System.Drawing.Size(101, 17);
            this.remChordsCB.TabIndex = 11;
            this.remChordsCB.Text = "Remove chords";
            this.remChordsCB.UseVisualStyleBackColor = true;
            this.remChordsCB.CheckedChanged += new System.EventHandler(this.remChordsCB_CheckedChanged);
            // 
            // NDDcbx
            // 
            this.NDDcbx.AutoSize = true;
            this.NDDcbx.Enabled = false;
            this.NDDcbx.ForeColor = System.Drawing.Color.SlateGray;
            this.NDDcbx.Location = new System.Drawing.Point(10, 250);
            this.NDDcbx.Name = "NDDcbx";
            this.NDDcbx.Size = new System.Drawing.Size(80, 17);
            this.NDDcbx.TabIndex = 12;
            this.NDDcbx.Text = "Make NDD";
            this.NDDcbx.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(276, 228);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Phase lenght :";
            // 
            // GeneratRampUPcstp
            // 
            this.GeneratRampUPcstp.Enabled = false;
            this.GeneratRampUPcstp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GenRMPPitm});
            this.GeneratRampUPcstp.Name = "GeneratRampUPcstp";
            this.GeneratRampUPcstp.Size = new System.Drawing.Size(176, 26);
            this.GeneratRampUPcstp.Text = "Not supported yet";
            // 
            // GenRMPPitm
            // 
            this.GenRMPPitm.Enabled = false;
            this.GenRMPPitm.Name = "GenRMPPitm";
            this.GenRMPPitm.Size = new System.Drawing.Size(175, 22);
            this.GenRMPPitm.Text = "Generate Ramp-Up";
            this.GenRMPPitm.ToolTipText = "Not suppurted yet";
            // 
            // phaseLenNum
            // 
            this.phaseLenNum.Location = new System.Drawing.Point(357, 226);
            this.phaseLenNum.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.phaseLenNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.phaseLenNum.Name = "phaseLenNum";
            this.phaseLenNum.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.phaseLenNum.Size = new System.Drawing.Size(56, 20);
            this.phaseLenNum.TabIndex = 17;
            this.phaseLenNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.phaseLenNum.ThousandsSeparator = true;
            this.phaseLenNum.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // RampUPcbbx
            // 
            this.RampUPcbbx.ContextMenuStrip = this.GeneratRampUPcstp;
            this.RampUPcbbx.Cue = "Ramp-Up Model (.xml)";
            this.RampUPcbbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.RampUPcbbx.ForeColor = System.Drawing.Color.Gray;
            this.RampUPcbbx.Location = new System.Drawing.Point(10, 192);
            this.RampUPcbbx.Name = "RampUPcbbx";
            this.RampUPcbbx.Size = new System.Drawing.Size(434, 20);
            this.RampUPcbbx.TabIndex = 8;
            // 
            // ArrFilePathTB
            // 
            this.ArrFilePathTB.Cue = "Arrangement.xml (.xml)";
            this.ArrFilePathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArrFilePathTB.ForeColor = System.Drawing.Color.Gray;
            this.ArrFilePathTB.Location = new System.Drawing.Point(10, 160);
            this.ArrFilePathTB.Name = "ArrFilePathTB";
            this.ArrFilePathTB.Size = new System.Drawing.Size(434, 20);
            this.ArrFilePathTB.TabIndex = 8;
            // 
            // basePH
            // 
            this.basePH.AutoSize = true;
            this.basePH.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.basePH.Location = new System.Drawing.Point(417, 226);
            this.basePH.Name = "basePH";
            this.basePH.Size = new System.Drawing.Size(20, 17);
            this.basePH.TabIndex = 18;
            this.basePH.Text = "/4";
            // 
            // DDC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.basePH);
            this.Controls.Add(this.phaseLenNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NDDcbx);
            this.Controls.Add(this.remChordsCB);
            this.Controls.Add(this.DescriptionDDC);
            this.Controls.Add(this.RampUPcbbx);
            this.Controls.Add(this.ArrFilePathTB);
            this.Controls.Add(this.delsustainsBT);
            this.Controls.Add(this.rampUpBT);
            this.Controls.Add(this.ProduceDDbt);
            this.Controls.Add(this.AddArrBT);
            this.Controls.Add(this.pictureBox1);
            this.MinimumSize = new System.Drawing.Size(530, 380);
            this.Name = "DDC";
            this.Size = new System.Drawing.Size(530, 380);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.GeneratRampUPcstp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.phaseLenNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button AddArrBT;
        private System.Windows.Forms.Button ProduceDDbt;
        private System.Windows.Forms.Button rampUpBT;
        private System.Windows.Forms.CheckBox delsustainsBT;
        private CueTextBox ArrFilePathTB;
        private System.Diagnostics.Process process1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox NDDcbx;
        private System.Windows.Forms.CheckBox remChordsCB;
        private System.Windows.Forms.LinkLabel DescriptionDDC;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DLCPackageCreator.NumericUpDownFixed phaseLenNum;
        private CueTextBox RampUPcbbx;
        private System.Windows.Forms.ContextMenuStrip GeneratRampUPcstp;
        private System.Windows.Forms.ToolStripMenuItem GenRMPPitm;
        private System.Windows.Forms.Label basePH;
    }
}
