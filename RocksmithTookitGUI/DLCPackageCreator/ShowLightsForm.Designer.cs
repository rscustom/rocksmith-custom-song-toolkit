using System.Windows.Forms;
namespace RocksmithToolkitGUI.DLCPackageCreator
{
    partial class ShowLightsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowLightsForm));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShowLights = new System.Windows.Forms.Button();
            this.rtbShowlights = new System.Windows.Forms.RichTextBox();
            this.rtbBlank = new System.Windows.Forms.RichTextBox();
            this.lstKey = new System.Windows.Forms.ListBox();
            this.lblKey = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtShowLights = new RocksmithToolkitGUI.CueTextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnOk.Location = new System.Drawing.Point(567, 356);
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
            this.btnCancel.Location = new System.Drawing.Point(469, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 101);
            this.label1.TabIndex = 4;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnShowLights
            // 
            this.btnShowLights.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowLights.Location = new System.Drawing.Point(611, 48);
            this.btnShowLights.Name = "btnShowLights";
            this.btnShowLights.Size = new System.Drawing.Size(31, 22);
            this.btnShowLights.TabIndex = 6;
            this.btnShowLights.Text = "...";
            this.btnShowLights.UseVisualStyleBackColor = true;
            this.btnShowLights.Click += new System.EventHandler(this.btnShowLights_Click);
            // 
            // rtbShowlights
            // 
            this.rtbShowlights.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbShowlights.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbShowlights.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbShowlights.Location = new System.Drawing.Point(27, 128);
            this.rtbShowlights.Name = "rtbShowlights";
            this.rtbShowlights.Size = new System.Drawing.Size(419, 237);
            this.rtbShowlights.TabIndex = 30;
            this.rtbShowlights.Text = "";
            this.rtbShowlights.WordWrap = false;
            // 
            // rtbBlank
            // 
            this.rtbBlank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbBlank.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.rtbBlank.Location = new System.Drawing.Point(16, 116);
            this.rtbBlank.Name = "rtbBlank";
            this.rtbBlank.Size = new System.Drawing.Size(441, 262);
            this.rtbBlank.TabIndex = 31;
            this.rtbBlank.Text = "";
            // 
            // lstKey
            // 
            this.lstKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstKey.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lstKey.FormattingEnabled = true;
            this.lstKey.HorizontalScrollbar = true;
            this.lstKey.Location = new System.Drawing.Point(469, 115);
            this.lstKey.Name = "lstKey";
            this.lstKey.Size = new System.Drawing.Size(173, 225);
            this.lstKey.TabIndex = 32;
            // 
            // lblKey
            // 
            this.lblKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(471, 99);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(54, 13);
            this.lblKey.TabIndex = 33;
            this.lblKey.Text = "Note Key:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(327, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Showlights XML File Path:";
            // 
            // txtShowLights
            // 
            this.txtShowLights.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShowLights.Cue = "Select ShowLights XML file here or in Creator GUI Arrangment group box";
            this.txtShowLights.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtShowLights.ForeColor = System.Drawing.Color.Gray;
            this.txtShowLights.Location = new System.Drawing.Point(330, 38);
            this.txtShowLights.Multiline = true;
            this.txtShowLights.Name = "txtShowLights";
            this.txtShowLights.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtShowLights.Size = new System.Drawing.Size(267, 44);
            this.txtShowLights.TabIndex = 2;
            // 
            // ShowLightsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(658, 391);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.lstKey);
            this.Controls.Add(this.rtbShowlights);
            this.Controls.Add(this.rtbBlank);
            this.Controls.Add(this.btnShowLights);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtShowLights);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "ShowLightsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit ShowLights";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private Button btnShowLights;
        private CueTextBox txtShowLights;
        public RichTextBox rtbShowlights;
        public RichTextBox rtbBlank;
        private ListBox lstKey;
        private Label lblKey;
        private Label label2;
    }
}
