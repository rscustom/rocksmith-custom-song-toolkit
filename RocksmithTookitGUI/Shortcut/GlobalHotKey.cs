using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RocksmithToolkitGUI.Shortcut {
    public class GlobalHotKey {
        #region CONSTANTS

        // Modifiers
        public const int NOMOD = 0x0000;
        public const int ALT = 0x0001;
        public const int CTRL = 0x0002;
        public const int SHIFT = 0x0004;
        public const int WIN = 0x0008;

        // Windows message id for keyboard key
        public const int WM_HOTKEY_MSG_ID = 0x0312;

        #endregion

        #region PRIVATES

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private List<ShortcutKey> _shortcuts;

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Shortcuts
        /// </summary>
        public List<ShortcutKey> Shortcuts {
            get {
                if (_shortcuts == null)
                    _shortcuts = new List<ShortcutKey>();
                return _shortcuts;
            }
        }

        /// <summary>
        /// Register global hotkey in Windows environment
        /// </summary>
        public bool Register() {
            bool isReg = true;
            foreach (ShortcutKey sk in Shortcuts) {
                if (!RegisterHotKey(sk.HWnd, sk.Id, sk.Modifier, sk.Key)) {
                    isReg = false;
                }
            }
            return isReg; 
        }

        /// <summary>
        /// Delete global hotkey in Windows environment
        /// </summary>
        public bool Unregister() {
            bool isReg = true;
            foreach (ShortcutKey sk in Shortcuts) {
                if (!UnregisterHotKey(sk.HWnd, sk.Id)) {
                    isReg = false;
                }
            }
            return isReg; 
        }

        /// <summary>
        /// Get key
        /// </summary>
        public static Keys GetKey(IntPtr LParam) {
            return (Keys)(LParam.ToInt32() >> 16);
        }

        #endregion
    }
}
