using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using RocksmithToolkitLib;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Globalization;

namespace RocksmithToolkitGUI
{
    public partial class MainForm : Form
    {
        internal BackgroundWorker bWorker;
        private ToolkitVersionOnline onlineVersion;

        public static bool IsInDesignMode
        {
            get
            {
                return Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1 || Debugger.IsAttached;
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            var ci = new CultureInfo("en-US");
            var thread = System.Threading.Thread.CurrentThread;
            Application.CurrentCulture = thread.CurrentCulture = thread.CurrentUICulture = ci;
            Application.CurrentInputLanguage = InputLanguage.FromCulture(ci);

            if (args.Length > 0 && File.Exists(args[0]))
                LoadTemplate(args[0]);

            this.Text = String.Format("Custom Song Creator Toolkit (v{0} beta)", ToolkitVersion.version);
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {// Disable updates for Mac (speedup) -1.5 secconds here
                updateButton.Enabled = false;
                updateButton.Text = "Updates Disabled";
                updateButton.Visible = true;
            }
            else
            {
                bWorker = new BackgroundWorker();
                bWorker.DoWork += CheckForUpdate;
                bWorker.RunWorkerCompleted += EnableUpdate;
                bWorker.RunWorkerAsync();
            }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        //TODO: keep tabs data please.
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Show this tab only by 'Configuration' click
            tabControl1.TabPages.Remove(GeneralConfigTab);

            // position main form at top center of screen to avoid having to reposition on low res displays
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, 0);
        }

        private void CheckForUpdate(object sender, DoWorkEventArgs e)
        {
            try
            {// CHECK FOR NEW AVAILABLE VERSION AND ENABLE UPDATE
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
            if (onlineVersion == null) return;
            if (ToolkitVersion.commit != "nongit")
                updateButton.Visible = updateButton.Enabled = onlineVersion.UpdateAvailable;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || !e.Shift) return;
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
            using (var u = new UpdateForm())
            {
                u.Init(onlineVersion);
                u.ShowDialog();
            }
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowConfigScreen();
        }

        private void ShowConfigScreen()
        {
            configurationToolStripMenuItem.Enabled = false;

            // Remove all tabs
            tabControl1.TabPages.Clear();

            // Add config
            if (!tabControl1.TabPages.Contains(GeneralConfigTab))
                tabControl1.TabPages.Add(GeneralConfigTab);
        }

        public void ReloadControls()
        {
            this.Controls.Clear();
            InitializeComponent();
            tabControl1.TabPages.Remove(GeneralConfigTab);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // cleanup temp folder garbage carefully
#if !DEBUG
            var di = new DirectoryInfo(Path.GetTempPath());
 
            // confirm this is the 'Local Settings\Temp' directory
            if (di.Parent != null)
                if (di.Parent.Name == "Local Settings" && di.Name == "Temp")
                {
                    foreach (FileInfo file in di.GetFiles())
                        file.Delete();

                    foreach (DirectoryInfo dir in di.GetDirectories())
                        dir.Delete(true);
                }
#endif
        }


    }
}
