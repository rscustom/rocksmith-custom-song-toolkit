using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RocksmithToolkitLib.Extensions
{
    public static class GlobalExtension
    {
        private static Label _currentOperationLabel;
        public static Label CurrentOperationLabel
        {
            get { return _currentOperationLabel ?? (_currentOperationLabel = new Label()); }
            set { _currentOperationLabel = value; }
        }

        // TODO: confirm error checking for > 100 works
        private static ProgressBar _updateProgress;
        /// <summary>
        /// Place a progress bar (updateProgress)
        /// and a label (currentOperationLabel) on the form or user control
        /// 
        /// Declare class object getter/setter:        
        /// public static ProgressBar UpdateProgress { get; set; }
        /// public static Label CurrentOperationLabel { get; set; }
        /// 
        /// In the class declaration:  
        /// InitializeComponent();
        /// GlobalExtension.UpdateProgress = this.updateProgress;
        /// GlobalExtension.CurrentOperationLabel = this.currentOperationLabel;
        /// Thread.Sleep(100); 
        /// 
        /// Usage example: GlobalExtension.ShowProgress("Packing archive ...");
        /// </summary>
        public static ProgressBar UpdateProgress
        {
            get { return _updateProgress ?? (_updateProgress = new ProgressBar()); }
            set
            {
                if (value.Value > 100)
                    _updateProgress.Value = 100;
                else
                    _updateProgress = value;
            }
        }

        public static void HideProgress()
        {
            UpdateProgress.Visible = false;
            CurrentOperationLabel.Visible = false;
        }

        public static void ShowProgress(string currentOperation = "...", int progressValue = 0)
        {
            // getter/setter checks this so should not need here
            // if (progressValue > 100)
            //    progressValue = 100;
            
            UpdateProgress.Visible = true;
            CurrentOperationLabel.Visible = true;
            UpdateProgress.Value = progressValue;
            CurrentOperationLabel.Text = currentOperation;
            UpdateProgress.Refresh();
            CurrentOperationLabel.Refresh();
        }

        public static void Dispose()
        {
            HideProgress();
            // do not use dispose here!
            _updateProgress = null;
            _currentOperationLabel = null;
        }

    }
}
