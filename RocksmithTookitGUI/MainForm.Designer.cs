namespace RocksmithToolkitGUI
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

        protected void LoadTemplate(string path)
        {
            this.dlcPackageCreatorControl.loadTemplate(path);
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
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.dlcPackageCreatorTab = new System.Windows.Forms.TabPage();
            this.dlcPackageCreatorControl = new RocksmithToolkitGUI.DLCPackageCreator.DLCPackageCreator();
            this.dlcPackerUnpackerTab = new System.Windows.Forms.TabPage();
            this.dlcPackerUnpackerControl = new RocksmithToolkitGUI.DLCPackerUnpacker.DLCPackerUnpacker();
            this.dlcConverterTab = new System.Windows.Forms.TabPage();
            this.dlcConverterControl = new RocksmithToolkitGUI.DLCConverter.DLCConverter();
            this.DDCtabControl = new System.Windows.Forms.TabPage();
            this.ddc1 = new RocksmithToolkitGUI.DDC.DDC();
            this.oggConverterTab = new System.Windows.Forms.TabPage();
            this.oggConverterControl = new RocksmithToolkitGUI.OggConverter.OggConverter();
            this.sngToTabConverterTab = new System.Windows.Forms.TabPage();
            this.sngToTabConverter1 = new RocksmithToolkitGUI.SngToTabConverter.SngToTabConverter();
            this.zigProConverterTab = new System.Windows.Forms.TabPage();
            this.convertInput1 = new RocksmithToolkitGUI.ZiggyProEditorConverter.ConvertInput();
            this.sngConverterTab = new System.Windows.Forms.TabPage();
            this.sngConverterControl = new RocksmithToolkitGUI.SngConverter.SngConverter();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.dlcPackageCreatorTab.SuspendLayout();
            this.dlcPackerUnpackerTab.SuspendLayout();
            this.dlcConverterTab.SuspendLayout();
            this.DDCtabControl.SuspendLayout();
            this.oggConverterTab.SuspendLayout();
            this.sngToTabConverterTab.SuspendLayout();
            this.zigProConverterTab.SuspendLayout();
            this.sngConverterTab.SuspendLayout();
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
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restartToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.dlcPackageCreatorTab);
            this.tabControl1.Controls.Add(this.dlcPackerUnpackerTab);
            this.tabControl1.Controls.Add(this.dlcConverterTab);
            this.tabControl1.Controls.Add(this.DDCtabControl);
            this.tabControl1.Controls.Add(this.sngConverterTab);
            this.tabControl1.Controls.Add(this.oggConverterTab);
            this.tabControl1.Controls.Add(this.sngToTabConverterTab);
            this.tabControl1.Controls.Add(this.zigProConverterTab);
            this.tabControl1.Location = new System.Drawing.Point(17, 93);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(8);
            this.tabControl1.MinimumSize = new System.Drawing.Size(550, 590);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(550, 590);
            this.tabControl1.TabIndex = 16;
            // 
            // dlcPackageCreatorTab
            // 
            this.dlcPackageCreatorTab.Controls.Add(this.dlcPackageCreatorControl);
            this.dlcPackageCreatorTab.Location = new System.Drawing.Point(4, 22);
            this.dlcPackageCreatorTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcPackageCreatorTab.Name = "dlcPackageCreatorTab";
            this.dlcPackageCreatorTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcPackageCreatorTab.Size = new System.Drawing.Size(542, 564);
            this.dlcPackageCreatorTab.TabIndex = 0;
            this.dlcPackageCreatorTab.Text = "Creator";
            this.dlcPackageCreatorTab.UseVisualStyleBackColor = true;
            // 
            // dlcPackageCreatorControl
            // 
            this.dlcPackageCreatorControl.Album = "";
            this.dlcPackageCreatorControl.AlbumYear = "";
            this.dlcPackageCreatorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dlcPackageCreatorControl.Location = new System.Drawing.Point(16, 6);
            this.dlcPackageCreatorControl.Margin = new System.Windows.Forms.Padding(4);
            this.dlcPackageCreatorControl.MinimumSize = new System.Drawing.Size(520, 555);
            this.dlcPackageCreatorControl.Name = "dlcPackageCreatorControl";
            this.dlcPackageCreatorControl.PackageVersion = "";
            this.dlcPackageCreatorControl.Size = new System.Drawing.Size(630, 600);
            this.dlcPackageCreatorControl.TabIndex = 0;
            // 
            // dlcPackerUnpackerTab
            // 
            this.dlcPackerUnpackerTab.Controls.Add(this.dlcPackerUnpackerControl);
            this.dlcPackerUnpackerTab.Location = new System.Drawing.Point(4, 22);
            this.dlcPackerUnpackerTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcPackerUnpackerTab.Name = "dlcPackerUnpackerTab";
            this.dlcPackerUnpackerTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcPackerUnpackerTab.Size = new System.Drawing.Size(542, 574);
            this.dlcPackerUnpackerTab.TabIndex = 1;
            this.dlcPackerUnpackerTab.Text = "Packer/Unpacker";
            this.dlcPackerUnpackerTab.UseVisualStyleBackColor = true;
            // 
            // dlcPackerUnpackerControl
            // 
            this.dlcPackerUnpackerControl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dlcPackerUnpackerControl.Location = new System.Drawing.Point(58, 60);
            this.dlcPackerUnpackerControl.Margin = new System.Windows.Forms.Padding(4);
            this.dlcPackerUnpackerControl.Name = "dlcPackerUnpackerControl";
            this.dlcPackerUnpackerControl.Size = new System.Drawing.Size(419, 574);
            this.dlcPackerUnpackerControl.TabIndex = 1;
            // 
            // dlcConverterTab
            // 
            this.dlcConverterTab.Controls.Add(this.dlcConverterControl);
            this.dlcConverterTab.Location = new System.Drawing.Point(4, 22);
            this.dlcConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.dlcConverterTab.Name = "dlcConverterTab";
            this.dlcConverterTab.Padding = new System.Windows.Forms.Padding(2);
            this.dlcConverterTab.Size = new System.Drawing.Size(542, 564);
            this.dlcConverterTab.TabIndex = 2;
            this.dlcConverterTab.Text = "Converter";
            this.dlcConverterTab.UseVisualStyleBackColor = true;
            // 
            // dlcConverterControl
            // 
            this.dlcConverterControl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dlcConverterControl.Location = new System.Drawing.Point(70, 30);
            this.dlcConverterControl.Margin = new System.Windows.Forms.Padding(4);
            this.dlcConverterControl.Name = "dlcConverterControl";
            this.dlcConverterControl.Size = new System.Drawing.Size(419, 270);
            this.dlcConverterControl.TabIndex = 2;
            // 
            // DDCtabControl
            // 
            this.DDCtabControl.Controls.Add(this.ddc1);
            this.DDCtabControl.Location = new System.Drawing.Point(4, 22);
            this.DDCtabControl.Name = "DDCtabControl";
            this.DDCtabControl.Padding = new System.Windows.Forms.Padding(3);
            this.DDCtabControl.Size = new System.Drawing.Size(542, 564);
            this.DDCtabControl.TabIndex = 3;
            this.DDCtabControl.Text = "DDC";
            this.DDCtabControl.ToolTipText = "Generator of low levels for arrangement.";
            this.DDCtabControl.UseVisualStyleBackColor = true;
            // 
            // ddc1
            // 
            this.ddc1.Location = new System.Drawing.Point(6, 6);
            this.ddc1.MinimumSize = new System.Drawing.Size(530, 380);
            this.ddc1.Name = "ddc1";
            this.ddc1.Size = new System.Drawing.Size(530, 380);
            this.ddc1.TabIndex = 3;
            // 
            // oggConverterTab
            // 
            this.oggConverterTab.Controls.Add(this.oggConverterControl);
            this.oggConverterTab.Location = new System.Drawing.Point(4, 22);
            this.oggConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.oggConverterTab.Name = "oggConverterTab";
            this.oggConverterTab.Padding = new System.Windows.Forms.Padding(2);
            this.oggConverterTab.Size = new System.Drawing.Size(542, 564);
            this.oggConverterTab.TabIndex = 5;
            this.oggConverterTab.Text = "OGG";
            this.oggConverterTab.UseVisualStyleBackColor = true;
            // 
            // oggConverterControl
            // 
            this.oggConverterControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oggConverterControl.Location = new System.Drawing.Point(18, 24);
            this.oggConverterControl.Margin = new System.Windows.Forms.Padding(4);
            this.oggConverterControl.MinimumSize = new System.Drawing.Size(498, 122);
            this.oggConverterControl.Name = "oggConverterControl";
            this.oggConverterControl.Size = new System.Drawing.Size(498, 204);
            this.oggConverterControl.TabIndex = 5;
            // 
            // sngToTabConverterTab
            // 
            this.sngToTabConverterTab.Controls.Add(this.sngToTabConverter1);
            this.sngToTabConverterTab.Location = new System.Drawing.Point(4, 22);
            this.sngToTabConverterTab.Name = "sngToTabConverterTab";
            this.sngToTabConverterTab.Padding = new System.Windows.Forms.Padding(3);
            this.sngToTabConverterTab.Size = new System.Drawing.Size(542, 564);
            this.sngToTabConverterTab.TabIndex = 6;
            this.sngToTabConverterTab.Text = "Sng2Tab";
            this.sngToTabConverterTab.UseVisualStyleBackColor = true;
            // 
            // sngToTabConverter1
            // 
            this.sngToTabConverter1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sngToTabConverter1.Location = new System.Drawing.Point(65, 20);
            this.sngToTabConverter1.Name = "sngToTabConverter1";
            this.sngToTabConverter1.Size = new System.Drawing.Size(522, 282);
            this.sngToTabConverter1.TabIndex = 6;
            // 
            // zigProConverterTab
            // 
            this.zigProConverterTab.Controls.Add(this.convertInput1);
            this.zigProConverterTab.Location = new System.Drawing.Point(4, 22);
            this.zigProConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.zigProConverterTab.Name = "zigProConverterTab";
            this.zigProConverterTab.Size = new System.Drawing.Size(542, 564);
            this.zigProConverterTab.TabIndex = 7;
            this.zigProConverterTab.Text = "Ziggy Pro";
            this.zigProConverterTab.UseVisualStyleBackColor = true;
            // 
            // convertInput1
            // 
            this.convertInput1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.convertInput1.Location = new System.Drawing.Point(18, 24);
            this.convertInput1.MinimumSize = new System.Drawing.Size(483, 111);
            this.convertInput1.Name = "convertInput1";
            this.convertInput1.Size = new System.Drawing.Size(483, 111);
            this.convertInput1.TabIndex = 7;
            // 
            // sngConverterTab
            // 
            this.sngConverterTab.Controls.Add(this.sngConverterControl);
            this.sngConverterTab.Location = new System.Drawing.Point(4, 22);
            this.sngConverterTab.Margin = new System.Windows.Forms.Padding(2);
            this.sngConverterTab.Name = "sng2014Tab";
            this.sngConverterTab.Padding = new System.Windows.Forms.Padding(2);
            this.sngConverterTab.Size = new System.Drawing.Size(542, 564);
            this.sngConverterTab.TabIndex = 4;
            this.sngConverterTab.Text = "SNG";
            this.sngConverterTab.UseVisualStyleBackColor = true;
            // 
            // sngConverterControl
            // 
            this.sngConverterControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sngConverterControl.Location = new System.Drawing.Point(18, 24);
            this.sngConverterControl.Margin = new System.Windows.Forms.Padding(4);
            this.sngConverterControl.MinimumSize = new System.Drawing.Size(494, 307);
            this.sngConverterControl.Name = "sngConverterControl";
            this.sngConverterControl.Size = new System.Drawing.Size(503, 307);
            this.sngConverterControl.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Image = global::RocksmithToolkitGUI.Properties.Resources.toolkit_logo;
            this.pictureBox1.Location = new System.Drawing.Point(79, 24);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(398, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(584, 694);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 688);
            this.Name = "MainForm";
            this.Text = "Custom Song Creator Toolkit";
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.dlcPackageCreatorTab.ResumeLayout(false);
            this.dlcPackerUnpackerTab.ResumeLayout(false);
            this.dlcConverterTab.ResumeLayout(false);
            this.DDCtabControl.ResumeLayout(false);
            this.oggConverterTab.ResumeLayout(false);
            this.sngToTabConverterTab.ResumeLayout(false);
            this.zigProConverterTab.ResumeLayout(false);
            this.sngConverterTab.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage sngConverterTab;
        private System.Windows.Forms.TabPage oggConverterTab;
        private SngConverter.SngConverter sngConverterControl;
        private OggConverter.OggConverter oggConverterControl;
        private System.Windows.Forms.TabPage dlcPackerUnpackerTab;
        private System.Windows.Forms.TabPage dlcConverterTab;
        private DLCConverter.DLCConverter dlcConverterControl;
        private DLCPackerUnpacker.DLCPackerUnpacker dlcPackerUnpackerControl;
        private DLCPackageCreator.DLCPackageCreator dlcPackageCreatorControl;
        private System.Windows.Forms.TabPage zigProConverterTab;
        private ZiggyProEditorConverter.ConvertInput convertInput1;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
		private System.Windows.Forms.TabPage sngToTabConverterTab;
        private SngToTabConverter.SngToTabConverter sngToTabConverter1;
        private System.Windows.Forms.TabPage DDCtabControl;
        private DDC.DDC ddc1;
    }
}

