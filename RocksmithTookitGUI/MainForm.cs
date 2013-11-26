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
        public MainForm()
        {
            InitializeComponent();
            this.Text = String.Format("Custom Song Creator Toolkit (v{0} beta)", RocksmithToolkitLib.ToolkitVersion.version,
                Assembly.GetExecutingAssembly().GetName().Version.Minor,
                Assembly.GetExecutingAssembly().GetName().Version.Build,
                Assembly.GetExecutingAssembly().GetName().Version.Revision);
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
