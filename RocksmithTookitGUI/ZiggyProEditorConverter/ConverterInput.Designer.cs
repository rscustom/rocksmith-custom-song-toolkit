namespace RocksmithToolkitGUI.ZiggyProEditorConverter
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
            this.label1 = new System.Windows.Forms.Label();
            this.inputXmlTextBox = new System.Windows.Forms.TextBox();
            this.sngFileCreatorModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.xmlBrowseButton = new System.Windows.Forms.Button();
            this.outputBrowseButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // sngConvertButton
            // 
            this.sngConvertButton.Location = new System.Drawing.Point(205, 76);
            this.sngConvertButton.Margin = new System.Windows.Forms.Padding(2);
            this.sngConvertButton.Name = "sngConvertButton";
            this.sngConvertButton.Size = new System.Drawing.Size(74, 29);
            this.sngConvertButton.TabIndex = 8;
            this.sngConvertButton.Text = "Convert";
            this.sngConvertButton.UseVisualStyleBackColor = true;
            this.sngConvertButton.Click += new System.EventHandler(this.sngConvertButton_Click);
            // 
            // outputFileTextBox
            // 
            this.outputFileTextBox.Location = new System.Drawing.Point(2, 52);
            this.outputFileTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.outputFileTextBox.Name = "outputFileTextBox";
            this.outputFileTextBox.Size = new System.Drawing.Size(419, 20);
            this.outputFileTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ziggy Pro Input XML File:";
            // 
            // inputXmlTextBox
            // 
            this.inputXmlTextBox.Location = new System.Drawing.Point(2, 15);
            this.inputXmlTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.inputXmlTextBox.Name = "inputXmlTextBox";
            this.inputXmlTextBox.Size = new System.Drawing.Size(419, 20);
            this.inputXmlTextBox.TabIndex = 1;
            // 
            // xmlBrowseButton
            // 
            this.xmlBrowseButton.Location = new System.Drawing.Point(425, 15);
            this.xmlBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.xmlBrowseButton.Name = "xmlBrowseButton";
            this.xmlBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.xmlBrowseButton.TabIndex = 2;
            this.xmlBrowseButton.Text = "Browse";
            this.xmlBrowseButton.UseVisualStyleBackColor = true;
            this.xmlBrowseButton.Click += new System.EventHandler(this.xmlBrowseButton_Click);
            // 
            // outputBrowseButton
            // 
            this.outputBrowseButton.Location = new System.Drawing.Point(425, 52);
            this.outputBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.outputBrowseButton.Name = "outputBrowseButton";
            this.outputBrowseButton.Size = new System.Drawing.Size(56, 20);
            this.outputBrowseButton.TabIndex = 9;
            this.outputBrowseButton.Text = "Browse";
            this.outputBrowseButton.UseVisualStyleBackColor = true;
            this.outputBrowseButton.Click += new System.EventHandler(this.outputXmlButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Rocksmith Output XML File:";
            // 
            // ConvertInput
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.outputBrowseButton);
            this.Controls.Add(this.sngConvertButton);
            this.Controls.Add(this.outputFileTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputXmlTextBox);
            this.Controls.Add(this.xmlBrowseButton);
            this.Name = "ConvertInput";
            this.Size = new System.Drawing.Size(490, 111);
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sngConvertButton;
        private System.Windows.Forms.TextBox outputFileTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputXmlTextBox;
        private System.Windows.Forms.Button xmlBrowseButton;
        private System.Windows.Forms.BindingSource sngFileCreatorModelBindingSource;
        private System.Windows.Forms.Button outputBrowseButton;
        private System.Windows.Forms.Label label2;
    }
}
