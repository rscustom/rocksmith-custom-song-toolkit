namespace RocksmithDLCPackager
{
    partial class PackerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackerForm));
            this.PackButton = new System.Windows.Forms.Button();
            this.UnpackButton = new System.Windows.Forms.Button();
            this.UseCryptographyCheckbox = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // PackButton
            // 
            this.PackButton.Location = new System.Drawing.Point(119, 56);
            this.PackButton.Name = "PackButton";
            this.PackButton.Size = new System.Drawing.Size(75, 23);
            this.PackButton.TabIndex = 0;
            this.PackButton.Text = "Pack";
            this.PackButton.UseVisualStyleBackColor = true;
            this.PackButton.Click += new System.EventHandler(this.PackButton_Click);
            // 
            // UnpackButton
            // 
            this.UnpackButton.Location = new System.Drawing.Point(119, 85);
            this.UnpackButton.Name = "UnpackButton";
            this.UnpackButton.Size = new System.Drawing.Size(75, 23);
            this.UnpackButton.TabIndex = 1;
            this.UnpackButton.Text = "Unpack";
            this.UnpackButton.UseVisualStyleBackColor = true;
            this.UnpackButton.Click += new System.EventHandler(this.UnpackButton_Click);
            // 
            // UseCryptographyCheckbox
            // 
            this.UseCryptographyCheckbox.AutoSize = true;
            this.UseCryptographyCheckbox.Checked = true;
            this.UseCryptographyCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseCryptographyCheckbox.Location = new System.Drawing.Point(13, 13);
            this.UseCryptographyCheckbox.Name = "UseCryptographyCheckbox";
            this.UseCryptographyCheckbox.Size = new System.Drawing.Size(105, 17);
            this.UseCryptographyCheckbox.TabIndex = 2;
            this.UseCryptographyCheckbox.Text = "Use cryptography";
            this.UseCryptographyCheckbox.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 36);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 103);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // PackerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(209, 151);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.UseCryptographyCheckbox);
            this.Controls.Add(this.UnpackButton);
            this.Controls.Add(this.PackButton);
            this.Name = "PackerForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button PackButton;
        private System.Windows.Forms.Button UnpackButton;
        private System.Windows.Forms.CheckBox UseCryptographyCheckbox;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

