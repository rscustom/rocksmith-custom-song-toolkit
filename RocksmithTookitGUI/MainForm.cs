using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Extensions;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace RocksmithToolkitGUI
{
    public partial class MainForm : Form
    {
        internal BackgroundWorker bWorker = new BackgroundWorker();
        private ToolkitVersionOnline onlineVersion = null;

        public static bool IsInDesignMode
        {
            get
            {
                if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1
                   || System.Diagnostics.Debugger.IsAttached)
                    return true;

                return false;
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0 && File.Exists(args[0]))
                LoadTemplate(args[0]);

            this.Text = String.Format("Custom Song Creator Toolkit (v{0} beta)", ToolkitVersion.version);

            bWorker.DoWork += new DoWorkEventHandler(CheckForUpdate);
            bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EnableUpdate);
            bWorker.RunWorkerAsync();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            // Show only by 'Configuration' click
            tabControl1.TabPages.Remove(GeneralConfigTab);
  
            // position main form at top center of screen to avoid having to reposition on low res displays
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, 0);
        }

        private void CheckForUpdate(object sender, DoWorkEventArgs e)
        {
            try
            {
                // CHECK FOR NEW AVAILABLE VERSION AND ENABLE UPDATE
                onlineVersion = ToolkitVersionOnline.Load();
            }
            catch (WebException) { /* Do nothing on 404 */ }
            catch (Exception)
            {
                throw;
            }
        }

        private void EnableUpdate(object sender, RunWorkerCompletedEventArgs e)
        {
            if (onlineVersion != null)
                if (ToolkitVersion.commit != "nongit")
                    updateButton.Visible = updateButton.Enabled = onlineVersion.UpdateAvailable;
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
                        dlcPackageCreatorControl.SaveTemplateFile();
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
            ReloadControls();
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

        private void updateButton_Click(object sender, EventArgs e)
        {
            using (var u = new UpdateForm()) {
                u.Init(onlineVersion);
                u.ShowDialog();
            }
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowConfigScreen();
        }

        private void ShowConfigScreen() {
            configurationToolStripMenuItem.Enabled = false;

            // Remove all tabs
            tabControl1.TabPages.Clear();

            // Add config
            if (!tabControl1.TabPages.Contains(GeneralConfigTab))
                tabControl1.TabPages.Add(GeneralConfigTab);
        }

        public void ReloadControls() {
            this.Controls.Clear();
            InitializeComponent();
            tabControl1.TabPages.Remove(GeneralConfigTab);
        }
    }
}
