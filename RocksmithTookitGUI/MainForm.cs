using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace RocksmithToolkitGUI
{
    public partial class MainForm : Form
    {
        public static bool IsInDesignMode
        {
            get
            {
                if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
                    return true;

                return false;
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                LoadTemplate(args[0]);
            this.Text = String.Format("Custom Song Creator Toolkit (v{0} beta)", RocksmithToolkitLib.ToolkitVersion.version);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift) {
                switch (e.KeyCode)
	            {
                    case Keys.O: //<< Open Template
                        dlcPackageCreatorControl.dlcLoadButton_Click();
                        break;
                    case Keys.S: //<< Save Template
                        dlcPackageCreatorControl.dlcSaveButton_Click();
                        break;
                    case Keys.I: //<< Import Template
                        dlcPackageCreatorControl.dlcImportButton_Click();
                        break;
                    case Keys.G: //<< Generate Package
                        dlcPackageCreatorControl.dlcGenerateButton_Click();
                        break;
                    case Keys.A: //<< Add Arrangement
                        dlcPackageCreatorControl.arrangementAddButton_Click();
                        break;
                    case Keys.T: //<< Add Tone
                        dlcPackageCreatorControl.toneAddButton_Click();
                        break;
                    default:
                        break;
	            }
            }
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var a = new AboutForm())
            {
                a.ShowDialog();
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var h = new HelpForm())
            {
                h.ShowDialog();
            }
        }
    }
}
