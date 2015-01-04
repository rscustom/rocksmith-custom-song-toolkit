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
            this.OkButton = new System.Windows.Forms.Button();
            this.cancelBT = new System.Windows.Forms.Button();
            this.SngPathCTB = new RocksmithToolkitGUI.CueTextBox();
            this.ArtPathCTB = new RocksmithToolkitGUI.CueTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.artpathFD = new System.Windows.Forms.Button();
            this.sngpathFD = new System.Windows.Forms.Button();
            this.isCustomCB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            this.OkButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.OkButton.Location = new System.Drawing.Point(207, 159);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 0;
            this.OkButton.Text = "Ok";
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelBT
            // 
            this.cancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBT.Location = new System.Drawing.Point(122, 159);
            this.cancelBT.Name = "cancelBT";
            this.cancelBT.Size = new System.Drawing.Size(75, 23);
            this.cancelBT.TabIndex = 1;
            this.cancelBT.Text = "Cancel";
            this.cancelBT.UseVisualStyleBackColor = true;
            // 
            // SngPathCTB
            // 
            this.SngPathCTB.Cue = "Select SNG file with custom font (.sng)";
            this.SngPathCTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SngPathCTB.ForeColor = System.Drawing.Color.Gray;
            this.SngPathCTB.Location = new System.Drawing.Point(12, 43);
            this.SngPathCTB.Name = "SngPathCTB";
            this.SngPathCTB.Size = new System.Drawing.Size(240, 20);
            this.SngPathCTB.TabIndex = 2;
            this.SngPathCTB.TextChanged += new System.EventHandler(this.SngPathCTB_TextChanged);
            // 
            // ArtPathCTB
            // 
            this.ArtPathCTB.Cue = "Select DDS file with Lyrics Art (.dds)";
            this.ArtPathCTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ArtPathCTB.ForeColor = System.Drawing.Color.Gray;
            this.ArtPathCTB.Location = new System.Drawing.Point(12, 99);
            this.ArtPathCTB.Name = "ArtPathCTB";
            this.ArtPathCTB.Size = new System.Drawing.Size(240, 20);
            this.ArtPathCTB.TabIndex = 3;
            this.ArtPathCTB.TextChanged += new System.EventHandler(this.ArtPathCTB_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "For now you can prepare sng file by hands, \r\nPlease do so and select it below";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Vocals should have it\'s glyph texture, select such below";
            // 
            // artpathFD
            // 
            this.artpathFD.Location = new System.Drawing.Point(258, 99);
            this.artpathFD.Name = "artpathFD";
            this.artpathFD.Size = new System.Drawing.Size(31, 20);
            this.artpathFD.TabIndex = 6;
            this.artpathFD.Text = "...";
            this.artpathFD.UseVisualStyleBackColor = true;
            this.artpathFD.Click += new System.EventHandler(this.ArtpathFD_Click);
            // 
            // sngpathFD
            // 
            this.sngpathFD.Location = new System.Drawing.Point(258, 42);
            this.sngpathFD.Name = "sngpathFD";
            this.sngpathFD.Size = new System.Drawing.Size(31, 20);
            this.sngpathFD.TabIndex = 6;
            this.sngpathFD.Text = "...";
            this.sngpathFD.UseVisualStyleBackColor = true;
            this.sngpathFD.Click += new System.EventHandler(this.SngpathFD_Click);
            // 
            // isCustomCB
            // 
            this.isCustomCB.Location = new System.Drawing.Point(12, 125);
            this.isCustomCB.Name = "isCustomCB";
            this.isCustomCB.Size = new System.Drawing.Size(104, 24);
            this.isCustomCB.TabIndex = 7;
            this.isCustomCB.Text = "Custom Font";
            this.isCustomCB.UseVisualStyleBackColor = true;
            this.isCustomCB.CheckedChanged += new System.EventHandler(this.IsCustomCB_CheckedChanged);
            // 
            // VocalsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(294, 191);
            this.Controls.Add(this.isCustomCB);
            this.Controls.Add(this.sngpathFD);
            this.Controls.Add(this.artpathFD);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ArtPathCTB);
            this.Controls.Add(this.SngPathCTB);
            this.Controls.Add(this.cancelBT);
            this.Controls.Add(this.OkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 216);
            this.MinimumSize = new System.Drawing.Size(300, 216);
            this.Name = "VocalsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vocals Form";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.CheckBox isCustomCB;
        private System.Windows.Forms.Button sngpathFD;
        private System.Windows.Forms.Button artpathFD;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private RocksmithToolkitGUI.CueTextBox ArtPathCTB;
        private RocksmithToolkitGUI.CueTextBox SngPathCTB;
        private System.Windows.Forms.Button cancelBT;
        private System.Windows.Forms.Button OkButton;
    }
}
