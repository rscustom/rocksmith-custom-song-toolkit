using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace RocksmithToolkitGUI
{
    public sealed class CueTextBox : TextBox
    {
        // adds transparency color option
        //public CueTextBox()
        //{
        //    SetStyle(ControlStyles.SupportsTransparentBackColor |
        //        ControlStyles.OptimizedDoubleBuffer |
        //        ControlStyles.AllPaintingInWmPaint |
        //        ControlStyles.ResizeRedraw, true);
        //    // commented out prevents cue text from showing
        //    // || ControlStyles.UserPaint, true); 
        //    BackColor = Color.White;
        //}

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (this.UsePrompt)
            {
                this.UsePrompt = false;
                this.Text = string.Empty;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (this.TextLength == 0 || this.Text == this.Cue)
            {
                this.UsePrompt = true;
                this.Text = this.Cue;
            }
            base.OnLostFocus(e);
        }

        private string cue = "Any banner message here";
        public string Cue
        {
            get { return cue; }
            set
            {
                cue = value;
                if (this.UsePrompt && !string.IsNullOrEmpty(this.cue))
                {
                    this.Text = value;
                }
            }
        }

        private bool usePrompt = true;
        private bool UsePrompt
        {
            get { return usePrompt; }
            set
            {
                usePrompt = value;
                if (usePrompt)
                {
                    this.Font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular);
                    this.ForeColor = Color.Gray;
                }
                else
                {
                    // TODO: don't hardcode the user given values.
                    this.Font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular);
                    this.ForeColor = Color.Black;
                }
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.UsePrompt = true;
                this.Text = this.Cue;
            }
            base.OnParentChanged(e);
        }

        public override string Text
        {
            get
            {
                if (this.UsePrompt)
                {
                    return string.Empty;
                }
                return base.Text;
            }
            set
            {
                if (this.UsePrompt && (!string.IsNullOrEmpty(value) && value != this.Cue))
                {
                    this.UsePrompt = false;
                }

                if (string.IsNullOrEmpty(value) && !this.Focused && !string.IsNullOrEmpty(this.cue))
                {
                    this.UsePrompt = true;
                    this.Text = this.Cue;
                    return;
                }

                base.Text = value;
            }
        }
    }
}
