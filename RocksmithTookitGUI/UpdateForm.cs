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

namespace RocksmithToolkitGUI
{
    partial class UpdateForm : Form
    {
        private const string APP_UPDATER = "RocksmithToolkitUpdater.exe";
        private const string APP_UPDATING = "RocksmithToolkitUpdating.exe";

        private string RootDirectory {
            get {
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
            var updatingApp = Path.Combine(RootDirectory, APP_UPDATING);
            if (File.Exists(updatingApp))
                File.Delete(updatingApp);

            currentVersionLabel.Text = ToolkitVersion.version;
            newVersionLabel.Text = String.Format("{0}-{1}", onlineVersion.Version, onlineVersion.Revision);
            dateLabel.Text = onlineVersion.Date.ToShortDateString();

            if (onlineVersion.CommitMessages != null)
            {
                commitMessageDataGrid.Visible = true;
                commitMessageDataGrid.Rows.Clear();
                for (var i = 0; i < onlineVersion.CommitMessages.Length; i++)
                {
                    commitMessageDataGrid.Rows.Add();
                    commitMessageDataGrid.Rows[i].Cells["Message"].Value = onlineVersion.CommitMessages[i];
                }
            }
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            var updaterApp = Path.Combine(RootDirectory, APP_UPDATER);
            var updatingApp = Path.Combine(RootDirectory, APP_UPDATING);

            // COPY TO NOT LOCK PROCESS ON UPDATE
            if (File.Exists(updaterApp))
                File.Copy(updaterApp, updatingApp, true);
            else
            {
                var errMsg = "Can not find " + APP_UPDATER + Environment.NewLine + "Please reinstall/update the toolkit manually.";
                BetterDialog2.ShowDialog(errMsg, "Toolkit Updater Error", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Error.Handle), "Error", 150, 150);
                return;
            }

            // START AUTO UPDATE
            GeneralExtensions.RunExternalExecutable(updatingApp);
            
            // EXIT TOOLKIT
            Application.Exit();
        }
    }
}
