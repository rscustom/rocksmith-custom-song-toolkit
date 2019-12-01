using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Security.Permissions;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib;
using RocksmithToolkitLib.XmlRepository;
using System.Threading;
using RocksmithToolkitGUI.Config;
using NLog;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace RocksmithToolkitGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            // make the logger available globally in application
            GlobalsConfig.Log = LogManager.GetCurrentClassLogger();
            // TODO: figure out way for native mac\linux OS
            var logPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "_RSToolkit_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");

            // verify external apps in 'tools' and 'ddc' directory
            ExternalApps.VerifyExternalApps(); // throws necessary exception if missing

            // workaround fix for Win10 NET4.6 compatiblity issue
            var updaterVersion = "Null";
            try
            {
                updaterVersion = ToolkitVersion.RSTKUpdaterVersion();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n\n" + e.ToString());
                /* DO NOTHING */
            }

            var assembly = Assembly.LoadFile(typeof(RocksmithToolkitLib.ToolkitVersion).Assembly.Location);
            var assemblyConfiguration = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).Cast<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration.ToString() ?? "";
            var dtuLib = DateTime.Parse(assemblyConfiguration, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            GlobalsConfig.Log.Info(//OSVersion on unix will return it's Kernel version, urgh.
                String.Format(" - RocksmithToolkitGUI: v{0}\r\n ", ToolkitVersion.RSTKGuiVersion) +
                String.Format(" - RocksmithToolkitLib: v{0} [{1}]\r\n ", ToolkitVersion.RSTKLibVersion(), dtuLib) +
                String.Format(" - RocksmithToolkitUpdater: v{0}\r\n ", updaterVersion) +
                String.Format(" - Dynamic Difficulty Creator: v{0}\r\n ", FileVersionInfo.GetVersionInfo(Path.Combine(ExternalApps.TOOLKIT_ROOT, ExternalApps.APP_DDC)).ProductVersion) +
                String.Format(" - OS: {0} ({1} bit)\r\n ", Environment.OSVersion, Environment.Is64BitOperatingSystem ? "64" : "32") +
                String.Format(" - .NET Framework Runtime: v{0}\r\n ", Environment.Version) +
                String.Format(" - CultureInfo: ({0}) \r\n ", CultureInfo.CurrentCulture.ToString()) +
                String.Format(" - Current Local DateTime: [{0}]\r\n ", DateTime.Now.ToString()) +
                String.Format(" - Current UTC DateTime: [{0}]\r\n ", DateTime.UtcNow.ToString()) +
                String.Format(" - JIT: {0}\r\n ", JitVersionInfo.GetJitVersion()) +
                String.Format(" - WINE_INSTALLED: {0}\r\n ", GeneralExtension.IsWine()) +
                String.Format(" - MacOSX: {0} ", Environment.OSVersion.Platform == PlatformID.MacOSX)
                );

            if (!Environment.Version.ToString().Contains("4.0.30319") &&
                ConfigRepository.Instance().GetBoolean("general_firstrun"))
            {
                var envMsg = "The toolkit runs best with .NET 4.0.30319 installed." + Environment.NewLine +
                    "You are currently running .NET " + Environment.Version.ToString() + Environment.NewLine +
                    "Install the correct version if you experinece problems running the toolkit.   " + Environment.NewLine + Environment.NewLine +
                    "Click 'Yes' to download and install the correct version now from:" + Environment.NewLine +
                    "https://www.microsoft.com/en-us/download/confirmation.aspx?id=17718";

                if (MessageBox.Show(envMsg, "Incorrect .NET Version ...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Process.Start("https://www.microsoft.com/en-us/download/confirmation.aspx?id=17718");
                    Thread.Sleep(500);
                    Process.Start("https://www.howtogeek.com/118869/how-to-easily-install-previous-versions-of-the-.net-framework-in-windows-8");

                    // Kill current toolkit process now that download process is started
                    Environment.Exit(0);
                }
            }

            // use custom event handlers
            if (!GeneralExtension.IsInDesignMode)
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    var exception = e.ExceptionObject as Exception;
                    GlobalsConfig.Log.Error(" - Unhandled.Exception:\n\nSource: {0}\nTarget: {1}\n{2}", exception.Source, exception.TargetSite, exception.ToString());

                    if (MessageBox.Show(String.Format("Unhandled.Exception:\n\n{0}\nPlease send us the {1} file if you need help.  Open log file now?", 
                        exception.Message.ToString(), Path.GetFileName(logPath)), 
                        "Please Read This Important Message Completely ...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        Process.Start(logPath);
                    }

                    GlobalExtension.HideProgress();
                };

                // UI thread exceptions handling.
                Application.ThreadException += (s, e) =>
                {
                    var exception = e.Exception;
                    var packerErrMsg = RocksmithToolkitLib.DLCPackage.Packer.ErrMsg.ToString();

                    if (String.IsNullOrEmpty(packerErrMsg))
                        GlobalsConfig.Log.Error(" - Application.ThreadException\n\nSource: {0}\nTarget: {1}\n{2}", exception.Source, exception.TargetSite, exception.ToString());
                    else
                        GlobalsConfig.Log.Error(" - Application.ThreadException\n\nSource: {0}\nTarget: {1}\n{2}\n\nPacker.ThreadException (Corrupt CDLC): {3}", exception.Source, exception.TargetSite, exception.ToString(), packerErrMsg.Trim());

                    if (exception.Message != null && exception.Message.Contains("expired"))
                    {
                        MessageBox.Show(String.Format("Activation.ThreadException:\n\n{0}", exception.Message), 
                            "Please Read This Important Message Completely ...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Application.Exit();
                    }
                    else
                    {
                        var exMessage = String.IsNullOrEmpty(packerErrMsg) ? exception.Message : String.Format("{0}\nPacker.ThreadException (Corrupt CDLC):\n{1}\n", exception.Message, packerErrMsg.Trim());
                        if (MessageBox.Show(String.Format("Application.ThreadException:\n\n{0}\n\nPlease send us the {1} file if you need help.  Open log file now?", exMessage, Path.GetFileName(logPath)), 
                            "Please Read This Important Message Completely ...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            Process.Start(logPath);
                        }
                    }

                    GlobalExtension.HideProgress();
                };
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }
    }
}
