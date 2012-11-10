namespace RocksmithTookitGUI.DLCPackerUnpacker
{
    partial class DLCPackerUnpacker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DLCPackerUnpacker));
            this.useCryptographyCheckbox = new System.Windows.Forms.CheckBox();
            this.unpackButton = new System.Windows.Forms.Button();
            this.packButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // useCryptographyCheckbox
            // 
            this.useCryptographyCheckbox.AutoSize = true;
            this.useCryptographyCheckbox.Checked = true;
            this.useCryptographyCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useCryptographyCheckbox.Location = new System.Drawing.Point(109, 16);
            this.useCryptographyCheckbox.Name = "useCryptographyCheckbox";
            this.useCryptographyCheckbox.Size = new System.Drawing.Size(109, 17);
            this.useCryptographyCheckbox.TabIndex = 10;
            this.useCryptographyCheckbox.Text = "Use cryptography";
            this.useCryptographyCheckbox.UseVisualStyleBackColor = true;
            // 
            // unpackButton
            // 
            this.unpackButton.Location = new System.Drawing.Point(109, 68);
            this.unpackButton.Name = "unpackButton";
            this.unpackButton.Size = new System.Drawing.Size(75, 23);
            this.unpackButton.TabIndex = 9;
            this.unpackButton.Text = "Unpack";
            this.unpackButton.UseVisualStyleBackColor = true;
            this.unpackButton.Click += new System.EventHandler(this.unpackButton_Click);
            // 
            // packButton
            // 
            this.packButton.Location = new System.Drawing.Point(109, 39);
            this.packButton.Name = "packButton";
            this.packButton.Size = new System.Drawing.Size(75, 23);
            this.packButton.TabIndex = 8;
            this.packButton.Text = "Pack";
            this.packButton.UseVisualStyleBackColor = true;
            this.packButton.Click += new System.EventHandler(this.packButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 103);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // DLCPackerUnpacker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.useCryptographyCheckbox);
            this.Controls.Add(this.unpackButton);
            this.Controls.Add(this.packButton);
            this.Name = "DLCPackerUnpacker";
            this.Size = new System.Drawing.Size(217, 110);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox useCryptographyCheckbox;
        private System.Windows.Forms.Button unpackButton;
        private System.Windows.Forms.Button packButton;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
