namespace RocksmithToolkitUpdater {
    partial class AutoUpdaterForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoUpdaterForm));
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.pbUpdate = new System.Windows.Forms.ProgressBar();
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblDownloaded = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pbIcon
            // 
            this.pbIcon.Image = global::RocksmithToolkitUpdater.Properties.Resources.guitar_256;
            this.pbIcon.Location = new System.Drawing.Point(12, 8);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(77, 71);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIcon.TabIndex = 0;
            this.pbIcon.TabStop = false;
            // 
            // pbUpdate
            // 
            this.pbUpdate.Location = new System.Drawing.Point(99, 27);
            this.pbUpdate.Name = "pbUpdate";
            this.pbUpdate.Size = new System.Drawing.Size(334, 26);
            this.pbUpdate.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbUpdate.TabIndex = 1;
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.AutoSize = true;
            this.lblCurrentOperation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCurrentOperation.Location = new System.Drawing.Point(96, 8);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.Size = new System.Drawing.Size(16, 13);
            this.lblCurrentOperation.TabIndex = 2;
            this.lblCurrentOperation.Text = "...";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSpeed.Location = new System.Drawing.Point(96, 58);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(16, 13);
            this.lblSpeed.TabIndex = 3;
            this.lblSpeed.Text = "...";
            // 
            // lblDownloaded
            // 
            this.lblDownloaded.AutoSize = true;
            this.lblDownloaded.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDownloaded.Location = new System.Drawing.Point(215, 58);
            this.lblDownloaded.Name = "lblDownloaded";
            this.lblDownloaded.Size = new System.Drawing.Size(16, 13);
            this.lblDownloaded.TabIndex = 5;
            this.lblDownloaded.Text = "...";
            // 
            // AutoUpdaterForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(445, 83);
            this.Controls.Add(this.lblDownloaded);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.lblCurrentOperation);
            this.Controls.Add(this.pbUpdate);
            this.Controls.Add(this.pbIcon);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoUpdaterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Song Creator Toolkit for Rocksmith - AutoUpdater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoUpdaterForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.ProgressBar pbUpdate;
        private System.Windows.Forms.Label lblCurrentOperation;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblDownloaded;
    }
}

