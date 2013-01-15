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
            this.repackButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.appIdCombo = new System.Windows.Forms.ComboBox();
            this.AppIdTB = new RocksmithTookitGUI.CueTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // useCryptographyCheckbox
            // 
            this.useCryptographyCheckbox.AutoSize = true;
            this.useCryptographyCheckbox.Checked = true;
            this.useCryptographyCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useCryptographyCheckbox.Location = new System.Drawing.Point(145, 20);
            this.useCryptographyCheckbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.useCryptographyCheckbox.Name = "useCryptographyCheckbox";
            this.useCryptographyCheckbox.Size = new System.Drawing.Size(142, 21);
            this.useCryptographyCheckbox.TabIndex = 10;
            this.useCryptographyCheckbox.Text = "Use cryptography";
            this.useCryptographyCheckbox.UseVisualStyleBackColor = true;
            // 
            // unpackButton
            // 
            this.unpackButton.Location = new System.Drawing.Point(145, 84);
            this.unpackButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.unpackButton.Name = "unpackButton";
            this.unpackButton.Size = new System.Drawing.Size(119, 28);
            this.unpackButton.TabIndex = 9;
            this.unpackButton.Text = "Unpack";
            this.unpackButton.UseVisualStyleBackColor = true;
            this.unpackButton.Click += new System.EventHandler(this.unpackButton_Click);
            // 
            // packButton
            // 
            this.packButton.Location = new System.Drawing.Point(145, 48);
            this.packButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.packButton.Name = "packButton";
            this.packButton.Size = new System.Drawing.Size(119, 28);
            this.packButton.TabIndex = 8;
            this.packButton.Text = "Pack";
            this.packButton.UseVisualStyleBackColor = true;
            this.packButton.Click += new System.EventHandler(this.packButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(4, 4);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(133, 127);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // repackButton
            // 
            this.repackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.repackButton.Location = new System.Drawing.Point(141, 52);
            this.repackButton.Margin = new System.Windows.Forms.Padding(4);
            this.repackButton.Name = "repackButton";
            this.repackButton.Size = new System.Drawing.Size(119, 28);
            this.repackButton.TabIndex = 12;
            this.repackButton.Text = "Choose DLC";
            this.repackButton.UseVisualStyleBackColor = true;
            this.repackButton.Click += new System.EventHandler(this.repackButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AppIdTB);
            this.groupBox1.Controls.Add(this.appIdCombo);
            this.groupBox1.Controls.Add(this.repackButton);
            this.groupBox1.Location = new System.Drawing.Point(4, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 87);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Update App ID";
            // 
            // appIdCombo
            // 
            this.appIdCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appIdCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appIdCombo.FormattingEnabled = true;
            this.appIdCombo.Location = new System.Drawing.Point(6, 21);
            this.appIdCombo.Name = "appIdCombo";
            this.appIdCombo.Size = new System.Drawing.Size(255, 24);
            this.appIdCombo.TabIndex = 13;
            this.appIdCombo.SelectedValueChanged += new System.EventHandler(this.cmbAppIds_SelectedValueChanged);
            // 
            // AppIdTB
            // 
            this.AppIdTB.Cue = "APP ID";
            this.AppIdTB.Location = new System.Drawing.Point(7, 55);
            this.AppIdTB.Margin = new System.Windows.Forms.Padding(4);
            this.AppIdTB.Name = "AppIdTB";
            this.AppIdTB.Size = new System.Drawing.Size(126, 22);
            this.AppIdTB.TabIndex = 41;
            // 
            // DLCPackerUnpacker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.useCryptographyCheckbox);
            this.Controls.Add(this.unpackButton);
            this.Controls.Add(this.packButton);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DLCPackerUnpacker";
            this.Size = new System.Drawing.Size(281, 236);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox useCryptographyCheckbox;
        private System.Windows.Forms.Button unpackButton;
        private System.Windows.Forms.Button packButton;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button repackButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox appIdCombo;
        private CueTextBox AppIdTB;
    }
}
