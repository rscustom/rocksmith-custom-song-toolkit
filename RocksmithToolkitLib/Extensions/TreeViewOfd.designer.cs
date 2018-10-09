using RocksmithToolkitLib.Properties;
namespace RocksmithToolkitLib.Extensions
{
    partial class TreeViewOfd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeViewOfd));
            this.btnOk = new System.Windows.Forms.Button();
            this.butDown = new System.Windows.Forms.Button();
            this.butUp = new System.Windows.Forms.Button();
            this.butRemove = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.butAdd = new System.Windows.Forms.Button();
            this.lblFilesOfType = new System.Windows.Forms.Label();
            this.cmbFilesOfType = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.treeViewBrowser = new RocksmithToolkitLib.Extensions.TreeViewBrowser();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(413, 222);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 21);
            this.btnOk.TabIndex = 16;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // butDown
            // 
            this.butDown.Image = global::RocksmithToolkitLib.Properties.Resources.down;
            this.butDown.Location = new System.Drawing.Point(351, 162);
            this.butDown.Name = "butDown";
            this.butDown.Size = new System.Drawing.Size(32, 32);
            this.butDown.TabIndex = 15;
            this.toolTip.SetToolTip(this.butDown, "Move Selected Item Down");
            this.butDown.UseVisualStyleBackColor = true;
            this.butDown.Click += new System.EventHandler(this.butDown_Click);
            // 
            // butUp
            // 
            this.butUp.Image = global::RocksmithToolkitLib.Properties.Resources.up;
            this.butUp.Location = new System.Drawing.Point(351, 115);
            this.butUp.Name = "butUp";
            this.butUp.Size = new System.Drawing.Size(32, 32);
            this.butUp.TabIndex = 14;
            this.toolTip.SetToolTip(this.butUp, "Move Selected Item Up");
            this.butUp.UseVisualStyleBackColor = true;
            this.butUp.Click += new System.EventHandler(this.butUp_Click);
            // 
            // butRemove
            // 
            this.butRemove.Image = global::RocksmithToolkitLib.Properties.Resources.left;
            this.butRemove.Location = new System.Drawing.Point(351, 68);
            this.butRemove.Name = "butRemove";
            this.butRemove.Size = new System.Drawing.Size(32, 32);
            this.butRemove.TabIndex = 12;
            this.toolTip.SetToolTip(this.butRemove, "Remove Selected Item");
            this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.BackColor = System.Drawing.SystemColors.Window;
            this.listView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listView.Location = new System.Drawing.Point(393, 8);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(215, 203);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 11;
            this.toolTip.SetToolTip(this.listView, "The Bass arrangement (if present) should be\r\nselected first so that toolkit popul" +
                    "ates correctly.");
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.List;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // butAdd
            // 
            this.butAdd.Image = global::RocksmithToolkitLib.Properties.Resources.right;
            this.butAdd.Location = new System.Drawing.Point(351, 21);
            this.butAdd.Name = "butAdd";
            this.butAdd.Size = new System.Drawing.Size(32, 32);
            this.butAdd.TabIndex = 10;
            this.toolTip.SetToolTip(this.butAdd, "Add Selected Item");
            this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
            // 
            // lblFilesOfType
            // 
            this.lblFilesOfType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFilesOfType.AutoSize = true;
            this.lblFilesOfType.Location = new System.Drawing.Point(9, 225);
            this.lblFilesOfType.Name = "lblFilesOfType";
            this.lblFilesOfType.Size = new System.Drawing.Size(66, 13);
            this.lblFilesOfType.TabIndex = 19;
            this.lblFilesOfType.Text = "Files of type:";
            // 
            // cmbFilesOfType
            // 
            this.cmbFilesOfType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFilesOfType.FormattingEnabled = true;
            this.cmbFilesOfType.Location = new System.Drawing.Point(81, 222);
            this.cmbFilesOfType.Name = "cmbFilesOfType";
            this.cmbFilesOfType.Size = new System.Drawing.Size(261, 21);
            this.cmbFilesOfType.TabIndex = 20;
            this.cmbFilesOfType.SelectedIndexChanged += new System.EventHandler(this.cmbFilesOfType_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(510, 222);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 300;
            this.toolTip.AutoPopDelay = 6000;
            this.toolTip.InitialDelay = 300;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 60;
            // 
            // treeViewBrowser
            // 
            this.treeViewBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewBrowser.DefaultFolders = ((System.Collections.Generic.List<string>)(resources.GetObject("treeViewBrowser.DefaultFolders")));
            this.treeViewBrowser.Filter = ((System.Collections.Generic.Dictionary<string, string>)(resources.GetObject("treeViewBrowser.Filter")));
            this.treeViewBrowser.InitialDirectory = "";
            this.treeViewBrowser.ListDirectories = true;
            this.treeViewBrowser.ListFiles = true;
            this.treeViewBrowser.Location = new System.Drawing.Point(8, 8);
            this.treeViewBrowser.Multiselect = false;
            this.treeViewBrowser.Name = "treeViewBrowser";
            this.treeViewBrowser.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("treeViewBrowser.SelectedNodes")));
            this.treeViewBrowser.Size = new System.Drawing.Size(334, 203);
            this.treeViewBrowser.TabIndex = 22;
            this.toolTip.SetToolTip(this.treeViewBrowser, "Use Shift+Left Click to quickly select mutiple\r\narrangements in the desired order" +
                    " of appearance. ");
            // 
            // TreeViewOfd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 252);
            this.Controls.Add(this.treeViewBrowser);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cmbFilesOfType);
            this.Controls.Add(this.lblFilesOfType);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.butDown);
            this.Controls.Add(this.butUp);
            this.Controls.Add(this.butRemove);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.butAdd);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TreeViewOfd";
            this.Text = "CustomTreeViewOfd by Cozy";
            this.Load += new System.EventHandler(this.TreeViewOfd_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button butDown;
        private System.Windows.Forms.Button butUp;
        private System.Windows.Forms.Button butRemove;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Button butAdd;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Label lblFilesOfType;
        private System.Windows.Forms.ComboBox cmbFilesOfType;
        private System.Windows.Forms.Button btnCancel;
        private TreeViewBrowser treeViewBrowser;
        private System.Windows.Forms.ToolTip toolTip;
    }
}