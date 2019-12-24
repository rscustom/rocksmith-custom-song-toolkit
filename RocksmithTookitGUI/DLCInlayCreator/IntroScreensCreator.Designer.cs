namespace RocksmithToolkitGUI.DLCInlayCreator
{
    partial class IntroScreensCreator
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
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblCredits = new System.Windows.Forms.Label();
            this.lblUbi = new System.Windows.Forms.Label();
            this.lblLightspeed = new System.Windows.Forms.Label();
            this.lblPedals = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picPedals = new System.Windows.Forms.PictureBox();
            this.picTitle = new System.Windows.Forms.PictureBox();
            this.picLightspeed = new System.Windows.Forms.PictureBox();
            this.picUbi = new System.Windows.Forms.PictureBox();
            this.picCredits = new System.Windows.Forms.PictureBox();
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSeqName = new RocksmithToolkitGUI.CueTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAuthor = new RocksmithToolkitGUI.CueTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbarStatus = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.helpLink = new System.Windows.Forms.LinkLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPedals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLightspeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUbi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCredits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.Location = new System.Drawing.Point(342, 361);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(159, 29);
            this.btnGenerate.TabIndex = 10;
            this.btnGenerate.Text = "Generate";
            this.toolTip.SetToolTip(this.btnGenerate, "Create your custom intro screen\r\nsequence for use in the game.");
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(350, 179);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(121, 30);
            this.lblTitle.TabIndex = 66;
            this.lblTitle.Text = "\r\nClick Here! (optional)";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackground.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblBackground.Location = new System.Drawing.Point(25, 49);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(121, 30);
            this.lblBackground.TabIndex = 67;
            this.lblBackground.Text = "\r\nClick Here! (optional)";
            this.lblBackground.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBackground.Click += new System.EventHandler(this.lblBackground_Click);
            // 
            // lblCredits
            // 
            this.lblCredits.AutoSize = true;
            this.lblCredits.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCredits.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCredits.Location = new System.Drawing.Point(25, 179);
            this.lblCredits.Name = "lblCredits";
            this.lblCredits.Size = new System.Drawing.Size(121, 30);
            this.lblCredits.TabIndex = 68;
            this.lblCredits.Text = "\r\nClick Here! (optional)";
            this.lblCredits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCredits.Click += new System.EventHandler(this.lblCredits_Click);
            // 
            // lblUbi
            // 
            this.lblUbi.AutoSize = true;
            this.lblUbi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUbi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblUbi.Location = new System.Drawing.Point(187, 49);
            this.lblUbi.Name = "lblUbi";
            this.lblUbi.Size = new System.Drawing.Size(121, 30);
            this.lblUbi.TabIndex = 69;
            this.lblUbi.Text = "\r\nClick Here! (optional)";
            this.lblUbi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUbi.Click += new System.EventHandler(this.lblUbi_Click);
            // 
            // lblLightspeed
            // 
            this.lblLightspeed.AutoSize = true;
            this.lblLightspeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLightspeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblLightspeed.Location = new System.Drawing.Point(187, 179);
            this.lblLightspeed.Name = "lblLightspeed";
            this.lblLightspeed.Size = new System.Drawing.Size(121, 30);
            this.lblLightspeed.TabIndex = 70;
            this.lblLightspeed.Text = "\r\nClick Here! (optional)";
            this.lblLightspeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLightspeed.Click += new System.EventHandler(this.lblLightspeed_Click);
            // 
            // lblPedals
            // 
            this.lblPedals.AutoSize = true;
            this.lblPedals.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPedals.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblPedals.Location = new System.Drawing.Point(350, 49);
            this.lblPedals.Name = "lblPedals";
            this.lblPedals.Size = new System.Drawing.Size(121, 30);
            this.lblPedals.TabIndex = 71;
            this.lblPedals.Text = "\r\nClick Here! (optional)";
            this.lblPedals.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPedals.Click += new System.EventHandler(this.lblPedals_Click);
            // 
            // btnSave
            // 
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnSave.Location = new System.Drawing.Point(107, 362);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 29);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save Template";
            this.toolTip.SetToolTip(this.btnSave, "Save your intro screen sequence as a\r\ntemplate.  Templates can be shared \r\nwith o" +
                    "ther users.   Click on Help for\r\nadditional information.");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnLoad.Location = new System.Drawing.Point(3, 361);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(94, 29);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load Template";
            this.toolTip.SetToolTip(this.btnLoad, "Load an intro screen sequence template.  \r\nAdditional shared templates can found " +
                    "on \r\nthe website.  Click on Help for additional \r\ninformation.");
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnRestore.Location = new System.Drawing.Point(217, 362);
            this.btnRestore.Margin = new System.Windows.Forms.Padding(2);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(67, 29);
            this.btnRestore.TabIndex = 9;
            this.btnRestore.Text = "Restore";
            this.toolTip.SetToolTip(this.btnRestore, "Restore the intro screens to the original content.\r\nWARNING ... this will overwri" +
                    "te any user changes!");
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 25000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // picPedals
            // 
            this.picPedals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPedals.Location = new System.Drawing.Point(332, 19);
            this.picPedals.Name = "picPedals";
            this.picPedals.Size = new System.Drawing.Size(157, 108);
            this.picPedals.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPedals.TabIndex = 59;
            this.picPedals.TabStop = false;
            this.picPedals.Click += new System.EventHandler(this.picPedals_Click);
            // 
            // picTitle
            // 
            this.picTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTitle.Location = new System.Drawing.Point(332, 148);
            this.picTitle.Name = "picTitle";
            this.picTitle.Size = new System.Drawing.Size(157, 108);
            this.picTitle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picTitle.TabIndex = 58;
            this.picTitle.TabStop = false;
            this.picTitle.Click += new System.EventHandler(this.picTitle_Click);
            // 
            // picLightspeed
            // 
            this.picLightspeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLightspeed.Location = new System.Drawing.Point(169, 149);
            this.picLightspeed.Name = "picLightspeed";
            this.picLightspeed.Size = new System.Drawing.Size(157, 108);
            this.picLightspeed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLightspeed.TabIndex = 57;
            this.picLightspeed.TabStop = false;
            this.picLightspeed.Click += new System.EventHandler(this.picLightspeed_Click);
            // 
            // picUbi
            // 
            this.picUbi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picUbi.Location = new System.Drawing.Point(169, 19);
            this.picUbi.Name = "picUbi";
            this.picUbi.Size = new System.Drawing.Size(157, 108);
            this.picUbi.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUbi.TabIndex = 56;
            this.picUbi.TabStop = false;
            this.picUbi.Click += new System.EventHandler(this.picUbi_Click);
            // 
            // picCredits
            // 
            this.picCredits.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picCredits.Location = new System.Drawing.Point(6, 149);
            this.picCredits.Name = "picCredits";
            this.picCredits.Size = new System.Drawing.Size(157, 108);
            this.picCredits.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCredits.TabIndex = 55;
            this.picCredits.TabStop = false;
            this.picCredits.Click += new System.EventHandler(this.picCredits_Click);
            // 
            // picBackground
            // 
            this.picBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBackground.Location = new System.Drawing.Point(6, 19);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(157, 108);
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBackground.TabIndex = 2;
            this.picBackground.TabStop = false;
            this.picBackground.Click += new System.EventHandler(this.picBackground_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSeqName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtAuthor);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 70);
            this.groupBox1.TabIndex = 83;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Intro Screens Template Info:";
            // 
            // txtSeqName
            // 
            this.txtSeqName.Cue = "Sequence Name";
            this.txtSeqName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtSeqName.ForeColor = System.Drawing.Color.Gray;
            this.txtSeqName.Location = new System.Drawing.Point(204, 31);
            this.txtSeqName.Name = "txtSeqName";
            this.txtSeqName.Size = new System.Drawing.Size(288, 20);
            this.txtSeqName.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(204, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 71;
            this.label5.Text = "Sequence:";
            // 
            // txtAuthor
            // 
            this.txtAuthor.Cue = "Author";
            this.txtAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAuthor.ForeColor = System.Drawing.Color.Gray;
            this.txtAuthor.Location = new System.Drawing.Point(10, 31);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(188, 20);
            this.txtAuthor.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "Author:";
            // 
            // pbarStatus
            // 
            this.pbarStatus.Location = new System.Drawing.Point(3, 395);
            this.pbarStatus.Name = "pbarStatus";
            this.pbarStatus.Size = new System.Drawing.Size(498, 18);
            this.pbarStatus.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbarStatus.TabIndex = 86;
            this.pbarStatus.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblStatus.Location = new System.Drawing.Point(3, 416);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(71, 13);
            this.lblStatus.TabIndex = 87;
            this.lblStatus.Text = "Generating ...";
            this.lblStatus.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(278, 416);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 91;
            this.label3.Text = "Help can be found at:";
            // 
            // helpLink
            // 
            this.helpLink.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.helpLink.ActiveLinkColor = System.Drawing.Color.RosyBrown;
            this.helpLink.AutoEllipsis = true;
            this.helpLink.AutoSize = true;
            this.helpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helpLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.helpLink.Location = new System.Drawing.Point(385, 414);
            this.helpLink.Name = "helpLink";
            this.helpLink.Size = new System.Drawing.Size(116, 15);
            this.helpLink.TabIndex = 90;
            this.helpLink.TabStop = true;
            this.helpLink.Text = "http://goo.gl/pJxMuz";
            this.helpLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.helpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpLink_LinkClicked);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblTitle);
            this.groupBox2.Controls.Add(this.lblBackground);
            this.groupBox2.Controls.Add(this.lblCredits);
            this.groupBox2.Controls.Add(this.lblPedals);
            this.groupBox2.Controls.Add(this.lblUbi);
            this.groupBox2.Controls.Add(this.lblLightspeed);
            this.groupBox2.Controls.Add(this.picBackground);
            this.groupBox2.Controls.Add(this.picUbi);
            this.groupBox2.Controls.Add(this.picPedals);
            this.groupBox2.Controls.Add(this.picCredits);
            this.groupBox2.Controls.Add(this.picLightspeed);
            this.groupBox2.Controls.Add(this.picTitle);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(3, 79);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(498, 277);
            this.groupBox2.TabIndex = 92;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Intro Screens:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label9.Location = new System.Drawing.Point(330, 259);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 96;
            this.label9.Text = "RS Title Logo";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label8.Location = new System.Drawing.Point(168, 259);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 95;
            this.label8.Text = "Lightspeed Logo";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Location = new System.Drawing.Point(6, 259);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 94;
            this.label6.Text = "Credits (Gibson)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label4.Location = new System.Drawing.Point(330, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 93;
            this.label4.Text = "Studio Pedals";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Location = new System.Drawing.Point(167, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 92;
            this.label2.Text = "Ubisoft Logo";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Location = new System.Drawing.Point(4, 129);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 91;
            this.label7.Text = "Gray Background";
            // 
            // IntroScreensCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.helpLink);
            this.Controls.Add(this.pbarStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnGenerate);
            this.Name = "IntroScreensCreator";
            this.Size = new System.Drawing.Size(506, 450);
            this.toolTip.SetToolTip(this, "ReadMe: \r\nThis toolkit feature is no longer maintained or updated.\r\nPlease use th" +
                    "e latestest version of Custom Game Toolkit\r\nto create custom game intro screens " +
                    "and other mods.");
            this.Disposed += new System.EventHandler(this.IntroScreensCreator_Dispose);
            ((System.ComponentModel.ISupportInitialize)(this.picPedals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLightspeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUbi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCredits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.PictureBox picCredits;
        private System.Windows.Forms.PictureBox picUbi;
        private System.Windows.Forms.PictureBox picLightspeed;
        private System.Windows.Forms.PictureBox picTitle;
        private System.Windows.Forms.PictureBox picPedals;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.Label lblCredits;
        private System.Windows.Forms.Label lblUbi;
        private System.Windows.Forms.Label lblLightspeed;
        private System.Windows.Forms.Label lblPedals;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox groupBox1;
        public CueTextBox txtSeqName;
        private System.Windows.Forms.Label label5;
        public CueTextBox txtAuthor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar pbarStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel helpLink;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
    }
}