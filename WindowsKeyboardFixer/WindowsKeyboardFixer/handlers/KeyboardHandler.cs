using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace WindowsKeyboardFixer {

    public class KeyboardHandler {

        //some constants can be found at : https://msdn.microsoft.com/en-us/library/windows/desktop/ms646289(v=vs.85).aspx

        #region constants

        const uint WH_CALLWNDPROC = 0x04;
        const int WM_INPUTLANGCHANGE = 0x0051;

        const uint KLF_REPLACELANG = 0x00000010;
        /// <summary>
        /// Activates the specified locale identifier for the entire process,
        /// and sends the WM_INPUTLANGCHANGE message to the current thread's focus or active window.
        /// </summary>
        const uint KLF_SETFORPROCESS = 0x00000100;

        const uint KLF_ACTIVATE = 1; //activate the layout
        /// <summary>
        /// If this bit is set, the system's circular list of loaded locale identifiers is reordered by moving the locale identifier to the head of the list.
        /// If this bit is not set, the list is rotated without a change of order.
        /// </summary>
        const uint KLF_REORDER = 0x00000008;
        const uint KLF_RESET = 0x40000000; //controls if using shift can change caps lock (requires KLF_SHIFTLOCK to function that way) 
        const uint KLF_SUBSTITUTE_OK = 0x00000002;
        const uint KLF_NOTELLSHELL = 0x00000080;
        /// <summary>
        /// a wrapper arond the way we set a new layout.
        /// </summary>
        const int KLF_ACTIVATE_KEYBOARD_LAYOUT = (int)(KLF_SETFORPROCESS | KLF_REORDER | KLF_RESET);

        const int KL_NAMELENGTH = 9; // length of the keyboard buffer



        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        #endregion


        #region variables
        private string selectedLang = "";
        private string selectedLangHKL = "";

        private IntPtr keyboardHook = IntPtr.Zero;
        private IntPtr m_hhook;
        private DLLImports.WinEventDelegate dele;
        /// <summary>
        /// where we are going to do a lot of magic.
        /// </summary>
        private InvisableForm handlerForm = new InvisableForm();

        public bool IsListening
        {
            get
            {
                return m_hhook != IntPtr.Zero;
            }
        }


        #endregion

        /// <summary>
        /// registers the listener
        /// </summary>
        /// <param name="info"></param>
        public void Register(CultureInfo info) {
            if (!IsListening && info != null ) {
                //store the selection.
                selectedLangHKL = info.LCID.ToString();
                selectedLang = info.LCID.ToString("X8");
                removeOtherLayouts();
                enableHook();
            }
        }
        /// <summary>
        /// removes the listener from the system
        /// </summary>
        public void UnRegister() {
            if (IsListening) {
                removeHook();
            }
        }

        /// <summary>
        /// stops listening.
        /// </summary>
        private void removeHook() {
            DLLImports.UnhookWinEvent(m_hhook);
            m_hhook = IntPtr.Zero;
            dele = null;
        }

        /// <summary>
        /// start listening 
        /// </summary>
        private void enableHook() {
            dele = new DLLImports.WinEventDelegate(WinEventProc);
            m_hhook = DLLImports.SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        /// <summary>
        /// the windows callback handle. 
        /// </summary>
        /// <param name="hoohMethod"></param>
        /// <param name="eventType"></param>
        /// <param name="hwnd"></param>
        /// <param name="idObject"></param>
        /// <param name="idChild"></param>
        /// <param name="dwEventThread"></param>
        /// <param name="dwmsEventTime"></param>
        public void WinEventProc(IntPtr hoohMethod, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
            if (!haveKeyboardChanged(dwEventThread)) {
                var itm = DLLImports.GetKeyboardLayout(dwEventThread);
                var inHex = itm.convertTo32BitHex();
                DLLImports.UnhookWinEvent(m_hhook);

                //well do magic, so we are allowed to perform the changes requried.
                handlerForm.Show();

                //Show();
                DLLImports.SetForegroundWindow(handlerForm.Handle);
                removeOtherLayouts();
                LoadKeyboardLayout();
                // and lets not disturb the user. so he / she wont see what just happend.
                handlerForm.Hide();
                DLLImports.SetForegroundWindow(hwnd);
                enableHook();
            }
        }

        /// <summary>
        /// select the keyboard layout
        /// </summary>
        private void LoadKeyboardLayout() {
            DLLImports.LoadKeyboardLayout(selectedLang, KLF_ACTIVATE);
        }

        /// <summary>
        /// have it changed ?? 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns>true if it have changed from the selcetd one.</returns>
        private bool haveKeyboardChanged(uint hwnd) {
            return DLLImportHelperFunctions.haveKeyboardChanged(hwnd, selectedLang);
        }

        /// <summary>
        /// removes all layout that are not the selected layoyt.
        /// </summary>
        private void removeOtherLayouts() {
            //windows 8 fucks up the otherwise simple code.
            IntPtr[] arra = new IntPtr[100];
            int count = DLLImports.GetKeyboardLayoutList(100, arra); //assume the user have less than a 100 installed layouts..
            int i = 0;
            foreach (var itm in arra) {
                if (i >= count) {
                    break;
                }
                var hex = itm.convertTo32BitHex();
                if (hex?.Equals(selectedLang) ?? false) {
                    continue;
                }
                DLLImports.UnloadKeyboardLayout(itm);
                i++;

            }
            IntPtr strPtr = Marshal.StringToHGlobalUni(selectedLangHKL);
            DLLImports.ActivateKeyboardLayout(strPtr, KLF_ACTIVATE_KEYBOARD_LAYOUT);
            Marshal.FreeHGlobal(strPtr);
        }

        /// <summary>
        /// get the current keyboard layout name
        /// </summary>
        /// <returns></returns>
        public static string getKeyboardLayoutName() {
            var name = new StringBuilder(KL_NAMELENGTH);
            return DLLImports.GetKeyboardLayoutName(name).ToString();
        }

    }
}
