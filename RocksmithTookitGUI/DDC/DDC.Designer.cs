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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DDC));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.AddArrBT = new System.Windows.Forms.Button();
            this.ProduceDDbt = new System.Windows.Forms.Button();
            this.rampUpBT = new System.Windows.Forms.Button();
            this.delsustainsBT = new System.Windows.Forms.CheckBox();
            this.process1 = new System.Diagnostics.Process();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.DescriptionDDC = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.ramUpMdlsCbox = new System.Windows.Forms.ComboBox();
            this.DDCfilesDgw = new System.Windows.Forms.DataGridView();
            this.PathColnm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColnm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestPathCbx = new System.Windows.Forms.CheckBox();
            this.DDprogress = new System.Windows.Forms.ProgressBar();
            this.phaseLenNum = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DDCfilesDgw)).BeginInit();
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
            this.AddArrBT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.AddArrBT.Location = new System.Drawing.Point(457, 152);
            this.AddArrBT.Name = "AddArrBT";
            this.AddArrBT.Size = new System.Drawing.Size(64, 24);
            this.AddArrBT.TabIndex = 3;
            this.AddArrBT.Text = "Add..";
            this.AddArrBT.UseVisualStyleBackColor = true;
            this.AddArrBT.Click += new System.EventHandler(this.AddArrBT_Click);
            // 
            // ProduceDDbt
            // 
            this.ProduceDDbt.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ProduceDDbt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.ProduceDDbt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
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
            this.rampUpBT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.rampUpBT.Location = new System.Drawing.Point(457, 260);
            this.rampUpBT.Name = "rampUpBT";
            this.rampUpBT.Size = new System.Drawing.Size(64, 22);
            this.rampUpBT.TabIndex = 6;
            this.rampUpBT.Text = "Add...";
            this.rampUpBT.UseVisualStyleBackColor = true;
            this.rampUpBT.Click += new System.EventHandler(this.rampUpBT_Click);
            // 
            // delsustainsBT
            // 
            this.delsustainsBT.AutoSize = true;
            this.delsustainsBT.Location = new System.Drawing.Point(17, 297);
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
            this.DescriptionDDC.AutoEllipsis = true;
            this.DescriptionDDC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DescriptionDDC.LinkArea = new System.Windows.Forms.LinkArea(99, 32);
            this.DescriptionDDC.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DescriptionDDC.Location = new System.Drawing.Point(145, 10);
            this.DescriptionDDC.Name = "DescriptionDDC";
            this.DescriptionDDC.Size = new System.Drawing.Size(369, 128);
            this.DescriptionDDC.TabIndex = 10;
            this.DescriptionDDC.TabStop = true;
            this.DescriptionDDC.Text = resources.GetString("DescriptionDDC.Text");
            this.DescriptionDDC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DescriptionDDC.UseCompatibleTextRendering = true;
            this.DescriptionDDC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DescriptionDDC_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(142, 298);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Phrase length :";
            // 
            // ramUpMdlsCbox
            // 
            this.ramUpMdlsCbox.AllowDrop = true;
            this.ramUpMdlsCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ramUpMdlsCbox.FormattingEnabled = true;
            this.ramUpMdlsCbox.Location = new System.Drawing.Point(10, 260);
            this.ramUpMdlsCbox.MinimumSize = new System.Drawing.Size(20, 0);
            this.ramUpMdlsCbox.Name = "ramUpMdlsCbox";
            this.ramUpMdlsCbox.Size = new System.Drawing.Size(441, 21);
            this.ramUpMdlsCbox.Sorted = true;
            this.ramUpMdlsCbox.TabIndex = 36;
            this.ramUpMdlsCbox.DropDown += new System.EventHandler(this.ramUpMdlsCbox_DropDown);
            // 
            // DDCfilesDgw
            // 
            this.DDCfilesDgw.AllowUserToAddRows = false;
            this.DDCfilesDgw.AllowUserToResizeColumns = false;
            this.DDCfilesDgw.AllowUserToResizeRows = false;
            this.DDCfilesDgw.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DDCfilesDgw.BackgroundColor = System.Drawing.SystemColors.Window;
            this.DDCfilesDgw.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.DDCfilesDgw.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DDCfilesDgw.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PathColnm,
            this.TypeColnm});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DDCfilesDgw.DefaultCellStyle = dataGridViewCellStyle1;
            this.DDCfilesDgw.GridColor = System.Drawing.SystemColors.AppWorkspace;
            this.DDCfilesDgw.Location = new System.Drawing.Point(10, 152);
            this.DDCfilesDgw.MinimumSize = new System.Drawing.Size(441, 102);
            this.DDCfilesDgw.Name = "DDCfilesDgw";
            this.DDCfilesDgw.RowHeadersWidth = 4;
            this.DDCfilesDgw.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.DDCfilesDgw.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.DDCfilesDgw.RowTemplate.ErrorText = "#####";
            this.DDCfilesDgw.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DDCfilesDgw.ShowEditingIcon = false;
            this.DDCfilesDgw.Size = new System.Drawing.Size(441, 102);
            this.DDCfilesDgw.StandardTab = true;
            this.DDCfilesDgw.TabIndex = 37;
            this.DDCfilesDgw.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DDCfilesDgw_UserDeletingRow);
            // 
            // PathColnm
            // 
            this.PathColnm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PathColnm.HeaderText = "Path";
            this.PathColnm.Name = "PathColnm";
            this.PathColnm.ReadOnly = true;
            // 
            // TypeColnm
            // 
            this.TypeColnm.HeaderText = "Type";
            this.TypeColnm.MinimumWidth = 50;
            this.TypeColnm.Name = "TypeColnm";
            this.TypeColnm.ReadOnly = true;
            this.TypeColnm.Width = 80;
            // 
            // DestPathCbx
            // 
            this.DestPathCbx.AutoSize = true;
            this.DestPathCbx.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DestPathCbx.Location = new System.Drawing.Point(457, 182);
            this.DestPathCbx.Name = "DestPathCbx";
            this.DestPathCbx.Size = new System.Drawing.Size(76, 31);
            this.DestPathCbx.TabIndex = 38;
            this.DestPathCbx.Text = "Change \r\nout folder";
            this.DestPathCbx.UseVisualStyleBackColor = true;
            // 
            // DDprogress
            // 
            this.DDprogress.Location = new System.Drawing.Point(10, 336);
            this.DDprogress.MarqueeAnimationSpeed = 80;
            this.DDprogress.MinimumSize = new System.Drawing.Size(364, 32);
            this.DDprogress.Name = "DDprogress";
            this.DDprogress.Size = new System.Drawing.Size(364, 32);
            this.DDprogress.TabIndex = 39;
            this.DDprogress.Visible = false;
            // 
            // phaseLenNum
            // 
            this.phaseLenNum.Location = new System.Drawing.Point(219, 296);
            this.phaseLenNum.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.phaseLenNum.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.phaseLenNum.Name = "phaseLenNum";
            this.phaseLenNum.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.phaseLenNum.Size = new System.Drawing.Size(52, 20);
            this.phaseLenNum.TabIndex = 17;
            this.phaseLenNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.phaseLenNum.ThousandsSeparator = true;
            this.phaseLenNum.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // DDC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DDprogress);
            this.Controls.Add(this.DestPathCbx);
            this.Controls.Add(this.DDCfilesDgw);
            this.Controls.Add(this.ramUpMdlsCbox);
            this.Controls.Add(this.phaseLenNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DescriptionDDC);
            this.Controls.Add(this.delsustainsBT);
            this.Controls.Add(this.rampUpBT);
            this.Controls.Add(this.ProduceDDbt);
            this.Controls.Add(this.AddArrBT);
            this.Controls.Add(this.pictureBox1);
            this.MinimumSize = new System.Drawing.Size(530, 380);
            this.Name = "DDC";
            this.Size = new System.Drawing.Size(530, 380);
            this.Load += new System.EventHandler(this.DDC_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DDCfilesDgw)).EndInit();
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
        private System.Diagnostics.Process process1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel DescriptionDDC;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DLCPackageCreator.NumericUpDownFixed phaseLenNum;
        private System.Windows.Forms.ComboBox ramUpMdlsCbox;
        private System.Windows.Forms.DataGridView DDCfilesDgw;
        private System.Windows.Forms.CheckBox DestPathCbx;
        private System.Windows.Forms.ProgressBar DDprogress;
        private System.Windows.Forms.DataGridViewTextBoxColumn PathColnm;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColnm;
    }
}
