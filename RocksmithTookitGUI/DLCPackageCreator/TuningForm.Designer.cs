using System.Windows.Forms;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    partial class TuningForm
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
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbTuning = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtString5 = new RocksmithToolkitGUI.CueTextBox();
            this.txtString4 = new RocksmithToolkitGUI.CueTextBox();
            this.txtString3 = new RocksmithToolkitGUI.CueTextBox();
            this.txtString2 = new RocksmithToolkitGUI.CueTextBox();
            this.txtString1 = new RocksmithToolkitGUI.CueTextBox();
            this.txtString0 = new RocksmithToolkitGUI.CueTextBox();
            this.chkAddTuning = new System.Windows.Forms.CheckBox();
            this.noteLabel = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.txtUIName = new RocksmithToolkitGUI.CueTextBox();
            this.txtName = new RocksmithToolkitGUI.CueTextBox();
            this.gbTuning.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.AutoSize = true;
            this.btnOK.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnOK.Location = new System.Drawing.Point(205, 133);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbTuning
            // 
            this.gbTuning.BackColor = System.Drawing.SystemColors.Control;
            this.gbTuning.Controls.Add(this.label6);
            this.gbTuning.Controls.Add(this.label5);
            this.gbTuning.Controls.Add(this.label4);
            this.gbTuning.Controls.Add(this.label3);
            this.gbTuning.Controls.Add(this.label2);
            this.gbTuning.Controls.Add(this.label1);
            this.gbTuning.Controls.Add(this.txtString5);
            this.gbTuning.Controls.Add(this.txtString4);
            this.gbTuning.Controls.Add(this.txtString3);
            this.gbTuning.Controls.Add(this.txtString2);
            this.gbTuning.Controls.Add(this.txtString1);
            this.gbTuning.Controls.Add(this.txtString0);
            this.gbTuning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbTuning.Location = new System.Drawing.Point(12, 38);
            this.gbTuning.Name = "gbTuning";
            this.gbTuning.Size = new System.Drawing.Size(286, 71);
            this.gbTuning.TabIndex = 2;
            this.gbTuning.TabStop = false;
            this.gbTuning.Text = "Tuning (Low to High)";
            this.toolTip.SetToolTip(this.gbTuning, "Enter the values as the number of \r\nhalf steps from E standard tuning\r\nwhich is s" +
                    "hown as 0, 0, 0, 0, 0, 0.\r\nEntered Low to High.");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Wheat;
            this.label6.ForeColor = System.Drawing.Color.Fuchsia;
            this.label6.Location = new System.Drawing.Point(237, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "String 5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Wheat;
            this.label5.ForeColor = System.Drawing.Color.Green;
            this.label5.Location = new System.Drawing.Point(191, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "String 4";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Wheat;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(145, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "String 3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Wheat;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(99, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "String 2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Wheat;
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(53, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "String 1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Wheat;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "String 0";
            // 
            // txtString5
            // 
            this.txtString5.Cue = "S5";
            this.txtString5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtString5.ForeColor = System.Drawing.Color.Gray;
            this.txtString5.Location = new System.Drawing.Point(238, 40);
            this.txtString5.Name = "txtString5";
            this.txtString5.Size = new System.Drawing.Size(40, 20);
            this.txtString5.TabIndex = 8;
            // 
            // txtString4
            // 
            this.txtString4.Cue = "S4";
            this.txtString4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtString4.ForeColor = System.Drawing.Color.Gray;
            this.txtString4.Location = new System.Drawing.Point(192, 40);
            this.txtString4.Name = "txtString4";
            this.txtString4.Size = new System.Drawing.Size(40, 20);
            this.txtString4.TabIndex = 7;
            // 
            // txtString3
            // 
            this.txtString3.Cue = "S3";
            this.txtString3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtString3.ForeColor = System.Drawing.Color.Gray;
            this.txtString3.Location = new System.Drawing.Point(146, 40);
            this.txtString3.Name = "txtString3";
            this.txtString3.Size = new System.Drawing.Size(40, 20);
            this.txtString3.TabIndex = 6;
            // 
            // txtString2
            // 
            this.txtString2.Cue = "S2";
            this.txtString2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtString2.ForeColor = System.Drawing.Color.Gray;
            this.txtString2.Location = new System.Drawing.Point(100, 40);
            this.txtString2.Name = "txtString2";
            this.txtString2.Size = new System.Drawing.Size(40, 20);
            this.txtString2.TabIndex = 5;
            // 
            // txtString1
            // 
            this.txtString1.Cue = "S1";
            this.txtString1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtString1.ForeColor = System.Drawing.Color.Gray;
            this.txtString1.Location = new System.Drawing.Point(54, 40);
            this.txtString1.Name = "txtString1";
            this.txtString1.Size = new System.Drawing.Size(40, 20);
            this.txtString1.TabIndex = 4;
            // 
            // txtString0
            // 
            this.txtString0.Cue = "S0";
            this.txtString0.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtString0.ForeColor = System.Drawing.Color.Gray;
            this.txtString0.Location = new System.Drawing.Point(8, 40);
            this.txtString0.Name = "txtString0";
            this.txtString0.Size = new System.Drawing.Size(40, 20);
            this.txtString0.TabIndex = 3;
            // 
            // chkAddTuning
            // 
            this.chkAddTuning.AutoSize = true;
            this.chkAddTuning.Location = new System.Drawing.Point(22, 137);
            this.chkAddTuning.Name = "chkAddTuning";
            this.chkAddTuning.Size = new System.Drawing.Size(118, 17);
            this.chkAddTuning.TabIndex = 9;
            this.chkAddTuning.Text = "Add as new Tuning";
            this.chkAddTuning.UseVisualStyleBackColor = true;
            this.chkAddTuning.CheckedChanged += new System.EventHandler(this.chkAddTuning_CheckedChanged);
            // 
            // noteLabel
            // 
            this.noteLabel.AutoSize = true;
            this.noteLabel.ForeColor = System.Drawing.Color.LightSlateGray;
            this.noteLabel.Location = new System.Drawing.Point(7, 112);
            this.noteLabel.Name = "noteLabel";
            this.noteLabel.Size = new System.Drawing.Size(300, 13);
            this.noteLabel.TabIndex = 23;
            this.noteLabel.Text = "If bass tuning, also fill strings 4 and 5 to allow for use on guitar.";
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            // 
            // txtUIName
            // 
            this.txtUIName.Cue = "UI Name";
            this.txtUIName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtUIName.ForeColor = System.Drawing.Color.Gray;
            this.txtUIName.Location = new System.Drawing.Point(14, 12);
            this.txtUIName.Name = "txtUIName";
            this.txtUIName.Size = new System.Drawing.Size(140, 20);
            this.txtUIName.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtUIName, "Tuning user interface/common name (spaces allowed)");
            this.txtUIName.TextChanged += new System.EventHandler(this.txtUIName_TextChanged);
            // 
            // txtName
            // 
            this.txtName.Cue = "Name";
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtName.ForeColor = System.Drawing.Color.Gray;
            this.txtName.Location = new System.Drawing.Point(158, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(140, 20);
            this.txtName.TabIndex = 2;
            this.toolTip.SetToolTip(this.txtName, "Tuning name no spaces (white space) allowed.");
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // TuningForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(310, 167);
            this.Controls.Add(this.chkAddTuning);
            this.Controls.Add(this.noteLabel);
            this.Controls.Add(this.txtUIName);
            this.Controls.Add(this.gbTuning);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TuningForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Creator, Edit or Confirm Tuning";
            this.gbTuning.ResumeLayout(false);
            this.gbTuning.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbTuning;
        private CueTextBox txtUIName;
        private CueTextBox txtName;
        private System.Windows.Forms.Label label1;
        private CueTextBox txtString5;
        private CueTextBox txtString4;
        private CueTextBox txtString3;
        private CueTextBox txtString2;
        private CueTextBox txtString1;
        private CueTextBox txtString0;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkAddTuning;
        private System.Windows.Forms.Label noteLabel;
        private ToolTip toolTip;
    }
}