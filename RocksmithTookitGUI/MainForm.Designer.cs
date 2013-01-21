namespace RocksmithTookitGUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.sngFileCreatorTab = new System.Windows.Forms.TabPage();
            this.sngFileCreatorControl = new RocksmithTookitGUI.SngFileCreator.SngFileCreator();
            this.oggConverterTab = new System.Windows.Forms.TabPage();
            this.oggConverterControl = new RocksmithTookitGUI.OggConverter.OggConverter();
            this.dlcPackageCreatorTab = new System.Windows.Forms.TabPage();
            this.dlcPackageCreatorControl = new RocksmithTookitGUI.DLCPackageCreator.DLCPackageCreator();
            this.dlcPackerUnpackerTab = new System.Windows.Forms.TabPage();
            this.dlcPackerUnpackerControl = new RocksmithTookitGUI.DLCPackerUnpacker.DLCPackerUnpacker();
            this.zigProConverterTab = new System.Windows.Forms.TabPage();
            this.convertInput1 = new RocksmithTookitGUI.ZiggyProEditorConverter.ConvertInput();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.sngFileCreatorTab.SuspendLayout();
            this.oggConverterTab.SuspendLayout();
            this.dlcPackageCreatorTab.SuspendLayout();
            this.dlcPackerUnpackerTab.SuspendLayout();
            this.zigProConverterTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(749, 28);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(102, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(119, 24);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(119, 24);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.sngFileCreatorTab);
            this.tabControl1.Controls.Add(this.oggConverterTab);
            this.tabControl1.Controls.Add(this.dlcPackageCreatorTab);
            this.tabControl1.Controls.Add(this.dlcPackerUnpackerTab);
            this.tabControl1.Controls.Add(this.zigProConverterTab);
            this.tabControl1.Location = new System.Drawing.Point(23, 154);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(704, 567);
            this.tabControl1.TabIndex = 16;
            // 
            // sngFileCreatorTab
            // 
            this.sngFileCreatorTab.Controls.Add(this.sngFileCreatorControl);
            this.sngFileCreatorTab.Location = new System.Drawing.Point(4, 25);
            this.sngFileCreatorTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sngFileCreatorTab.Name = "sngFileCreatorTab";
            this.sngFileCreatorTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sngFileCreatorTab.Size = new System.Drawing.Size(696, 538);
            this.sngFileCreatorTab.TabIndex = 1;
            this.sngFileCreatorTab.Text = "SNG File Creator";
            this.sngFileCreatorTab.UseVisualStyleBackColor = true;
            // 
            // sngFileCreatorControl
            // 
            this.sngFileCreatorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sngFileCreatorControl.Location = new System.Drawing.Point(24, 30);
            this.sngFileCreatorControl.Margin = new System.Windows.Forms.Padding(5);
            this.sngFileCreatorControl.Name = "sngFileCreatorControl";
            this.sngFileCreatorControl.Size = new System.Drawing.Size(644, 378);
            this.sngFileCreatorControl.TabIndex = 0;
            // 
            // oggConverterTab
            // 
            this.oggConverterTab.Controls.Add(this.oggConverterControl);
            this.oggConverterTab.Location = new System.Drawing.Point(4, 25);
            this.oggConverterTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.oggConverterTab.Name = "oggConverterTab";
            this.oggConverterTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.oggConverterTab.Size = new System.Drawing.Size(696, 538);
            this.oggConverterTab.TabIndex = 2;
            this.oggConverterTab.Text = "OGG Converter";
            this.oggConverterTab.UseVisualStyleBackColor = true;
            // 
            // oggConverterControl
            // 
            this.oggConverterControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oggConverterControl.Location = new System.Drawing.Point(24, 30);
            this.oggConverterControl.Margin = new System.Windows.Forms.Padding(5);
            this.oggConverterControl.Name = "oggConverterControl";
            this.oggConverterControl.Size = new System.Drawing.Size(143, 142);
            this.oggConverterControl.TabIndex = 0;
            // 
            // dlcPackageCreatorTab
            // 
            this.dlcPackageCreatorTab.Controls.Add(this.dlcPackageCreatorControl);
            this.dlcPackageCreatorTab.Location = new System.Drawing.Point(4, 25);
            this.dlcPackageCreatorTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dlcPackageCreatorTab.Name = "dlcPackageCreatorTab";
            this.dlcPackageCreatorTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dlcPackageCreatorTab.Size = new System.Drawing.Size(696, 538);
            this.dlcPackageCreatorTab.TabIndex = 0;
            this.dlcPackageCreatorTab.Text = "DLC Package Creator";
            this.dlcPackageCreatorTab.UseVisualStyleBackColor = true;
            // 
            // dlcPackageCreatorControl
            // 
            this.dlcPackageCreatorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dlcPackageCreatorControl.Location = new System.Drawing.Point(8, 7);
            this.dlcPackageCreatorControl.Margin = new System.Windows.Forms.Padding(5);
            this.dlcPackageCreatorControl.Name = "dlcPackageCreatorControl";
            this.dlcPackageCreatorControl.Size = new System.Drawing.Size(176, 57);
            this.dlcPackageCreatorControl.TabIndex = 0;
            // 
            // dlcPackerUnpackerTab
            // 
            this.dlcPackerUnpackerTab.Controls.Add(this.dlcPackerUnpackerControl);
            this.dlcPackerUnpackerTab.Location = new System.Drawing.Point(4, 25);
            this.dlcPackerUnpackerTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dlcPackerUnpackerTab.Name = "dlcPackerUnpackerTab";
            this.dlcPackerUnpackerTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dlcPackerUnpackerTab.Size = new System.Drawing.Size(696, 538);
            this.dlcPackerUnpackerTab.TabIndex = 3;
            this.dlcPackerUnpackerTab.Text = "DLC Packer/Unpacker";
            this.dlcPackerUnpackerTab.UseVisualStyleBackColor = true;
            // 
            // dlcPackerUnpackerControl
            // 
            this.dlcPackerUnpackerControl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dlcPackerUnpackerControl.Location = new System.Drawing.Point(-68, 33);
            this.dlcPackerUnpackerControl.Margin = new System.Windows.Forms.Padding(5);
            this.dlcPackerUnpackerControl.Name = "dlcPackerUnpackerControl";
            this.dlcPackerUnpackerControl.Size = new System.Drawing.Size(292, 251);
            this.dlcPackerUnpackerControl.TabIndex = 0;
            // 
            // zigProConverterTab
            // 
            this.zigProConverterTab.Controls.Add(this.convertInput1);
            this.zigProConverterTab.Location = new System.Drawing.Point(4, 25);
            this.zigProConverterTab.Name = "zigProConverterTab";
            this.zigProConverterTab.Size = new System.Drawing.Size(696, 538);
            this.zigProConverterTab.TabIndex = 4;
            this.zigProConverterTab.Text = "Ziggy Pro Editor Converter";
            this.zigProConverterTab.UseVisualStyleBackColor = true;
            // 
            // convertInput1
            // 
            this.convertInput1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.convertInput1.Location = new System.Drawing.Point(24, 30);
            this.convertInput1.Margin = new System.Windows.Forms.Padding(4);
            this.convertInput1.Name = "convertInput1";
            this.convertInput1.Size = new System.Drawing.Size(133, 137);
            this.convertInput1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Image = global::RocksmithTookitGUI.Properties.Resources.toolkit_logo;
            this.pictureBox1.Location = new System.Drawing.Point(105, 39);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(531, 95);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 742);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Custom Song Creator Toolkit";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.sngFileCreatorTab.ResumeLayout(false);
            this.oggConverterTab.ResumeLayout(false);
            this.dlcPackageCreatorTab.ResumeLayout(false);
            this.dlcPackerUnpackerTab.ResumeLayout(false);
            this.zigProConverterTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage dlcPackageCreatorTab;
        private System.Windows.Forms.TabPage sngFileCreatorTab;
        private System.Windows.Forms.TabPage oggConverterTab;
        private SngFileCreator.SngFileCreator sngFileCreatorControl;
        private OggConverter.OggConverter oggConverterControl;
        private System.Windows.Forms.TabPage dlcPackerUnpackerTab;
        private DLCPackerUnpacker.DLCPackerUnpacker dlcPackerUnpackerControl;
        private DLCPackageCreator.DLCPackageCreator dlcPackageCreatorControl;
        private System.Windows.Forms.TabPage zigProConverterTab;
        private ZiggyProEditorConverter.ConvertInput convertInput1;
    }
}

