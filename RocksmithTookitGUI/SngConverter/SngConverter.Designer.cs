namespace RocksmithToolkitGUI.SngConverter
{
    partial class SngConverter
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gbPlatform = new System.Windows.Forms.GroupBox();
            this.platformCombo = new System.Windows.Forms.ComboBox();
            this.gbOperation = new System.Windows.Forms.GroupBox();
            this.unpackRadio = new System.Windows.Forms.RadioButton();
            this.packRadio = new System.Windows.Forms.RadioButton();
            this.packUnpackButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.vocalRadio = new System.Windows.Forms.RadioButton();
            this.instrumentRadio = new System.Windows.Forms.RadioButton();
            this.browseSngXmlButton = new System.Windows.Forms.Button();
            this.browseManifestButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.platform2Combo = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.xml2sngRadio = new System.Windows.Forms.RadioButton();
            this.sng2xmlRadio = new System.Windows.Forms.RadioButton();
            this.convertSngXmlButton = new System.Windows.Forms.Button();
            this.sngXmlTB = new RocksmithToolkitGUI.CueTextBox();
            this.manifestTB = new RocksmithToolkitGUI.CueTextBox();
            this.groupBox2.SuspendLayout();
            this.gbPlatform.SuspendLayout();
            this.gbOperation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gbPlatform);
            this.groupBox2.Controls.Add(this.gbOperation);
            this.groupBox2.Controls.Add(this.packUnpackButton);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(9, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(477, 73);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SNG Pack-Encrypt / Decrypt-Unpack";
            // 
            // gbPlatform
            // 
            this.gbPlatform.Controls.Add(this.platformCombo);
            this.gbPlatform.Location = new System.Drawing.Point(161, 20);
            this.gbPlatform.Name = "gbPlatform";
            this.gbPlatform.Size = new System.Drawing.Size(141, 46);
            this.gbPlatform.TabIndex = 5;
            this.gbPlatform.TabStop = false;
            this.gbPlatform.Text = "Platform";
            // 
            // platformCombo
            // 
            this.platformCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.platformCombo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platformCombo.Location = new System.Drawing.Point(5, 18);
            this.platformCombo.Margin = new System.Windows.Forms.Padding(2);
            this.platformCombo.Name = "platformCombo";
            this.platformCombo.Size = new System.Drawing.Size(131, 21);
            this.platformCombo.TabIndex = 17;
            // 
            // gbOperation
            // 
            this.gbOperation.Controls.Add(this.unpackRadio);
            this.gbOperation.Controls.Add(this.packRadio);
            this.gbOperation.Location = new System.Drawing.Point(6, 20);
            this.gbOperation.Name = "gbOperation";
            this.gbOperation.Size = new System.Drawing.Size(149, 46);
            this.gbOperation.TabIndex = 4;
            this.gbOperation.TabStop = false;
            this.gbOperation.Text = "Type";
            // 
            // unpackRadio
            // 
            this.unpackRadio.AutoSize = true;
            this.unpackRadio.Checked = true;
            this.unpackRadio.Location = new System.Drawing.Point(80, 20);
            this.unpackRadio.Name = "unpackRadio";
            this.unpackRadio.Size = new System.Drawing.Size(63, 17);
            this.unpackRadio.TabIndex = 1;
            this.unpackRadio.TabStop = true;
            this.unpackRadio.Text = "Unpack";
            this.unpackRadio.UseVisualStyleBackColor = true;
            // 
            // packRadio
            // 
            this.packRadio.AutoSize = true;
            this.packRadio.Location = new System.Drawing.Point(17, 20);
            this.packRadio.Name = "packRadio";
            this.packRadio.Size = new System.Drawing.Size(50, 17);
            this.packRadio.TabIndex = 0;
            this.packRadio.Text = "Pack";
            this.packRadio.UseVisualStyleBackColor = true;
            // 
            // packUnpackButton
            // 
            this.packUnpackButton.Location = new System.Drawing.Point(307, 33);
            this.packUnpackButton.Margin = new System.Windows.Forms.Padding(2);
            this.packUnpackButton.Name = "packUnpackButton";
            this.packUnpackButton.Size = new System.Drawing.Size(164, 28);
            this.packUnpackButton.TabIndex = 3;
            this.packUnpackButton.Text = "Choose SNG";
            this.packUnpackButton.UseVisualStyleBackColor = true;
            this.packUnpackButton.Click += new System.EventHandler(this.packUnpackButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sngXmlTB);
            this.groupBox1.Controls.Add(this.manifestTB);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.browseSngXmlButton);
            this.groupBox1.Controls.Add(this.browseManifestButton);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.convertSngXmlButton);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(9, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(477, 154);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sng 2 Xml / Xml 2 Sng Converter";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.vocalRadio);
            this.groupBox5.Controls.Add(this.instrumentRadio);
            this.groupBox5.Location = new System.Drawing.Point(173, 20);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(149, 46);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Arrangement Type";
            // 
            // vocalRadio
            // 
            this.vocalRadio.AutoSize = true;
            this.vocalRadio.Location = new System.Drawing.Point(94, 19);
            this.vocalRadio.Name = "vocalRadio";
            this.vocalRadio.Size = new System.Drawing.Size(52, 17);
            this.vocalRadio.TabIndex = 1;
            this.vocalRadio.Text = "Vocal";
            this.vocalRadio.UseVisualStyleBackColor = true;
            // 
            // instrumentRadio
            // 
            this.instrumentRadio.AutoSize = true;
            this.instrumentRadio.Checked = true;
            this.instrumentRadio.Location = new System.Drawing.Point(8, 19);
            this.instrumentRadio.Name = "instrumentRadio";
            this.instrumentRadio.Size = new System.Drawing.Size(81, 17);
            this.instrumentRadio.TabIndex = 0;
            this.instrumentRadio.TabStop = true;
            this.instrumentRadio.Text = "Guitar/Bass";
            this.instrumentRadio.UseVisualStyleBackColor = true;
            // 
            // browseSngXmlButton
            // 
            this.browseSngXmlButton.Location = new System.Drawing.Point(415, 94);
            this.browseSngXmlButton.Margin = new System.Windows.Forms.Padding(2);
            this.browseSngXmlButton.Name = "browseSngXmlButton";
            this.browseSngXmlButton.Size = new System.Drawing.Size(56, 20);
            this.browseSngXmlButton.TabIndex = 9;
            this.browseSngXmlButton.Text = "Browse";
            this.browseSngXmlButton.UseVisualStyleBackColor = true;
            this.browseSngXmlButton.Click += new System.EventHandler(this.browseSngXmlButton_Click);
            // 
            // browseManifestButton
            // 
            this.browseManifestButton.Location = new System.Drawing.Point(415, 70);
            this.browseManifestButton.Margin = new System.Windows.Forms.Padding(2);
            this.browseManifestButton.Name = "browseManifestButton";
            this.browseManifestButton.Size = new System.Drawing.Size(56, 20);
            this.browseManifestButton.TabIndex = 8;
            this.browseManifestButton.Text = "Browse";
            this.browseManifestButton.UseVisualStyleBackColor = true;
            this.browseManifestButton.Click += new System.EventHandler(this.browseManifestButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.platform2Combo);
            this.groupBox3.Location = new System.Drawing.Point(324, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(147, 46);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Platform";
            // 
            // platform2Combo
            // 
            this.platform2Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.platform2Combo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.platform2Combo.Location = new System.Drawing.Point(11, 18);
            this.platform2Combo.Margin = new System.Windows.Forms.Padding(2);
            this.platform2Combo.Name = "platform2Combo";
            this.platform2Combo.Size = new System.Drawing.Size(131, 21);
            this.platform2Combo.TabIndex = 17;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.xml2sngRadio);
            this.groupBox4.Controls.Add(this.sng2xmlRadio);
            this.groupBox4.Location = new System.Drawing.Point(6, 20);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(161, 46);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Type";
            // 
            // xml2sngRadio
            // 
            this.xml2sngRadio.AutoSize = true;
            this.xml2sngRadio.Location = new System.Drawing.Point(85, 19);
            this.xml2sngRadio.Name = "xml2sngRadio";
            this.xml2sngRadio.Size = new System.Drawing.Size(73, 17);
            this.xml2sngRadio.TabIndex = 1;
            this.xml2sngRadio.Text = "Xml > Sng";
            this.xml2sngRadio.UseVisualStyleBackColor = true;
            this.xml2sngRadio.CheckedChanged += new System.EventHandler(this.sng2xmlRadio_CheckedChanged);
            // 
            // sng2xmlRadio
            // 
            this.sng2xmlRadio.AutoSize = true;
            this.sng2xmlRadio.Checked = true;
            this.sng2xmlRadio.Location = new System.Drawing.Point(7, 19);
            this.sng2xmlRadio.Name = "sng2xmlRadio";
            this.sng2xmlRadio.Size = new System.Drawing.Size(73, 17);
            this.sng2xmlRadio.TabIndex = 0;
            this.sng2xmlRadio.TabStop = true;
            this.sng2xmlRadio.Text = "Sng > Xml";
            this.sng2xmlRadio.UseVisualStyleBackColor = true;
            this.sng2xmlRadio.CheckedChanged += new System.EventHandler(this.sng2xmlRadio_CheckedChanged);
            // 
            // convertSngXmlButton
            // 
            this.convertSngXmlButton.Location = new System.Drawing.Point(161, 119);
            this.convertSngXmlButton.Margin = new System.Windows.Forms.Padding(2);
            this.convertSngXmlButton.Name = "convertSngXmlButton";
            this.convertSngXmlButton.Size = new System.Drawing.Size(164, 28);
            this.convertSngXmlButton.TabIndex = 3;
            this.convertSngXmlButton.Text = "Convert";
            this.convertSngXmlButton.UseVisualStyleBackColor = true;
            this.convertSngXmlButton.Click += new System.EventHandler(this.convertSngXmlButton_Click);
            // 
            // sngXmlTB
            // 
            this.sngXmlTB.Cue = "SNG file";
            this.sngXmlTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.sngXmlTB.ForeColor = System.Drawing.Color.Gray;
            this.sngXmlTB.Location = new System.Drawing.Point(6, 94);
            this.sngXmlTB.Name = "sngXmlTB";
            this.sngXmlTB.Size = new System.Drawing.Size(404, 20);
            this.sngXmlTB.TabIndex = 43;
            // 
            // manifestTB
            // 
            this.manifestTB.Cue = "Manifest file";
            this.manifestTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.manifestTB.ForeColor = System.Drawing.Color.Gray;
            this.manifestTB.Location = new System.Drawing.Point(6, 70);
            this.manifestTB.Name = "manifestTB";
            this.manifestTB.Size = new System.Drawing.Size(404, 20);
            this.manifestTB.TabIndex = 42;
            // 
            // SngConverter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "SngConverter";
            this.Size = new System.Drawing.Size(496, 245);
            this.groupBox2.ResumeLayout(false);
            this.gbPlatform.ResumeLayout(false);
            this.gbOperation.ResumeLayout(false);
            this.gbOperation.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button packUnpackButton;
        private System.Windows.Forms.GroupBox gbPlatform;
        private System.Windows.Forms.GroupBox gbOperation;
        private System.Windows.Forms.RadioButton unpackRadio;
        private System.Windows.Forms.RadioButton packRadio;
        private System.Windows.Forms.ComboBox platformCombo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox platform2Combo;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton xml2sngRadio;
        private System.Windows.Forms.RadioButton sng2xmlRadio;
        private System.Windows.Forms.Button convertSngXmlButton;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton vocalRadio;
        private System.Windows.Forms.RadioButton instrumentRadio;
        private System.Windows.Forms.Button browseSngXmlButton;
        private System.Windows.Forms.Button browseManifestButton;
        private CueTextBox sngXmlTB;
        private CueTextBox manifestTB;
    }
}
