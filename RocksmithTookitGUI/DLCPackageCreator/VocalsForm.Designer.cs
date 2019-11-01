using System.Windows.Forms;
namespace RocksmithToolkitGUI.DLCPackageCreator
{
    partial class VocalsForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtVocalsDdsPath = new RocksmithToolkitGUI.CueTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnVocalsDdsPath = new System.Windows.Forms.Button();
            this.rtbVocals = new System.Windows.Forms.RichTextBox();
            this.rtbBlank = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnVocalsXmlPath = new System.Windows.Forms.Button();
            this.txtVocalsXmlPath = new RocksmithToolkitGUI.CueTextBox();
            this.lblKey = new System.Windows.Forms.Label();
            this.lstKey = new System.Windows.Forms.ListBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnRemoveFont = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnOk.Location = new System.Drawing.Point(683, 372);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(585, 372);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtVocalsDdsPath
            // 
            this.txtVocalsDdsPath.Cue = "Select custom lyrics font DDS file that is accompanied by a .glyphs.xml file";
            this.txtVocalsDdsPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVocalsDdsPath.ForeColor = System.Drawing.Color.Gray;
            this.txtVocalsDdsPath.Location = new System.Drawing.Point(10, 69);
            this.txtVocalsDdsPath.Multiline = true;
            this.txtVocalsDdsPath.Name = "txtVocalsDdsPath";
            this.txtVocalsDdsPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVocalsDdsPath.Size = new System.Drawing.Size(510, 20);
            this.txtVocalsDdsPath.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Custom Lyrics Font DDS Glyph Texture:";
            // 
            // btnVocalsDdsPath
            // 
            this.btnVocalsDdsPath.Location = new System.Drawing.Point(526, 68);
            this.btnVocalsDdsPath.Name = "btnVocalsDdsPath";
            this.btnVocalsDdsPath.Size = new System.Drawing.Size(31, 20);
            this.btnVocalsDdsPath.TabIndex = 6;
            this.btnVocalsDdsPath.Text = "...";
            this.toolTip.SetToolTip(this.btnVocalsDdsPath, "Browse for a custom font");
            this.btnVocalsDdsPath.UseVisualStyleBackColor = true;
            this.btnVocalsDdsPath.Click += new System.EventHandler(this.btnVocalsDdsPath_Click);
            // 
            // rtbVocals
            // 
            this.rtbVocals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbVocals.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbVocals.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbVocals.Location = new System.Drawing.Point(21, 118);
            this.rtbVocals.Name = "rtbVocals";
            this.rtbVocals.Size = new System.Drawing.Size(525, 264);
            this.rtbVocals.TabIndex = 32;
            this.rtbVocals.Text = "";
            this.rtbVocals.WordWrap = false;
            // 
            // rtbBlank
            // 
            this.rtbBlank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbBlank.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.rtbBlank.Location = new System.Drawing.Point(10, 106);
            this.rtbBlank.Name = "rtbBlank";
            this.rtbBlank.Size = new System.Drawing.Size(547, 289);
            this.rtbBlank.TabIndex = 33;
            this.rtbBlank.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "Vocals XML File Path:";
            // 
            // btnVocalsXmlPath
            // 
            this.btnVocalsXmlPath.Location = new System.Drawing.Point(526, 22);
            this.btnVocalsXmlPath.Name = "btnVocalsXmlPath";
            this.btnVocalsXmlPath.Size = new System.Drawing.Size(31, 20);
            this.btnVocalsXmlPath.TabIndex = 36;
            this.btnVocalsXmlPath.Text = "...";
            this.btnVocalsXmlPath.UseVisualStyleBackColor = true;
            this.btnVocalsXmlPath.Click += new System.EventHandler(this.btnVocalsXmlPath_Click);
            // 
            // txtVocalsXmlPath
            // 
            this.txtVocalsXmlPath.Cue = "Select vocals.xml or jvocals.xml file here or in Creator GUI Arrangment group box" +
                "";
            this.txtVocalsXmlPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVocalsXmlPath.ForeColor = System.Drawing.Color.Gray;
            this.txtVocalsXmlPath.Location = new System.Drawing.Point(10, 23);
            this.txtVocalsXmlPath.Multiline = true;
            this.txtVocalsXmlPath.Name = "txtVocalsXmlPath";
            this.txtVocalsXmlPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVocalsXmlPath.Size = new System.Drawing.Size(510, 19);
            this.txtVocalsXmlPath.TabIndex = 35;
            this.toolTip.SetToolTip(this.txtVocalsXmlPath, "TIP: Use jvocals.xml when working with custom lyric fonts files.");
            // 
            // lblKey
            // 
            this.lblKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(565, 106);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(63, 13);
            this.lblKey.TabIndex = 39;
            this.lblKey.Text = "Vocals Key:";
            // 
            // lstKey
            // 
            this.lstKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstKey.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lstKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstKey.FormattingEnabled = true;
            this.lstKey.HorizontalScrollbar = true;
            this.lstKey.Location = new System.Drawing.Point(568, 122);
            this.lstKey.Name = "lstKey";
            this.lstKey.Size = new System.Drawing.Size(202, 225);
            this.lstKey.TabIndex = 38;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // btnRemoveFont
            // 
            this.btnRemoveFont.Location = new System.Drawing.Point(568, 68);
            this.btnRemoveFont.Name = "btnRemoveFont";
            this.btnRemoveFont.Size = new System.Drawing.Size(75, 20);
            this.btnRemoveFont.TabIndex = 40;
            this.btnRemoveFont.Text = "Remove";
            this.btnRemoveFont.UseVisualStyleBackColor = true;
            this.btnRemoveFont.Click += new System.EventHandler(this.btnRemoveFont_Click);
            // 
            // VocalsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(782, 408);
            this.Controls.Add(this.btnRemoveFont);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.lstKey);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnVocalsXmlPath);
            this.Controls.Add(this.txtVocalsXmlPath);
            this.Controls.Add(this.rtbVocals);
            this.Controls.Add(this.rtbBlank);
            this.Controls.Add(this.btnVocalsDdsPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtVocalsDdsPath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MinimumSize = new System.Drawing.Size(315, 229);
            this.Name = "VocalsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Vocals";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Button btnVocalsDdsPath;
        private System.Windows.Forms.Label label2;
        private RocksmithToolkitGUI.CueTextBox txtVocalsDdsPath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        public RichTextBox rtbVocals;
        public RichTextBox rtbBlank;
        private Label label3;
        private Button btnVocalsXmlPath;
        private CueTextBox txtVocalsXmlPath;
        private Label lblKey;
        private ListBox lstKey;
        private ToolTip toolTip;
        private Button btnRemoveFont;
    }
}
