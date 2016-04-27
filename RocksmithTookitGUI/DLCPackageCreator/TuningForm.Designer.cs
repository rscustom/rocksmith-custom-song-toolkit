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
            this.okButton = new System.Windows.Forms.Button();
            this.gbTuning = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.string5TB = new RocksmithToolkitGUI.CueTextBox();
            this.string4TB = new RocksmithToolkitGUI.CueTextBox();
            this.string3TB = new RocksmithToolkitGUI.CueTextBox();
            this.string2TB = new RocksmithToolkitGUI.CueTextBox();
            this.string1TB = new RocksmithToolkitGUI.CueTextBox();
            this.string0TB = new RocksmithToolkitGUI.CueTextBox();
            this.nameTB = new RocksmithToolkitGUI.CueTextBox();
            this.uiNameTB = new RocksmithToolkitGUI.CueTextBox();
            this.StateAdd = new System.Windows.Forms.CheckBox();
            this.noteLabel = new System.Windows.Forms.Label();
            this.gbTuning.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okButton.AutoSize = true;
            this.okButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.okButton.Location = new System.Drawing.Point(205, 133);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(93, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // gbTuning
            // 
            this.gbTuning.Controls.Add(this.label6);
            this.gbTuning.Controls.Add(this.label5);
            this.gbTuning.Controls.Add(this.label4);
            this.gbTuning.Controls.Add(this.label3);
            this.gbTuning.Controls.Add(this.label2);
            this.gbTuning.Controls.Add(this.label1);
            this.gbTuning.Controls.Add(this.string5TB);
            this.gbTuning.Controls.Add(this.string4TB);
            this.gbTuning.Controls.Add(this.string3TB);
            this.gbTuning.Controls.Add(this.string2TB);
            this.gbTuning.Controls.Add(this.string1TB);
            this.gbTuning.Controls.Add(this.string0TB);
            this.gbTuning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gbTuning.Location = new System.Drawing.Point(12, 38);
            this.gbTuning.Name = "gbTuning";
            this.gbTuning.Size = new System.Drawing.Size(286, 71);
            this.gbTuning.TabIndex = 2;
            this.gbTuning.TabStop = false;
            this.gbTuning.Text = "Tuning";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
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
            this.label4.BackColor = System.Drawing.Color.Transparent;
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
            this.label3.BackColor = System.Drawing.Color.Silver;
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
            this.label2.BackColor = System.Drawing.Color.Silver;
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
            this.label1.BackColor = System.Drawing.Color.Silver;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "String 0";
            // 
            // string5TB
            // 
            this.string5TB.Cue = "S5";
            this.string5TB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.string5TB.ForeColor = System.Drawing.Color.Gray;
            this.string5TB.Location = new System.Drawing.Point(238, 40);
            this.string5TB.Name = "string5TB";
            this.string5TB.Size = new System.Drawing.Size(40, 20);
            this.string5TB.TabIndex = 8;
            // 
            // string4TB
            // 
            this.string4TB.Cue = "S4";
            this.string4TB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.string4TB.ForeColor = System.Drawing.Color.Gray;
            this.string4TB.Location = new System.Drawing.Point(192, 40);
            this.string4TB.Name = "string4TB";
            this.string4TB.Size = new System.Drawing.Size(40, 20);
            this.string4TB.TabIndex = 7;
            // 
            // string3TB
            // 
            this.string3TB.Cue = "S3";
            this.string3TB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.string3TB.ForeColor = System.Drawing.Color.Gray;
            this.string3TB.Location = new System.Drawing.Point(146, 40);
            this.string3TB.Name = "string3TB";
            this.string3TB.Size = new System.Drawing.Size(40, 20);
            this.string3TB.TabIndex = 6;
            // 
            // string2TB
            // 
            this.string2TB.Cue = "S2";
            this.string2TB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.string2TB.ForeColor = System.Drawing.Color.Gray;
            this.string2TB.Location = new System.Drawing.Point(100, 40);
            this.string2TB.Name = "string2TB";
            this.string2TB.Size = new System.Drawing.Size(40, 20);
            this.string2TB.TabIndex = 5;
            // 
            // string1TB
            // 
            this.string1TB.Cue = "S1";
            this.string1TB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.string1TB.ForeColor = System.Drawing.Color.Gray;
            this.string1TB.Location = new System.Drawing.Point(54, 40);
            this.string1TB.Name = "string1TB";
            this.string1TB.Size = new System.Drawing.Size(40, 20);
            this.string1TB.TabIndex = 4;
            // 
            // string0TB
            // 
            this.string0TB.Cue = "S0";
            this.string0TB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.string0TB.ForeColor = System.Drawing.Color.Gray;
            this.string0TB.Location = new System.Drawing.Point(8, 40);
            this.string0TB.Name = "string0TB";
            this.string0TB.Size = new System.Drawing.Size(40, 20);
            this.string0TB.TabIndex = 3;
            // 
            // nameTB
            // 
            this.nameTB.Cue = "Name";
            this.nameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nameTB.ForeColor = System.Drawing.Color.Gray;
            this.nameTB.Location = new System.Drawing.Point(158, 12);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(140, 20);
            this.nameTB.TabIndex = 2;
            this.nameTB.TextChanged += new System.EventHandler(this.nameTB_TextChanged);
            // 
            // uiNameTB
            // 
            this.uiNameTB.Cue = "UI Name";
            this.uiNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.uiNameTB.ForeColor = System.Drawing.Color.Gray;
            this.uiNameTB.Location = new System.Drawing.Point(14, 12);
            this.uiNameTB.Name = "uiNameTB";
            this.uiNameTB.Size = new System.Drawing.Size(140, 20);
            this.uiNameTB.TabIndex = 1;
            this.uiNameTB.TextChanged += new System.EventHandler(this.uiNameTB_TextChanged);
            // 
            // StateAdd
            // 
            this.StateAdd.AutoSize = true;
            this.StateAdd.Location = new System.Drawing.Point(22, 137);
            this.StateAdd.Name = "StateAdd";
            this.StateAdd.Size = new System.Drawing.Size(118, 17);
            this.StateAdd.TabIndex = 9;
            this.StateAdd.Text = "Add as new Tuning";
            this.StateAdd.UseVisualStyleBackColor = true;
            this.StateAdd.CheckedChanged += new System.EventHandler(this.StateAdd_CheckedChanged);
            // 
            // noteLabel
            // 
            this.noteLabel.AutoSize = true;
            this.noteLabel.ForeColor = System.Drawing.Color.LightSlateGray;
            this.noteLabel.Location = new System.Drawing.Point(7, 112);
            this.noteLabel.Name = "noteLabel";
            this.noteLabel.Size = new System.Drawing.Size(296, 13);
            this.noteLabel.TabIndex = 23;
            this.noteLabel.Text = "If a bass tuning, fill strings 4 and 5 to allow for use on guitar ...";
            // 
            // TuningForm
            // 
            this.StartPosition = FormStartPosition.CenterParent;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(310, 167);
            this.Controls.Add(this.StateAdd);
            this.Controls.Add(this.noteLabel);
            this.Controls.Add(this.uiNameTB);
            this.Controls.Add(this.gbTuning);
            this.Controls.Add(this.nameTB);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TuningForm";
            this.Text = "Tuning Editor";
            this.gbTuning.ResumeLayout(false);
            this.gbTuning.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox gbTuning;
        private CueTextBox uiNameTB;
        private CueTextBox nameTB;
        private System.Windows.Forms.Label label1;
        private CueTextBox string5TB;
        private CueTextBox string4TB;
        private CueTextBox string3TB;
        private CueTextBox string2TB;
        private CueTextBox string1TB;
        private CueTextBox string0TB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox StateAdd;
        private System.Windows.Forms.Label noteLabel;
    }
}