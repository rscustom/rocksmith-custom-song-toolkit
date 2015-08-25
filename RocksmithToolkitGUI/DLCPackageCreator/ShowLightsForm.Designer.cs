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
            this.txtShowLights = new RocksmithToolkitGUI.CueTextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnOk.Location = new System.Drawing.Point(222, 151);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(125, 151);
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
            this.label1.Size = new System.Drawing.Size(285, 68);
            this.label1.TabIndex = 4;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnShowLights
            // 
            this.btnShowLights.Location = new System.Drawing.Point(266, 93);
            this.btnShowLights.Name = "btnShowLights";
            this.btnShowLights.Size = new System.Drawing.Size(31, 30);
            this.btnShowLights.TabIndex = 6;
            this.btnShowLights.Text = "...";
            this.btnShowLights.UseVisualStyleBackColor = true;
            this.btnShowLights.Click += new System.EventHandler(this.btnShowLights_Click);
            // 
            // txtShowLights
            // 
            this.txtShowLights.Cue = "Select ShowLights XML file here or in Creator GUI Arrangment group box";
            this.txtShowLights.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtShowLights.ForeColor = System.Drawing.Color.Gray;
            this.txtShowLights.Location = new System.Drawing.Point(12, 88);
            this.txtShowLights.Multiline = true;
            this.txtShowLights.Name = "txtShowLights";
            this.txtShowLights.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtShowLights.Size = new System.Drawing.Size(240, 40);
            this.txtShowLights.TabIndex = 2;
            this.txtShowLights.TextChanged += new System.EventHandler(this.txtShowLights_TextChanged);
            // 
            // ShowLightsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(309, 186);
            this.Controls.Add(this.btnShowLights);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtShowLights);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ShowLightsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ShowLights (Future GUI Area)";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private Button btnShowLights;
        private CueTextBox txtShowLights;
    }
}
