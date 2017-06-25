using System;
using System.Drawing;
using System.Reflection;
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
            // EH keeps main form responsive/refreshed
            this.Shown += MainForm_Splash;

            var ci = new CultureInfo("en-US");
            var thread = System.Threading.Thread.CurrentThread;
            Application.CurrentCulture = thread.CurrentCulture = thread.CurrentUICulture = ci;
            Application.CurrentInputLanguage = InputLanguage.FromCulture(ci);

            if (args.Length > 0 && File.Exists(args[0]))
                LoadTemplate(args[0]);

            InitMainForm();
        }

        private void InitMainForm()
        {
            // NOTE: WHEN ISSUING NEW RELEASE VERION OF TOOLKIT
            // edit version number in AssemblyInfo.cs for GUI, Lib and Updater
            // edit the hard coded version number in PatchAssemblyVersion.ps1 used by AppVeyor
            // comment out as necessary when issuing new release version
            // this.Text = String.Format("Rocksmith Custom Song Toolkit (v{0} beta)", ToolkitVersion.version);
            this.Text = String.Format("Rocksmith Custom Song Toolkit (v{0})", ToolkitVersion.version);

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
            if ((Screen.PrimaryScreen.WorkingArea.Height - this.Height) > 0)
                this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            else
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
            // hidden easter eggs ...
            if (!e.Control || !e.Shift) return;
            switch (e.KeyCode)
            {
                case Keys.O: //<< Load Template
                    dlcPackageCreator1.btnTemplateLoad_Click();
                    break;
                case Keys.S: //<< Save Template
                    dlcPackageCreator1.SaveTemplateFile();
                    break;
                case Keys.I: //<< Import Package
                    dlcPackageCreator1.btnPackageImport_Click();
                    break;
                case Keys.G: //<< Generate Package
                    dlcPackageCreator1.btnPackageGenerate_Click();
                    break;
                case Keys.A: //<< Add Arrangement
                    dlcPackageCreator1.btnArrangementAdd_Click();
                    break;
                case Keys.T: //<< Add Tone
                    dlcPackageCreator1.btnToneAdd_Click();
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

            // reset to display the revision note
            ConfigRepository.Instance()["general_showrevnote"] = "true";
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
            // autosave the dlc.xml template on closing
            if (dlcPackageCreator1.IsDirty && ConfigRepository.Instance().GetBoolean("creator_autosavetemplate"))
                dlcPackageCreator1.SaveTemplateFile(dlcPackageCreator1.UnpackedDir, false);

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

        private void MainForm_Splash(object sender, EventArgs e)
        {
#if !DEBUG  // don't bug the Developers when in debug mode ;)
            bool showRevNote = ConfigRepository.Instance().GetBoolean("general_showrevnote");
            if (showRevNote)
            {
                if (this.Text.Contains("beta"))
                    ShowHelpForm();

                ConfigRepository.Instance()["general_showrevnote"] = "false";
            }

            this.Refresh();


            // check for first run //Check if author set at least, then it's not a first run tho, but let it show msg anyways...
            bool firstRun = ConfigRepository.Instance().GetBoolean("general_firstrun");
            if (!firstRun)
                return;
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

        private void ShowHelpForm()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream streamBetaInfo = assembly.GetManifestResourceStream("RocksmithToolkitGUI.Resources.BetaInfo.rtf"))
            {
                using (var helpViewer = new HelpForm())
                {
                    helpViewer.Text = String.Format("{0}", "TOOLKIT BETA RELEASE MESSAGE ...");
                    helpViewer.PopulateRichText(streamBetaInfo);
                    helpViewer.ShowDialog();
                }
            }
        }


    }
}
