namespace RocksmithDLCCreator
{
    partial class Form1
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
            this.DlcNameTB = new System.Windows.Forms.TextBox();
            this.SongDisplayNameTB = new System.Windows.Forms.TextBox();
            this.ArtistTB = new System.Windows.Forms.TextBox();
            this.AlbumTB = new System.Windows.Forms.TextBox();
            this.YearTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ArrangementLB = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.OggPathTB = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.AlbumArtPathTB = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DlcNameTB
            // 
            this.DlcNameTB.Location = new System.Drawing.Point(13, 13);
            this.DlcNameTB.Name = "DlcNameTB";
            this.DlcNameTB.Size = new System.Drawing.Size(100, 20);
            this.DlcNameTB.TabIndex = 0;
            this.DlcNameTB.Text = "DLCName";
            // 
            // SongDisplayNameTB
            // 
            this.SongDisplayNameTB.Location = new System.Drawing.Point(119, 13);
            this.SongDisplayNameTB.Name = "SongDisplayNameTB";
            this.SongDisplayNameTB.Size = new System.Drawing.Size(428, 20);
            this.SongDisplayNameTB.TabIndex = 1;
            this.SongDisplayNameTB.Text = "SongDisplayName";
            // 
            // ArtistTB
            // 
            this.ArtistTB.Location = new System.Drawing.Point(13, 39);
            this.ArtistTB.Name = "ArtistTB";
            this.ArtistTB.Size = new System.Drawing.Size(100, 20);
            this.ArtistTB.TabIndex = 2;
            this.ArtistTB.Text = "Artist";
            // 
            // AlbumTB
            // 
            this.AlbumTB.Location = new System.Drawing.Point(119, 38);
            this.AlbumTB.Name = "AlbumTB";
            this.AlbumTB.Size = new System.Drawing.Size(101, 20);
            this.AlbumTB.TabIndex = 3;
            this.AlbumTB.Text = "Album";
            // 
            // YearTB
            // 
            this.YearTB.Location = new System.Drawing.Point(226, 38);
            this.YearTB.Name = "YearTB";
            this.YearTB.Size = new System.Drawing.Size(100, 20);
            this.YearTB.TabIndex = 4;
            this.YearTB.Text = "1997";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Arrangements";
            // 
            // ArrangementLB
            // 
            this.ArrangementLB.FormattingEnabled = true;
            this.ArrangementLB.Location = new System.Drawing.Point(15, 107);
            this.ArrangementLB.Name = "ArrangementLB";
            this.ArrangementLB.Size = new System.Drawing.Size(451, 225);
            this.ArrangementLB.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(473, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(473, 137);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Remove";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // OggPathTB
            // 
            this.OggPathTB.Location = new System.Drawing.Point(15, 339);
            this.OggPathTB.Name = "OggPathTB";
            this.OggPathTB.Size = new System.Drawing.Size(451, 20);
            this.OggPathTB.TabIndex = 9;
            this.OggPathTB.Text = "Ogg File";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(472, 337);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Open";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(238, 365);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "Generate";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // AlbumArtPathTB
            // 
            this.AlbumArtPathTB.Location = new System.Drawing.Point(12, 65);
            this.AlbumArtPathTB.Name = "AlbumArtPathTB";
            this.AlbumArtPathTB.Size = new System.Drawing.Size(454, 20);
            this.AlbumArtPathTB.TabIndex = 12;
            this.AlbumArtPathTB.Text = "Album Art";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(473, 65);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 13;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 393);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.AlbumArtPathTB);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.OggPathTB);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ArrangementLB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.YearTB);
            this.Controls.Add(this.AlbumTB);
            this.Controls.Add(this.ArtistTB);
            this.Controls.Add(this.SongDisplayNameTB);
            this.Controls.Add(this.DlcNameTB);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DlcNameTB;
        private System.Windows.Forms.TextBox SongDisplayNameTB;
        private System.Windows.Forms.TextBox ArtistTB;
        private System.Windows.Forms.TextBox AlbumTB;
        private System.Windows.Forms.TextBox YearTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox ArrangementLB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox OggPathTB;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox AlbumArtPathTB;
        private System.Windows.Forms.Button button5;
    }
}

