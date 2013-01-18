namespace RocksmithTookitGUI.DLCPackageCreator
{
    partial class ToneForm
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
            this.toneControl1 = new RocksmithTookitGUI.DLCPackageCreator.ToneControl();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toneControl1
            // 
            this.toneControl1.Location = new System.Drawing.Point(13, 13);
            this.toneControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toneControl1.Name = "toneControl1";
            this.toneControl1.Size = new System.Drawing.Size(776, 267);
            this.toneControl1.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okButton.AutoSize = true;
            this.okButton.Location = new System.Drawing.Point(305, 288);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 27);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ToneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 327);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.toneControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToneForm";
            this.Text = "Edit Tone";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToneControl toneControl1;
        private System.Windows.Forms.Button okButton;
    }
}