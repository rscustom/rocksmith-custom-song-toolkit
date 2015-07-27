using System;
using System.Globalization;
using System.Windows.Forms;
using System.Security.Permissions;
using NLog;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI
{
    static class Program
    {
        /// <summary>
        /// Usage: RocksmithToolkitGUI.log.Error(«ERROR: {0}», this.Text);
        /// </summary>
        public static Logger Log;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            Log = LogManager.GetCurrentClassLogger();

            Log.Info(
                String.Format("Version: {0}\r\n ", RocksmithToolkitLib.ToolkitVersion.version) +
                String.Format("OS: {0}\r\n ", Environment.OSVersion) +
                String.Format("Command: {0}", Environment.CommandLine) +
                String.Format("Runtime Version: v{0}", Environment.Version) +
                String.Format("JIT: {0}", JitVersionInfo.GetJitVersion())
            );

            var ci = new CultureInfo("en-US");
            Application.CurrentCulture = ci;
            Application.CurrentInputLanguage = InputLanguage.FromCulture(ci);
            var thread = System.Threading.Thread.CurrentThread;
            thread.CurrentCulture = ci;
            thread.CurrentUICulture = ci;
            // can't figure out how disable this while debugging in IDE...
            // how about like this ...

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                MessageBox.Show(String.Format("Application.ThreadException\n{0}\n{1}\nPlease send us \"_RSToolkit_{2}.log\", you can find it in Toolkit folder.",
                    exception.ToString(), exception.Message.ToString(), DateTime.Now.ToString("yyyy-MM-dd")), "Unhandled Exception catched!");
                Log.ErrorException(String.Format("\n{0}\n{1}\nException catched:\n{2}\n", exception.Source, exception.TargetSite, exception.InnerException), exception);
            };

            // UI thread exceptions handling.
            Application.ThreadException += (s, e) =>
            {
                var exception = (Exception)e.Exception;
                MessageBox.Show(String.Format("Application.ThreadException\n{0}\n{1}\nPlease send us \"_RSToolkit_{2}.log\", you can find it in Toolkit folder.",
                    exception.ToString(),  exception.Message.ToString(), DateTime.Now.ToString("yyyy-MM-dd")), "Thread Exception catched!");
                Log.ErrorException(String.Format("\n{0}\n{1}\nException catched:\n{2}\n", exception.Source, exception.TargetSite, exception.InnerException), exception);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }
    }
}
