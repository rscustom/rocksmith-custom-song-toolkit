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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.AddArrBT = new System.Windows.Forms.Button();
            this.rampUpBT = new System.Windows.Forms.Button();
            this.chkRemoveSustains = new System.Windows.Forms.CheckBox();
            this.process1 = new System.Diagnostics.Process();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.DescriptionDDC = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.ramUpMdlsCbox = new System.Windows.Forms.ComboBox();
            this.DDCfilesDgw = new System.Windows.Forms.DataGridView();
            this.PathColnm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColnm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkGenArrIds = new System.Windows.Forms.CheckBox();
            this.chkGenLogFile = new System.Windows.Forms.CheckBox();
            this.chkOverwrite = new System.Windows.Forms.CheckBox();
            this.ConfigFilesCbx = new System.Windows.Forms.ComboBox();
            this.ConfigFilesBtn = new System.Windows.Forms.Button();
            this.phaseLenNum = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.deleteArrBT = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ddcVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ProduceDDbt = new System.Windows.Forms.Button();
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.pbUpdateProgress = new System.Windows.Forms.ProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
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
            this.pictureBox1.Location = new System.Drawing.Point(104, 10);
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
            // chkRemoveSustains
            // 
            this.chkRemoveSustains.AutoSize = true;
            this.chkRemoveSustains.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.chkRemoveSustains.Location = new System.Drawing.Point(168, 82);
            this.chkRemoveSustains.Name = "chkRemoveSustains";
            this.chkRemoveSustains.Size = new System.Drawing.Size(109, 17);
            this.chkRemoveSustains.TabIndex = 7;
            this.chkRemoveSustains.Text = "Remove Sustains";
            this.chkRemoveSustains.UseVisualStyleBackColor = true;
            this.chkRemoveSustains.CheckStateChanged += new System.EventHandler(this.Highlight_CheckStateChanged);
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
            this.DescriptionDDC.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DescriptionDDC.Location = new System.Drawing.Point(260, 84);
            this.DescriptionDDC.Name = "DescriptionDDC";
            this.DescriptionDDC.Size = new System.Drawing.Size(182, 18);
            this.DescriptionDDC.TabIndex = 0;
            this.DescriptionDDC.TabStop = true;
            this.DescriptionDDC.Text = "http://ddcreator.wordpress.com";
            this.DescriptionDDC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DescriptionDDC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DescriptionDDC_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(13, 84);
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
            this.ramUpMdlsCbox.Location = new System.Drawing.Point(94, 18);
            this.ramUpMdlsCbox.MinimumSize = new System.Drawing.Size(20, 0);
            this.ramUpMdlsCbox.Name = "ramUpMdlsCbox";
            this.ramUpMdlsCbox.Size = new System.Drawing.Size(341, 21);
            this.ramUpMdlsCbox.Sorted = true;
            this.ramUpMdlsCbox.TabIndex = 2;
            this.ramUpMdlsCbox.DropDown += new System.EventHandler(this.ramUpMdlsCbox_DropDown);
            this.ramUpMdlsCbox.SelectedIndexChanged += new System.EventHandler(this.ramUpMdlsCbox_SelectedIndexChanged);
            // 
            // DDCfilesDgw
            // 
            this.DDCfilesDgw.AllowDrop = true;
            this.DDCfilesDgw.AllowUserToAddRows = false;
            this.DDCfilesDgw.AllowUserToResizeColumns = false;
            this.DDCfilesDgw.AllowUserToResizeRows = false;
            this.DDCfilesDgw.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DDCfilesDgw.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.DDCfilesDgw.BackgroundColor = System.Drawing.SystemColors.Window;
            this.DDCfilesDgw.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DDCfilesDgw.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DDCfilesDgw.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.DDCfilesDgw.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DDCfilesDgw.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PathColnm,
            this.TypeColnm});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DDCfilesDgw.DefaultCellStyle = dataGridViewCellStyle1;
            this.DDCfilesDgw.GridColor = System.Drawing.SystemColors.AppWorkspace;
            this.DDCfilesDgw.Location = new System.Drawing.Point(6, 19);
            this.DDCfilesDgw.Name = "DDCfilesDgw";
            this.DDCfilesDgw.RowHeadersWidth = 4;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.DDCfilesDgw.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.DDCfilesDgw.RowTemplate.ErrorText = "#####";
            this.DDCfilesDgw.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DDCfilesDgw.ShowEditingIcon = false;
            this.DDCfilesDgw.Size = new System.Drawing.Size(429, 84);
            this.DDCfilesDgw.StandardTab = true;
            this.DDCfilesDgw.TabIndex = 0;
            this.DDCfilesDgw.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DDCfilesDgw_UserDeletingRow);
            this.DDCfilesDgw.DragDrop += new System.Windows.Forms.DragEventHandler(this.DDCfilesDgw_DragDrop);
            this.DDCfilesDgw.DragEnter += new System.Windows.Forms.DragEventHandler(this.DDCfilesDgw_DragEnter);
            // 
            // PathColnm
            // 
            this.PathColnm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.PathColnm.HeaderText = "Path";
            this.PathColnm.MinimumWidth = 340;
            this.PathColnm.Name = "PathColnm";
            this.PathColnm.ReadOnly = true;
            this.PathColnm.Width = 340;
            // 
            // TypeColnm
            // 
            this.TypeColnm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TypeColnm.HeaderText = "Type";
            this.TypeColnm.MinimumWidth = 40;
            this.TypeColnm.Name = "TypeColnm";
            this.TypeColnm.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkGenArrIds);
            this.groupBox1.Controls.Add(this.chkGenLogFile);
            this.groupBox1.Controls.Add(this.chkOverwrite);
            this.groupBox1.Controls.Add(this.ConfigFilesCbx);
            this.groupBox1.Controls.Add(this.ramUpMdlsCbox);
            this.groupBox1.Controls.Add(this.ConfigFilesBtn);
            this.groupBox1.Controls.Add(this.rampUpBT);
            this.groupBox1.Controls.Add(this.phaseLenNum);
            this.groupBox1.Controls.Add(this.chkRemoveSustains);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(10, 260);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 130);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // chkGenArrIds
            // 
            this.chkGenArrIds.AutoSize = true;
            this.chkGenArrIds.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.chkGenArrIds.Location = new System.Drawing.Point(291, 105);
            this.chkGenArrIds.Name = "chkGenArrIds";
            this.chkGenArrIds.Size = new System.Drawing.Size(196, 17);
            this.chkGenArrIds.TabIndex = 10;
            this.chkGenArrIds.Text = "Generate Arrangement Identification";
            this.toolTip.SetToolTip(this.chkGenArrIds, "Notice: \r\nGenerating Arrangment Identification\r\nwill reset song stats and helps e" +
                    "lminate\r\nsome in game errors.");
            this.chkGenArrIds.UseVisualStyleBackColor = true;
            this.chkGenArrIds.CheckStateChanged += new System.EventHandler(this.Highlight_CheckStateChanged);
            // 
            // chkGenLogFile
            // 
            this.chkGenLogFile.AutoSize = true;
            this.chkGenLogFile.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.chkGenLogFile.Location = new System.Drawing.Point(168, 105);
            this.chkGenLogFile.Name = "chkGenLogFile";
            this.chkGenLogFile.Size = new System.Drawing.Size(91, 17);
            this.chkGenLogFile.TabIndex = 9;
            this.chkGenLogFile.Text = "Generate Log";
            this.chkGenLogFile.UseVisualStyleBackColor = true;
            this.chkGenLogFile.CheckStateChanged += new System.EventHandler(this.Highlight_CheckStateChanged);
            // 
            // chkOverwrite
            // 
            this.chkOverwrite.AutoSize = true;
            this.chkOverwrite.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.chkOverwrite.Location = new System.Drawing.Point(291, 82);
            this.chkOverwrite.Name = "chkOverwrite";
            this.chkOverwrite.Size = new System.Drawing.Size(128, 17);
            this.chkOverwrite.TabIndex = 8;
            this.chkOverwrite.Text = "Overwrite Original File";
            this.chkOverwrite.UseVisualStyleBackColor = true;
            this.chkOverwrite.CheckStateChanged += new System.EventHandler(this.Highlight_CheckStateChanged);
            // 
            // ConfigFilesCbx
            // 
            this.ConfigFilesCbx.AllowDrop = true;
            this.ConfigFilesCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConfigFilesCbx.FormattingEnabled = true;
            this.ConfigFilesCbx.Location = new System.Drawing.Point(94, 47);
            this.ConfigFilesCbx.MinimumSize = new System.Drawing.Size(20, 0);
            this.ConfigFilesCbx.Name = "ConfigFilesCbx";
            this.ConfigFilesCbx.Size = new System.Drawing.Size(341, 21);
            this.ConfigFilesCbx.Sorted = true;
            this.ConfigFilesCbx.TabIndex = 4;
            this.ConfigFilesCbx.DropDown += new System.EventHandler(this.ConfigFilesCbx_DropDown);
            // 
            // ConfigFilesBtn
            // 
            this.ConfigFilesBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.ConfigFilesBtn.Location = new System.Drawing.Point(441, 46);
            this.ConfigFilesBtn.Name = "ConfigFilesBtn";
            this.ConfigFilesBtn.Size = new System.Drawing.Size(64, 23);
            this.ConfigFilesBtn.TabIndex = 5;
            this.ConfigFilesBtn.Text = "Add";
            this.ConfigFilesBtn.UseVisualStyleBackColor = true;
            this.ConfigFilesBtn.Click += new System.EventHandler(this.ConfigFilesBtn_Click);
            // 
            // phaseLenNum
            // 
            this.phaseLenNum.Location = new System.Drawing.Point(94, 81);
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
            this.phaseLenNum.TabIndex = 6;
            this.phaseLenNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.phaseLenNum.ThousandsSeparator = true;
            this.toolTip.SetToolTip(this.phaseLenNum, "Number of Bars");
            this.phaseLenNum.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(32, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Config file:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(4, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Ramp-up model:";
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
            // ddcVersion
            // 
            this.ddcVersion.BackColor = System.Drawing.Color.Transparent;
            this.ddcVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ddcVersion.ForeColor = System.Drawing.Color.Crimson;
            this.ddcVersion.Location = new System.Drawing.Point(202, 126);
            this.ddcVersion.Name = "ddcVersion";
            this.ddcVersion.Size = new System.Drawing.Size(40, 16);
            this.ddcVersion.TabIndex = 10;
            this.ddcVersion.Text = "v0.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(259, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Dynamic Difficulty Creator";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(260, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Help can be found on:";
            // 
            // ProduceDDbt
            // 
            this.ProduceDDbt.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ProduceDDbt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProduceDDbt.Location = new System.Drawing.Point(417, 424);
            this.ProduceDDbt.Name = "ProduceDDbt";
            this.ProduceDDbt.Size = new System.Drawing.Size(97, 29);
            this.ProduceDDbt.TabIndex = 37;
            this.ProduceDDbt.Text = "Generate DD";
            this.ProduceDDbt.UseVisualStyleBackColor = false;
            this.ProduceDDbt.Click += new System.EventHandler(this.ProduceDDbt_Click);
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblCurrentOperation.Location = new System.Drawing.Point(23, 401);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCurrentOperation.Size = new System.Drawing.Size(149, 17);
            this.lblCurrentOperation.TabIndex = 1000;
            this.lblCurrentOperation.Text = "...";
            this.lblCurrentOperation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCurrentOperation.Visible = false;
            // 
            // pbUpdateProgress
            // 
            this.pbUpdateProgress.Location = new System.Drawing.Point(178, 401);
            this.pbUpdateProgress.Name = "pbUpdateProgress";
            this.pbUpdateProgress.Size = new System.Drawing.Size(336, 17);
            this.pbUpdateProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbUpdateProgress.TabIndex = 1001;
            this.pbUpdateProgress.Visible = false;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 9000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // DDC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lblCurrentOperation);
            this.Controls.Add(this.pbUpdateProgress);
            this.Controls.Add(this.ProduceDDbt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ddcVersion);
            this.Controls.Add(this.DescriptionDDC);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.MinimumSize = new System.Drawing.Size(530, 380);
            this.Name = "DDC";
            this.Size = new System.Drawing.Size(530, 470);
            this.Load += new System.EventHandler(this.DDC_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DDCfilesDgw)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.phaseLenNum)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button AddArrBT;
        private System.Windows.Forms.Button rampUpBT;
        private System.Windows.Forms.CheckBox chkRemoveSustains;
        private System.Diagnostics.Process process1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel DescriptionDDC;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DLCPackageCreator.NumericUpDownFixed phaseLenNum;
        private System.Windows.Forms.ComboBox ramUpMdlsCbox;
        private System.Windows.Forms.DataGridView DDCfilesDgw;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button deleteArrBT;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn PathColnm;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColnm;
        private System.Windows.Forms.CheckBox chkOverwrite;
        private System.Windows.Forms.Label ddcVersion;
        private System.Windows.Forms.CheckBox chkGenLogFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ConfigFilesCbx;
        private System.Windows.Forms.Button ConfigFilesBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ProduceDDbt;
        private System.Windows.Forms.Label lblCurrentOperation;
        private System.Windows.Forms.ProgressBar pbUpdateProgress;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkGenArrIds;
    }
}
