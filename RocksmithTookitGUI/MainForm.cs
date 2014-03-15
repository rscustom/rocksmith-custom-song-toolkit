using System;
using System.Collections.Generic;
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
        private const string APP_UPDATER = "RocksmithToolkitUpdater.exe";
        private const string APP_UPDATING = "RocksmithToolkitUpdating.exe";

        internal BackgroundWorker bWorker = new BackgroundWorker();
        private bool NewVersionAvailable = false;

        public static bool IsInDesignMode
        {
            get
            {
                if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
                    return true;

                return false;
            }
        }

        private string RootDirectory {
            get {
                return Path.GetDirectoryName(Application.ExecutablePath);
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0)
                LoadTemplate(args[0]);

            this.Text = String.Format("Custom Song Creator Toolkit (v{0} beta)", ToolkitVersion.version);

            bWorker.DoWork += new DoWorkEventHandler(CheckForUpdate);
            bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EnableUpdate);
            bWorker.RunWorkerAsync();
        }

        private void CheckForUpdate(object sender, DoWorkEventArgs e)
        {
            try
            {
                // DELETE OLD UPDATER APP IF EXISTS
                var updatingApp = Path.Combine(RootDirectory, APP_UPDATING);
                if (File.Exists(updatingApp))
                    File.Delete(updatingApp);

                // CHECK FOR NEW AVAILABLE VERSION AND ENABLE UPDATE
                if (ToolkitVersionOnline.HasNewVersion())
                    NewVersionAvailable = true;
            }
            catch (WebException) { /* Do nothing on 404 */ }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnableUpdate(object sender, RunWorkerCompletedEventArgs e)
        {
            updateButton.Visible = updateButton.Enabled = NewVersionAvailable;            
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

        private void updateButton_Click(object sender, EventArgs e) {
            var updaterApp = Path.Combine(RootDirectory, APP_UPDATER);
            var updatingApp = Path.Combine(RootDirectory, APP_UPDATING);

            // COPY TO NOT LOCK PROCESS ON UPDATE
            if (File.Exists(updaterApp)) {
                File.Copy(updaterApp, updatingApp, true);
            }                

            // START AUTO UPDATE
            GeneralExtensions.OpenExecutable(updatingApp);
            
            // EXIT TOOLKIT
            Application.Exit();
        }
    }
}
