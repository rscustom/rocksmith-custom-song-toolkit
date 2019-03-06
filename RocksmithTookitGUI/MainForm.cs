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
using System.Threading;
using RocksmithToolkitGUI.Config;
//
// DEVNOTE: WHEN ISSUING NEW RELEASE VERION OF TOOLKIT ...
// Modify the RocksmithToolkitLib prebuild event which will update
// PatchAssemblyVersion.ps1 file '$AssemblyVersion' and '$AssemblyConfiguration' values 
//
namespace RocksmithToolkitGUI
{
    public partial class MainForm : Form
    {
        internal BackgroundWorker bWorker;
        private ToolkitVersionOnline onlineVersion;

        public MainForm(string[] args)
        {
            // load order is important
            InitializeComponent();

            var ci = new CultureInfo("en-US");
            var thread = Thread.CurrentThread;
            Application.CurrentCulture = thread.CurrentCulture = thread.CurrentUICulture = ci;
            //Application.CurrentInputLanguage = InputLanguage.FromCulture(ci); //may cause issues for non us cultures esp on wineMAC build got report of such issue.

            // it is better to be hidden initially and then unhide when needed
            if (GeneralExtension.IsInDesignMode)
                btnDevTestMethod.Visible = true;

            InitMainForm();
        }

        private void InitMainForm()
        {
            this.Text = String.Format("Song Creator Toolkit for Rocksmith (v{0})", ToolkitVersion.RSTKGuiVersion);

            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.BackColor = SystemColors.Control;
            btnUpdate.Text = "Updates are disabled";
            btnUpdate.Enabled = false;

            try
            {
                // updates are disabled on real Mac platform, or according to general_autoupdate setting                
                if (Environment.OSVersion.Platform != PlatformID.MacOSX &&
                    ConfigRepository.Instance().GetBoolean("general_autoupdate"))
                {
                    bWorker = new BackgroundWorker();
                    bWorker.DoWork += CheckForUpdate;
                    bWorker.RunWorkerCompleted += EnableUpdate;
                    bWorker.RunWorkerAsync();
                }

                // update current VersionInfo.txt file based on what's running
                ToolkitVersion.UpdateVersionInfoFile();

            }
            catch {/* DO NOTHING */}
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Show this tab only by 'Configuration' click
            tabControl1.TabPages.Remove(GeneralConfigTab);

            // position main form at top center of screen to avoid having to reposition on low res displays
            if ((Screen.PrimaryScreen.WorkingArea.Height - this.Height) > 0)
                this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            else
                this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, 0);

            // this.AutoScaleMode = AutoScaleMode.None; // preserve integrity of form dimensional layout
        }

        private void CheckForUpdate(object sender, DoWorkEventArgs e)
        {
            // CHECK FOR NEW AVAILABLE REVISION AND ENABLE UPDATE
            try
            {
                onlineVersion = ToolkitVersionOnline.GetVersionInfo();
            }
            catch (WebException) { /* Do nothing on 404 */ }
            catch (Exception)
            {
                throw;
            }
        }

        private void EnableUpdate(object sender, RunWorkerCompletedEventArgs e)
        {
            if (onlineVersion == null)
            {
                MessageBox.Show("Check Internet Connection ... ToolkitVersionOnline: null");
                return;
            }

            if (onlineVersion.UpdateAvailable)
            {
                btnUpdate.BackColor = Color.Orange;
                btnUpdate.FlatStyle = FlatStyle.Standard;
                btnUpdate.Text = "Click here to update";
            }
            else
            {
                btnUpdate.BackColor = Color.LightGreen;
                btnUpdate.FlatStyle = FlatStyle.Popup;
                btnUpdate.Text = "Updates are enabled";
            }

            btnUpdate.Enabled = true;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // hidden easter eggs ... commented out bad practice
            //if (!e.Control || !e.Shift) return;
            //switch (e.KeyCode)
            //{
            //    case Keys.O: //<< Load Template
            //        dlcPackageCreator1.btnTemplateLoad_Click();
            //        break;
            //    case Keys.S: //<< Save Template
            //        dlcPackageCreator1.SaveTemplateFile();
            //        break;
            //    case Keys.I: //<< Import Package
            //        dlcPackageCreator1.btnPackageImport_Click();
            //        break;
            //    case Keys.G: //<< Generate Package
            //        dlcPackageCreator1.btnPackageGenerate_Click();
            //        break;
            //    case Keys.A: //<< Add Arrangement
            //        dlcPackageCreator1.btnArrangementAdd_Click();
            //        break;
            //    case Keys.T: //<< Add Tone
            //        dlcPackageCreator1.btnToneAdd_Click();
            //        break;
            //}
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
            ShowHelpForm(); //Just show initial help form here!
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            btnUpdate.Enabled = false;

            using (var u = new UpdateForm())
            {
                u.Init(onlineVersion);
                u.ShowDialog();
            }

            btnUpdate.Enabled = true;
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowConfigScreen();
        }

