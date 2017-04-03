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
            this.txtVocalsSngPath = new RocksmithToolkitGUI.CueTextBox();
            this.txtVocalsDdsPath = new RocksmithToolkitGUI.CueTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnVocalsDdsPath = new System.Windows.Forms.Button();
            this.sngpathFD = new System.Windows.Forms.Button();
            this.chkCustomFont = new System.Windows.Forms.CheckBox();
            this.rtbVocals = new System.Windows.Forms.RichTextBox();
            this.rtbBlank = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnVocalsXmlPath = new System.Windows.Forms.Button();
            this.txtVocalsXmlPath = new RocksmithToolkitGUI.CueTextBox();
            this.lblKey = new System.Windows.Forms.Label();
            this.lstKey = new System.Windows.Forms.ListBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnOk.Location = new System.Drawing.Point(580, 388);
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
            this.btnCancel.Location = new System.Drawing.Point(482, 388);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtVocalsSngPath
            // 
            this.txtVocalsSngPath.Cue = "Select SNG file with custom font (.sng)";
            this.txtVocalsSngPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVocalsSngPath.ForeColor = System.Drawing.Color.Gray;
            this.txtVocalsSngPath.Location = new System.Drawing.Point(12, 31);
            this.txtVocalsSngPath.Multiline = true;
            this.txtVocalsSngPath.Name = "txtVocalsSngPath";
            this.txtVocalsSngPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVocalsSngPath.Size = new System.Drawing.Size(271, 20);
            this.txtVocalsSngPath.TabIndex = 2;
            // 
            // txtVocalsDdsPath
            // 
            this.txtVocalsDdsPath.Cue = "Select DDS file with Lyrics Art (.dds)";
            this.txtVocalsDdsPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVocalsDdsPath.ForeColor = System.Drawing.Color.Gray;
            this.txtVocalsDdsPath.Location = new System.Drawing.Point(12, 81);
            this.txtVocalsDdsPath.Multiline = true;
            this.txtVocalsDdsPath.Name = "txtVocalsDdsPath";
            this.txtVocalsDdsPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVocalsDdsPath.Size = new System.Drawing.Size(271, 20);
            this.txtVocalsDdsPath.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "For now you can prepare a SNG custom font file by hand: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Custom vocals should have glyph DDS texture:";
            // 
            // btnVocalsDdsPath
            // 
            this.btnVocalsDdsPath.Location = new System.Drawing.Point(289, 81);
            this.btnVocalsDdsPath.Name = "btnVocalsDdsPath";
            this.btnVocalsDdsPath.Size = new System.Drawing.Size(31, 20);
            this.btnVocalsDdsPath.TabIndex = 6;
            this.btnVocalsDdsPath.Text = "...";
            this.toolTip.SetToolTip(this.btnVocalsDdsPath, "For Expert Charter Usage Only");
            this.btnVocalsDdsPath.UseVisualStyleBackColor = true;
            this.btnVocalsDdsPath.Click += new System.EventHandler(this.btnVocalsDdsPath_Click);
            // 
            // sngpathFD
            // 
            this.sngpathFD.Location = new System.Drawing.Point(289, 31);
            this.sngpathFD.Name = "sngpathFD";
            this.sngpathFD.Size = new System.Drawing.Size(31, 20);
            this.sngpathFD.TabIndex = 6;
            this.sngpathFD.Text = "...";
            this.toolTip.SetToolTip(this.sngpathFD, "For Expert Charter Usage Only");
            this.sngpathFD.UseVisualStyleBackColor = true;
            this.sngpathFD.Click += new System.EventHandler(this.btnVocalsSngPath_Click);
            // 
            // chkCustomFont
            // 
            this.chkCustomFont.AutoSize = true;
            this.chkCustomFont.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCustomFont.Location = new System.Drawing.Point(342, 33);
            this.chkCustomFont.Margin = new System.Windows.Forms.Padding(0);
            this.chkCustomFont.Name = "chkCustomFont";
            this.chkCustomFont.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkCustomFont.Size = new System.Drawing.Size(110, 17);
            this.chkCustomFont.TabIndex = 7;
            this.chkCustomFont.Text = "Use Custom Font:";
            this.toolTip.SetToolTip(this.chkCustomFont, "For Expert Charter Usage Only");
            this.chkCustomFont.UseVisualStyleBackColor = true;
            // 
            // rtbVocals
            // 
            this.rtbVocals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbVocals.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbVocals.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbVocals.Location = new System.Drawing.Point(21, 138);
            this.rtbVocals.Name = "rtbVocals";
            this.rtbVocals.Size = new System.Drawing.Size(435, 260);
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
            this.rtbBlank.Location = new System.Drawing.Point(10, 126);
            this.rtbBlank.Name = "rtbBlank";
            this.rtbBlank.Size = new System.Drawing.Size(457, 285);
            this.rtbBlank.TabIndex = 33;
            this.rtbBlank.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(344, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "Vocals XML File Path:";
            // 
            // btnVocalsXmlPath
            // 
            this.btnVocalsXmlPath.Location = new System.Drawing.Point(624, 81);
            this.btnVocalsXmlPath.Name = "btnVocalsXmlPath";
            this.btnVocalsXmlPath.Size = new System.Drawing.Size(31, 20);
            this.btnVocalsXmlPath.TabIndex = 36;
            this.btnVocalsXmlPath.Text = "...";
            this.btnVocalsXmlPath.UseVisualStyleBackColor = true;
            this.btnVocalsXmlPath.Click += new System.EventHandler(this.btnVocalsXmlPath_Click);
            // 
            // txtVocalsXmlPath
            // 
            this.txtVocalsXmlPath.Cue = "Select ShowLights XML file here or in Creator GUI Arrangment group box";
            this.txtVocalsXmlPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVocalsXmlPath.ForeColor = System.Drawing.Color.Gray;
            this.txtVocalsXmlPath.Location = new System.Drawing.Point(347, 81);
            this.txtVocalsXmlPath.Multiline = true;
            this.txtVocalsXmlPath.Name = "txtVocalsXmlPath";
            this.txtVocalsXmlPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVocalsXmlPath.Size = new System.Drawing.Size(271, 20);
            this.txtVocalsXmlPath.TabIndex = 35;
            // 
            // lblKey
            // 
            this.lblKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(485, 122);
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
            this.lstKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstKey.FormattingEnabled = true;
            this.lstKey.HorizontalScrollbar = true;
            this.lstKey.ItemHeight = 15;
            this.lstKey.Location = new System.Drawing.Point(483, 138);
            this.lstKey.Name = "lstKey";
            this.lstKey.Size = new System.Drawing.Size(172, 229);
            this.lstKey.TabIndex = 38;
            // 
            // VocalsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(667, 424);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.lstKey);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnVocalsXmlPath);
            this.Controls.Add(this.txtVocalsXmlPath);
            this.Controls.Add(this.rtbVocals);
            this.Controls.Add(this.rtbBlank);
            this.Controls.Add(this.chkCustomFont);
            this.Controls.Add(this.sngpathFD);
            this.Controls.Add(this.btnVocalsDdsPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtVocalsDdsPath);
            this.Controls.Add(this.txtVocalsSngPath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(315, 229);
            this.Name = "VocalsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Vocals";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.CheckBox chkCustomFont;
        private System.Windows.Forms.Button sngpathFD;
        private System.Windows.Forms.Button btnVocalsDdsPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private RocksmithToolkitGUI.CueTextBox txtVocalsDdsPath;
        private RocksmithToolkitGUI.CueTextBox txtVocalsSngPath;
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
    }
}
