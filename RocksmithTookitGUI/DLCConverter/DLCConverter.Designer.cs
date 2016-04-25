using System.Windows.Forms;

namespace RocksmithToolkitGUI.DLCConverter
{
    partial class DLCConverter
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
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblAppId = new System.Windows.Forms.Label();
            this.txtAppId = new System.Windows.Forms.TextBox();
            this.cmbAppId = new System.Windows.Forms.ComboBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbTargetPlatform = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSourcePlatform = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.pbUpdateProgress = new System.Windows.Forms.ProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picLogo
            // 
            this.picLogo.Image = global::RocksmithToolkitGUI.Properties.Resources.brasil_logo;
            this.picLogo.Location = new System.Drawing.Point(8, 12);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(75, 75);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 11;
            this.picLogo.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblAppId);
            this.groupBox1.Controls.Add(this.txtAppId);
            this.groupBox1.Controls.Add(this.cmbAppId);
            this.groupBox1.Controls.Add(this.btnConvert);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbTargetPlatform);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbSourcePlatform);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(7, 93);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(382, 140);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Platform";
            // 
            // lblAppId
            // 
            this.lblAppId.AutoSize = true;
            this.lblAppId.ForeColor = System.Drawing.Color.Black;
            this.lblAppId.Location = new System.Drawing.Point(143, 110);
            this.lblAppId.Name = "lblAppId";
            this.lblAppId.Size = new System.Drawing.Size(142, 13);
            this.lblAppId.TabIndex = 51;
            this.lblAppId.Text = "Enter a Custom App ID here:";
            // 
            // txtAppId
            // 
            this.txtAppId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtAppId.ForeColor = System.Drawing.Color.Gray;
            this.txtAppId.Location = new System.Drawing.Point(291, 107);
            this.txtAppId.Name = "txtAppId";
            this.txtAppId.Size = new System.Drawing.Size(82, 20);
            this.txtAppId.TabIndex = 50;
            this.toolTip.SetToolTip(this.txtAppId, "Specify any valid App ID\r\nby typing it into this box");
            this.txtAppId.Validating += new System.ComponentModel.CancelEventHandler(this.txtAppId_Validating);
            // 
            // cmbAppId
            // 
            this.cmbAppId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppId.FormattingEnabled = true;
            this.cmbAppId.Location = new System.Drawing.Point(54, 77);
            this.cmbAppId.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAppId.Name = "cmbAppId";
            this.cmbAppId.Size = new System.Drawing.Size(319, 21);
            this.cmbAppId.TabIndex = 2;
            this.cmbAppId.SelectedIndexChanged += new System.EventHandler(this.cmbAppId_SelectedIndexChanged);
            // 
            // btnConvert
            // 
            this.btnConvert.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConvert.ForeColor = System.Drawing.Color.Black;
            this.btnConvert.Location = new System.Drawing.Point(207, 25);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(166, 33);
            this.btnConvert.TabIndex = 3;
            this.btnConvert.Text = "Choose CDLC and Convert";
            this.btnConvert.UseVisualStyleBackColor = false;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(9, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Target:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbTargetPlatform
            // 
            this.cmbTargetPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetPlatform.FormattingEnabled = true;
            this.cmbTargetPlatform.Location = new System.Drawing.Point(54, 47);
            this.cmbTargetPlatform.Margin = new System.Windows.Forms.Padding(2);
            this.cmbTargetPlatform.Name = "cmbTargetPlatform";
            this.cmbTargetPlatform.Size = new System.Drawing.Size(108, 21);
            this.cmbTargetPlatform.TabIndex = 1;
            this.cmbTargetPlatform.SelectedIndexChanged += new System.EventHandler(this.platformTargetCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "Source:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbSourcePlatform
            // 
            this.cmbSourcePlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourcePlatform.FormattingEnabled = true;
            this.cmbSourcePlatform.Location = new System.Drawing.Point(54, 17);
            this.cmbSourcePlatform.Margin = new System.Windows.Forms.Padding(2);
            this.cmbSourcePlatform.Name = "cmbSourcePlatform";
            this.cmbSourcePlatform.Size = new System.Drawing.Size(108, 21);
            this.cmbSourcePlatform.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Location = new System.Drawing.Point(111, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(249, 15);
            this.label3.TabIndex = 38;
            this.label3.Text = "Only Compatible with Rocksmith 2014";
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.AutoSize = true;
            this.lblCurrentOperation.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblCurrentOperation.Location = new System.Drawing.Point(16, 243);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.Size = new System.Drawing.Size(16, 13);
            this.lblCurrentOperation.TabIndex = 19;
            this.lblCurrentOperation.Text = "...";
            this.lblCurrentOperation.Visible = false;
            // 
            // pbUpdateProgress
            // 
            this.pbUpdateProgress.Location = new System.Drawing.Point(19, 262);
            this.pbUpdateProgress.Name = "pbUpdateProgress";
            this.pbUpdateProgress.Size = new System.Drawing.Size(361, 20);
            this.pbUpdateProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbUpdateProgress.TabIndex = 0;
            this.pbUpdateProgress.Visible = false;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 9000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // DLCConverter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lblCurrentOperation);
            this.Controls.Add(this.pbUpdateProgress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.picLogo);
            this.MinimumSize = new System.Drawing.Size(400, 279);
            this.Name = "DLCConverter";
            this.Size = new System.Drawing.Size(400, 302);
            this.Load += new System.EventHandler(this.DLCConverter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTargetPlatform;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSourcePlatform;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.ComboBox cmbAppId;
        private System.Windows.Forms.TextBox txtAppId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCurrentOperation;
        private System.Windows.Forms.ProgressBar pbUpdateProgress;
        private System.Windows.Forms.Label lblAppId;
        private ToolTip toolTip;
    }
}
