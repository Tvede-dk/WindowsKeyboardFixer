using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsKeyboardFixer {
    public class AppSettings {
        public int selectedLangLCID
        {
            get {
                return Properties.Settings.Default.selectedLangLCID;
            }
            set {
                Properties.Settings.Default.selectedLangLCID = value;
                Properties.Settings.Default.Save();
            }
        }



    }
}
