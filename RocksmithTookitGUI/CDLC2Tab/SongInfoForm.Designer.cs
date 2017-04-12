namespace RocksmithToolkitGUI.CDLC2Tab
{
    partial class SongInfoForm
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
            this.gbSongList = new System.Windows.Forms.GroupBox();
            this.btnContinue = new System.Windows.Forms.Button();
            this.lstSongList = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbSongList.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSongList
            // 
            this.gbSongList.Controls.Add(this.btnContinue);
            this.gbSongList.Controls.Add(this.lstSongList);
            this.gbSongList.Location = new System.Drawing.Point(12, 12);
            this.gbSongList.Name = "gbSongList";
            this.gbSongList.Size = new System.Drawing.Size(308, 196);
            this.gbSongList.TabIndex = 44;
            this.gbSongList.TabStop = false;
            this.gbSongList.Text = "Select Songs and Arrangements to Convert";
            // 
            // btnContinue
            // 
            this.btnContinue.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnContinue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnContinue.Location = new System.Drawing.Point(188, 161);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(94, 27);
            this.btnContinue.TabIndex = 42;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = false;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // lstSongList
            // 
            this.lstSongList.FormattingEnabled = true;
            this.lstSongList.HorizontalScrollbar = true;
            this.lstSongList.Location = new System.Drawing.Point(13, 20);
            this.lstSongList.Name = "lstSongList";
            this.lstSongList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstSongList.Size = new System.Drawing.Size(282, 134);
            this.lstSongList.TabIndex = 41;
            this.toolTip1.SetToolTip(this.lstSongList, "Left Click to Select/Deselect Single Song\r\nRight Click to Select/Deselect All Son" +
                    "gs");
            this.lstSongList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstSongList_MouseDown);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 200;
            this.toolTip1.AutoPopDelay = 8000;
            this.toolTip1.InitialDelay = 200;
            this.toolTip1.ReshowDelay = 40;
            // 
            // SongInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 222);
            this.ControlBox = false;
            this.Controls.Add(this.gbSongList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SongInfoForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Song List";
            this.gbSongList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSongList;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.ListBox lstSongList;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}