namespace RocksmithToolkitGUI.DLCPackageCreator
{
    partial class ToneImportForm
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
            this.gbToneList = new System.Windows.Forms.GroupBox();
            this.btnContinue = new System.Windows.Forms.Button();
            this.lstToneList = new System.Windows.Forms.ListBox();
            this.gbToneList.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbToneList
            // 
            this.gbToneList.Controls.Add(this.btnContinue);
            this.gbToneList.Controls.Add(this.lstToneList);
            this.gbToneList.Location = new System.Drawing.Point(12, 12);
            this.gbToneList.Name = "gbToneList";
            this.gbToneList.Size = new System.Drawing.Size(308, 196);
            this.gbToneList.TabIndex = 44;
            this.gbToneList.TabStop = false;
            this.gbToneList.Text = "Select Tone(s) to Import (Right click to select all)";
            // 
            // btnContinue
            // 
            this.btnContinue.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnContinue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnContinue.Location = new System.Drawing.Point(201, 163);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(94, 27);
            this.btnContinue.TabIndex = 42;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = false;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // lstToneList
            // 
            this.lstToneList.FormattingEnabled = true;
            this.lstToneList.HorizontalScrollbar = true;
            this.lstToneList.Location = new System.Drawing.Point(13, 20);
            this.lstToneList.Name = "lstToneList";
            this.lstToneList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstToneList.Size = new System.Drawing.Size(282, 134);
            this.lstToneList.TabIndex = 41;
            this.lstToneList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstSongList_MouseDown);
            // 
            // ToneImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 222);
            this.Controls.Add(this.gbToneList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToneImportForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tone List";
            this.gbToneList.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox gbToneList;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.ListBox lstToneList;
    }
}