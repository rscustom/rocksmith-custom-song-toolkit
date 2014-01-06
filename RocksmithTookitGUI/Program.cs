using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NLog;
using System.Security.Permissions;

namespace RocksmithToolkitGUI
{
    static class Program
    {
        /// <summary>
        /// Usage: RocksmithToolkitGUI.log.Error(«ERROR: {0}», this.Text);
        /// </summary>
        public static Logger log;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            log = LogManager.GetCurrentClassLogger();

            log.Info("Version: {0}", RocksmithToolkitLib.ToolkitVersion.version);
            log.Info("OS: {0}", Environment.OSVersion.ToString());
            log.Info("Command: {0}", Environment.CommandLine.ToString());

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                MessageBox.Show(String.Format("Application.ThreadException\r\n{0}\r\n{1}", exception.ToString(),
                    "\r\nPLease send us \"_RSToolkit_XXXX.log\", you can fing it in Toolkit folder."), exception.Message.ToString());
                log.ErrorException(String.Format("\r\n{0}\r\n{1}\r\nException catched:\r\n", exception.Source, exception.TargetSite), exception);
            };

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // UI thread exceptions handling.
            Application.ThreadException += (s, e) =>
            {
                var exception = (Exception)e.Exception;
                MessageBox.Show(String.Format("Application.ThreadException\r\n{0}\r\n{1}", exception.ToString(),
                    "\r\nPLease send us \"_RSToolkit_XXXX.log\", you can fing it in Toolkit folder."), exception.Message.ToString());
                log.ErrorException(String.Format("\r\n{0}\r\n{1}\r\nException catched:\r\n", exception.Source, exception.TargetSite), exception);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Application.Run(new MainForm(args));
        }
    }
}
