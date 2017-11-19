using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

namespace RocksmithToolkitUpdater
{
    public static class Helpers
    {
        public static bool IsInDesignMode
        {
            get
            {
                var bVshostCheck = Process.GetCurrentProcess().ProcessName.IndexOf("vshost", StringComparison.OrdinalIgnoreCase) > -1 ? true : false;
                var bModeCheck = LicenseManager.UsageMode == LicenseUsageMode.Designtime ? true : false;
                var bDevEnvCheck = Application.ExecutablePath.IndexOf("devenv", StringComparison.OrdinalIgnoreCase) > -1 ? true : false;
                var bDebuggerAttached = Debugger.IsAttached;

                if (bDebuggerAttached || bDevEnvCheck || bModeCheck || bVshostCheck)
                    return true;

                return false;
            }
        }

        public static void InvokeIfRequired<T>(this T c, Action<T> action) where T : Control
        {
            if (c.InvokeRequired)
                c.Invoke(new Action(() => action(c)));
            else
                action(c);
        }

    }
}
