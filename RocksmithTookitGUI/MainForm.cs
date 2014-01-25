using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using RocksmithToolkitGUI.Shortcut;

namespace RocksmithToolkitGUI
{
    public partial class MainForm : Form
    {
        private GlobalHotKey globalHotKeys;

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

            // Registering new shortcut keys
            try {
                globalHotKeys = new GlobalHotKey();
                globalHotKeys.Shortcuts.Add(new ShortcutKey() { Modifier = GlobalHotKey.CTRL + GlobalHotKey.SHIFT, Key = (int)Keys.O, HWnd = this.Handle }); // Open Package Template
                globalHotKeys.Shortcuts.Add(new ShortcutKey() { Modifier = GlobalHotKey.CTRL + GlobalHotKey.SHIFT, Key = (int)Keys.S, HWnd = this.Handle }); // Save Package Template
                globalHotKeys.Shortcuts.Add(new ShortcutKey() { Modifier = GlobalHotKey.CTRL + GlobalHotKey.SHIFT, Key = (int)Keys.I, HWnd = this.Handle }); // Import Package Template
                globalHotKeys.Shortcuts.Add(new ShortcutKey() { Modifier = GlobalHotKey.CTRL + GlobalHotKey.SHIFT, Key = (int)Keys.G, HWnd = this.Handle }); // Generate Package
                globalHotKeys.Shortcuts.Add(new ShortcutKey() { Modifier = GlobalHotKey.CTRL + GlobalHotKey.SHIFT, Key = (int)Keys.A, HWnd = this.Handle }); // Add Arrangement
                globalHotKeys.Shortcuts.Add(new ShortcutKey() { Modifier = GlobalHotKey.CTRL + GlobalHotKey.SHIFT, Key = (int)Keys.T, HWnd = this.Handle }); // Add Tone
                globalHotKeys.Register();
            } catch {
                // If not Windows environment, shortcuts will not work :(
            }
        }

        /// <summary>
        /// Windows process event
        /// </summary>
        /// <param name="message">Message</param>
        protected override void WndProc(ref Message message) {
            if (message.Msg == GlobalHotKey.WM_HOTKEY_MSG_ID)
                HandleHotkey(GlobalHotKey.GetKey(message.LParam));
            base.WndProc(ref message);
        }

        private void HandleHotkey(Keys hotKey) {
            switch (hotKey) {
                case Keys.O:
                    dlcPackageCreatorControl.dlcLoadButton_Click();
                    break;
                case Keys.S:
                    dlcPackageCreatorControl.dlcSaveButton_Click();
                    break;
                case Keys.I:
                    dlcPackageCreatorControl.dlcImportButton_Click();
                    break;
                case Keys.G:
                    dlcPackageCreatorControl.dlcGenerateButton_Click();
                    break;
                case Keys.A:
                    dlcPackageCreatorControl.arrangementAddButton_Click();
                    break;
                case Keys.T:
                    dlcPackageCreatorControl.toneAddButton_Click();
                    break;
                default:
                    break;
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            globalHotKeys.Unregister();
            Application.Exit();
        }
    }
}
