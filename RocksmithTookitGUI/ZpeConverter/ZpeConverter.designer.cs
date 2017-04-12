namespace RocksmithToolkitGUI.ZpeConverter
{
    partial class ZpeConverter
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
            this.btnConvert = new System.Windows.Forms.Button();
            this.txtRs1Output = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtZpeInput = new System.Windows.Forms.TextBox();
            this.sngFileCreatorModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnZpeInput = new System.Windows.Forms.Button();
            this.btnRs1Output = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRs2014Output = new System.Windows.Forms.Button();
            this.txtRs2014Output = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(416, 168);
            this.btnConvert.Margin = new System.Windows.Forms.Padding(2);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(74, 29);
            this.btnConvert.TabIndex = 8;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // txtRs1Output
            // 
            this.txtRs1Output.Location = new System.Drawing.Point(8, 72);
            this.txtRs1Output.Margin = new System.Windows.Forms.Padding(2);
            this.txtRs1Output.Name = "txtRs1Output";
            this.txtRs1Output.Size = new System.Drawing.Size(419, 20);
            this.txtRs1Output.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ziggy Pro XML v70 (Input):";
            // 
            // txtZpeInput
            // 
            this.txtZpeInput.Location = new System.Drawing.Point(8, 24);
            this.txtZpeInput.Margin = new System.Windows.Forms.Padding(2);
            this.txtZpeInput.Name = "txtZpeInput";
            this.txtZpeInput.Size = new System.Drawing.Size(419, 20);
            this.txtZpeInput.TabIndex = 1;
            // 
            // btnZpeInput
            // 
            this.btnZpeInput.Location = new System.Drawing.Point(431, 24);
            this.btnZpeInput.Margin = new System.Windows.Forms.Padding(2);
            this.btnZpeInput.Name = "btnZpeInput";
            this.btnZpeInput.Size = new System.Drawing.Size(56, 20);
            this.btnZpeInput.TabIndex = 2;
            this.btnZpeInput.Text = "Browse";
            this.btnZpeInput.UseVisualStyleBackColor = true;
            this.btnZpeInput.Click += new System.EventHandler(this.btnZpeInput_Click);
            // 
            // btnRs1Output
            // 
            this.btnRs1Output.Location = new System.Drawing.Point(431, 72);
            this.btnRs1Output.Margin = new System.Windows.Forms.Padding(2);
            this.btnRs1Output.Name = "btnRs1Output";
            this.btnRs1Output.Size = new System.Drawing.Size(56, 20);
            this.btnRs1Output.TabIndex = 9;
            this.btnRs1Output.Text = "Browse";
            this.btnRs1Output.UseVisualStyleBackColor = true;
            this.btnRs1Output.Click += new System.EventHandler(this.btnRs1Output_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 57);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Rocksmith 2012 XML (Output Lead):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 105);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(304, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Rocksmith 2014 XML (Output Lead, Rhythm, Bass if available):";
            // 
            // btnRs2014Output
            // 
            this.btnRs2014Output.Location = new System.Drawing.Point(431, 120);
            this.btnRs2014Output.Margin = new System.Windows.Forms.Padding(2);
            this.btnRs2014Output.Name = "btnRs2014Output";
            this.btnRs2014Output.Size = new System.Drawing.Size(56, 20);
            this.btnRs2014Output.TabIndex = 12;
            this.btnRs2014Output.Text = "Browse";
            this.btnRs2014Output.UseVisualStyleBackColor = true;
            this.btnRs2014Output.Click += new System.EventHandler(this.btnRs2014Output_Click_1);
            // 
            // txtRs2014Output
            // 
            this.txtRs2014Output.Location = new System.Drawing.Point(8, 120);
            this.txtRs2014Output.Margin = new System.Windows.Forms.Padding(2);
            this.txtRs2014Output.Name = "txtRs2014Output";
            this.txtRs2014Output.Size = new System.Drawing.Size(419, 20);
            this.txtRs2014Output.TabIndex = 11;
            // 
            // ZpeConverter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnRs2014Output);
            this.Controls.Add(this.txtRs2014Output);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRs1Output);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.txtRs1Output);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtZpeInput);
            this.Controls.Add(this.btnZpeInput);
            this.Name = "ZpeConverter";
            this.Size = new System.Drawing.Size(500, 386);
            ((System.ComponentModel.ISupportInitialize)(this.sngFileCreatorModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.TextBox txtRs1Output;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtZpeInput;
        private System.Windows.Forms.Button btnZpeInput;
        private System.Windows.Forms.BindingSource sngFileCreatorModelBindingSource;
        private System.Windows.Forms.Button btnRs1Output;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRs2014Output;
        private System.Windows.Forms.TextBox txtRs2014Output;
    }
}
