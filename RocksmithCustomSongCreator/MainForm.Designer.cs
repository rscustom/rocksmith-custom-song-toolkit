namespace RocksmithSngCreator
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
            this.browseButton = new System.Windows.Forms.Button();
            this.inputXmlTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.outputFileTextBox = new System.Windows.Forms.TextBox();
            this.convertBtn = new System.Windows.Forms.Button();
            this.littleEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.bigEndianRadioBtn = new System.Windows.Forms.RadioButton();
            this.instrumentRadioButton = new System.Windows.Forms.RadioButton();
            this.vocalsRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(432, 181);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 0;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // inputXmlTextBox
            // 
            this.inputXmlTextBox.Location = new System.Drawing.Point(43, 182);
            this.inputXmlTextBox.Name = "inputXmlTextBox";
            this.inputXmlTextBox.ReadOnly = true;
            this.inputXmlTextBox.Size = new System.Drawing.Size(383, 22);
            this.inputXmlTextBox.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input XML File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 224);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output SNG File:";
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Location = new System.Drawing.Point(46, 247);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.ReadOnly = true;
            this.outputFileTextBox.Size = new System.Drawing.Size(383, 22);
            this.outputFileTextBox.TabIndex = 4;
            // 
            // convertBtn
            // 
            this.convertBtn.Location = new System.Drawing.Point(240, 501);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(75, 23);
            this.convertBtn.TabIndex = 5;
            this.convertBtn.Text = "Convert";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // littleEndianRadioBtn
            // 
            this.littleEndianRadioBtn.AutoSize = true;
            this.littleEndianRadioBtn.Checked = true;
            this.littleEndianRadioBtn.Location = new System.Drawing.Point(44, 35);
            this.littleEndianRadioBtn.Name = "littleEndianRadioBtn";
            this.littleEndianRadioBtn.Size = new System.Drawing.Size(139, 21);
            this.littleEndianRadioBtn.TabIndex = 6;
            this.littleEndianRadioBtn.TabStop = true;
            this.littleEndianRadioBtn.Text = "PC (Little Endian)";
            this.littleEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // bigEndianRadioBtn
            // 
            this.bigEndianRadioBtn.AutoSize = true;
            this.bigEndianRadioBtn.Location = new System.Drawing.Point(232, 35);
            this.bigEndianRadioBtn.Name = "bigEndianRadioBtn";
            this.bigEndianRadioBtn.Size = new System.Drawing.Size(204, 21);
            this.bigEndianRadioBtn.TabIndex = 7;
            this.bigEndianRadioBtn.Text = "Game Console (Big Endian)";
            this.bigEndianRadioBtn.UseVisualStyleBackColor = true;
            // 
            // instrumentRadioButton
            // 
            this.instrumentRadioButton.AutoSize = true;
            this.instrumentRadioButton.Checked = true;
            this.instrumentRadioButton.Location = new System.Drawing.Point(47, 35);
            this.instrumentRadioButton.Name = "instrumentRadioButton";
            this.instrumentRadioButton.Size = new System.Drawing.Size(95, 21);
            this.instrumentRadioButton.TabIndex = 10;
            this.instrumentRadioButton.TabStop = true;
            this.instrumentRadioButton.Text = "Instrument";
            this.instrumentRadioButton.UseVisualStyleBackColor = true;
            // 
            // vocalsRadioButton
            // 
            this.vocalsRadioButton.AutoSize = true;
            this.vocalsRadioButton.Location = new System.Drawing.Point(235, 35);
            this.vocalsRadioButton.Name = "vocalsRadioButton";
            this.vocalsRadioButton.Size = new System.Drawing.Size(120, 21);
            this.vocalsRadioButton.TabIndex = 9;
            this.vocalsRadioButton.Text = "Vocals / Lyrics";
            this.vocalsRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.vocalsRadioButton);
            this.groupBox1.Controls.Add(this.instrumentRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(43, 294);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 79);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input File Type";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.littleEndianRadioBtn);
            this.groupBox2.Controls.Add(this.bigEndianRadioBtn);
            this.groupBox2.Location = new System.Drawing.Point(46, 395);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(464, 79);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Platform";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(558, 28);
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
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RocksmithSngCreator.Properties.Resources.logo_small;
            this.pictureBox1.Location = new System.Drawing.Point(16, 46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(530, 88);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 541);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.outputFileTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputXmlTextBox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Rocksmith .SNG File Creator (v0.2 alpha)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox inputXmlTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.RadioButton littleEndianRadioBtn;
        private System.Windows.Forms.RadioButton bigEndianRadioBtn;
        private System.Windows.Forms.RadioButton instrumentRadioButton;
        private System.Windows.Forms.RadioButton vocalsRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
    }
}

