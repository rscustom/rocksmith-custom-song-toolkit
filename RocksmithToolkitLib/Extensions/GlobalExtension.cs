using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RocksmithToolkitLib.Extensions
{
    public static class GlobalExtension
    {
        public static Label CurrentOperationLabel { get; set; }
        // TODO: implement error checking for values > 100
        public static ProgressBar UpdateProgress { get; set; }

        public static void HideProgress()
        {
            UpdateProgress.Visible = false;
            CurrentOperationLabel.Visible = false;
        }

        public static void ShowProgress(string currentOperation = "...", int progressValue = 0)
        {
            //if (progressValue > 100)
            //    progressValue = 100;
            UpdateProgress.Value = progressValue;
            CurrentOperationLabel.Text = currentOperation;
            UpdateProgress.Visible = true;
            CurrentOperationLabel.Visible = true;
            CurrentOperationLabel.Refresh();
        }


    }
}
