namespace RocksmithTookitGUI.ZiggyProEditorConverter
{
    partial class ConvertInput
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
            this.components = new System.ComponentModel.Container();
            this.sngConvertButton = new System.Windows.Forms.Button();
            this.outputFileTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.inputXmlTextBox = new System.Windows.Forms.TextBox();
            this.sngFileCreatorModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.xmlBrowseButton = new System.Windows.Forms.Button();
            this.outputBrowseButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // sngConvertButton
            // 
            this.sngConvertButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.sngConvertButton.Location = new System.Drawing.Point(265, 90);
            this.sngConvertButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sngConvertButton.Name = "sngConvertButton";
            this.sngConvertButton.Size = new System.Drawing.Size(99, 36);
            this.sngConvertButton.TabIndex = 8;
            this.sngConvertButton.Text = "Convert";
            this.sngConvertButton.UseVisualStyleBackColor = true;
            this.sngConvertButton.Click += new System.EventHandler(this.sngConvertButton_Click);
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFileTextBox.Location = new System.Drawing.Point(3, 64);
            this.outputFileTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.Size = new System.Drawing.Size(559, 22);
            this.outputFileTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output XML File:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input XML File:";
            // 
            // inputXmlTextBox
            // 
            this.inputXmlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputXmlTextBox.Location = new System.Drawing.Point(3, 18);
            this.inputXmlTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.inputXmlTextBox.Name = "inputXmlTextBox";
            this.inputXmlTextBox.Size = new System.Drawing.Size(559, 22);
            this.inputXmlTextBox.TabIndex = 1;
            // 
            // xmlBrowseButton
            // 
            this.xmlBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlBrowseButton.Location = new System.Drawing.Point(568, 17);
            this.xmlBrowseButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.xmlBrowseButton.Name = "xmlBrowseButton";
            this.xmlBrowseButton.Size = new System.Drawing.Size(75, 25);
            this.xmlBrowseButton.TabIndex = 2;
            this.xmlBrowseButton.Text = "Browse";
            this.xmlBrowseButton.UseVisualStyleBackColor = true;
            this.xmlBrowseButton.Click += new System.EventHandler(this.xmlBrowseButton_Click);
            // 
            // outputBrowseButton
            // 
            this.outputBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputBrowseButton.Location = new System.Drawing.Point(568, 61);
            this.outputBrowseButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.outputBrowseButton.Name = "outputBrowseButton";
            this.outputBrowseButton.Size = new System.Drawing.Size(75, 25);
            this.outputBrowseButton.TabIndex = 9;
            this.outputBrowseButton.Text = "Browse";
            this.outputBrowseButton.UseVisualStyleBackColor = true;
            this.outputBrowseButton.Click += new System.EventHandler(this.outputXmlButton_Click);
            // 
            // ConvertInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outputBrowseButton);
            this.Controls.Add(this.sngConvertButton);
            this.Controls.Add(this.outputFileTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputXmlTextBox);
            this.Controls.Add(this.xmlBrowseButton);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConvertInput";
            this.Size = new System.Drawing.Size(655, 137);
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sngConvertButton;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputXmlTextBox;
        private System.Windows.Forms.Button xmlBrowseButton;
        private System.Windows.Forms.BindingSource sngFileCreatorModelBindingSource;
        private System.Windows.Forms.Button outputBrowseButton;
    }
}
