namespace RocksmithToolkitGUI.SngToTabConverter
{
    partial class ReplaceDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.destinationFileName = new System.Windows.Forms.Label();
            this.yesButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.yesToAllButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.noToAllButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File already exists at destination: ";
            // 
            // destinationFileName
            // 
            this.destinationFileName.AutoSize = true;
            this.destinationFileName.Location = new System.Drawing.Point(12, 32);
            this.destinationFileName.Name = "destinationFileName";
            this.destinationFileName.Size = new System.Drawing.Size(0, 13);
            this.destinationFileName.TabIndex = 0;
            // 
            // yesButton
            // 
            this.yesButton.Location = new System.Drawing.Point(12, 72);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Do you want to replace the file?";
            // 
            // yesToAllButton
            // 
            this.yesToAllButton.Location = new System.Drawing.Point(93, 72);
            this.yesToAllButton.Name = "yesToAllButton";
            this.yesToAllButton.Size = new System.Drawing.Size(75, 23);
            this.yesToAllButton.TabIndex = 1;
            this.yesToAllButton.Text = "Yes to all";
            this.yesToAllButton.UseVisualStyleBackColor = true;
            this.yesToAllButton.Click += new System.EventHandler(this.yesToAllButton_Click);
            // 
            // noButton
            // 
            this.noButton.Location = new System.Drawing.Point(174, 72);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.TabIndex = 1;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            this.noButton.Click += new System.EventHandler(this.noButton_Click);
            // 
            // noToAllButton
            // 
            this.noToAllButton.Location = new System.Drawing.Point(255, 72);
            this.noToAllButton.Name = "noToAllButton";
            this.noToAllButton.Size = new System.Drawing.Size(75, 23);
            this.noToAllButton.TabIndex = 1;
            this.noToAllButton.Text = "No to all";
            this.noToAllButton.UseVisualStyleBackColor = true;
            this.noToAllButton.Click += new System.EventHandler(this.noToAllButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(336, 72);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ReplaceDialog
            // 
            this.AcceptButton = this.yesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(416, 103);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.noToAllButton);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesToAllButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.destinationFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReplaceDialog";
            this.Text = "Replace Files?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label destinationFileName;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button yesToAllButton;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.Button noToAllButton;
        private System.Windows.Forms.Button cancelButton;
    }
}