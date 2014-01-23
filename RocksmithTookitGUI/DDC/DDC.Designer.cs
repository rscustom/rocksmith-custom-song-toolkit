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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.DDprogress = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.phaseLenNum = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.deleteArrBT = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cleanCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DDCfilesDgw)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.phaseLenNum)).BeginInit();
            this.groupBox2.SuspendLayout();
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
            this.AddArrBT.Location = new System.Drawing.Point(441, 19);
            this.AddArrBT.Name = "AddArrBT";
            this.AddArrBT.Size = new System.Drawing.Size(64, 24);
            this.AddArrBT.TabIndex = 0;
            this.AddArrBT.Text = "Add";
            this.AddArrBT.UseVisualStyleBackColor = true;
            this.AddArrBT.Click += new System.EventHandler(this.AddArrBT_Click);
            // 
            // ProduceDDbt
            // 
            this.ProduceDDbt.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ProduceDDbt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.ProduceDDbt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.ProduceDDbt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProduceDDbt.Location = new System.Drawing.Point(388, 342);
            this.ProduceDDbt.Name = "ProduceDDbt";
            this.ProduceDDbt.Size = new System.Drawing.Size(134, 32);
            this.ProduceDDbt.TabIndex = 7;
            this.ProduceDDbt.Text = "Generate DD";
            this.ProduceDDbt.UseVisualStyleBackColor = false;
            this.ProduceDDbt.Click += new System.EventHandler(this.ProduceDDbt_Click);
            // 
            // rampUpBT
            // 
            this.rampUpBT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.rampUpBT.Location = new System.Drawing.Point(441, 17);
            this.rampUpBT.Name = "rampUpBT";
            this.rampUpBT.Size = new System.Drawing.Size(64, 23);
            this.rampUpBT.TabIndex = 3;
            this.rampUpBT.Text = "Add";
            this.rampUpBT.UseVisualStyleBackColor = true;
            this.rampUpBT.Click += new System.EventHandler(this.rampUpBT_Click);
            // 
            // delsustainsBT
            // 
            this.delsustainsBT.AutoSize = true;
            this.delsustainsBT.ForeColor = System.Drawing.SystemColors.ControlText;
            this.delsustainsBT.Location = new System.Drawing.Point(402, 44);
            this.delsustainsBT.Name = "delsustainsBT";
            this.delsustainsBT.Size = new System.Drawing.Size(107, 17);
            this.delsustainsBT.TabIndex = 5;
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
            this.DescriptionDDC.TabIndex = 0;
            this.DescriptionDDC.TabStop = true;
            this.DescriptionDDC.Text = resources.GetString("DescriptionDDC.Text");
            this.DescriptionDDC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DescriptionDDC.UseCompatibleTextRendering = true;
            this.DescriptionDDC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DescriptionDDC_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(6, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Phrase length:";
            // 
            // ramUpMdlsCbox
            // 
            this.ramUpMdlsCbox.AllowDrop = true;
            this.ramUpMdlsCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ramUpMdlsCbox.FormattingEnabled = true;
            this.ramUpMdlsCbox.Location = new System.Drawing.Point(6, 18);
            this.ramUpMdlsCbox.MinimumSize = new System.Drawing.Size(20, 0);
            this.ramUpMdlsCbox.Name = "ramUpMdlsCbox";
            this.ramUpMdlsCbox.Size = new System.Drawing.Size(429, 21);
            this.ramUpMdlsCbox.Sorted = true;
            this.ramUpMdlsCbox.TabIndex = 2;
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
            this.DDCfilesDgw.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DDCfilesDgw.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DDCfilesDgw.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.DDCfilesDgw.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DDCfilesDgw.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PathColnm,
            this.TypeColnm});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DDCfilesDgw.DefaultCellStyle = dataGridViewCellStyle3;
            this.DDCfilesDgw.GridColor = System.Drawing.SystemColors.AppWorkspace;
            this.DDCfilesDgw.Location = new System.Drawing.Point(6, 19);
            this.DDCfilesDgw.Name = "DDCfilesDgw";
            this.DDCfilesDgw.RowHeadersWidth = 4;
            this.DDCfilesDgw.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.DDCfilesDgw.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.DDCfilesDgw.RowTemplate.ErrorText = "#####";
            this.DDCfilesDgw.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DDCfilesDgw.ShowEditingIcon = false;
            this.DDCfilesDgw.Size = new System.Drawing.Size(429, 84);
            this.DDCfilesDgw.StandardTab = true;
            this.DDCfilesDgw.TabIndex = 0;
            this.DDCfilesDgw.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DDCfilesDgw_UserDeletingRow);
            // 
            // PathColnm
            // 
            this.PathColnm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PathColnm.HeaderText = "Path";
            this.PathColnm.MinimumWidth = 80;
            this.PathColnm.Name = "PathColnm";
            this.PathColnm.ReadOnly = true;
            // 
            // TypeColnm
            // 
            this.TypeColnm.HeaderText = "Type";
            this.TypeColnm.MinimumWidth = 30;
            this.TypeColnm.Name = "TypeColnm";
            this.TypeColnm.ReadOnly = true;
            this.TypeColnm.Width = 50;
            // 
            // DDprogress
            // 
            this.DDprogress.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.DDprogress.Location = new System.Drawing.Point(10, 343);
            this.DDprogress.MarqueeAnimationSpeed = 80;
            this.DDprogress.Name = "DDprogress";
            this.DDprogress.Size = new System.Drawing.Size(372, 30);
            this.DDprogress.TabIndex = 0;
            this.DDprogress.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cleanCheckbox);
            this.groupBox1.Controls.Add(this.ramUpMdlsCbox);
            this.groupBox1.Controls.Add(this.rampUpBT);
            this.groupBox1.Controls.Add(this.phaseLenNum);
            this.groupBox1.Controls.Add(this.delsustainsBT);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(10, 260);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 76);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // phaseLenNum
            // 
            this.phaseLenNum.Location = new System.Drawing.Point(84, 43);
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
            this.phaseLenNum.TabIndex = 4;
            this.phaseLenNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.phaseLenNum.ThousandsSeparator = true;
            this.phaseLenNum.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // deleteArrBT
            // 
            this.deleteArrBT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.deleteArrBT.Location = new System.Drawing.Point(441, 48);
            this.deleteArrBT.Name = "deleteArrBT";
            this.deleteArrBT.Size = new System.Drawing.Size(64, 24);
            this.deleteArrBT.TabIndex = 1;
            this.deleteArrBT.Text = "Delete";
            this.deleteArrBT.UseVisualStyleBackColor = true;
            this.deleteArrBT.Click += new System.EventHandler(this.deleteArrBT_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AddArrBT);
            this.groupBox2.Controls.Add(this.deleteArrBT);
            this.groupBox2.Controls.Add(this.DDCfilesDgw);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(10, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(511, 109);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Package or Arrangement XML file";
            // 
            // cleanCheckbox
            // 
            this.cleanCheckbox.AutoSize = true;
            this.cleanCheckbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cleanCheckbox.Location = new System.Drawing.Point(220, 45);
            this.cleanCheckbox.Name = "cleanCheckbox";
            this.cleanCheckbox.Size = new System.Drawing.Size(94, 17);
            this.cleanCheckbox.TabIndex = 6;
            this.cleanCheckbox.Text = "Clean Process";
            this.cleanCheckbox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Location = new System.Drawing.Point(155, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(224, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Overwrite original file and doesn\'t generate log";
            // 
            // DDC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.DDprogress);
            this.Controls.Add(this.DescriptionDDC);
            this.Controls.Add(this.ProduceDDbt);
            this.Controls.Add(this.pictureBox1);
            this.MinimumSize = new System.Drawing.Size(530, 380);
            this.Name = "DDC";
            this.Size = new System.Drawing.Size(530, 380);
            this.Load += new System.EventHandler(this.DDC_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DDCfilesDgw)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.phaseLenNum)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.ProgressBar DDprogress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button deleteArrBT;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn PathColnm;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColnm;
        private System.Windows.Forms.CheckBox cleanCheckbox;
        private System.Windows.Forms.Label label2;
    }
}
