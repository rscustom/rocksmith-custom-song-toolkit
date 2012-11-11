using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RocksmithTookitGUI
{
    // http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/a07c453a-c5dd-40ed-8895-6615cc808d91
    public class CueTextBox : TextBox
    {
        private string mCue;
        public string Cue
        {
            get { return mCue; }
            set
            {
                mCue = value;
                UpdateCue();
            }
        }
        private void UpdateCue()
        {
            if (!IsHandleCreated || string.IsNullOrEmpty(mCue)) return;
            IntPtr mem = Marshal.StringToHGlobalUni(mCue);
            NativeMethods.SendMessage(Handle, 0x1501, (IntPtr)1, mem);
            Marshal.FreeHGlobal(mem);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateCue();
        }
    }
}
