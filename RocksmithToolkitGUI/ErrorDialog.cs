using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitGUI
{
    internal static class ErrorDialog
    {
        public static void Show(string errorMessage, Exception exception)
        {
            var exceptionMessage = exception.Message;
            if (exception.InnerException != null && !string.IsNullOrEmpty(exception.InnerException.Message))
                exceptionMessage += "\n" + exception.InnerException.Message;

            PSTaskDialog.cTaskDialog.MessageBox("Error", errorMessage, exceptionMessage, exception.ToString(),
                "Click 'show details' for complete exception information.", "",
                PSTaskDialog.eTaskDialogButtons.OK, PSTaskDialog.eSysIcons.Error, PSTaskDialog.eSysIcons.Information);
        }
    }
}
