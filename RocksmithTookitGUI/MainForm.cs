using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using RocksmithToolkitLib;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Globalization;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;

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
            this.Shown += MainForm_Shown;

            var ci = new CultureInfo("en-US");
            var thread = System.Threading.Thread.CurrentThread;
            Application.CurrentCulture = thread.CurrentCulture = thread.CurrentUICulture = ci;
            Application.CurrentInputLanguage = InputLanguage.FromCulture(ci);

            if (args.Length > 0 && File.Exists(args[0]))
                LoadTemplate(args[0]);

            // get the user's attention that this is a special beta release
            var errMsg = "This is a special beta release of the toolkit so there could be some bugs." + Environment.NewLine +
                          "Reverted parseArrangements method in Sng2014FileWriter to earlier version for testing." + Environment.NewLine +
                          "Revised mastery to make it more difficult to obtain 100%." + Environment.NewLine +
                          "Revised Techniques and Codes Section of JSON Manifest output." + Environment.NewLine + Environment.NewLine +
                          "Please let the toolkit devs know if experience any in game issues or not as a result." + Environment.NewLine;
            BetterDialog2.ShowDialog(errMsg, "SPECIAL TOOLKIT BETA RELEASE MESSAGE ... 100% BUG ISSUES #3", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Information.Handle), "Information", 150, 150);

            InitMainForm();
        }

        private void InitMainForm()
        {
            // comment out as necessary when issuing new release version
            // update (remove beta) from AssemblyInfo.cs in GUI, Lib and Updater
            this.Text = String.Format("Rocksmith Custom Song Toolkit (v{0} beta) 100% BUG ISSUES #3", ToolkitVersion.version);
            //this.Text = String.Format("Rocksmith Custom Song Toolkit (v{0})", ToolkitVersion.version);

            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {// Disable updates for Mac (speedup) -1.5 seconds here
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
                    dlcPackageCreator1.dlcLoadButton_Click();
                    break;
                case Keys.S: //<< Save Template
                    dlcPackageCreator1.SaveTemplateFile();
                    break;
                case Keys.I: //<< Import Template
                    dlcPackageCreator1.dlcImportButton_Click();
                    break;
                case Keys.G: //<< Generate Package
                    dlcPackageCreator1.dlcGenerateButton_Click();
                    break;
                case Keys.A: //<< Add Arrangement
                    dlcPackageCreator1.arrangementAddButton_Click();
                    break;
                case Keys.T: //<< Add Tone
                    dlcPackageCreator1.toneAddButton_Click();
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

            // Save Data
            //GeneralConfigTab.cachedTabs = tabControl1.TabPages;

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
            InitMainForm();
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
                        try
                        {
                            file.Delete();
                        }
                        catch { /*Don't worry just skip locked file*/ }

                    foreach (DirectoryInfo dir in di.GetDirectories())
                        try
                        {
                            dir.Delete(true);
                        }
                        catch { /*Don't worry just skip locked directory*/ }
                }
#endif
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.Refresh();

            // don't bug the Developers when in debug mode ;)
#if !DEBUG
            // check for first run //Check if author set at least, then it's not a first run tho, but let it show msg anyways...
            bool firstRun = ConfigRepository.Instance().GetBoolean("general_firstrun");
            if (!firstRun) return;
            MessageBox.Show(new Form { TopMost = true },
                "    Welcome to the Song Creator Toolkit for Rocksmith." + Environment.NewLine +
                "          Commonly known as, 'the toolkit'." + Environment.NewLine + Environment.NewLine +
                "It looks like this may be your first time running the toolkit.  " + Environment.NewLine +
                "  Please fill in the Configuration menu with your selections.", "Song Creator Toolkit for Rocksmith ... First Run",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ShowConfigScreen();
            BringToFront();
#endif

        }

    }
}
