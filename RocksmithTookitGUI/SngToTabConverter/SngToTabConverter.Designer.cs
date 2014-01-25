namespace RocksmithToolkitGUI.SngToTabConverter
{
    partial class SngToTabConverter
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
            this.difficultyAll = new System.Windows.Forms.RadioButton();
            this.difficultyMax = new System.Windows.Forms.RadioButton();
            this.convertButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // difficultyAll
            // 
            this.difficultyAll.AutoSize = true;
            this.difficultyAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.difficultyAll.ForeColor = System.Drawing.SystemColors.ControlText;
            this.difficultyAll.Location = new System.Drawing.Point(10, 56);
            this.difficultyAll.Name = "difficultyAll";
            this.difficultyAll.Size = new System.Drawing.Size(128, 17);
            this.difficultyAll.TabIndex = 20;
            this.difficultyAll.TabStop = true;
            this.difficultyAll.Text = "All difficulty levels";
            this.difficultyAll.UseVisualStyleBackColor = true;
            // 
            // difficultyMax
            // 
            this.difficultyMax.AutoSize = true;
            this.difficultyMax.Checked = true;
            this.difficultyMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.difficultyMax.ForeColor = System.Drawing.SystemColors.ControlText;
            this.difficultyMax.Location = new System.Drawing.Point(10, 19);
            this.difficultyMax.Name = "difficultyMax";
            this.difficultyMax.Size = new System.Drawing.Size(186, 17);
            this.difficultyMax.TabIndex = 21;
            this.difficultyMax.TabStop = true;
            this.difficultyMax.Text = "Maximum difficulty level only";
            this.difficultyMax.UseVisualStyleBackColor = true;
            // 
            // convertButton
            // 
            this.convertButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.convertButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.convertButton.Location = new System.Drawing.Point(154, 90);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(105, 23);
            this.convertButton.TabIndex = 24;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = false;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.difficultyMax);
            this.groupBox1.Controls.Add(this.convertButton);
            this.groupBox1.Controls.Add(this.difficultyAll);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 137);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Convert SNG to ASCI Tablature plain text:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(39, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Will create multiple output files per arrangement";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(39, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "One file with last level including all notes";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Location = new System.Drawing.Point(122, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(165, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "Compatible only with Rocksmith 1";
            // 
            // SngToTabConverter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox1);
            this.Name = "SngToTabConverter";
            this.Size = new System.Drawing.Size(406, 145);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton difficultyAll;
        private System.Windows.Forms.RadioButton difficultyMax;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;

    }
}
