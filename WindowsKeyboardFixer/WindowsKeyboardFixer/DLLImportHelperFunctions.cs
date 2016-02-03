using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsKeyboardFixer {
    /// <summary>
    /// helper functions based on the DLLImports, that are commenly used.
    /// </summary>
    public class DLLImportHelperFunctions {
        private const int nChars = 256;

        public static string GetActiveWindowTitle() {
            var Buff = new StringBuilder(nChars);
            var handle = DLLImports.GetForegroundWindow();
            if (DLLImports.GetWindowText(handle, Buff, nChars) > 0) {
                return Buff.ToString();
            }
            return null;
        }

        public static bool haveKeyboardChanged(uint hwnd, String selectedLang) {
            var itm = DLLImports.GetKeyboardLayout(hwnd);
            var inHex = itm.convertTo32BitHex();
            return inHex.Equals(selectedLang);
        }

    }
}
