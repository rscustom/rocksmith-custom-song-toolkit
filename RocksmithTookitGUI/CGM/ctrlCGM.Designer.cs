using System.Drawing;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.CGM
{
    partial class ctrlCGM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlCGM));
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.txtAppId = new System.Windows.Forms.TextBox();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.cmbAppId = new System.Windows.Forms.ComboBox();
            this.picInlay = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbPlatform = new System.Windows.Forms.ComboBox();
            this.txtRS2014Path = new System.Windows.Forms.TextBox();
            this.btnRS2014Dir = new System.Windows.Forms.Button();
            this.btnSaveCGM = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnLoadCGM = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.picGuitar = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCreator = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtGuitar = new System.Windows.Forms.TextBox();
            this.chkFlipX = new System.Windows.Forms.CheckBox();
            this.picFlipX = new System.Windows.Forms.PictureBox();
            this.picFlipY = new System.Windows.Forms.PictureBox();
            this.chkFlipY = new System.Windows.Forms.CheckBox();
            this.chkColor = new System.Windows.Forms.CheckBox();
            this.chk24Fret = new System.Windows.Forms.CheckBox();
            this.txtOpenCGM = new System.Windows.Forms.TextBox();
            this.lblCredits1 = new System.Windows.Forms.Label();
            this.lblCredits2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGuitar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipY)).BeginInit();
            this.SuspendLayout();
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.picLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picLogo.BackgroundImage")));
            this.picLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
            this.picLogo.InitialImage = null;
            this.picLogo.Location = new System.Drawing.Point(94, 13);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(402, 102);
            this.picLogo.TabIndex = 70;
            this.picLogo.TabStop = false;
            // 
            // txtAppId
            // 
            this.txtAppId.ForeColor = System.Drawing.Color.Gray;
            this.txtAppId.Location = new System.Drawing.Point(425, 147);
            this.txtAppId.Margin = new System.Windows.Forms.Padding(2);
            this.txtAppId.Name = "txtAppId";
            this.txtAppId.Size = new System.Drawing.Size(57, 20);
            this.txtAppId.TabIndex = 69;
            this.txtAppId.Text = "APPID";
            this.txtAppId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // picIcon
            // 
            this.picIcon.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picIcon.Location = new System.Drawing.Point(68, 393);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(72, 72);
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picIcon.TabIndex = 67;
            this.picIcon.TabStop = false;
            this.picIcon.Click += new System.EventHandler(this.picIcon_Click);
            // 
            // cmbAppId
            // 
            this.cmbAppId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAppId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppId.FormattingEnabled = true;
            this.cmbAppId.Location = new System.Drawing.Point(114, 147);
            this.cmbAppId.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAppId.Name = "cmbAppId";
            this.cmbAppId.Size = new System.Drawing.Size(289, 21);
            this.cmbAppId.TabIndex = 68;
            this.cmbAppId.SelectedIndexChanged += new System.EventHandler(this.cmbAppId_SelectedIndexChanged);
            // 
            // picInlay
            // 
            this.picInlay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picInlay.Location = new System.Drawing.Point(198, 393);
            this.picInlay.Name = "picInlay";
            this.picInlay.Size = new System.Drawing.Size(284, 128);
            this.picInlay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picInlay.TabIndex = 66;
            this.picInlay.TabStop = false;
            this.picInlay.Click += new System.EventHandler(this.picInlay_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(21, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 63;
            this.label2.Text = "Platform:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 181);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(185, 13);
            this.label6.TabIndex = 61;
            this.label6.Text = "Custom Guitar Inlay Storage Location:";
            // 
            // cmbPlatform
            // 
            this.cmbPlatform.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlatform.FormattingEnabled = true;
            this.cmbPlatform.Location = new System.Drawing.Point(21, 147);
            this.cmbPlatform.Margin = new System.Windows.Forms.Padding(2);
            this.cmbPlatform.Name = "cmbPlatform";
            this.cmbPlatform.Size = new System.Drawing.Size(73, 21);
            this.cmbPlatform.TabIndex = 62;
            this.cmbPlatform.SelectedIndexChanged += new System.EventHandler(this.cmbPlatform_SelectedIndexChanged);
            // 
            // txtRS2014Path
            // 
            this.txtRS2014Path.BackColor = System.Drawing.SystemColors.Window;
            this.txtRS2014Path.Cursor = System.Windows.Forms.Cursors.No;
            this.txtRS2014Path.ForeColor = System.Drawing.Color.Gray;
            this.txtRS2014Path.Location = new System.Drawing.Point(21, 198);
            this.txtRS2014Path.Margin = new System.Windows.Forms.Padding(2);
            this.txtRS2014Path.Multiline = true;
            this.txtRS2014Path.Name = "txtRS2014Path";
            this.txtRS2014Path.ReadOnly = true;
            this.txtRS2014Path.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRS2014Path.Size = new System.Drawing.Size(399, 35);
            this.txtRS2014Path.TabIndex = 64;
            this.txtRS2014Path.TabStop = false;
            this.txtRS2014Path.Text = "Select the RS2014 Root Directory and CGM will put the new inlay in the correct fo" +
                "lder, or choose some other location to store the dlc/Inlay/*.psarc file";
            // 
            // btnRS2014Dir
            // 
            this.btnRS2014Dir.Location = new System.Drawing.Point(444, 205);
            this.btnRS2014Dir.Margin = new System.Windows.Forms.Padding(2);
            this.btnRS2014Dir.Name = "btnRS2014Dir";
            this.btnRS2014Dir.Size = new System.Drawing.Size(56, 20);
            this.btnRS2014Dir.TabIndex = 65;
            this.btnRS2014Dir.Text = "Select";
            this.btnRS2014Dir.UseVisualStyleBackColor = true;
            this.btnRS2014Dir.Click += new System.EventHandler(this.btnRS2014Dir_Click);
            // 
            // btnSaveCGM
            // 
            this.btnSaveCGM.Location = new System.Drawing.Point(380, 311);
            this.btnSaveCGM.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveCGM.Name = "btnSaveCGM";
            this.btnSaveCGM.Size = new System.Drawing.Size(56, 20);
            this.btnSaveCGM.TabIndex = 59;
            this.btnSaveCGM.Text = "Save";
            this.btnSaveCGM.UseVisualStyleBackColor = true;
            this.btnSaveCGM.Click += new System.EventHandler(this.btnSaveCGM_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 245);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 55;
            this.label4.Text = "Custom Guitar Inlays:";
            // 
            // btnLoadCGM
            // 
            this.btnLoadCGM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadCGM.Location = new System.Drawing.Point(380, 253);
            this.btnLoadCGM.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadCGM.Name = "btnLoadCGM";
            this.btnLoadCGM.Size = new System.Drawing.Size(56, 33);
            this.btnLoadCGM.TabIndex = 57;
            this.btnLoadCGM.Text = "Load";
            this.btnLoadCGM.UseVisualStyleBackColor = true;
            this.btnLoadCGM.Click += new System.EventHandler(this.btnLoadCGM_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreate.Location = new System.Drawing.Point(21, 481);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(159, 40);
            this.btnCreate.TabIndex = 53;
            this.btnCreate.Text = "Create Custom Inlay";
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // picGuitar
            // 
            this.picGuitar.Image = ((System.Drawing.Image)(resources.GetObject("picGuitar.Image")));
            this.picGuitar.InitialImage = ((System.Drawing.Image)(resources.GetObject("picGuitar.InitialImage")));
            this.picGuitar.Location = new System.Drawing.Point(23, 30);
            this.picGuitar.Name = "picGuitar";
            this.picGuitar.Size = new System.Drawing.Size(57, 70);
            this.picGuitar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picGuitar.TabIndex = 52;
            this.picGuitar.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 71;
            this.label5.Text = "Song:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(422, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 72;
            this.label7.Text = "AppID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 296);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 74;
            this.label3.Text = "CGM Creator Name:";
            // 
            // txtCreator
            // 
            this.txtCreator.ForeColor = System.Drawing.Color.Gray;
            this.txtCreator.Location = new System.Drawing.Point(21, 311);
            this.txtCreator.Margin = new System.Windows.Forms.Padding(2);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.Size = new System.Drawing.Size(131, 20);
            this.txtCreator.TabIndex = 73;
            this.txtCreator.Text = "CREATOR";
            this.txtCreator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(173, 296);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 76;
            this.label8.Text = "CGM Inlay Name:";
            // 
            // txtGuitar
            // 
            this.txtGuitar.ForeColor = System.Drawing.Color.Gray;
            this.txtGuitar.Location = new System.Drawing.Point(176, 311);
            this.txtGuitar.Margin = new System.Windows.Forms.Padding(2);
            this.txtGuitar.Name = "txtGuitar";
            this.txtGuitar.Size = new System.Drawing.Size(176, 20);
            this.txtGuitar.TabIndex = 75;
            this.txtGuitar.Text = "INLAY";
            this.txtGuitar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chkFlipX
            // 
            this.chkFlipX.AutoSize = true;
            this.chkFlipX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.chkFlipX.Location = new System.Drawing.Point(359, 359);
            this.chkFlipX.Name = "chkFlipX";
            this.chkFlipX.Size = new System.Drawing.Size(15, 14);
            this.chkFlipX.TabIndex = 77;
            this.chkFlipX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkFlipX.UseVisualStyleBackColor = true;
            // 
            // picFlipX
            // 
            this.picFlipX.Image = ((System.Drawing.Image)(resources.GetObject("picFlipX.Image")));
            this.picFlipX.Location = new System.Drawing.Point(380, 355);
            this.picFlipX.Name = "picFlipX";
            this.picFlipX.Size = new System.Drawing.Size(23, 22);
            this.picFlipX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFlipX.TabIndex = 78;
            this.picFlipX.TabStop = false;
            // 
            // picFlipY
            // 
            this.picFlipY.Image = ((System.Drawing.Image)(resources.GetObject("picFlipY.Image")));
            this.picFlipY.Location = new System.Drawing.Point(446, 355);
            this.picFlipY.Name = "picFlipY";
            this.picFlipY.Size = new System.Drawing.Size(22, 22);
            this.picFlipY.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFlipY.TabIndex = 80;
            this.picFlipY.TabStop = false;
            // 
            // chkFlipY
            // 
            this.chkFlipY.AutoSize = true;
            this.chkFlipY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.chkFlipY.Location = new System.Drawing.Point(425, 359);
            this.chkFlipY.Name = "chkFlipY";
            this.chkFlipY.Size = new System.Drawing.Size(15, 14);
            this.chkFlipY.TabIndex = 79;
            this.chkFlipY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkFlipY.UseVisualStyleBackColor = true;
            // 
            // chkColor
            // 
            this.chkColor.AutoSize = true;
            this.chkColor.Location = new System.Drawing.Point(254, 358);
            this.chkColor.Name = "chkColor";
            this.chkColor.Size = new System.Drawing.Size(87, 17);
            this.chkColor.TabIndex = 81;
            this.chkColor.Text = "Colored Inlay";
            this.chkColor.UseVisualStyleBackColor = true;
            // 
            // chk24Fret
            // 
            this.chk24Fret.AutoSize = true;
            this.chk24Fret.Location = new System.Drawing.Point(184, 358);
            this.chk24Fret.Name = "chk24Fret";
            this.chk24Fret.Size = new System.Drawing.Size(64, 17);
            this.chk24Fret.TabIndex = 82;
            this.chk24Fret.Text = "24 Frets";
            this.chk24Fret.UseVisualStyleBackColor = true;
            // 
            // txtOpenCGM
            // 
            this.txtOpenCGM.BackColor = System.Drawing.SystemColors.Window;
            this.txtOpenCGM.Cursor = System.Windows.Forms.Cursors.No;
            this.txtOpenCGM.ForeColor = System.Drawing.Color.Gray;
            this.txtOpenCGM.Location = new System.Drawing.Point(21, 260);
            this.txtOpenCGM.Margin = new System.Windows.Forms.Padding(2);
            this.txtOpenCGM.Name = "txtOpenCGM";
            this.txtOpenCGM.ReadOnly = true;
            this.txtOpenCGM.Size = new System.Drawing.Size(331, 20);
            this.txtOpenCGM.TabIndex = 56;
            this.txtOpenCGM.TabStop = false;
            this.txtOpenCGM.Text = "Open file *.cgm";
            // 
            // lblCredits1
            // 
            this.lblCredits1.AutoSize = true;
            this.lblCredits1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblCredits1.Font = new System.Drawing.Font("Magneto", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCredits1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblCredits1.Location = new System.Drawing.Point(187, 73);
            this.lblCredits1.Name = "lblCredits1";
            this.lblCredits1.Size = new System.Drawing.Size(213, 19);
            this.lblCredits1.TabIndex = 83;
            this.lblCredits1.Text = "Inspired by : Baoulettes";
            // 
            // lblCredits2
            // 
            this.lblCredits2.AutoSize = true;
            this.lblCredits2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblCredits2.Font = new System.Drawing.Font("Magneto", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCredits2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblCredits2.Location = new System.Drawing.Point(167, 95);
            this.lblCredits2.Name = "lblCredits2";
            this.lblCredits2.Size = new System.Drawing.Size(255, 17);
            this.lblCredits2.TabIndex = 84;
            this.lblCredits2.Text = "This CSC Toolkit addon by : Cozy1";
            // 
            // ctrlCGM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCredits2);
            this.Controls.Add(this.lblCredits1);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chk24Fret);
            this.Controls.Add(this.chkColor);
            this.Controls.Add(this.picFlipY);
            this.Controls.Add(this.chkFlipY);
            this.Controls.Add(this.picFlipX);
            this.Controls.Add(this.chkFlipX);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtGuitar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCreator);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtAppId);
            this.Controls.Add(this.picIcon);
            this.Controls.Add(this.cmbAppId);
            this.Controls.Add(this.picInlay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbPlatform);
            this.Controls.Add(this.txtRS2014Path);
            this.Controls.Add(this.btnRS2014Dir);
            this.Controls.Add(this.btnSaveCGM);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOpenCGM);
            this.Controls.Add(this.btnLoadCGM);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.picGuitar);
            this.Name = "ctrlCGM";
            this.Size = new System.Drawing.Size(517, 534);
            this.Load += new System.EventHandler(this.ctrlCGM_Load);
            this.Disposed += new System.EventHandler(this.ctrlCGM_Dispose);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGuitar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlipY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.TextBox txtAppId;
        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.ComboBox cmbAppId;
        private System.Windows.Forms.PictureBox picInlay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbPlatform;
        private System.Windows.Forms.TextBox txtRS2014Path;
        private System.Windows.Forms.Button btnRS2014Dir;
        private System.Windows.Forms.Button btnSaveCGM;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLoadCGM;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.PictureBox picGuitar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCreator;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtGuitar;
        private System.Windows.Forms.CheckBox chkFlipX;
        private System.Windows.Forms.PictureBox picFlipX;
        private System.Windows.Forms.PictureBox picFlipY;
        private System.Windows.Forms.CheckBox chkFlipY;
        private System.Windows.Forms.CheckBox chkColor;
        private System.Windows.Forms.CheckBox chk24Fret;
        private System.Windows.Forms.TextBox txtOpenCGM;
        private System.Windows.Forms.Label lblCredits1;
        private System.Windows.Forms.Label lblCredits2;

   }
}
