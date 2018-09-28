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
            GlobalExtension.Log = LogManager.GetCurrentClassLogger();
            // TODO: figure out way for native mac\linux OS
            var logPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "_RSToolkit_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");

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

            GlobalExtension.Log.Info(//OSVersion on unix will return it's Kernel version, urgh.
                String.Format("RocksmithToolkitGUI: v{0}\r\n ", ToolkitVersion.RSTKGuiVersion) +
                String.Format("RocksmithToolkitLib: v{0}\r\n ", ToolkitVersion.RSTKLibVersion()) +
                String.Format("RocksmithToolkitUpdater: v{0}\r\n ", updaterVersion) +
                String.Format("Dynamic Difficulty Creator: v{0}\r\n ", FileVersionInfo.GetVersionInfo(Path.Combine(ExternalApps.TOOLKIT_ROOT, ExternalApps.APP_DDC)).ProductVersion) +
                String.Format("OS: {0} ({1} bit)\r\n ", Environment.OSVersion, Environment.Is64BitOperatingSystem ? "64" : "32") +
                String.Format(".NET Framework Runtime: v{0}\r\n ", Environment.Version) +
                String.Format("JIT: {0}\r\n ", JitVersionInfo.GetJitVersion()) +
                String.Format("Wine: {0}", GeneralExtensions._wine())
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

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = e.ExceptionObject as Exception;
                GlobalExtension.Log.Error(exception, "\n{0}\n{1}\nException cached:\n{2}\n\n", exception.Source, exception.TargetSite, exception.InnerException);
                //Log.Error("Application Stdout:\n\n{0}", new StreamReader(_stdout.ToString()).ReadToEnd());

                if (MessageBox.Show(String.Format("Application.ThreadException met.\n\n\"{0}\"\n\n{1}\n\nPlease send us \"{2}\", open log file now?",
                    exception.ToString(), exception.Message.ToString(), Path.GetFileName(logPath)), "Unhandled Exception", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //figure out how to call it single time
                    //Process.Start("explorer.exe", string.Format("/select,\"{0}\"", logPath));
                    Process.Start(logPath);
                }
                //Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true }); //write back to Stdout in console could use custom streamwriter so you could write to console from there
            };

            // UI thread exceptions handling.
            Application.ThreadException += (s, e) =>
            {
                var exception = e.Exception;
                GlobalExtension.Log.Error(exception, "\n{0}\n{1}\nException cached:\n{2}\n\n", exception.Source, exception.TargetSite, exception.InnerException);
                //Log.Error("Application Stdout:\n\n{0}", new StreamReader(_stdout.ToString()).ReadToEnd());

                if (MessageBox.Show(String.Format("Application.ThreadException met.\n\n\"{0}\"\n\n{1}\n\nPlease send us \"{2}\", open log file now?",
                    exception.ToString(), exception.Message.ToString(), Path.GetFileName(logPath)), "Thread Exception", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //Process.Start("explorer.exe", string.Format("/select,\"{0}\"", logPath));
                    Process.Start(logPath);
                }
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }
    }
}
