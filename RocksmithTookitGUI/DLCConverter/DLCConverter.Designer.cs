namespace RocksmithToolkitGUI.DLCConverter
{
    partial class DLCConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DLCConverter));
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AppIdTB = new RocksmithToolkitGUI.CueTextBox();
            this.appIdCombo = new System.Windows.Forms.ComboBox();
            this.audioPathTB = new RocksmithToolkitGUI.CueTextBox();
            this.openAudioButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.platformTargetCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.platformSourceCombo = new System.Windows.Forms.ComboBox();
            this.convertButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(158, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 103);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.AppIdTB);
            this.groupBox1.Controls.Add(this.appIdCombo);
            this.groupBox1.Controls.Add(this.audioPathTB);
            this.groupBox1.Controls.Add(this.openAudioButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.platformTargetCombo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.platformSourceCombo);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(3, 111);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(395, 108);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Platform";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Location = new System.Drawing.Point(6, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(346, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Song preview must have the same file name with \"_preview\" in the end.";
            // 
            // AppIdTB
            // 
            this.AppIdTB.Cue = "APP ID";
            this.AppIdTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AppIdTB.ForeColor = System.Drawing.Color.Gray;
            this.AppIdTB.Location = new System.Drawing.Point(9, 83);
            this.AppIdTB.Name = "AppIdTB";
            this.AppIdTB.Size = new System.Drawing.Size(74, 20);
            this.AppIdTB.TabIndex = 50;
            // 
            // appIdCombo
            // 
            this.appIdCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appIdCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.appIdCombo.FormattingEnabled = true;
            this.appIdCombo.Location = new System.Drawing.Point(88, 83);
            this.appIdCombo.Margin = new System.Windows.Forms.Padding(2);
            this.appIdCombo.Name = "appIdCombo";
            this.appIdCombo.Size = new System.Drawing.Size(301, 21);
            this.appIdCombo.TabIndex = 49;
            this.appIdCombo.SelectedIndexChanged += new System.EventHandler(this.appIdCombo_SelectedIndexChanged);
            // 
            // audioPathTB
            // 
            this.audioPathTB.Cue = "Converted audio on Wwise 2013 for Target Platform (*.wem)";
            this.audioPathTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.audioPathTB.ForeColor = System.Drawing.Color.Gray;
            this.audioPathTB.Location = new System.Drawing.Point(9, 43);
            this.audioPathTB.Name = "audioPathTB";
            this.audioPathTB.Size = new System.Drawing.Size(341, 20);
            this.audioPathTB.TabIndex = 47;
            // 
            // openAudioButton
            // 
            this.openAudioButton.Location = new System.Drawing.Point(356, 41);
            this.openAudioButton.Name = "openAudioButton";
            this.openAudioButton.Size = new System.Drawing.Size(34, 23);
            this.openAudioButton.TabIndex = 48;
            this.openAudioButton.Text = "...";
            this.openAudioButton.UseVisualStyleBackColor = true;
            this.openAudioButton.Click += new System.EventHandler(this.openAudioButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(214, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Target:";
            // 
            // platformTargetCombo
            // 
            this.platformTargetCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.platformTargetCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.platformTargetCombo.FormattingEnabled = true;
            this.platformTargetCombo.Location = new System.Drawing.Point(259, 17);
            this.platformTargetCombo.Margin = new System.Windows.Forms.Padding(2);
            this.platformTargetCombo.Name = "platformTargetCombo";
            this.platformTargetCombo.Size = new System.Drawing.Size(130, 21);
            this.platformTargetCombo.TabIndex = 45;
            this.platformTargetCombo.SelectedIndexChanged += new System.EventHandler(this.platformTargetCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(5, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "Source:";
            // 
            // platformSourceCombo
            // 
            this.platformSourceCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.platformSourceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.platformSourceCombo.FormattingEnabled = true;
            this.platformSourceCombo.Location = new System.Drawing.Point(54, 17);
            this.platformSourceCombo.Margin = new System.Windows.Forms.Padding(2);
            this.platformSourceCombo.Name = "platformSourceCombo";
            this.platformSourceCombo.Size = new System.Drawing.Size(130, 21);
            this.platformSourceCombo.TabIndex = 43;
            this.platformSourceCombo.SelectedIndexChanged += new System.EventHandler(this.platformSourceCombo_SelectedIndexChanged);
            // 
            // convertButton
            // 
            this.convertButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.convertButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.convertButton.Location = new System.Drawing.Point(240, 224);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(159, 29);
            this.convertButton.TabIndex = 32;
            this.convertButton.Text = "Choose DLC to Convert";
            this.convertButton.UseVisualStyleBackColor = false;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // DLCConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox2);
            this.Name = "DLCConverter";
            this.Size = new System.Drawing.Size(400, 259);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox platformTargetCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox platformSourceCombo;
        private CueTextBox audioPathTB;
        private System.Windows.Forms.Button openAudioButton;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.ComboBox appIdCombo;
        private CueTextBox AppIdTB;
        private System.Windows.Forms.Label label3;
    }
}
