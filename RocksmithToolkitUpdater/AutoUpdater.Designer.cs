namespace RocksmithToolkitUpdater {
    partial class AutoUpdater {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoUpdater));
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.updateProgress = new System.Windows.Forms.ProgressBar();
            this.currentOperationLabel = new System.Windows.Forms.Label();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.labelDownloaded = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Image = global::RocksmithToolkitUpdater.Properties.Resources.guitar_256;
            this.pictureBoxIcon.Location = new System.Drawing.Point(12, 3);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(77, 71);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxIcon.TabIndex = 0;
            this.pictureBoxIcon.TabStop = false;
            // 
            // updateProgress
            // 
            this.updateProgress.Location = new System.Drawing.Point(99, 27);
            this.updateProgress.Name = "updateProgress";
            this.updateProgress.Size = new System.Drawing.Size(334, 26);
            this.updateProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.updateProgress.TabIndex = 1;
            // 
            // currentOperationLabel
            // 
            this.currentOperationLabel.AutoSize = true;
            this.currentOperationLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.currentOperationLabel.Location = new System.Drawing.Point(96, 8);
            this.currentOperationLabel.Name = "currentOperationLabel";
            this.currentOperationLabel.Size = new System.Drawing.Size(16, 13);
            this.currentOperationLabel.TabIndex = 2;
            this.currentOperationLabel.Text = "...";
            // 
            // labelSpeed
            // 
            this.labelSpeed.AutoSize = true;
            this.labelSpeed.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelSpeed.Location = new System.Drawing.Point(96, 58);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(16, 13);
            this.labelSpeed.TabIndex = 3;
            this.labelSpeed.Text = "...";
            // 
            // labelDownloaded
            // 
            this.labelDownloaded.AutoSize = true;
            this.labelDownloaded.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelDownloaded.Location = new System.Drawing.Point(215, 58);
            this.labelDownloaded.Name = "labelDownloaded";
            this.labelDownloaded.Size = new System.Drawing.Size(16, 13);
            this.labelDownloaded.TabIndex = 5;
            this.labelDownloaded.Text = "...";
            // 
            // AutoUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 76);
            this.Controls.Add(this.labelDownloaded);
            this.Controls.Add(this.labelSpeed);
            this.Controls.Add(this.currentOperationLabel);
            this.Controls.Add(this.updateProgress);
            this.Controls.Add(this.pictureBoxIcon);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoUpdater";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RocksmithToolkit Auto Update";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.ProgressBar updateProgress;
        private System.Windows.Forms.Label currentOperationLabel;
        private System.Windows.Forms.Label labelSpeed;
        private System.Windows.Forms.Label labelDownloaded;
    }
}

