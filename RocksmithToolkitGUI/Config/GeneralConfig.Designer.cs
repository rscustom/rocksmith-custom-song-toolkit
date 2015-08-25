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
            this.creator_useacronyms = new System.Windows.Forms.CheckBox();
            this.general_defaultgameversion = new System.Windows.Forms.ComboBox();
            this.creator_structured = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.general_defaultappid_RS2012 = new System.Windows.Forms.ComboBox();
            this.gbGeneral = new System.Windows.Forms.GroupBox();
            this.WwisePathButton = new System.Windows.Forms.Button();
            this.general_wwisepath = new RocksmithToolkitGUI.CueTextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.gbAutoUpdate = new System.Windows.Forms.GroupBox();
            this.general_replacerepo = new System.Windows.Forms.CheckBox();
            this.rs2014PathButton = new System.Windows.Forms.Button();
            this.rs1PathButton = new System.Windows.Forms.Button();
            this.general_rs2014path = new RocksmithToolkitGUI.CueTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.general_rs1path = new RocksmithToolkitGUI.CueTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.general_defaultauthor = new RocksmithToolkitGUI.CueTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.general_defaultappid_RS2014 = new System.Windows.Forms.ComboBox();
            this.gbDDC = new System.Windows.Forms.GroupBox();
            this.ddc_phraselength = new RocksmithToolkitGUI.DLCPackageCreator.NumericUpDownFixed();
            this.ddc_removesustain = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ddc_config = new System.Windows.Forms.ComboBox();
            this.ddc_rampup = new System.Windows.Forms.ComboBox();
            this.gbConverter = new System.Windows.Forms.GroupBox();
            this.converter_target = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.converter_source = new System.Windows.Forms.ComboBox();
            this.closeConfigButton = new System.Windows.Forms.Button();
            this.gbCreator.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.creator_scrollspeed)).BeginInit();
            this.gbGeneral.SuspendLayout();
            this.gbAutoUpdate.SuspendLayout();
            this.gbDDC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ddc_phraselength)).BeginInit();
            this.gbConverter.SuspendLayout();
            this.SuspendLayout();
            // 
            // general_usebeta
            // 
            this.general_usebeta.AutoSize = true;
            this.general_usebeta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.general_usebeta.Location = new System.Drawing.Point(12, 19);
            this.general_usebeta.Name = "general_usebeta";
            this.general_usebeta.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.general_usebeta.Size = new System.Drawing.Size(111, 17);
            this.general_usebeta.TabIndex = 8;
            this.general_usebeta.Text = "Use beta releases";
            this.general_usebeta.UseVisualStyleBackColor = true;
            this.general_usebeta.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbCreator
            // 
            this.gbCreator.Controls.Add(this.groupBox1);
            this.gbCreator.Controls.Add(this.label3);
            this.gbCreator.Controls.Add(this.creator_useacronyms);
            this.gbCreator.Controls.Add(this.general_defaultgameversion);
            this.gbCreator.Controls.Add(this.creator_structured);
            this.gbCreator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbCreator.Location = new System.Drawing.Point(3, 192);
            this.gbCreator.Margin = new System.Windows.Forms.Padding(2);
            this.gbCreator.Name = "gbCreator";
            this.gbCreator.Padding = new System.Windows.Forms.Padding(2);
            this.gbCreator.Size = new System.Drawing.Size(516, 87);
            this.gbCreator.TabIndex = 0;
            this.gbCreator.TabStop = false;
            this.gbCreator.Text = "DLC Creator";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.creator_scrollspeed);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(5, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(237, 42);
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
            this.creator_scrollspeed.TabIndex = 11;
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
            this.label3.Location = new System.Drawing.Point(11, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Game Version:";
            // 
            // creator_useacronyms
            // 
            this.creator_useacronyms.AutoSize = true;
            this.creator_useacronyms.ForeColor = System.Drawing.SystemColors.ControlText;
            this.creator_useacronyms.Location = new System.Drawing.Point(291, 40);
            this.creator_useacronyms.Name = "creator_useacronyms";
            this.creator_useacronyms.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.creator_useacronyms.Size = new System.Drawing.Size(216, 17);
            this.creator_useacronyms.TabIndex = 13;
            this.creator_useacronyms.Text = "Use Acronym effect for long artist names";
            this.creator_useacronyms.UseVisualStyleBackColor = true;
            this.creator_useacronyms.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // general_defaultgameversion
            // 
            this.general_defaultgameversion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.general_defaultgameversion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.general_defaultgameversion.Location = new System.Drawing.Point(122, 15);
            this.general_defaultgameversion.Margin = new System.Windows.Forms.Padding(2);
            this.general_defaultgameversion.Name = "general_defaultgameversion";
            this.general_defaultgameversion.Size = new System.Drawing.Size(120, 21);
            this.general_defaultgameversion.TabIndex = 10;
            this.general_defaultgameversion.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // creator_structured
            // 
            this.creator_structured.AutoSize = true;
            this.creator_structured.ForeColor = System.Drawing.SystemColors.ControlText;
            this.creator_structured.Location = new System.Drawing.Point(349, 17);
            this.creator_structured.Name = "creator_structured";
            this.creator_structured.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.creator_structured.Size = new System.Drawing.Size(158, 17);
            this.creator_structured.TabIndex = 12;
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
            this.general_defaultappid_RS2012.TabIndex = 1;
            this.general_defaultappid_RS2012.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbGeneral
            // 
            this.gbGeneral.Controls.Add(this.WwisePathButton);
            this.gbGeneral.Controls.Add(this.general_wwisepath);
            this.gbGeneral.Controls.Add(this.label14);
            this.gbGeneral.Controls.Add(this.label12);
            this.gbGeneral.Controls.Add(this.gbAutoUpdate);
            this.gbGeneral.Controls.Add(this.rs2014PathButton);
            this.gbGeneral.Controls.Add(this.rs1PathButton);
            this.gbGeneral.Controls.Add(this.general_rs2014path);
            this.gbGeneral.Controls.Add(this.label11);
            this.gbGeneral.Controls.Add(this.general_rs1path);
            this.gbGeneral.Controls.Add(this.label10);
            this.gbGeneral.Controls.Add(this.label9);
            this.gbGeneral.Controls.Add(this.general_defaultauthor);
            this.gbGeneral.Controls.Add(this.label4);
            this.gbGeneral.Controls.Add(this.general_defaultappid_RS2014);
            this.gbGeneral.Controls.Add(this.label2);
            this.gbGeneral.Controls.Add(this.general_defaultappid_RS2012);
            this.gbGeneral.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbGeneral.Location = new System.Drawing.Point(2, 2);
            this.gbGeneral.Margin = new System.Windows.Forms.Padding(2);
            this.gbGeneral.Name = "gbGeneral";
            this.gbGeneral.Padding = new System.Windows.Forms.Padding(2);
            this.gbGeneral.Size = new System.Drawing.Size(516, 186);
            this.gbGeneral.TabIndex = 14;
            this.gbGeneral.TabStop = false;
            this.gbGeneral.Text = "General";
            // 
            // WwisePathButton
            // 
            this.WwisePathButton.Location = new System.Drawing.Point(473, 158);
            this.WwisePathButton.Name = "WwisePathButton";
            this.WwisePathButton.Size = new System.Drawing.Size(34, 23);
            this.WwisePathButton.TabIndex = 102;
            this.WwisePathButton.Text = "...";
            this.WwisePathButton.UseVisualStyleBackColor = true;
            this.WwisePathButton.Click += new System.EventHandler(this.WwisePathButton_Click);
            // 
            // general_wwisepath
            // 
            this.general_wwisepath.Cue = "Wwise v2013.2.x build 48xx path (must be this build series)";
            this.general_wwisepath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.general_wwisepath.ForeColor = System.Drawing.Color.Gray;
            this.general_wwisepath.Location = new System.Drawing.Point(122, 160);
            this.general_wwisepath.Name = "general_wwisepath";
            this.general_wwisepath.Size = new System.Drawing.Size(345, 20);
            this.general_wwisepath.TabIndex = 101;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(11, 163);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 13);
            this.label14.TabIndex = 103;
            this.label14.Text = "Wwise path:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label12.Location = new System.Drawing.Point(119, 88);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(169, 13);
            this.label12.TabIndex = 100;
            this.label12.Text = "Will be written inside the package.";
            // 
            // gbAutoUpdate
            // 
            this.gbAutoUpdate.Controls.Add(this.general_replacerepo);
            this.gbAutoUpdate.Controls.Add(this.general_usebeta);
            this.gbAutoUpdate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbAutoUpdate.Location = new System.Drawing.Point(377, 10);
            this.gbAutoUpdate.Name = "gbAutoUpdate";
            this.gbAutoUpdate.Size = new System.Drawing.Size(129, 77);
            this.gbAutoUpdate.TabIndex = 99;
            this.gbAutoUpdate.TabStop = false;
            this.gbAutoUpdate.Text = "Auto-update";
            // 
            // general_replacerepo
            // 
            this.general_replacerepo.AutoSize = true;
            this.general_replacerepo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.general_replacerepo.Location = new System.Drawing.Point(13, 42);
            this.general_replacerepo.Name = "general_replacerepo";
            this.general_replacerepo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.general_replacerepo.Size = new System.Drawing.Size(110, 17);
            this.general_replacerepo.TabIndex = 9;
            this.general_replacerepo.Text = "Reset repositories";
            this.general_replacerepo.UseVisualStyleBackColor = true;
            // 
            // rs2014PathButton
            // 
            this.rs2014PathButton.Location = new System.Drawing.Point(473, 134);
            this.rs2014PathButton.Name = "rs2014PathButton";
            this.rs2014PathButton.Size = new System.Drawing.Size(34, 23);
            this.rs2014PathButton.TabIndex = 7;
            this.rs2014PathButton.Text = "...";
            this.rs2014PathButton.UseVisualStyleBackColor = true;
            this.rs2014PathButton.Click += new System.EventHandler(this.rs2014PathButton_Click);
            // 
            // rs1PathButton
            // 
            this.rs1PathButton.Location = new System.Drawing.Point(473, 108);
            this.rs1PathButton.Name = "rs1PathButton";
            this.rs1PathButton.Size = new System.Drawing.Size(34, 23);
            this.rs1PathButton.TabIndex = 5;
            this.rs1PathButton.Text = "...";
            this.rs1PathButton.UseVisualStyleBackColor = true;
            this.rs1PathButton.Click += new System.EventHandler(this.rs1PathButton_Click);
            // 
            // general_rs2014path
            // 
            this.general_rs2014path.Cue = "Rocksmith 2014 path";
            this.general_rs2014path.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.general_rs2014path.ForeColor = System.Drawing.Color.Gray;
            this.general_rs2014path.Location = new System.Drawing.Point(122, 136);
            this.general_rs2014path.Name = "general_rs2014path";
            this.general_rs2014path.Size = new System.Drawing.Size(345, 20);
            this.general_rs2014path.TabIndex = 6;
            this.general_rs2014path.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(11, 139);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 13);
            this.label11.TabIndex = 50;
            this.label11.Text = "Rocksmith 2014 path:";
            // 
            // general_rs1path
            // 
            this.general_rs1path.Cue = "Rocksmith path";
            this.general_rs1path.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.general_rs1path.ForeColor = System.Drawing.Color.Gray;
            this.general_rs1path.Location = new System.Drawing.Point(122, 110);
            this.general_rs1path.Name = "general_rs1path";
            this.general_rs1path.Size = new System.Drawing.Size(345, 20);
            this.general_rs1path.TabIndex = 4;
            this.general_rs1path.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(11, 112);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 13);
            this.label10.TabIndex = 48;
            this.label10.Text = "Rocksmith path:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(11, 70);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 13);
            this.label9.TabIndex = 47;
            this.label9.Text = "Package Author:";
            // 
            // general_defaultauthor
            // 
            this.general_defaultauthor.Cue = "Author";
            this.general_defaultauthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.general_defaultauthor.ForeColor = System.Drawing.Color.Gray;
            this.general_defaultauthor.Location = new System.Drawing.Point(122, 67);
            this.general_defaultauthor.Name = "general_defaultauthor";
            this.general_defaultauthor.Size = new System.Drawing.Size(250, 20);
            this.general_defaultauthor.TabIndex = 3;
            this.general_defaultauthor.Leave += new System.EventHandler(this.ConfigurationChanged);
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
            this.general_defaultappid_RS2014.TabIndex = 2;
            this.general_defaultappid_RS2014.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbDDC
            // 
            this.gbDDC.Controls.Add(this.ddc_phraselength);
            this.gbDDC.Controls.Add(this.ddc_removesustain);
            this.gbDDC.Controls.Add(this.label6);
            this.gbDDC.Controls.Add(this.label13);
            this.gbDDC.Controls.Add(this.label5);
            this.gbDDC.Controls.Add(this.ddc_config);
            this.gbDDC.Controls.Add(this.ddc_rampup);
            this.gbDDC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbDDC.Location = new System.Drawing.Point(3, 284);
            this.gbDDC.Name = "gbDDC";
            this.gbDDC.Size = new System.Drawing.Size(516, 94);
            this.gbDDC.TabIndex = 15;
            this.gbDDC.TabStop = false;
            this.gbDDC.Text = "DDC";
            // 
            // ddc_phraselength
            // 
            this.ddc_phraselength.Location = new System.Drawing.Point(122, 67);
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
            this.ddc_phraselength.TabIndex = 16;
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
            this.ddc_removesustain.Location = new System.Drawing.Point(400, 18);
            this.ddc_removesustain.Name = "ddc_removesustain";
            this.ddc_removesustain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ddc_removesustain.Size = new System.Drawing.Size(107, 17);
            this.ddc_removesustain.TabIndex = 17;
            this.ddc_removesustain.Text = "Remove sustains";
            this.ddc_removesustain.UseVisualStyleBackColor = true;
            this.ddc_removesustain.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(12, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "Phrase length:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(11, 45);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 46;
            this.label13.Text = "Config File:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(11, 19);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Rampage:";
            // 
            // ddc_config
            // 
            this.ddc_config.AllowDrop = true;
            this.ddc_config.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddc_config.FormattingEnabled = true;
            this.ddc_config.Location = new System.Drawing.Point(122, 42);
            this.ddc_config.MinimumSize = new System.Drawing.Size(20, 0);
            this.ddc_config.Name = "ddc_config";
            this.ddc_config.Size = new System.Drawing.Size(171, 21);
            this.ddc_config.Sorted = true;
            this.ddc_config.TabIndex = 15;
            this.ddc_config.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // ddc_rampup
            // 
            this.ddc_rampup.AllowDrop = true;
            this.ddc_rampup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddc_rampup.FormattingEnabled = true;
            this.ddc_rampup.Location = new System.Drawing.Point(122, 16);
            this.ddc_rampup.MinimumSize = new System.Drawing.Size(20, 0);
            this.ddc_rampup.Name = "ddc_rampup";
            this.ddc_rampup.Size = new System.Drawing.Size(171, 21);
            this.ddc_rampup.Sorted = true;
            this.ddc_rampup.TabIndex = 14;
            this.ddc_rampup.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // gbConverter
            // 
            this.gbConverter.Controls.Add(this.converter_target);
            this.gbConverter.Controls.Add(this.label7);
            this.gbConverter.Controls.Add(this.label8);
            this.gbConverter.Controls.Add(this.converter_source);
            this.gbConverter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbConverter.Location = new System.Drawing.Point(3, 384);
            this.gbConverter.Name = "gbConverter";
            this.gbConverter.Size = new System.Drawing.Size(516, 45);
            this.gbConverter.TabIndex = 50;
            this.gbConverter.TabStop = false;
            this.gbConverter.Text = "Converter";
            // 
            // converter_target
            // 
            this.converter_target.AllowDrop = true;
            this.converter_target.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.converter_target.FormattingEnabled = true;
            this.converter_target.Location = new System.Drawing.Point(387, 15);
            this.converter_target.MinimumSize = new System.Drawing.Size(20, 0);
            this.converter_target.Name = "converter_target";
            this.converter_target.Size = new System.Drawing.Size(120, 21);
            this.converter_target.Sorted = true;
            this.converter_target.TabIndex = 19;
            this.converter_target.SelectedIndexChanged += new System.EventHandler(this.ConfigurationChanged);
            this.converter_target.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(288, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "Target Platform:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(11, 18);
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
            this.converter_source.Location = new System.Drawing.Point(122, 15);
            this.converter_source.MinimumSize = new System.Drawing.Size(20, 0);
            this.converter_source.Name = "converter_source";
            this.converter_source.Size = new System.Drawing.Size(120, 21);
            this.converter_source.Sorted = true;
            this.converter_source.TabIndex = 18;
            this.converter_source.Leave += new System.EventHandler(this.ConfigurationChanged);
            // 
            // closeConfigButton
            // 
            this.closeConfigButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.closeConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeConfigButton.Location = new System.Drawing.Point(404, 435);
            this.closeConfigButton.Name = "closeConfigButton";
            this.closeConfigButton.Size = new System.Drawing.Size(116, 29);
            this.closeConfigButton.TabIndex = 0;
            this.closeConfigButton.Text = "Close";
            this.closeConfigButton.UseVisualStyleBackColor = false;
            this.closeConfigButton.Click += new System.EventHandler(this.closeConfigButton_Click);
            // 
            // GeneralConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.closeConfigButton);
            this.Controls.Add(this.gbConverter);
            this.Controls.Add(this.gbDDC);
            this.Controls.Add(this.gbGeneral);
            this.Controls.Add(this.gbCreator);
            this.Name = "GeneralConfig";
            this.Size = new System.Drawing.Size(521, 505);
            this.gbCreator.ResumeLayout(false);
            this.gbCreator.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.creator_scrollspeed)).EndInit();
            this.gbGeneral.ResumeLayout(false);
            this.gbGeneral.PerformLayout();
            this.gbAutoUpdate.ResumeLayout(false);
            this.gbAutoUpdate.PerformLayout();
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
        private System.Windows.Forms.ComboBox general_defaultgameversion;
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
        private System.Windows.Forms.Button closeConfigButton;
        private System.Windows.Forms.Label label9;
        private CueTextBox general_defaultauthor;
        private CueTextBox general_rs1path;
        private System.Windows.Forms.Label label10;
        private CueTextBox general_rs2014path;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button rs2014PathButton;
        private System.Windows.Forms.Button rs1PathButton;
        private System.Windows.Forms.CheckBox general_replacerepo;
        private System.Windows.Forms.GroupBox gbAutoUpdate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox ddc_config;
        private System.Windows.Forms.Button WwisePathButton;
        private CueTextBox general_wwisepath;
        private System.Windows.Forms.Label label14;
    }
}
