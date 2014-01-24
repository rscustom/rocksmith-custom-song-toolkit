using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RocksmithToolkitGUI.Shortcut {
    public class ShortcutKey {
        #region PROPERTIES

        public IntPtr HWnd { get; set; }
        public int Id {
            get {
                return GetHashCode();
            }
        }
        public int Modifier { get; set; }
        public int Key { get; set; }

        #endregion

        #region METHODS

        public override int GetHashCode() {
            return Modifier ^ Key ^ HWnd.ToInt32();
        }

        #endregion
    }
}
