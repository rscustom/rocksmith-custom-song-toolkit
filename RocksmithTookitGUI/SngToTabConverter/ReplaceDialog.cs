using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.SngToTabConverter
{
    public partial class ReplaceDialog : Form
    {
        public enum ResultType
        {
            CANCEL,
            YES,
            YES_TO_ALL,
            NO,
            NO_TO_ALL
        }

        private ResultType _result;

        public ResultType Result
        {
            private set
            {
                _result = value;
            }

             get
            {
                return _result;
            }
        }

        public ReplaceDialog(string filename)
        {
            InitializeComponent();
            destinationFileName.Text = filename;
            Result = ResultType.CANCEL;
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            Result = ResultType.YES;
            Close();
        }

        private void yesToAllButton_Click(object sender, EventArgs e)
        {
            Result = ResultType.YES_TO_ALL;
            Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            Result = ResultType.NO;
            Close();
        }

        private void noToAllButton_Click(object sender, EventArgs e)
        {
            Result = ResultType.NO_TO_ALL;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Result = ResultType.CANCEL;
            Close();
        }
    }
}
