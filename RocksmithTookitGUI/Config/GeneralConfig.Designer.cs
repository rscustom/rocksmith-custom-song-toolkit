namespace RocksmithToolkitGUI.Config
{
    partial class GeneralConfig
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
            this.general_usebeta = new System.Windows.Forms.CheckBox();
            this.gbCreator = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.creator_scrollspeed = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.creator_gameversion = new System.Windows.Forms.ComboBox();
            this.creator_structured = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.general_defaultappid_RS2012 = new System.Windows.Forms.ComboBox();
            this.gbGeneral = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.general_defaultappid_RS2014 = new System.Windows.Forms.ComboBox();
            this.gbDDC = new System.Windows.Forms.GroupBox();
            this.ddc_phraselength = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.ddc_removesustain = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ddc_rampup = new System.Windows.Forms.ComboBox();
            this.gbConverter = new System.Windows.Forms.GroupBox();
            this.converter_target = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.converter_source = new System.Windows.Forms.ComboBox();
            this.creator_useacronyms = new System.Windows.Forms.CheckBox();
            this.gbCreator.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.creator_scrollspeed)).BeginInit();
            this.gbGeneral.SuspendLayout();
            this.gbDDC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ddc_phraselength)).BeginInit();
            this.gbConverter.SuspendLayout();
            this.SuspendLayout();
            // 
            // general_usebeta
            // 
            this.general_usebeta.AutoSize = true;
            this.general_usebeta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.general_usebeta.Location = new System.Drawing.Point(377, 18);
            this.general_usebeta.Name = "general_usebeta";
            this.general_usebeta.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.general_usebeta.Size = new System.Drawing.Size(130, 17);
            this.general_usebeta.TabIndex = 2;
            this.general_usebeta.Text = "Auto-update with beta";
            this.general_usebeta.UseVisualStyleBackColor = true;
            this.general_usebeta.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbCreator
            // 
            this.gbCreator.Controls.Add(this.groupBox1);
            this.gbCreator.Controls.Add(this.label3);
            this.gbCreator.Controls.Add(this.creator_gameversion);
            this.gbCreator.Controls.Add(this.creator_structured);
            this.gbCreator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbCreator.Location = new System.Drawing.Point(2, 78);
            this.gbCreator.Margin = new System.Windows.Forms.Padding(2);
            this.gbCreator.Name = "gbCreator";
            this.gbCreator.Padding = new System.Windows.Forms.Padding(2);
            this.gbCreator.Size = new System.Drawing.Size(516, 92);
            this.gbCreator.TabIndex = 0;
            this.gbCreator.TabStop = false;
            this.gbCreator.Text = "DLC Creator";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.creator_scrollspeed);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.creator_useacronyms);
            this.groupBox1.Location = new System.Drawing.Point(5, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 42);
            this.groupBox1.TabIndex = 45;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Arrangement";
            // 
            // creator_scrollspeed
            // 
            this.creator_scrollspeed.DecimalPlaces = 1;
            this.creator_scrollspeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.creator_scrollspeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.creator_scrollspeed.Location = new System.Drawing.Point(117, 15);
            this.creator_scrollspeed.Maximum = new decimal(new int[] {
            45,
            0,
            0,
            65536});
            this.creator_scrollspeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.creator_scrollspeed.Name = "creator_scrollspeed";
            this.creator_scrollspeed.Size = new System.Drawing.Size(48, 20);
            this.creator_scrollspeed.TabIndex = 5;
            this.creator_scrollspeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.creator_scrollspeed.Value = new decimal(new int[] {
            45,
            0,
            0,
            65536});
            this.creator_scrollspeed.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Default Scroll Speed:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(11, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Game Version:";
            // 
            // creator_gameversion
            // 
            this.creator_gameversion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.creator_gameversion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.creator_gameversion.Location = new System.Drawing.Point(122, 17);
            this.creator_gameversion.Margin = new System.Windows.Forms.Padding(2);
            this.creator_gameversion.Name = "creator_gameversion";
            this.creator_gameversion.Size = new System.Drawing.Size(120, 21);
            this.creator_gameversion.TabIndex = 3;
            this.creator_gameversion.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // creator_structured
            // 
            this.creator_structured.AutoSize = true;
            this.creator_structured.ForeColor = System.Drawing.SystemColors.ControlText;
            this.creator_structured.Location = new System.Drawing.Point(349, 19);
            this.creator_structured.Name = "creator_structured";
            this.creator_structured.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.creator_structured.Size = new System.Drawing.Size(158, 17);
            this.creator_structured.TabIndex = 4;
            this.creator_structured.Text = "Import Structured Packages";
            this.creator_structured.UseVisualStyleBackColor = true;
            this.creator_structured.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(11, 19);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "App ID (RS1):";
            // 
            // general_defaultappid_RS2012
            // 
            this.general_defaultappid_RS2012.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.general_defaultappid_RS2012.ForeColor = System.Drawing.SystemColors.ControlText;
            this.general_defaultappid_RS2012.Location = new System.Drawing.Point(122, 16);
            this.general_defaultappid_RS2012.Margin = new System.Windows.Forms.Padding(2);
            this.general_defaultappid_RS2012.Name = "general_defaultappid_RS2012";
            this.general_defaultappid_RS2012.Size = new System.Drawing.Size(250, 21);
            this.general_defaultappid_RS2012.TabIndex = 0;
            this.general_defaultappid_RS2012.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbGeneral
            // 
            this.gbGeneral.Controls.Add(this.label4);
            this.gbGeneral.Controls.Add(this.general_defaultappid_RS2014);
            this.gbGeneral.Controls.Add(this.general_usebeta);
            this.gbGeneral.Controls.Add(this.label2);
            this.gbGeneral.Controls.Add(this.general_defaultappid_RS2012);
            this.gbGeneral.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbGeneral.Location = new System.Drawing.Point(2, 2);
            this.gbGeneral.Margin = new System.Windows.Forms.Padding(2);
            this.gbGeneral.Name = "gbGeneral";
            this.gbGeneral.Padding = new System.Windows.Forms.Padding(2);
            this.gbGeneral.Size = new System.Drawing.Size(516, 72);
            this.gbGeneral.TabIndex = 14;
            this.gbGeneral.TabStop = false;
            this.gbGeneral.Text = "General";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(11, 44);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "App ID (RS2014):";
            // 
            // general_defaultappid_RS2014
            // 
            this.general_defaultappid_RS2014.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.general_defaultappid_RS2014.ForeColor = System.Drawing.SystemColors.ControlText;
            this.general_defaultappid_RS2014.Location = new System.Drawing.Point(122, 41);
            this.general_defaultappid_RS2014.Margin = new System.Windows.Forms.Padding(2);
            this.general_defaultappid_RS2014.Name = "general_defaultappid_RS2014";
            this.general_defaultappid_RS2014.Size = new System.Drawing.Size(250, 21);
            this.general_defaultappid_RS2014.TabIndex = 1;
            this.general_defaultappid_RS2014.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbDDC
            // 
            this.gbDDC.Controls.Add(this.ddc_phraselength);
            this.gbDDC.Controls.Add(this.ddc_removesustain);
            this.gbDDC.Controls.Add(this.label6);
            this.gbDDC.Controls.Add(this.label5);
            this.gbDDC.Controls.Add(this.ddc_rampup);
            this.gbDDC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbDDC.Location = new System.Drawing.Point(2, 175);
            this.gbDDC.Name = "gbDDC";
            this.gbDDC.Size = new System.Drawing.Size(516, 73);
            this.gbDDC.TabIndex = 15;
            this.gbDDC.TabStop = false;
            this.gbDDC.Text = "DDC";
            // 
            // ddc_phraselength
            // 
            this.ddc_phraselength.Location = new System.Drawing.Point(122, 46);
            this.ddc_phraselength.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.ddc_phraselength.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.ddc_phraselength.Name = "ddc_phraselength";
            this.ddc_phraselength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ddc_phraselength.Size = new System.Drawing.Size(52, 20);
            this.ddc_phraselength.TabIndex = 8;
            this.ddc_phraselength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ddc_phraselength.ThousandsSeparator = true;
            this.ddc_phraselength.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.ddc_phraselength.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // ddc_removesustain
            // 
            this.ddc_removesustain.AutoSize = true;
            this.ddc_removesustain.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ddc_removesustain.Location = new System.Drawing.Point(400, 21);
            this.ddc_removesustain.Name = "ddc_removesustain";
            this.ddc_removesustain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ddc_removesustain.Size = new System.Drawing.Size(107, 17);
            this.ddc_removesustain.TabIndex = 7;
            this.ddc_removesustain.Text = "Remove sustains";
            this.ddc_removesustain.UseVisualStyleBackColor = true;
            this.ddc_removesustain.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(11, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "Phrase length:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(11, 22);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Rampage:";
            // 
            // ddc_rampup
            // 
            this.ddc_rampup.AllowDrop = true;
            this.ddc_rampup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddc_rampup.FormattingEnabled = true;
            this.ddc_rampup.Location = new System.Drawing.Point(122, 19);
            this.ddc_rampup.MinimumSize = new System.Drawing.Size(20, 0);
            this.ddc_rampup.Name = "ddc_rampup";
            this.ddc_rampup.Size = new System.Drawing.Size(171, 21);
            this.ddc_rampup.Sorted = true;
            this.ddc_rampup.TabIndex = 6;
            this.ddc_rampup.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbConverter
            // 
            this.gbConverter.Controls.Add(this.converter_target);
            this.gbConverter.Controls.Add(this.label7);
            this.gbConverter.Controls.Add(this.label8);
            this.gbConverter.Controls.Add(this.converter_source);
            this.gbConverter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbConverter.Location = new System.Drawing.Point(2, 254);
            this.gbConverter.Name = "gbConverter";
            this.gbConverter.Size = new System.Drawing.Size(516, 73);
            this.gbConverter.TabIndex = 50;
            this.gbConverter.TabStop = false;
            this.gbConverter.Text = "Converter";
            // 
            // converter_target
            // 
            this.converter_target.AllowDrop = true;
            this.converter_target.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.converter_target.FormattingEnabled = true;
            this.converter_target.Location = new System.Drawing.Point(122, 45);
            this.converter_target.MinimumSize = new System.Drawing.Size(20, 0);
            this.converter_target.Name = "converter_target";
            this.converter_target.Size = new System.Drawing.Size(120, 21);
            this.converter_target.Sorted = true;
            this.converter_target.TabIndex = 10;
            this.converter_target.SelectedIndexChanged += new System.EventHandler(this.ConfigurationChanged);
            this.converter_target.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(11, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "Target Platform:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(11, 22);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Source Platform:";
            // 
            // converter_source
            // 
            this.converter_source.AllowDrop = true;
            this.converter_source.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.converter_source.FormattingEnabled = true;
            this.converter_source.Location = new System.Drawing.Point(122, 19);
            this.converter_source.MinimumSize = new System.Drawing.Size(20, 0);
            this.converter_source.Name = "converter_source";
            this.converter_source.Size = new System.Drawing.Size(120, 21);
            this.converter_source.Sorted = true;
            this.converter_source.TabIndex = 9;
            this.converter_source.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // isAcronymUsed
            // 
            this.creator_useacronyms.AutoSize = true;
            this.creator_useacronyms.ForeColor = System.Drawing.SystemColors.ControlText;
            this.creator_useacronyms.Location = new System.Drawing.Point(286, 16);
            this.creator_useacronyms.Name = "creator_useacronyms";
            this.creator_useacronyms.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.creator_useacronyms.Size = new System.Drawing.Size(216, 17);
            this.creator_useacronyms.TabIndex = 4;
            this.creator_useacronyms.Text = "Use Acronym effect for long artist names";
            this.creator_useacronyms.UseVisualStyleBackColor = true;
            this.creator_useacronyms.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // GeneralConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.gbConverter);
            this.Controls.Add(this.gbDDC);
            this.Controls.Add(this.gbGeneral);
            this.Controls.Add(this.gbCreator);
            this.Name = "GeneralConfig";
            this.Size = new System.Drawing.Size(521, 419);
            this.gbCreator.ResumeLayout(false);
            this.gbCreator.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.creator_scrollspeed)).EndInit();
            this.gbGeneral.ResumeLayout(false);
            this.gbGeneral.PerformLayout();
            this.gbDDC.ResumeLayout(false);
            this.gbDDC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ddc_phraselength)).EndInit();
            this.gbConverter.ResumeLayout(false);
            this.gbConverter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox general_usebeta;
        private System.Windows.Forms.GroupBox gbCreator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox general_defaultappid_RS2012;
        private System.Windows.Forms.CheckBox creator_structured;
        private System.Windows.Forms.GroupBox gbGeneral;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox creator_gameversion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox general_defaultappid_RS2014;
        private DLCPackageCreator.NumericUpDownFixed creator_scrollspeed;
        private System.Windows.Forms.GroupBox gbDDC;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ddc_rampup;
        private DLCPackageCreator.NumericUpDownFixed ddc_phraselength;
        private System.Windows.Forms.CheckBox ddc_removesustain;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox gbConverter;
        private System.Windows.Forms.ComboBox converter_target;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox converter_source;
        private System.Windows.Forms.CheckBox creator_useacronyms;
    }
}