        private void ShowConfigScreen()
        {
            configurationToolStripMenuItem.Enabled = false;

            // Save data
            //GeneralConfigTab.cachedTabs = tabControl1.TabPages;

            // Remove all tabs
            tabControl1.TabPages.Clear();

            // Add config tab
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
            //if (dlcPackageCreator1.IsDirty && ConfigRepository.Instance().GetBoolean("creator_autosavetemplate"))
            //    dlcPackageCreator1.SaveTemplateFile(dlcPackageCreator1.UnpackedDir, false);

            // leave temp folder contents for developer debugging
            if (GeneralExtension.IsInDesignMode)
                return;

            // cleanup temp folder garbage carefully
            // confirm this is the 'Local Settings\Temp' directory
            var di = new DirectoryInfo(Path.GetTempPath());
            if (di.Parent != null)
                return;

            if (di.Parent.Name == "Local Settings" && di.Name == "Temp")
            {
                foreach (FileInfo file in di.GetFiles())
                    try
                    {
                        file.Delete();
                    }
                    catch {/*Don't worry just skip locked file*/}

                foreach (DirectoryInfo dir in di.GetDirectories())
                    try
                    {
                        dir.Delete(true);
                    }
                    catch {/*Don't worry just skip locked directory*/}
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // don't bug the Developers when in design mode ;)
            if (GeneralExtension.IsInDesignMode)
                return;

            bool showRevNote = ConfigRepository.Instance().GetBoolean("general_showrevnote");
            if (showRevNote)
            {
                ShowHelpForm();
                ConfigRepository.Instance()["general_showrevnote"] = "false";
            }

            this.Refresh();

            // check for first run //Check if author set at least, then it's not a first run tho, but let it show msg anyways...
            bool firstRun = ConfigRepository.Instance().GetBoolean("general_firstrun");
            if (!firstRun)
                return;

            // validate display setting
            ConfigGlobals.Log.Info(" - System Display DPI Setting (" + GeneralExtension.GetDisplayDpi(this) + ")" + Environment.NewLine);
            ConfigGlobals.Log.Info(" - System Display Screen Scale Factor (" + GeneralExtension.GetDisplayScalingFactor(this) * 100 + "%)  " + Environment.NewLine);
            if (!GeneralExtension.ValidateDisplaySettings(this, this))
                ConfigGlobals.Log.Info(" - Adjusted AutoScaleDimensions, AutoScaleMode, and AutoSize ..." + Environment.NewLine);

            MessageBox.Show(new Form { TopMost = true },
                "Welcome to the Song Creator Toolkit for Rocksmith ..." + Environment.NewLine +
                "Commonly known as, 'the toolkit'." + Environment.NewLine + Environment.NewLine +
                "It looks like this may be your first time running the toolkit.  " + Environment.NewLine +
                "Please fill in the Configuration menu with your selections.",
                "Song Creator Toolkit for Rocksmith - FIRST RUN", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ShowConfigScreen();
            BringToFront();
        }

        private void ShowHelpForm()
        {
            using (var helpViewer = new HelpForm())
            {
                helpViewer.Text = String.Format("{0}", "Viewing ReleaseNotes.txt ...");
                // using ascii text file for web browser viewing
                helpViewer.PopulateAsciiText(File.ReadAllText("ReleaseNotes.txt"));
                helpViewer.ShowDialog();
            }
        }

        private void DevTestMethod()
        {
            // developer sandbox debugging area
            GeneralExtension.ValidateDisplaySettings(this, this, true, true);
            return;

            ShowHelpForm();
            return;

            var args = new string[]
            {
                "-u",
                "-input=D:\\Temp\\PeppaPig_p.psarc", 
                "-x", 
                "-d",
                "-f=Pc",
                "-v=RS2014",
                "-output=D:\\Temp",
                "-c"
            };

            var cmdArgs = String.Join(" ", args);
            var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var cliPath = Path.Combine(appDir, "packer.exe");

            if (File.Exists(cliPath))
                GeneralExtension.RunExternalExecutable(cliPath, arguments: cmdArgs);
            else
                MessageBox.Show("'Build, Rebuild Solution' while configuration is set to 'Debug w CLI'", "WRONG CONFIGURATION IS SELECTED ...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnDevTestMethod_Click(object sender, EventArgs e)
        {
            DevTestMethod();
        }

    }
}
