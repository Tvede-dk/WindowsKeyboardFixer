using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsKeyboardFixer {


	public partial class InvisableForm : Form {


		public InvisableForm() {
			InitializeComponent();
			this.TransparencyKey = this.BackColor;
		}

		//public void removeOterLayouts(String selectedLang, String selectedLangHKL) {
		//	//windows 8 fucks up the otherwise simple code.
		//	IntPtr[] arra = new IntPtr[100];
		//	int count = GetKeyboardLayoutList(100 , arra);
		//	int i = 0;
		//	foreach ( var itm in arra ) {
		//		if ( i >= count ) {
		//			break;
		//		}
		//		int lcid = itm.ToInt32() & 0x0000ffff;
		//		if ( lcid != 0 ) {
		//			String inHex = lcid.ToString("X8");
		//			if ( inHex.Equals(selectedLang) ) {
		//				continue;
		//			}
		//			UnloadKeyboardLayout(itm);
		//		}
				
		//		i++;

		//	}
		//	IntPtr strPtr = Marshal.StringToHGlobalUni(selectedLangHKL);
		//	ActivateKeyboardLayout(strPtr , 0x00000100 | 0x00000008 | 0x40000000);
		//	Marshal.FreeHGlobal(strPtr);
		//}

		//public void setLayout( string layoutVal ) {
		//	//LoadKeyboardLayout(layoutVal , KLF_ACTIVATE);
		//}

		//public void selectLayout( string selectedLang ) {
		//	//setLayout(selectedLang);
		//}
	}
}
