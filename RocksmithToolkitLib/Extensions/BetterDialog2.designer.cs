namespace RocksmithToolkitLib.Extensions
{
    partial class BetterDialog2
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
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.lblIconMessage = new System.Windows.Forms.Label();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.lblDialogMessage = new System.Windows.Forms.Label();
            this.tlpDialog = new System.Windows.Forms.TableLayoutPanel();
            this.pbLine = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.tlpDialog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLine)).BeginInit();
            this.SuspendLayout();
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.Transparent;
            this.pbIcon.Location = new System.Drawing.Point(5, 5);
            this.pbIcon.Margin = new System.Windows.Forms.Padding(5);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(30, 30);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbIcon.TabIndex = 0;
            this.pbIcon.TabStop = false;
            // 
            // lblIconMessage
            // 
            this.lblIconMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIconMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblIconMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIconMessage.Location = new System.Drawing.Point(43, 0);
            this.lblIconMessage.Name = "lblIconMessage";
            this.lblIconMessage.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblIconMessage.Size = new System.Drawing.Size(248, 40);
            this.lblIconMessage.TabIndex = 2;
            this.lblIconMessage.Text = "Icon Message in Bold";
            this.lblIconMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn3
            // 
            this.btn3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn3.Location = new System.Drawing.Point(200, 138);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(75, 23);
            this.btn3.TabIndex = 1;
            this.btn3.Text = "Cancel";
            this.btn3.UseVisualStyleBackColor = true;
            // 
            // btn1
            // 
            this.btn1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn1.Location = new System.Drawing.Point(18, 138);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(75, 23);
            this.btn1.TabIndex = 0;
            this.btn1.Text = "Yes";
            this.btn1.UseVisualStyleBackColor = true;
            // 
            // btn2
            // 
            this.btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn2.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btn2.Location = new System.Drawing.Point(109, 138);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(75, 23);
            this.btn2.TabIndex = 3;
            this.btn2.Text = "No";
            this.btn2.UseVisualStyleBackColor = true;
            // 
            // lblDialogMessage
            // 
            this.lblDialogMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDialogMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblDialogMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDialogMessage.Location = new System.Drawing.Point(43, 40);
            this.lblDialogMessage.Name = "lblDialogMessage";
            this.lblDialogMessage.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.lblDialogMessage.Size = new System.Drawing.Size(248, 83);
            this.lblDialogMessage.TabIndex = 4;
            this.lblDialogMessage.Text = "Dialog Message";
            // 
            // tlpDialog
            // 
            this.tlpDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpDialog.BackColor = System.Drawing.SystemColors.Window;
            this.tlpDialog.ColumnCount = 2;
            this.tlpDialog.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpDialog.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDialog.Controls.Add(this.pbIcon, 0, 0);
            this.tlpDialog.Controls.Add(this.lblIconMessage, 1, 0);
            this.tlpDialog.Controls.Add(this.lblDialogMessage, 1, 1);
            this.tlpDialog.Location = new System.Drawing.Point(1, 1);
            this.tlpDialog.Name = "tlpDialog";
            this.tlpDialog.RowCount = 2;
            this.tlpDialog.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpDialog.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDialog.Size = new System.Drawing.Size(294, 123);
            this.tlpDialog.TabIndex = 5;
            // 
            // pbLine
            // 
            this.pbLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLine.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pbLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbLine.Location = new System.Drawing.Point(-9, 125);
            this.pbLine.Name = "pbLine";
            this.pbLine.Size = new System.Drawing.Size(313, 51);
            this.pbLine.TabIndex = 6;
            this.pbLine.TabStop = false;
            // 
            // BetterDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btn3;
            this.ClientSize = new System.Drawing.Size(294, 168);
            this.Controls.Add(this.tlpDialog);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.pbLine);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BetterDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Dialog Title";
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.tlpDialog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLine)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblIconMessage;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Label lblDialogMessage;
        private System.Windows.Forms.TableLayoutPanel tlpDialog;
        private System.Windows.Forms.PictureBox pbLine;
    }
}