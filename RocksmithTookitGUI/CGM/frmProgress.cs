using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.CGM
{
    public partial class frmProgress : Form
    {
        public frmProgress()
        {
            InitializeComponent();
        }

        public void RunPbar(int miliSec)
        {
            // future ... not working yet ... probably need background worker
            int i;
            int iSleep = miliSec/100;
            lblFooter.Text = "";
            Visible = true;
            Refresh();

            for (i = 0; i <= 100; i++)
            {
                progBar.Value = i;
                // Display the textual value of the ProgressBar in the forms footer text.
                lblFooter.Text = progBar.Value.ToString() + "% Completed";
                Thread.Sleep(iSleep);
            }
            lblFooter.Text = "Done ...";
            Refresh();
            Thread.Sleep(400);
            Visible = false;
        }

        public void PbarHeader(string strHeader)
        {
            lblHeader.Text = strHeader;
            Visible = true;
        }

        public void PbarFooter(string strFooter)
        {
            lblFooter.Text = strFooter;
            Visible = true;
        }

        public void PbarValue(int iValue)
        {
            progBar.Value = iValue;
            Visible = true;
            Refresh();
            if (iValue == 100)
            {
                Console.Beep();
                Thread.Sleep(1500);
                Visible = false;
            }
        }
    }
}