using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using RocksmithToolkitLib;
using System.IO;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitUpdater;
using System.Diagnostics;
using RocksmithToolkitLib.XmlRepository;
using System.Threading.Tasks;

namespace RocksmithToolkitGUI
{
    partial class UpdateForm : Form
    {
        // TODO: users may need to add both files to AV whitelist to run updater
        private const string APP_UPDATER = "RocksmithToolkitUpdater.exe";
        private const string APP_UPDATING = "RocksmithToolkitUpdating.exe";

        private string localToolkitDir
        {
            get
            {
                return Path.GetDirectoryName(Application.ExecutablePath);
            }
        }

        public UpdateForm()
        {
            InitializeComponent();
        }

        public void Init(ToolkitVersionOnline onlineVersion)
        {
            // DELETE OLD UPDATER APP IF EXISTS
            var updatingAppPath = Path.Combine(localToolkitDir, APP_UPDATING);
            if (File.Exists(updatingAppPath))
                File.Delete(updatingAppPath);

            var useBeta = ConfigRepository.Instance().GetBoolean("general_usebeta");
            lblCurrentVersion.Text = ToolkitVersion.RSTKGuiVersion;
            lblNewVersion.Text = String.Format("{0}-{1} {2}", onlineVersion.Version, onlineVersion.Revision, useBeta ? "BETA" : "");
            lblNewVersionDate.Text = onlineVersion.Date.ToShortDateString();

            if (onlineVersion.CommitMessages != null)
            {
                dgvCommitMessage.Visible = true;
                dgvCommitMessage.Rows.Clear();
                for (var i = 0; i < onlineVersion.CommitMessages.Length; i++)
                {
                    dgvCommitMessage.Rows.Add();
                    dgvCommitMessage.Rows[i].Cells["Message"].Value = onlineVersion.CommitMessages[i];
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            btnInstall.Enabled = false;
            // reset to display the revision note on next restart
            ConfigRepository.Instance()["general_showrevnote"] = "true";

            var tempToolkitDir = Path.Combine(Path.GetTempPath(), "RocksmithToolkit");
            var updaterAppPath = Path.Combine(localToolkitDir, APP_UPDATER);
            var updatingAppPath = Path.Combine(tempToolkitDir, APP_UPDATING);

            if (!File.Exists(updaterAppPath))
            {
                var errMsg = "Could not find file: " + APP_UPDATER + Environment.NewLine + "Please reinstall the toolkit manually.";
                BetterDialog2.ShowDialog(errMsg, "Toolkit Updater Error", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                return;
            }

            // create temp process backup folder 
            if (Directory.Exists(tempToolkitDir))
                Directory.Delete(tempToolkitDir, true);

            Directory.CreateDirectory(tempToolkitDir);

            try
            {
                // make a copy of AutoUpdater to prevent locking the process during update
                File.Copy(updaterAppPath, updatingAppPath, true);

                // normal operation
                if (!GeneralExtensions.IsInDesignMode)
                {
                    // passing args for process and backup directories to RocksmithToolkitUpdating.exe (Primary Usage Mode)
                    var cmdArgs = String.Format("\"{0}\" \"{1}\"", localToolkitDir, tempToolkitDir);

                    try // different AutoUpdater shells for MacWine testing
                    {
                        GeneralExtensions.RunExternalExecutable(updatingAppPath, arguments: cmdArgs);
                    }
                    catch (Exception ex)
                    {
                        // changed external process call for MacWine debugging
                        MessageBox.Show("Report hit updater try #2 to the developers: " + Environment.NewLine + ex.Message);

                        var startInfo = new ProcessStartInfo
                        {
                            FileName = updatingAppPath,
                            Arguments = cmdArgs,
                            UseShellExecute = false,
                            CreateNoWindow = true, // hide command window
                        };

                        using (var updater = new Process())
                        {
                            updater.StartInfo = startInfo;
                            updater.Start();
                        }
                    }

                    // Kill current toolkit process now that AutoUpdater process is started
                    Environment.Exit(0);
                }
                else // allow updater to be run in design mode for developers
                {
                    var args = new string[] { localToolkitDir, tempToolkitDir };
                    using (var autoUpdater = new AutoUpdaterForm(args))
                        autoUpdater.Show();
                }
            }
            catch (ObjectDisposedException)
            {
                /* Do nothing  - user cancelled the download */
            }
            catch (Exception ex)
            {
                var errMsg = "Could not run file: " + APP_UPDATING + Environment.NewLine +
                    "Please reinstall the toolkit manually." + Environment.NewLine +
                    ex.Message;
                BetterDialog2.ShowDialog(errMsg, "Toolkit Updater Error", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
            }

            btnInstall.Enabled = true;
        }

    }
}
