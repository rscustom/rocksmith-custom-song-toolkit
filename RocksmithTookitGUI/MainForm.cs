using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using NLog;

namespace RocksmithToolkitGUI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Usage: RocksmithToolkitGUI.log.Error(«ERROR: {0}», this.Text);
        /// </summary>
        public static Logger log;
        public MainForm(string[] args)
        {
            log = LogManager.GetCurrentClassLogger();

            log.Info("Version: {0}", RocksmithToolkitLib.ToolkitVersion.version);
            log.Info("OS: {0}", Environment.OSVersion.ToString());
            log.Info("Command: {0}", Environment.CommandLine.ToString());

            InitializeComponent();
            if (args.Length > 0)
                LoadTemplate(args[0]);
            this.Text = String.Format("Custom Song Creator Toolkit (v{0} beta)", RocksmithToolkitLib.ToolkitVersion.version);
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
